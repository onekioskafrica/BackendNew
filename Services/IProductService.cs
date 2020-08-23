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
    public interface IProductService
    {
        Task<GenericResponse> GetAllCategoriesAsync();
        Task<GenericResponse> GetCategoryByIdAsync(int categoryId);
        Task<GenericResponse> CreateProductAsync(CreateProductRequest request);
        Task<GenericResponse> UploadProductPhotosAsync(UploadProductPhotosRequest request);
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId, PaginationFilter pagination = null);
        Task<List<Product>> GetProductsByStoreAsync(Guid storeId, PaginationFilter pagination = null);
        Task<GenericResponse> GetProductByIdAsync(Guid productId);
        Task<GenericResponse> ReviewProductAsync(ReviewProductRequest request);
        Task<List<ProductReview>> GetProductReviewAsync(Guid ProductId, PaginationFilter pagination = null);
        Task<GenericResponse> RestockProductAsync(RestockProductRequest request);
    }
}
