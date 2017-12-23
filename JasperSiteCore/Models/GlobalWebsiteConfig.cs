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
        public GlobalWebsiteConfig(GlobalConfigDataProviderJson gcdpj)
        {
            this.ConfigurationDataObject = gcdpj.GetGlobalConfigData() ?? throw new GlobalConfigDataException();
            GlobalConfigDataProviderJson = gcdpj;
        }

        public GlobalConfigDataProviderJson GlobalConfigDataProviderJson { get; set; }
        public GlobalConfigData ConfigurationDataObject { get; set; } 

       public void SaveGlobalConfigData(GlobalConfigData dataToSave)
        {
            GlobalConfigDataProviderJson.SaveGlobalConfigData(dataToSave);
        }

        public string ThemeName
        {
            get { return GetThemeName(); }
        }

        public  string ThemeFolder
        {
            get { return GetThemeFolder(); }
        }





        private string GetThemeFolder()
        {
            return ConfigurationDataObject.themeFolder;
        }

        private  string GetThemeName()
        {
            return ConfigurationDataObject.themeName;
        }        

       
      
    }


}
