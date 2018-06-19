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
            else
            {
                // Error page
                if (!Env.Hosting.IsDevelopment())
                    return View(Configuration.CustomRouting.GetErrorPageFile());
                else return View();
            }

        }

      

        [HttpGet("/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            // return Content("errr"+ statusCode);
            return View(Configuration.CustomRouting.GetErrorPageFile(),model:statusCode);
        }

    }
}
