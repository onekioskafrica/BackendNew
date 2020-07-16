using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OK_OnBoarding.Contracts.V1;
using OK_OnBoarding.Data;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
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

        public CustomersService(DataContext dataContext, JwtSettings jwtSettings)
        {
            _dataContext = dataContext;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthenticationResponse> CreateCustomerAsync(Customer customer, string password)
        {
            if (string.IsNullOrEmpty(customer.FirstName) || string.IsNullOrEmpty(customer.LastName) || string.IsNullOrEmpty(customer.Email))
                return new AuthenticationResponse { Errors = new[] { "FirstName, LastName and Email cannot be empty" } };
            var customerExist = await _dataContext.Customers.FirstOrDefaultAsync(c => c.Email == customer.Email || c.PhoneNumber == customer.PhoneNumber);

            if (customerExist != null)
                return new AuthenticationResponse { Errors = new[] { "Customer with this email and phonenumber already exists." } };


            /*if (!customerExist.Verified)
                return new AuthenticationResponse { Errors = new[] { "Customer already exist but not verified." } };*/

            byte[] passwordHash, passwordSalt;
            try
            {
                Security.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            }catch(Exception ex){
                return new AuthenticationResponse { Errors = new[] { "Error Occurred." } };
            }
 
            customer.PasswordHash = passwordHash;
            customer.PasswordSalt = passwordSalt;

            _dataContext.Customers.Add(customer);
            var created = await _dataContext.SaveChangesAsync();
            if(created <= 0)
            {
                return new AuthenticationResponse { Errors = new[] { "Failed to register customer." } };
            }

            var token = GenerateAuthenticationTokenForCustomer(customer);

            return new AuthenticationResponse { Success = true, Token = token };
        }

        public async Task<AuthenticationResponse> LoginCustomerAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
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
            await _dataContext.SaveChangesAsync();
            
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
