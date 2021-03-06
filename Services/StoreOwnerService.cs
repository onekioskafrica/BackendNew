﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        private readonly Random random = new Random();
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

        public async Task<GenericResponse> CloseStoreAsync(CloseStoreRequest request)
        {
            var storeExist = await _dataContext.Stores.Include(s => s.StoreOwner).FirstOrDefaultAsync(s => s.Id == request.StoreId);
            if (storeExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Store" };
            if (storeExist.StoreOwner.Id != request.StoreOwnerId)
                return new GenericResponse { Status = false, Message = "Store not owned by store owner." };

            storeExist.IsClosed = request.IsClosed;
            _dataContext.Entry(storeExist).State = EntityState.Modified;
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
                return new GenericResponse { Status = false, Message = "Unable to close store." };

            // Log to StoreOwnerActivityLogs {Hangfire}
            await _dataContext.StoreOwnerActivityLogs.AddAsync(new StoreOwnerActivityLog { StoreId = request.StoreId, StoreOwnerId = request.StoreOwnerId, Action = request.IsClosed ? StoreOwnerActionsEnum.STORE_CLOSURE.ToString() : StoreOwnerActionsEnum.OPEN_STORE.ToString(), DateOfAction = DateTime.Now });

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Do Nothing
            }

            return new GenericResponse { Status = true, Message = "Store Closed Successfully." };
        }

        public async Task<GenericResponse> SetProductVisibilityAsync(SetProductVisibilityRequest request)
        {
            var productExist = await _dataContext.Products.Include(p => p.Store).FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if(productExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Product" };
            if (productExist.Store.StoreOwner.Id == request.StoreOwnerId)
                return new GenericResponse { Status = false, Message = "Product not owned by Store Owner" };

            productExist.IsVisible = request.IsVisible;
            _dataContext.Entry(productExist).State = EntityState.Modified;

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
                return new GenericResponse { Status = false, Message = "Failed to set product visiblity" };

            // Log to StoreOwnerActivityLogs {Hangfire}
            await _dataContext.StoreOwnerActivityLogs.AddAsync(new StoreOwnerActivityLog { ProductId = request.ProductId, StoreOwnerId = request.StoreOwnerId, Action = request.IsVisible ? StoreOwnerActionsEnum.OPEN_PRODUCT.ToString() : StoreOwnerActionsEnum.HIDE_PRODUCT.ToString(), DateOfAction = DateTime.Now });

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Do Nothing
            }

            return new GenericResponse { Status = true, Message = "Product Hidden Successfully." };

        }

        public async Task<GenericResponse> ConfigureDiscountAsync(StoreOwnerConfigureDiscountRequest request)
        {
            var storeOwnerExist = await _dataContext.StoreOwners.FirstOrDefaultAsync(s => s.Id == request.StoreOwnerId);
            if (storeOwnerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Storeowner." };
            if (!storeOwnerExist.IsVerified)
                return new GenericResponse { Status = false, Message = "Please verify with OTP sent to your phone/email." };

            var storeExist = await _dataContext.Stores.FirstOrDefaultAsync(s => s.Id == request.StoreId);
            if (storeExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Store." };
            if (storeExist.StoreOwnerId != request.StoreOwnerId)
                return new GenericResponse { Status = false, Message = "Store not for Storeowner." };

            var utility = new OnekioskUtility();

            var validationResponse = utility.ValidateDiscountInputs(request.IsPercentageDiscount, request.IsAmountDiscount, request.IsSetPrice, request.PercentageDiscount);
            if (!validationResponse.Status)
                return validationResponse;

            var discountConfiguration = new Coupon {
                IsStoreOwnerConfigured = true,
                StoreId = request.StoreId,
                IsForAllStoresOwnByAStoreOwner = request.IsForAllStoresOwnByAStoreOwner,
                StoreOwnerId = request.StoreOwnerId,
                Code = GenerateCouponCode(_appSettings.LengthOfCouponCode),
                Title = request.Title,
                IsActive = request.IsActive,
                AdminId = null,
                IsAdminConfigured = false,
                IsForCategory = request.IsForCategory,
                CategoryId = request.CategoryId,
                IsForProduct = request.IsForProduct,
                ProductId = request.ProductId,
                IsForAllProducts = request.IsForAllProduct,
                IsForShipping = request.IsForShipping,
                IsForPrice = request.IsForPrice,
                IsSlotSet = request.IsSlotSet,
                AllocatedSlot = request.AllocatedSlot,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsPercentageDiscount = request.IsPercentageDiscount,
                PercentageDiscount = request.PercentageDiscount,
                IsAmountDiscount = request.IsAmountDiscount,
                AmountDiscount = request.AmountDiscount,
                IsSetPrice = request.IsSetPrice,
                SetPrice = request.SetPrice
            };

            await _dataContext.Coupons.AddAsync(discountConfiguration);
            var created = 0;
            try
            {
                created = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (created <= 0)
                return new GenericResponse { Status = false, Message = "Couldn't create Discount Coupon." };

            // Log to StoreOwnerActivityLogs {Hangfire}
            await _dataContext.StoreOwnerActivityLogs.AddAsync(new StoreOwnerActivityLog { DiscountId = discountConfiguration.Id, StoreOwnerId = request.StoreOwnerId, StoreId = request.StoreId, Action = StoreOwnerActionsEnum.DISCOUNT_CREATION.ToString(), DateOfAction = DateTime.Now });

            return new GenericResponse { Status = true, Message = "Success", Data = discountConfiguration };
        }

        public async Task<GenericResponse> ActivateDiscountAsync(StoreOwnerActivateDiscountRequest request)
        {
            var storeOwnerValidationResponse = await ValidateStoreOwner(request.StoreOwnerId);
            if (!storeOwnerValidationResponse.Status)
                return storeOwnerValidationResponse;

            var configuredDiscountExist = await _dataContext.Coupons.FirstOrDefaultAsync(c => c.Id == request.DiscountId);
            if (configuredDiscountExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Discount Id" };

            if (configuredDiscountExist.IsActive == request.Activate)
            {
                return new GenericResponse { Status = false, Message = configuredDiscountExist.IsActive ? "Discount is already active" : "Discount is already inactive" };
            }

            if (configuredDiscountExist.StoreOwnerId != request.StoreOwnerId)
                return new GenericResponse { Status = false, Message = "Discount does not belong to any of your stores." };

            configuredDiscountExist.IsActive = request.Activate;
            _dataContext.Entry(configuredDiscountExist).State = EntityState.Modified;
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
                return new GenericResponse { Status = false, Message = request.Activate ? "Failed to activate Discount" : "Failed to deactivate Discount" };

            // Log to StoreOwnerActivityLogs {Hangfire}
            await _dataContext.StoreOwnerActivityLogs.AddAsync(new StoreOwnerActivityLog { DiscountId = configuredDiscountExist.Id, StoreOwnerId = request.StoreOwnerId, Action = request.Activate ? StoreOwnerActionsEnum.ACTIVATE_DISCOUNT.ToString() : StoreOwnerActionsEnum.DEACTIVATE_DISCOUNT.ToString(), DateOfAction = DateTime.Now });

            return new GenericResponse { Status = true, Message = "Success" };
        }

        public async Task<List<Coupon>> GetAllStoreDiscountsAsync(Guid StoreOwnerId, Guid StoreId, PaginationFilter paginationFilter = null)
        {
            List<Coupon> allDiscounts = null;
            if (paginationFilter == null)
            {
                allDiscounts = await _dataContext.Coupons.Where(c => c.IsStoreOwnerConfigured && c.StoreOwnerId == StoreOwnerId && c.StoreId == StoreId).ToListAsync<Coupon>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allDiscounts = await _dataContext.Coupons.Where(c => c.IsStoreOwnerConfigured && c.StoreOwnerId == StoreOwnerId && c.StoreId == StoreId).Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
            }
            return allDiscounts;
        }

        public async Task<List<Coupon>> GetAllStoreOwnerDiscountsAsync(Guid StoreOwnerId, PaginationFilter paginationFilter = null)
        {
            List<Coupon> allDiscounts = null;
            if (paginationFilter == null)
            {
                allDiscounts = await _dataContext.Coupons.Where(c => c.IsStoreOwnerConfigured && c.StoreOwnerId == StoreOwnerId).ToListAsync<Coupon>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allDiscounts = await _dataContext.Coupons.Where(c => c.IsStoreOwnerConfigured && c.StoreOwnerId == StoreOwnerId).Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
            }
            return allDiscounts;
        }

        public async Task<GenericResponse> GetDiscountByIdAsync(Guid StoreOwnerId, Guid Id)
        {
            var discount = await _dataContext.Coupons.FirstOrDefaultAsync(c => c.Id == Id && c.StoreOwnerId == StoreOwnerId);
            if (discount == null)
                return new GenericResponse { Status = false, Message = "Invalid Discount" };

            return new GenericResponse { Status = true, Message = "Success", Data = discount };
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

        public async Task<GenericResponse> ValidateStoreOwner(Guid storeOwnerId)
        {
            var storeOwnerExist = await _dataContext.StoreOwners.FirstOrDefaultAsync(s => s.Id == storeOwnerId);
            if (storeOwnerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Storeowner." };
            if (!storeOwnerExist.IsVerified)
                return new GenericResponse { Status = false, Message = "Please verify with OTP sent to your phone/email." };

            return new GenericResponse { Status = true, Message = "Valid" };
        }

        public string GenerateCouponCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
