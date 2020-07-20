using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
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
    public class DeliverymanController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IDelivermanService _delivermanService;

        public DeliverymanController(IMapper mapper, IDelivermanService delivermanService)
        {
            _mapper = mapper;
            _delivermanService = delivermanService;
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
            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
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
            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
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
            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }

        [AllowAnonymous]
        [HttpPost(ApiRoute.Deliveryman.FacebookAuth)]
        public async Task<IActionResult> FacebookAuth([FromBody] FacebookAuthRequest request)
        {
            var authResponse = await _delivermanService.FacebookLoginDeliverymanAsync(request.AccessToken);
            if (!authResponse.Success)
                return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }
    }
}
