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
            //Initialize();
        }

        public static void Initialize()
        {
            // Global jasper.json file is mapped to config classes
            GlobalConfigDataProviderJson globalJsonProvider = new GlobalConfigDataProviderJson("jasper.json");
            GlobalWebsiteConfig globalConfig = new GlobalWebsiteConfig(globalJsonProvider);

            // Theme jasper.json file is mapped to theme config classes
            // Theme jasper.json file depends on global settings of theme folder location
            ConfigurationObjectProviderJson configurationObjectJsonProvider = new ConfigurationObjectProviderJson(globalConfig, "jasper.json");
            WebsiteConfig websiteConfig = new WebsiteConfig(configurationObjectJsonProvider);

            // Custom routing manager
            CustomRouting customRouting = new CustomRouting(websiteConfig, globalConfig);

            // Theme helper manager
            ThemeHelper themeHelper = new ThemeHelper();

            // Assigning variables of this static class - cross request persistent
            GlobalWebsiteConfig = globalConfig;
            WebsiteConfig = websiteConfig;
            CustomRouting = customRouting;
            ThemeHelper = themeHelper;

            // Database manager
         // CreateAndSeedDb();
                        
        }

        public static void CreateAndSeedDb(DatabaseContext dbContext,bool ensureDbIsDeleted = false)
        {
            if (GlobalWebsiteConfig.InstallationCompleted)
            {
                //try
                //{               
                             
              

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
            if (!Configuration.GlobalWebsiteConfig.InstallationCompleted)
            {
                return false;
            }
            else return true;
        }

        public static GlobalWebsiteConfig GlobalWebsiteConfig { get; set; }
        public static WebsiteConfig WebsiteConfig { get; set; }
        public static CustomRouting CustomRouting { get; set; }
        public static ThemeHelper ThemeHelper { get; set; }        
        
    }
}
