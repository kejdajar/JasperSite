using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JasperSiteCore.Areas.Admin.Models;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Models.Providers;
using JasperSiteCore.Models;
using Microsoft.EntityFrameworkCore;
using JasperSiteCore.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Html;
using System.Text;
using JasperSiteCore.Helpers;

namespace JasperSiteCore.Models.Database
{
    public class DbHelper: IJasperDataService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <exception cref="DatabaseContextNullException"></exception>
        public DbHelper(IDatabaseContext database)
        {
            this.Database = database ?? throw new DatabaseContextNullException();
            this.Components = new Components(Database,this);
        }

        public IDatabaseContext Database { get; set; }
        public Components Components { get; set; }
        
        /// <summary>
        /// Returns list of all articles with categories included.
        /// </summary>
        /// <returns>Returns list of all articles.</returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<Article> GetAllArticles()
        {
            try
            {
                List<Article> articles = Database.Articles.Include(a => a.Category).ToList();
                return articles;
            }
           catch(Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }      
            
        }

        public List<Article> GetAllArticles(int categoryId)
        {
            if (Database.Articles.Any())
            {
                return Database.Articles.Where(a => a.CategoryId == categoryId).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns article by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns single article.</returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public Article GetArticleById(int id)
        {          
            try
            {
                return Database.Articles.Where(a => a.Id == id).Single();
            }
            catch(Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }
          
             
           
        }

       

        public int AddArticle()
        {
            IDatabaseContext database = Database;
            JasperSiteCore.Models.Database.Article articleEntity = new Article()
            {
                HtmlContent = "Váš článek začíná zde...",
                Name = "Nový článek",
                PublishDate = DateTime.Now,

                // New article will be automatically assigned to uncategorized category
                CategoryId = GetAllCategories().Where(c => c.Name == "Nezařazeno").Single().Id
               
            };
            database.Articles.Add(articleEntity);
            database.SaveChanges();
            return articleEntity.Id;

        }

        public void EditArticle(EditArticleViewModel articleViewModel)
        {
            IDatabaseContext database = Database;
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
            IDatabaseContext database = Database;
            Article articleToDelete = database.Articles.Where(a => a.Id == articleId).Single();
            database.Articles.Remove(articleToDelete);
            database.SaveChanges();
        }



        public List<Category> GetAllCategories()
        {
            IDatabaseContext database = Database;
            if (database.Categories.Any())
            {
                return database.Categories.Include(c=>c.Articles).ToList();
            }
            else
            {
                return null;
            }
        }

        public string GetCategoryNameById(int id)
        {
            return Database.Categories.Where(c => c.Id == id).Single().Name;
        }

        public void AddNewCategory(string categoryName)
        {
            IDatabaseContext database = Database;
            database.Categories.Add(new Category() { Name = categoryName });
            database.SaveChanges();
        }

        /// <summary>
        /// Deletes category with corresponding Id and marks assigned articles as uncategorized.
        /// </summary>
        /// <param name="categoryId">Id of the category to be removed.</param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void DeleteCategory(int categoryId)
        {
              IDatabaseContext database = Database;
              Category categoryToBeRemoved = GetAllCategories().Where(c => c.Id == categoryId).Single();
              Category  categoryForUnassignedArticles = GetAllCategories().Where(c => c.Name == "Nezařazeno").Single();

            if (categoryToBeRemoved.Name=="Nezařazeno")
            {
                throw new DatabaseHelperException("Výchozí rubriku \"nezařazeno\" není možné odstranit.");
            }

            // EF Core - default on delete cascade - assigned articles would be removed by default
            // This will prevent assigned articles to be deleted
            List<Article> assignedArticles = (from a in GetAllArticles()
                                                  where a.CategoryId == categoryId
                                                  select a).ToList();

                // Articles will be now returned to the DB as uncategorized               
                foreach (Article returningArticle in assignedArticles)
                {
                    returningArticle.CategoryId = categoryToBeRemoved.Id;
                }
                database.SaveChanges();

               
                database.Categories.Remove(categoryToBeRemoved);
                database.SaveChanges();
           
            
        }


       

        // **************** USERS **************** //
        public User GetUserWithUsername(string username)
        {
            return Database.Users.Include(u=>u.Role).Where(u => u.Username.Trim() == username.Trim()).Single();
        }

        public User GetUserById(int userId)
        {
            return Database.Users.Include(u => u.Role).Where(u => u.Id == userId).Single();
        }

        public List<User> GetAllUsers()
        {
            return Database.Users.Include(u => u.Role).ToList();
        }

        public List<Role> GetAllRoles()
        {
            return Database.Roles.ToList();
        }

        public void ChangePassword(int userId, string newHashedPassword, string newSalt)
        {
            User u = GetUserById(userId);
            u.Password = newHashedPassword;
            u.Salt = newSalt;
            Database.SaveChanges();
        }

        public void SaveChanges()
        {
            Database.SaveChanges();
        }

        // Settings
        public string GetWebsiteName()
        {
            return Database.Settings.Where(s => s.Key == "WebsiteName").Select(s => s.Value).Single();
        }
        public void SetWebsiteName(string newWebsiteName)
        {
            // search for setting
            Setting websiteNameSetting = Database.Settings.Where(s => s.Key.Trim() == "WebsiteName").Single();
            websiteNameSetting.Value = newWebsiteName;
            Database.SaveChanges();
        }

        // Holders
        public List<BlockHolder> GetAllBlockHolders()
        {
            return Database.BlockHolders.Include(b=>b.Theme).ToList();
        }

        public List<Theme> GetAllThemes()
        {
            return Database.Themes.ToList();
        }

        public List<TextBlock> GetAllTextBlocks()
        {
            return Database.TextBlocks.ToList();
        }

        public List<Holder_Block> GetAllHolder_Blocks()
        {
            return Database.Holder_Block.ToList();
        }


        public TextBlock AddNewBlock(TextBlock block)
        {
            Database.TextBlocks.Add(block);
            Database.SaveChanges();
            return Database.TextBlocks.Where(b => b.Name == block.Name).Single();
        }

        public void AddNewHolder_Block(Holder_Block hb)
        {
            Database.Holder_Block.Add(hb);
            Database.SaveChanges();
        }

        public void DeleteTextBlockById(int id)
        {
           TextBlock goner= Database.TextBlocks.Where(t => t.Id == id).Single();
            Database.TextBlocks.Remove(goner);
            Database.SaveChanges();
        }

        public void AddHolderToBlock(int holderId, int blockId)
        {
            Holder_Block h_b = new Holder_Block() { BlockHolderId = holderId, TextBlockId = blockId };
            Database.Holder_Block.Add(h_b);
            Database.SaveChanges();
        }

        public void RemoveHolderFromBlock(int holderId, int blockId)
        {
            Holder_Block goner_h_b = Database.Holder_Block.Where(h_b => h_b.BlockHolderId == holderId && h_b.TextBlockId == blockId).Single();
            Database.Holder_Block.Remove(goner_h_b);
            Database.SaveChanges();
        }

        public void DeleteBlockHolderById(int id)
        {
            BlockHolder goner = Database.BlockHolders.Where(bh => bh.Id == id).Single();
            Database.BlockHolders.Remove(goner);
            Database.SaveChanges();
        }

      
        public void DeleteThemeByName(string name)
        {
            Theme goner = Database.Themes.Where(t => t.Name == name).Single();
            Database.Themes.Remove(goner);
            Database.SaveChanges();          

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
                Database.Themes.Add(theme);
                Database.SaveChanges();

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
                    Database.BlockHolders.Add(blockHolder);

                }
                Database.SaveChanges();

            }
        }

