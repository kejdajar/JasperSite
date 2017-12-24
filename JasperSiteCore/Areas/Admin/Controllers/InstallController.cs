using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Providers;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InstallController : Controller
    {   [HttpGet]
        public IActionResult Index()
        {
            InstallViewModel model = new InstallViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(InstallViewModel model)
        {
            string selectedDb = model.SelectedDatabase;
            string connectionString = model.ConnectionString;

            GlobalConfigData oldData = Configuration.GlobalWebsiteConfig.ConfigurationDataObject;
            oldData.typeOfDatabase = selectedDb;
            oldData.connectionString = connectionString;
            oldData.installationCompleted = "true";

            Configuration.GlobalWebsiteConfig.SaveGlobalConfigData(oldData);

            if(ModelState.IsValid)
            {
                Configuration.CreateAndSeedDb(true); // Checks if Db contains another data, if it does, they are all deleted.
                return RedirectToAction("Index", "Home", new { area = "admin" });
            }
            
                // Reload configuration data
                GlobalConfigData updatedConfigData = Configuration.GlobalWebsiteConfig.ConfigurationDataObject;
                InstallViewModel model2 = new InstallViewModel();
                model2.ConnectionString = updatedConfigData.connectionString;
                model2.SelectedDatabase = updatedConfigData.typeOfDatabase;
                ModelState.Clear();
                return View(model2);
            
           


        }
    }
}