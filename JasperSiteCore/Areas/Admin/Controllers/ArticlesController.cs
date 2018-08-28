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
                TempData["ErrorMessage"] = "Dan� �l�nek nebyl nalezen";
                return RedirectToAction("Index");
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
                    SelectedCategoryId = articleToEdit.CategoryId,
                    
                };

                // URL rewriting
                model.AllUrl = dbHelper.GetUrls(articleToEdit.Id);
                model.ArticlesRoute = Configuration.WebsiteConfig.ArticleRoute;

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
                    Article newArticleData = new Article();
                    newArticleData.Id = model.Id;
                    newArticleData.HtmlContent = model.HtmlContent;
                    newArticleData.Name = model.Name;
                    newArticleData.PublishDate = (DateTime)model.PublishDate;
                    newArticleData.CategoryId = model.SelectedCategoryId;
                    Article articleReference = dbHelper.EditArticle(newArticleData);

                    // URL rewriting

                    if (!string.IsNullOrEmpty(model.Url))
                    {
                      
                            dbHelper.SetUrl(articleReference, model.Url);
                        
                    }

                    TempData["Success"] = true;

                }
                else
                {
                    throw new Exception();
                }
            }
            catch(InvalidUrlRewriteException ex)
            {
                try
                {
                    string alreadyAssignedAricleName = dbHelper.GetArticleById(ex.AssignedArticleId).Name;
                    TempData["ErrorMessage"] = "Zadan� URL adresa je ji� obsazen� �l�nkem: "+alreadyAssignedAricleName+ ".";
                }
                catch 
                {
                    
                    TempData["ErrorMessage"] = "Zadan� URL adresa je ji� obsazen�.";

                }
            }
            catch (Exception)
            {
               
                TempData["ErrorMessage"] = "Zm�ny nebylo mo�n� ulo�it.";
            }

            if (isAjax)
            {
                // return Content(string.Empty); 

                // Return URL routes only for current article
                UrlListViewModel partialModel = new UrlListViewModel();
                partialModel.Urls = dbHelper.GetAllUrls().Where(url => url.ArticleId == model.Id).Select(u => u.Url).ToList();
                partialModel.ArticleId = model.Id;

              return PartialView("UrlListPartialView", partialModel);
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
                int articleId = dbHelper.AddArticle().Id;
                return RedirectToAction("GetArticle", new { id = articleId });
            }
            catch(Exception)
            {
                TempData["ErrorMessage"] = "Dan� �l�nek nebylo mo�n� vytvo�it";

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
                TempData["ErrorMessage"] = "Dan� �l�nek nebylo mo�n� odstranit";
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
                TempData["ErrorMessage"] = "Kategorii \"neza�azeno\" se nepoda�ilo vytvo�it.";                
            }
            
            return RedirectToAction("Index");
        }

       
        [HttpGet]
        public IActionResult DeleteRoute(string name, int currentArticleId)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            try
            {
                dbHelper.DeleteUrl(name);
                TempData["Success"] = true;
            }
            catch 
            {
                TempData["ErrorMessage"] = "Polo�ku se nepoda�ilo odstranit";            
            }

            if(isAjax)
            {
                // Return URL routes only for current article
                UrlListViewModel model = new UrlListViewModel();
                model.Urls = dbHelper.GetAllUrls().Where(url => url.ArticleId == currentArticleId).Select(u => u.Url).ToList();
                model.ArticleId = currentArticleId;

                return PartialView("UrlListPartialView", model);
            }
            else
            {
                return RedirectToAction("Index");
            }            

           
        }

    }
}