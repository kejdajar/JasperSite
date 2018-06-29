using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}