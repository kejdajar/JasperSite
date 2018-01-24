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

namespace JasperSiteCore.Areas.Admin.Controllers
{
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

        // GET: Admin/Home
        public ActionResult Index()
        {          
            return View();
        }

        public ActionResult Categories()
        {
           return View();
        }

        public ActionResult Articles()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Themes()
        {
            int itemsPerPage = 3;
            int currentPage = 1;
            List<ThemeInfo> themeInfoList = Configuration.ThemeHelper.GetInstalledThemesInfo();
            JasperPaging<ThemeInfo> paging = new JasperPaging<ThemeInfo>(themeInfoList, currentPage, itemsPerPage);
                       
            ThemesViewModel model = new ThemesViewModel();
            model.SelectedThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
            model.ThemeFolder = Configuration.GlobalWebsiteConfig.ThemeFolder;
            model.PageNumber = paging.CurrentPageNumber;
            model.ItemsPerPage = paging.ItemsPerPage;
            model.TotalNumberOfPages = paging.NumberOfPagesNeeded;

            model.ThemeInfoList = paging.GetCurrentPageItems();

            ModelState.Clear();
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Themes(ThemesViewModel model,IFormCollection collection)
        {
            int itemsPerPage = 3;
            int currentPage = model.PageNumber;

            string next = collection["next"];
            string prev = collection["prev"];

            if (next != null)
                currentPage++;
            if (prev != null)
                currentPage--;

            List<ThemeInfo> themeInfoList = Configuration.ThemeHelper.GetInstalledThemesInfo();
            JasperPaging<ThemeInfo> paging = new JasperPaging<ThemeInfo>(themeInfoList,currentPage,itemsPerPage);          
                     
            ThemesViewModel model2 = new ThemesViewModel();
            model2.SelectedThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
            model2.ThemeFolder = Configuration.GlobalWebsiteConfig.ThemeFolder;
            model2.PageNumber = paging.CurrentPageNumber;
            model2.ItemsPerPage = paging.ItemsPerPage;
            model2.TotalNumberOfPages = paging.NumberOfPagesNeeded;

            model2.ThemeInfoList = paging.GetCurrentPageItems();            

            ModelState.Clear();
            return PartialView("ThemesPartialView", model2);
            
        }


        public ActionResult UpdateConfiguration()
        {            
            JasperSiteCore.Models.Configuration.Initialize();
            return RedirectToAction("Themes");
         }

      

    }
}