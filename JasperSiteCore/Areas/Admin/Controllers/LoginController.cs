using JasperSiteCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            #region InstallationCheck
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
            #endregion

            return View();
        }
    }
}