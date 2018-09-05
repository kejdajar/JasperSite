using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSite.Areas.Admin.ViewModels;
using JasperSite.Models;
using JasperSite.Models.Database;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace JasperSite.Areas.Admin.Controllers
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
                model.model3 = UpdateJasperJsonThemeViewModel();
                return model;
            }
            catch (Exception) 
            {
                model.model1.WebsiteName = string.Empty;
                model.model2.JasperJson = string.Empty;

                model.model3.JasperJson = string.Empty;
                model.model3.Themes = new List<Theme>();
            }
           
            return model;
        }

        [HttpPost]
        public IActionResult SaveThemeJasperJson()
        {
            return Ok();
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

        public JasperJsonThemeViewModel UpdateJasperJsonThemeViewModel()
        {
            JasperJsonThemeViewModel model = new JasperJsonThemeViewModel();
            try
            {               
                model.JasperJson = Configuration.WebsiteConfig.GetThemeJsonFileAsString();
                model.Themes = OrderAllThemesByActive();
                return model;
            }
            catch
            {
                model.JasperJson = null;
                model.Themes = null;
                return model;
            }
        }

        private List<Theme> OrderAllThemesByActive()
        {
            // First item in the list will be the current theme
            List<Theme> allThemes = _dbHelper.GetAllThemes();
            int currentThemeId = _dbHelper.GetCurrentThemeIdFromDb();
            Theme currentTheme = allThemes.Where(t => t.Id == currentThemeId).Single();
            currentTheme.Name += " (aktuální)";
            allThemes.Remove(currentTheme);
            allThemes.Insert(0, currentTheme);
            return allThemes;
        }

        [HttpGet]
        public IActionResult FetchThemeJasperJsonData(int themeId)
        {
           string themeName= _dbHelper.GetAllThemes().Where(t => t.Id == themeId).Single().Name;
           string jsonThemeData= Configuration.WebsiteConfig.GetThemeJsonFileAsString(themeName);

            JasperJsonThemeViewModel model = new JasperJsonThemeViewModel();
            model.JasperJson = jsonThemeData;

            // currently selected theme will be first in the list
            List<Theme> allThemes = _dbHelper.GetAllThemes();
            Theme themeBeingShown = allThemes.Where(t => t.Id == themeId).Single();           
            allThemes.Remove(themeBeingShown);
            allThemes.Insert(0, themeBeingShown);

            // currently activated theme will be marked
            int currentThemeid = _dbHelper.GetCurrentThemeIdFromDb();
            allThemes.Where(t => t.Id == currentThemeid).Single().Name += " (aktuální)";

            model.Themes = allThemes;

            return PartialView("JasperJsonThemePartialView", model);
        }

        [HttpPost]
        public IActionResult PostThemeJasperJsonData(JasperJsonThemeViewModel viewModel)
        {  
            bool isAjaxRequest = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            int selectedThemeId = viewModel.SelectedThemeId;
            string themeNameToBeUpdated = _dbHelper.GetAllThemes().Where(t => t.Id == selectedThemeId).Single().Name;
            string oldDataBackup = string.Empty;

            try
            {
                oldDataBackup =  Configuration.WebsiteConfig.GetThemeJsonFileAsString(themeNameToBeUpdated);

                string newJsonData = viewModel.JasperJson;

                Configuration.WebsiteConfig.SaveThemeJsonFileAsString(newJsonData,themeNameToBeUpdated);
                Configuration.ThemeHelper.UpdateAllThemeRelatedData(_databaseContext); // apply changes             

                TempData["Success"] = true;
             
                
            }
            catch 
            {
                TempData["ErrorMessage"] = "Soubor nebylo možné aktualizovat.";
              
                // revert chages
                Configuration.WebsiteConfig.SaveThemeJsonFileAsString(oldDataBackup, themeNameToBeUpdated);
            }

            //// update model
            //JasperJsonThemeViewModel model = new JasperJsonThemeViewModel();
            //model.JasperJson = Configuration.WebsiteConfig.GetThemeJsonFileAsString();
            //// currently selected theme will be first in the list
            //List<Theme> allThemes = _dbHelper.GetAllThemes();
            //Theme themeToBeUpdated = _dbHelper.GetAllThemes().Where(t => t.Id == selectedThemeId).Single();
            //themeToBeUpdated.Name += " (aktuální)";
            //allThemes.Remove(themeToBeUpdated);
            //allThemes.Insert(0, themeToBeUpdated);


            JasperJsonThemeViewModel model = new JasperJsonThemeViewModel();
            model.JasperJson = Configuration.WebsiteConfig.GetThemeJsonFileAsString(themeNameToBeUpdated);

            // currently selected theme will be first in the list
            List<Theme> allThemes = _dbHelper.GetAllThemes();
            Theme themeBeingShown = allThemes.Where(t => t.Id == selectedThemeId).Single();
            allThemes.Remove(themeBeingShown);
            allThemes.Insert(0, themeBeingShown);

            // currently activated theme will be marked
            int currentThemeid = _dbHelper.GetCurrentThemeIdFromDb();
            allThemes.Where(t => t.Id == currentThemeid).Single().Name += " (aktuální)";

            model.Themes = allThemes;

            ModelState.Clear();
            if (isAjaxRequest)
            {
                return PartialView("JasperJsonThemePartialView", model);
            }
            else return RedirectToAction("Index");

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