using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JasperSiteCore.Models
{
    public class ConfigurationObjectProviderJson
    {
        /// <summary>
        /// ConfigurationObjectProviderJson class
        /// </summary>
        /// <param name="globalWebsiteConfig"></param>
        /// <exception cref="ConfigurationObjectProviderJsonException"></exception>
        public ConfigurationObjectProviderJson(GlobalWebsiteConfig globalWebsiteConfig, string jsonPath = "jasper.json")
        {
            this.GlobalWebsiteConfig = globalWebsiteConfig ?? throw new ConfigurationObjectProviderJsonException();
            if(!string.IsNullOrWhiteSpace(jsonPath))
            {
                _jsonPath = jsonPath;
            }
            else
            {
                throw new ConfigurationObjectProviderJsonException();
            }
        }

        private string _jsonPath;

        public GlobalWebsiteConfig GlobalWebsiteConfig { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ConfigurationObjectProviderJsonException"></exception>
        public ConfigurationObject GetConfigData()
        {
            try
            {
                string jasperJsonUrl = GetThemeJasperJsonLocation();
                string jsonSettings = System.IO.File.ReadAllText(jasperJsonUrl);
                ConfigurationObject configData = JsonConvert.DeserializeObject<ConfigurationObject>(jsonSettings);
                return configData;
            }
            catch(Exception ex)
            {
                throw new ConfigurationObjectProviderJsonException(ex);
            }
           

            //string[] setting1 = configData.routing.homePage;
            //string key1 = configData.appSettings[0].key01;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ThemeConfigurationFileNotFoundException"></exception>
        public string GetThemeJasperJsonLocation()
        {
            try
            {
                string path = System.IO.Path.Combine(Env.Hosting.ContentRootPath, GlobalWebsiteConfig.ThemeFolder);
                string[] directories = Directory.GetDirectories(path, GlobalWebsiteConfig.ThemeName);
                if (directories.Count() == 1)
                {
                    string themeDirectoryUrl = directories[0]; // např. ~/Themes/JasperTheme
                    return Path.Combine(themeDirectoryUrl, _jsonPath);
                }
                else return null;
            }
            catch(Exception ex)
            {
                throw new ThemeConfigurationFileNotFoundException(ex);
            }
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
}
