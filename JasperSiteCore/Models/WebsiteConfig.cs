using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace JasperSiteCore.Models
{
    public static class WebsiteConfig
    {
        public static IHostingEnvironment Hosting;

        //public static string themeName = "Jasper";
        //public static string themeFolder = "~/Themes";
        //public static string themeName = GlobalWebsiteConfig.ThemeName;
        //public static string themeFolder = GlobalWebsiteConfig.ThemeFolder;


        private static string GetJasperJsonLocation(string themeName,string themeFolderUrl)
        {
            string[] directories = Directory.GetDirectories(System.IO.Path.Combine(Hosting.ContentRootPath,themeFolderUrl),themeName);
            if (directories.Count() == 1)
            {
                string themeDirectoryUrl = directories[0]; // např. ~/Themes/JasperTheme
                return Path.Combine(themeDirectoryUrl,"jasper.json");
            }
            else return null;
            
        }

       


        public static ConfigurationObject GetConfigData()
        {
            string jasperJsonUrl = GetJasperJsonLocation(GlobalWebsiteConfig.ThemeName, GlobalWebsiteConfig.ThemeFolder);
            string jsonSettings = System.IO.File.ReadAllText(jasperJsonUrl);
            ConfigurationObject configData = JsonConvert.DeserializeObject<ConfigurationObject>(jsonSettings);
            return configData;

            //string[] setting1 = configData.routing.homePage;
            //string key1 = configData.appSettings[0].key01;            
        }

       

       


    }

    /// <summary>        
    /// Představuje kontejner pro konfigurační data z JSON souboru.
    /// </summary>
    public class ConfigurationObject
    {
        public class Routing
        {
            [JsonProperty("homePage")]
            public string[] homePage { get; set; }

            [JsonProperty("homePageFile")]
            public string homePageFile { get; set; }

            [JsonProperty("errorPageFile")]
            public string errorPageFile { get; set; }
        }

        public class KeyValue
        {
            [JsonProperty("key")]
            public string key { get; set; }

            [JsonProperty("value")]
            public string value { get; set; }
        }

        [JsonProperty("routing")]
        public Routing routing { get; set; }

        [JsonProperty("appSettings")]
        public List<KeyValue> appSettings { get; set; }

        // CustomPageMapping
        [JsonProperty("customPageMapping")]
        public List<RouteMapping> customPageMapping { get; set; }

        public class RouteMapping
        {
            [JsonProperty("routes")]
            public string[] routes { get; set; }

            [JsonProperty("file")]
            public string file { get; set; }
        }
                
    }

    public static class GlobalWebsiteConfig
    {
        
        public static string ThemeName
        {
            
            get { return GetThemeName(); }
        }

       
        public static string ThemeFolder
        {
            get { return GetThemeFolder(); }
        }




        private static string GetThemeFolder()
        {
            return GetGlobalConfigData().themeFolder;
        }

        private static string GetThemeName()
        {
            return GetGlobalConfigData().themeName;
        }

       // private static GlobalConfigData globalData = GetGlobalConfigData();
        private static GlobalConfigData GetGlobalConfigData()
        {
            string jsonGlobalSettings = System.IO.File.ReadAllText(System.IO.Path.Combine(WebsiteConfig.Hosting.ContentRootPath,"jasper.json"));
            GlobalConfigData configData = JsonConvert.DeserializeObject<GlobalConfigData>(jsonGlobalSettings);
            return configData;
        }

        private class GlobalConfigData
        {
            [JsonProperty("themeName")]
            public string themeName { get; set; }

            [JsonProperty("themeFolder")]
            public string themeFolder { get; set; }
        }
    }
}