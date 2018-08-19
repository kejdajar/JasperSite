using JasperSiteCore.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class SettingsViewModel
    {
      
       public SettingsNameViewModel model1 { get; set; }
        public JasperJsonViewModel model2 { get; set; }
       

    }

    public class SettingsNameViewModel
    {
        [Display(Name = "Název vašeho webu")]
        [Required(ErrorMessage = "Zvolte název vašeho webu")]
       public string WebsiteName { get; set; }
    }

    public class JasperJsonViewModel
    {
        
 public string JasperJson { get; set; }
    }
}
