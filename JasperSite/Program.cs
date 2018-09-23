using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using JasperSite.Models.Database;
using Microsoft.Extensions.Logging;

namespace JasperSite
{
    public class Program
    {
        public static void Main(string[] args)
        {          

            var host = BuildWebHost(args);
           
            // Old seeding
            //using (var scope = host.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    try
            //    {
            //        var context = services.GetRequiredService<DatabaseContext>();
            //        DbInitializer init = new DbInitializer(context);
            //        init.Initialize();
            //    }
            //    catch(Exception ex)
            //    {
            //        var logger = services.GetRequiredService<ILogger<Program>>();
            //        logger.LogError(ex, "An error occurred while seeding the database");
            //    }
            //    host.Run();
            //}

            host.Run();
        }
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseUrls("http://*:5000") // Kestrel server is available from another computer in LAN on port 5000 and IP address
            .Build();
    }
}
