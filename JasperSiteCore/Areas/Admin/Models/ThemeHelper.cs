using JasperSiteCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

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

        public string GetThemeDescription(string pathToTheme)
        {            
            string desc = File.ReadAllText(Path.Combine(pathToTheme,"desc.txt"));
            return desc;
        }

        public string GetThemeThumbnailUrl(string themeFolder, string themeName)
        {   // vyřešit lépe
            return "~/" + themeFolder + "/" + themeName + "/thumbnail.jpg"; 
        }

        public string GetThemeThumbnailUrl(string themeName)
        {   // vyřešit lépe
            string themeFolder = Configuration.CustomRouting.GlobalWebsiteConfig.ThemeFolder;          
            return  Path.Combine("~/", themeFolder, themeName,"thumbnail.jpg").Replace('\\','/');
        }

        public List<ThemeInfo> GetInstalledThemesInfo(string themeFolderPath)
        {
            //string themeFolder = Configuration.GlobalWebsiteConfig.ThemeFolder;
            List<string> themeSubfolders = Directory.GetDirectories(themeFolderPath).ToList();

            List<ThemeInfo> themeInfos = new List<ThemeInfo>();
            foreach (string themeName in themeSubfolders)
            {
                ThemeInfo ti = new ThemeInfo()
                {
                    ThemeName = Path.GetFileName(themeName),
                    ThemeFolder = themeFolderPath,
                    ThemeDescription = GetThemeDescription(themeName)
                    
                };
                themeInfos.Add(ti);
            }
            return themeInfos;
        }
    }

   

    public class ThemeInfo
    {
        public string ThemeName { get; set; }
        public string ThemeFolder { get; set; }
        public string ThemeDescription { get; set; } = "demo desc";
       // thumbnail img - TODO
    }
}
