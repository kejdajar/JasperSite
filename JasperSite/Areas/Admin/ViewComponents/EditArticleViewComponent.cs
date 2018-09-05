using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;
using JasperSite.Areas.Admin.ViewModels;

namespace JasperSite.Areas.Admin.ViewComponents
{
    //[ViewComponent(Name ="EditArticle")]
    //public class EditArticleViewComponent:ViewComponent
    //{
    //    private readonly DatabaseContext _db;
    //    private readonly DbHelper _dbHelper;

    //    // Dependency injection
    //    public EditArticleViewComponent(DatabaseContext db)
    //    {
    //        _db = db;
    //        _dbHelper = new DbHelper(db);
    //    }

    //    public async Task<IViewComponentResult> InvokeAsync(int articleId)
    //    {
    //        Article articleToEdit = await GetArticleToEdit(articleId);
    //        EditArticleViewModel model = new EditArticleViewModel
    //        {
    //            Id = articleToEdit.Id,
    //            HtmlContent = articleToEdit.HtmlContent,
    //            Name = articleToEdit.Name,
    //            PublishDate = articleToEdit.PublishDate,
    //            Categories= _dbHelper.GetAllCategories(),
    //            SelectedCategoryId = articleToEdit.CategoryId
    //        };
    //        return View(model);
    //    }

    //    private Task<Article> GetArticleToEdit(int articleId)
    //    {
    //        if(_db.Articles.Any())
    //        {
    //            return Task.FromResult( _db.Articles.Where(a => a.Id == articleId).Single());
    //        }
    //        return null;
           
    //    }
    //}
}
