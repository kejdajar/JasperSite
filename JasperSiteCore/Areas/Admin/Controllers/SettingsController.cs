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
                model.model1 = UpdateSettingsNameViewModel();
                model.model2 = UpdateJasperJsonViewModel();              
                return model;
            }
            catch (Exception) 
            {
                model.model1.WebsiteName = string.Empty;
                model.model2.JasperJson = string.Empty;
            }
           
            return model;
        }

        [HttpPost]
        public IActionResult SaveJasperJson(JasperJsonViewModel model)
        {
            // In case of error old settings will be restored
            string oldSettingsBackup = Configuration.GlobalWebsiteConfig.GetGlobalJsonFileAsString();

            bool isAjaxRequest = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            try
            {             
               Configuration.GlobalWebsiteConfig.SaveGlobalJsonFileAsString(model.JasperJson);
               Configuration.Initialize();
               TempData["Success"] = true;
            }
            catch
            {
                TempData["ErrorMessage"] = "Změny nemohly být provedeny";

                // Restore old settings
                Configuration.GlobalWebsiteConfig.SaveGlobalJsonFileAsString(oldSettingsBackup);             
               
            }

            if(isAjaxRequest)
            {
                ModelState.Clear(); // Updating model in POST

                // Fresh data (without previously entered errors) will be served
                return PartialView("JasperJsonPartialView", UpdateJasperJsonViewModel());
            }
            else
            {
              return  RedirectToAction("Index", UpdatePage());
            }
        }

        public JasperJsonViewModel UpdateJasperJsonViewModel()
        {
            JasperJsonViewModel model = new JasperJsonViewModel();
            try
            {
                model.JasperJson = Configuration.GlobalWebsiteConfig.GetGlobalJsonFileAsString();
                return model;
            }
            catch 
            {
                model.JasperJson = string.Empty;
                return model;
            }
        }

        public SettingsNameViewModel UpdateSettingsNameViewModel()
        {
            SettingsNameViewModel model = new SettingsNameViewModel();
            try
            {
                model.WebsiteName = _dbHelper.GetWebsiteName();
                return model;
            }
            catch
            {
                model.WebsiteName = string.Empty;
                return model;
            };
        }


        [HttpPost]
        public IActionResult SaveSettings(SettingsNameViewModel model)
        {
            bool isAjaxRequest =  Request.Headers["x-requested-with"] == "XMLHttpRequest";

            try
            {
                if (ModelState.IsValid)
                {
                    _dbHelper.SetWebsiteName(model.WebsiteName);
                    TempData["Success"] = true;
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
                ModelState.Clear(); // Updating model in POST

                // Fresh data (without previously entered errors) will be served
                return PartialView("WebsiteNamePartialView",UpdateSettingsNameViewModel()); 
            }
            else // JS disabled - no partial updates available
            {               
                // Refreshing the page will not cause re-post
                return RedirectToAction("Index", UpdatePage());
            }
        }       
    }
}