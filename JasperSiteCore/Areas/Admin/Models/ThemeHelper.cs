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
        public ThemeHelper()
        {

        }

        //public string GetAbsoluteThemeFolderPath()
        //{
        //    string themeFolder = Configuration.GlobalWebsiteConfig.ThemeFolder;
        //    return Configuration.CustomRouting.RelativeThemePathToRootRelativePath(themeFolder);
        //}

        private string GetThemeDescription(string pathToTheme)
        {
            string desc = File.ReadAllText(Path.Combine(pathToTheme, "desc.txt"));
            return desc;
        }

        // public string GetThemeThumbnailUrl(string themeFolder, string themeName)
        // {   
        //     return "~/" + themeFolder + "/" + themeName + "/thumbnail.jpg"; 
        //    // return Configuration.CustomRouting.RelativeThemePathToRootRelativePath("thumbnail.jpg");
        // }

        public string GetThemeThumbnailUrl(string themeName)
        {   // vyřešit lépe
            string themeFolder = Configuration.CustomRouting.GlobalWebsiteConfig.ThemeFolder;
            return Path.Combine("~/", themeFolder, themeName, "thumbnail.jpg").Replace('\\', '/');
        }


        public List<ThemeInfo> GetInstalledThemesInfo()
        {
            return GetInstalledThemesInfo(Configuration.GlobalWebsiteConfig.ThemeFolder);
        }
        public List<ThemeInfo> GetInstalledThemesInfo(string themeFolderPath)
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

        /// <summary>
        /// Searches through Theme folder, indexing themes and returns collection sorted by name, with current theme first.
        /// </summary>
        /// <returns></returns>
        public List<ThemeInfo> GetInstalledThemesInfoByNameAndActive()
        {
            List<ThemeInfo> themeInfoList = Configuration.ThemeHelper.GetInstalledThemesInfo();
            themeInfoList.OrderBy(o => o.ThemeName);
            ThemeInfo currentTheme = themeInfoList.Where(i => i.ThemeName == Configuration.GlobalWebsiteConfig.ThemeName).First();
            themeInfoList.Remove(currentTheme);
            themeInfoList.Insert(0, currentTheme);
            return themeInfoList;
        }

        public bool DeleteThemeByName(string themeName)
        {
            try
            {
                Configuration.DbHelper.DeleteThemeByName(themeName);

                string themeFolder = Configuration.CustomRouting.GlobalWebsiteConfig.ThemeFolder;
                string themeFolderPath = Path.Combine("./", themeFolder, themeName).Replace('\\', '/');
                System.IO.Directory.Delete(themeFolderPath, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Reconstruct_Theme_TextBlock_BlockHolder_HolderBlockDatabase()
        {
            Configuration.DbHelper.Reconstruct_Theme_TextBlock_BlockHolder_HolderBlockDatabase();
        }


}




public class ThemeInfo
    {
        
        public string ThemeName { get; set; }
        public string ThemeFolder { get; set; }
        public string ThemeDescription { get; set; }
       // thumbnail img - TODO
       public string ThemeThumbnailUrl {get;set;}
    }
}
