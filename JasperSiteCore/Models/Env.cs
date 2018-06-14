using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Models
{
    public static class Env
    {
        public static IHostingEnvironment Hosting;
        public static Microsoft.Extensions.DependencyInjection.IServiceCollection Services { get; set; }
        public static System.IServiceProvider ServiceProvider { get; set; }
    }
}
