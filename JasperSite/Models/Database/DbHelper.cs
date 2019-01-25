using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JasperSite.Areas.Admin.Models;
using JasperSite.Models.Database;
using JasperSite.Models.Providers;
using JasperSite.Models;
using Microsoft.EntityFrameworkCore;
using JasperSite.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Html;
using System.Text;
using JasperSite.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;


namespace JasperSite.Models.Database
{

    // *********************** ADMINISTRATION BUILDING ****************************** //

    /// <summary>
    /// An interface used for injecting data into ADMINISTRATION views. Not suitable for THEME views.
    /// Specified methods SHOULD throw exception on failure.
    /// </summary>
    public interface IJasperDataService
    {
        // Properties
        IDatabaseContext Database { get; set; }
        Components Components { get; set; }

        #region Articles

        /// <summary>
        /// Returns list of all articles with categories included.
        /// </summary>
        /// <returns>Returns list of articles.</returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<Article> GetAllArticles();

        /// <summary>
        /// Gets all articles in the specified category.
        /// </summary>
        /// <param name="categoryId">Required category Id.</param>
        /// <returns>Returns list of articles.</returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<Article> GetAllArticles(int categoryId);

        /// <summary>
        /// Returns article by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns single article.</returns>
        /// <exception cref="DatabaseHelperException">Returns single article.</exception>
        Article GetArticleById(int id);

        /// <summary>
        /// Adds new default article. Returns article Db Id.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        Article AddArticle();

        /// <summary>
        /// Updates article with new values and returns DB attached object.
        /// </summary>
        /// <param name="articleViewModel"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        Article EditArticle(Article article);

        /// <summary>
        /// Deletes article from database.
        /// </summary>
        /// <param name="articleId"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        void DeleteArticle(int articleId);

        #endregion

        #region Categories

        /// <summary>
        /// Renames the category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="newName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DatabaseHelperException"></exception>
        void RenameCategory(int categoryId, string newName);

        /// <summary>
        /// Returns list of all categories.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<Category> GetAllCategories();

        /// <summary>
        /// Returns category name by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        string GetCategoryNameById(int id);

        /// <summary>
        /// Adds new category.
        /// </summary>
        /// <param name="categoryName">Category name.</param>
        /// <exception cref="DatabaseHelperException"></exception>
        Category AddCategory(string categoryName);

        /// <summary>
        /// Deletes category with corresponding Id and marks assigned articles as uncategorized.
        /// </summary>
        /// <param name="categoryId">Id of the category to be removed.</param>
        /// <exception cref="DatabaseHelperException"></exception>
        void DeleteCategory(int categoryId);

        /// <summary>
        /// If uncategorized category exists, then returns true.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        bool UncategorizedCategoryExists();

        /// <summary>
        /// Adds uncategorized category 
        /// </summary>
        /// <exception cref="DatabaseHelperException"></exception>
        void CreateUncategorizedCategory();

        #endregion

        #region Users

        /// <summary>
        /// Returns user with the passed name.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        User GetUserWithUsername(string username);

        /// <summary>
        /// Returns user with passed Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        User GetUserById(int userId);

        /// <summary>
        /// Returns list of all users.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<User> GetAllUsers();

        /// <summary>
        /// Hashes, adds salt and saves new password for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newHashedPassword"></param>
        /// <param name="newSalt"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        void ChangePassword(int userId, string newHashedPassword, string newSalt);

        /// <summary>
        /// Returns the list of users with "admin" role
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<User> GetAllAdministrators();

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="u"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        User AddUser(User u);

        /// <summary>
        /// Deletes user by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        void DeleteUserById(int id);

        #endregion

        #region Roles

        /// <summary>
        /// Returns list of all user roles.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<Role> GetAllRoles();

        #endregion

        #region Themes

        /// <summary>
        /// Returns list of all themes.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<Theme> GetAllThemes();

        /// <summary>
        /// Deletes theme by name.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        void DeleteThemeByName(string name);

        /// <summary>
        /// Creates database theme objects with corresponding relationships.
        /// </summary>
        /// <param name="themeNamesToBeAdded"></param>        
        /// <exception cref="DatabaseHelperException"></exception>
        void AddThemesFromFolderToDatabase(List<string> themeNamesToBeAdded, IRequestCultureFeature culture);

        /// <summary>
        /// 1) Removes all themes from database.
        /// 2) All text block and their content will be untouched.
        /// 3) Themes records & block holders in Db will be recreated based on physical files in Theme folder.
        /// 4) All text blocks will be marked as unassigned.
        /// </summary>
        /// <exception cref="DatabaseHelperException"></exception>
        void ReconstructAndClearThemeData(IRequestCultureFeature culture);

        /// <summary>       
        /// Returns all theme names that have not yet been stored in the database.
        /// </summary>
        /// <exception cref="DatabaseHelperException"></exception>
        List<string> CheckThemeFolderAndDatabaseIntegrity(IRequestCultureFeature culture);

        /// <summary>
        /// Finds theme names that are registered in Db but not physically present in theme folder.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<string> FindManuallyDeletedThemes(IRequestCultureFeature culture);

        /// <summary>
        /// Returns current theme Id.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        int GetCurrentThemeIdFromDb();

        #endregion

        #region Settings

        /// <summary>
        /// Returns the name of the website based on key: "WebsiteName"
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        string GetWebsiteName();

        /// <summary>
        /// Sets the website name based od key: "WebsiteName"
        /// </summary>
        /// <param name="newWebsiteName"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        void SetWebsiteName(string newWebsiteName);

        #endregion

        #region Blocks

        /// <summary>
        /// Returns list of all BlockHolders
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<BlockHolder> GetAllBlockHolders();

        /// <summary>
        /// Returns list of all TextBlocks
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<TextBlock> GetAllTextBlocks();

        /// <summary>
        /// Returns all Holder_Blocks (= join table for Holder and TextBlocks)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<Holder_Block> GetAllHolder_Blocks();

        /// <summary>
        /// Adds new block and returns it.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        TextBlock AddNewBlock(TextBlock block);

        /// <summary>
        /// Adds new holder_block (= join table)
        /// </summary>
        /// <param name="hb"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        Holder_Block AddHolder_Block(Holder_Block hb);

        /// <summary>
        /// Deletes a text block by ID. 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        void DeleteTextBlockById(int id);

