using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Providers;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InstallController : Controller
    {
        private readonly DatabaseContext dbContext;

        public InstallController(DatabaseContext context)
        {
            this.dbContext = context;
        }

        [HttpGet]
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
                Configuration.CreateAndSeedDb(dbContext,true); // Checks if Db contains another data, if it does, they are all deleted.
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

        public IActionResult ResetGlobalConfigJson()
        {
            Configuration.GlobalWebsiteConfig.ConnectionString = "";
            Configuration.GlobalWebsiteConfig.InstallationCompleted = false;
            Configuration.GlobalWebsiteConfig.ThemeFolder = "Themes";
            Configuration.GlobalWebsiteConfig.ThemeName = "Default";
            Configuration.GlobalWebsiteConfig.TypeOfDatabase = "mssql";
            return RedirectToAction("Index");
        }
    }
}