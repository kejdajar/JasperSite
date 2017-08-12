using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;

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
            ModelState.Clear();
            model = new EditArticleViewModel();
            return ViewComponent("EditArticle", new { articleId = 1 });

        }


    }
}