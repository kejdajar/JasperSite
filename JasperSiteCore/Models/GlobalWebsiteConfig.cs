using JasperSiteCore.Models.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Models
{
    public class GlobalWebsiteConfig
    {
        public GlobalWebsiteConfig(GlobalConfigData configurationDataObject)
        {
            this.ConfigurationDataObject = configurationDataObject;
        }

        public GlobalConfigData ConfigurationDataObject { get; set; } 

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
