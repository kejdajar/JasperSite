using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Models;
using Microsoft.AspNetCore.Hosting;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.ViewModels;

namespace JasperSiteCore.Controllers
{

    public class HomeController : Controller
    {
        // HomeController catches all requests coming from /Home/*
        public HomeController(IJasperDataServicePublic dataServicePublic)
        {
            this._dataServicePublic = dataServicePublic;
           
        }

        private IJasperDataServicePublic _dataServicePublic { get; set; }
      

        // GET: Home
        /// <summary>
        /// Main action method for custom routing system.
        /// 1) If installation has not yet been completed, redirects to install page.
        /// 2) If the rawUrl is pointing to _FatalError.cshtml, let's serve it immediatelly.
        /// 3)
        /// </summary>
        /// <returns>Returns appropriate view.</returns>
        [HttpGet]
        public IActionResult Index()
        {
            try
            {

                // Test, whether the installation was already completed and database was seeded
                if (!Configuration.InstallationCompleted())
                {
                    return RedirectToAction("Index", "Install", new { area = "admin" });
                }

                string rawUrl = Request.Path; // Gets ie.: /MyController/MyActionName, WITHOUT HOST NAME
                
                // In the startup.cs there is: app.UseExceptionHandler("/Views/Shared/_FatalError.cshtml").
                // We don't want to look in custom rounting system for this url and just serve it as is.
                // This error view will be returned usually in case of syntax error in view file.
                TempData["ErrorInPageLevelCode"] = "1";
                if (rawUrl == "/Views/Shared/_FatalError.cshtml") return View(rawUrl);
                               
                string file;// helper variable for storing physical file adress for custom routing system

                if (Configuration.CustomRouting.IsHomePage(rawUrl)) // For the main (index) page only
                {
                    string viewToReturn = Configuration.CustomRouting.GetHomePageFile(); // throws CustomRouting exception when view is not found
                   
                    // url can't contain "%20" - normal space is required 
                    return View(viewToReturn.Replace("%20", " "));

                }
                else if (!string.IsNullOrEmpty(file = Configuration.CustomRouting.MapUrlToFile(rawUrl))) // Custom mapping for non-index pages, throws exception if view is not found
                {
                    // url can't contain "%20" - normal space is required 

                    // if urlRewriting is disabled + request for article page + request has parameter Id => id has to be
                    // supplied for article view model                                    

                    if(!Configuration.WebsiteConfig.UrlRewriting && UrlRewriting.CleanseUrl(rawUrl) == UrlRewriting.CleanseUrl(Configuration.WebsiteConfig.ArticleRoute))
                    {
                        string queryId;
                        if (!string.IsNullOrEmpty(queryId=Request.Query["id"].ToString()))
                        {
                            try
                            {
                                return View(file.Replace("%20", " "),Convert.ToInt32(queryId));
                            }
                            catch 
                            {
                               // program will continue
                            }
                        }
                    }

                    return View(file.Replace("%20", " "));
                }
                // --------------URL REWRITING FOR ARTICLES-----------------------------------------------------------------
                // first condition checks whether is URL rewriting allowed in theme jasper.json
                else if (Configuration.WebsiteConfig.UrlRewriting && UrlRewriting.IsUrlRewriteRequest(rawUrl))
                {
                    int articleIdFromRequest = UrlRewriting.ReturnArticleIdFromNiceUrl(rawUrl, _dataServicePublic);
                    if (articleIdFromRequest != -1) // appropriate articleId was found in the DB
                    {
                        string articleFileLocation = Configuration.WebsiteConfig.ArticleFile;
                        string physicalFileLocation = Configuration.CustomRouting.RelativeThemePathToRootRelativePath(articleFileLocation);
                        return View(physicalFileLocation, articleIdFromRequest); // this returns view based one path: ie: /Themes/Jasper/Article.cshtml and passes model data.
                    }
                    else // URL rewriting rule for current article request does not exist
                    {
                        // Return Article view and pass Null instead of Id --> view itself will handle the null value and show ie.: article not found
                        string articleFileLocation = Configuration.WebsiteConfig.ArticleFile;
                        string physicalFileLocation = Configuration.CustomRouting.RelativeThemePathToRootRelativePath(articleFileLocation);
                        return View(physicalFileLocation, null);
                    }

                } // --------------END URL REWRITING FOR ARTICLES--------------------------------------------------------------
                else // page was not found
                {
                    bool isRequestFromAdminArea = (rawUrl.ToLower()).StartsWith("/admin") ? true : false;
                    if (isRequestFromAdminArea)
                    {
                        // Admin error page
                        if (!Env.Hosting.IsDevelopment())
                            return RedirectToAction("Error", "Home", new { area = "admin" }); // URL will be changed
                                                                                              //  return View("~/Areas/Admin/Views/Shared/_Error.cshtml"); // URL will remain the same
                        else return View();
                    }
                    else
                    {
                        // Website error page
                        if (!Env.Hosting.IsDevelopment())
                        {
                            try
                            {
                                string errorPageFileLocation = Configuration.CustomRouting.GetErrorPageFile();
                                return View(errorPageFileLocation); // Custom error page from the theme will be served.
                            }
                            catch (Exception ex)
                            {
                                throw new CustomRoutingException(ex); // This will eventually raise the Fatal Error Page in Production mode.
                            }

                        }
                        else
                        {
                            return View(); // ASP.NET error page will be shown
                        }


                    }

                }
            }
            catch (Exception ex)
            {

                TempData["ExceptionMessage"] = ex;

                return View("_FatalError", UpdateModel());
            }

        }

        private JasperJsonThemeViewModel UpdateJasperJsonThemeViewModel()
        {
            JasperJsonThemeViewModel model = new JasperJsonThemeViewModel();
            model.JasperJson = Configuration.WebsiteConfig.GetThemeJsonFileAsString();

            model.Themes = OrderAllThemesByActive();
            return model;
        }

        private List<Theme> OrderAllThemesByActive()
        {
            // First item in the list will be the current theme
            List<Theme> allThemes = _dataServicePublic.GetAllThemes();
            int currentThemeId = _dataServicePublic.GetCurrentThemeIdFromDb();
            Theme currentTheme = allThemes.Where(t => t.Id == currentThemeId).Single();
            currentTheme.Name += " (aktuální)";
            allThemes.Remove(currentTheme);
            allThemes.Insert(0, currentTheme);
            return allThemes;
        }

        public JasperJsonViewModel UpdateJasperJsonViewModel()
        {
            JasperJsonViewModel model = new JasperJsonViewModel();
            try
            {
                model.JasperJson = Configuration.GlobalWebsiteConfig.GetGlobalJsonFileAsString();
                return model;
            }
            catch
            {
                model.JasperJson = string.Empty;
                return model;
            }
        }

        public SettingsViewModel UpdateModel()
        {
            SettingsViewModel model = new SettingsViewModel();
            try
            {

                model.model2 = UpdateJasperJsonViewModel();
                model.model3 = UpdateJasperJsonThemeViewModel();

            }
            catch (Exception)
            {

                model.model2.JasperJson = string.Empty;
                model.model3.JasperJson = string.Empty;
                model.model3.Themes = new List<Theme>();
            }
            return model;
        }


    }
}
