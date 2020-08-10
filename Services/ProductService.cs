using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Data;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IAwsS3UploadService _s3UploadService;
        private readonly AwsS3BucketOptions _s3BucketOptions;

        public ProductService(DataContext dataContext, IMapper mapper, IAwsS3UploadService s3UploadService, AwsS3BucketOptions s3BucketOptions)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _s3UploadService = s3UploadService;
            _s3BucketOptions = s3BucketOptions;
        }

        public async Task<GenericResponse> CreateProductAsync(CreateProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.StoreId.ToString()) || string.IsNullOrWhiteSpace(request.StoreOwnerId.ToString()) || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.ProductDescription))
                return new GenericResponse { Status = false, Message = "StoreId, StoreOwnerId, Name and ProductDescription cannot be empty." };

            var validateStoreOwnerGenericResponse = await ValidateStoreOwner(request.StoreOwnerId, request.StoreId);
            if (!validateStoreOwnerGenericResponse.Status)
                return validateStoreOwnerGenericResponse;

            var product = _mapper.Map<Product>(request);
            var categoryIds = request.CategoryIds;
            var thisProductCategories = new ProductCategory[categoryIds.Length];

            for (int i = 0; i < categoryIds.Length; i++)
            {
                thisProductCategories[i] = new ProductCategory { ProductId = product.Id, CategoryId = categoryIds[i] };
            }

            product.ProductCategories = thisProductCategories;
            product.DateCreated = DateTime.Now;
            product.IsActive = true;

            var thisProductPricing = new ProductPricing();
            thisProductPricing.ProductId = product.Id;
            thisProductPricing.SellerSku = request.SellerSku;
            thisProductPricing.Price = request.Price;
            thisProductPricing.SalePrice = request.SalePrice;
            thisProductPricing.SaleStartDate = request.SaleStartDate;
            thisProductPricing.SaleEndDate = request.SaleEndDate;
            product.ProductPricing = thisProductPricing;

            await _dataContext.Products.AddAsync(product);
            
            var productCreated = 0;
            try
            {
                productCreated = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (productCreated <= 0)
                return new GenericResponse { Status = false, Message = "Failed to add product" };

            return new GenericResponse { Status = true, Message = "Product Added Successfully.", Data = product };
        }

        public async Task<GenericResponse> GetAllCategoriesAsync()
        {
            List<Category> allCategories = null;
            try
            {
                allCategories = await _dataContext.Categories.ToListAsync<Category>();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            return new GenericResponse { Status = true, Message = "Success", Data = allCategories };
            
        }

        public async Task<GenericResponse> GetCategoryByIdAsync(int categoryId)
        {
            Category category = null;
            try
            {
                category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            return new GenericResponse { Status = true, Message = "Success", Data = category };
        }

        public async Task<GenericResponse> GetProductByIdAsync(Guid productId)
        {
            if (string.IsNullOrWhiteSpace(productId.ToString()))
                return new GenericResponse { Status = false, Message = "Product ID cannot be empty." };

            var product = await _dataContext.Products.Include(p => p.ProductCategories).Include(p => p.ProductImages).Include(p => p.ProductReviews).Include(p => p.ProductPricing).Include(p => p.Store).FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return new GenericResponse { Status = false, Message = "Invalid Product Id" };

            return new GenericResponse { Status = true, Message = "Success", Data = product };
        }

        public Task<List<Product>> GetProductsByCategoryAsync(int categoryId, PaginationFilter pagination = null)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetProductsByStoreAsync(Guid storeId, PaginationFilter paginationFilter = null)
        {
            List<Product> allProductsByStore = null;
            if (paginationFilter == null)
            {
                allProductsByStore = await _dataContext.Products.Include(p => p.ProductCategories).Include(p => p.ProductImages).Include(p => p.ProductPricing).Where(p => p.StoreId == storeId && p.IsActive && p.IsVisible).ToListAsync<Product>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allProductsByStore = await _dataContext.Products.Include(p => p.ProductCategories).Include(p => p.ProductImages).Include(p => p.ProductPricing).Where(p => p.StoreId == storeId && p.IsActive && p.IsVisible).Skip(skip).Take(paginationFilter.PageSize).ToListAsync<Product>();
            }
            return allProductsByStore;
        }

        public async Task<GenericResponse> UploadProductPhotosAsync(UploadProductPhotosRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.StoreId.ToString()) || string.IsNullOrWhiteSpace(request.ProductId.ToString()) || string.IsNullOrWhiteSpace(request.StoreOwnerId.ToString()))
                return new GenericResponse { Status = false, Message = "StoreId, StoreOwnerId and ProductDescription cannot be empty." };

            if (request.ProductPhotos == null)
                return new GenericResponse { Status = false, Message = "No Product Images to upload." };

            var validateStoreOwnerGenericResponse = await ValidateStoreOwner(request.StoreOwnerId, request.StoreId);
            if (!validateStoreOwnerGenericResponse.Status)
                return validateStoreOwnerGenericResponse;

            Product product = null;
            try
            {
                product = await _dataContext.Products.Include(p => p.ProductPricing).Include(p => p.ProductCategories).FirstOrDefaultAsync(p => p.Id == request.ProductId);
                if (product == null)
                    return new GenericResponse { Status = false, Message = "Invalid ProductId" };
                if (!product.IsActive)
                    return new GenericResponse { Status = false, Message = "Cannot upload images for an inactive product." };

                if (product.StoreId != request.StoreId)
                    return new GenericResponse { Status = false, Message = "Product doesn't belong to store." };
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }

            var productPhotos = new List<ProductImage>();

            foreach(var photo in request.ProductPhotos)
            {
                var photoAwsUrl = await _s3UploadService.UploadFileAsync(photo.OpenReadStream(), photo.FileName, _s3BucketOptions.ProductFolderName);
                productPhotos.Add(new ProductImage { ProductId = request.ProductId, ImageUrl = photoAwsUrl });
            }

            await _dataContext.ProductImage.AddRangeAsync(productPhotos);
            var imageUrlsRecorded = 0;
            try
            {
                imageUrlsRecorded = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            if (imageUrlsRecorded <= 0)
                return new GenericResponse { Status = false, Message = "Error Occurred." };

            return new GenericResponse { Status = true, Message = "Uploaded Successfully.", Data = product };
        }

        public async Task<GenericResponse> ValidateStoreOwner(Guid storeOwnerId, Guid storeId)
        {
            try
            {
                var storeOwner = await _dataContext.StoreOwners.AsNoTracking().Include(s => s.Stores).FirstOrDefaultAsync(s => s.Id == storeOwnerId);
                if (storeOwner == null)
                    return new GenericResponse { Status = false, Message = "Invalid StoreOwner" };
                if (!storeOwner.IsVerified)
                    return new GenericResponse { Status = false, Message = "StoreOwner not verified." };
                var storeOwned = storeOwner.Stores.ToList().Find(s => s.Id == storeId);
                if (storeOwned == null)
                    return new GenericResponse { Status = false, Message = "Store not owned by this store owner." };
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Error Occurred." };
            }
            return new GenericResponse { Status = true, Message = "Valid StoreOwner" };
        }
    }
}
