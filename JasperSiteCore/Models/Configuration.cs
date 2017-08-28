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

            ConfigurationObjectProviderJson configurationObjectProvider = new ConfigurationObjectProviderJson(globalConfig,"jasper.json");
            WebsiteConfig websiteConfig = new WebsiteConfig(configurationObjectProvider.GetConfigData());

            CustomRouting customRouting = new CustomRouting(websiteConfig, globalConfig);

            GlobalWebsiteConfig = globalConfig;
            WebsiteConfig = websiteConfig;
            CustomRouting = customRouting;
        }

        
        public static GlobalWebsiteConfig GlobalWebsiteConfig { get; set; }
        public static WebsiteConfig WebsiteConfig { get; set; }
        public static CustomRouting CustomRouting { get; set; }
    }
}
