using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using JasperSite.Models.Database;
using JasperSite.Areas.Admin.ViewModels;
using JasperSite.Models;
using JasperSite.Areas.Admin.Models;
using JasperSite.Helpers;
using System.IO;
using System.Net.Http.Headers;
using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace JasperSite.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class HomeController : Controller
    {  
     
        private readonly DatabaseContext databaseContext;
        private readonly DbHelper dbHelper;

        public HomeController(DatabaseContext dbContext, IStringLocalizer<HomeController> localizer)
        {
            this.databaseContext = dbContext;
            this.dbHelper = new DbHelper(dbContext);
            this._localizer = localizer;
        }

        private readonly IStringLocalizer<HomeController> _localizer;

        // GET: Admin/Home
        public ActionResult Index()
        {
            ViewData["Language"] = _localizer["Language"];
            return View(UpdatePage());
        }  

        public HomeViewModel UpdatePage()
        {
            try
            {
                HomeViewModel model = new HomeViewModel();
                model.Articles = dbHelper.GetAllArticles();
                model.Categories = dbHelper.GetAllCategories();

                string activeUserName = User.Identity.Name;
                JasperSite.Models.Database.User currentUser = dbHelper.GetAllUsers().Where(u => u.Username.Trim().ToLower() == activeUserName.Trim().ToLower()).Single();
                model.CurrentUser = currentUser;

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
        
        [HttpGet]
        public IActionResult Refresh()
        {
            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            try
            {
                Configuration.Initialize();
                Configuration.ThemeHelper.UpdateAllThemeRelatedData(databaseContext);

                if (isAjaxCall)
                {
                    return Ok();
                }
                else
                {
                    return Redirect(Request.Headers["Referer"].ToString()); // refreshes current page
                }

            }
            catch (Exception)
            {
                if (isAjaxCall)
                {
                    return StatusCode(500);
                }
                else
                {
                    return Redirect(Request.Headers["Referer"].ToString()); // refreshes current page
                }
            }          
            
        }


        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

    }
}