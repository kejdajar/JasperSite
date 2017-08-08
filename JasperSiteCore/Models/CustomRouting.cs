using System;
using System.Collections.Generic;
using System.Linq;


namespace JasperSiteCore.Models
{
    public static class CustomRouting
    {
        public static bool IsHomePage(string rawUrl)
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

        public static string[] GetHomePageUrls()
        {
            return WebsiteConfig.GetConfigData().routing.homePage;
        }

        public static string GetHomePageFile()
        {
            string physicalFileUrl = WebsiteConfig.GetConfigData().routing.homePageFile;
            return RelativeThemePathToAbsolute(physicalFileUrl);
        }

        public static string GetErrorPageFile()
        {string physicalFileUrl = WebsiteConfig.GetConfigData().routing.errorPageFile;
            return RelativeThemePathToAbsolute(physicalFileUrl);
        }

        public static string MapUrlToFile(string rawUrl)
        {
            List<ConfigurationObject.RouteMapping> collection = WebsiteConfig.GetConfigData().customPageMapping;
            foreach(ConfigurationObject.RouteMapping routeObject in collection)
            {
                 if(routeObject.routes.Contains(rawUrl))
                {
                    string physicalFileUrl = routeObject.file;
                    return RelativeThemePathToAbsolute(physicalFileUrl);
                }
            }
            return null;
        }

        /// <summary>
        /// Cestu je potřeba předat
        /// custom routing enginu v absolutní cestě s vlnovkou na začátku. 
        /// </summary>
        /// <param name="path">Absolutní/relativní cesta k souboru, který je součástí vzhledu.</param>
        /// <returns></returns>
        private static string RelativeThemePathToAbsolute(string path)
        {
            if (path.StartsWith("~")) // pokud v configu bude celá cesta (tzn. s vlnovkou), tak ponechat
            {
                return path;
            }
            else // Z relativní cesty udělat absolutní
            {
                return System.IO.Path.Combine(GlobalWebsiteConfig.ThemeFolder, GlobalWebsiteConfig.ThemeName, path);

            }
        }

    }
}