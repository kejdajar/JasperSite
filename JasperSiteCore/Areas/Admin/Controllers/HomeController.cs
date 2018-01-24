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
            int pageNumber = 1;
            int itemsPerPage = 3;

            ThemesViewModel model = new ThemesViewModel();
            model.SelectedThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
            model.ThemeFolder = Configuration.GlobalWebsiteConfig.ThemeFolder;  
            model.PageNumber = pageNumber;
            model.ItemsPerPage = itemsPerPage;


            // Testing purposes
            model.ThemeInfoList = new List<ThemeInfo>();
            List<ThemeInfo> generic = new List<ThemeInfo>();
            for(int i=1;i<=10;i++)
            {
                ThemeInfo info = new ThemeInfo() { ThemeFolder = "Folder:" + i.ToString(), ThemeName = "ThemeX" + i.ToString(), ThemeThumbnailUrl = "http://google.cz" };
                generic.Add(info);
            }
            model.ThemeInfoList.AddRange(generic);
            // End testing purposes

            model.ThemeInfoList.AddRange(Configuration.ThemeHelper.GetInstalledThemesInfo());
            model.ThemeInfoList = model.ThemeInfoList.Skip(itemsPerPage * (pageNumber - 1)).Take(itemsPerPage).ToList();



          return View(model);
        }

        [HttpPost]
        public ActionResult Themes(ThemesViewModel model,IFormCollection collection)
        {
            string next_ = collection["next"];
            string prev_ = collection["prev"];

            int itemsPerPage = 3;
            int currentPage = model.PageNumber;
            ;
           
            if(next_ != null)
            currentPage++;
            if (prev_ != null)
                currentPage--;

            ThemesViewModel model2 = new ThemesViewModel();
            model2.SelectedThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
            model2.ThemeFolder = Configuration.GlobalWebsiteConfig.ThemeFolder;
            model2.PageNumber = currentPage;
            model2.ItemsPerPage = itemsPerPage;


            // Testing purposes
            model2.ThemeInfoList = new List<ThemeInfo>();
            List<ThemeInfo> generic = new List<ThemeInfo>();
            for (int i = 1; i <= 10; i++)
            {
                ThemeInfo info = new ThemeInfo() { ThemeFolder = "Folder:" + i.ToString(), ThemeName = "ThemeX" + i.ToString(), ThemeThumbnailUrl = "http://google.cz" };
                generic.Add(info);
            }
            model2.ThemeInfoList.AddRange(generic);
            // End testing purposes

            model2.ThemeInfoList.AddRange(Configuration.ThemeHelper.GetInstalledThemesInfo());
            model2.ThemeInfoList = model2.ThemeInfoList.Skip(itemsPerPage * (currentPage - 1)).Take(itemsPerPage).ToList();

            ModelState.Clear();
            return PartialView("ThemesPartialView", model2);
            //return PartialView("TestPartial", DateTime.Now);
        }


        public ActionResult UpdateConfiguration()
        {
            
            JasperSiteCore.Models.Configuration.Initialize();
            return RedirectToAction("Themes");
         }

      

    }
}