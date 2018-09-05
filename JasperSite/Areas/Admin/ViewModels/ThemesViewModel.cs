using JasperSite.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class ThemesViewModel
    {
        public List<string> NotRegisteredThemeNames { get; set; }
        public List<string> ManuallyDeletedThemeNames { get; set; }

        public string ErrorFlag { get; set; }
        public string SelectedThemeName { get; set; }
        public string ThemeFolder { get; set; }
        public List<ThemeInfo> ThemeInfoList { get; set; }

        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalNumberOfPages { get; set; }
       
    }
}
