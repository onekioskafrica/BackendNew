using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.ExternalContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface IStoreOwnerService
    {
        Task<GenericResponse> ResetPassword(ForgotPasswordRequest request);
        Task<AuthenticationResponse> GoogleLoginStoreOwnerAsync(GoogleAuthRequest request);
        Task<AuthenticationResponse> CreateStoreOwnerAsync(StoreOwner storeOwner, string password);
        Task<AuthenticationResponse> LoginStoreOwnerAsync(string email, string password);
        Task<AuthenticationResponse> FacebookLoginStoreOwnerAsync(string accessToken);
        Task<GenericResponse> CloseStoreAsync(CloseStoreRequest request);
        Task<GenericResponse> SetProductVisibilityAsync(SetProductVisibilityRequest request);
        Task<GenericResponse> ConfigureDiscountAsync(StoreOwnerConfigureDiscountRequest request);
        Task<GenericResponse> ActivateDiscountAsync(StoreOwnerActivateDiscountRequest request);
        Task<List<Coupon>> GetAllStoreOwnerDiscountsAsync(Guid StoreOwnerId, PaginationFilter paginationFilter = null);
        Task<List<Coupon>> GetAllStoreDiscountsAsync(Guid StoreOwnerId, Guid StoreId, PaginationFilter paginationFilter = null);
        Task<GenericResponse> GetDiscountByIdAsync(Guid StoreOwnerId, Guid Id);
    }
}
