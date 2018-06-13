using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models;
using JasperSiteCore.Areas.Admin.Models;
using JasperSiteCore.Models.Providers;
using System.IO;

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
            // Resets the configuration file in case it was modified
           // Configuration.GlobalWebsiteConfig.ResetToDefaults();

            //Configuration.DbHelper = new DbHelper(DatabaseContext); // Old implementation without dependency injection

            if (ensureDbIsDeleted) DatabaseContext.Database.EnsureDeleted();
           

            DatabaseContext.Database.EnsureCreated();

                // If there is at least one user, the DB was already seeded
                if (DatabaseContext.Users.Any())
                {
                    return;   // DB has been seeded
                }

           
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
            DatabaseContext.SaveChanges();



            // Demo article
            StreamReader sr = new StreamReader(Env.Hosting.ContentRootPath+"/Areas/Admin/DemoDataResources/DemoArticle.txt");
            string articleContent = sr.ReadToEnd();

            Article[] articles = new Article[]
            {
                new Article {Name="Ukázkový článek", HtmlContent=articleContent,PublishDate = DateTime.Now,Category=categories[0]},
                 new Article {Name="Druhý článek", HtmlContent="<b>kurzíva</b>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(60),Category=categories[1]},
                 new Article {Name="Třetí článek", HtmlContent="<b>kurzíva</b>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(60),Category=categories[1]},
                   new Article {Name="Čtvrtý článek", HtmlContent="<h2>nadpis text</h2>", PublishDate = DateTime.Now + TimeSpan.FromMinutes(120),Category=categories[2]},
                    new Article {Name="Pátý článek", HtmlContent="test", PublishDate = DateTime.Now + TimeSpan.FromMinutes(180),Category=categories[2]},
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

            User admin = new User() { Nickname = "Administrátor", Username = "admin",Role=adminRole };
            string salt, hashedPassword;
            JasperSiteCore.Models.Security.Authentication.HashPassword("admin", out salt, out hashedPassword);
            admin.Password = hashedPassword;
            admin.Salt = salt;
            DatabaseContext.Users.Add(admin);

            User redactor = new User() { Nickname = "Redaktor", Username = "red", Role = redactorRole };
            string salt2, hashedPassword2;
            JasperSiteCore.Models.Security.Authentication.HashPassword("red", out salt2, out hashedPassword2);
            redactor.Password = hashedPassword2;
            redactor.Salt = salt2;
            DatabaseContext.Users.Add(redactor);

            DatabaseContext.SaveChanges();

            // Settings
            Setting websiteNameSetting = new Setting() { Key = "WebsiteName", Value = "Název vašeho webu" };
            _databaseContext.Settings.Add(websiteNameSetting);
            DatabaseContext.SaveChanges();

            // Text blocks

            List<ThemeInfo> themesInfo= Configuration.ThemeHelper.GetInstalledThemesInfoByNameAndActive();
            foreach(ThemeInfo ti in themesInfo)
            {
                Theme theme = new Theme() { Name = ti.ThemeName };
                _databaseContext.Themes.Add(theme);
                _databaseContext.SaveChanges();

                // Emulation of website configuration for every theme
                GlobalConfigDataProviderJson globalJsonProvider = new GlobalConfigDataProviderJson("jasper.json");
                GlobalWebsiteConfig globalConfig = new GlobalWebsiteConfig(globalJsonProvider);
                globalConfig.SetTemporaryThemeName(ti.ThemeName); // This will not write enytihing to the underlying .json file 
                ConfigurationObjectProviderJson configurationObjectJsonProvider = new ConfigurationObjectProviderJson(globalConfig, "jasper.json");
                WebsiteConfig websiteConfig = new WebsiteConfig(configurationObjectJsonProvider);

                if (ti.ThemeName == "Default")
                {
                    TextBlock textBlock1 = new TextBlock() { Name = "WelcomePageTextBlock", Content = "Uvítací stránka - obsah tohoto bloku upravte pomocí redakčního systému." };
                    TextBlock textBlock2 = new TextBlock() { Name = "AboutPageTextBlock", Content = "Informace o autorovi - obsah tohoto bloku upravte pomocí redakčního systému." };
                    TextBlock textBlock3 = new TextBlock() { Name = "FooterPageTextBlock", Content = "&copy;2018-2019 - Bc. Jaromír Kejda, PEF ČZU Praha, diplomová práce: Návrh a implementace redakčního systému v ASP.NET Core, vytvořeno pomocí redakčního systému JasperSiteCore." };
                    _databaseContext.TextBlocks.Add(textBlock1);
                    _databaseContext.TextBlocks.Add(textBlock2);
                    _databaseContext.TextBlocks.Add(textBlock3);
                    _databaseContext.SaveChanges();
                

                List<string> holders = websiteConfig.BlockHolders;
                
                foreach(string holderName in holders)
                {
                    BlockHolder blockHolder = new BlockHolder() { Name = holderName, ThemeId = theme.Id };
                    _databaseContext.BlockHolders.Add(blockHolder);

                    if(holderName.Contains("Main"))
                    {
Holder_Block hb1 = new Holder_Block() { BlockHolder = blockHolder, TextBlock = textBlock1 };
_databaseContext.Holder_Block.Add(hb1);
                    }
                    else if (holderName.Contains("About"))
                    {
Holder_Block hb2 = new Holder_Block() { BlockHolder = blockHolder, TextBlock = textBlock2 };
 _databaseContext.Holder_Block.Add(hb2);
                    }
                    else
                    {
                        Holder_Block hb3 = new Holder_Block() { BlockHolder = blockHolder, TextBlock = textBlock3 };
                        _databaseContext.Holder_Block.Add(hb3);
                    }

                }
                _databaseContext.SaveChanges();
                }

                // DemoImages
                string imgPath =Env.Hosting.ContentRootPath+ "/Areas/Admin/DemoDataResources/demo_background.jpg";              
                byte[] bytes = System.IO.File.ReadAllBytes(imgPath);
                Image img = new Image();
                img.Name = "Ukázkový obrázek";

                ImageData imgData = new ImageData() { Data = bytes };

                img.ImageData = imgData;

                _databaseContext.Images.Add(img);
                _databaseContext.ImageData.Add(imgData);
                
                _databaseContext.SaveChanges();

            

            }            

            
            

           
           



        }
        }
    
}


