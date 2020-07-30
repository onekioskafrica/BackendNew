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
        [HttpGet(ApiRoute.Admin.GetStoreDetailsById)]
        public async Task<IActionResult> GetStoreDetailsById([FromQuery] Guid storeId)
        {
            var genericResponse = await _adminService.GetStoreDetailsByIdAsync(storeId);

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

            var genericResponse = await _adminService.CreateAdminAsync(admin, model.Password, model.CallerId);

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
