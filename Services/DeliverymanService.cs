﻿using Microsoft.EntityFrameworkCore;
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
    public class DeliverymanService : IDelivermanService
    {
        private readonly DataContext _dataContext;
        private readonly JwtSettings _jwtSettings;
        private readonly IFacebookAuthService _facebookAuthService;

        public DeliverymanService(DataContext dataContext, JwtSettings jwtSettings, IFacebookAuthService facebookAuthService)
        {
            _dataContext = dataContext;
            _jwtSettings = jwtSettings;
            _facebookAuthService = facebookAuthService;
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

            var token = GenerateAuthenticationTokenForDeliveryman(deliveryman);
            return new AuthenticationResponse { Success = true, Token = token };
        }

        public async Task<AuthenticationResponse> FacebookLoginDeliverymanAsync(string accessToken)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticationResponse> GoogleLoginDeliverymanAsync(GoogleAuthRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticationResponse> LoginDeliverymanAsync(string email, string password)
        {
            throw new NotImplementedException();
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
