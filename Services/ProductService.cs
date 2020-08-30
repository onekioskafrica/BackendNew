using AutoMapper;
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

            var theCategories = await _dataContext.Categories.Where(c => categoryIds.Contains(c.Id)).ToArrayAsync<Category>();
            var thisProductCategories = new ProductCategory[theCategories.Length];

            for (int i = 0; i < theCategories.Length; i++)
            {
                var thisCategory = theCategories[i];
                thisProductCategories[i] = new ProductCategory { ProductId = product.Id, CategoryId = thisCategory.Id, CategoryTitle = thisCategory.Title };
            }

            product.ProductCategories = thisProductCategories;
            product.DateCreated = DateTime.Now;
            product.IsActive = true;

            var thisProductPricing = new ProductPricing();
            thisProductPricing.ProductId = product.Id;
            thisProductPricing.SellerSku = request.SellerSku;
            thisProductPricing.Price = request.Price;
            thisProductPricing.IsSalePriceSet = request.IsSalePriceSet;
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
            if (category == null)
                return new GenericResponse { Status = false, Message = "Invalid Category ID" };
            return new GenericResponse { Status = true, Message = "Success", Data = category };
        }

        public async Task<GenericResponse> GetProductByIdAsync(Guid productId)
        {
            if (string.IsNullOrWhiteSpace(productId.ToString()))
                return new GenericResponse { Status = false, Message = "Product ID cannot be empty." };

            var product = await _dataContext.Products.Include(p => p.ProductCategories).Include(p => p.ProductImages).Include(p => p.ProductPricing).Include(p => p.Store).FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return new GenericResponse { Status = false, Message = "Invalid Product Id" };

            product.ProductReviews = null;

            return new GenericResponse { Status = true, Message = "Success", Data = product };
        }

        public async Task<List<ProductReview>> GetProductReviewAsync(Guid ProductId, PaginationFilter paginationFilter = null)
        {
            List<ProductReview> allProductReviews = null;
            if(paginationFilter == null)
            {
                allProductReviews = await _dataContext.ProductReviews.Include(p => p.Customer).Where(p => p.ProductId == ProductId && p.IsPublished).ToListAsync<ProductReview>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allProductReviews = await _dataContext.ProductReviews.Where(p => p.ProductId == ProductId && p.IsPublished).Include(p => p.Customer).Skip(skip).Take(paginationFilter.PageSize).ToListAsync<ProductReview>();
            }
            if(allProductReviews != null)
            {
                foreach(var review in allProductReviews)
                {
                    review.Customer.PasswordHash = null;
                    review.Customer.PasswordSalt = null;
                    review.Customer.Orders = null;
                    review.Customer.Carts = null;
                }
            }
            return allProductReviews;
        }

        public  async Task<List<Product>> GetProductsByCategoryAsync(int categoryId, PaginationFilter paginationFilter = null)
        {
            List<Guid> allProductIDsByCategory = null;
            List<Product> allProductsByCategory = null;
            if(paginationFilter == null)
            {
                allProductIDsByCategory = await _dataContext.ProductCategories.Where(pc => pc.CategoryId == categoryId).Select(pc => pc.ProductId).ToListAsync();
                allProductsByCategory = await _dataContext.Products.Include(p => p.ProductCategories).Include(p => p.ProductImages).Include(p => p.ProductPricing).Where(p => allProductIDsByCategory.Contains(p.Id) && p.IsActive && p.IsVisible).ToListAsync<Product>();
            }
            else
            {
                var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
                allProductIDsByCategory = await _dataContext.ProductCategories.Where(pc => pc.CategoryId == categoryId).Select(pc => pc.ProductId).ToListAsync();
                allProductsByCategory = await _dataContext.Products.Include(p => p.ProductCategories).Include(p => p.ProductImages).Include(p => p.ProductPricing).Where(p => allProductIDsByCategory.Contains(p.Id) && p.IsActive && p.IsVisible).Skip(skip).Take(paginationFilter.PageSize).ToListAsync<Product>();
            }
            return allProductsByCategory;
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

        public async Task<GenericResponse> RestockProductAsync(RestockProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.StoreOwnerId.ToString()) || string.IsNullOrWhiteSpace(request.ProductId.ToString()))
                return new GenericResponse { Status = false, Message = "StoreOwnerId and ProductId cannot be empty." };

            if (request.InStock < 0)
                return new GenericResponse { Status = false, Message = "Instock cannot be less than zero" };

            var productExist = await _dataContext.Products.Include(p => p.Store).Include(p => p.ProductPricing).FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if (productExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Product" };
            if (!productExist.IsActive)
                return new GenericResponse { Status = false, Message = "Product is not active." };
            if (productExist.Store.StoreOwnerId != request.StoreOwnerId)
                return new GenericResponse { Status = false, Message = "Product is not for storeowner." };

            //Get Old ProductPricing
            var OldPrice = productExist.ProductPricing.Price;
            var OldSalePrice = productExist.ProductPricing.SalePrice;
            var OldSaleStartDate = productExist.ProductPricing.SaleStartDate;
            var OldSaleEndDate = productExist.ProductPricing.SaleEndDate;

            productExist.InStock = productExist.InStock + request.InStock;
            productExist.ProductPricing.Price = request.Price;
            productExist.ProductPricing.SalePrice = request.SalePrice;
            productExist.ProductPricing.SaleStartDate = request.SaleStartDate;
            productExist.ProductPricing.SaleEndDate = request.SaleEndDate;

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
                return new GenericResponse { Status = false, Message = "Couldn't Restock" };

            // Log to StoreOwnerActivityLogs {Hangfire}
            await _dataContext.StoreOwnerActivityLogs.AddAsync(new StoreOwnerActivityLog { StoreId = productExist.Store.Id, StoreOwnerId = request.StoreOwnerId, Action =  StoreOwnerActionsEnum.RESTOCK_PRODUCT.ToString(), ProductId = request.ProductId, DateOfAction = DateTime.Now });

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Do Nothing
            }

            // Log to ProductPricingHistories table {Hangfire}
            await _dataContext.ProductPricingHistories.AddAsync(new ProductPricingHistory { StoreOwnerId = request.StoreOwnerId, ProductId = request.ProductId, OldPrice = OldPrice, OldSalePrice = OldSalePrice, OldSaleStartDate = OldSaleStartDate, OldSaleEndDate = OldSaleEndDate, DateOfAction = DateTime.Now});
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Do Nothing
            }

            return new GenericResponse { Status = false, Message = "Restocked Product Successfully." };
        }

        public async Task<GenericResponse> ReviewProductAsync(ReviewProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerId.ToString()) || string.IsNullOrWhiteSpace(request.ProductId.ToString()) || string.IsNullOrWhiteSpace(request.Content))
                return new GenericResponse { Status = false, Message = "CustomerId, ProductId and Content cannot be empty." };

            if (request.Rating > 5 || request.Rating < 0)
                return new GenericResponse { Status = false, Message = "Rating must be within 0-5" };

            var customerExist = await _dataContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId);
            if (customerExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Customer." };
            if (!customerExist.IsVerified)
                return new GenericResponse { Status = false, Message = "Please verify with OTP sent to your phone." };

            var productExist = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if (productExist == null)
                return new GenericResponse { Status = false, Message = "Invalid Product." };
            if(!productExist.IsVisible)
                return new GenericResponse { Status = false, Message = "Cannot review an invisible product." };
            if (!productExist.IsActive)
                return new GenericResponse { Status = false, Message = "Cannot review an inactive product." };

            var productReview =  _mapper.Map<ProductReview>(request);
            productReview.IsPublished = false;
            productReview.CreatedAt = DateTime.Now;

            await _dataContext.ProductReviews.AddAsync(productReview);
            var created = 0;
            try
            {
                created = await _dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new GenericResponse { Status = false, Message = "Could not review product." };
            }
            if (created <= 0)
                return new GenericResponse { Status = false, Message = "Error Occurred." };

            productReview.Customer.PasswordHash = null;
            productReview.Customer.PasswordSalt = null;
            productReview.Customer.Carts = null;
            productReview.Customer.Orders = null;

            return new GenericResponse { Status = true, Message = "Reviewed Successfully.", Data = productReview};

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
