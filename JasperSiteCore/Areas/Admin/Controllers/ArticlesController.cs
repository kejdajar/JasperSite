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

            try
            {
                List<Article> articles = dbHelper.GetAllArticles();
                if (articles.Count > 0)
                    model.Articles = articles;
                else model.Articles = null;
            }
            catch
            {
                model.Articles = null;
            }
            
            try
            {
                model.NumberOfCategories = dbHelper.GetAllCategories().Count();
            }
            catch
            {
                model.NumberOfCategories = 0;
            }
            
            try
            {
                model.NumberOfArticles = model.Articles.Count();
            }
            catch
            {
                model.NumberOfArticles = 0;
            }

            try
            {
               model.UncategorizedCategoryExists= dbHelper.UncategorizedCategoryExists();
            }
            catch
            {
                model.UncategorizedCategoryExists = default(bool);
            }
        
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

            EditArticleViewModel model = GetEditArticleModel(id);
            if (model == null || id <=0)
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = "Daný èlánek nebyl nalezen.";
                return View("Index", UpdatePage());
            }

            return View("ArticleEdit",GetEditArticleModel(id));   
        }

        public EditArticleViewModel GetEditArticleModel(int id)
        {
            try
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
            catch
            {
                return null;
            }
            
        }

        [HttpPost]
        public IActionResult PostArticle(EditArticleViewModel model)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            
            if (ModelState.IsValid)
            {
                try
                {
                    dbHelper.EditArticle(model);
                }
                catch
                {
                    // TODO: error message
                }
            }
            else
            {
                // list of all categories is not passed back so it needs to be loaded again
                model.Categories = dbHelper.GetAllCategories();
                return View("ArticleEdit", model);
            }

            if (isAjax)
            {
                return PartialView("EditArticlePartialView", GetEditArticleModel(model.Id));
            }
            else
            {

                return RedirectToAction("GetArticle", new { id = model.Id });
            }

        }

        [HttpGet]
        public IActionResult Add()
        {
            try
            {                
                int articleId = dbHelper.AddArticle();
                return RedirectToAction("GetArticle", new { id = articleId });
            }
            catch(Exception ex)
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = "Nový èlánek nebylo možné vytvoøit.";
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                { ViewBag.ErrorMessage += "Popis chyby:"+ ex.InnerException.Message; }
                return View("Index", UpdatePage());
                
            }        
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            try
            {
             
                dbHelper.DeleteArticle(id);
               
            }
            catch (Exception ex)
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = "Èlánek nebylo možné odstranit";
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                { ViewBag.ErrorMessage += "Popis chyby:" + ex.InnerException.Message; }


                if (isAjax)
                {
                    return PartialView("ArticlesListPartialView", dbHelper.GetAllArticles());
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }

            
            if (isAjax)
            {
                return PartialView("ArticlesListPartialView", dbHelper.GetAllArticles());
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult CreateUncategorizedCategory()
        {
            try
            {
                dbHelper.CreateUncategorizedCategory();
            }
            catch(Exception ex)
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = "Rubriku \"Nezaøezeno\" se nepodaøilo vytvoøit.";
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                { ViewBag.ErrorMessage += "Popis chyby:" + ex.InnerException.Message; }
                return View("Index", UpdatePage());
            }
            
            return RedirectToAction("Index");
        }

    }
}