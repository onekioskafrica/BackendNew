using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using OK_OnBoarding.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet(ApiRoute.Products.GetAllCategories)]
        public async Task<IActionResult> GetAllCategories()
        {
            var genericResponse = await _productService.GetAllCategoriesAsync();
            if (!genericResponse.Status)
                return BadRequest(genericResponse);
            return Ok(genericResponse);
        }

    }
}
