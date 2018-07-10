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

        private readonly DatabaseContext databaseContext;
        private readonly DbHelper dbHelper;
        public ArticlesController(DatabaseContext dbContext)
        {
            this.databaseContext = dbContext;
            this.dbHelper = new DbHelper(dbContext);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetArticle(int id)
        {
            
            return View("ArticleEdit",id);   
        }

        [HttpPost]
        public IActionResult PostArticle(EditArticleViewModel model)
        {
            

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            dbHelper.EditArticle(model);

            if(isAjax)
            {
                return ViewComponent("EditArticle",new { articleId=model.Id});
            }
            else
            {
                return Redirect("/Admin/Article/Index?id=" + model.Id);
            }

            

        }

        [HttpGet]
        public IActionResult Add()
        {
          
            int articleId =  dbHelper.AddArticle();
            return Redirect("/Admin/Articles/GetArticle?id=" + articleId);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            
            dbHelper.DeleteArticle(id);     

            return RedirectToAction("Index");
        }

    }
}