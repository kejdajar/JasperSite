﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JasperSiteCore.Areas.Admin.Models;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Models.Providers;
using Microsoft.EntityFrameworkCore;

namespace JasperSiteCore.Models.Database
{
    public class DbHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <exception cref="DatabaseContextNullException"></exception>
        public DbHelper(IDatabaseContext database)
        {
            this._db = database ?? throw new DatabaseContextNullException();
        }

        public IDatabaseContext _db;

        public List<Article> GetAllArticles()
        {
            if (_db.Articles.Any())
            {
                return _db.Articles.ToList();
            }
            else
            {
                return null;
            }
        }

        public List<Article> GetAllArticles(int categoryId)
        {
            if (_db.Articles.Any())
            {
                return _db.Articles.Where(a => a.CategoryId == categoryId).ToList();
            }
            else
            {
                return null;
            }
        }

        public Article GetArticleById(int id)
        {
            IDatabaseContext database = _db;

            if (database.Articles.Any())
            {
                return database.Articles.Where(a => a.Id == id).Single();
            }
            else
            {
                return null;
            }
        }

       

        public int AddArticle()
        {
            IDatabaseContext database = _db;
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

        public void EditArticle(JasperSiteCore.Areas.Admin.ViewModels.EditArticleViewModel articleViewModel)
        {
            IDatabaseContext database = _db;
            Article oldArticleToChange = database.Articles.Where(a => a.Id == articleViewModel.Id).Single();


            oldArticleToChange.HtmlContent = articleViewModel.HtmlContent;
            oldArticleToChange.Name = articleViewModel.Name;
            oldArticleToChange.PublishDate = articleViewModel.PublishDate;
            oldArticleToChange.CategoryId = articleViewModel.SelectedCategoryId;

            // database.Articles.Add(oldArticleToChange);
            database.SaveChanges();
        }


        public void DeleteArticle(int articleId)
        {
            IDatabaseContext database = _db;
            Article articleToDelete = database.Articles.Where(a => a.Id == articleId).Single();
            database.Articles.Remove(articleToDelete);
            database.SaveChanges();
        }



        public List<Category> GetAllCategories()
        {
            IDatabaseContext database = _db;
            if (database.Categories.Any())
            {
                return database.Categories.ToList();
            }
            else
            {
                return null;
            }
        }

        public string GetCategoryNameById(int id)
        {
            return _db.Categories.Where(c => c.Id == id).Single().Name;
        }

        public void AddNewCategory(string categoryName)
        {
            IDatabaseContext database = _db;
            database.Categories.Add(new Category() { Name = categoryName });
            database.SaveChanges();
        }

        public void DeleteCategory(int categoryId)
        {
            IDatabaseContext database = _db;
            Category goner = database.Categories.Where(c => c.Id == categoryId).Single();
            database.Categories.Remove(goner);
            database.SaveChanges();
        }


       

        // **************** USERS **************** //
        public User GetUserWithUsername(string username)
        {
            return _db.Users.Where(u => u.Username.Trim() == username.Trim()).Single();
        }

        public User GetUserById(int userId)
        {
            return _db.Users.Include(u => u.Role).Where(u => u.Id == userId).Single();
        }

        public List<User> GetAllUsers()
        {
            return _db.Users.Include(u => u.Role).ToList();
        }

        public List<Role> GetAllRoles()
        {
            return _db.Roles.ToList();
        }

        public void ChangePassword(int userId, string newHashedPassword, string newSalt)
        {
            User u = GetUserById(userId);
            u.Password = newHashedPassword;
            u.Salt = newSalt;
            _db.SaveChanges();
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        // Settings
        public string GetWebsiteName()
        {
            return _db.Settings.Where(s => s.Key == "WebsiteName").Select(s => s.Value).Single();
        }
        public void SetWebsiteName(string newWebsiteName)
        {
            // search for setting
            Setting websiteNameSetting = _db.Settings.Where(s => s.Key.Trim() == "WebsiteName").Single();
            websiteNameSetting.Value = newWebsiteName;
            _db.SaveChanges();
        }

        // Holders
        public List<BlockHolder> GetAllBlockHolders()
        {
            return _db.BlockHolders.ToList();
        }

        public List<Theme> GetAllThemes()
        {
            return _db.Themes.ToList();
        }

        public List<TextBlock> GetAllTextBlocks()
        {
            return _db.TextBlocks.ToList();
        }

        public List<Holder_Block> GetAllHolder_Blocks()
        {
            return _db.Holder_Block.ToList();
        }


        public TextBlock AddNewBlock(TextBlock block)
        {
            _db.TextBlocks.Add(block);
            _db.SaveChanges();
            return _db.TextBlocks.Where(b => b.Name == block.Name).Single();
        }

        public void AddNewHolder_Block(Holder_Block hb)
        {
            _db.Holder_Block.Add(hb);
            _db.SaveChanges();
        }

        public void DeleteTextBlockById(int id)
        {
           TextBlock goner= _db.TextBlocks.Where(t => t.Id == id).Single();
            _db.TextBlocks.Remove(goner);
            _db.SaveChanges();
        }

        public void AddHolderToBlock(int holderId, int blockId)
        {
            Holder_Block h_b = new Holder_Block() { BlockHolderId = holderId, TextBlockId = blockId };
            _db.Holder_Block.Add(h_b);
            _db.SaveChanges();
        }

        public void RemoveHolderFromBlock(int holderId, int blockId)
        {
            Holder_Block goner_h_b = _db.Holder_Block.Where(h_b => h_b.BlockHolderId == holderId && h_b.TextBlockId == blockId).Single();
            _db.Holder_Block.Remove(goner_h_b);
            _db.SaveChanges();
        }

        public void DeleteBlockHolderById(int id)
        {
            BlockHolder goner = _db.BlockHolders.Where(bh => bh.Id == id).Single();
            _db.BlockHolders.Remove(goner);
            _db.SaveChanges();
        }

        public void DeleteThemeByName(string name)
        {
            Theme goner = _db.Themes.Where(t => t.Name == name).Single();
            _db.Themes.Remove(goner);
            _db.SaveChanges();
        }

        /// <summary>
        /// Creates database theme objects with corresponding relationships.
        /// </summary>
        /// <param name="themeNamesToBeAdded"></param>        
        public void AddThemesFromFolderToDatabase(List<string> themeNamesToBeAdded)
        {
            // All themes info, even those that were already registered
            List<ThemeInfo> themesInfo = Configuration.ThemeHelper.GetInstalledThemesInfoByNameAndActive();

            // Subset of new themes that will be stored in DB
            List<ThemeInfo> subset = new List<ThemeInfo>();

            foreach(ThemeInfo themeInfoObject in themesInfo)
            {
                foreach(string nameToBeAdded in themeNamesToBeAdded)
                {
                    if(themeInfoObject.ThemeName.Trim()==nameToBeAdded.Trim())
                    {
                        subset.Add(themeInfoObject);
                    }
                }
            }



            foreach (ThemeInfo ti in subset)
            {
                Theme theme = new Theme() { Name = ti.ThemeName };
                _db.Themes.Add(theme);
                _db.SaveChanges();

                // Emulation of website configuration for every theme
                GlobalConfigDataProviderJson globalJsonProvider = new GlobalConfigDataProviderJson("jasper.json");
                GlobalWebsiteConfig globalConfig = new GlobalWebsiteConfig(globalJsonProvider);
                globalConfig.SetTemporaryThemeName(ti.ThemeName); // This will not write enytihing to the underlying .json file 
                ConfigurationObjectProviderJson configurationObjectJsonProvider = new ConfigurationObjectProviderJson(globalConfig, "jasper.json");
                WebsiteConfig websiteConfig = new WebsiteConfig(configurationObjectJsonProvider);



                List<string> holders = websiteConfig.BlockHolders;

                foreach (string holderName in holders)
                {
                    BlockHolder blockHolder = new BlockHolder() { Name = holderName, ThemeId = theme.Id };
                    _db.BlockHolders.Add(blockHolder);

                }
                _db.SaveChanges();

            }
        }

        public void Reconstruct_Theme_TextBlock_BlockHolder_HolderBlockDatabase()
        {
            // Delete all corrupted themes records
            _db.Themes.RemoveRange(_db.Themes);
            _db.SaveChanges();

            // Old text blocks will not be deleted

            List<ThemeInfo> themesInfo = Configuration.ThemeHelper.GetInstalledThemesInfoByNameAndActive();
            foreach (ThemeInfo ti in themesInfo)
            {
                Theme theme = new Theme() { Name = ti.ThemeName };
                _db.Themes.Add(theme);
                _db.SaveChanges();

                // Emulation of website configuration for every theme
                GlobalConfigDataProviderJson globalJsonProvider = new GlobalConfigDataProviderJson("jasper.json");
                GlobalWebsiteConfig globalConfig = new GlobalWebsiteConfig(globalJsonProvider);
                globalConfig.SetTemporaryThemeName(ti.ThemeName); // This will not write enytihing to the underlying .json file 
                ConfigurationObjectProviderJson configurationObjectJsonProvider = new ConfigurationObjectProviderJson(globalConfig, "jasper.json");
                WebsiteConfig websiteConfig = new WebsiteConfig(configurationObjectJsonProvider);
                              


                List<string> holders = websiteConfig.BlockHolders;

                foreach (string holderName in holders)
                {
                    BlockHolder blockHolder = new BlockHolder() { Name = holderName, ThemeId = theme.Id };
                    _db.BlockHolders.Add(blockHolder);                    

                }
                _db.SaveChanges();

            }
        }

       public List<Image> GetAllImages()
        {
            return _db.Images.Include(i=>i.ImageData).ToList();
        }

        public TextBlock GetTextBlockById(int id)
        {
            return _db.TextBlocks.Where(tb => tb.Id == id).Single();
        }

    }
}
