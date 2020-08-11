using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Data;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.ExternalContract;
using OK_OnBoarding.Helpers;
using OK_OnBoarding.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public class StoreOwnerService : IStoreOwnerService
    {
        private readonly DataContext _dataContext;
        private readonly JwtSettings _jwtSettings;
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IOTPService _otpService;
        private readonly IMapper _mapper;
        private readonly TermiiAuthSettings _termiiAuthSettings;
        private readonly AppSettings _appSettings;

        public StoreOwnerService(DataContext dataContext, JwtSettings jwtSettings, IFacebookAuthService facebookAuthService, IOTPService otpService, IMapper mapper, TermiiAuthSettings termiiAuthSettings, AppSettings appSettings)
        {
            _dataContext = dataContext;
            _jwtSettings = jwtSettings;
            _facebookAuthService = facebookAuthService;
            _otpService = otpService;
            _mapper = mapper;
            _termiiAuthSettings = termiiAuthSettings;
            _appSettings = appSettings;
        }

        public async Task<GenericResponse> ResetPassword(ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.OTP))
                return new GenericResponse { Status = false, Message = "Email and OTP cannot be empty." };

            var storeOwnerExist = await _dataContext.StoreOwners.FirstOrDefaultAsync(d => d.Email == request.Email);
            if (storeOwnerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid StoreOwner Email." };

            var otpResponse = await _otpService.VerifyOTPForStoreOwner(request.OTP, storeOwnerExist.PhoneNumber);
            if (!otpResponse.Status)
                return new GenericResponse { Status = false, Message = otpResponse.Message };

            byte[] passwordHash, passwordSalt;
            try
            {
                Security.CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }

            storeOwnerExist.PasswordHash = passwordHash;
            storeOwnerExist.PasswordSalt = passwordSalt;
            _dataContext.Entry(storeOwnerExist).State = EntityState.Modified;
            var updated = 0;
            try
            {
                updated = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (updated <= 0)
                return new GenericResponse { Status = false, Message = "Error Occurred." };

            return new GenericResponse { Status = true, Message = "Password Changed." };
        }

        public async Task<AuthenticationResponse> CreateStoreOwnerAsync(StoreOwner storeOwner, string password)
        {
            if(string.IsNullOrWhiteSpace(storeOwner.FirstName) || string.IsNullOrWhiteSpace(storeOwner.LastName) || string.IsNullOrWhiteSpace(storeOwner.Email))
                return new AuthenticationResponse { Errors = new[] { "FirstName, LastName and Email cannot be empty" } };

            var storeOwnerExist = await _dataContext.StoreOwners.FirstOrDefaultAsync(s => s.Email == storeOwner.Email || s.PhoneNumber == storeOwner.PhoneNumber);

            if(storeOwnerExist != null)
            {
                if(!storeOwnerExist.IsVerified)
                    return new AuthenticationResponse { Errors = new[] { "Store Owner already exist but not verified." } };

                return new AuthenticationResponse { Errors = new[] { "Store Owner with this email and phonenumber already exists." } };
            }

            byte[] passwordHash, passwordSalt;
            try
            {
                Security.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            }
            catch(Exception)
            {
                return new AuthenticationResponse { Errors = new[] { "Error Occurred." } };
            }

            storeOwner.PasswordHash = passwordHash;
            storeOwner.PasswordSalt = passwordSalt;
            storeOwner.DateRegistered = DateTime.Now;
            storeOwner.IsOneKioskContractAccepted = true;

            await _dataContext.StoreOwners.AddAsync(storeOwner);
            var created = await _dataContext.SaveChangesAsync();
            if(created <= 0)
                return new AuthenticationResponse { Errors = new[] { "Failed to register store owner." } };

            var genericResponse = await _otpService.GenerateOTPForStoreOwner(OTPGenerationReason.OTPGENERATION_FOR_SIGN_UP.ToString(), storeOwner.PhoneNumber, storeOwner.Email);
            if (genericResponse.Status)
            {
                // Send Sms
                var termiiRequest = new TermiiRequest()
                {
                    To = storeOwner.PhoneNumber,
                    From = _termiiAuthSettings.SenderId,
                    Sms = _appSettings.AccountCreationOTPMsg.Replace("{TOKEN}", genericResponse.Message),
                    Type = _termiiAuthSettings.Type,
                    Channel = _termiiAuthSettings.Channel,
                    ApiKey = _termiiAuthSettings.ApiKey
                };
                var termiiResponse = ApiHelper.DoWebRequestAsync<TermiiResponse>(_termiiAuthSettings.Url, termiiRequest, "post");
            }
            var userData = _mapper.Map<UserDataResponse>(storeOwner);

            var token = GenerateAuthenticationTokenForStoreOwner(storeOwner);
            return new AuthenticationResponse { Success = true, Token = token, Data = userData };
        }

        public async Task<AuthenticationResponse> FacebookLoginStoreOwnerAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return new AuthenticationResponse { Errors = new[] { "AccessToken cannot be empty." } };

            var validateTokenResult = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);
            if(validateTokenResult.Data != null)
            {
                if (!validateTokenResult.Data.IsValid)
                    return new AuthenticationResponse { Errors = new[] { "Invalid Facebook Token." } };

                var fbUserInfo = await _facebookAuthService.GetUserInfoAsync(accessToken);
                if (fbUserInfo.Id == "Failed")
                    return new AuthenticationResponse { Errors = new[] { "Failed to Get Facebook User. " } };

                var storeOwnerExist = await _dataContext.StoreOwners.FirstOrDefaultAsync(s => s.Email == fbUserInfo.Email);
                if(storeOwnerExist == null) //Register StoreOwner
                {
                    var newStoreOwner = new StoreOwner()
                    {
                        Email = fbUserInfo.Email,
                        FirstName = fbUserInfo.FirstName,
                        MiddleName = fbUserInfo.MiddleName,
                        LastName = fbUserInfo.LastName,
                        PhoneNumber = string.Empty,
                        ProfilePicUrl = fbUserInfo.Picture.FacebookPictureData.Url.ToString(),
                        IsVerified = true,
                        DateRegistered = DateTime.Now,
                        IsFacebookRegistered = true
                    };
                    await _dataContext.StoreOwners.AddAsync(newStoreOwner);
                    var created = await _dataContext.SaveChangesAsync();
                    if(created <= 0)
                        return new AuthenticationResponse { Errors = new[] { "Failed to create customer" } };

                    var userData = _mapper.Map<UserDataResponse>(newStoreOwner);

                    var token = GenerateAuthenticationTokenForStoreOwner(newStoreOwner);
                    return new AuthenticationResponse { Success = true, Token = token, Data = userData };
                }
                else //Signin StoreOwner
                {
                    storeOwnerExist.LastLoginDate = DateTime.Now;
                    _dataContext.Entry(storeOwnerExist).State = EntityState.Modified;
                    var updated = await _dataContext.SaveChangesAsync();
                    if(updated <= 0)
                        return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };

                    var userData = _mapper.Map<UserDataResponse>(storeOwnerExist);

                    var token = GenerateAuthenticationTokenForStoreOwner(storeOwnerExist);
                    return new AuthenticationResponse { Success = true, Token = token, Data = userData };
                }
            }
            else
            {
                return new AuthenticationResponse { Errors = new[] { "Failed to Validate Facebook." } };
            }
        }

        public async Task<AuthenticationResponse> GoogleLoginStoreOwnerAsync(GoogleAuthRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Email))
                return new AuthenticationResponse { Errors = new[] { "FirstName, LastName and Email cannot be empty." } };
            var storeOwnerExist = await _dataContext.StoreOwners.FirstOrDefaultAsync(s => s.Email == request.Email);
            if(storeOwnerExist != null) // Sign Store Owner in
            {
                storeOwnerExist.LastLoginDate = DateTime.Now;
                _dataContext.Entry(storeOwnerExist).State = EntityState.Modified;
                var updated = await _dataContext.SaveChangesAsync();
                if(updated <= 0)
                    return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };

                var userData = _mapper.Map<UserDataResponse>(storeOwnerExist);

                var token = GenerateAuthenticationTokenForStoreOwner(storeOwnerExist);
                return new AuthenticationResponse { Success = true, Token = token, Data = userData };
            }
            else // Register Store Owner
            {
                var newStoreOwner = new StoreOwner()
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    ProfilePicUrl = request.ImageUrl,
                    IsVerified = true,
                    DateRegistered = DateTime.Now,
                    IsGoogleRegistered = true
                };
                await _dataContext.StoreOwners.AddAsync(newStoreOwner);
                var created = await _dataContext.SaveChangesAsync();
                if(created <= 0)
                    return new AuthenticationResponse { Errors = new[] { "Failed to register customer." } };

                var userData = _mapper.Map<UserDataResponse>(newStoreOwner);

                var token = GenerateAuthenticationTokenForStoreOwner(newStoreOwner);
                return new AuthenticationResponse { Success = true, Token = token, Data = userData };
            }
        }

        public async Task<AuthenticationResponse> LoginStoreOwnerAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return new AuthenticationResponse { Errors = new[] { "Email/Password cannot be empty." } };

            var storeOwner = await _dataContext.StoreOwners.SingleOrDefaultAsync(s => s.Email == email);
            if(storeOwner == null)
                return new AuthenticationResponse { Errors = new[] { "Store Owner does not exist." } };
            if(!storeOwner.IsVerified)
                return new AuthenticationResponse { Errors = new[] { "Please verify with the OTP sent to your phone." } };

            bool isPasswordCorrect = false;
            try
            {
                isPasswordCorrect = Security.VerifyPassword(password, storeOwner.PasswordHash, storeOwner.PasswordSalt);
            }
            catch (Exception ex)
            {
                return new AuthenticationResponse { Errors = new[] { ex.Message } };
            }
            if(!isPasswordCorrect)
                return new AuthenticationResponse { Errors = new[] { "Store Owner Email/Password is not correct." } };

            storeOwner.LastLoginDate = DateTime.Now;
            _dataContext.Entry(storeOwner).State = EntityState.Modified;
            var updated = await _dataContext.SaveChangesAsync();
            if(updated <= 0)
                return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };

            var userData = _mapper.Map<UserDataResponse>(storeOwner);

            var token = GenerateAuthenticationTokenForStoreOwner(storeOwner);
            return new AuthenticationResponse { Success = true, Token = token, Data = userData };
        }

        private string GenerateAuthenticationTokenForStoreOwner(StoreOwner storeOwner)
        {
            var claims = new[]
            {
                new Claim("FirstName", storeOwner.FirstName),
                new Claim("LastName", storeOwner.LastName),
                new Claim("Email", storeOwner.Email),
                new Claim("PhoneNumber", storeOwner.PhoneNumber),
                new Claim(ClaimTypes.Role, Roles.StoreOwner),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Issuer,
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                notBefore: DateTime.UtcNow,
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
