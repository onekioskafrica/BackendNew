﻿using GeoCoordinatePortable;
using Microsoft.AspNetCore.Http;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface IStoreService
    {
        Task<GenericResponse> CreateStoreBasicAsync(Store store, IFormFile logo);
        Task<GenericResponse> UploadStoreBusinessInfoAsync(StoresBusinessInformation storesBusiness, IFormFile vatInfoFile, Guid StoreOwnerId);
        Task<GenericResponse> UploadStoreBankDetailsAsync(StoresBankAccount storesBankAccount, Guid StoreOwnerId);
        Task<GenericResponse> GetStoreByIdAsync(Guid storeId);
        Task<GenericResponse> GetStoreByStoreIdAsync(string storeId);
        Task<List<Store>> GetAllStoresByStoreOwnerIdAsync(Guid storeOwnerId, PaginationFilter paginationFilter = null);
        Task<List<Store>> GetAllStoresAsync(GeoCoordinate searchCoordinate, PaginationFilter paginationFilter = null);
        Task<GenericResponse> ReviewStoreAsync(ReviewStoreRequest request);
        Task<List<StoreReview>> GetStoreReviewsAsync(Guid storeId, PaginationFilter paginationFilter = null);
    }
}
