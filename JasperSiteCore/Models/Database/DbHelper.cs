using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JasperSiteCore.Models.Database
{
    public static class DbHelper
    {

        public static List<Article> GetAllArticles()
        {
            DatabaseContext database = DbInitializer.Database;

            if (database.Articles.Any())
            {
                return database.Articles.ToList();
            }
           else
            {
                return null;
            }
        }

        public static Article GetArticleById(int id)
        {
            DatabaseContext database = DbInitializer.Database;

            if (database.Articles.Any())
            {
                return database.Articles.Where(a => a.Id==id).Single();
            }
            else
            {
                return null;
            }
        }

        public static int AddArticle()
        {
            DatabaseContext database = DbInitializer.Database;
            JasperSiteCore.Models.Database.Article articleEntity = new Article()
            {               
                HtmlContent = "Váš článek začíná zde...",
                Name = "Nový článek",
                PublishDate = DateTime.Now
            };
            database.Articles.Add(articleEntity);
            database.SaveChanges();
            return articleEntity.Id;

        }

        public static void EditArticle(JasperSiteCore.Areas.Admin.ViewModels.EditArticleViewModel article)
        {
            DatabaseContext database = DbInitializer.Database;
            Article oldArticleToChange = database.Articles.Where(a => a.Id == article.Id).Single();


            oldArticleToChange.HtmlContent = article.HtmlContent;
            oldArticleToChange.Name = article.Name;
            oldArticleToChange.PublishDate = article.PublishDate;
           
           // database.Articles.Add(oldArticleToChange);
            database.SaveChanges();
        }


        public static void DeleteArticle(int articleId)
        {
            DatabaseContext database = DbInitializer.Database;
            Article articleToDelete = database.Articles.Where(a => a.Id == articleId).Single();
            database.Articles.Remove(articleToDelete);
            database.SaveChanges();
        }



        public static List<Category> GetAllCategories()
        {
            DatabaseContext database = DbInitializer.Database;
            if(database.Categories.Any())
            {
                return database.Categories.ToList();
            }
            else
            {
                return null;
            }
        }

        public static int GetNumberOfEntities(string nameOfContextProperty)
        {
            DatabaseContext database = DbInitializer.Database;
            if (database.Categories.Any())
            {
                object i = database.GetType().GetProperty(nameOfContextProperty).GetValue(database);
                IEnumerable<Article> a = i as IEnumerable<Article>;
                return a.Count();
            }
            else
            {
                return 0;
            }
        }
    }
}
