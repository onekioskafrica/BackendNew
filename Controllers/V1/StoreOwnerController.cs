using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Requests.Queries;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.ExternalContract;
using OK_OnBoarding.Helpers;
using OK_OnBoarding.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StoreOwnerController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IStoreOwnerService _storeOwnerService;
        private readonly IOTPService _otpService;

        public StoreOwnerController(IMapper mapper, IStoreOwnerService storeOwnerService, IOTPService otpService)
        {
            _mapper = mapper;
            _storeOwnerService = storeOwnerService;
            _otpService = otpService;
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
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.StoreOwner.Login)]
        public async Task<IActionResult> Login([FromBody] StoreOwnerLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _storeOwnerService.LoginStoreOwnerAsync(request.Email, request.Password);
            if(!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });

            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.StoreOwner.GoogleAuth)]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _storeOwnerService.GoogleLoginStoreOwnerAsync(request);
            if(!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.StoreOwner.FacebookAuth)]
        public async Task<IActionResult> FacebookAuth([FromBody] FacebookAuthRequest request)
        {
            var authResponse = await _storeOwnerService.FacebookLoginStoreOwnerAsync(request.AccessToken);
            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [AllowAnonymous]
        [HttpGet(ApiRoute.StoreOwner.SendOTPForForgotPassword)]
        public async Task<IActionResult> SendOTPForForgotPassword([FromQuery] string email)
        {
            var genericResponse = await _otpService.SendOTPToStoreOwnerForPasswordReset(OTPGenerationReason.OTPGENERATION_FORGOTPASSWORD.ToString(), email);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.StoreOwner.ForgotPassword)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var genericResponse = await _storeOwnerService.ResetPassword(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpPost(ApiRoute.StoreOwner.EnableStoreOwnerCreation)]
        public async Task<IActionResult> EnableStoreOwnerCreation([FromBody] EnableUserCreationRequest request)
        {
            var genericResponse = await _otpService.VerifyOTPForStoreOwner(request.OTP, request.PhoneNumber);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.StoreOwner)]
        [HttpPost(ApiRoute.StoreOwner.CloseStore)]
        public async Task<IActionResult> CloseStore([FromBody] CloseStoreRequest request)
        {
            var genericResponse = await _storeOwnerService.CloseStoreAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.StoreOwner)]
        [HttpPost(ApiRoute.StoreOwner.SetProductVisibility)]
        public async Task<IActionResult> SetProductVisibility([FromBody] SetProductVisibilityRequest request)
        {
            var genericResponse = await _storeOwnerService.SetProductVisibilityAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.StoreOwner.ResendOTP)]
        public async Task<IActionResult> ResendOTP([FromBody] ResendOTPRequest request)
        {
            var genericResponse = await _otpService.ResendOTPForStoreOwner(OTPGenerationReason.OTPGENERATION_RESEND.ToString(), request.Email);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.StoreOwner)]
        [HttpPost(ApiRoute.StoreOwner.ConfigureDiscount)]
        public async Task<IActionResult> ConfigureDiscount([FromBody] StoreOwnerConfigureDiscountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var genericResponse = await _storeOwnerService.ConfigureDiscountAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.StoreOwner)]
        [HttpPost(ApiRoute.StoreOwner.ActivateDiscount)]
        public async Task<IActionResult> ActivateDiscount([FromBody] StoreOwnerActivateDiscountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var genericResponse = await _storeOwnerService.ActivateDiscountAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [Authorize(Roles = Roles.StoreOwner)]
        [HttpGet(ApiRoute.StoreOwner.GetAllStoreDiscounts)]
        public async Task<IActionResult> GetAllStoreDiscounts([FromQuery] Guid StoreId, [FromQuery] Guid StoreOwnerId, [FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var allStoreDiscounts = await _storeOwnerService.GetAllStoreDiscountsAsync(StoreOwnerId, StoreId, pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<Coupon>(allStoreDiscounts));

            var paginationResponse = new PagedResponse<Coupon>
            {
                Data = allStoreDiscounts,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.StoreOwner)]
        [HttpGet(ApiRoute.StoreOwner.GetAllStoreOwnerDiscounts)]
        public async Task<IActionResult> GetAllStoreOwnerDiscounts([FromQuery] Guid StoreOwnerId, [FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var allStoreDiscounts = await _storeOwnerService.GetAllStoreOwnerDiscountsAsync(StoreOwnerId, pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<Coupon>(allStoreDiscounts));

            var paginationResponse = new PagedResponse<Coupon>
            {
                Data = allStoreDiscounts,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [Authorize(Roles = Roles.StoreOwner)]
        [HttpGet(ApiRoute.StoreOwner.GetDiscountById)]
        public async Task<IActionResult> GetDiscountById([FromQuery] Guid DiscountId, [FromQuery] Guid StoreOwnerId)
        {
            var genericResponse = await _storeOwnerService.GetDiscountByIdAsync(StoreOwnerId, DiscountId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

    }
}
