using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Requests.Queries;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.Helpers;
using OK_OnBoarding.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet(ApiRoute.Products.GetAllCategories)]
        public async Task<IActionResult> GetAllCategories()
        {
            var genericResponse = await _productService.GetAllCategoriesAsync();
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [AllowAnonymous]
        [HttpGet(ApiRoute.Products.GetCategoryById)]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            var genericResponse = await _productService.GetCategoryByIdAsync(categoryId);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.StoreOwner)]
        [HttpPost(ApiRoute.Products.CreateProduct)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var genericResponse = await _productService.CreateProductAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpPost(ApiRoute.Products.ReviewProduct)]
        public async Task<IActionResult> ReviewProduct([FromBody] ReviewProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var genericResponse = await _productService.ReviewProductAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpGet(ApiRoute.Products.GetProductReviews)]
        public async Task<IActionResult> GetProductReviews([FromQuery] [Required] Guid productId, PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var allProductReviews = await _productService.GetProductReviewAsync(productId, pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<ProductReview>(allProductReviews));

            var paginationResponse = new PagedResponse<ProductReview>
            {
                Data = allProductReviews,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.StoreOwner)]
        [HttpPut(ApiRoute.Products.UploadProductPhotos)]
        public async Task<IActionResult> UploadProductPhotos([FromForm] UploadProductPhotosRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var genericResponse = await _productService.UploadProductPhotosAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpGet(ApiRoute.Products.GetProductById)]
        public async Task<IActionResult> GetProductById([FromQuery] [Required] Guid productId)
        {
            var genericResponse = await _productService.GetProductByIdAsync(productId);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpGet(ApiRoute.Products.GetProductsByStore)]
        public async Task<IActionResult> GetProductsByStore([FromQuery] [Required] Guid storeId, PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var allProductByStore = await _productService.GetProductsByStoreAsync(storeId, pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<Product>(allProductByStore));

            var paginationResponse = new PagedResponse<Product>
            {
                Data = allProductByStore,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [HttpGet(ApiRoute.Products.GetProductsByCategory)]
        public async Task<IActionResult> GetProductsByCategory([FromQuery] [Required] int categoryId, PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var allProductByCategory = await _productService.GetProductsByCategoryAsync(categoryId, pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<Product>(allProductByCategory));

            var paginationResponse = new PagedResponse<Product>
            {
                Data = allProductByCategory,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }
    }
}
