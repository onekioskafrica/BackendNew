using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using OK_OnBoarding.Contracts.V1;
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
    public class CustomersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICustomersService _customersService;

        public CustomersController(IMapper mapper, ICustomersService customersService) {
            _mapper = mapper;
            _customersService = customersService;
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Customer.Signup)]
        public async Task<IActionResult> Signup([FromBody] CustomerSignupRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var customer = _mapper.Map<Customer>(model);

            var authResponse = await _customersService.CreateCustomerAsync(customer, model.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            }
            return Ok(new AuthSuccessResponse { Token = authResponse.Token });

        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Customer.Login)]
        public async Task<IActionResult> Login([FromBody] CustomerLoginRequest request)
        {
            var authResponse = await _customersService.LoginCustomerAsync(request.Email, request.Password);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            }
            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }
    }
}
