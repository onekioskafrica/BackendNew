﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OK_OnBoarding.Contracts.V1;
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
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace OK_OnBoarding.Services
{
    public class CustomersService : ICustomersService
    {
        private readonly DataContext _dataContext;
        private readonly JwtSettings _jwtSettings;
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IOTPService _otpService;
        private readonly IMapper _mapper;
        private readonly TermiiAuthSettings _termiiAuthSettings;
        private readonly AppSettings _appSettings;

        public CustomersService(DataContext dataContext, JwtSettings jwtSettings, IFacebookAuthService facebookAuthService, IOTPService otpService, IMapper mapper, TermiiAuthSettings termiiAuthSettings, AppSettings appSettings)
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

            var customerExist = await _dataContext.Customers.FirstOrDefaultAsync(d => d.Email == request.Email);
            if (customerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Customer Email." };

            var otpResponse = await _otpService.VerifyOTPForCustomer(request.OTP, customerExist.PhoneNumber);
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

            customerExist.PasswordHash = passwordHash;
            customerExist.PasswordSalt = passwordSalt;
            _dataContext.Entry(customerExist).State = EntityState.Modified;
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

        public async Task<AuthenticationResponse> GoogleLoginCustomerAsync(GoogleAuthRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Email))
                return new AuthenticationResponse { Errors = new[] { "FirstName, LastName and Email cannot be empty." } };
            var customerExist = await _dataContext.Customers.FirstOrDefaultAsync(c => c.Email == request.Email);
            if(customerExist != null) //Sign Customer in
            {
                customerExist.LastLoginDate = DateTime.Now;
                _dataContext.Entry(customerExist).State = EntityState.Modified;
                var updated = await _dataContext.SaveChangesAsync();
                if (updated <= 0)
                    return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };
                var userData = _mapper.Map<CustomerUserDataResponse>(customerExist);

                var token = GenerateAuthenticationTokenForCustomer(customerExist);
                return new AuthenticationResponse { Success = true, Token = token, Data = userData };
            }
            else // Register Customer
            {
                var newCustomer = new Customer()
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
                await _dataContext.Customers.AddAsync(newCustomer);
                var created = await _dataContext.SaveChangesAsync();
                if (created <= 0)
                    return new AuthenticationResponse { Errors = new[] { "Failed to register customer." } };
                var userData = _mapper.Map<CustomerUserDataResponse>(newCustomer);

                var token = GenerateAuthenticationTokenForCustomer(newCustomer);
                return new AuthenticationResponse { Success = true, Token = token, Data = userData };
            }
        }

        public async Task<AuthenticationResponse> CreateCustomerAsync(Customer customer, string password)
        {
            if (string.IsNullOrWhiteSpace(customer.FirstName) || string.IsNullOrWhiteSpace(customer.LastName) || string.IsNullOrWhiteSpace(customer.Email))
                return new AuthenticationResponse { Errors = new[] { "FirstName, LastName and Email cannot be empty" } };
            var customerExist = await _dataContext.Customers.FirstOrDefaultAsync(c => c.Email == customer.Email || c.PhoneNumber == customer.PhoneNumber);

            if (customerExist != null)
            {
                if (!customerExist.IsVerified)
                 return new AuthenticationResponse { Errors = new[] { "Customer already exist but not verified." } };

                return new AuthenticationResponse { Errors = new[] { "Customer with this email and phonenumber already exists." } };
            }
             

            byte[] passwordHash, passwordSalt;
            try
            {
                Security.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            }catch(Exception){
                return new AuthenticationResponse { Errors = new[] { "Error Occurred." } };
            }
 
            customer.PasswordHash = passwordHash;
            customer.PasswordSalt = passwordSalt;
            customer.DateRegistered = DateTime.Now;

            await _dataContext.Customers.AddAsync(customer);
            var created = await _dataContext.SaveChangesAsync();
            if(created <= 0)
            {
                return new AuthenticationResponse { Errors = new[] { "Failed to register customer." } };
            }
            var genericResponse = await _otpService.GenerateOTPForCustomer(OTPGenerationReason.OTPGENERATION_FOR_SIGN_UP.ToString(), customer.PhoneNumber, customer.Email);
            if (genericResponse.Status)
            {
                // Send Sms 
                var termiiRequest = new TermiiRequest()
                {
                    To = customer.PhoneNumber,
                    From = _termiiAuthSettings.SenderId,
                    Sms = _appSettings.AccountCreationOTPMsg.Replace("{TOKEN}", genericResponse.Message),
                    Type = _termiiAuthSettings.Type,
                    Channel = _termiiAuthSettings.Channel,
                    ApiKey = _termiiAuthSettings.ApiKey
                };
                var termiiResponse = ApiHelper.DoWebRequestAsync<TermiiResponse>(_termiiAuthSettings.Url, termiiRequest, "post");
            }
            var userData = _mapper.Map<CustomerUserDataResponse>(customer);
            var token = GenerateAuthenticationTokenForCustomer(customer);

            return new AuthenticationResponse { Success = true, Token = token, Data = userData };
        }

        public async Task<AuthenticationResponse> FacebookLoginCustomerAsync(string accessToken)
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

                var customerExist = await _dataContext.Customers.FirstOrDefaultAsync(c => c.Email == fbUserInfo.Email);
                if(customerExist == null) //Register Customer
                {
                    var newCustomer = new Customer() {
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
                    await _dataContext.Customers.AddAsync(newCustomer);
                    var created = await _dataContext.SaveChangesAsync();
                    if (created <= 0)
                        return new AuthenticationResponse { Errors = new[] { "Failed to create customer" } };
                    var userData = _mapper.Map<CustomerUserDataResponse>(newCustomer);

                    var token = GenerateAuthenticationTokenForCustomer(newCustomer);

                    return new AuthenticationResponse { Success = true, Token = token, Data = userData };
                }
                else //Signin Customer
                {
                    customerExist.LastLoginDate = DateTime.Now;
                    _dataContext.Entry(customerExist).State = EntityState.Modified;
                    var updated = await _dataContext.SaveChangesAsync();
                    if (updated <= 0)
                        return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };
                    var userData = _mapper.Map<CustomerUserDataResponse>(customerExist);

                    var token = GenerateAuthenticationTokenForCustomer(customerExist);
                    return new AuthenticationResponse { Success = true, Token = token, Data = userData };
                }
            }
            else
            {
                return new AuthenticationResponse { Errors = new[] { "Failed to Validate Facebook." } };
            }

        }

        public async Task<AuthenticationResponse> LoginCustomerAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return new AuthenticationResponse { Errors = new[] { "Email/Password cannot be empty." } };

            var customer = await _dataContext.Customers.SingleOrDefaultAsync(c => c.Email == email);

            if (customer == null)
                return new AuthenticationResponse { Errors = new[] { "Customer does not exist." } };
            

            var isUpdateComplete = true;
            if (string.IsNullOrWhiteSpace(customer.Line1))
                isUpdateComplete = false;

            if (!customer.IsVerified)
            {
                var userResponseData = _mapper.Map<CustomerUserDataResponse>(customer);
                userResponseData.IsUpdateComplete = isUpdateComplete;

                return new AuthenticationResponse { Errors = new[] { "Please verify with the otp sent to your phone." }, Data =  userResponseData};
            }
                
            bool isPasswordCorrect = false;
            try
            {
                isPasswordCorrect = Security.VerifyPassword(password, customer.PasswordHash, customer.PasswordSalt);
            }
            catch (Exception ex)
            {
                return new AuthenticationResponse { Errors = new[] { ex.Message } };
            }
            if (!isPasswordCorrect)
                return new AuthenticationResponse { Errors = new[] { "Customer Email/Password is not correct." } };

            customer.LastLoginDate = DateTime.Now;
            _dataContext.Entry(customer).State = EntityState.Modified;
            var updated = await _dataContext.SaveChangesAsync();
            if (updated <= 0)
                return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };

            var userData = _mapper.Map<CustomerUserDataResponse>(customer);
            userData.IsUpdateComplete = isUpdateComplete;

            var token = GenerateAuthenticationTokenForCustomer(customer);
            return new AuthenticationResponse { Success = true, Token = token, Data = userData };
        }

        public async Task<GenericResponse> UpdateAddressAsync(UpdateAddressRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PerformerId.ToString()) || string.IsNullOrWhiteSpace(request.Country) || string.IsNullOrWhiteSpace(request.State) || string.IsNullOrWhiteSpace(request.City) || string.IsNullOrWhiteSpace(request.Line1))
                return new GenericResponse { Status = false, Message = "PerformerId, Country, State, City or Home Address cannot be empty. " };

            var customerExist = await _dataContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == request.PerformerId);
            if (customerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid CustomerId. " };
            if (!customerExist.IsVerified)
                return new GenericResponse { Status = false, Message = "Customer is unverified." };

            customerExist.Country = request.Country;
            customerExist.State = request.State;
            customerExist.City = request.City;
            customerExist.Line1 = request.Line1;

            _dataContext.Entry(customerExist).State = EntityState.Modified;
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
                return new GenericResponse { Status = false, Message = "Couldn't add address details." };

            return new GenericResponse { Status = true, Message = "Address Updated Successfully." };
        }

        private string GenerateAuthenticationTokenForCustomer(Customer customer)
        {
            var claims = new[]
            {
                new Claim("FirstName", customer.FirstName),
                new Claim("LastName", customer.LastName),
                new Claim("Email", customer.Email),
                new Claim("PhoneNumber", customer.PhoneNumber),
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
