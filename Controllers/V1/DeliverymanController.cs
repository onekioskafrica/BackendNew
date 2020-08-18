using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Responses;
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
    [Authorize]
    public class DeliverymanController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IDelivermanService _delivermanService;
        private readonly IOTPService _otpService;

        public DeliverymanController(IMapper mapper, IDelivermanService delivermanService, IOTPService otpService)
        {
            _mapper = mapper;
            _delivermanService = delivermanService;
            _otpService = otpService;
        }

        
        [AllowAnonymous]
        [HttpPost(ApiRoute.Deliveryman.Signup)]
        public async Task<IActionResult> Signup([FromBody] DeliverymanSignUpRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var deliveryMan = _mapper.Map<Deliveryman>(model);

            var authResponse = await _delivermanService.CreateDeliverymanAsync(deliveryMan, model.Password);

            if(!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Deliveryman.Login)]
        public async Task<IActionResult> Login([FromBody] DeliverymanLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _delivermanService.LoginDeliverymanAsync(request.Email, request.Password);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            }
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Deliveryman.GoogleAuth)]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var authResponse = await _delivermanService.GoogleLoginDeliverymanAsync(request);
            if(!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Deliveryman.FacebookAuth)]
        public async Task<IActionResult> FacebookAuth([FromBody] FacebookAuthRequest request)
        {
            var authResponse = await _delivermanService.FacebookLoginDeliverymanAsync(request.AccessToken);
            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            return Ok(new AuthSuccessResponse { Token = authResponse.Token, Data = authResponse.Data });
        }

        [HttpPost(ApiRoute.Deliveryman.EnableDeliverymanCreation)]
        public async Task<IActionResult> EnableDeliverymanCreation([FromBody] EnableUserCreationRequest request)
        {
            var genericResponse = await _otpService.VerifyOTPForDeliveryman(request.OTP, request.PhoneNumber);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);

        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Deliveryman.ResendOTP)]
        public async Task<IActionResult> ResendOTP([FromBody] ResendOTPRequest request)
        {
            var genericResponse = await _otpService.ResendOTPForDeliveryman(OTPGenerationReason.OTPGENERATION_RESEND.ToString(), request.Email);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [AllowAnonymous]
        [HttpGet(ApiRoute.Deliveryman.SendOTPForForgotPassword)]
        public async Task<IActionResult> SendOTPForForgotPassword([FromQuery] string email)
        {
            var genericResponse = await _otpService.SendOTPToDeliverymanForPasswordReset(OTPGenerationReason.OTPGENERATION_FORGOTPASSWORD.ToString(), email);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Deliveryman.ForgotPassword)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var genericResponse = await _delivermanService.ResetPassword(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpPut(ApiRoute.Deliveryman.UpdateAddress)]
        public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var genericResponse = await _delivermanService.UpdateAddressAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpPut(ApiRoute.Deliveryman.UpdateGeneralInformation)]
        public async Task<IActionResult> UpdateGeneralInformation([FromBody] DeliverymanGeneralInfoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var genericResponse = await _delivermanService.UpdateGeneralInformationAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpPut(ApiRoute.Deliveryman.UploadDoc)]
        public async Task<IActionResult> UploadDoc([FromForm] DeliverymanUploadDocumentsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var genericResponse = await _delivermanService.UploadDocumentsAsync(request);
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

    }
}
