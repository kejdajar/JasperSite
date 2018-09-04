﻿using JasperSiteCore.Models.Providers;
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
            get { string name = ConfigurationDataObject.ThemeName;
                if (name == null) return string.Empty; else return name;
            }
            set
            {
                ConfigurationDataObject.ThemeName = value;
                CommitChanges();
            }
        }      

        public string ConnectionString
        {
            get { string connString = ConfigurationDataObject.ConnectionString;
                if (connString == null) return string.Empty; else return connString;
            }
            set
            {
                ConfigurationDataObject.ConnectionString = value;
                CommitChanges();
            }
        }

        public string TypeOfDatabase
        {
            get { string typeOfDb= ConfigurationDataObject.TypeOfDatabase;
                if (typeOfDb == null) return string.Empty; else return typeOfDb;
                    }
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

        public void ResetToDefaults()
        {
            Configuration.GlobalWebsiteConfig.ConnectionString = "";
            Configuration.GlobalWebsiteConfig.InstallationCompleted = false;           
            Configuration.GlobalWebsiteConfig.ThemeName = "Default";
            Configuration.GlobalWebsiteConfig.TypeOfDatabase = "mssql";
        }

        public string GetGlobalJsonFileAsString()
        {
            string path = GlobalConfigDataProvider.GetGlobalJsonFilePath();
            return System.IO.File.ReadAllText(path);
        }

        /// <summary>
        /// Saves string to jasper.json global configuration file.
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <exception cref="GlobalConfigDataException"></exception>
        public void SaveGlobalJsonFileAsString(string jsonContent)
        {
            try
            {
                string path = GlobalConfigDataProvider.GetGlobalJsonFilePath();
                System.IO.File.WriteAllText(path, jsonContent);
            }
            catch (Exception)
            {
                throw new GlobalConfigDataException();
            }
        }

        

    }

    public interface IGlobalWebsiteConfigProvider
    {
       GlobalConfigData GetFreshData();
       void SaveData(GlobalConfigData dataToSave);
        string GetGlobalJsonFilePath();
    }

    public class GlobalConfigData
    {
        [JsonProperty("themeName")]
        public string ThemeName { get; set; }    


        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }


        [JsonProperty("typeOfDatabase")]
        public string TypeOfDatabase { get; set; }

        [JsonProperty("installationCompleted")]
        public string InstallationCompleted { get; set; }
    }

}
