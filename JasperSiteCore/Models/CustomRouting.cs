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
            string[] homePageUrls = WebsiteConfig.RoutingList.HomePage;
            if(homePageUrls == null || homePageUrls.Length <1)
            {
                return null;
            }
            else
            {
                return homePageUrls;
            }
        }

        /// <summary>
        /// Returns root-relative path to homePage file.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CustomRoutingException"></exception>
        public string GetHomePageFile()
        {
            string physicalFileUrl = WebsiteConfig.RoutingList.HomePageFile;
            string path = RelativeThemePathToRootRelativePath(physicalFileUrl);
            if(System.IO.File.Exists(path))
            {
                return path;
            }
            else
            {
                throw new CustomRoutingException("Following view could not be found: " + path);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CustomRoutingException"></exception>
        public string GetErrorPageFile()
        {string physicalFileUrl = WebsiteConfig.RoutingList.ErrorPageFile;
            string path = RelativeThemePathToRootRelativePath(physicalFileUrl);
            if(System.IO.File.Exists(path))
            {
                return path;
            }
            else
            {
                throw new CustomRoutingException("Error page specified in theme's jasper.json file was not found:"+path);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawUrl"></param>
        /// <returns></returns>
        /// <exception cref="CustomRoutingException"></exception>
        public string MapUrlToFile(string rawUrl)
        {
            List<ConfigurationObject.RouteMapping> collection = WebsiteConfig.CustomPageMapping;
            foreach(ConfigurationObject.RouteMapping routeObject in collection)
            {
                 if(routeObject.Routes.Contains(rawUrl))
                {
                    string physicalFileUrl = routeObject.File;
                    string path = RelativeThemePathToRootRelativePath(physicalFileUrl);
                    if (System.IO.File.Exists(path))
                    {
                        return path;
                    }
                    else
                    {
                        throw new CustomRoutingException("Required view is specified in jasper.json file, but could not be found physically: " + path);
                    }
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
            if (string.IsNullOrWhiteSpace(path)) return null;            

             // can contain relative parts ie. "Themes\\Jasper\\..//Styles/style.css"
            string p = System.IO.Path.Combine(Configuration.ThemeFolder, GlobalWebsiteConfig.ThemeName, path);

            // creates full path and resolves relative parts ==> "c:\\...\Themes\\Jasper\\Styles\\style.css" 
            string fullPath = System.IO.Path.GetFullPath(p); 

            // creates full path to the Themes folder ===> "c:\\..\\Themes"
            string refFullPath = System.IO.Path.GetFullPath(Configuration.ThemeFolder);

            // convert strings to absolute uris
            Uri urifullPath = new Uri(fullPath, UriKind.Absolute);
            Uri uriRefFullPath = new Uri(refFullPath, UriKind.Absolute);
            
            // creates root relative Uri 
            Uri rootRelativeUrl = uriRefFullPath.MakeRelativeUri(urifullPath);

            string result = rootRelativeUrl.ToString();

            return result.Replace("%20"," "); // url cant contain %20 as space
            
        }

      

    }
}