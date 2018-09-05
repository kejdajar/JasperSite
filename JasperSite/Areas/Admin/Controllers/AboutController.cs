using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace JasperSite.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
               
    }
}