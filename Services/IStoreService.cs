using Microsoft.AspNetCore.Http;
using OK_OnBoarding.Contracts.V1.Responses;
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
        Task<GenericResponse> UploadStoreBusinessInfoAsync(StoresBusinessInformation storesBusiness, IFormFile vatInfoFile);
        Task<GenericResponse> UploadStoreBankDetailsAsync(StoresBankAccount storesBankAccount);
    }
}
