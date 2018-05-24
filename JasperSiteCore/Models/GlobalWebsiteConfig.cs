using JasperSiteCore.Models.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Models
{
    /// <summary>
    /// Global website configuration class
    /// </summary>
    /// <exception cref="GlobalConfigDataException"></exception>
    public class GlobalWebsiteConfig
    {
        public GlobalWebsiteConfig(IGlobalWebsiteConfigProvider dataProvider)
        {
            this.ConfigurationDataObject = dataProvider.GetFreshData() ?? throw new GlobalConfigDataException();
            GlobalConfigDataProvider = dataProvider;
        }

        public IGlobalWebsiteConfigProvider GlobalConfigDataProvider { get; set; }
        private GlobalConfigData ConfigurationDataObject { get; set; } 

        public void SaveData(GlobalConfigData dataToSave)
        {
            GlobalConfigDataProvider.SaveData(dataToSave);
        }

        public void SetTemporaryThemeName(string themeName)
        {
            this.ConfigurationDataObject.ThemeName = themeName;
        }

        public string ThemeName
        {
            get { return ConfigurationDataObject.ThemeName; }
            set
            {
                ConfigurationDataObject.ThemeName = value;
                CommitChanges();
            }
        }

        public  string ThemeFolder
        {
            get { return ConfigurationDataObject.ThemeFolder; }
            set
            {
                ConfigurationDataObject.ThemeFolder = value;
                CommitChanges();
            }
        }

        public string ConnectionString
        {
            get { return ConfigurationDataObject.ConnectionString; }
            set
            {
                ConfigurationDataObject.ConnectionString = value;
                CommitChanges();
            }
        }

        public string TypeOfDatabase
        {
            get { return ConfigurationDataObject.TypeOfDatabase; }
            set
            {
                ConfigurationDataObject.TypeOfDatabase = value;
                CommitChanges();
            }
        }

        public bool InstallationCompleted
        {
            get {
                bool flag;
                bool parsed = Boolean.TryParse(ConfigurationDataObject.InstallationCompleted, out flag);
                if (flag) return parsed; else return false;              
            }
            set
            {
                ConfigurationDataObject.InstallationCompleted = value.ToString();
                CommitChanges();
            }
        }

        public void CommitChanges()
        {
            SaveData(ConfigurationDataObject);
        }

        public void RefreshData()
        {
            this.ConfigurationDataObject = GlobalConfigDataProvider.GetFreshData();
        }

    }

    public interface IGlobalWebsiteConfigProvider
    {
       GlobalConfigData GetFreshData();
       void SaveData(GlobalConfigData dataToSave);
    }

    public class GlobalConfigData
    {
        [JsonProperty("themeName")]
        public string ThemeName { get; set; }

        [JsonProperty("themeFolder")]
        public string ThemeFolder { get; set; }


        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }


        [JsonProperty("typeOfDatabase")]
        public string TypeOfDatabase { get; set; }

        [JsonProperty("installationCompleted")]
        public string InstallationCompleted { get; set; }
    }

}
