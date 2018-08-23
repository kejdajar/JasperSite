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
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

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

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Configuration.InstallationCompleted())
            {
                base.OnActionExecuting(filterContext);
            }
            else
            { 
                if (User.IsInRole("Admin"))
                {
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                { "Controller", "Login" },
                { "Action", "Index" },
                        {"Area","Admin" }
                    });
                    base.OnActionExecuting(filterContext);
                }
            }



        }

       
        [HttpGet]
        public IActionResult Index()
        {
           return View(UpdateModel());
        }

        public InstallViewModel UpdateModel()
        {

            List<DatabaseListItem> allDatabases = new List<DatabaseListItem>() { new DatabaseListItem() { Id = 1, Name = "MSSQL" }, new DatabaseListItem() { Id = 2, Name = "MySQL" } };

            try
            {
                // Every request will be granted the most up-to-date data
                Configuration.GlobalWebsiteConfig.RefreshData();

                // View model
                InstallViewModel model = new InstallViewModel();
                // HTML Select 
                model.AllDatabases = allDatabases;

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
            catch 
            {

                InstallViewModel model = new InstallViewModel();
                model.AllDatabases = allDatabases;
                model.ConnectionString = string.Empty;
                model.SelectedDatabase = default(int);
                return model;
            }
        }

        [HttpPost]
        public IActionResult Index(InstallViewModel model)
        {
            string selectedDb = string.Empty;
            int selectedDbId = model.SelectedDatabase;
            switch (selectedDbId)
            {
                case 1: selectedDb = "mssql"; break;
                case 2: selectedDb = "mysql"; break;
            }

            string connectionString = model.ConnectionString;


            // DATABASE INSTALLATION
            if (ModelState.IsValid)
            {
                string oldConnString = Configuration.GlobalWebsiteConfig.ConnectionString;
                bool oldInstallationCompleted = Configuration.GlobalWebsiteConfig.InstallationCompleted;
                try
                {

                    Configuration.GlobalWebsiteConfig.TypeOfDatabase = selectedDb;
                    Configuration.GlobalWebsiteConfig.ConnectionString = connectionString;
                    Configuration.GlobalWebsiteConfig.InstallationCompleted = true;

                    // Env.Services.AddDbContext<DatabaseContext>(); // does not work here, only in ConfigureServices

                    //DatabaseContext dbContext = ((ServiceProvider)Env.ServiceProvider).GetRequiredService<DatabaseContext>(); // OLD WAY - problems with detached and atached objects

                    JasperSiteCore.Models.Configuration.CreateAndSeedDb(dbContext, true);

                    // All changes in jasper.json will be saved in memory
                    // Without this statement, in-memory jasper.json data will not be up-to-date
                    Configuration.Initialize();

                    return RedirectToAction("Index", "Home", new { area = "admin" });

                }
                catch (Exception ex)
                {
                    string errorMesage = ex.Message;

                    if (ex.InnerException != null)
                    {
                        errorMesage += "Podrobnosti: " + ex.InnerException.Message;
                    }

                    TempData["ErrorMessage"] = errorMesage;

                    // Reset settings 
                    Configuration.GlobalWebsiteConfig.ConnectionString = oldConnString;
                    Configuration.GlobalWebsiteConfig.InstallationCompleted = oldInstallationCompleted;

                    ModelState.Clear();
                    return View(UpdateModel());
                }

            }
            else
            {
                TempData["ErrorMessage"] = "Některé zadané informace jsou neplatné. Žádné změny nebyly provedeny.";
                return RedirectToAction("Index");
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