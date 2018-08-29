using JasperSiteCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Models.Providers;

namespace JasperSiteCore.Areas.Admin.Models
{
    public class ThemeHelper
    {

        /// <summary>
        /// Returns content of desc.txt file in root of the folder with theme.
        /// </summary>
        /// <param name="pathToTheme"></param>
        /// <returns></returns>        
        private string GetThemeDescription(string pathToTheme)
        {
            try
            {
                string desc = File.ReadAllText(Path.Combine(pathToTheme, "desc.txt"));
                return desc;
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Returns url (~/...) to the thumbnail.jpg file.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        /// <exception cref="ThemeHelperException"></exception>
        public string GetThemeThumbnailUrl(string themeName)
        {
            try
            {
                string themeFolder = Configuration.CustomRouting.GlobalWebsiteConfig.ThemeFolder;
                string path = Path.Combine("~/", themeFolder, themeName, "thumbnail.jpg").Replace('\\', '/');
                string pathExistsCheck = Path.Combine(Env.Hosting.ContentRootPath, themeFolder, themeName, "thumbnail.jpg");
                //bool fileExists = File.Exists(@"C:/Users/kejda/Desktop/Projekty/JasperSiteCore/JasperSiteCore/Themes/Default/thumbnail.jpg");
                bool fileExists = File.Exists(pathExistsCheck);
                if (fileExists)
                    return path;
                else return string.Empty;

            }
            catch
            {

                return string.Empty;
            }
        }

        /// <summary>
        /// Returns list of installed themes in folder specified by Configuration.GlobalWebsiteConfig.ThemeFolder.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ThemeHelperException"></exception>
        public List<ThemeInfo> GetInstalledThemesInfo()
        {

            return GetInstalledThemesInfo(Configuration.GlobalWebsiteConfig.ThemeFolder);

        }

        /// <summary>
        /// Returns list with info about al installed themes. Data are obtained from FILESYSTEM, not DB!
        /// </summary>
        /// <param name="themeFolderPath">Path to the folder with themes.</param>
        /// <returns></returns>
        /// <exception cref="ThemeHelperException"></exception>
        public List<ThemeInfo> GetInstalledThemesInfo(string themeFolderPath)
        {

            try
            {
                List<string> themeSubfolders = Directory.GetDirectories(themeFolderPath).ToList();

                List<ThemeInfo> themeInfos = new List<ThemeInfo>();
                foreach (string themeSubdirPath in themeSubfolders)
                {
                    string _themeName = Path.GetFileName(themeSubdirPath);
                    ThemeInfo ti = new ThemeInfo()
                    {

                        ThemeName = _themeName,
                        ThemeFolder = themeFolderPath,
                        ThemeDescription = GetThemeDescription(themeSubdirPath),
                        ThemeThumbnailUrl = GetThemeThumbnailUrl(_themeName)
                    };
                    themeInfos.Add(ti);

                }
                return themeInfos;
            }
            catch (Exception ex)
            {

                throw new ThemeHelperException(ex);
            }
        }

        /// <summary>
        /// Searches through Theme folder, indexing themes and returns collection sorted by name, with current theme first.
        /// </summary>
        /// <returns></returns>       
        /// <exception cref="ThemeHelperException"></exception>
        /// <exception cref="ThemeNotExistsException">Jasper.json file property themeName points to theme that does not exist.</exception>
        public List<ThemeInfo> GetInstalledThemesInfoByNameAndActive()
        {


            try
            {
                List<ThemeInfo> themeInfoList = Configuration.ThemeHelper.GetInstalledThemesInfo();
                themeInfoList.OrderBy(o => o.ThemeName);
                ThemeInfo currentTheme = themeInfoList.Where(i => i.ThemeName == Configuration.GlobalWebsiteConfig.ThemeName).First();
                themeInfoList.Remove(currentTheme);
                themeInfoList.Insert(0, currentTheme);
                return themeInfoList;
            }
            catch (ThemeHelperException ex)
            {
                throw new ThemeHelperException(ex);
            }
            catch (Exception ex)
            {
                ThemeNotExistsException exceptionToBeThrown = new ThemeNotExistsException("Jasper.json property themeName is set to:" + Configuration.GlobalWebsiteConfig.ThemeName + " which does not exist.", ex);
                exceptionToBeThrown.MissingThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
                throw exceptionToBeThrown;
            }



        }

        /// <summary>
        /// Deletes theme from Database and Filesystem
        /// </summary>
        /// <param name="themeName"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        /// <exception cref="ThemeHelperException"></exception>
        public bool DeleteThemeByNameFromDbAndFolder(string themeName, DatabaseContext dbContext)
        {
            try
            {
                DbHelper dbHelper = new DbHelper(dbContext);
                dbHelper.DeleteThemeByName(themeName);

                string themeFolder = Configuration.CustomRouting.GlobalWebsiteConfig.ThemeFolder;
                string themeFolderPath = Path.Combine("./", themeFolder, themeName).Replace('\\', '/');
                System.IO.Directory.Delete(themeFolderPath, true);
                return true;
            }
            catch (Exception ex)
            {
                throw new ThemeHelperException(ex);
            }
        }

        /// <summary>
        /// All manually deleted themes will be removed from the database,
        /// all unregistered themes will be added to the database,
        /// blockHolders in database will be synchronized with jasper.json
        /// </summary>
        /// <param name="Database"></param>
        /// <exception cref="ThemeHelperException"></exception>
        public void UpdateAllThemeRelatedData(DatabaseContext Database)
        {
            try
            {
                DbHelper dbHelper = new DbHelper(Database);

            // Firstly, this will add all unregistered themes to the database
            dbHelper.AddThemesFromFolderToDatabase(dbHelper.CheckThemeFolderAndDatabaseIntegrity());

            //Secondly, all manually deleted themes will be removed from the DB
            List<string> themesToDelete = dbHelper.FindManuallyDeletedThemes();
            foreach (string name in themesToDelete)
            {
                dbHelper.DeleteThemeByName(name);
            }

            // All cached data will be updated
            Configuration.WebsiteConfig.RefreshData();

            // All blockHolders will be checked:
            // if they don't exist in jasper.json, all related data will be deleted from the DB
            // if they exist in jasper.json, everything in the DB will remain as is
            // if there are new blockHolders in jasper.json, they will be added to the DB

           
                // All themes info, even those that were already registered
                List<ThemeInfo> themesInfo = Configuration.ThemeHelper.GetInstalledThemesInfo();


                foreach (ThemeInfo ti in themesInfo)
                {

                    // Find this theme in DB
                    Theme themeInDB = dbHelper.GetAllThemes().Where(t => t.Name == ti.ThemeName).Single();
                    var allBlockHolders = dbHelper.GetAllBlockHolders();
                    var blockHoldersForThisTheme = allBlockHolders.Where(bh => bh.ThemeId == themeInDB.Id);

                    // Emulation of website configuration for every theme
                    GlobalConfigDataProviderJson globalJsonProvider = new GlobalConfigDataProviderJson("jasper.json");
                    GlobalWebsiteConfig globalConfig = new GlobalWebsiteConfig(globalJsonProvider);
                    globalConfig.SetTemporaryThemeName(ti.ThemeName); // This will not write enytihing to the underlying .json file 
                    ConfigurationObjectProviderJson configurationObjectJsonProvider = new ConfigurationObjectProviderJson(globalConfig, "jasper.json");
                    WebsiteConfig websiteConfig = new WebsiteConfig(configurationObjectJsonProvider);

                    List<string> holders = websiteConfig.BlockHolders;

                    // excess holders will be removed from DB
                    foreach(BlockHolder holderFromDb in blockHoldersForThisTheme)
                    {
                        if(!holders.Contains(holderFromDb.Name))
                        {
                            Database.BlockHolders.Remove(holderFromDb);
                            Database.SaveChanges();
                        }
                    }

                    // Newly added holders will be saved into the database
                    foreach (string holderName in holders)
                    {
                        bool isHolderInDB = false;

                        foreach (BlockHolder singleBlockHolderInDb in blockHoldersForThisTheme)
                        {
                            if (singleBlockHolderInDb.Name == holderName)
                            {
                                isHolderInDB = true;
                            }
                           
                        }

                        if(!isHolderInDB)
                        {
                            // New blockHolder will be registered into the DB
                            Database.BlockHolders.Add(new BlockHolder() { Name = holderName, ThemeId = themeInDB.Id });
                        }


                    }
                    Database.SaveChanges();

                }


            }
            catch {
                throw new ThemeHelperException();
            }

        }

       
    }

    public class ThemeInfo
    {
        public string ThemeName { get; set; }
        public string ThemeFolder { get; set; }
        public string ThemeDescription { get; set; }
        public string ThemeThumbnailUrl { get; set; }
    }
}
