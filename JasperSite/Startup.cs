using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using JasperSite.Models;
using JasperSite.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace JasperSite
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {          
            // Configuration files for the whole project
            var builder = new ConfigurationBuilder()
            
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Save IHostingEnvironment to static class (ie. to get Root path from controllers/other classes)
           JasperSite.Models.Env.Hosting = env;           
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
                options.LoginPath = "/Admin/Login";
                options.AccessDeniedPath = "/Admin/Login/UnauthorizedUser";
            });           

            // Added compatibility with new features, added support for localization
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                  .AddDataAnnotationsLocalization();

            // Localization folder is "Resources"
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // Configure supported cultures and localization options
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("cs")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");              
                options.SupportedCultures = supportedCultures;               
                options.SupportedUICultures = supportedCultures;

            });

            // DB Context service has to be added in ConfigureServices() method
            services.AddDbContext<DatabaseContext>();

            // First param is interface, second is actual class          
            services.AddScoped<IJasperDataServicePublic, DbHelperPublic>();

           // services.AddScoped<IJasperDataService, DbHelper>(); ---> this will inject non-public version of DbHelper class,
           // which relies heavily on exceptions 

            // DbHelper uses IDatabaseContext, otherwise error: "Unable to resolve service for type IDatabaseContext"
            // If the DbHelper class used only "DatabaseContext", this statement could be ommited. 
            services.AddScoped<IDatabaseContext, DatabaseContext>();

            // Stores services for accessing DatabaseContext from installation page, where
            // dependecy injection does not work, because the context was not build at that time yet
            // DbContext fetched from this static property is not uniqe per request, thus not suitable
            // for use in other controllers
            Env.Services = services;            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            //    APPLICATON DATABASE SEEDING
            //  DatabaseContext dbContext = ((ServiceProvider)serviceProvider).GetRequiredService<DatabaseContext>();
             
            //    Initializes GlobalWebsiteConfig, WebsiteCongig, ThemeHelper, CustomRouting
            //    Does not require DB Context, only reads data from text files on disc
               JasperSite.Models.Configuration.Initialize();

            //    DATABASE CAN NOT BE SEEDED HERE, BECAUSE THE CONNECTION STRING IS NOT AVAILABLE RIGH NOW
            //    JasperSite.Models.Configuration.CreateAndSeedDb(dbContext);

            Env.ServiceProvider = serviceProvider;


            CultureInfo en = new CultureInfo("en");
            en.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";

            CultureInfo cs = new CultureInfo("cs");
            cs.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";

            var supportedCultures = new[]
            { 
                en,
                cs
            };

           

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                // Default culture is English
                DefaultRequestCulture = new RequestCulture("en"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            // authentication - maybe not necessary here
            app.UseAuthentication();             

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
              
            }
            else
            {  
                // This exception handler will be risen when there is jasper.json
                // mapping record & existing file in the theme directory, BUT ERRROR
                // WILL OCCURE WITHIN THAT FILE (ie. syntax error)
                app.UseExceptionHandler("/Views/Shared/_FatalError.cshtml");           
            }
          
            

            #region Ignore jasper.json files
            app.Use((context, next) => {
                // Ignore requests that don't point to static files.
                string path = context.Request.Path;
                if (!path.EndsWith("jasper.json"))
                {
                    return next();
                }

                // Stop processing the request and return a 401 response.                
                context.Response.StatusCode = 401; //Unauthorized
                return Task.FromResult(0);
               
            });
            #endregion
            
            // wwwroot serves now static files
            app.UseStaticFiles();

            // Themes folder serves now static files as well
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //    Path.Combine(Directory.GetCurrentDirectory(), "Themes")), // Physical folder location
            //    RequestPath = new PathString("/Themes") // Url
            //});

                     
            
            //Theme folder must serve static files
                app.UseStaticFiles(new StaticFileOptions()
            {
                
                FileProvider = new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), JasperSite.Models.Configuration.ThemeFolder)), // Physical folder location
                RequestPath = new PathString("/" + JasperSite.Models.Configuration.ThemeFolder) // Url
            });


            // Area/Admin/Content servers static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), "Areas/Admin/Content")), // Physical folder location
                RequestPath = new PathString("/Areas/Admin/Content") // Url
            });

            //work in progress - some packages are directly served from node_modules
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //   Path.Combine(Directory.GetCurrentDirectory(), "node_modules")), // Physical folder location
            //    RequestPath = new PathString("/node_modules") // Url
            //});


            #region DefaultRoutingDisabled

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
            #endregion

            //Custom routing: Home controller handles all incoming requests.
            app.UseMvc(routes =>
            {
                //areas
                routes.MapRoute(name: "areaRoute",
                template: "{area:exists}/{controller=Home}/{action=Index}");

                // custom routing
                routes.MapRoute(
                    name: "dynamic",
                    defaults: new { controller = "Home", action = "Index" },
                    template: "{*.any}");
            });           
        }
    }
}
