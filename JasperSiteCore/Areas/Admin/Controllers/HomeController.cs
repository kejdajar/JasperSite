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
        // Firstly, the installation must have already been completed before accesing administration panel
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {          
            if (!Configuration.InstallationCompleted())
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                { "Controller", "Install" },
                { "Action", "Index" },
                        {"Area","Admin" }
                    });
            }

            base.OnActionExecuting(filterContext);
        }

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
            return View();
        }

       

        public ActionResult Articles()
        {
            return View();
        }

        

        [HttpGet]
        public ActionResult Categories()
        {            
            return View(UpdateCategoryPage());
        }

        public CategoriesViewModel UpdateCategoryPage()
        {
            CategoriesViewModel model = new CategoriesViewModel();
            model.Categories =dbHelper.GetAllCategories();
            return model;
        }

        [HttpPost]
        public IActionResult CreateNewCategory(string btnCategoryName)
        {
            dbHelper.AddNewCategory(btnCategoryName);

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest"; 
            if(isAjaxCall)
            {

                return PartialView("AddNewCategoryPartialView", UpdateCategoryPage());
            }
            else
            {
                return RedirectToAction("Categories");
            }
            
        }

        [HttpGet]
        public IActionResult DeleteCategory(CategoriesViewModel model,int id)
        {
            dbHelper.DeleteCategory(id);
            CategoriesViewModel viewModel = new CategoriesViewModel();
            viewModel.Categories = dbHelper.GetAllCategories();
            ModelState.Clear();


            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("AddNewCategoryPartialView", viewModel);
            }
            else
            {
                return View("Categories", viewModel);
            }
                
        }


        

       

    }
}