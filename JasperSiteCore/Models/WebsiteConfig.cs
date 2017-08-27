using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using JasperSiteCore.Models.Providers;

namespace JasperSiteCore.Models
{
    public class WebsiteConfig
    {
        public WebsiteConfig(ConfigurationObject configurationObject)
        {
            this.ConfigurationObject = configurationObject;
        }
        public ConfigurationObject ConfigurationObject { get; set; }
        

        //public static string themeName = "Jasper";
        //public static string themeFolder = "~/Themes";
        //public static string themeName = GlobalWebsiteConfig.ThemeName;
        //public static string themeFolder = GlobalWebsiteConfig.ThemeFolder;

        public ConfigurationObject GetConfigData()
        {
            return this.ConfigurationObject;
        }

    }

    

   
}