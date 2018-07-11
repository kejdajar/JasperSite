using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Models;
using Microsoft.AspNetCore.Hosting;


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
            // Test, whether the installation was already completed and database was seeded
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
               
            string rawUrl = Request.Path; // Gets ie.: /MyController/MyActionName
            string file;
            if (Configuration.CustomRouting.IsHomePage(rawUrl)) // For the main (index) page only
            {
                return View(Configuration.CustomRouting.GetHomePageFile());
            }
            else if (!string.IsNullOrEmpty(file = Configuration.CustomRouting.MapUrlToFile(rawUrl))) // Other pages are mapped as well
            {
                return View(file);
            }
            else // page was not found
            {
                bool isRequestFromAdminArea = (rawUrl.ToLower()).StartsWith("/admin") ? true : false;
                if(isRequestFromAdminArea)
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
                        return View(Configuration.CustomRouting.GetErrorPageFile()); // URL will remain unchanged
                    else return View();
                }
                
            }

        }

      

       

    }
}
