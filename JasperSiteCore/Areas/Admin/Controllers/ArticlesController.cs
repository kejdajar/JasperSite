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

                    // At least one ULR rewriting rule has to exist and URL rewriting has to be allowed
                    int numberOfRules = dbHelper.GetUrls(model.Id).Count();
                    if(numberOfRules<1 && Configuration.WebsiteConfig.UrlRewriting)
                    {
                        throw new NoUrlRulesException("At least one rewriting rule has to be present.");
                    }

                    TempData["Success"] = true;

                }
                else
                {
                    throw new Exception();
                }
            }
            catch (NoUrlRulesException)
            {
                TempData["ErrorMessage"] = "Je nutné pøiøadit alespoò jednu URL adresu.";
            }
            catch(InvalidUrlRewriteException ex)
            {
                try
                {
                    string alreadyAssignedAricleName = dbHelper.GetArticleById(ex.AssignedArticleId).Name;
                    TempData["ErrorMessage"] = "Zadaná URL adresa je již obsazená èlánkem: "+alreadyAssignedAricleName+ ".";
                }
                catch 
                {                    
                    TempData["ErrorMessage"] = "Zadaná URL adresa je již obsazená.";
                }
            }
            catch (Exception)
            {
               
                TempData["ErrorMessage"] = "Zmìny nebylo možné uložit.";
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

       
        [HttpGet]
        public IActionResult DeleteRoute(string name, int? currentArticleId)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            try
            {
                // Both parameters has to be filled in
                if (string.IsNullOrEmpty(name) || currentArticleId == null)
                {
                    TempData["ErrorMessage"] = "Položku se nepodaøilo odstranit.";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Let's test if article and URL exists
                    try
                    {
                        Article article = dbHelper.GetArticleById((int)currentArticleId);
                        UrlRewrite rule= dbHelper.GetAllUrls().Where(url => url.Url == name).Single();
                    }
                    catch
                    {
                        TempData["ErrorMessage"] = "Položku se nepodaøilo odstranit.";
                        return RedirectToAction("Index");
                    }
                   
                    
                }

                // At least one ULR rewriting rule has to exist and URL rewriting has to be allowed
                int numberOfRules = dbHelper.GetUrls((int)currentArticleId).Count();                                               

                if (numberOfRules < 2 && Configuration.WebsiteConfig.UrlRewriting)
                {
                    throw new NoUrlRulesException("At least one rewriting rule has to be present.");
                }

                dbHelper.DeleteUrl(name);

                TempData["Success"] = true;
                  

            }
            catch(InvalidUrlRewriteException)
            {
                TempData["ErrorMessage"] = "Daná URL adresa nebyla nalezena pro operaci odstranìní.";
            }
            catch (NoUrlRulesException)
            {
                TempData["ErrorMessage"] = "Pro daný èlánek musí existovat alespoò jedna URL adresa.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Položku se nepodaøilo odstranit";
            }


            if (isAjax)
            {
                              
                    // Return URL routes only for current article
                    UrlListViewModel model = new UrlListViewModel();
                    model.Urls = dbHelper.GetUrls((int)currentArticleId);
                    model.ArticleId = (int)currentArticleId;
                    return PartialView("UrlListPartialView", model); 
                
            }
            else
            {
                return RedirectToAction("Index");
            }


        }

    }
}