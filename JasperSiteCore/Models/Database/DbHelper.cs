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
    /// <summary>
    /// DbHelper is class is used in theme views and administration controllers. It implements the IJasperDataService interface, which
    /// is used for dependency injection.
    /// </summary>
    public class DbHelper : IJasperDataService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <exception cref="DatabaseContextNullException"></exception>
        public DbHelper(IDatabaseContext database)
        {
            this.Database = database ?? throw new DatabaseContextNullException();
            this.Components = new Components(Database, this);
        }

        public IDatabaseContext Database { get; set; }
        public Components Components { get; set; }

        #region Articles
        /// <summary>
        /// Returns list of all articles with categories included.
        /// </summary>
        /// <returns>Returns list of articles.</returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<Article> GetAllArticles(bool throwException = true)
        {
            try
            {
                List<Article> articles = Database.Articles.Include(a => a.Category).ToList();
                return articles;
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw new DatabaseHelperException(ex);
                }
                else return new List<Article>();
                
            }

        }

        /// <summary>
        /// Gets all articles in the specified category.
        /// </summary>
        /// <param name="categoryId">Required category Id.</param>
        /// <returns>Returns list of articles.</returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<Article> GetAllArticles(int categoryId, bool throwException = true)
        {
            try
            {
                return Database.Articles.Where(a => a.CategoryId == categoryId).ToList();
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw new DatabaseHelperException(ex);
                }
                else
                {
                    return new List<Article>();
                }                
            }

        }

        /// <summary>
        /// Returns article by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns single article.</returns>
        /// <exception cref="DatabaseHelperException">Returns single article.</exception>
        public Article GetArticleById(int id)
        {
            try
            {
                return Database.Articles.Where(a => a.Id == id).Single();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }

        }


        /// <summary>
        /// Adds new default article.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public int AddArticle()
        {
            try
            {
                Article articleEntity = new Article()
                {
                    HtmlContent = "Váš článek začíná zde...",
                    Name = "Nový článek",
                    PublishDate = DateTime.Now,

                    // New article will be automatically assigned to uncategorized category
                    CategoryId = GetAllCategories().Where(c => c.Name == "Nezařazeno").Single().Id

                };
                Database.Articles.Add(articleEntity);
                Database.SaveChanges();
                return articleEntity.Id;
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }


        }

        /// <summary>
        /// Updates article with new values.
        /// </summary>
        /// <param name="articleViewModel"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void EditArticle(EditArticleViewModel articleViewModel)
        {
           
            try
            {
               
                Article oldArticleToChange = Database.Articles.Where(a => a.Id == articleViewModel.Id).Single();
                oldArticleToChange.HtmlContent = articleViewModel.HtmlContent;
                oldArticleToChange.Name = articleViewModel.Name;
                oldArticleToChange.PublishDate = (DateTime)articleViewModel.PublishDate;
                oldArticleToChange.CategoryId = articleViewModel.SelectedCategoryId;
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }

        }

        /// <summary>
        /// Deletes article from database.
        /// </summary>
        /// <param name="articleId"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void DeleteArticle(int articleId)
        {
            try
            {
                IDatabaseContext database = Database;
                Article articleToDelete = database.Articles.Where(a => a.Id == articleId).Single();
                database.Articles.Remove(articleToDelete);
                database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException("Specified article could not be found.",ex);
            }

        }
        #endregion

        #region Categories
        /// <summary>
        /// Returns list of all categories.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<Category> GetAllCategories(bool throwException = true)
        {
            try
            {
                return Database.Categories.Include(c => c.Articles).ToList();
            }
            catch (Exception ex)
            {
                if(throwException)
                {
                    throw new DatabaseHelperException(ex);
                }
                else
                {
                    return new List<Category>();
                }                
            }


        }

        /// <summary>
        /// Returns category name by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public string GetCategoryNameById(int id, bool throwException = true)
        {
            try
            {
                return Database.Categories.Where(c => c.Id == id).Single().Name;
            }
            catch (Exception ex)
            {
                if(throwException)
                {
                    throw new DatabaseHelperException(ex);
                }
                else
                {
                    return string.Empty;
                }
            
            }

        }

        /// <summary>
        /// Adds new category.
        /// </summary>
        /// <param name="categoryName">Category name.</param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void AddNewCategory(string categoryName)
        {
            try
            {
                Database.Categories.Add(new Category() { Name = categoryName });
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }

        }

        /// <summary>
        /// Deletes category with corresponding Id and marks assigned articles as uncategorized.
        /// </summary>
        /// <param name="categoryId">Id of the category to be removed.</param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void DeleteCategory(int categoryId)
        {

            try
            {
                Category categoryToBeRemoved = GetAllCategories().Where(c => c.Id == categoryId).Single();
                Category categoryForUnassignedArticles = GetAllCategories().Where(c => c.Name == "Nezařazeno").Single();

                if (categoryToBeRemoved.Name == "Nezařazeno")
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
                Database.SaveChanges();


                Database.Categories.Remove(categoryToBeRemoved);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }

        }

        /// <summary>
        /// If uncategorized category exists, then returns true.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public bool UncategorizedCategoryExists()
        {
            try
            {
                Category uncategorized = Database.Categories.Where(c => c.Name == "Nezařazeno").Single();
                if (uncategorized != null) return true; else return false;
            }
            catch
            {
                return false;
            }      
        }

        /// <summary>
        /// Adds uncategorized category 
        /// </summary>
        /// <exception cref="DatabaseHelperException"></exception>
        public void CreateUncategorizedCategory()
        {          
            try
            {
                if (UncategorizedCategoryExists()) throw new DatabaseHelperException("Uncategorized category already exists."); 
                Category uncategorized = new Category() { Name = "Nezařazeno" };
                Database.Categories.Add(uncategorized);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException("Uncategorized category could not be created.",ex);
            }
        }

        #endregion

        #region Users

        /// <summary>
        /// Returns user with the passed name.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public User GetUserWithUsername(string username)
        {
            try
            {
                return Database.Users.Include(u => u.Role).Where(u => u.Username.Trim() == username.Trim()).Single();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);               
            }
        }

        /// <summary>
        /// Returns user with passed Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public User GetUserById(int userId)
        {
            try
            {
                return Database.Users.Include(u => u.Role).Where(u => u.Id == userId).Single();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Returns list of all users.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<User> GetAllUsers()
        {
            try
            {
                return Database.Users.Include(u => u.Role).ToList();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Hashes, adds salt and saves new password for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newHashedPassword"></param>
        /// <param name="newSalt"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void ChangePassword(int userId, string newHashedPassword, string newSalt)
        {
            try
            {
                User u = GetUserById(userId);
                u.Password = newHashedPassword;
                u.Salt = newSalt;
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="u"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void AddNewUser(User u)
        {
            try
            {
                Database.Users.Add(u);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Deletes user by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void DeleteUserById(int id)
        {
            try
            {
                User goner = Database.Users.Where(u => u.Id == id).Single();
                Database.Users.Remove(goner);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException("Specified user could not be found",ex) ;
            }
        }

        /// <summary>
        /// Returns the list of users with "admin" role
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<User> GetAllAdministrators()
        {
            try
            {
                int adminRoleId = Database.Roles.Where(r => r.Name == "admin").Single().Id;
                return Database.Users.Where(u=>u.RoleId == adminRoleId).ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException("List of all admin users could not be fetched.", ex);
            }
        }

        #endregion

        #region Roles
        /// <summary>
        /// Returns list of all user roles.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<Role> GetAllRoles()
        {
            try
            {
                return Database.Roles.ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }
        }
        #endregion

        public void SaveChanges()
        {
            Database.SaveChanges();
        }

        #region Themes

        /// <summary>
        /// Returns list of all themes.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<Theme> GetAllThemes()
        {
            try
            {
                return Database.Themes.ToList();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Deletes theme by name.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void DeleteThemeByName(string name)
        {
            try
            {
                Theme goner = Database.Themes.Where(t => t.Name == name).Single();
                Database.Themes.Remove(goner);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Creates database theme objects with corresponding relationships.
        /// </summary>
        /// <param name="themeNamesToBeAdded"></param>        
        /// <exception cref="DatabaseHelperException"></exception>
        public void AddThemesFromFolderToDatabase(List<string> themeNamesToBeAdded)
        {
            try
            {
                // All themes info, even those that were already registered
                List<ThemeInfo> themesInfo = Configuration.ThemeHelper.GetInstalledThemesInfoByNameAndActive();

                // Subset of new themes that will be stored in DB
                List<ThemeInfo> subset = new List<ThemeInfo>();

                foreach (ThemeInfo themeInfoObject in themesInfo)
                {
                    foreach (string nameToBeAdded in themeNamesToBeAdded)
                    {
                        if (themeInfoObject.ThemeName.Trim() == nameToBeAdded.Trim())
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
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// 1) Removes all themes from database.
        /// 2) All text block and their content will be untouched.
        /// 3) Themes records & block holders in Db will be recreated based on physical files in Theme folder.
        /// 4) All text blocks will be marked as unassigned.
        /// </summary>
        /// <exception cref="DatabaseHelperException"></exception>
        public void Reconstruct_Theme_TextBlock_BlockHolder_HolderBlockDatabase()
        {
            try
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
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Sometimes user can manually add a theme, without registering that change in the database.
        /// Therefore it is necessary to check whether folder strucure is in accord with database.
        /// </summary>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<string> CheckThemeFolderAndDatabaseIntegrity()
        {
            try
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
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Finds theme names that are registered in Db but not physically present in theme folder.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<string> FindManuallyDeletedThemes()
        {
            try
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
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Returns current theme Id.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public int GetCurrentThemeIdFromDb()
        {
            try
            {
                string themeName = Configuration.GlobalWebsiteConfig.ThemeName;
                int id = Database.Themes.Where(t => t.Name == themeName).Single().Id;
                return id;
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException("Current theme Id could not be found", ex);           
            }
        }

        #endregion

        #region Settings
        /// <summary>
        /// Returns the name of the website based on key: "WebsiteName"
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public string GetWebsiteName(bool throwException = true)
        {
            try
            {
                return Database.Settings.Where(s => s.Key == "WebsiteName").Select(s => s.Value).Single();
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw new DatabaseHelperException(ex);
                }
                else return string.Empty;
               
            }
        }

        /// <summary>
        /// Sets the website name based od key: "WebsiteName"
        /// </summary>
        /// <param name="newWebsiteName"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void SetWebsiteName(string newWebsiteName)
        {
            try
            {
                // search for setting
                Setting websiteNameSetting = Database.Settings.Where(s => s.Key.Trim() == "WebsiteName").Single();
                websiteNameSetting.Value = newWebsiteName;
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }
        }
        #endregion

        #region Blocks and Holders

        /// <summary>
        /// Returns list of all BlockHolders
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<BlockHolder> GetAllBlockHolders()
        {
            try
            {
                return Database.BlockHolders.Include(b => b.Theme).ToList();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Returns list of all TextBlocks
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<TextBlock> GetAllTextBlocks()
        {
            try
            {
                return Database.TextBlocks.ToList();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Returns all Holder_Blocks (= join table for Holder and TextBlocks)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<Holder_Block> GetAllHolder_Blocks()
        {
            try
            {
                return Database.Holder_Block.ToList();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Adds new block and returns it.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public TextBlock AddNewBlock(TextBlock block)
        {
            try
            {
                Database.TextBlocks.Add(block);
                Database.SaveChanges();
                return Database.TextBlocks.Where(b => b.Name == block.Name).Single();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Adds new holder_block (= join table)
        /// </summary>
        /// <param name="hb"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void AddNewHolder_Block(Holder_Block hb)
        {
            try
            {
                Database.Holder_Block.Add(hb);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            };
        }

        /// <summary>
        /// Deletes a text block by ID. 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void DeleteTextBlockById(int id)
        {
            try
            {
                TextBlock goner = Database.TextBlocks.Where(t => t.Id == id).Single();
                Database.TextBlocks.Remove(goner);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Adds holder to block.
        /// </summary>
        /// <param name="holderId"></param>
        /// <param name="blockId"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void AddHolderToBlock(int holderId, int blockId)
        {
            try
            {
                Holder_Block h_b = new Holder_Block() { BlockHolderId = holderId, TextBlockId = blockId };
                Database.Holder_Block.Add(h_b);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Removes holder from block.
        /// </summary>
        /// <param name="holderId"></param>
        /// <param name="blockId"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void RemoveHolderFromBlock(int holderId, int blockId)
        {
            try
            {
                Holder_Block goner_h_b = Database.Holder_Block.Where(h_b => h_b.BlockHolderId == holderId && h_b.TextBlockId == blockId).Single();
                Database.Holder_Block.Remove(goner_h_b);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Deletes block holder by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void DeleteBlockHolderById(int id)
        {
            try
            {
                BlockHolder goner = Database.BlockHolders.Where(bh => bh.Id == id).Single();
                Database.BlockHolders.Remove(goner);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Returns text block by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public TextBlock GetTextBlockById(int id)
        {
            try
            {
                return Database.TextBlocks.Where(tb => tb.Id == id).Single();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Returns Holder_Block join table
        /// </summary>
        /// <param name="textBlockId"></param>
        /// <param name="holderId"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public Holder_Block GetHolder_Block_JoinTable(int textBlockId, int holderId)
        {
            try
            {
                Holder_Block holder = Database.Holder_Block.Where(h => h.TextBlockId == textBlockId && h.BlockHolderId == holderId).Single();
                return holder;
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

        /// <summary>
        /// Text blocks are stored in holders in custom order. This methods specifies in which order text blocks
        /// are displayed on page.
        /// </summary>
        /// <param name="textBlockId"></param>
        /// <param name="holderId"></param>
        /// <param name="order"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void SaveTextBlockOrderNumberInHolder(int textBlockId, int holderId, int order)
        {
            try
            {
                Holder_Block holder = Database.Holder_Block.Where(h => h.TextBlockId == textBlockId && h.BlockHolderId == holderId).Single();
                holder.Order = order;
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }

        }
        #endregion
        
        #region Images

        /// <summary>
        /// Returns list of all images.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public List<Image> GetAllImages()
        {
            try
            {
                return Database.Images.Include(i => i.ImageData).ToList();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }


        /// <summary>
        /// Deletes image by Id.
        /// </summary>
        /// <param name="imgId"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        public void DeleteImageById(int imgId)
        {
            try
            {
                ImageData imgToBeRemoved = Database.ImageData.Where(i => i.Id == imgId).Single();
                Database.ImageData.Remove(imgToBeRemoved);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }
        #endregion
        
    }

    public interface IJasperDataService
    {
        // Properties
        IDatabaseContext Database { get; set; }
        Components Components { get; set; }

        // Methods
        List<Article> GetAllArticles(bool throwException = true);
        List<Article> GetAllArticles(int categoryId, bool throwException = true);
        Article GetArticleById(int id);
        int AddArticle();
        void EditArticle(EditArticleViewModel articleViewModel);
        void DeleteArticle(int articleId);
        List<Category> GetAllCategories(bool throwException = true);
        string GetCategoryNameById(int id, bool throwException = true);
        void AddNewCategory(string categoryName);
        void DeleteCategory(int categoryId);
        User GetUserWithUsername(string username);
        User GetUserById(int userId);
        List<User> GetAllUsers();
        List<Role> GetAllRoles();
        void ChangePassword(int userId, string newHashedPassword, string newSalt);
        void SaveChanges();
        string GetWebsiteName(bool throwException = true);
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
        Holder_Block GetHolder_Block_JoinTable(int textBlockId, int holderId);
        void SaveTextBlockOrderNumberInHolder(int textBlockId, int holderId, int order);
        void DeleteImageById(int imgId);
        void AddNewUser(User u);
        void DeleteUserById(int id);
        List<string> CheckThemeFolderAndDatabaseIntegrity();
        List<string> FindManuallyDeletedThemes();
    }
}
