using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Database;
using Microsoft.AspNetCore.Authorization;


namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly DbHelper _dbHelper;

        public SettingsController(DatabaseContext dbContext)
        {
            this._databaseContext = dbContext;
            this._dbHelper = new DbHelper(dbContext);
        }

        [HttpGet]
        public IActionResult Index()
        {
            SettingsViewModel model = new SettingsViewModel();

            try
            {
                model.WebsiteName = _dbHelper.GetWebsiteName();
                model.JasperJson = Configuration.GlobalWebsiteConfig.GetGlobalJsonFileAsString();
                return View(model);
            }
            catch 
            {
                model.WebsiteName = string.Empty;
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult SaveJasperJson(SettingsViewModel model)
        {
            try
            {
             
                    Configuration.GlobalWebsiteConfig.SaveGlobalJsonFileAsString(model.JasperJson);
                    JasperSiteCore.Models.Configuration.Initialize();

                return RedirectToAction("Index");
            }
            catch
            {
                // TODO: error
                return View(model);
            }
        }


        [HttpPost]
        public IActionResult SaveSettings(SettingsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _dbHelper.SetWebsiteName(model.WebsiteName);
                }

                return View(model);
            }
            catch
            {
                // TODO: error
                return View(model);
            }
        }

       
    }
}