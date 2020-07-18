using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Entities;
using OK_OnBoarding.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Controllers.V1
{
    [Authorize]
    public class StoreOwnerController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IStoreOwnerService _storeOwnerService;

        public StoreOwnerController(IMapper mapper, IStoreOwnerService storeOwnerService)
        {
            _mapper = mapper;
            _storeOwnerService = storeOwnerService;
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.StoreOwner.Signup)]
        public async Task<IActionResult> Signup([FromBody] StoreOwnerSignUpRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var storeOwner = _mapper.Map<StoreOwner>(model);

            var authResponse = await _storeOwnerService.CreateStoreOwnerAsync(storeOwner, model.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            }
            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }
    }
}
