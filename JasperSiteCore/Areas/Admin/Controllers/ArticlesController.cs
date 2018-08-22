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
            try
            {
                EditArticleViewModel model = GetEditArticleModel(id);

                if (model == null || id <= 0)
                {
                    throw new Exception();
                }

                return View("ArticleEdit", model);
            }
            catch
            {
                TempData["ErrorMessage"] = "Daný èlánek nebyl nalezen";
                return View("Index", UpdatePage());
            }           

         
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

        // This action method does not return partial view - too slow and laggy for tinyMCE
        [HttpPost]
        public IActionResult PostArticle(EditArticleViewModel model)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            try
            {
                if (ModelState.IsValid)
                {

                    dbHelper.EditArticle(model);

                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 500; // jquery (error: function(){...}) will show error slide-down window
            }

            if (isAjax)
            {
                return Content(string.Empty);
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
            catch(Exception)
            {
                TempData["ErrorMessage"] = "Daný èlánek nebylo možné vytvoøit";

                return RedirectToAction("Index");
                
            }        
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            try
            {
                dbHelper.DeleteArticle(id);
                TempData["Success"] = true;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Daný èlánek nebylo možné odstranit";
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
                TempData["Success"] = true;
            }
            catch(Exception)
            {
                TempData["ErrorMessage"] = "Kategorii \"nezaøazeno\" se nepodaøilo vytvoøit.";                
            }
            
            return RedirectToAction("Index");
        }

    }
}