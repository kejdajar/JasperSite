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
            if(string.IsNullOrWhiteSpace(jsonPath)) throw new GlobalConfigDataProviderException();
            _jsonPath = jsonPath;
        }

        private string _jsonPath;

        /// <summary>
        /// Gets the lastest data from json configuration file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GlobalConfigDataProviderException"></exception>
        public GlobalConfigData GetGlobalConfigData()
        {
            try
            {
                string jsonGlobalSettings = System.IO.File.ReadAllText(System.IO.Path.Combine(Env.Hosting.ContentRootPath, _jsonPath));
                GlobalConfigData configData = JsonConvert.DeserializeObject<GlobalConfigData>(jsonGlobalSettings);
                return configData;
            }
            catch(Exception ex)
            {
                throw new GlobalConfigDataProviderException(ex);
            }
           
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
