using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.ViewModels;

namespace JasperSiteCore.Areas.Admin.ViewComponents
{
    [ViewComponent(Name ="EditArticle")]
    public class EditArticleViewComponent:ViewComponent
    {
        private readonly DatabaseContext _db;

        public EditArticleViewComponent(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int articleId)
        {
            Article articleToEdit = await GetArticleToEdit(articleId);
            EditArticleViewModel model = new EditArticleViewModel
            {
                Id = articleToEdit.Id,
                HtmlContent = articleToEdit.HtmlContent,
                Name = articleToEdit.Name,
                PublishDate = articleToEdit.PublishDate
            };
            return View(model);
        }

        private Task<Article> GetArticleToEdit(int articleId)
        {
            if(_db.Articles.Any())
            {
                return Task.FromResult( _db.Articles.Where(a => a.Id == articleId).Single());
            }
            return null;
           
        }
    }
}
