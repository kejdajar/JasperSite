using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSite.Areas.Admin.ViewModels;
using JasperSite.Models.Database;
using JasperSite.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using JasperSite.Helpers;
using Microsoft.Extensions.Localization;

namespace JasperSite.Areas.Admin.Controllers
{    
    [Authorize]
    [Area("Admin")]
    public class ArticlesController : Controller
    {
        private readonly DatabaseContext databaseContext;
        private readonly DbHelper dbHelper;

        private readonly IStringLocalizer _localizer;
        public ArticlesController(DatabaseContext dbContext, IStringLocalizer<ArticlesController> localizer)
        {
            this.databaseContext = dbContext;
            this.dbHelper = new DbHelper(dbContext);
            this._localizer = localizer;
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

                // Paging section --> first page, items per page
                model.ArticleListModel = UpdateArticleListModel(1, 10);

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

        public ArticleListViewModel UpdateArticleListModel(int currentPage, int itemsPerPage)
        {
            ArticleListViewModel modelOutput = new ArticleListViewModel();
            List<Article> articleList = dbHelper.GetAllArticles();
            articleList.OrderBy(a => a.Name);

            JasperPaging<Article> paging = new JasperPaging<Article>(articleList, currentPage, itemsPerPage);

            modelOutput.CurrentPage = paging.CurrentPageNumber;
            modelOutput.TotalNumberOfPages = paging.NumberOfPagesNeeded;
            modelOutput.Articles = paging.GetCurrentPageItems();
            return modelOutput;

        }

        [HttpPost]
        public IActionResult ArticlePaging(ArticleListViewModel model, IFormCollection collection)
        {

            ArticleListViewModel modelOutput = new ArticleListViewModel();

            try
            {
                int itemsPerPage = 10;
                int currentPage = model.CurrentPage;

                string next = collection["next"];
                string prev = collection["prev"];

                if (next != null)
                    currentPage++;
                if (prev != null)
                    currentPage--;

                modelOutput = UpdateArticleListModel(currentPage, itemsPerPage);

            }
            catch
            {
                TempData["ErrorMessage"] = _localizer["Your request could not be completed."];
            }

            bool isAjaxRequest = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxRequest)
            {
                ModelState.Clear();
                return PartialView("ArticlesListPartialView", modelOutput);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        // Returns only articles with Published=true
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
                TempData["ErrorMessage"] = _localizer["The required article was not found."];
                return RedirectToAction("Index");
            }           

         
        }

        public EditArticleViewModel GetEditArticleModel(int id)
        {
            try
            {
                Article articleToEdit = dbHelper.GetArticleById(id);

                //if (!articleToEdit.Publish) return null; // not published article will not be served

                EditArticleViewModel model = new EditArticleViewModel
                {
                    Id = articleToEdit.Id,
                    HtmlContent = articleToEdit.HtmlContent,
                    Name = articleToEdit.Name,
                    PublishDate = articleToEdit.PublishDate.ToShortDateString(),
                    Categories = dbHelper.GetAllCategories(),
                    SelectedCategoryId = articleToEdit.CategoryId,
                    Publish = articleToEdit.Publish,
                    Keywords = articleToEdit.Keywords                    
                    
                };

                // Localize "Uncategorized" category
                model.Categories.Where(c => c.Name == "Uncategorized").Single().Name = _localizer["Uncategorized"];
               

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
                    newArticleData.PublishDate = DateTime.Parse(model.PublishDate);
                    newArticleData.CategoryId = model.SelectedCategoryId;
                    newArticleData.Publish = model.Publish;
                    newArticleData.Keywords = model.Keywords;
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
                TempData["ErrorMessage"] = _localizer["There has to be at least one URL address."];
            }
            catch(InvalidUrlRewriteException ex)
            {
                try
                {
                    string alreadyAssignedAricleName = dbHelper.GetArticleById(ex.AssignedArticleId).Name;
                    TempData["ErrorMessage"] = _localizer["The URL adress has already an article assigned:"] + " "+alreadyAssignedAricleName+ ".";
                }
                catch 
                {                    
                    TempData["ErrorMessage"] = _localizer["The URL adress is already in use."];
                }
            }
            catch (Exception)
            {
               
                TempData["ErrorMessage"] = _localizer["The changes could not be saved."];
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
                TempData["ErrorMessage"] = _localizer["The article could not be created."];

                return RedirectToAction("Index");
                
            }        
        }

        [HttpGet]
        public IActionResult Delete(int id, int page)
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            try
            {
                dbHelper.DeleteArticle(id);
                TempData["Success"] = true;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = _localizer["The article could not be deleted."];
            }

            
            if (isAjax)
            {
                return PartialView("ArticlesListPartialView", UpdateArticleListModel(page,10)); 
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
                TempData["ErrorMessage"] = _localizer["Category  \"Uncategorized\" could not be created."];                
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
                    TempData["ErrorMessage"] = _localizer["The item could not be deleted."];
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
                        TempData["ErrorMessage"] = _localizer["The item could not be deleted."];
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
                TempData["ErrorMessage"] = _localizer["The required URL was not found to be deleted."];
            }
            catch (NoUrlRulesException)
            {
                TempData["ErrorMessage"] = _localizer["There has to be at least one URL for a article."];
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = _localizer["The item could not be deleted."];
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