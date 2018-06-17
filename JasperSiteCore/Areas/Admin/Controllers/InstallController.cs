using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Providers;
using JasperSiteCore.Models.Database;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InstallController : Controller
    {
        //DEPENDECY INJECTION - NOT WORKING HERE, BECAUSE CONTEXT WAS NOT BUILT YET
        //private readonly DatabaseContext dbContext;
        //public InstallController(DatabaseContext context)
        //{
        //    this.dbContext = context;
        //}

        
        [HttpGet]
        public IActionResult Index()
        {
           return View(UpdateModel());
        }

        public InstallViewModel UpdateModel()
        {
            // Every request will be granted the most up-to-date data
            Configuration.GlobalWebsiteConfig.RefreshData();

            // View model
            InstallViewModel model = new InstallViewModel();
            // HTML Select 
            model.AllDatabases = new List<DatabaseListItem>() { new DatabaseListItem() { Id = 1, Name = "MSSQL" }, new DatabaseListItem() { Id = 2, Name = "MySQL" } };

            // If the installation was completed, the user is shown actual connection string and database in use
            if (Configuration.GlobalWebsiteConfig.InstallationCompleted)
            {
                int idOfDb = 1;
                switch (Configuration.GlobalWebsiteConfig.TypeOfDatabase)
                {
                    case "mssql": idOfDb = 1; break;
                    case "mysql": idOfDb = 2; break;
                }
                model.SelectedDatabase = idOfDb;
                model.ConnectionString = Configuration.GlobalWebsiteConfig.ConnectionString;
            }
            return model;
        }

        [HttpPost]
        public IActionResult Index(InstallViewModel model)
        {
           string selectedDb = string.Empty;
           int selectedDbId = model.SelectedDatabase;
           switch(selectedDbId)
            {
                case 1: selectedDb = "mssql";break;
                case 2: selectedDb = "mysql";break;
            }

            string connectionString = model.ConnectionString;           

       
           
            // DATABASE INSTALLATION
            if(ModelState.IsValid)
            {
                string oldConnString = Configuration.GlobalWebsiteConfig.ConnectionString;
                bool oldInstallationCompleted = Configuration.GlobalWebsiteConfig.InstallationCompleted;
                try
                {
                    Configuration.GlobalWebsiteConfig.TypeOfDatabase = selectedDb;
                    Configuration.GlobalWebsiteConfig.ConnectionString = connectionString;
                    Configuration.GlobalWebsiteConfig.InstallationCompleted = true;

                    // Env.Services.AddDbContext<DatabaseContext>(); // does not work here, only in ConfigureServices
                    DatabaseContext dbContext = ((ServiceProvider)Env.ServiceProvider).GetRequiredService<DatabaseContext>();
                    JasperSiteCore.Models.Configuration.CreateAndSeedDb(dbContext, true);
                    return RedirectToAction("Index", "Home", new { area = "admin" });

            }
                catch(Exception ex)
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = ex.Message+", "+ex.InnerException;
                
                // Reset settings 
                Configuration.GlobalWebsiteConfig.ConnectionString = oldConnString;
                Configuration.GlobalWebsiteConfig.InstallationCompleted = oldInstallationCompleted;

                return View(UpdateModel());
            }

        }
            else
            {
               return View(model);
            }
            

        }

        public IActionResult ResetGlobalConfigJson()
        {
            Configuration.GlobalWebsiteConfig.ResetToDefaults();
            return RedirectToAction("Index");
        }
    }

    public class DatabaseListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}