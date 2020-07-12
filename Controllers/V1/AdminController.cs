using Microsoft.AspNetCore.Mvc;
using OK_OnBoarding.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK_OnBoarding.Controllers.V1
{
    public class AdminController : Controller
    {
        [HttpGet(ApiRoute.Admin.Get)]
        public IActionResult Get(int id)
        {
            return Ok(new { name = "Waje" });
        }
    }
}
