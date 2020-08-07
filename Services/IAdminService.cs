using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Requests.Queries;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public interface IAdminService
    {
        Task<AuthenticationResponse> LoginAdminAsync(string email, string password);
        Task<GenericResponse> CreateAdminAsync(Admin admin, string password, Guid callerId);
        Task<GenericResponse> ChangePassword(AdminChangePasswordRequest request);
        Task<List<Store>> GetAllStoresAsync(PaginationFilter paginationFilter = null);
        Task<List<Store>> GetAllActivatedStoresAsync(PaginationFilter paginationFilter = null);
        Task<List<Store>> GetAllUnActivatedStoresAsync(PaginationFilter paginationFilter = null);
        Task<GenericResponse> GetStoreDetailsByIdAsync(Guid storeId);
        Task<GenericResponse> ActivateStore(ActivateStoreRequest request);
        Task<List<DeliverymanResponse>> GetAllDeliverymenAsync(PaginationFilter paginationFilter = null);
        Task<List<DeliverymanResponse>> GetAllActivatedDeliverymenAsync(PaginationFilter paginationFilter = null);
        Task<List<DeliverymanResponse>> GetAllUnActivatedDeliverymenAsync(PaginationFilter paginationFilter = null);
        Task<GenericResponse> GetDeliverymanDetailsByIdAsync(Guid deliverymanId);
        Task<GenericResponse> ActivateDeliveryman(ActivateDeliverymanRequest request);
        Task<List<AdminResponse>> GetAllAdminsAsync(PaginationFilter paginationFilter = null);
        Task<GenericResponse> GetAdminDetailsByIdAsync(Guid AdminId);
        Task<GenericResponse> ActivateAdminAsync(ActivateAdminRequest request);
        Task<GenericResponse> CreateProductCategoryAsync(Category category, Guid adminId);
    }
}
