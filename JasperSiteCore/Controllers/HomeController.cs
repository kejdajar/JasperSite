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
        {                   // předtím Request.RawUrl
            string rawUrl = Request.Path; // Získá např: /MyController/MyActionName
            string file;
            if (Configuration.CustomRouting.IsHomePage(rawUrl)) // Pouze pro hlavní stránku
            {
                return View(Configuration.CustomRouting.GetHomePageFile());
            }
            else if (!string.IsNullOrEmpty(file = Configuration.CustomRouting.MapUrlToFile(rawUrl))) // Mapping pro ostatní stránky
            {
                return View(file);
            }
            else
            {
                //  return Content("špatný požadavek na server");
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
