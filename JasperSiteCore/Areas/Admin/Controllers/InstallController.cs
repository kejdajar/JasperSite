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

            //GlobalConfigData oldData = Configuration.GlobalWebsiteConfig.ConfigurationDataObject;
            //oldData.TypeOfDatabase = selectedDb;
            //oldData.ConnectionString = connectionString;
            //oldData.InstallationCompleted = "true";

            //Configuration.GlobalWebsiteConfig.SaveData(oldData);

            Configuration.GlobalWebsiteConfig.TypeOfDatabase = selectedDb;
            Configuration.GlobalWebsiteConfig.ConnectionString = connectionString;
            Configuration.GlobalWebsiteConfig.InstallationCompleted = true;
            Configuration.GlobalWebsiteConfig.CommitChanges();

            if(ModelState.IsValid)
            {
                Configuration.CreateAndSeedDb(true); // Checks if Db contains another data, if it does, they are all deleted.
                return RedirectToAction("Index", "Home", new { area = "admin" });
            }

            // Reload configuration data

            //GlobalConfigData updatedConfigData = Configuration.GlobalWebsiteConfig.ConfigurationDataObject;
            //    InstallViewModel model2 = new InstallViewModel();
            //    model2.ConnectionString = updatedConfigData.ConnectionString;
            //    model2.SelectedDatabase = updatedConfigData.TypeOfDatabase;

            InstallViewModel model2 = new InstallViewModel();
            Configuration.GlobalWebsiteConfig.RefreshData(); // Refreshes data from file
            model2.ConnectionString = Configuration.GlobalWebsiteConfig.ConnectionString;
            model2.SelectedDatabase = Configuration.GlobalWebsiteConfig.TypeOfDatabase;

            ModelState.Clear();
           return View(model2);          
           


        }
    }
}