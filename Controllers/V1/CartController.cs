using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using OK_OnBoarding.Contracts.V1.Requests;
using OK_OnBoarding.Contracts.V1.Requests.Queries;
using OK_OnBoarding.Contracts.V1.Responses;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using OK_OnBoarding.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Controllers.V1
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public CartController(ICartService cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
        }

        [HttpPost(ApiRoute.Cart.CreateCart)]
        public async Task<IActionResult> CreateCart([FromQuery][Required] Guid CustomerId, [FromQuery][Required] string SessionId)
        {
            var genericResponse = await _cartService.CreateCartAsync(CustomerId, SessionId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpPost(ApiRoute.Cart.AddCartItem)]
        public async Task<IActionResult> AddCartItem([FromBody] AddCartItemRequest request)
        {
            var genericResponse = await _cartService.AddCartItemAsync(request);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpPost(ApiRoute.Cart.RemoveCartItem)]
        public async Task<IActionResult> RemoveCartItem([FromQuery] int cartItemId)
        {
            var genericResponse = await _cartService.RemoveCartItemAsync(cartItemId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpPost(ApiRoute.Cart.GetCart)]
        public async Task<IActionResult> GetCart([FromQuery][Required] int cartId)
        {
            var genericResponse = await _cartService.GetCartAsync(cartId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpDelete(ApiRoute.Cart.ClearCart)]
        public async Task<IActionResult> ClearCart([FromQuery][Required] int cartId)
        {
            var genericResponse = await _cartService.ClearCartAsync(cartId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpGet(ApiRoute.Cart.Checkout)]
        public async Task<IActionResult> Checkout([FromQuery] CheckoutRequest request)
        {
            var genericResponse = await _cartService.CheckoutAsync(request);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

        [HttpGet(ApiRoute.Cart.GetAllCarts)]
        public async Task<IActionResult> GetAllCarts([Required] Guid customerId, PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var allCustomerCarts = await _cartService.GetAllCustomerCartsAsync(customerId, pagination);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<Cart>(allCustomerCarts));

            var paginationResponse = new PagedResponse<Cart>
            {
                Data = allCustomerCarts,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [HttpGet(ApiRoute.Cart.GetAllOrders)]
        public async Task<IActionResult> GetCustomerOrders([Required] Guid customerId, PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var allCustomerOrders = await _cartService.GetAllCustomerOrdersAsync(customerId, pagination);


            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<Order>(allCustomerOrders));

            var paginationResponse = new PagedResponse<Order>
            {
                Data = allCustomerOrders,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null
            };
            return Ok(paginationResponse);
        }

        [HttpGet(ApiRoute.Cart.GetOrder)]
        public async Task<IActionResult> GetOrder([Required] string sessionId)
        {
            var genericResponse = await _cartService.GetOrderAsync(sessionId);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }
    }
}
