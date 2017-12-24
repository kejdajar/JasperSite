using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Models;

namespace JasperSiteCore.Areas.Admin.Controllers
{    
    [Area("Admin")]
    public class ArticleController : Controller
    {

        [HttpGet]
        public IActionResult Index(int id)
        {
            #region InstallationCheck
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
            #endregion
            return View(id);   
        }

        [HttpPost]
        public IActionResult Index(EditArticleViewModel model)
        {
            #region InstallationCheck
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
            #endregion

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
            #region InstallationCheck
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
            #endregion
            int articleId =  Configuration.DbHelper.AddArticle();
            return Redirect("/Admin/Article/Index?id=" + articleId);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            #region InstallationCheck
            if (!Configuration.InstallationCompleted())
            {
                return RedirectToAction("Index", "Install", new { area = "admin" });
            }
            #endregion
            Configuration.DbHelper.DeleteArticle(id);
            return RedirectToAction("Articles", "Home");
        }

    }
}