﻿using JasperSiteCore.Models;
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

        private string GetThemeDescription(string pathToTheme)
        {            
            string desc = File.ReadAllText(Path.Combine(pathToTheme,"desc.txt"));
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
            return  Path.Combine("~/", themeFolder, themeName,"thumbnail.jpg").Replace('\\','/');
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

        public bool DeleteThemeByName(string themeName)
        {
            try
            {
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