        /// <summary>
        /// Adds holder to block.
        /// </summary>
        /// <param name="holderId"></param>
        /// <param name="blockId"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        Holder_Block AddHolderToBlock(int holderId, int blockId);

        /// <summary>
        /// Removes holder from block.
        /// </summary>
        /// <param name="holderId"></param>
        /// <param name="blockId"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        void RemoveHolderFromBlock(int holderId, int blockId);

        /// <summary>
        /// Deletes block holder by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        void DeleteBlockHolderById(int id);

        /// <summary>
        /// Returns text block by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        TextBlock GetTextBlockById(int id);

        /// <summary>
        /// Returns Holder_Block join table
        /// </summary>
        /// <param name="textBlockId"></param>
        /// <param name="holderId"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        Holder_Block GetHolder_Block(int textBlockId, int holderId);

        /// <summary>
        /// Text blocks are stored in holders in custom order. This methods specifies in which order text blocks
        /// are displayed on page.
        /// </summary>
        /// <param name="textBlockId"></param>
        /// <param name="holderId"></param>
        /// <param name="order"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        void SaveTextBlockOrderNumberInHolder(int textBlockId, int holderId, int order);

        #endregion

        #region Images

        /// <summary>
        /// Returns list of all images.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<Image> GetAllImages();

        /// <summary>
        /// Deletes image by Id.
        /// </summary>
        /// <param name="imgId"></param>
        /// <exception cref="DatabaseHelperException"></exception>
        void DeleteImageById(int imgId);

        /// <summary>
        /// Returns a image by Id.
        /// </summary>
        /// <param name="imgId"></param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        Image GetImageById(int imgId);

        #endregion

        #region Url

        /// <summary>
        /// Returns all UrlRewrite records.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidUrlRewriteException"></exception>
        List<UrlRewrite> GetAllUrls();

        /// <summary>
        /// Returns list of all URL for the specified article.
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidUrlRewriteException"></exception>
        List<string> GetUrls(int articleId);

        /// <summary>
        /// Updates or creates a new URL rewrite rule.
        /// </summary>
        /// <param name="article"></param>
        /// <param name="url"></param>
        /// <exception cref="InvalidUrlRewriteException"></exception>
        void SetUrl(Article article, string url);

        /// <summary>
        /// Returns the first URL found.
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        /// <exception cref="InvalidUrlRewriteException"></exception>
        string Url(Article article);

        /// <summary>
        /// Deletes all records with specified URL.
        /// </summary>
        /// <param name="url"></param>
        /// <exception cref="InvalidUrlRewriteException"></exception>
        void DeleteUrl(string url);

        /// <summary>
        /// Transforms Theme-relative Url to Root-relative Url
        /// </summary>
        /// <param name="url">Theme-relative Url</param>
        /// <returns></returns>
        /// <exception cref="CustomRoutingException"></exception>
        string File(string url);

        /// <summary>
        /// This method is used for setting Layout of a Razor view page.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="CustomRoutingException"></exception>
        string Layout(string url, HttpRequest request);

        #endregion
    }


    /// <summary>
    /// DbHelper is class is used in administration pages and administration controllers. It implements the IJasperDataService interface, which
    /// is used for dependency injection. This class is not suitable for building themes, as it throws exceptions.
    /// </summary>
    public class DbHelper : IJasperDataService
    {
        
        public DbHelper(IDatabaseContext database)
        {
            this.Database = database ?? throw new DatabaseContextNullException();
            this.Components = new Components(Database, this);
        }

        // Properties
        public IDatabaseContext Database { get; set; }
        public Components Components { get; set; }

        #region Articles
       
    
       /// <see cref="IJasperDataService.GetAllArticles"/>     
       public List<Article> GetAllArticles()
        {
            try
            {
                List<Article> articles = Database.Articles.Include(a => a.Category).ToList();
                return articles;
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }

        }

       
        /// <see cref="IJasperDataService.GetAllArticles(int)"/>  
        public List<Article> GetAllArticles(int categoryId)
        {
            try
            {
                return Database.Articles.Where(a => a.CategoryId == categoryId).ToList();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);

            }

        }
       
        /// <see cref="IJasperDataService.GetArticleById(int)"/> 
        public Article GetArticleById(int id)
        {
            try
            {
                return Database.Articles.Where(a => a.Id == id).Include(c=>c.Category).Single();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }

        }


        
        /// <see cref="IJasperDataService.AddArticle"/> 
        public Article AddArticle()
        {
            try
            {
                Article articleEntity = new Article()
                {
                    HtmlContent = "",
                    Name = "Lorem Ipsum",
                    PublishDate = DateTime.Now,
                    // New article will be automatically assigned to uncategorized category
                    CategoryId = GetAllCategories().Where(c => c.Name == "Uncategorized").Single().Id
                };

                Database.Articles.Add(articleEntity);
                Database.SaveChanges();
                return articleEntity;
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }
        }

       
        /// <see cref="IJasperDataService.EditArticle(Article)"/> 
        public Article EditArticle(Article article)
        {
            try
            {
                Article oldArticleToChange = Database.Articles.Where(a => a.Id == article.Id).Single();
                oldArticleToChange.HtmlContent = article.HtmlContent;
                oldArticleToChange.Name = article.Name;
                oldArticleToChange.PublishDate = (DateTime)article.PublishDate;
                oldArticleToChange.CategoryId = article.CategoryId;
                oldArticleToChange.Publish = article.Publish;
                oldArticleToChange.Keywords = article.Keywords;
                Database.SaveChanges();
                return oldArticleToChange;
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }

        }
     
