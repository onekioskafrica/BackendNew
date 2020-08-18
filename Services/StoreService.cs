using Amazon.S3.Model.Internal.MarshallTransformations;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Data;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.Helpers;
using OK_OnBoarding.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public class StoreService : IStoreService
    {
        private static Random random = new Random();
        private readonly AppSettings _appSettings;
        private readonly IAwsS3UploadService _s3UploadService;
        private readonly AwsS3BucketOptions _s3BucketOptions;
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public StoreService(AppSettings appSettings, IAwsS3UploadService s3UploadService, AwsS3BucketOptions s3BucketOptions, DataContext dataContext, IMapper mapper)
        {
            _appSettings = appSettings;
            _s3UploadService = s3UploadService;
            _s3BucketOptions = s3BucketOptions;
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<GenericResponse> CreateStoreBasicAsync(Store store, IFormFile logo)
        {
            GenericResponse response = new GenericResponse();
            if(string.IsNullOrWhiteSpace(store.StoreOwnerId.ToString()) || string.IsNullOrWhiteSpace(store.StoreName) || string.IsNullOrWhiteSpace(store.StorePhoneNumber) || string.IsNullOrWhiteSpace(store.StoreEmailAddress) || string.IsNullOrWhiteSpace(store.StoreIntro))
                return new GenericResponse { Status = false, Message = "StoreownerId, StoreName, StorePhoneNumber, StoreEmailAddress and StoreIntro cannot be empty" };

            var storeOwnerExist = await _dataContext.StoreOwners.FirstOrDefaultAsync(s => s.Id == store.StoreOwnerId);
            if (storeOwnerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid StoreOwner" };
            if (!storeOwnerExist.IsVerified)
                return new GenericResponse { Status = false, Message = "Please verify with the OTP sent to your phone." };

            store.StoreId = GenerateStoreId(_appSettings.LengthOfStoreId);
            store.DateCreated = DateTime.Now;
            store.IsActivated = false;

            string logoUrl = string.Empty;
            if(logo != null)
            {
                logoUrl = await _s3UploadService.UploadFileAsync(logo.OpenReadStream(), logo.FileName, _s3BucketOptions.StoreLogoFolderName);
            }

            store.LogoUrl = logoUrl;

            await _dataContext.Stores.AddAsync(store);
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
                return new GenericResponse { Status = false, Message = "Failed to create store." };

            // Send Mail to StoreOwner {Hangfire}

            // Add to StoreOwnerActivityLog Table {Hangfire}
            await _dataContext.StoreOwnerActivityLogs.AddAsync(new StoreOwnerActivityLog { StoreId = store.Id, StoreOwnerId = store.StoreOwnerId, DateOfAction = DateTime.Now, Action = StoreOwnerActionsEnum.STORE_CREATION.ToString() });
            await _dataContext.SaveChangesAsync();

            var storeResponseData = _mapper.Map<StoreCreationDataResponse>(store);
            response.Data = storeResponseData;

            if (logoUrl == string.Empty)
            {
                response.Status = true;
                response.Message = "Successfully created store but logo was not uploaded.";
            }
            else
            {
                response.Status = true;
                response.Message = "Successfully created store.";
            }
            return response;
        }

        public async Task<GenericResponse> UploadStoreBankDetailsAsync(StoresBankAccount storesBankAccount, Guid StoreOwnerId)
        {
            GenericResponse response = new GenericResponse();
            if (string.IsNullOrWhiteSpace(storesBankAccount.Bank) || string.IsNullOrWhiteSpace(storesBankAccount.StoreId.ToString()) || string.IsNullOrWhiteSpace(storesBankAccount.AccountName) || string.IsNullOrWhiteSpace(storesBankAccount.AccountNumber))
                return new GenericResponse { Status = false, Message = "Bank, StoreId, Account Name, Account Number cannot be empty." };

            var storeExist = await _dataContext.Stores.FirstOrDefaultAsync(s => s.Id == storesBankAccount.StoreId);
            if (storeExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Store." };
            if(storeExist.StoreOwnerId != StoreOwnerId)
                return new GenericResponse { Status = false, Message = "This Store doesnt't belong to this storeowner." };

            await _dataContext.StoresBankAccounts.AddAsync(storesBankAccount);
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
                return new GenericResponse { Status = false, Message = "Failed to add store bank info." };

            // Add to StoreOwnerActivityLog Table {Hangfire}
            await _dataContext.StoreOwnerActivityLogs.AddAsync(new StoreOwnerActivityLog { StoreId = storesBankAccount.StoreId, StoreOwnerId = storeExist.StoreOwnerId, DateOfAction = DateTime.Now, Action = StoreOwnerActionsEnum.ADDED_BANK_INFO.ToString() });
            await _dataContext.SaveChangesAsync();

            var storeBankAccountInfo = _mapper.Map<StoresBankAccount>(storesBankAccount);

            response.Status = true;
            response.Data = storeBankAccountInfo;
            response.Message = "Successfully added store bank information";
            return response;
        }

        public async Task<GenericResponse> UploadStoreBusinessInfoAsync(StoresBusinessInformation storesBusiness, IFormFile vatInfoFile, Guid StoreOwnerId)
        {
            GenericResponse response = new GenericResponse();
            if (string.IsNullOrWhiteSpace(storesBusiness.StoreId.ToString()) || string.IsNullOrWhiteSpace(storesBusiness.Line1) || string.IsNullOrWhiteSpace(storesBusiness.City) || string.IsNullOrWhiteSpace(storesBusiness.State) || string.IsNullOrWhiteSpace(storesBusiness.Country) || string.IsNullOrWhiteSpace(storesBusiness.PersonInCharge))
                return new GenericResponse { Status = false, Message = "StoreId, Address Line1, City, State, Country and PersonInCharge cannot be empty" };

            var storeExist = await _dataContext.Stores.FirstOrDefaultAsync(s => s.Id == storesBusiness.StoreId);
            if (storeExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Store." };
            if (storeExist.StoreOwnerId != StoreOwnerId)
                return new GenericResponse { Status = false, Message = "This Store doesnt't belong to this storeowner." };

            string vatFileUrl = string.Empty;
            if (vatInfoFile != null)
            {
                vatFileUrl = await _s3UploadService.UploadFileAsync(vatInfoFile.OpenReadStream(), vatInfoFile.FileName, _s3BucketOptions.CredentialsFolderName);
            }
            
            storesBusiness.VatInformationFileUrl = vatFileUrl;

            await _dataContext.StoresBusinessInformation.AddAsync(storesBusiness);
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
                return new GenericResponse { Status = false, Message = "Failed to add store business info." };

            // Add to StoreOwnerActivityLog Table {Hangfire}
            await _dataContext.StoreOwnerActivityLogs.AddAsync(new StoreOwnerActivityLog { StoreId = storesBusiness.StoreId, StoreOwnerId = storeExist.StoreOwnerId, DateOfAction = DateTime.Now, Action = StoreOwnerActionsEnum.ADDED_BUSINESS_INFO.ToString() });
            await _dataContext.SaveChangesAsync();

            var storeBusinessInfoData = _mapper.Map<StoresBusinessInfoDataResponse>(storesBusiness);

            if (vatFileUrl == string.Empty)
            {
                response.Status = true;
                response.Message = "Successfully added store business info but vat file was not uploaded.";
            }
            else
            {
                response.Status = true;
                response.Message = "Successfully added store business info.";
            }
            return response;
        }

        public async Task<GenericResponse> ReviewStoreAsync(ReviewStoreRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerId.ToString()) || string.IsNullOrWhiteSpace(request.StoreId.ToString()) || string.IsNullOrWhiteSpace(request.Content))
                return new GenericResponse { Status = false, Message = "CustomerId, StoreId and Content cannot be empty." };

            if (request.Rating > 5 || request.Rating < 0)
                return new GenericResponse { Status = false, Message = "Rating must be within 0-5" };

            var customerExist = await _dataContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId);
            if (customerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Customer." };
            if (!customerExist.IsVerified)
                return new GenericResponse { Status = false, Message = "Please verify with OTP sent to your phone." };

            var storeExist = await _dataContext.Stores.FirstOrDefaultAsync(s => s.Id == request.StoreId);
            if (storeExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Store" };
            if (storeExist.IsClosed || !storeExist.IsActivated)
                return new GenericResponse { Status = false, Message = "Store is closed/inactive" };

            var storeReview = _mapper.Map<StoreReview>(request);
            storeReview.IsPublished = false;
            storeReview.CreatedAt = DateTime.Now;

            await _dataContext.StoreReviews.AddAsync(storeReview);
            var created = 0;
            try
            {
                created = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Could not review store." };
            }
            if (created <= 0)
                return new GenericResponse { Status = false, Message = "Error Occurred." };

            storeReview.Customer.PasswordHash = null;
            storeReview.Customer.PasswordSalt = null;
            storeReview.Customer.Carts = null;
            storeReview.Customer.Orders = null;

            return new GenericResponse { Status = true, Message = "Reviewed Successfully.", Data = storeReview };
        }

        public async Task<GenericResponse> GetStoreByIdAsync(Guid storeId)
        {
            var store = await _dataContext.Stores.Include(s => s.StoresBankAccount).Include(s => s.StoresBusinessInformation).Include(s => s.StoreOwner).FirstOrDefaultAsync(s => s.Id == storeId);
            if (store == null)
                return new GenericResponse { Status = false, Message = "Invalid Store Id." };
            store.StoreOwner.PasswordHash = null;
            store.StoreOwner.PasswordSalt = null;

            return new GenericResponse { Status = true, Message = "Success", Data = store };
        }

        public async Task<GenericResponse> GetStoreByStoreIdAsync(string storeId)
        {
            var store = await _dataContext.Stores.Include(s => s.StoresBankAccount).Include(s => s.StoresBusinessInformation).Include(s => s.StoreOwner).FirstOrDefaultAsync(s => s.StoreId == storeId);
            if (store == null)
                return new GenericResponse { Status = false, Message = "Invalid Store Id." };
            store.StoreOwner.PasswordHash = null;
            store.StoreOwner.PasswordSalt = null;

            return new GenericResponse { Status = true, Message = "Success", Data = store };
        }

        public async Task<List<Store>> GetAllStoresByStoreOwnerIdAsync(Guid storeOwnerId, PaginationFilter paginationFilter = null)
        {
            List<Store> allStores = null;
            if(paginationFilter == null)
            {
                allStores = await _dataContext.Stores.Where(s => s.StoreOwnerId == storeOwnerId).Include(s => s.StoresBankAccount).Include(s => s.StoresBusinessInformation).ToListAsync<Store>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allStores = await _dataContext.Stores.Where(s => s.StoreOwnerId == storeOwnerId).Include(s => s.StoresBankAccount).Include(s => s.StoresBusinessInformation).Skip(skip).Take(paginationFilter.PageSize).ToListAsync<Store>();
            }
            return allStores;
        }

        public async Task<List<Store>> GetAllStoresAsync(PaginationFilter paginationFilter = null)
        {
            List<Store> allStores = null;
            if (paginationFilter == null)
            {
                allStores = await _dataContext.Stores.Include(s => s.StoresBankAccount).Include(s => s.StoresBusinessInformation).ToListAsync<Store>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allStores = await _dataContext.Stores.Skip(skip).Take(paginationFilter.PageSize).Include(s => s.StoresBankAccount).Include(s => s.StoresBusinessInformation).ToListAsync<Store>();
            }
            return allStores;
        }

        public async Task<List<StoreReview>> GetStoreReviewsAsync(Guid storeId, PaginationFilter paginationFilter = null)
        {
            List<StoreReview> allStoreReviews = null;
            if (paginationFilter == null)
            {
                allStoreReviews = await _dataContext.StoreReviews.Where(s => s.StoreId == storeId && s.IsPublished).ToListAsync<StoreReview>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allStoreReviews = await _dataContext.StoreReviews.Where(s => s.StoreId == storeId && s.IsPublished).Skip(skip).Take(paginationFilter.PageSize).ToListAsync<StoreReview>();
            }
            return allStoreReviews;
        }

        public string GenerateStoreId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
