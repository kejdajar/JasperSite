using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;

namespace JasperSiteCore.Areas.Admin.Controllers
{    
    [Authorize]
    [Area("Admin")]
    public class ArticleController : Controller
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

        [HttpGet]
        public IActionResult Index(int id)
        {
            
            return View(id);   
        }

        [HttpPost]
        public IActionResult Index(EditArticleViewModel model)
        {
            

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            Configuration.DbHelper.EditArticle(model);

            if(isAjax)
            {
                return ViewComponent("EditArticle");
            }
            else
            {
                return Redirect("/Admin/Article/Index?id=" + model.Id);
            }

            

        }

        [HttpGet]
        public IActionResult Add()
        {
          
            int articleId =  Configuration.DbHelper.AddArticle();
            return Redirect("/Admin/Article/Index?id=" + articleId);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            
            Configuration.DbHelper.DeleteArticle(id);
            return RedirectToAction("Articles", "Home");
        }

    }
}