        /// <see cref="IJasperDataService.DeleteArticle(int)"/> 
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
                throw new DatabaseHelperException("Specified article could not be found.", ex);
            }

        }
        #endregion

        #region Categories


        public void RenameCategory(int categoryId, string newName)
        {
           
            if (newName == null)
            {
                throw new ArgumentNullException();
            }

            if(newName == string.Empty)
            {
                throw new DatabaseHelperException("Category name can't be an empty string.");
            }

            try
            {
                Category categoryToBeRenamed = Database.Categories.Where(c => c.Id == categoryId).Single();

                if(categoryToBeRenamed.Name == "Uncategorized")
                {
                    throw new DatabaseHelperException("This default category can't be renamed.");
                }

                categoryToBeRenamed.Name = newName;
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException("Category name could not be updated.",ex);
            }
           
        }
        
        /// <see cref="IJasperDataService.GetAllCategories"/>    
        public List<Category> GetAllCategories()
        {
            try
            {
                return Database.Categories.Include(c => c.Articles).ToList();
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);

            }
        }
     

        /// <see cref="IJasperDataService.GetCategoryNameById(int)"/>    
        public string GetCategoryNameById(int id)
        {
            try
            {
                return Database.Categories.Where(c => c.Id == id).Single().Name;
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }

        }
      
        /// <see cref="IJasperDataService.AddCategory(string)"/>    
        public Category AddCategory(string categoryName)
        {
          
            try
            {
                
                if (categoryName == "Uncategorized")
                {
                    // Check if uncategorized category exists:
                    bool uncategorizedExists = Database.Categories.Any(c => c.Name == "Uncategorized");
                    throw new DatabaseHelperException("There has to be only one Uncategorized category");
                }  


                Category newCategory = new Category() { Name = categoryName };
                Database.Categories.Add(newCategory);
                Database.SaveChanges();
                return newCategory;
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }

        }

      
        /// <see cref="IJasperDataService.DeleteCategory(int)"/>    
        public void DeleteCategory(int categoryId)
        {
            try
            {
                List<Category> allCategories = GetAllCategories();
                Category categoryToBeRemoved = allCategories.Where(c => c.Id == categoryId).Single();
                Category categoryForUnassignedArticles = allCategories.Where(c => c.Name == "Uncategorized").Single();

                if (categoryToBeRemoved.Name == "Uncategorized")
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
                    returningArticle.CategoryId = categoryForUnassignedArticles.Id;
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

       
        /// <see cref="IJasperDataService.UncategorizedCategoryExists"/>    
        public bool UncategorizedCategoryExists()
        {
            try
            {
                Category uncategorized = Database.Categories.Where(c => c.Name == "Uncategorized").Single();
                if (uncategorized != null) return true; else return false;
            }
            catch
            {
                return false;
            }
        }

   
        /// <see cref="IJasperDataService.CreateUncategorizedCategory"/>    
        public void CreateUncategorizedCategory()
        {
            try
            {
                if (UncategorizedCategoryExists()) throw new DatabaseHelperException("Uncategorized category already exists.");
                Category uncategorized = new Category() { Name = "Uncategorized" };
                Database.Categories.Add(uncategorized);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException("Uncategorized category could not be created.", ex);
            }
        }

        #endregion

        #region Users

       
        /// <see cref="IJasperDataService.GetUserWithUsername(string)"/>    
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

        
        /// <see cref="IJasperDataService.GetUserById(int)"/>  
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

       
        /// <see cref="IJasperDataService.GetAllUsers"/>  
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

        
        /// <see cref="IJasperDataService.ChangePassword(int, string, string)"/>  
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

       
        /// <see cref="IJasperDataService.AddUser(User)"/>  
        public User AddUser(User newUser)
        {
            try
            {
                Database.Users.Add(newUser);
                Database.SaveChanges();
                return newUser;
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

       
        /// <see cref="IJasperDataService.DeleteUserById(int)"/>  
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

                throw new DatabaseHelperException("Specified user could not be found", ex);
            }
        }

      
        /// <see cref="IJasperDataService."/>  
        public List<User> GetAllAdministrators()
        {
            try
            {
                int adminRoleId = Database.Roles.Where(r => r.Name == "admin").Single().Id;
                return Database.Users.Where(u => u.RoleId == adminRoleId).ToList();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException("List of all admin users could not be fetched.", ex);
            }
        }

        #endregion

        #region Roles

       
        /// <see cref="IJasperDataService.GetAllRoles"/>    
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

        #region Themes

       
        /// <see cref="IJasperDataService.GetAllThemes"/>
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

       
        /// <see cref="IJasperDataService.DeleteThemeByName(string)"/>
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

        
        ///<see cref="IJasperDataService.AddThemesFromFolderToDatabase(List{string})"/>
        public void AddThemesFromFolderToDatabase(List<string> themeNamesToBeAdded, IRequestCultureFeature culture)
        {
            try
            {
                // All themes info, even those that were already registered
                List<ThemeInfo> themesInfo = Configuration.ThemeHelper.GetInstalledThemesInfoByNameAndActive(culture);

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

       
        /// <see cref="IJasperDataService.ReconstructAndClearThemeData"/>
        public void ReconstructAndClearThemeData(IRequestCultureFeature culture)
        {
            try
            {
                // Delete all corrupted themes records
                Database.Themes.RemoveRange(Database.Themes);
                Database.SaveChanges();

                // Old text blocks will not be deleted

                List<ThemeInfo> themesInfo = Configuration.ThemeHelper.GetInstalledThemesInfoByNameAndActive(culture);
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

      
        /// <see cref="IJasperDataService.CheckThemeFolderAndDatabaseIntegrity"/>
        public List<string> CheckThemeFolderAndDatabaseIntegrity(IRequestCultureFeature culture)
        {
            try
            {
                List<ThemeInfo> themeInfosFromFolder = Configuration.ThemeHelper.GetInstalledThemesInfo(culture);
                List<Theme> themesStoredInDb = GetAllThemes();

                // All names that are to be found in DB will be removed from this list
                // Eventually this list will contain all names that have not yet been stored in the DB.
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

       
        /// <see cref="IJasperDataService.FindManuallyDeletedThemes"/>
        public List<string> FindManuallyDeletedThemes(IRequestCultureFeature culture)
        {
            try
            {
               
                List<ThemeInfo> themeInfosFromFolder = Configuration.ThemeHelper.GetInstalledThemesInfo(culture);
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

       
        /// <see cref="IJasperDataService.GetCurrentThemeIdFromDb"/>
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
        
        /// <see cref="IJasperDataService.GetWebsiteName"/>
        public string GetWebsiteName()
        {
            try
            {
                return Database.Settings.Where(s => s.Key == "WebsiteName").Select(s => s.Value).Single();
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }
        }

       
        /// <see cref="IJasperDataService.SetWebsiteName(string)"/>
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

        #region Blocks

       
        /// <see cref="IJasperDataService.GetAllBlockHolders"/>
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

       
        /// <see cref="IJasperDataService.GetAllTextBlocks"/>
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

       
        /// <see cref="IJasperDataService.GetAllHolder_Blocks"/>
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

       
        /// <see cref="IJasperDataService.AddNewBlock(TextBlock)"/>
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

      
        /// <see cref="IJasperDataService.AddHolder_Block(Holder_Block)"/>
        public Holder_Block AddHolder_Block(Holder_Block hb)
        {
            try
            {
                Database.Holder_Block.Add(hb);
                Database.SaveChanges();
                return hb;
            }
            catch (Exception ex)
            {
                throw new DatabaseHelperException(ex);
            };
        }

        
        /// <see cref="IJasperDataService.DeleteTextBlockById(int)"/>
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

      
        /// <see cref="IJasperDataService.AddHolderToBlock(int, int)"/>
        public Holder_Block AddHolderToBlock(int holderId, int blockId)
        {
            try
            {
                Holder_Block h_b = new Holder_Block() { BlockHolderId = holderId, TextBlockId = blockId };
                Database.Holder_Block.Add(h_b);
                Database.SaveChanges();
                return h_b;
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex);
            }
        }

     
        /// <see cref="IJasperDataService.RemoveHolderFromBlock(int, int)"/>
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

        
        /// <see cref="IJasperDataService.DeleteBlockHolderById(int)"/>
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

       
        /// <see cref="IJasperDataService.GetTextBlockById(int)"/>
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

      
        /// <see cref="IJasperDataService.GetHolder_Block(int, int)"/>
        public Holder_Block GetHolder_Block(int textBlockId, int holderId)
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

      
        /// <see cref="IJasperDataService.SaveTextBlockOrderNumberInHolder(int, int, int)/>
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
      
        /// <see cref="IJasperDataService.GetAllImages"/>
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


     
        /// <see cref="IJasperDataService.DeleteImageById(int)"/>
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

      
        /// <see cref="IJasperDataService.GetImageById(int)"/>
        public Image GetImageById(int imgId)
        {
            try
            {
                return Database.Images.Where(i => i.Id == imgId).Single();
            }
            catch(Exception ex)
            {
                throw new DatabaseHelperException(ex);
            }
        }

        #endregion

        #region Url

        
        /// <see cref="IJasperDataService.GetAllUrls"/>
        public List<UrlRewrite> GetAllUrls()
        {
            try
            {
                return Database.UrlRewrite.ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidUrlRewriteException(ex);
            }
        }

       
        /// <see cref="IJasperDataService.GetUrls(int)"/>
        public List<string> GetUrls(int articleId)
        {
            try
            {
                List<string> urls = Database.UrlRewrite.Where(ur => ur.ArticleId == articleId).Select(s => s.Url).ToList();
                return urls;
            }
            catch (Exception ex)
            {
                throw new InvalidUrlRewriteException(ex);
            }
        }

     
        /// <see cref="IJasperDataService.SetUrl(Article, string)"/>
        public void SetUrl(Article article, string url)
        {
            UrlRewrite urlRewrite = new UrlRewrite()
            {
                Article = article,
                Url = url
            };

            // Check for duplicity
            List<UrlRewrite> allRewrites = GetAllUrls();
            foreach (UrlRewrite ur in allRewrites)
            {
                if (ur.Url == url)
                {
                    throw new InvalidUrlRewriteException("Specified URL is already assigned.") { AssignedArticleId = ur.ArticleId };
                }
            }

            Database.UrlRewrite.Add(urlRewrite);
            Database.SaveChanges();
        }

     
        /// <see cref="IJasperDataService.Url(Article)"/>
        public string Url(Article article)
        {
            try
            {
                if (Configuration.WebsiteConfig.UrlRewriting) // URL rewriting has to be allowed
                {
                    UrlRewrite firstUrlRule = Database.UrlRewrite.Where(ur => ur.Article.Id == article.Id).First();

                    // ie.: /first_article
                    string lastPartOfUrl = firstUrlRule.Url;

                    // ie.: /Home/Articles
                    string articleRoute = Configuration.WebsiteConfig.ArticleRoute;

                    return articleRoute + lastPartOfUrl;
                }
                else
                {
                    string articleRouteFromJasperJson = Configuration.WebsiteConfig.ArticleRoute;
                    if (!string.IsNullOrEmpty(articleRouteFromJasperJson))
                    {
                        // remove last slash from url if present
                        if (articleRouteFromJasperJson.EndsWith('/'))
                        {
                            articleRouteFromJasperJson = articleRouteFromJasperJson.Remove(articleRouteFromJasperJson.Length - 1);
                        }

                        return articleRouteFromJasperJson + "?id=" + article.Id.ToString();
                    }
                    else return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidUrlRewriteException(ex);
            }
        }

    
        /// <see cref="IJasperDataService.DeleteUrl(string)"/>
        public void DeleteUrl(string url)
        {
            try
            {
                var itemsToDelete = Database.UrlRewrite.Where(u => u.Url == url);
                Database.UrlRewrite.RemoveRange(itemsToDelete);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidUrlRewriteException(ex);
            }
        }       

       
        /// <see cref="IJasperDataService.File(string)"/>
        public string File(string url)
        {
            try
            {
                // without slash on the beginning it would work only on pages without route parameters
                // urls has to look like: /Themes/Jasper/Styles/style.css
                // without the slash it would create the following: localhost/Home/Category/Themes/Jasper/Styles/style.css = undesirable
                string path = "/" + Configuration.CustomRouting.RelativeThemePathToRootRelativePath(url);


                // If the theme name (== folder with theme) contains space, it will be by default rendered as %20 which will
                // eventually break the: return View() method. Therefore the %20 has to be replaced by regular space.
                string returnPath = path.Replace("%20", " ");
                return returnPath;
            }
            catch (Exception ex)
            {
                throw new CustomRoutingException("Path to the specified file could not be resolved.", ex);
            }
        }

        
        /// <see cref="IJasperDataService.Layout(string)"/>
        public string Layout(string url, HttpRequest request)
        {
            try
            {
                string path = File(url);
                path = "~" + path;

                // Layout page could be globalized e.x : ~/Themes/Jasper/_Layout.cs.cshtml (for Czech language)
                path = Globalization.GlobalizeView(path, request);

                return path;
            }
            catch (Exception ex)
            {
                throw new CustomRoutingException("Path to the specified file could not be resolved.", ex);
            }
        }
  #endregion

    }  


    // *********************** THEME BUILDING ************************************** //

    /// <summary>
    /// An interface used for injecting data into THEME views. Not suitable for ADMINISTRATION views.
    /// Specified methods SHOULDN'T throw exception on failure.
    /// </summary>
    public interface IJasperDataServicePublic
    {
        // Properties
        IDatabaseContext Database { get; set; }
        Components Components { get; set; }

        #region Articles

        /// <summary>
        /// Returns list of all !PUBLISHED! articles. In case of failure returns null.
        /// If the URL rewriting is activated and article has no assigned URL, it will not be returned.
        /// </summary>
        /// <returns></returns>
        List<Article> GetAllArticles();

        /// <summary>
        /// Returns list of all !PUBLISHED! articles from required category. In case of failure returns null;
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        List<Article> GetAllArticles(int categoryId);

        /// <summary>
        /// Returns database attached object of !PUBLISHED! Article by Id. In case of failure returns null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Article GetArticleById(int id);

        /// <summary>
        /// Adds new article and returns DB attached object. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        Article AddArticle();

        /// <summary>
        /// Updates article data and returns database attached object. In case of error returns null;
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        Article EditArticle(Article articleViewModel);

        /// <summary>
        /// Deletes article and returns true. In case of failure returns false.
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        bool DeleteArticle(int articleId);

        #endregion

        #region Categories

        /// <summary>
        /// Renames the category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="newName"></param>
        bool RenameCategory(int categoryId, string newName);

        /// <summary>
        /// Returns list of all categories. In case of failure returns null.
        /// </summary>
        /// <returns></returns>       
        List<Category> GetAllCategories();

        /// <summary>
        /// Returns category name by Id. In case of failure returns null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetCategoryNameById(int id);

        /// <summary>
        /// Adds new category and returns DB attached object. In case of error returns null.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        Category AddCategory(string categoryName);

        /// <summary>
        /// Deletes category and returns true. In case of failure returns false.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        bool DeleteCategory(int categoryId);

        /// <summary>
        /// If uncategorized category exists, then returns true.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        bool UncategorizedCategoryExists();

        /// <summary>
        /// Adds uncategorized category. In case of failure returns false;
        /// </summary>
        /// <exception cref="DatabaseHelperException"></exception>
        bool CreateUncategorizedCategory();

        #endregion

        #region Users

        /// <summary>
        /// Returns database attached User. In case of failure returns null.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetUserWithUsername(string username);

        /// <summary>
        /// Returns database attached User. In case of failure returns null.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        User GetUserById(int userId);

        /// <summary>
        /// Returns list of all users. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        List<User> GetAllUsers();

        /// <summary>
        /// Changes user's password and returns true. In case of failure returns false.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newHashedPassword"></param>
        /// <param name="newSalt"></param>
        /// <returns></returns>
        bool ChangePassword(int userId, string newHashedPassword, string newSalt);

        /// <summary>
        /// Returns the list of users with the "admin" role. In case of error returns empty list.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        List<User> GetAllAdministrators();

        /// <summary>
        /// Adds new user and returns DB attached object. In case of failure returns null.
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        User AddUser(User newUser);

        /// <summary>
        /// Deletes user by Id and returns true. In case of error returns false.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteUserById(int id);

        #endregion

        #region Roles

        /// <summary>
        /// Returns list of all roles. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        List<Role> GetAllRoles();

        #endregion

        #region Themes

        /// <summary>
        /// Returns list of all themes. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        List<Theme> GetAllThemes();

        /// <summary>
        /// Deletes theme by name and returns true. In case of failure returns false.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool DeleteThemeByName(string name);

        /// <summary>
        /// Adds themes from folder to database and returns true. In case of failure returns false.
        /// </summary>
        /// <param name="themeNamesToBeAdded"></param>
        /// <returns></returns>
        bool AddThemesFromFolderToDatabase(List<string> themeNamesToBeAdded, IRequestCultureFeature culture);

        /// <summary>
        /// 1) Removes all themes from database.
        /// 2) All text block and their content will be untouched.
        /// 3) Themes records & block holders in Db will be recreated based on physical files in Theme folder.
        /// 4) All text blocks will be marked as unassigned. 
        /// 5) Finally returns true. In case of failure returns false.
        /// </summary>
        /// <returns></returns>
        bool ReconstructAndClearThemeData(IRequestCultureFeature culture);

        /// <summary>
        /// Returns all theme names that have not yet been stored in the database. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        List<string> CheckThemeFolderAndDatabaseIntegrity(IRequestCultureFeature culture);

        /// <summary>
        /// Returns list of names of manually deleted themes. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        List<string> FindManuallyDeletedThemes(IRequestCultureFeature culture);

        /// <summary>
        /// Returns current theme Id. In case of failure returns -1.
        /// </summary>
        /// <returns></returns>
        int GetCurrentThemeIdFromDb();

        #endregion

        #region Settings

        /// <summary>
        /// Returns website name. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        string GetWebsiteName();

        /// <summary>
        /// Sets the website name. In case if failure returns false.
        /// </summary>
        /// <param name="newWebsiteName"></param>
        /// <returns></returns>
        bool SetWebsiteName(string newWebsiteName);

        #endregion

        #region Blocks

        /// <summary>
        /// Returns list of all blockHolders. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        List<BlockHolder> GetAllBlockHolders();

        /// <summary>
        /// Returns list of all TextBlocks. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        List<TextBlock> GetAllTextBlocks();

        /// <summary>
        /// Returns list of all Holder_Blocks. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        List<Holder_Block> GetAllHolder_Blocks();

        /// <summary>
        /// Adds new block and returns DB attached object. In case of error returns null.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        TextBlock AddNewBlock(TextBlock block);

        /// <summary>
        /// Adds new holder_block and returns DB attached object. In case of failure returns null.
        /// </summary>
        /// <param name="hb"></param>
        /// <returns></returns>
        Holder_Block AddHolder_Block(Holder_Block hb);

        /// <summary>
        /// Deletes TextBlock by Id and returns true. In case of failure returns false.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteTextBlockById(int id);

        /// <summary>
        /// Adds holder to block and returns DB attached object. In case of failure returns null.
        /// </summary>
        /// <param name="holderId"></param>
        /// <param name="blockId"></param>
        /// <returns></returns>
        Holder_Block AddHolderToBlock(int holderId, int blockId);

        /// <summary>
        /// Removes holder from block and returns true. In case of failure returns false.
        /// </summary>
        /// <param name="holderId"></param>
        /// <param name="blockId"></param>
        /// <returns></returns>
        bool RemoveHolderFromBlock(int holderId, int blockId);

        /// <summary>
        /// Deletes BlockHolder by Id and returns true. In case of failure returns false.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteBlockHolderById(int id);

        /// <summary>
        /// Returns database attached TextBlock. In case of failure returns null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TextBlock GetTextBlockById(int id);

        /// <summary>
        /// Returns database attached Holder_Block. In case of failure returns false.
        /// </summary>
        /// <param name="textBlockId"></param>
        /// <param name="holderId"></param>
        /// <returns></returns>
        Holder_Block GetHolder_Block(int textBlockId, int holderId);

        /// <summary>
        /// Saves order of TextBlock in Holder. In case of failure returns false.
        /// </summary>
        /// <param name="textBlockId"></param>
        /// <param name="holderId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        bool SaveTextBlockOrderNumberInHolder(int textBlockId, int holderId, int order);

        #endregion

        #region Images

        /// <summary>
        /// Returns list of all images. In case of failure returns null.
        /// </summary>
        /// <returns></returns>
        List<Image> GetAllImages();

        /// <summary>
        /// Deletes image by Id and returns true. In case of failure returns false.
        /// </summary>
        /// <param name="imgId"></param>
        /// <returns></returns>
        bool DeleteImageById(int imgId);

        /// <summary>
        /// Returns a image by Id. In case of failure returns Null.
        /// </summary>
        /// <param name="imgId"></param>
        /// <returns></returns>
        Image GetImageById(int imgId);
       

        #endregion

        #region Url

        /// <summary>
        /// Returns all UrlRewrite records. In case of failure returns null.
        /// </summary>
        /// <returns></returns>   
        List<UrlRewrite> GetAllUrls();

        /// <summary>
        /// Returns list of all URL for the specified article. In case of failure returns null.
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>    
        List<string> GetUrls(int articleId);

        /// <summary>
        /// Updates or creates a new URL rewrite rule and returns true. In case of error returns false.
        /// </summary>
        /// <param name="article"></param>
        /// <param name="url"></param>  
        bool SetUrl(Article article, string url);

        /// <summary>
        /// Returns the first URL found. In case of failure returns string.empty.
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>       
        string Url(Article article);

        /// <summary>
        /// Deletes all records with specified URL and returns true. In case of error returns false.
        /// </summary>
        /// <param name="url"></param>
        /// <exception cref="InvalidUrlRewriteException"></exception>
        bool DeleteUrl(string url);

        /// <summary>
        /// Transforms Theme-relative Url to Root-relative Url. In case of error returns empty string.
        /// </summary>
        /// <param name="url">Theme-relative Url</param>
        /// <returns></returns>
        /// <exception cref="CustomRoutingException"></exception>
        string File(string url);

        /// <summary>
        /// This method is used for setting Layout of a Razor view page. In case of error returns empty string.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="CustomRoutingException"></exception>
        string Layout(string url,HttpRequest request);

        #endregion
        
        
    }


    /// <summary>
    /// This class methods never throw exceptions and are suitable for building themes.
    /// </summary>
    public class DbHelperPublic : IJasperDataServicePublic
    {
        public DbHelperPublic(IDatabaseContext database)
        {
            this.Database = database ?? throw new DatabaseContextNullException();

            // slave object
            this._dbHelper = new DbHelper(database);

            this.Components = new Components(Database, _dbHelper);

        }

        public IDatabaseContext Database { get; set; }
        public Components Components { get; set; }
        private DbHelper _dbHelper { get; set; }


        #region Articles

       
        /// <see cref="IJasperDataServicePublic.GetAllArticles"/>
        public List<Article> GetAllArticles()
        {
            try
            {
                List<Article> allPublishedArticles = _dbHelper.GetAllArticles().Where(a => a.Publish).ToList();
                if (Configuration.WebsiteConfig.UrlRewriting)
                {
                    List<Article> articlesToBeRemovedFromList = new List<Article>();
                    foreach (Article a in allPublishedArticles)
                    {
                        if (_dbHelper.GetUrls(a.Id).Count() < 1)
                        {
                            articlesToBeRemovedFromList.Add(a);
                        }
                    }

                    // Cycle through items that have to be removed
                    foreach (Article toBeDeleted in articlesToBeRemovedFromList)
                    {
                        allPublishedArticles.Remove(toBeDeleted);
                    }
                }
                return allPublishedArticles;
            }
            catch
            {
                return null;
            }
        }

     
        /// <see cref="IJasperDataServicePublic.GetAllArticles(int)"/>
        public List<Article> GetAllArticles(int categoryId)
        {
            try
            {
                return _dbHelper.GetAllArticles(categoryId).Where(a => a.Publish).ToList();
            }
            catch
            {
                return null;
            }
        }

      
        /// <see cref="IJasperDataServicePublic.GetArticleById(int)"/>
        public Article GetArticleById(int id)
        {
            try
            {
                Article article = _dbHelper.GetArticleById(id);
                if (article.Publish)
                {
                    // Keywords are returning string.empty, not null,
                    // because keywords are often used in _Layout: ViewData["Keywords"]
                    if (article.Keywords == null)
                    {
                        article.Keywords = string.Empty;
                    }

                    return article;
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }

        
        /// <see cref="IJasperDataServicePublic.AddArticle"/>
        public Article AddArticle()
        {
            try
            {
                return _dbHelper.AddArticle();
            }
            catch
            {
                return null;
            }
        }    
       
        /// <see cref="IJasperDataServicePublic.EditArticle(Article)"/>
        public Article EditArticle(Article article)
        {
            try
            {
                return _dbHelper.EditArticle(article);
            }
            catch
            {
                return null;
            }
        } 

    
        /// <see cref="IJasperDataServicePublic.DeleteArticle(int)"/>
        public bool DeleteArticle(int articleId)
        {
            try
            {
                _dbHelper.DeleteArticle(articleId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Categories

        
        /// <see cref="IJasperDataServicePublic.RenameCategory"></see>
        public bool RenameCategory(int categoryId, string newName)
        {
            try
            {
                _dbHelper.RenameCategory(categoryId, newName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <see cref="IJasperDataServicePublic.GetAllCategories"/>
        public List<Category> GetAllCategories()
        {
            try
            {
                return _dbHelper.GetAllCategories();
            }
            catch
            {
                return null;
            }
        }

      
        /// <see cref="IJasperDataServicePublic.GetCategoryNameById(int)"/>
        public string GetCategoryNameById(int id)
        {
            try
            {
                return _dbHelper.GetCategoryNameById(id);
            }
            catch
            {
                return null;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.AddCategory(string)"/>
        public Category AddCategory(string categoryName)
        {
            try
            {
                return _dbHelper.AddCategory(categoryName);
            }
            catch
            {
                return null;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.DeleteCategory(int)"/>
        public bool DeleteCategory(int categoryId)
        {
            try
            {
                _dbHelper.DeleteCategory(categoryId);
                return true;
            }
            catch
            {
                return false;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.UncategorizedCategoryExists"/>    
        public bool UncategorizedCategoryExists()
        {
            try
            {
                _dbHelper.UncategorizedCategoryExists();
                return true;
            }
            catch
            {
                return false;
            }
        }
       
        /// <see cref="IJasperDataServicePublic.CreateUncategorizedCategory"/>    
        public bool CreateUncategorizedCategory()
        {
            try
            {
                _dbHelper.CreateUncategorizedCategory();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Users

     
        /// <see cref="IJasperDataServicePublic.GetUserWithUsername(string)"/>
        public User GetUserWithUsername(string username)
        {
            try
            {
                return _dbHelper.GetUserWithUsername(username);
            }
            catch
            {
                return null;
            }
        }

  
        /// <see cref="IJasperDataServicePublic.GetUserById(int)"/>
        public User GetUserById(int userId)
        {
            try
            {
                return _dbHelper.GetUserById(userId);
            }
            catch
            {
                return null;
            }
        }

      
        /// <see cref="IJasperDataServicePublic.GetAllUsers"/>
        public List<User> GetAllUsers()
        {
            try
            {
                return _dbHelper.GetAllUsers();
            }
            catch
            {
                return null;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.ChangePassword(int, string, string)"/>
        public bool ChangePassword(int userId, string newHashedPassword, string newSalt)
        {
            try
            {
                _dbHelper.ChangePassword(userId, newHashedPassword, newSalt);
                return true;
            }
            catch
            {
                return false;
            }
        }

        
        /// <see cref="IJasperDataServicePublic.GetAllAdministrators"/>  
        public List<User> GetAllAdministrators()
        {
            try
            {
                return _dbHelper.GetAllAdministrators();
            }
            catch
            {
                return new List<User>();
            }
        }

        
        /// <see cref="IJasperDataServicePublic.AddUser(User)"/>
        public User AddUser(User newUser)
        {
            try
            {
                return _dbHelper.AddUser(newUser);
            }
            catch
            {
                return null;
            }
        }

     
        /// <see cref="IJasperDataServicePublic.DeleteUserById(int)"/>
        public bool DeleteUserById(int id)
        {
            try
            {
                _dbHelper.DeleteUserById(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Roles

      
        /// <see cref="IJasperDataServicePublic.GetAllRoles"/>
        public List<Role> GetAllRoles()
        {
            try
            {
                return _dbHelper.GetAllRoles();
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Themes

      
        /// <see cref="IJasperDataServicePublic.GetAllThemes"/>
        public List<Theme> GetAllThemes()
        {
            try
            {
                return _dbHelper.GetAllThemes();
            }
            catch
            {
                return null;
            }
        }

        
        /// <see cref="IJasperDataServicePublic.DeleteThemeByName(string)"/>
        public bool DeleteThemeByName(string name)
        {
            try
            {
                _dbHelper.DeleteThemeByName(name);
                return true;
            }
            catch
            {
                return false;
            }
        }

      
        /// <see cref="IJasperDataServicePublic.AddThemesFromFolderToDatabase(List{string})"/>
        public bool AddThemesFromFolderToDatabase(List<string> themeNamesToBeAdded, IRequestCultureFeature culture)
        {
            try
            {
                _dbHelper.AddThemesFromFolderToDatabase(themeNamesToBeAdded,culture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        
        /// <see cref="IJasperDataServicePublic.ReconstructAndClearThemeData"/>
        public bool ReconstructAndClearThemeData(IRequestCultureFeature culture)
        {
            try
            {
                _dbHelper.ReconstructAndClearThemeData(culture);
                return true;
            }
            catch
            {
                return false;
            }
        }


       
        /// <see cref="IJasperDataServicePublic.CheckThemeFolderAndDatabaseIntegrity"/>
        public List<string> CheckThemeFolderAndDatabaseIntegrity(IRequestCultureFeature culture)
        {
            try
            {
                return _dbHelper.CheckThemeFolderAndDatabaseIntegrity(culture);
            }
            catch
            {
                return null;
            }
        }

      
        /// <see cref="IJasperDataServicePublic.FindManuallyDeletedThemes"/>
        public List<string> FindManuallyDeletedThemes(IRequestCultureFeature culture)
        {
            try
            {
                return _dbHelper.FindManuallyDeletedThemes(culture);
            }
            catch
            {
                return null;
            }
        }

       
        /// <exception cref="DatabaseHelperException"></exception>
        public int GetCurrentThemeIdFromDb()
        {
            try
            {
                string themeName = Configuration.GlobalWebsiteConfig.ThemeName;
                int id = Database.Themes.Where(t => t.Name == themeName).Single().Id;
                return id;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion

        #region Settings 
      
        /// <see cref="IJasperDataServicePublic.GetWebsiteName"/>
        public string GetWebsiteName()
        {
            try
            {
                return _dbHelper.GetWebsiteName();
            }
            catch
            {
                return null;
            }
        }

     
        /// <see cref="IJasperDataServicePublic.SetWebsiteName(string)"/>
        public bool SetWebsiteName(string newWebsiteName)
        {
            try
            {
                _dbHelper.SetWebsiteName(newWebsiteName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Blocks

        /// <see cref="IJasperDataServicePublic.GetAllBlockHolders"/>
        public List<BlockHolder> GetAllBlockHolders()
        {
            try
            {
                return _dbHelper.GetAllBlockHolders();
            }
            catch
            {
                return null;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.GetAllTextBlocks"/>
        public List<TextBlock> GetAllTextBlocks()
        {
            try
            {
                return _dbHelper.GetAllTextBlocks();
            }
            catch
            {
                return null;
            }
        }

      
        /// <see cref="IJasperDataServicePublic.GetAllHolder_Blocks"/>
        public List<Holder_Block> GetAllHolder_Blocks()
        {
            try
            {
                return _dbHelper.GetAllHolder_Blocks();
            }
            catch
            {
                return null;
            }
        }

       

      
        /// <see cref="IJasperDataServicePublic.AddNewBlock(TextBlock)"/>
        public TextBlock AddNewBlock(TextBlock block)
        {
            try
            {
                return _dbHelper.AddNewBlock(block);

            }
            catch
            {
                return null;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.AddHolder_Block(Holder_Block)"/>
        public Holder_Block AddHolder_Block(Holder_Block hb)
        {
            try
            {
                return _dbHelper.AddHolder_Block(hb);
            }
            catch
            {
                return null;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.DeleteTextBlockById(int)"/>
        public bool DeleteTextBlockById(int id)
        {
            try
            {
                _dbHelper.DeleteTextBlockById(id);
                return true;
            }
            catch
            {
                return false;
            }
        }


       
        /// <see cref="IJasperDataServicePublic.AddHolderToBlock(int, int)"/>
        public Holder_Block AddHolderToBlock(int holderId, int blockId)
        {
            try
            {
                return _dbHelper.AddHolderToBlock(holderId, blockId);
            }
            catch
            {
                return null;
            }
        }



       
        /// <see cref="IJasperDataServicePublic.RemoveHolderFromBlock(int, int)"/>
        public bool RemoveHolderFromBlock(int holderId, int blockId)
        {
            try
            {
                _dbHelper.RemoveHolderFromBlock(holderId, blockId);
                return true;
            }
            catch
            {
                return false;
            }
        }

      
        /// <see cref="IJasperDataServicePublic.DeleteBlockHolderById(int)"/>
        public bool DeleteBlockHolderById(int id)
        {
            try
            {
                _dbHelper.DeleteBlockHolderById(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.GetTextBlockById(int)"/>
        public TextBlock GetTextBlockById(int id)
        {
            try
            {
                return _dbHelper.GetTextBlockById(id);
            }
            catch
            {
                return null;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.GetHolder_Block(int, int)"/>
        public Holder_Block GetHolder_Block(int textBlockId, int holderId)
        {
            try
            {
                return _dbHelper.GetHolder_Block(textBlockId, holderId);
            }
            catch
            {
                return null;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.SaveTextBlockOrderNumberInHolder(int, int, int)"/>
        public bool SaveTextBlockOrderNumberInHolder(int textBlockId, int holderId, int order)
        {
            try
            {
                _dbHelper.SaveTextBlockOrderNumberInHolder(textBlockId, holderId, order);
                return true;
            }
            catch
            {
                return false;
            }
        }


        #endregion

        #region Images

       
        /// <see cref="IJasperDataServicePublic.GetAllImages"/>
        public List<Image> GetAllImages()
        {
            try
            {
                return _dbHelper.GetAllImages();
            }
            catch
            {
                return null;
            }
        }

        
        /// <see cref="IJasperDataServicePublic.DeleteImageById(int)"/>
        public bool DeleteImageById(int imgId)
        {
            try
            {
                _dbHelper.DeleteImageById(imgId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <see cref="IJasperDataServicePublic.GetImageById(int)"/>
        public Image GetImageById(int imgId)
        {
            try
            {
                return _dbHelper.GetImageById(imgId);
            }
            catch
            {
                return null;
            }
        }

            #endregion

            #region Url


            /// <see cref="IJasperDataServicePublic.GetAllUrls"/>
            public List<UrlRewrite> GetAllUrls()
        {
            try
            {
                return _dbHelper.GetAllUrls();
            }
            catch
            {
                return null;
            }
        }

          
        /// <see cref="IJasperDataServicePublic.GetUrls(int)"/>
        public List<string> GetUrls(int articleId)
        {
            try
            {
                return _dbHelper.GetUrls(articleId);
            }
            catch
            {
                return null;
            }
        }

            
        /// <see cref="IJasperDataServicePublic.SetUrl(Article, string)"/>
        public bool SetUrl(Article article, string url)
        {
            try
            {
                _dbHelper.SetUrl(article, url);
                return true;
            }
            catch
            {
                return false;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.Url(Article)"/>
        public string Url(Article article)
        {
            try
            {
                return _dbHelper.Url(article);
            }
            catch
            {
                return string.Empty;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.DeleteUrl(string)"/>
        public bool DeleteUrl(string url)
        {
            try
            {
                var itemsToDelete = Database.UrlRewrite.Where(u => u.Url == url);
                Database.UrlRewrite.RemoveRange(itemsToDelete);
                Database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

       
        /// <see cref="IJasperDataServicePublic.File(string)"/>
        public string File(string url)
        {
            try
            {
                // without slash on the beginning it would work only on pages without route parameters
                // urls has to look like: /Themes/Jasper/Styles/style.css
                // without the slash it would create the following: localhost/Home/Category/Themes/Jasper/Styles/style.css = undesirable
                string path = "/" + Configuration.CustomRouting.RelativeThemePathToRootRelativePath(url);


                // If the theme name (== folder with theme) contains space, it will be by default rendered as %20 which will
                // eventually break the: return View() method. Therefore the %20 has to be replaced by regular space.
                string returnPath = path.Replace("%20", " ");
                return returnPath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        
        /// <see cref="IJasperDataServicePublic.Layout(string)/>
        public string Layout(string url, HttpRequest request)
        {
            try
            {
                string path = File(url);
                path = "~" + path;

                // Layout page could be globalized e.x : ~/Themes/Jasper/_Layout.cs.cshtml (for Czech language)
                path=Globalization.GlobalizeView(path,request);

                return path;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion
       
    }
}


