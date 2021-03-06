﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
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

namespace OK_OnBoarding.Services
{
    public class SuperAdminService : ISuperAdminService
    {
        private static Random random = new Random();
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly AppSettings _appSettings;

        public SuperAdminService(DataContext dataContext, JwtSettings jwtSettings, IMapper mapper, AppSettings appSettings)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _jwtSettings = jwtSettings;
            _appSettings = appSettings;
        }

        public async Task<GenericResponse> ChangePassword(SuperAdminChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword) || string.IsNullOrWhiteSpace(request.SuperAdminId.ToString()))
                return new GenericResponse { Status = false, Message = "OldPassword, NewPassword and AdminId cannot be empty." };

            var superAdminExist = await _dataContext.SuperAdmin.FirstOrDefaultAsync(s => s.Id == request.SuperAdminId);

            if (superAdminExist == null)
            {
                return new GenericResponse { Status = false, Message = "Invalid Id" };
            }

            //Check the correctness of OldPassword
            bool isPasswordCorrect = false;
            try
            {
                isPasswordCorrect = Security.VerifyPassword(request.OldPassword, superAdminExist.PasswordHash, superAdminExist.PasswordSalt);
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (!isPasswordCorrect)
                return new GenericResponse { Status = false, Message = "Incorrect Old Password." };

            //Create new PasswordHash and PasswordSalt from NewPassword
            byte[] passwordHash, passwordSalt;
            try
            {
                Security.CreatePasswordHash(request.NewPassword, out passwordHash, out passwordSalt);
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred" };
            }

            superAdminExist.PasswordHash = passwordHash;
            superAdminExist.PasswordSalt = passwordSalt;

            _dataContext.Entry(superAdminExist).State = EntityState.Modified;
            var updated = 0;
            try
            {
                updated = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Failed to change password." };
            }
            if (updated <= 0)
                return new GenericResponse { Status = false, Message = "Failed to change password." };

            //Send mail to SuperAdmin

            //Add this action to AdminActivityLog
            await _dataContext.SuperAdminActivityLogs.AddAsync(new SuperAdminActivityLog { Action = SuperAdminActionsEnum.SUPER_CHANGE_PASSWORD.ToString(), DateOfAction = DateTime.Now });
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Do nothing
            }

            return new GenericResponse { Status = true, Message = "Password Changed Successfully." };
        }

        public async Task<GenericResponse> CreateAdminAsync(Admin admin)
        {
            
            GenericResponse response = new GenericResponse();
            //Create Admin
            if (string.IsNullOrWhiteSpace(admin.FirstName) || string.IsNullOrWhiteSpace(admin.LastName) || string.IsNullOrWhiteSpace(admin.Email))
                return new GenericResponse { Status = false, Message = "FirstName, LastName and Email cannot be empty"  };

            var adminExist = await _dataContext.Admins.FirstOrDefaultAsync(a => a.Email == admin.Email || a.PhoneNumber == admin.PhoneNumber);

            if (adminExist != null)
            {
                return new GenericResponse { Status = false,  Message =  "Admin with this email or phonenumber already exists." };
            }
            //string password = GenerateAdminPassword(_appSettings.LengthOfGeneratedPassword);
            string password = "Test123@";
            byte[] passwordHash, passwordSalt;
            try
            {
                Security.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred" };
            }

            admin.PasswordHash = passwordHash;
            admin.PasswordSalt = passwordSalt;
            admin.DateCreated = DateTime.Now;
            admin.IsActive = true;

            await _dataContext.Admins.AddAsync(admin);
            var created = await _dataContext.SaveChangesAsync();
            if(created <= 0)
            {
                response.Status = false;
                response.Message = "Failed to create Admin";
                return response;
            }

            //Send Mail to Admin {Hangfire} send password also

            //Log Superadmin activity
            await _dataContext.SuperAdminActivityLogs.AddAsync(new SuperAdminActivityLog { Action = SuperAdminActionsEnum.SUPER_CREATE_ADMIN.ToString(), AdminId = admin.AdminId, DateOfAction = DateTime.Now });
            await _dataContext.SaveChangesAsync();

            var userData = _mapper.Map<AdminUserDataResponse>(admin);
            response.Status = true;
            response.Message = "Success";
            response.Data = userData;
            return response;
        }

        public async Task<AuthenticationResponse> CreateSuperAdminAsync(SuperAdmin superAdmin, string password)
        {
            if (string.IsNullOrWhiteSpace(superAdmin.FirstName) || string.IsNullOrWhiteSpace(superAdmin.LastName) || string.IsNullOrWhiteSpace(superAdmin.Email))
                return new AuthenticationResponse { Errors = new[] { "FirstName, LastName and Email cannot be empty" } };

            var superAdminExist = await _dataContext.SuperAdmin.FirstOrDefaultAsync(s => s.Email == superAdmin.Email || s.PhoneNumber == superAdmin.PhoneNumber);

            if(superAdminExist != null)
            {
                return new AuthenticationResponse { Errors = new[] { "SuperAdmin with this email and phonenumber already exists." } };
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

            superAdmin.PasswordHash = passwordHash;
            superAdmin.PasswordSalt = passwordSalt;
            superAdmin.DateCreated = DateTime.Now;

            await _dataContext.SuperAdmin.AddAsync(superAdmin);
            var created = await _dataContext.SaveChangesAsync();
            if(created <= 0)
            {
                return new AuthenticationResponse { Errors = new[] { "Failed to register super admin." } };
            }
            //Send mail to SuperAdmin
            var userData = _mapper.Map<SuperAdminUserDataResponse>(superAdmin);

            return new AuthenticationResponse { Success = true, Data = userData };
        }

        public async Task<GenericResponse> DeactivateAdminAsync(Guid adminId)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse> GetAdminByIdAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse> GetAllAdminsActivityLogAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse> GetAllAdminsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse> GetAllPrivilegesAsync()
        {
            GenericResponse response = new GenericResponse();
            var allPrivileges = await _dataContext.Priviliges.ToListAsync();
            response.Status = true;
            response.Data = allPrivileges;
            return response;
        }

        public Task<GenericResponse> GetSingleAdminAcitvityLogAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticationResponse> LoginSuperAdminAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return new AuthenticationResponse { Errors = new[] { "Email/Password cannot be empty." } };

            var superAdmin = await _dataContext.SuperAdmin.SingleOrDefaultAsync(s => s.Email == email);
            if(superAdmin == null)
                return new AuthenticationResponse { Errors = new[] { "Super Admin does not exist." } };

            bool isPasswordCorrect = false;
            try
            {
                isPasswordCorrect = Security.VerifyPassword(password, superAdmin.PasswordHash, superAdmin.PasswordSalt);
            }
            catch (Exception)
            {
                return new AuthenticationResponse { Errors = new[] { "Error Occurred." } };
            }
            if (!isPasswordCorrect)
                return new AuthenticationResponse { Errors = new[] { "SuperAdmin Email/Password is not correct." } };

            superAdmin.LastLoginDate = DateTime.Now;
            _dataContext.Entry(superAdmin).State = EntityState.Modified;
            var updated = await _dataContext.SaveChangesAsync();
            if (updated <= 0)
                return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };
            //Add activity to superadmin log
            await _dataContext.SuperAdminActivityLogs.AddAsync(new SuperAdminActivityLog() { Action = SuperAdminActionsEnum.LOGIN.ToString(),  DateOfAction = DateTime.Now });
            await _dataContext.SaveChangesAsync();

            var userData = _mapper.Map<SuperAdminUserDataResponse>(superAdmin);
            var token = GenerateAuthenticationTokenForSuperAdmin(superAdmin);
            return new AuthenticationResponse { Success = true, Token = token, Data = userData };
        }

        public Task<GenericResponse> SuperAdminChangePasswordAsync(int superAdminId, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        private string GenerateAuthenticationTokenForSuperAdmin(SuperAdmin superAdmin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor { 
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("FirstName", superAdmin.FirstName),
                    new Claim("LastName", superAdmin.LastName),
                    new Claim("Email", superAdmin.Email),
                    new Claim("PhoneNumber", superAdmin.PhoneNumber),
                    new Claim(ClaimTypes.Role, Roles.SuperAdmin),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires= DateTime.UtcNow.AddMinutes(10),
                NotBefore= DateTime.UtcNow,
                SigningCredentials= creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateAdminPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%&*()_-+=";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