        public void Reconstruct_Theme_TextBlock_BlockHolder_HolderBlockDatabase()
        {
            // Delete all corrupted themes records
            Database.Themes.RemoveRange(Database.Themes);
            Database.SaveChanges();

            // Old text blocks will not be deleted

            List<ThemeInfo> themesInfo = Configuration.ThemeHelper.GetInstalledThemesInfoByNameAndActive();
            foreach (ThemeInfo ti in themesInfo)
            {
                Theme theme = new Theme() { Name = ti.ThemeName };
                Database.Themes.Add(theme);
                Database.SaveChanges();

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
                    Database.BlockHolders.Add(blockHolder);                    

                }
                Database.SaveChanges();

            }
        }

       public List<Image> GetAllImages()
        {
            return Database.Images.Include(i=>i.ImageData).ToList();
        }

        public TextBlock GetTextBlockById(int id)
        {
            return Database.TextBlocks.Where(tb => tb.Id == id).Single();
        }

        public Holder_Block GetTextBlockOrderNumberInHolder(int textBlockId,int holderId)
        {
            Holder_Block holder = Database.Holder_Block.Where(h => h.TextBlockId == textBlockId && h.BlockHolderId == holderId).Single();
            return holder;
        }

        public void SaveTextBlockOrderNumberInHolder(int textBlockId, int holderId, int order)
        {
            Holder_Block holder = Database.Holder_Block.Where(h => h.TextBlockId == textBlockId && h.BlockHolderId == holderId).Single();
            holder.Order = order;
            Database.SaveChanges();
           
        }

        public void DeleteImageById(int imgId)
        {
            ImageData imgToBeRemoved = Database.ImageData.Where(i => i.Id == imgId).Single();
            Database.ImageData.Remove(imgToBeRemoved);
            Database.SaveChanges();
        }

        public void AddNewUser(User u)
        {
            Database.Users.Add(u);
            Database.SaveChanges();
        }

        public void DeleteUserById(int id)
        {
            User goner = Database.Users.Where(u => u.Id == id).Single();
            Database.Users.Remove(goner);
            Database.SaveChanges();
        }


        /*FORMERLY IN THEME HELPER CLASS*/
        

        /// <summary>
        /// Sometimes user can manually add a theme, without registering that change in the database.
        /// Therefore it is necessary to check whether folder strucure is in accord with database.
        /// </summary>
        public List<string> CheckThemeFolderAndDatabaseIntegrity()
        {
            List<ThemeInfo> themeInfosFromFolder = Configuration.ThemeHelper.GetInstalledThemesInfo();
            List<Theme> themesStoredInDb = GetAllThemes();

            // All names that are to be found in DB will be removed from this list
            // Eventually this list will contain all names that has not yet been stored in DB
            List<string> allNamesInFolder = themeInfosFromFolder.Select(t => t.ThemeName.Trim()).ToList();

            foreach (ThemeInfo folder in themeInfosFromFolder)
            {
                foreach (Theme db in themesStoredInDb)
                {
                    if (folder.ThemeName.Trim() == db.Name.Trim())
                    {
                        allNamesInFolder.Remove(folder.ThemeName.Trim());
                    }
                }
            }

            return allNamesInFolder;
        }

        public List<string> FindManuallyDeletedThemes()
        {
            List<ThemeInfo> themeInfosFromFolder = Configuration.ThemeHelper.GetInstalledThemesInfo();
            List<Theme> themesStoredInDb = GetAllThemes();


            List<string> themeNamesOnlyInDatabaseAndNotInFolder = themesStoredInDb.Select(n => n.Name).ToList();

            foreach (Theme dbName in themesStoredInDb)
            {
                foreach (ThemeInfo folderName in themeInfosFromFolder)
                {
                    if (dbName.Name == folderName.ThemeName)
                    {
                        themeNamesOnlyInDatabaseAndNotInFolder.Remove(dbName.Name);
                    }
                }
            }
            return themeNamesOnlyInDatabaseAndNotInFolder;
        }

        

    }

    public interface IJasperDataService
    {
        IDatabaseContext Database { get; set; }
        Components Components { get; set; }

        List<Article> GetAllArticles();
        List<Article> GetAllArticles(int categoryId);
        Article GetArticleById(int id);
        int AddArticle();
        void EditArticle(EditArticleViewModel articleViewModel);
        void DeleteArticle(int articleId);
        List<Category> GetAllCategories();
        string GetCategoryNameById(int id);
        void AddNewCategory(string categoryName);
        void DeleteCategory(int categoryId);
        User GetUserWithUsername(string username);
        User GetUserById(int userId);
        List<User> GetAllUsers();
        List<Role> GetAllRoles();
        void ChangePassword(int userId, string newHashedPassword, string newSalt);
        void SaveChanges();
        string GetWebsiteName();
        void SetWebsiteName(string newWebsiteName);
        List<BlockHolder> GetAllBlockHolders();
        List<Theme> GetAllThemes();
        List<TextBlock> GetAllTextBlocks();
        List<Holder_Block> GetAllHolder_Blocks();
        TextBlock AddNewBlock(TextBlock block);
        void AddNewHolder_Block(Holder_Block hb);
        void DeleteTextBlockById(int id);
        void AddHolderToBlock(int holderId, int blockId);
        void RemoveHolderFromBlock(int holderId, int blockId);
        void DeleteBlockHolderById(int id);
        void DeleteThemeByName(string name);
        void AddThemesFromFolderToDatabase(List<string> themeNamesToBeAdded);
        void Reconstruct_Theme_TextBlock_BlockHolder_HolderBlockDatabase();
        List<Image> GetAllImages();
        TextBlock GetTextBlockById(int id);
        Holder_Block GetTextBlockOrderNumberInHolder(int textBlockId, int holderId);
        void SaveTextBlockOrderNumberInHolder(int textBlockId, int holderId, int order);
        void DeleteImageById(int imgId);
        void AddNewUser(User u);
        void DeleteUserById(int id);
        List<string> CheckThemeFolderAndDatabaseIntegrity();
        List<string> FindManuallyDeletedThemes();

    }



}
