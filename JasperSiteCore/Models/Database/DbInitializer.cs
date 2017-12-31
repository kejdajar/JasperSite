using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models;

namespace JasperSiteCore.Models.Database
{
   
        public  class DbInitializer
        {

        public DbInitializer(DatabaseContext context)
        {
            _databaseContext = context;
           
        }

        private DatabaseContext _databaseContext;  
        public  DatabaseContext DatabaseContext
        {
            get { return _databaseContext; }
           
        }


      public void Initialize(bool ensureDbIsDeleted = false)
            {
           
            Configuration.DbHelper = new DbHelper(DatabaseContext);

            if(ensureDbIsDeleted) DatabaseContext.Database.EnsureDeleted();
           

            DatabaseContext.Database.EnsureCreated();

                // Look for any articles
                if (DatabaseContext.Articles.Any())
                {
                    return;   // DB has been seeded
                }


            Article[] articles = new Article[]
            {
                new Article {Name="První článek", HtmlContent="<b>tučný text</b>", PublishDate = DateTime.Now},
                 new Article {Name="Druhý článek", HtmlContent="<b>kurzíva</b>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(60)},
                   new Article {Name="Třetí článek", HtmlContent="<h2>nadpis text</h2>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(120)},
                    new Article {Name="Čtvrtý článek", HtmlContent="test", PublishDate = DateTime.Now + TimeSpan.FromMinutes(180)},
            };

            foreach(Article a in articles)
            {
                DatabaseContext.Articles.Add(a);
            }

            Category[] categories = new Category[]
            {
                new Category { Name="Category1"},
                 new Category { Name="Category2"},
                  new Category { Name="Category3"},
                   new Category { Name="Category4"}
            };

            foreach(Category c in categories)
            {
                DatabaseContext.Categories.Add(c);
            }            
               DatabaseContext.SaveChanges();
            }
        }
    
}


