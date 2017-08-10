using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Models.Database
{
    public static class Database
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
    }
}
