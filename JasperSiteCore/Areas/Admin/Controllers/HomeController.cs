using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using JasperSiteCore.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using JasperSiteCore.Helpers;
using System.IO;
using System.Net.Http.Headers;
using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class HomeController : Controller
    {  
        //// Firstly, the installation must have already been completed before accesing administration panel
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{          
        //    if (!Configuration.InstallationCompleted())
        //    {
        //        filterContext.Result = new RedirectToRouteResult(
        //            new RouteValueDictionary {
        //        { "Controller", "Install" },
        //        { "Action", "Index" },
        //                {"Area","Admin" }
        //            });
        //    }

        //    base.OnActionExecuting(filterContext);
        //}

        private readonly DatabaseContext databaseContext;
        private readonly DbHelper dbHelper;

        public HomeController(DatabaseContext dbContext)
        {
            this.databaseContext = dbContext;
            this.dbHelper = new DbHelper(dbContext);
        }

        // GET: Admin/Home
        public ActionResult Index()
        {
            return View(UpdatePage());
        }  

        public HomeViewModel UpdatePage()
        {
            HomeViewModel model = new HomeViewModel();
            model.Articles = dbHelper.GetAllArticles();
            model.Categories = dbHelper.GetAllCategories();
            return model;
        }

        public IActionResult Error()
        {
            return View("_Error");
        }


        

       

    }
}