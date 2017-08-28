using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using JasperSiteCore.Models.Providers;
using JasperSiteCore.Models;


namespace JasperSiteCore.Models
{/// <summary>
/// Configuration class
/// </summary>
/// <exception cref="ConfigurationObjectException"></exception>
    public class WebsiteConfig
    {
        public WebsiteConfig(ConfigurationObject configurationObject)
        {
            this._configurationObject = configurationObject ?? throw new ConfigurationObjectException();
        }

        private ConfigurationObject _configurationObject;      

        //public static string themeName = "Jasper";
        //public static string themeFolder = "~/Themes";
        //public static string themeName = GlobalWebsiteConfig.ThemeName;
        //public static string themeFolder = GlobalWebsiteConfig.ThemeFolder;

        public ConfigurationObject GetConfigData()
        {
            return this._configurationObject;
        }

    }

    

   
}