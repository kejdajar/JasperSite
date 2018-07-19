using JasperSiteCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.Models
{
    public class ThemeHelper
    {
          
        /// <summary>
        /// Returns content of desc.txt file in root of the folder with theme.
        /// </summary>
        /// <param name="pathToTheme"></param>
        /// <returns></returns>
        /// <exception cref="ThemeHelperException"></exception>
        private string GetThemeDescription(string pathToTheme)
        {
            try
            {
                string desc = File.ReadAllText(Path.Combine(pathToTheme, "desc.txt"));
                return desc;
            }
            catch (Exception ex)
            {

                throw new ThemeHelperException(ex);
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
                return Path.Combine("~/", themeFolder, themeName, "thumbnail.jpg").Replace('\\', '/');
            }
            catch (Exception ex)
            {

                throw new ThemeHelperException(ex);
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
            catch (Exception ex)
            {

                throw new ThemeHelperException(ex);
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
            catch(Exception ex)
            {
                throw new ThemeHelperException(ex);
            }
        }      

}

public class ThemeInfo
    {        
        public string ThemeName { get; set; }
        public string ThemeFolder { get; set; }
        public string ThemeDescription { get; set; }     
        public string ThemeThumbnailUrl {get;set;}
    }
}
