using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Net;

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
            return View(UpdatePage());

        }

        public SettingsViewModel UpdatePage()
        {
            SettingsViewModel model = new SettingsViewModel();
            try
            {
               
                model.WebsiteName = _dbHelper.GetWebsiteName();
                model.JasperJson = Configuration.GlobalWebsiteConfig.GetGlobalJsonFileAsString();
                return model;
            }
            catch (Exception) 
            {
                model.WebsiteName = string.Empty;
                model.JasperJson = string.Empty;
            }

            return model;
        }

        [HttpPost]
        public IActionResult SaveJasperJson(SettingsViewModel model)
        {

            string oldSettingsBackup = Configuration.GlobalWebsiteConfig.GetGlobalJsonFileAsString();

            try
            {
             
                    Configuration.GlobalWebsiteConfig.SaveGlobalJsonFileAsString(model.JasperJson);
                    JasperSiteCore.Models.Configuration.Initialize();

                return View("Index",UpdatePage());
            }
            catch
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = "Při ukládání konfiguračního nastavení došlo k chybě. Změny nebyly provedeny.";

                // restore old settings
                Configuration.GlobalWebsiteConfig.SaveGlobalJsonFileAsString(oldSettingsBackup);

                ModelState.Clear();// !!! without this statement model binding won't work...
                return View("Index",UpdatePage());
            }
        }


        [HttpPost]
        public IActionResult SaveSettings(SettingsViewModel model)
        {
            bool isAjaxRequest = (Request.Headers["X-Requested-With"] == "XMLHttpRequest") ? true : false;
            
            try
            {
                if (ModelState.IsValid)
                {
                    _dbHelper.SetWebsiteName(model.WebsiteName);
                    TempData["success"] = true;
                }
                else
                {
                    throw new Exception();
                }                         
                            

            }
            catch
            {
                TempData["ErrorMessage"] = "Změny nemohly být provedeny";
            }

            if(isAjaxRequest)
            {
                ModelState.Clear();
                return PartialView("WebsiteNamePartialView",UpdatePage());
            }
            else
            {
               
                return View("Index", UpdatePage());
            }
        }       
    }
}