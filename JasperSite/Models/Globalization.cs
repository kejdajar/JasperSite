using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSite.Models
{
    public static class Globalization
    {

        /// <summary>
        /// Path to the view is transformed from Filename.cshtml to Filename.{culture}.cshtml.
        /// Requested view has to be .cshtml file and culture has to be different from "en". Otherwise
        /// the result will be unmodified. In case of failure an empty string is returned.
        /// </summary>
        /// <param name="viewToReturn"></param>
        /// <returns></returns>
        public static string GlobalizeView(string viewToReturn, HttpRequest request)
        {
            try
            {
                var rqf = request.HttpContext.Features.Get<IRequestCultureFeature>();
                string cultureName = rqf.RequestCulture.Culture.Name;

                // Default culture is English
                if (cultureName == "en") return viewToReturn;

                string fDir = Path.GetDirectoryName(viewToReturn);
                string fName = Path.GetFileNameWithoutExtension(viewToReturn);
                string fExt = Path.GetExtension(viewToReturn);

                if (fExt.ToLower() != ".cshtml") return viewToReturn;

                string globalizedViewPath = Path.Combine(fDir, String.Concat(fName, ".", cultureName, fExt)).Replace(@"\","/");

                // If the path starts with tilde, it has to be removed to test the existence of file.
                if(globalizedViewPath.StartsWith(@"~/")) 
                {
                    string alteredPath = globalizedViewPath.Remove(0, 2);
                    if (System.IO.File.Exists(alteredPath)) return globalizedViewPath;
                    else return viewToReturn;                    
                }

               
                if (System.IO.File.Exists(globalizedViewPath)) return globalizedViewPath;
                else return viewToReturn;
            }
            catch
            {
                return string.Empty;
            }

        }
    }
}
