using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Categories()
        {
            return View();
        }

        public ActionResult Articles()
        {
            return View();
        }
    }
}