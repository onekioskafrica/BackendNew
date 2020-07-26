using AutoMapper;
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
    public class AdminService : IAdminService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public AdminService(DataContext dataContext, JwtSettings jwtSettings, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _jwtSettings = jwtSettings;
        }

        public async Task<GenericResponse> ChangePassword(AdminChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword) || string.IsNullOrWhiteSpace(request.AdminId.ToString()))
                return new GenericResponse { Status = false, Message = "OldPassword, NewPassword and AdminId cannot be empty." };

            var adminExist = await _dataContext.Admins.FirstOrDefaultAsync(a => a.AdminId == request.AdminId);

            if(adminExist == null)
            {
                return new GenericResponse { Status = false, Message = "Invalid Admin Id" };
            }

            //Check the correctness of OldPassword
            bool isPasswordCorrect = false;
            try
            {
                isPasswordCorrect = Security.VerifyPassword(request.OldPassword, adminExist.PasswordHash, adminExist.PasswordSalt);
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

            adminExist.PasswordHash = passwordHash;
            adminExist.PasswordSalt = passwordSalt;

            _dataContext.Entry(adminExist).State = EntityState.Modified;
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

            //Send mail to Admin

            //Add this action to AdminActivityLog
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action = AdminActionsEnum.ADMIN_CHANGE_PASSWORD.ToString(), PerformerId = adminExist.AdminId, DateOfAction = DateTime.Now });
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

        public async Task<GenericResponse> CreateAdminAsync(Admin admin, string password, Guid callerId)
        {
            GenericResponse response = new GenericResponse();
            //Create Admin
            if (string.IsNullOrWhiteSpace(admin.FirstName) || string.IsNullOrWhiteSpace(admin.LastName) || string.IsNullOrWhiteSpace(admin.Email))
                return new GenericResponse { Status = false, Message = "FirstName, LastName and Email cannot be empty" };

            var adminExist = await _dataContext.Admins.FirstOrDefaultAsync(a => a.Email == admin.Email || a.PhoneNumber == admin.PhoneNumber);

            if (adminExist != null)
            {
                return new GenericResponse { Status = false, Message = "Admin with this email or phonenumber already exists." };
            }

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
            admin.CreatedBy = callerId;

            await _dataContext.Admins.AddAsync(admin);
            var created = 0;
            try
            {
                created = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Failed to create Admin";
                return response;
            }
            if (created <= 0)
            {
                response.Status = false;
                response.Message = "Failed to create Admin";
                return response;
            }

            //Send Mail to Admin

            //Log Admin Activity
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { PerformerId = callerId, Action = AdminActionsEnum.ADMIN_CREATE_ADMIN.ToString(), AdminId = admin.AdminId, DateOfAction = DateTime.Now });
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Do nothing
            }

            var userData = _mapper.Map<AdminUserDataResponse>(admin);
            response.Status = true;
            response.Message = "Success";
            response.Body = userData;
            return response;
        }

        public async Task<AuthenticationResponse> LoginAdminAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return new AuthenticationResponse { Errors = new[] { "Email/Password cannot be empty." } };

            var admin = await _dataContext.Admins.SingleOrDefaultAsync(a => a.Email == email);
            if(admin == null)
                return new AuthenticationResponse { Errors = new[] { "Admin does not exist." } };

            bool isPasswordCorrect = false;
            try
            {
                isPasswordCorrect = Security.VerifyPassword(password, admin.PasswordHash, admin.PasswordSalt);
            }
            catch (Exception)
            {
                return new AuthenticationResponse { Errors = new[] { "Error Occurred." } };
            }
            if (!isPasswordCorrect)
                return new AuthenticationResponse { Errors = new[] { "Admin Email/Password is not correct." } };

            admin.LastLoginDate = DateTime.Now;
            _dataContext.Entry(admin).State = EntityState.Modified;
            var updated = await _dataContext.SaveChangesAsync();
            if (updated <= 0)
                return new AuthenticationResponse { Errors = new[] { "Failed to signin." } };

            // Add activity to admin log
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action =  AdminActionsEnum.LOGIN.ToString(), DateOfAction = DateTime.Now } );
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Do nothing
            }


            var userData = _mapper.Map<AdminUserDataResponse>(admin);
            var token = GenerateAuthenticationTokenForAdmin(admin);
            return new AuthenticationResponse { Success = true, Token = token, Data = userData };
        }

        private string GenerateAuthenticationTokenForAdmin(Admin admin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("FirstName", admin.FirstName),
                    new Claim("LastName", admin.LastName),
                    new Claim("Email", admin.Email),
                    new Claim("PhoneNumber", admin.PhoneNumber),
                    new Claim(ClaimTypes.Role, Roles.Admin),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
