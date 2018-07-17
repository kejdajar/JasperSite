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


        private readonly DatabaseContext databaseContext;
        private readonly DbHelper dbHelper;

        public ArticlesController(DatabaseContext dbContext)
        {
            this.databaseContext = dbContext;
            this.dbHelper = new DbHelper(dbContext);
        }

        public ArticlesViewModel UpdatePage()
        {
            ArticlesViewModel model = new ArticlesViewModel();
            model.Articles = dbHelper.GetAllArticles();
            model.NumberOfCategories = dbHelper.GetAllCategories().Count();
            model.NumberOfArticles = model.Articles.Count();
            return model;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(UpdatePage());
        }

        [HttpGet]
        public IActionResult GetArticle(int id)
        {
            

            return View("ArticleEdit",GetEditArticleModel(id));   
        }

        public EditArticleViewModel GetEditArticleModel(int id)
        {
            Article articleToEdit = dbHelper.GetArticleById(id);
            EditArticleViewModel model = new EditArticleViewModel
            {
                Id = articleToEdit.Id,
                HtmlContent = articleToEdit.HtmlContent,
                Name = articleToEdit.Name,
                PublishDate = articleToEdit.PublishDate,
                Categories = dbHelper.GetAllCategories(),
                SelectedCategoryId = articleToEdit.CategoryId
            };
            return model;
        }

        [HttpPost]
        public IActionResult PostArticle(EditArticleViewModel model)
        {          
            
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if(ModelState.IsValid)
            {
                try
                {
                    dbHelper.EditArticle(model);
                }
                catch {
                    // TODO: error message
                }                
            }
            else
            {
                // list of all categories is not passed back so it needs to be loaded again
                model.Categories = dbHelper.GetAllCategories();
                return View("ArticleEdit",model);
            }

            if(isAjax)
            {
                return PartialView("EditArticlePartialView",GetEditArticleModel(model.Id));
            }
            else
            {
                
                return RedirectToAction("GetArticle",new { id=model.Id});
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

            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                return PartialView("ArticlesListPartialView", dbHelper.GetAllArticles());
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

    }
}