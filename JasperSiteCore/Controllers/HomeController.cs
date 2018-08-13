using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Models;
using Microsoft.AspNetCore.Hosting;
using JasperSiteCore.Models.Database;


namespace JasperSiteCore.Controllers
{
    
  
    public class HomeController : Controller
    {            
        
        // GET: Home
        /// <summary>
        /// Hlavní rozhodovací controller pro custom routing
        /// </summary>
        /// <returns>Vrací vyžádanou stránku dle nastavení v jasper.json</returns>
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

                string rawUrl = Request.Path; // Gets ie.: /MyController/MyActionName


                // In the startup.cs there is: app.UseExceptionHandler("/Views/Shared/_FatalError.cshtml").
                // We don't want to look in custom rounting system for this url and just serve it as is.
                TempData["ErrorInPageLevelCode"] = "1";
                if (rawUrl == "/Views/Shared/_FatalError.cshtml") return View(rawUrl); 
                     


                string file;
                if (Configuration.CustomRouting.IsHomePage(rawUrl)) // For the main (index) page only
                {
                    string viewToReturn = Configuration.CustomRouting.GetHomePageFile(); // throws CustomRouting exception when view is not found

                    // url cant contain "%20" - normal space is required
                    viewToReturn = viewToReturn.Replace("%20", " ");

                    return View(viewToReturn);
                }
                else if (!string.IsNullOrEmpty(file = Configuration.CustomRouting.MapUrlToFile(rawUrl))) // Other pages are mapped as well, throws exception if view is not found
                {
                    return View(file.Replace("%20", " "));
                }
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
                           
                        }else
                        {
                            return View(); // ASP.NET error page will be shown
                        }
                           
                       
                    }

                }
            }
            catch(Exception ex)
            {
              
                TempData["ExceptionMessage"] = ex;                
                
                return View("_FatalError");
            }

        }

      
      
       

    }
}
