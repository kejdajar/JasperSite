using System;
using System.Collections.Generic;
using System.Linq;


namespace JasperSiteCore.Models
{
    public  class CustomRouting
    {
       public CustomRouting(WebsiteConfig websiteConfig, GlobalWebsiteConfig globalWebsiteConfig)
        {
            this.WebsiteConfig = websiteConfig;
            this.GlobalWebsiteConfig = globalWebsiteConfig;
        }

        public WebsiteConfig WebsiteConfig { get; set; }
        public GlobalWebsiteConfig GlobalWebsiteConfig { get; set; }

        public bool IsHomePage(string rawUrl)
        {
            foreach (string homeRouteUrl in GetHomePageUrls())
            {
                if (rawUrl == homeRouteUrl)
                {
                    return true;
                }
            }
            return false;
        }

        public string[] GetHomePageUrls()
        {
            string[] homePageUrls = WebsiteConfig.GetConfigData().routing.homePage;
            if(homePageUrls == null || homePageUrls.Length <1)
            {
                return null;
            }
            else
            {
                return homePageUrls;
            }
        }

        public string GetHomePageFile()
        {
            string physicalFileUrl = WebsiteConfig.GetConfigData().routing.homePageFile;
            return RelativeThemePathToRootRelativePath(physicalFileUrl);
        }

        public string GetErrorPageFile()
        {string physicalFileUrl = WebsiteConfig.GetConfigData().routing.errorPageFile;
            return RelativeThemePathToRootRelativePath(physicalFileUrl);
        }

        public string MapUrlToFile(string rawUrl)
        {
            List<ConfigurationObject.RouteMapping> collection = WebsiteConfig.GetConfigData().customPageMapping;
            foreach(ConfigurationObject.RouteMapping routeObject in collection)
            {
                 if(routeObject.routes.Contains(rawUrl))
                {
                    string physicalFileUrl = routeObject.file;
                    return RelativeThemePathToRootRelativePath(physicalFileUrl);
                }
            }
            return null;
        }


        /// <summary>
        /// Takes url relative to Theme folder, for example ./Style/style.css or ../Style/style.css or Style/style.css
        /// and transforms it to root url, which is ie. : Themes/Jasper/Styles/style.css
        /// </summary>
        /// <param name="path">Path relative to Theme folder</param>
        /// <returns></returns>
        public  string RelativeThemePathToRootRelativePath(string path)
        {
             // can contain relative parts ie. "Themes\\Jasper\\..//Styles/style.css"
            string p = System.IO.Path.Combine(GlobalWebsiteConfig.ThemeFolder, GlobalWebsiteConfig.ThemeName, path);

            // creates full path and resolves relative parts ==> "c:\\...\Themes\\Jasper\\Styles\\style.css" 
            string fullPath = System.IO.Path.GetFullPath(p); 

            // creates full path to the Themes folder ===> "c:\\..\\Themes"
            string refFullPath = System.IO.Path.GetFullPath(GlobalWebsiteConfig.ThemeFolder);

            // convert strings to absolute uris
            Uri urifullPath = new Uri(fullPath, UriKind.Absolute);
            Uri uriRefFullPath = new Uri(refFullPath, UriKind.Absolute);
            
            // creates root relative Uri 
            Uri rootRelativeUrl = uriRefFullPath.MakeRelativeUri(urifullPath);

            string result = rootRelativeUrl.ToString();

            return result;
            
        }

    }
}