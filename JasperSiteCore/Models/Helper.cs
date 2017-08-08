using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Models
{
    public static class Helper
    {
        /// <summary>
        /// Converts relative path to server mapped path.
        /// </summary>
        /// <param name="url">Relative path.</param>
        /// <returns></returns>
        public static HtmlString JasperUrl(string url)
        {

            //string root = WebsiteConfig.Hosting.ContentRootPath;



            string path = CustomRouting.RelativeThemePathToRootRelativePath(url);
            string fullPath = System.IO.Path.GetFullPath(path);


            return new HtmlString(path);
        }


        public static HtmlString JasperUrl2(string url)
        {

            //string root = WebsiteConfig.Hosting.ContentRootPath;



            string path = CustomRouting.RelativeThemePathToRootRelativePath(url);
            string fullPath = System.IO.Path.GetFullPath(path);

            Uri fullPathUri = new Uri(fullPath, UriKind.Absolute);
           // Uri relPathUri = new Uri(CustomRouting.);


            return new HtmlString(path);
        }
    }
}
