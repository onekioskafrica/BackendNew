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

namespace OK_OnBoarding.Controllers.V1
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAdminService _adminService;

        public AdminController(IMapper mapper, IAdminService adminService)
        {
            _mapper = mapper;
            _adminService = adminService;
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Admin.Login)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _adminService.LoginAdminAsync(request.Email, request.Password);

            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });

            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAllProducts)]
        public async Task<IActionResult> GetAllProducts([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allProducts = await _adminService.GetAllProductsAsync(pagination);
            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<Product>(allProducts));

            var paginationResponse = new PagedResponse<Product>
            {
                Data = allProducts,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAllStores)]
        public async Task<IActionResult> GetAllStores([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allStores = await _adminService.GetAllStoresAsync(pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<Store>(allStores));

            var paginationResponse = new PagedResponse<Store> { 
                Data = allStores,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?) null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?) null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAllActivatedStores)]
        public async Task<IActionResult> GetAllActivatedStores([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allActivatedStores = await _adminService.GetAllActivatedStoresAsync(pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<Store>(allActivatedStores));

            var paginationResponse = new PagedResponse<Store>
            {
                Data = allActivatedStores,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAllUnActivatedStores)]
        public async Task<IActionResult> GetAllUnActivatedStores([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allUnactivatedStores = await _adminService.GetAllUnActivatedStoresAsync(pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<Store>(allUnactivatedStores));

            var paginationResponse = new PagedResponse<Store>
            {
                Data = allUnactivatedStores,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut(ApiRoute.Admin.ActivateProduct)]
        public async Task<IActionResult> ActivateProduct([FromBody] ActivateProductRequest request)
        {
            var genericResponse = await _adminService.ActivateProductAsync(request);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut(ApiRoute.Admin.ActivateStore)]
        public async Task<IActionResult> ActivateStore([FromBody] ActivateStoreRequest request)
        {
            var genericResponse = await _adminService.ActivateStore(request);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetStoreDetailsById)]
        public async Task<IActionResult> GetStoreDetailsById([FromQuery] Guid storeId)
        {
            var genericResponse = await _adminService.GetStoreDetailsByIdAsync(storeId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAllDeliverymen)]
        public async Task<IActionResult> GetAllDeliverymen([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allDeliverymen = await _adminService.GetAllDeliverymenAsync(pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<DeliverymanResponse>(allDeliverymen));

            var paginationResponse = new PagedResponse<DeliverymanResponse>
            {
                Data = allDeliverymen,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAllUnActivatedDeliveryman)]
        public async Task<IActionResult> GetAllUnActivatedDeliveryman([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allUnactivatedDeliverymen = await _adminService.GetAllUnActivatedDeliverymenAsync(pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<DeliverymanResponse>(allUnactivatedDeliverymen));

            var paginationResponse = new PagedResponse<DeliverymanResponse>
            {
                Data = allUnactivatedDeliverymen,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAllActivatedDeliveryman)]
        public async Task<IActionResult> GetAllActivatedDeliveryman([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allActivatedDeliverymen = await _adminService.GetAllActivatedDeliverymenAsync(pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<DeliverymanResponse>(allActivatedDeliverymen));

            var paginationResponse = new PagedResponse<DeliverymanResponse>
            {
                Data = allActivatedDeliverymen,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAllProductReviews)]
        public async Task<IActionResult> GetAllProductReviews([FromQuery] [Required] Guid productId, [FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allProductReviews = await _adminService.GetAllProductReviewsAsync(productId, pagination);

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

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAllStoreReviews)]
        public async Task<IActionResult> GetAllStoreReviews([FromQuery] [Required] Guid storeId, [FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allStoreReviews = await _adminService.GetAllStoreReviewsAsync(storeId, pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<StoreReview>(allStoreReviews));

            var paginationResponse = new PagedResponse<StoreReview>
            {
                Data = allStoreReviews,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetUnpublishedStoreReviews)]
        public async Task<IActionResult> GetUnpublishedStoreReviews([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allStoreReviews = await _adminService.GetUnpublishedStoreReviewsAsync(pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<StoreReview>(allStoreReviews));

            var paginationResponse = new PagedResponse<StoreReview>
            {
                Data = allStoreReviews,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetUnpublishedProductReview)]
        public async Task<IActionResult> GetUnpublishedProductReview([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);

            var allProductReviews = await _adminService.GetUnpublishedProductReviewsAsync(pagination);

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

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetStoreReviewById)]
        public async Task<IActionResult> GetStoreReviewById([FromQuery] [Required] Guid storeReviewId)
        {
            var genericResponse = await _adminService.GetStoreReviewByIdAsync(storeReviewId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetProductReviewById)]
        public async Task<IActionResult> GetProductReviewById([FromQuery] [Required] Guid productReviewId)
        {
            var genericResponse = await _adminService.GetProductReviewByIdAsync(productReviewId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetDeliverymanDetailsById)]
        public async Task<IActionResult> GetDeliverymanDetailsById([FromQuery] Guid deliverymanId)
        {
            var genericResponse = await _adminService.GetDeliverymanDetailsByIdAsync(deliverymanId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut(ApiRoute.Admin.ActivateDeliveryman)]
        public async Task<IActionResult> ActivateDeliveryman([FromBody] ActivateDeliverymanRequest request)
        {
            var genericResponse = await _adminService.ActivateDeliveryman(request);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAllAdmins)]
        public async Task<IActionResult> GetAllAdmins([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var allAdmins = await _adminService.GetAllAdminsAsync(pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<AdminResponse>(allAdmins));

            var paginationResponse = new PagedResponse<AdminResponse>
            {
                Data = allAdmins,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet(ApiRoute.Admin.GetAdminDetailsById)]
        public async Task<IActionResult> GetAdminDetailsById([FromQuery] Guid adminId)
        {
            var genericResponse = await _adminService.GetAdminDetailsByIdAsync(adminId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut(ApiRoute.Admin.ActivateAdmin)]
        public async Task<IActionResult> ActivateAdmin([FromBody] ActivateAdminRequest request)
        {
            var genericResponse = await _adminService.ActivateAdminAsync(request);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);

            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut(ApiRoute.Admin.PublishStoreReview)]
        public async Task<IActionResult> PublishStoreReview([FromBody] PublishStoreReview request)
        {
            var genericResponse = await _adminService.PublishStoreReviewAsync(request);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);

            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut(ApiRoute.Admin.PublishProductReview)]
        public async Task<IActionResult> PublishProductReview([FromBody] PublishProductReview request)
        {
            var genericResponse = await _adminService.PublishProductReviewAsync(request);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);

            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost(ApiRoute.Admin.CreateAdmin)]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var admin = _mapper.Map<Admin>(model);

            var genericResponse = await _adminService.CreateAdminAsync(admin, model.CallerId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost(ApiRoute.Admin.CreateProductCategory)]
        public async Task<IActionResult> CreateProductCategory([FromBody] CreateProductCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var category = _mapper.Map<Category>(request);

            var genericResponse = await _adminService.CreateProductCategoryAsync(category, request.AdminId);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost(ApiRoute.Admin.ChangePassword)]
        public async Task<IActionResult> ChangePassword([FromBody] AdminChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var genericResponse = await _adminService.ChangePassword(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }
    }
}
