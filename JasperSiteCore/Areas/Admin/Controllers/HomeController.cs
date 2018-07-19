using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using JasperSiteCore.Areas.Admin.Models;
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
            try
            {
                HomeViewModel model = new HomeViewModel();
                model.Articles = dbHelper.GetAllArticles();
                model.Categories = dbHelper.GetAllCategories();
                return model;
            }
            catch 
            {
                HomeViewModel model = new HomeViewModel();
                model.Articles = null;
                model.Categories = null;
                return model;                
            }
        }

        public IActionResult Error()
        {
            return View("_Error");
        }      

    }
}