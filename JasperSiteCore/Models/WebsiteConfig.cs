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
            this._configurationObject = _dataProvider.GetFreshData() ?? throw new ConfigurationObjectException();
        }

        private IWebsiteConfigProvider _dataProvider;
        private ConfigurationObject _configurationObject;      
              

        //public ConfigurationObject GetConfigData()
        //{
        //    return this._configurationObject;          
        //}

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
            get { return _configurationObject.RoutingList; }
            set
            {
                _configurationObject.RoutingList = value;
            }
        }

        public List<ConfigurationObject.KeyValue> AppSettings
        {
            get { return _configurationObject.AppSettings; }
            set
            {
                _configurationObject.AppSettings = value;
            }
        }

        public List<ConfigurationObject.RouteMapping> CustomPageMapping
        {
            get { return _configurationObject.CustomPageMapping; }
            set
            {
                _configurationObject.CustomPageMapping = value;
            }
        }

        public List<string> BlockHolders
        {
            get { return _configurationObject.BlockHolders; }
            set
            {
                _configurationObject.BlockHolders = value;
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