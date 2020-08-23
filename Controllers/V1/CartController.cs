using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using OK_OnBoarding.Contracts.V1.Requests;
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

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
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

        [HttpGet(ApiRoute.Cart.Checkout)]
        public async Task<IActionResult> Checkout([FromQuery] CheckoutRequest request)
        {
            var genericResponse = await _cartService.CheckoutAsync(request);

            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }
    }
}
