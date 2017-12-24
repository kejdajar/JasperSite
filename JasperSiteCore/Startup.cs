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
using JasperSiteCore.Models;
using JasperSiteCore.Models.Database;
using Microsoft.EntityFrameworkCore;


namespace JasperSiteCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Configuration files for whole project
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Save IHostingEnvironment to static class (ie. to get Root path from controllers/other classes)
           JasperSiteCore.Models.Env.Hosting = env;            
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>();
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DatabaseContext dbContext)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {    //TODO: Exception Handling
                //app.UseExceptionHandler("/Home/Error/");

            // Redirects http status code errors to error controller
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
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

            //"Theme" folder must serve static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), JasperSiteCore.Models.Configuration.GlobalWebsiteConfig.ThemeFolder)), // Physical folder location
                RequestPath = new PathString("/" + JasperSiteCore.Models.Configuration.GlobalWebsiteConfig.ThemeFolder) // Url
            });


            // Area/Admin/Content servers static files
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), "Areas/Admin/Content")), // Physical folder location
                RequestPath = new PathString("/Areas/Admin/Content") // Url
            });

            //npm
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), "node_modules")), // Physical folder location
                RequestPath = new PathString("/node_modules") // Url
            });


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

            // Asp.Net Core < 2.0.0
           // Models.Database.DbInitializer.Initialize(dbContext);
        }
    }
}
