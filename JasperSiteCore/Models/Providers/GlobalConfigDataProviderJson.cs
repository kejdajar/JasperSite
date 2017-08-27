using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Models.Providers
{
    public class GlobalConfigDataProviderJson
    {

        public GlobalConfigDataProviderJson(string jsonPath = "jasper.json")
        {
            JsonPath = jsonPath;
        }

        public string JsonPath { get; set; }

        /// <summary>
        /// Gets the lastest data from json configuration file
        /// </summary>
        /// <returns></returns>
        public GlobalConfigData GetGlobalConfigData()
        {
            string jsonGlobalSettings = System.IO.File.ReadAllText(System.IO.Path.Combine(Env.Hosting.ContentRootPath, JsonPath));
            GlobalConfigData configData = JsonConvert.DeserializeObject<GlobalConfigData>(jsonGlobalSettings);
            return configData;
        }

    }

    public class GlobalConfigData
    {
        [JsonProperty("themeName")]
        public string themeName { get; set; }

        [JsonProperty("themeFolder")]
        public string themeFolder { get; set; }
    }
}
