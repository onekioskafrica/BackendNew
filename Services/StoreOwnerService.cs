using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

namespace OK_OnBoarding.Services
{
    public class StoreOwnerService : IStoreOwnerService
    {
        private readonly DataContext _dataContext;
        private readonly JwtSettings _jwtSettings;
        private readonly IFacebookAuthService _facebookAuthService;

        public StoreOwnerService(DataContext dataContext, JwtSettings jwtSettings, IFacebookAuthService facebookAuthService)
        {
            _dataContext = dataContext;
            _jwtSettings = jwtSettings;
            _facebookAuthService = facebookAuthService;
        }

        public async Task<AuthenticationResponse> CreateStoreOwnerAsync(StoreOwner storeOwner, string password)
        {
            if(string.IsNullOrWhiteSpace(storeOwner.FirstName) || string.IsNullOrWhiteSpace(storeOwner.LastName) || string.IsNullOrWhiteSpace(storeOwner.EmailAddress))
                return new AuthenticationResponse { Errors = new[] { "FirstName, LastName and Email cannot be empty" } };

            var storeOwnerExist = await _dataContext.StoreOwners.FirstOrDefaultAsync(s => s.EmailAddress == storeOwner.EmailAddress || s.PhoneNumber == storeOwner.PhoneNumber);

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
            catch(Exception ex)
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

            var token = GenerateAuthenticationTokenForStoreOwner(storeOwner);
            return new AuthenticationResponse { Success = true, Token = token };
        }

        public async Task<AuthenticationResponse> FacebookLoginStoreOwnerAsync(string accessToken)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticationResponse> GoogleLoginStoreOwnerAsync(GoogleAuthRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticationResponse> LoginStoreOwnerAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        private string GenerateAuthenticationTokenForStoreOwner(StoreOwner storeOwner)
        {
            var claims = new[]
            {
                new Claim("FirstName", storeOwner.FirstName),
                new Claim("LastName", storeOwner.LastName),
                new Claim("Email", storeOwner.EmailAddress),
                new Claim("PhoneNumber", storeOwner.PhoneNumber),
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
