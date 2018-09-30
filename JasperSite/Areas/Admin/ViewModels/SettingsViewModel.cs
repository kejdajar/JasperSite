using JasperSite.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class SettingsViewModel
    {

        public SettingsNameViewModel model1 { get; set; }
        public JasperJsonViewModel model2 { get; set; }
        public JasperJsonThemeViewModel model3 { get; set; }

    }

    public class SettingsNameViewModel
    {
        [Display(Name = "Website name")]
        [Required(ErrorMessage = "Choose the name of your website")]
        public string WebsiteName { get; set; }
    }

    public class JasperJsonViewModel
    {
        public string JasperJson { get; set; }
    }

    public class JasperJsonThemeViewModel
    {
        public string JasperJson { get; set; }
        public int SelectedThemeId { get; set; }

        [Display(Name ="Choose the theme")]
        public List<Theme> Themes { get; set; }
    }


}
