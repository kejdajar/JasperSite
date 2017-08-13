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
