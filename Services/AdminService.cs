using Amazon.Runtime.Internal.Util;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Requests.Queries;
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
        private static Random random = new Random();
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly AppSettings _appSettings;

        public AdminService(DataContext dataContext, JwtSettings jwtSettings, IMapper mapper, AppSettings appSettings)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _jwtSettings = jwtSettings;
            _appSettings = appSettings;
        }

        public async Task<GenericResponse> ActivateAdminAsync(ActivateAdminRequest request)
        {

            var callerExist = await _dataContext.Admins.AsNoTracking().FirstOrDefaultAsync(a => a.AdminId == request.PerformerId);
            if (callerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Caller Id" };
            if (!callerExist.IsActive)
                return new GenericResponse { Status = false, Message = "Caller is not active" };
            

            var adminExist = await _dataContext.Admins.FirstOrDefaultAsync(a => a.AdminId == request.AdminId);
            if (adminExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Admin Id" };

            if (callerExist.AdminId == adminExist.AdminId)
                return new GenericResponse { Status = false, Message = "Cannot activate yourself." };

            adminExist.IsActive = request.Activate;
            _dataContext.Entry(adminExist).State = EntityState.Modified;
            var updated = 0;
            try
            {

                updated = await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (updated <= 0)
                return new GenericResponse { Status = false, Message = "Failed to activate Deliveryman" };


            // Insert into AdminActivity Log {Hangfire}
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action = request.Activate == false ? AdminActionsEnum.ADMIN_DEACTIVATE_ADMIN.ToString() : AdminActionsEnum.ADMIN_ACTIVATE_ADMIN.ToString(), AdminId = request.AdminId, ReasonOfAction = request.Reason, PerformerId = request.PerformerId, DateOfAction = DateTime.Now });
            await _dataContext.SaveChangesAsync();


            return new GenericResponse { Status = true, Message = "Success" };
        }

        public async Task<GenericResponse> ActivateDeliveryman(ActivateDeliverymanRequest request)
        {
            var deliverymanExist = await _dataContext.DeliveryMen.FirstOrDefaultAsync(d => d.Id == request.DeliverymanId);
            if (deliverymanExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Deliveryman Id" };

            var adminExist = await _dataContext.Admins.FirstOrDefaultAsync(a => a.AdminId == request.AdminId);
            if (adminExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Admin" };

            deliverymanExist.IsActive = request.Activate;
            _dataContext.Entry(deliverymanExist).State = EntityState.Modified;
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
                return new GenericResponse { Status = false, Message = request.Activate ? "Failed to activate Deliveryman" : "Failed to deactivate Deliveryman" };

            // Insert into AdminActivity Log {Hangfire}
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action = request.Activate == false ? AdminActionsEnum.ADMIN_DEACTIVATE_DELIVERYMAN.ToString() : AdminActionsEnum.ADMIN_ACTIVATE_DELIVERYMAN.ToString(), DeliverymanId = request.DeliverymanId, ReasonOfAction = request.Reason, PerformerId = request.AdminId, DateOfAction = DateTime.Now });
            await _dataContext.SaveChangesAsync();

            return new GenericResponse { Status = true, Message = "Success" };
        }

        public async Task<GenericResponse> ActivateProductAsync(ActivateProductRequest request)
        {
            var productExist = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if (productExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Store" };

            var checkAdminResponse = await CheckAdmin(request.AdminId);
            if (!checkAdminResponse.Status)
                return checkAdminResponse;

            productExist.IsActive = request.Activate;
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
            if(updated <= 0)
                return new GenericResponse { Status = false, Message = request.Activate ? "Failed to activate Product" : "Failed to deactivate Product" };

            // Insert into AdminActivity Log {Hangfire}
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action = request.Activate == false ? AdminActionsEnum.ADMIN_DEACTIVATE_PRODUCT.ToString() : AdminActionsEnum.ADMIN_ACTIVATE_PRODUCT.ToString(), ProductId = request.ProductId, ReasonOfAction = request.Reason, PerformerId = request.AdminId, DateOfAction = DateTime.Now });
            await _dataContext.SaveChangesAsync();

            return new GenericResponse { Status = true, Message = "Success" };
        }
        public async Task<GenericResponse> PublishStoreReviewAsync(PublishStoreReview request)
        {
            var storeReviewExist = await _dataContext.StoreReviews.FirstOrDefaultAsync(s => s.Id == request.StoreReviewId);
            if (storeReviewExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Store Review" };

            var checkAdminResponse = await CheckAdmin(request.AdminId);
            if (!checkAdminResponse.Status)
                return checkAdminResponse;

            storeReviewExist.IsPublished = request.ToPublish;
            storeReviewExist.PublishedAt = DateTime.Now;
            _dataContext.Entry(storeReviewExist).State = EntityState.Modified;
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
                return new GenericResponse { Status = false, Message = request.ToPublish ? "Failed to publish Store review" : "Failed to unpublish Store review" };

            // Insert into AdminActivity Log {Hangfire}
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action = request.ToPublish == false ? AdminActionsEnum.ADMIN_UNPUBLISHED_STORE_REVIEW.ToString() : AdminActionsEnum.ADMIN_PUBLISHED_STORE_REVIEW.ToString(), StoreReviewId = request.StoreReviewId, ReasonOfAction = request.Reason, PerformerId = request.AdminId, DateOfAction = DateTime.Now });
            await _dataContext.SaveChangesAsync();

            return new GenericResponse { Status = true, Message = "Success" };
        }

        public async Task<GenericResponse> PublishProductReviewAsync(PublishProductReview request)
        {
            var productReviewExist = await _dataContext.ProductReviews.FirstOrDefaultAsync(p => p.Id == request.ProductReviewId);
            if (productReviewExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Product Review" };

            var checkAdminResponse = await CheckAdmin(request.AdminId);
            if (!checkAdminResponse.Status)
                return checkAdminResponse;

            productReviewExist.IsPublished = request.ToPublish;
            productReviewExist.PublishedAt = DateTime.Now;
            _dataContext.Entry(productReviewExist).State = EntityState.Modified;
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
                return new GenericResponse { Status = false, Message = request.ToPublish ? "Failed to publish Product review" : "Failed to unpublish Product review" };

            // Insert into AdminActivity Log {Hangfire}
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action = request.ToPublish == false ? AdminActionsEnum.ADMIN_UNPUBLISH_PRODUCT_REVIEW.ToString() : AdminActionsEnum.ADMIN_PUBLISHED_PRODUCT_REVIEW.ToString(), ProductReviewId = request.ProductReviewId, ReasonOfAction = request.Reason, PerformerId = request.AdminId, DateOfAction = DateTime.Now });
            await _dataContext.SaveChangesAsync();

            return new GenericResponse { Status = true, Message = "Success" };
        }

        public async Task<GenericResponse> ActivateDiscountAsync(ActivateDiscountRequest request)
        {
            var checkAdminResponse = await CheckAdmin(request.AdminId);
            if (!checkAdminResponse.Status)
                return checkAdminResponse;

            var configuredDiscountExist = await _dataContext.Coupons.FirstOrDefaultAsync(c => c.Id == request.DiscountId);
            if (configuredDiscountExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Discount Id" };

            if(configuredDiscountExist.IsActive == request.Activate)
            {
                return new GenericResponse { Status = false, Message = configuredDiscountExist.IsActive ? "Discount is already active" : "Discount is already inactive" };
            }

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

            // Insert into AdminActivity Log {Hangfire}
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action = request.Activate == false ? AdminActionsEnum.ADMIN_DEACTIVATE_DISCOUNT.ToString() : AdminActionsEnum.ADMIN_ACTIVATE_DISCOUNT.ToString(), DiscountId = request.DiscountId, ReasonOfAction = request.Reason, PerformerId = request.AdminId, DateOfAction = DateTime.Now });
            await _dataContext.SaveChangesAsync();

            return new GenericResponse { Status = true, Message = "Success" };
        }

        public async Task<GenericResponse> ActivateStore(ActivateStoreRequest request)
        {
            var storeExist = await _dataContext.Stores.FirstOrDefaultAsync(s => s.Id == request.StoreId);
            if (storeExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Store" };

            var checkAdminResponse = await CheckAdmin(request.AdminId);
            if (!checkAdminResponse.Status)
                return checkAdminResponse;

            storeExist.IsActivated = request.Activate;
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
                return new GenericResponse { Status = false, Message = request.Activate ? "Failed to activate Store" : "Failed to deactivate Store" };

            // Insert into AdminActivity Log {Hangfire}
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action = request.Activate == false ? AdminActionsEnum.ADMIN_DEACTIVATE_STORE.ToString() : AdminActionsEnum.ADMIN_ACTIVATE_STORE.ToString(), StoreId = request.StoreId, ReasonOfAction = request.Reason, PerformerId = request.AdminId, DateOfAction = DateTime.Now });
            await _dataContext.SaveChangesAsync();

            return new GenericResponse { Status = true, Message = "Success" };
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

            //Add this action to AdminActivityLog {Hangfire}
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

        public async Task<GenericResponse> CreateAdminAsync(Admin admin, Guid callerId)
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

            //Send Mail to Admin {Hangfire} send password also

            //Log Admin Activity {Hangfire}
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
            response.Data = userData;
            return response;
        }

        public async Task<GenericResponse> ConfigureDiscountAsync(AdminConfigureDiscountRequest request)
        {
            var checkAdminResponse = await CheckAdmin(request.AdminId);
            if (!checkAdminResponse.Status)
                return checkAdminResponse;

            var utility = new OnekioskUtility();

            var validationResponse = utility.ValidateDiscountInputs(request.IsPercentageDiscount, request.IsAmountDiscount, request.IsSetPrice, request.PercentageDiscount);
            if (!validationResponse.Status)
                return validationResponse;

            var discountConfiguration = new Coupon { 
                IsStoreOwnerConfigured = false,
                StoreId = null,
                IsForAllStoresOwnByAStoreOwner = false,
                StoreOwnerId = null,
                Code = null,
                Title = request.Title,
                IsActive = request.IsActive,
                AdminId = request.AdminId,
                IsAdminConfigured = true,
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

            // Insert into AdminActivity Log {Hangfire}
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action = AdminActionsEnum.ADMIN_CONFIGURED_DISCOUNT.ToString(), DiscountId = discountConfiguration.Id, ReasonOfAction = request.ReasonForCreation, PerformerId = request.AdminId, DateOfAction = DateTime.Now });
            await _dataContext.SaveChangesAsync();

            discountConfiguration.Admin.PasswordHash = null;
            discountConfiguration.Admin.PasswordSalt = null;

            return new GenericResponse { Status = true, Message = "Success", Data = discountConfiguration };
        }


        public async Task<GenericResponse> CreateProductCategoryAsync(Category category, Guid adminId)
        {
            GenericResponse response = new GenericResponse();

            if (string.IsNullOrWhiteSpace(adminId.ToString()) || string.IsNullOrWhiteSpace(category.Title))
                return new GenericResponse { Status = false, Message = "AdminId and Title cannot be empty." };

            var adminExist = await _dataContext.Admins.FirstOrDefaultAsync(a => a.AdminId == adminId);

            if (adminExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Admin Id" };

            if (!adminExist.IsActive)
                return new GenericResponse { Status = false, Message = "Inactive Admin" };

            if(category.ParentId != 0)
            {
                var parentCategoryExist = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == category.ParentId);
                if (parentCategoryExist == null)
                    return new GenericResponse { Status = false, Message = "Invalid Parent Category." };
            }
                

            var categoryExist = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Title.ToLower() == category.Title.ToLower());
            if (categoryExist != null)
                return new GenericResponse { Status = false, Message = "Category already exists." };

            await _dataContext.Categories.AddAsync(category);
            var created = 0;
            try
            {
                created = await _dataContext.SaveChangesAsync();
            }
            catch(Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred" };
            }
            if (created <= 0)
                return new GenericResponse { Status = false, Message = "Failed to add category." };

            //Log Admin Activity {Hangfire}
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { PerformerId = adminId, Action = AdminActionsEnum.ADMIN_ADDED_PRODUCT_CATEGORY.ToString(), ProductCategoryId = category.Id, DateOfAction = DateTime.Now });
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Do nothing
            }

            return new GenericResponse { Status = true, Message = "Category Successfully added.", Data = category };
        }

        public async Task<GenericResponse> GetAdminDetailsByIdAsync(Guid AdminId)
        {
            var adminExist = await _dataContext.Admins.FirstOrDefaultAsync(a => a.AdminId == AdminId);

            if (adminExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Admin Id" };

            var adminResponse = _mapper.Map<AdminResponse>(adminExist);

            return new GenericResponse { Status = true, Data = adminResponse };
        }

        public async Task<List<DeliverymanResponse>> GetAllActivatedDeliverymenAsync(PaginationFilter paginationFilter = null)
        {
            List<Deliveryman> allActivatedDeliverymen = null;
            if(paginationFilter == null)
            {
                allActivatedDeliverymen = await _dataContext.DeliveryMen.Where(d => d.IsActive == true).ToListAsync<Deliveryman>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allActivatedDeliverymen = await _dataContext.DeliveryMen.Skip(skip).Take(paginationFilter.PageSize).Where(d => d.IsActive == true).ToListAsync<Deliveryman>();
            }
            var allDeliverymenResponse = _mapper.Map<List<DeliverymanResponse>>(allActivatedDeliverymen);
            return allDeliverymenResponse;
        }

        public async Task<List<Store>> GetAllActivatedStoresAsync(PaginationFilter paginationFilter = null)
        {
            List<Store> allActivatedStores = null;
            if(paginationFilter == null)
            {
                allActivatedStores = await _dataContext.Stores.Where(s => s.IsActivated == true).ToListAsync<Store>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allActivatedStores = await _dataContext.Stores.Skip(skip).Take(paginationFilter.PageSize).Where(s => s.IsActivated == true).ToListAsync();
            }
            
            return allActivatedStores;
        }

        public async Task<List<AdminResponse>> GetAllAdminsAsync(PaginationFilter paginationFilter = null)
        {
            List<Admin> allAdmins = null;
            if (paginationFilter == null)
            {
                allAdmins = await _dataContext.Admins.ToListAsync<Admin>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allAdmins = await _dataContext.Admins.Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
            }
            var allAdminResponse = _mapper.Map<List<AdminResponse>>(allAdmins);
            return allAdminResponse;
        }

        public async Task<List<DeliverymanResponse>> GetAllDeliverymenAsync(PaginationFilter paginationFilter = null)
        {
            List<Deliveryman> allDeliverymen = null;

            if (paginationFilter == null)
            {
                allDeliverymen = await _dataContext.DeliveryMen.ToListAsync<Deliveryman>();
            }
            else 
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allDeliverymen = await _dataContext.DeliveryMen.Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
            }
            var allDeliverymenResponse = _mapper.Map<List<DeliverymanResponse>>(allDeliverymen);
            return allDeliverymenResponse;
        }

        public async Task<List<ProductReview>> GetAllProductReviewsAsync(Guid ProductId, PaginationFilter paginationFilter = null)
        {
            List<ProductReview> allProductReviews = null;

            if (paginationFilter == null)
            {
                allProductReviews = await _dataContext.ProductReviews.ToListAsync<ProductReview>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allProductReviews = await _dataContext.ProductReviews.Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
            }
            return allProductReviews;
        }

        public async Task<List<StoreReview>> GetAllStoreReviewsAsync(Guid StoreId, PaginationFilter paginationFilter = null)
        {
            List<StoreReview> allStoreReviews = null;

            if (paginationFilter == null)
            {
                allStoreReviews = await _dataContext.StoreReviews.ToListAsync<StoreReview>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allStoreReviews = await _dataContext.StoreReviews.Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
            }
            return allStoreReviews;
        }

        public async Task<List<StoreReview>> GetUnpublishedStoreReviewsAsync(PaginationFilter paginationFilter = null)
        {
            List<StoreReview> unpublishedStoreReviews = null;
            if(paginationFilter == null)
            {
                unpublishedStoreReviews = await _dataContext.StoreReviews.Where(s => s.IsPublished == false).ToListAsync<StoreReview>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                unpublishedStoreReviews = await _dataContext.StoreReviews.Skip(skip).Take(paginationFilter.PageSize).Where(s => s.IsPublished == false).ToListAsync();
            }
            return unpublishedStoreReviews;
        }

        public async Task<List<ProductReview>> GetUnpublishedProductReviewsAsync(PaginationFilter paginationFilter = null)
        {
            List<ProductReview> unpublishedProductReviews = null;
            if (paginationFilter == null)
            {
                unpublishedProductReviews = await _dataContext.ProductReviews.Where(p => p.IsPublished == false).ToListAsync<ProductReview>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                unpublishedProductReviews = await _dataContext.ProductReviews.Skip(skip).Take(paginationFilter.PageSize).Where(p => p.IsPublished == false).ToListAsync();
            }
            return unpublishedProductReviews;
        }

        public async Task<List<Product>> GetAllProductsAsync(PaginationFilter paginationFilter = null)
        {
            List<Product> allProducts = null;
            if (paginationFilter == null)
            {
                allProducts = await _dataContext.Products.Include(p => p.ProductCategories).Include(p => p.ProductImages).Include(p => p.ProductPricing).ToListAsync<Product>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allProducts = await _dataContext.Products.Skip(skip).Take(paginationFilter.PageSize).Include(p => p.ProductCategories).Include(p => p.ProductImages).Include(p => p.ProductPricing).ToListAsync<Product>();
            }

            return allProducts;
        }

        public async Task<List<Store>> GetAllStoresAsync(PaginationFilter paginationFilter = null)
        {
            List<Store> allStores = null;

            if(paginationFilter == null)
            {
                allStores = await _dataContext.Stores.ToListAsync<Store>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allStores = await _dataContext.Stores.Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
            }

            return allStores;
        }

        public async Task<List<Coupon>> GetAllAdminConfiguredDiscountsAsync(PaginationFilter paginationFilter = null)
        {
            List<Coupon> allDiscounts = null;
            if(paginationFilter == null)
            {
                allDiscounts = await _dataContext.Coupons.Where(c => c.IsAdminConfigured).ToListAsync<Coupon>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allDiscounts = await _dataContext.Coupons.Where(c => c.IsAdminConfigured).Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
            }
            return allDiscounts;
        }

        public async Task<List<Coupon>> GetAllStoreOwnerConfiguredDiscountsAsync(PaginationFilter paginationFilter = null)
        {
            List<Coupon> allDiscounts = null;
            if (paginationFilter == null)
            {
                allDiscounts = await _dataContext.Coupons.Where(c => c.IsStoreOwnerConfigured).ToListAsync<Coupon>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allDiscounts = await _dataContext.Coupons.Where(c => c.IsStoreOwnerConfigured).Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
            }
            return allDiscounts;
        }

        public async Task<List<DeliverymanResponse>> GetAllUnActivatedDeliverymenAsync(PaginationFilter paginationFilter = null)
        {
            List<Deliveryman> allUnactivatedDeliveryMen = null;
            if(paginationFilter == null)
            {
                allUnactivatedDeliveryMen = await _dataContext.DeliveryMen.Where(d => d.IsActive == false).ToListAsync<Deliveryman>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allUnactivatedDeliveryMen = await _dataContext.DeliveryMen.Skip(skip).Take(paginationFilter.PageSize).Where(d => d.IsActive == false).ToListAsync<Deliveryman>();
            }
            var allDeliverymenResponse = _mapper.Map<List<DeliverymanResponse>>(allUnactivatedDeliveryMen);
            return allDeliverymenResponse;
        }

        public async Task<List<Store>> GetAllUnActivatedStoresAsync(PaginationFilter paginationFilter = null)
        {
            List<Store> allUnactivatedStores = null;
            if(paginationFilter == null)
            {
                allUnactivatedStores = await _dataContext.Stores.Where(s => s.IsActivated == false).ToListAsync<Store>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allUnactivatedStores = await _dataContext.Stores.Skip(skip).Take(paginationFilter.PageSize).Where(s => s.IsActivated == false).ToListAsync<Store>();
            }
            return allUnactivatedStores;
        }

        public async Task<GenericResponse> GetStoreReviewByIdAsync(Guid StoreReviewId)
        {
            var storeReviewExist = await _dataContext.StoreReviews.FirstOrDefaultAsync(s => s.Id == StoreReviewId);

            if (storeReviewExist == null)
                return new GenericResponse { Status = false, Message = "Invalid StoreReview Id" };


            return new GenericResponse { Status = true, Data = storeReviewExist };
        }

        public async Task<GenericResponse> GetProductReviewByIdAsync(Guid productReviewId)
        {
            var productReviewExist = await _dataContext.ProductReviews.FirstOrDefaultAsync(p => p.Id == productReviewId);

            if (productReviewExist == null)
                return new GenericResponse { Status = false, Message = "Invalid ProductReview Id" };


            return new GenericResponse { Status = true, Data = productReviewExist };
        }

        public async Task<GenericResponse> GetDeliverymanDetailsByIdAsync(Guid deliverymanId)
        {
            var deliverymanExist = await _dataContext.DeliveryMen.FirstOrDefaultAsync(d => d.Id == deliverymanId);

            if (deliverymanExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Deliveryman Id" };

            var deliverymanResponse = _mapper.Map<DeliverymanResponse>(deliverymanExist);

            return new GenericResponse { Status = true, Data = deliverymanResponse };
        }

        public async Task<GenericResponse> GetStoreDetailsByIdAsync(Guid storeId)
        {
            
            var storedetails = await _dataContext.Stores.Include(s => s.StoresBankAccount).Include(s => s.StoresBusinessInformation).Where(s => s.Id == storeId).FirstOrDefaultAsync();

            if (storedetails == null)
                return new GenericResponse { Status = false, Message = "Invalid StoreId." };

            return new GenericResponse { Status = true, Message = "Success", Data = storedetails };
        }

        public async Task<GenericResponse> GetDiscountByIdAsync(Guid Id)
        {
            var discount = await _dataContext.Coupons.FirstOrDefaultAsync(c => c.Id == Id);
            if (discount == null)
                return new GenericResponse { Status = false, Message = "Invalid Discount" };

            return new GenericResponse { Status = true, Message = "Success", Data = discount };
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

            // Add activity to admin log {Hangfire}
            await _dataContext.AdminActivityLogs.AddAsync(new AdminActivityLog { Action =  AdminActionsEnum.LOGIN.ToString(), DateOfAction = DateTime.Now, PerformerId = admin.AdminId } );
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

        public async Task<GenericResponse> CheckAdmin(Guid AdminId)
        {
            var adminExist = await _dataContext.Admins.FirstOrDefaultAsync(a => a.AdminId == AdminId);
            if (adminExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Admin" };
            if (!adminExist.IsActive)
                return new GenericResponse { Status = false, Message = "Inactive Admin" };

            return new GenericResponse { Status = true, Message = "Success" };
        }

        public string GenerateAdminPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%&*()_-+=";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
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
