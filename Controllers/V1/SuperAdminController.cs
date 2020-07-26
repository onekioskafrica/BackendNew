using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
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
    public class SuperAdminController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISuperAdminService _superAdminService;

        public SuperAdminController(IMapper mapper, ISuperAdminService superAdminService)
        {
            _mapper = mapper;
            _superAdminService = superAdminService;
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.SuperAdmin.CreateSuperAdmin)]
        public async Task<IActionResult> CreateSuperAdmin([FromBody] CreateSuperAdminRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var superadmin = _mapper.Map<SuperAdmin>(model);

            var authResponse = await _superAdminService.CreateSuperAdminAsync(superadmin, model.Password);
            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });

            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.SuperAdmin.Login)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _superAdminService.LoginSuperAdminAsync(request.Email, request.Password);

            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });

            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPost(ApiRoute.SuperAdmin.CreateAdmin)]
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

            var genericResponse = await _superAdminService.CreateAdminAsync(admin, model.Password);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpGet(ApiRoute.SuperAdmin.GetAllPrivileges)]
        public async Task<IActionResult> GetAllPrivileges()
        {
            var genericResponse = await _superAdminService.GetAllPrivilegesAsync();

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }
    }
}
