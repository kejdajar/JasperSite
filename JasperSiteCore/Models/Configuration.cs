using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Providers;
using Microsoft.AspNetCore.Hosting;

namespace JasperSiteCore.Models
{
    public static class Configuration
    {

        static Configuration()
        {
            GlobalConfigDataProviderJson globalProvider = new GlobalConfigDataProviderJson("jasper.json");
            GlobalWebsiteConfig globalConfig = new GlobalWebsiteConfig(globalProvider.GetGlobalConfigData());

            ConfigurationObjectProviderJson configurationObjectProvider = new ConfigurationObjectProviderJson(globalConfig);
            WebsiteConfig websiteConfig = new WebsiteConfig(configurationObjectProvider.GetConfigData());

            GlobalWebsiteConfig = globalConfig;
            WebsiteConfig = websiteConfig;
        }

        
        public static GlobalWebsiteConfig GlobalWebsiteConfig { get; set; }
        public static WebsiteConfig WebsiteConfig { get; set; }
    }
}
