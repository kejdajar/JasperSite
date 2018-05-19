using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            SettingsViewModel model = new SettingsViewModel();
            model.WebsiteName = Configuration.DbHelper.GetWebsiteName();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SettingsViewModel model)
        {
            if(ModelState.IsValid)
            {
                Configuration.DbHelper.SetWebsiteName(model.WebsiteName);
            }
            return View();
        }
    }
}