using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models;
using JasperSite.Areas.Admin.Models;
using JasperSite.Models.Providers;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace JasperSite.Models.Database
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
            // Resets the configuration file in case it was modified
            Configuration.GlobalWebsiteConfig.ThemeName = "Default";



            /*---START Does not work on certain webhostings--- */

            //if (ensureDbIsDeleted) DatabaseContext.Database.EnsureDeleted();
            //DatabaseContext.Database.EnsureCreated();

            /*---END Does not work on certain webhostings--- */

            DatabaseContext.Database.Migrate(); // creates the database - working solution

            //Configuration.DbHelper = new DbHelper(DatabaseContext); // Old implementation without dependency injection






            // If there is at least one user, the DB was already seeded
            //if (DatabaseContext.Users.Any())
            //{
            //    return;   // DB has been seeded
            //}


            Category[] categories = new Category[]
            {
                new Category(){Name="Nezařazeno"},
                new Category(){Name="První rubrika"},
                new Category(){Name="Druhá rubrika"},
            };

            foreach (Category c in categories)
            {
                DatabaseContext.Categories.Add(c);
            }
//DatabaseContext.SaveChanges();



            // Demo article
            StreamReader sr = new StreamReader(Env.Hosting.ContentRootPath+"/Areas/Admin/DemoDataResources/DemoArticle.txt");
            string articleContent = sr.ReadToEnd();

            Article[] articles = new Article[]
            {
                new Article {Name="Ukázkový článek",Publish=true, HtmlContent=articleContent,PublishDate = DateTime.Now,Category=categories[0],Keywords="článek, ukázka, demo"},
                 new Article {Name="Druhý článek", Publish=true, HtmlContent="<b>kurzíva</b>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(60),Category=categories[1],Keywords="článek, ukázka, demo"},
                 new Article {Name="Třetí článek", Publish=true, HtmlContent="<b>kurzíva</b>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(60),Category=categories[1],Keywords="článek, ukázka, demo"},
                   new Article {Name="Čtvrtý článek", Publish=true, HtmlContent="<h2>nadpis text</h2>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(120),Category=categories[2],Keywords="článek, ukázka, demo"},
                    new Article {Name="Pátý článek", Publish=true, HtmlContent="test", PublishDate = DateTime.Now + TimeSpan.FromMinutes(180),Category=categories[2],Keywords="článek, ukázka, demo"},
            };

            //for (int i = 1; i <= 10000; i++)
            //{
            //    Article a = new Article { Name = "Ukázkový článek", Publish = true, HtmlContent = articleContent, PublishDate = DateTime.Now, Category = categories[0], Keywords = "článek, ukázka, demo" };
            //    DatabaseContext.Articles.Add(a);
            //}

            foreach (Article a in articles)
            {
                DatabaseContext.Articles.Add(a);
            }
            //DatabaseContext.SaveChanges();

            Role adminRole = new Role() { Name = "Admin" };
            Role redactorRole = new Role() { Name = "Redaktor" };
            DatabaseContext.Roles.Add(adminRole);
            DatabaseContext.Roles.Add(redactorRole);
 //DatabaseContext.SaveChanges();

            User admin = new User() { Nickname = "Administrátor", Username = "admin",Role=adminRole };
            string salt, hashedPassword;
            JasperSite.Models.Security.Authentication.HashPassword("admin", out salt, out hashedPassword);
            admin.Password = hashedPassword;
            admin.Salt = salt;
            DatabaseContext.Users.Add(admin);

            User redactor = new User() { Nickname = "Redaktor", Username = "redaktor", Role = redactorRole };
            string salt2, hashedPassword2;
            JasperSite.Models.Security.Authentication.HashPassword("redaktor", out salt2, out hashedPassword2);
            redactor.Password = hashedPassword2;
            redactor.Salt = salt2;
            DatabaseContext.Users.Add(redactor);

//DatabaseContext.SaveChanges();

            // Settings
            Setting websiteNameSetting = new Setting() { Key = "WebsiteName", Value = "Název vašeho webu" };
            _databaseContext.Settings.Add(websiteNameSetting);
 //DatabaseContext.SaveChanges();

            // Text blocks

            List<ThemeInfo> themesInfo= Configuration.ThemeHelper.GetInstalledThemesInfoByNameAndActive();
            foreach(ThemeInfo ti in themesInfo)
            {
                Theme theme = new Theme() { Name = ti.ThemeName };
                _databaseContext.Themes.Add(theme);
 //_databaseContext.SaveChanges();

                // Emulation of website configuration for every theme
                GlobalConfigDataProviderJson globalJsonProvider = new GlobalConfigDataProviderJson("jasper.json");
                GlobalWebsiteConfig globalConfig = new GlobalWebsiteConfig(globalJsonProvider);
                globalConfig.SetTemporaryThemeName(ti.ThemeName); // This will not write enytihing to the underlying .json file 
                ConfigurationObjectProviderJson configurationObjectJsonProvider = new ConfigurationObjectProviderJson(globalConfig, "jasper.json");
                WebsiteConfig websiteConfig = new WebsiteConfig(configurationObjectJsonProvider);


                TextBlock textBlock1 = new TextBlock() { Name = "WelcomePageTextBlock", Content = "Uvítací stránka - obsah tohoto bloku upravte pomocí redakčního systému." };
                TextBlock textBlock2 = new TextBlock() { Name = "AboutPageTextBlock", Content = "Informace o autorovi - obsah tohoto bloku upravte pomocí redakčního systému." };
                TextBlock textBlock3 = new TextBlock() { Name = "FooterPageTextBlock", Content = "&copy;2018-2019 - Bc. Jaromír Kejda, PEF ČZU Praha, diplomová práce: Návrh a implementace redakčního systému v ASP.NET Core, vytvořeno pomocí redakčního systému JasperSite." };
                if (ti.ThemeName == "Default")
                {
                    // Demo text blocks
                    _databaseContext.TextBlocks.Add(textBlock1);
                    _databaseContext.TextBlocks.Add(textBlock2);
                    _databaseContext.TextBlocks.Add(textBlock3);
 //_databaseContext.SaveChanges();

                    // DemoImages
                    string imgPath = Env.Hosting.ContentRootPath + "/Areas/Admin/DemoDataResources/demo_background.jpg";
                    byte[] bytes = System.IO.File.ReadAllBytes(imgPath);
                    Image img = new Image();
                    img.Name = "Ukázkový obrázek";

                    ImageData imgData = new ImageData() { Data = bytes };

                    img.ImageData = imgData;

                    _databaseContext.Images.Add(img);
                    _databaseContext.ImageData.Add(imgData);

 //_databaseContext.SaveChanges();
                }

                List<string> holders = websiteConfig.BlockHolders;
                foreach (string holderName in holders)
                {
                    BlockHolder blockHolder = new BlockHolder() { Name = holderName, ThemeId = theme.Id };
                    _databaseContext.BlockHolders.Add(blockHolder);

                    if(ti.ThemeName=="Default")
                    {
                        if (holderName.Contains("Main"))
                        {
                            Holder_Block hb1 = new Holder_Block() { BlockHolder = blockHolder, TextBlock = textBlock1, Order=1};
                            _databaseContext.Holder_Block.Add(hb1);
                        }
                        else if (holderName.Contains("About"))
                        {
                            Holder_Block hb2 = new Holder_Block() { BlockHolder = blockHolder, TextBlock = textBlock2, Order = 1};
                            _databaseContext.Holder_Block.Add(hb2);
                        }
                        else
                        {
                            Holder_Block hb3 = new Holder_Block() { BlockHolder = blockHolder, TextBlock = textBlock3, Order = 1};
                            _databaseContext.Holder_Block.Add(hb3);
                        }
//_databaseContext.SaveChanges();
                    }

                }


               

               

            

            }


            // Add URL rewriting for articles

            UrlRewrite rule1 = new UrlRewrite() { ArticleId = articles[0].Id, Url = "first_article" };
            _databaseContext.UrlRewrite.Add(rule1);

            UrlRewrite rule2 = new UrlRewrite() { ArticleId = articles[1].Id, Url = "second_article" };
            _databaseContext.UrlRewrite.Add(rule2);

            UrlRewrite rule3 = new UrlRewrite() { ArticleId = articles[2].Id, Url = "third_article" };
            _databaseContext.UrlRewrite.Add(rule3);

            UrlRewrite rule4 = new UrlRewrite() { ArticleId = articles[3].Id, Url = "fourth_article" };
            _databaseContext.UrlRewrite.Add(rule4);

            UrlRewrite rule5 = new UrlRewrite() { ArticleId = articles[4].Id, Url = "fifth_article" };
            _databaseContext.UrlRewrite.Add(rule5);


            _databaseContext.SaveChanges();


        }
        }
    
}


