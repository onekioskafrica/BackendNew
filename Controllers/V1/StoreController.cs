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
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IMapper _mapper;

        public StoreController(IStoreService storeService, IMapper mapper)
        {
            _storeService = storeService;
            _mapper = mapper;
        }

        [HttpPost(ApiRoute.Store.CreateStoreBasic)]
        public async Task<IActionResult> CreateStoreBasic([FromForm] CreateStoreBasicRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            var store = _mapper.Map<Store>(request);
            var genericResponse = await _storeService.CreateStoreBasicAsync(store, request.Logo);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpPost(ApiRoute.Store.UploadStoreBusinessInfo)]
        public async Task<IActionResult> UploadStoreBusinessInfo([FromForm] CreateStoreBusinessInfoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var storeBizInfo = _mapper.Map<StoresBusinessInformation>(request);
            var genericResponse = await _storeService.UploadStoreBusinessInfoAsync(storeBizInfo, request.VatInformationFile);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpPost(ApiRoute.Store.UploadStoreBankDetails)]
        public async Task<IActionResult> UploadStoreBankDetails([FromBody] CreateStoreBankAccountInformationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }

            var storeBankAcct = _mapper.Map<StoresBankAccount>(request);
            var genericResponse = await _storeService.UploadStoreBankDetailsAsync(storeBankAcct);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }
    }
}
