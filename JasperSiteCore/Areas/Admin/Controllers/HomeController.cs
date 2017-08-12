using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.ViewModels;


namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Categories()
        {
            return View();
        }

        public ActionResult Articles()
        {
            return View();
        }

        public IActionResult EditArticle(int id)
        {
            return Content("test");
        //    Article articleToEdit = DbHelper.GetArticleById(id);
        //    EditArticleViewModel model = new EditArticleViewModel()
        //    {
        //        Id = articleToEdit.Id,
        //        Name = articleToEdit.Name,
        //        PublishDate = articleToEdit.PublishDate,
        //        HtmlContent = articleToEdit.HtmlContent

        //    };
        //    return PartialView("_EditArticlePartial",model);
        }
    }
}