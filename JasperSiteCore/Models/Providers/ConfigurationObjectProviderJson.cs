using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Models
{
    public class ConfigurationObjectProviderJson
    {
      

        public ConfigurationObjectProviderJson(GlobalWebsiteConfig globalWebsiteConfig)
            {
                this.GlobalWebsiteConfig = globalWebsiteConfig;
            }

            public GlobalWebsiteConfig GlobalWebsiteConfig { get; set; }

        public ConfigurationObject GetConfigData()
        {
            string jasperJsonUrl = GetJasperJsonLocation(GlobalWebsiteConfig.ThemeName, GlobalWebsiteConfig.ThemeFolder);
            string jsonSettings = System.IO.File.ReadAllText(jasperJsonUrl);
            ConfigurationObject configData = JsonConvert.DeserializeObject<ConfigurationObject>(jsonSettings);
            return configData;

            //string[] setting1 = configData.routing.homePage;
            //string key1 = configData.appSettings[0].key01;            
        }


        private string GetJasperJsonLocation(string themeName, string themeFolderUrl)
        {
            string[] directories = Directory.GetDirectories(System.IO.Path.Combine(Env.Hosting.ContentRootPath, themeFolderUrl), themeName);
            if (directories.Count() == 1)
            {
                string themeDirectoryUrl = directories[0]; // např. ~/Themes/JasperTheme
                return Path.Combine(themeDirectoryUrl, "jasper.json");
            }
            else return null;

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
