using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            // Firstly, the installation must have already been completed before accesing administration panel
            #region InstallationCheck
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
            #endregion

            return View();
        }

        public ActionResult Categories()
        {
            #region InstallationCheck
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
            #endregion
            return View();
        }

        public ActionResult Articles()
        {
            #region InstallationCheck
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
            #endregion
            return View();
        }

        public ActionResult Themes()
        {
            #region InstallationCheck
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
            #endregion
            return View();
        }

        public ActionResult UpdateConfiguration()
        {
            #region InstallationCheck
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
            #endregion

            JasperSiteCore.Models.Configuration.Initialize();
            return View("Themes");
        }

      

    }
}