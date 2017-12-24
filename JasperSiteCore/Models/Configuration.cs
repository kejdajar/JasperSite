using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Providers;
using Microsoft.AspNetCore.Hosting;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.Models;

namespace JasperSiteCore.Models
{
    public static class Configuration
    {

        static Configuration()
        {
            Initialize();
        }

        public static void Initialize()
        {
              
            GlobalConfigDataProviderJson globalProvider = new GlobalConfigDataProviderJson("jasper.json");
            GlobalWebsiteConfig globalConfig = new GlobalWebsiteConfig(globalProvider);

            ConfigurationObjectProviderJson configurationObjectProvider = new ConfigurationObjectProviderJson(globalConfig, "jasper.json");
            WebsiteConfig websiteConfig = new WebsiteConfig(configurationObjectProvider.GetConfigData());

            CustomRouting customRouting = new CustomRouting(websiteConfig, globalConfig);

            GlobalWebsiteConfig = globalConfig;
            WebsiteConfig = websiteConfig;
            CustomRouting = customRouting;

            ThemeHelper = new ThemeHelper();

            CreateAndSeedDb();
           
        }

        public static void CreateAndSeedDb(bool ensureDbIsDeleted = false)
        {
            if (GlobalWebsiteConfig.ConfigurationDataObject.installationCompleted == "true")
            {
                //try
                //{               
                DatabaseContext dbContext = new DatabaseContext();
                DbInitializer init = new DbInitializer(dbContext);
                init.Initialize(ensureDbIsDeleted);
                //}
                //catch(NotSupportedDatabaseException ex)
                //{

                //}
            }
            else
            {
                //  throw new NotImplementedException();
            }
        }
        
        public static bool InstallationCompleted()
        {
            if (Configuration.GlobalWebsiteConfig.ConfigurationDataObject.installationCompleted != "true")
            {
                return false;
            }
            else return true;
        }

        public static GlobalWebsiteConfig GlobalWebsiteConfig { get; set; }
        public static WebsiteConfig WebsiteConfig { get; set; }
        public static CustomRouting CustomRouting { get; set; }

        public static DbHelper DbHelper { get; set; }
        public static ThemeHelper ThemeHelper { get; set; }
    }
}
