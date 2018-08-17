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
        [RegularExpression(@"[^\s]+", ErrorMessage = "Název nemůže obsahovat pouze prázdné znaky.")]
        [Display(Name ="Název vašeho webu")]
        [Required(ErrorMessage ="Zvolte název vašeho webu")]
        public string WebsiteName { get; set; }

        public string JasperJson { get; set; }

    }
}
