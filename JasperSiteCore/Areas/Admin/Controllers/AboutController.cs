using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public class ArticlesController : Controller
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
        }
    }
}