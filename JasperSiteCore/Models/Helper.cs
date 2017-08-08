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
        /// Transforms Theme-relative Url to Root-relative Url
        /// </summary>
        /// <param name="url">Theme-relative Url</param>
        /// <returns></returns>
        public static string JasperUrl(string url, bool addTilde = false)
        {  
            // without slash on the beginning it would work only on pages without route parameters
            // urls has to look like: /Themes/Jasper/Styles/style.css
            // without the slash it would create the following: localhost/Home/Category/Themes/Jasper/Styles/style.css = undesirable
            string path = "/" + CustomRouting.RelativeThemePathToRootRelativePath(url);

            if(addTilde)
            {
                path = "~" + path;
            }

            return path;
        }
    }
}
