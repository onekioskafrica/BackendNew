using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using OK_OnBoarding.Contracts.V1;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Entities;
using OK_OnBoarding.ExternalContract;
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
        private readonly IOTPService _otpService;

        public CustomersController(IMapper mapper, ICustomersService customersService, IOTPService otpService) {
            _mapper = mapper;
            _customersService = customersService;
            _otpService = otpService;
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
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });

        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Customer.UpdateAddress)]
        public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var genericResponse = await _customersService.UpdateAddressAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Customer.Login)]
        public async Task<IActionResult> Login([FromBody] CustomerLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _customersService.LoginCustomerAsync(request.Email, request.Password);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            }
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Customer.FacebookAuth)]
        public async Task<IActionResult> FacebookAuth([FromBody] FacebookAuthRequest request)
        {
            var authResponse = await _customersService.FacebookLoginCustomerAsync(request.AccessToken);
            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Customer.GoogleAuth)]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _customersService.GoogleLoginCustomerAsync(request);
            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [HttpPost(ApiRoute.Customer.EnableCustomerCreation)]
        public async Task<IActionResult> EnableCustomerCreation([FromBody] EnableUserCreationRequest request)
        {
            var genericResponse = await _otpService.VerifyOTPForCustomer(request.OTP, request.PhoneNumber);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }
    }
}
