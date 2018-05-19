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

           
            Category[] categories = new Category[]
            {
                new Category(){Name="Nezařazeno"},
                new Category(){Name="Programování"},
            };

            foreach (Category c in categories)
            {
                DatabaseContext.Categories.Add(c);
            }
            DatabaseContext.SaveChanges();

            Article[] articles = new Article[]
            {
                new Article {Name="První článek", HtmlContent="<b>tučný text</b>",PublishDate = DateTime.Now,Category=categories[0]},
                 new Article {Name="Druhý článek", HtmlContent="<b>kurzíva</b>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(60),Category=categories[0]},
                   new Article {Name="Třetí článek", HtmlContent="<h2>nadpis text</h2>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(120),Category=categories[1]},
                    new Article {Name="Čtvrtý článek", HtmlContent="test", PublishDate = DateTime.Now + TimeSpan.FromMinutes(180),Category=categories[1]},
            };

            foreach(Article a in articles)
            {
                DatabaseContext.Articles.Add(a);
            }
            DatabaseContext.SaveChanges();

            Role adminRole = new Role() { Name = "Admin" };
            Role redactorRole = new Role() { Name = "Redaktor" };
            DatabaseContext.Roles.Add(adminRole);
            DatabaseContext.Roles.Add(redactorRole);
            DatabaseContext.SaveChanges();

            User admin = new User() { Nickname = "Administrátor", Username = "Admin",Role=adminRole };
            string salt, hashedPassword;
            JasperSiteCore.Models.Security.Authentication.HashPassword("admin", out salt, out hashedPassword);
            admin.Password = hashedPassword;
            admin.Salt = salt;
            DatabaseContext.Users.Add(admin);

            User redactor = new User() { Nickname = "Redaktor", Username = "Red", Role = redactorRole };
            string salt2, hashedPassword2;
            JasperSiteCore.Models.Security.Authentication.HashPassword("red", out salt2, out hashedPassword2);
            redactor.Password = hashedPassword2;
            redactor.Salt = salt2;
            DatabaseContext.Users.Add(redactor);

            DatabaseContext.SaveChanges();

            // Settings
            Setting websiteNameSetting = new Setting() { Key = "WebsiteName", Value = "My first JasperSite webpage" };
            _databaseContext.Settings.Add(websiteNameSetting);
            DatabaseContext.SaveChanges();


        }
        }
    
}


