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
            InstallViewModel model = new InstallViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(InstallViewModel model)
        {
            string selectedDb = model.SelectedDatabase;
            string connectionString = model.ConnectionString;           

            Configuration.GlobalWebsiteConfig.TypeOfDatabase = selectedDb;
            Configuration.GlobalWebsiteConfig.ConnectionString = connectionString;
            Configuration.GlobalWebsiteConfig.InstallationCompleted = true;
            //Configuration.GlobalWebsiteConfig.CommitChanges();

            if(ModelState.IsValid)
            {
               // Env.Services.AddDbContext<DatabaseContext>(); // does not work here, only in ConfigureServices
                DatabaseContext dbContext = ((ServiceProvider)Env.ServiceProvider).GetRequiredService<DatabaseContext>();
                JasperSiteCore.Models.Configuration.CreateAndSeedDb(dbContext,true);                                
                return RedirectToAction("Index", "Home", new { area = "admin" });
            }
            else
            {
                //InstallViewModel model2 = new InstallViewModel();
                //Configuration.GlobalWebsiteConfig.RefreshData(); // Refreshes data from file
                //model2.ConnectionString = Configuration.GlobalWebsiteConfig.ConnectionString;
                //model2.SelectedDatabase = Configuration.GlobalWebsiteConfig.TypeOfDatabase;
                //ModelState.Clear();
                return View(model);
            }
            

        }

        public IActionResult ResetGlobalConfigJson()
        {
            Configuration.GlobalWebsiteConfig.ResetToDefaults();
            return RedirectToAction("Index");
        }
    }
}