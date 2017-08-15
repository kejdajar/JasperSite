using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.Controllers
{    
    [Area("Admin")]
    public class ArticleController : Controller
    {

        [HttpGet]
        public IActionResult Index(int id)
        {
            return View(id);   
        }

        [HttpPost]
        public IActionResult Index(EditArticleViewModel model)
        {
            DbHelper.EditArticle(model);
            return Redirect("/Admin/Article/Index?id=" + model.Id);

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            DbHelper.DeleteArticle(id);
            return RedirectToAction("Articles", "Home");
        }

    }
}