using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    public class DeliverymanService : IDelivermanService
    {
        private readonly DataContext _dataContext;
        private readonly JwtSettings _jwtSettings;
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IOTPService _otpService;
        private readonly IMapper _mapper;
        private readonly TermiiAuthSettings _termiiAuthSettings;
        private readonly AppSettings _appSettings;

        public DeliverymanService(DataContext dataContext, JwtSettings jwtSettings, IFacebookAuthService facebookAuthService, IOTPService otpService, IMapper mapper, TermiiAuthSettings termiiAuthSettings, AppSettings appSettings)
        {
            _dataContext = dataContext;
            _jwtSettings = jwtSettings;
            _facebookAuthService = facebookAuthService;
            _otpService = otpService;
            _mapper = mapper;
            _termiiAuthSettings = termiiAuthSettings;
            _appSettings = appSettings;
        }

        public async Task<AuthenticationResponse> CreateDeliverymanAsync(Deliveryman deliveryman, string password)
        {
            if (string.IsNullOrWhiteSpace(deliveryman.FirstName) || string.IsNullOrWhiteSpace(deliveryman.LastName) || string.IsNullOrWhiteSpace(deliveryman.Email))
                return new AuthenticationResponse { Errors = new[] { "FirstName, LastName and Email cannot be empty" } };

            var deliverymanExist = await _dataContext.DeliveryMen.FirstOrDefaultAsync(d => d.Email == deliveryman.Email || d.PhoneNumber == deliveryman.PhoneNumber);

            if(deliverymanExist != null)
            {
                if(!deliverymanExist.IsVerified)
                    return new AuthenticationResponse { Errors = new[] { "Deliveryman already exist but not verified." } };

                return new AuthenticationResponse { Errors = new[] { "Deliveryman with this email and phonenumber already exists." } };
            }

            byte[] passwordHash, passwordSalt;
            try
            {
                Security.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            }
            catch (Exception)
            {
                return new AuthenticationResponse { Errors = new[] { "Error Occurred." } };
            }

            deliveryman.PasswordHash = passwordHash;
            deliveryman.PasswordSalt = passwordSalt;
            deliveryman.DateRegistered = DateTime.Now;

            await _dataContext.DeliveryMen.AddAsync(deliveryman);
            var created = await _dataContext.SaveChangesAsync();
            if (created <= 0)
            {
                return new AuthenticationResponse { Errors = new[] { "Failed to register deliveryman." } };
            }
            var genericResponse = await _otpService.GenerateOTPForDeliveryman(OTPGenerationReason.TokenGeneration.ToString(), deliveryman.PhoneNumber, deliveryman.Email);
            if (genericResponse.Status)
            {
                // Send Sms
                var termiiRequest = new TermiiRequest()
                {
                    To = deliveryman.PhoneNumber,
                    From = _termiiAuthSettings.SenderId,
                    Sms = _appSettings.AccountCreationOTPMsg.Replace("{TOKEN}", genericResponse.Message),
                    Type = _termiiAuthSettings.Type,
                    Channel = _termiiAuthSettings.Channel,
                    ApiKey = _termiiAuthSettings.ApiKey
                };
                var termiiResponse = ApiHelper.DoWebRequestAsync<TermiiResponse>(_termiiAuthSettings.Url, termiiRequest, "post");
            }
            var userData = _mapper.Map<UserDataResponse>(deliveryman);

            var token = GenerateAuthenticationTokenForDeliveryman(deliveryman);
            return new AuthenticationResponse { Success = true, Token = token, Data = userData };
        }

        public async Task<AuthenticationResponse> FacebookLoginDeliverymanAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return new AuthenticationResponse { Errors = new[] { "AccessToken cannot be empty." } };

            var validateTokenResult = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);
            if(validateTokenResult.Data != null)
            {
                if(!validateTokenResult.Data.IsValid)
                    return new AuthenticationResponse { Errors = new[] { "Invalid Facebook Token." } };

                var fbUserInfo = await _facebookAuthService.GetUserInfoAsync(accessToken);
                if(fbUserInfo.Id == "Failed")
                    return new AuthenticationResponse { Errors = new[] { "Failed to Get Facebook User. " } };


                var deliverymanExist = await _dataContext.DeliveryMen.FirstOrDefaultAsync(d => d.Email == fbUserInfo.Email);
                if(deliverymanExist == null) //Register Deliveryman
                {
                    var newDeliveryman = new Deliveryman()
                    {
                        Email = fbUserInfo.Email,
                        FirstName = fbUserInfo.FirstName,
                        MiddleName = fbUserInfo.MiddleName,
                        LastName = fbUserInfo.LastName,
                        PhoneNumber = string.Empty,
                        ProfilePicUrl = fbUserInfo.Picture.FacebookPictureData.Url.ToString(),
                        IsVerified = true,
                        DateRegistered = DateTime.Now,
                        IsGoogleRegistered = true
                    };
                    await _dataContext.DeliveryMen.AddAsync(newDeliveryman);
                    var created = await _dataContext.SaveChangesAsync();
                    if(created <= 0)
                        return new AuthenticationResponse { Errors = new[] { "Failed to create deliveryman" } };

                    var userData = _mapper.Map<UserDataResponse>(newDeliveryman);

                    var token = GenerateAuthenticationTokenForDeliveryman(newDeliveryman);
                    return new AuthenticationResponse { Success = true, Token = token, Data = userData };
                }
                else //Signin Customer
                {
                    deliverymanExist.LastLoginDate = DateTime.Now;
                    _dataContext.Entry(deliverymanExist).State = EntityState.Modified;
                    var updated = await _dataContext.SaveChangesAsync();
                    if(updated <= 0)
                        return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };

                    var userData = _mapper.Map<UserDataResponse>(deliverymanExist);

                    var token = GenerateAuthenticationTokenForDeliveryman(deliverymanExist);
                    return new AuthenticationResponse { Success = true, Token = token, Data = userData };
                }
            }
            else
            {
                return new AuthenticationResponse { Errors = new[] { "Failed to Validate Facebook." } };
            }
        }

        public async Task<AuthenticationResponse> GoogleLoginDeliverymanAsync(GoogleAuthRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Email))
                return new AuthenticationResponse { Errors = new[] { "FirstName, LastName and Email cannot be empty." } };
            var deliverymanExist = await _dataContext.DeliveryMen.FirstOrDefaultAsync(d => d.Email == request.Email);
            if(deliverymanExist != null) //Sign Deliveryman in
            {
                deliverymanExist.LastLoginDate = DateTime.Now;
                _dataContext.Entry(deliverymanExist).State = EntityState.Modified;
                var updated = await _dataContext.SaveChangesAsync();
                if(updated <= 0)
                    return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };

                var userData = _mapper.Map<UserDataResponse>(deliverymanExist);

                var token = GenerateAuthenticationTokenForDeliveryman(deliverymanExist);
                return new AuthenticationResponse { Success = true, Token = token, Data = userData };
            }
            else // Register Deliveryman
            {
                var newDeliveryman = new Deliveryman()
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
                await _dataContext.DeliveryMen.AddAsync(newDeliveryman);
                var created = await _dataContext.SaveChangesAsync();
                if(created <= 0)
                    return new AuthenticationResponse { Errors = new[] { "Failed to register deliveryman." } };

                var userData = _mapper.Map<UserDataResponse>(newDeliveryman);

                var token = GenerateAuthenticationTokenForDeliveryman(newDeliveryman);
                return new AuthenticationResponse { Success = true, Token = token, Data = newDeliveryman };
            }

        }

        public async Task<AuthenticationResponse> LoginDeliverymanAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return new AuthenticationResponse { Errors = new[] { "Email/Password cannot be empty." } };

            var deliveryman = await _dataContext.DeliveryMen.SingleOrDefaultAsync(d => d.Email == email);

            if (deliveryman == null)
                return new AuthenticationResponse { Errors = new[] { "Deliveryman does not exist." } };

            bool isPasswordCorrect = false;
            try
            {
                isPasswordCorrect = Security.VerifyPassword(password, deliveryman.PasswordHash, deliveryman.PasswordSalt);
            }
            catch (Exception)
            {
                return new AuthenticationResponse { Errors = new[] { "Error Occurred." } };
            }
            if(!isPasswordCorrect)
                return new AuthenticationResponse { Errors = new[] { "Deliveryman Email/Password is not correct." } };

            deliveryman.LastLoginDate = DateTime.Now;
            _dataContext.Entry(deliveryman).State = EntityState.Modified;
            var updated = await _dataContext.SaveChangesAsync();
            if(updated <= 0)
                return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };

            var userData = _mapper.Map<UserDataResponse>(deliveryman);

            var token = GenerateAuthenticationTokenForDeliveryman(deliveryman);
            return new AuthenticationResponse { Success = true, Token = token, Data = userData };
        }

        private string GenerateAuthenticationTokenForDeliveryman(Deliveryman deliveryman)
        {
            var claims = new[]
            {
                new Claim("FirstName", deliveryman.FirstName),
                new Claim("LastName", deliveryman.LastName),
                new Claim("Email", deliveryman.Email),
                new Claim("PhoneNumber", deliveryman.PhoneNumber),
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
