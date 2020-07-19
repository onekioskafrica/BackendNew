using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OK_OnBoarding.Contracts.V1;
using OK_OnBoarding.Data;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.ExternalContract;
using OK_OnBoarding.Helpers;
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

        public CustomersService(DataContext dataContext, JwtSettings jwtSettings, IFacebookAuthService facebookAuthService)
        {
            _dataContext = dataContext;
            _jwtSettings = jwtSettings;
            _facebookAuthService = facebookAuthService;
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

                var token = GenerateAuthenticationTokenForCustomer(customerExist);
                return new AuthenticationResponse { Success = true, Token = token };
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

                var token = GenerateAuthenticationTokenForCustomer(newCustomer);
                return new AuthenticationResponse { Success = true, Token = token };
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

            var token = GenerateAuthenticationTokenForCustomer(customer);

            return new AuthenticationResponse { Success = true, Token = token };
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

                    var token = GenerateAuthenticationTokenForCustomer(newCustomer);

                    return new AuthenticationResponse { Success = true, Token = token };
                }
                else //Signin Customer
                {
                    customerExist.LastLoginDate = DateTime.Now;
                    _dataContext.Entry(customerExist).State = EntityState.Modified;
                    var updated = await _dataContext.SaveChangesAsync();
                    if (updated <= 0)
                        return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };

                    var token = GenerateAuthenticationTokenForCustomer(customerExist);
                    return new AuthenticationResponse { Success = true, Token = token };
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

            bool isPasswordCorrect = false;
            try
            {
                isPasswordCorrect = Security.VerifyPassword(password, customer.PasswordHash, customer.PasswordSalt);
            }
            catch (Exception)
            {
                return new AuthenticationResponse { Errors = new[] { "Error Occurred." } };
            }
            if (!isPasswordCorrect)
                return new AuthenticationResponse { Errors = new[] { "Customer Email/Password is not correct." } };

            customer.LastLoginDate = DateTime.Now;
            _dataContext.Entry(customer).State = EntityState.Modified;
            var updated = await _dataContext.SaveChangesAsync();
            if (updated <= 0)
                return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };

            var token = GenerateAuthenticationTokenForCustomer(customer);
            return new AuthenticationResponse { Success = true, Token = token };
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
