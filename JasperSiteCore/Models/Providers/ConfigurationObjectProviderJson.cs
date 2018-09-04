using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JasperSiteCore.Models
{
    public class ConfigurationObjectProviderJson:IWebsiteConfigProvider
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
        public ConfigurationObject GetFreshData()
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
        /// Returns path to the currently activated theme.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ThemeConfigurationFileNotFoundException"></exception>
        /// <exception cref="ConfigurationObjectProviderJsonException"></exception>
        public string GetThemeJasperJsonLocation()
        {
            try
            {
                string path = System.IO.Path.Combine(Env.Hosting.ContentRootPath, Configuration.ThemeFolder);
                string[] directories = Directory.GetDirectories(path, GlobalWebsiteConfig.ThemeName);
                if (directories.Count() == 1)
                {
                    string themeDirectoryUrl = directories[0]; // např. ~/Themes/JasperTheme
                    return Path.Combine(themeDirectoryUrl, _jsonPath);
                }
                else throw new ConfigurationObjectProviderJsonException("Theme folder contains more themes with the same name.");
            }
            catch(Exception ex)
            {
                throw new ThemeConfigurationFileNotFoundException(ex);
            }
        }

        /// <summary>
        /// Returns path to the specified theme.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ThemeConfigurationFileNotFoundException"></exception>
        /// <exception cref="ConfigurationObjectProviderJsonException"></exception>
        public string GetThemeJasperJsonLocation(string themeName)
        {
            try
            {
                string path = System.IO.Path.Combine(Env.Hosting.ContentRootPath, Configuration.ThemeFolder);
                string[] directories = Directory.GetDirectories(path, themeName);
                if (directories.Count() == 1)
                {
                    string themeDirectoryUrl = directories[0]; // např. ~/Themes/JasperTheme
                    return Path.Combine(themeDirectoryUrl, _jsonPath);
                }
                else throw new ConfigurationObjectProviderJsonException("Theme folder contains more themes with the same name.");
            }
            catch (Exception ex)
            {
                throw new ThemeConfigurationFileNotFoundException(ex);
            }
        }




        public void SaveData(ConfigurationObject dataToSave)
        {
            string jsonThemeSettingsPath = GetThemeJasperJsonLocation();
            string convertedDataToSave = JsonConvert.SerializeObject(dataToSave, Formatting.Indented);
            try
            {
                System.IO.File.WriteAllText(jsonThemeSettingsPath, convertedDataToSave);
            }
            catch (Exception ex)
            {
                throw new ConfigurationObjectProviderJsonException(ex);
            }

        }

    }

    
}
