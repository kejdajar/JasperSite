using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSite.Models.Providers
{
    public class GlobalConfigDataProviderJson: IGlobalWebsiteConfigProvider
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
        public GlobalConfigData GetFreshData()
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

        public string GetGlobalJsonFilePath()
        {
            return System.IO.Path.Combine(Env.Hosting.ContentRootPath, _jsonPath);
        }

        public void SaveData(GlobalConfigData dataToSave)
        {
            string jsonGlobalSettingsPath = GetGlobalJsonFilePath();
            string convertedDataToSave = JsonConvert.SerializeObject(dataToSave,Formatting.Indented);
            try
            {
                System.IO.File.WriteAllText(jsonGlobalSettingsPath, convertedDataToSave);
            }
            catch (Exception ex)
            {
                throw new GlobalConfigDataProviderException(ex);
            }

        }

    }

   
}
