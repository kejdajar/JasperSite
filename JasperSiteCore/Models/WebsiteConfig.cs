using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using JasperSiteCore.Models.Providers;
using JasperSiteCore.Models;
using static JasperSiteCore.Models.ConfigurationObject;

namespace JasperSiteCore.Models
{/// <summary>
/// Configuration class
/// </summary>
/// <exception cref="ConfigurationObjectException"></exception>
    public class WebsiteConfig
    {
        public WebsiteConfig(IWebsiteConfigProvider dataProvider)
        {
            this._dataProvider = dataProvider ?? throw new ConfigurationObjectException();

            if (dataProvider == null) throw new ConfigurationObjectException("Data provider parameter can't be null.");

            try
            {
                ConfigurationObject settingsFromThemeJasperJson = _dataProvider.GetFreshData();
                this._configurationObject = settingsFromThemeJasperJson;
            }
            catch
            {
             this._configurationObject = null;
            }
          
        }

        private void ThrowError(Exception innerException)
        {
            string errorMsg = "JSON theme configuration data file could not be found or contains invalid data.";

            if (Configuration.GlobalWebsiteConfig != null)
                errorMsg += (Configuration.GlobalWebsiteConfig.ThemeName != null) ? "Folder with missing jasper.json: " + Configuration.GlobalWebsiteConfig.ThemeName.ToString() + ". " : string.Empty;

            throw new ConfigurationObjectProviderJsonException(errorMsg, innerException);
        }

        private IWebsiteConfigProvider _dataProvider;
        private ConfigurationObject _configurationObject;      
              
              

        public void RefreshData()
        {
            _configurationObject = _dataProvider.GetFreshData();
        }


        public void CommitChanges()
        {
            _dataProvider.SaveData(_configurationObject);
        }

        public void SaveData(ConfigurationObject dataToSave)
        {
           _dataProvider.SaveData(dataToSave);
        }

        // Properties supporting get + set (saves data in file after calling CommitChanges())

        public ConfigurationObject.Routing RoutingList
        {
            get
            {
                try
                {
                    return _configurationObject.RoutingList;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                    return null;
                }
            }
            set
            {
                try {
                    _configurationObject.RoutingList = value;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);                    
                }
            }
        }

        public List<ConfigurationObject.KeyValue> AppSettings
        {
            get
            {
                try
                {
                    return _configurationObject.AppSettings;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    _configurationObject.AppSettings = value;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                }
            }
        }

        public List<ConfigurationObject.RouteMapping> CustomPageMapping
        {
            get
            {
                try
                {
                    return _configurationObject.CustomPageMapping;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    _configurationObject.CustomPageMapping = value;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                }
            }
        }

        public List<string> BlockHolders
        {
            get
            {
                try
                {
                    return _configurationObject.BlockHolders;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    _configurationObject.BlockHolders = value;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);

                }
            }
        }

        public string MissingImagePath
        {
            get
            {
                try
                {
                    return _configurationObject.MissingImagePath;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    _configurationObject.MissingImagePath = value;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                }
            }
        }



        public bool UrlRewriting
        {
            get
            {
                try
                {
                    return _configurationObject.UrlRewriting;
                }
                catch (Exception)
                {                    
                    return false;
                }
            }
            set
            {
                try
                {
                    _configurationObject.UrlRewriting = value;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                }
            }
        }

        public string ArticleRoute
        {
            get
            {
                try
                {
                    
                        return _configurationObject.ArticleRoute;                   
                    
                }
                catch (Exception)
                {

                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    _configurationObject.ArticleRoute= value;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                }
            }
        }

        public string ArticleFile
        {
            get
            {
                try
                {
                  
                        return _configurationObject.ArticleFile;
                   
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    _configurationObject.ArticleFile= value;
                }
                catch (Exception ex)
                {
                    ThrowError(ex);
                }
            }
        }

    }

    public interface IWebsiteConfigProvider
    {
        ConfigurationObject GetFreshData();
        void SaveData(ConfigurationObject dataToSave);
    }

    /// <summary>        
    /// Představuje kontejner pro konfigurační data z JSON souboru.
    /// </summary>
    public class ConfigurationObject
    {
        [JsonProperty("urlRewriting")]
        public bool UrlRewriting { get; set; }

        [JsonProperty("articleFile")]
        public string ArticleFile { get; set; }

        [JsonProperty("articleRoute")]
        public string ArticleRoute { get; set; }

        [JsonProperty("missingImagePath")]
        public string MissingImagePath { get; set; }

        [JsonProperty("blockHolders")]
        public List<string> BlockHolders { get; set; }

        public class Routing
        {
            [JsonProperty("homePage")]
            public string[] HomePage { get; set; }

            [JsonProperty("homePageFile")]
            public string HomePageFile { get; set; }

            [JsonProperty("errorPageFile")]
            public string ErrorPageFile { get; set; }
        }

        public class KeyValue
        {
            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        [JsonProperty("routingList")]
        public Routing RoutingList { get; set; }

        [JsonProperty("appSettings")]
        public List<KeyValue> AppSettings { get; set; }

        // CustomPageMapping
        [JsonProperty("customPageMapping")]
        public List<RouteMapping> CustomPageMapping { get; set; }

        public class RouteMapping
        {
            [JsonProperty("routes")]
            public string[] Routes { get; set; }

            [JsonProperty("file")]
            public string File { get; set; }
        }

    }

}