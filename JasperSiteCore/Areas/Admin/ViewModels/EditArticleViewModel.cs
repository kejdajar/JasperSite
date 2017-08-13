using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class EditArticleViewModel
    {
       
        public int Id { get; set; }

        [Required]
        [Display(Name="Název článku")]
        public string Name { get; set; }

        [Display(Name = "Text článku")]
        public string HtmlContent { get; set; }

        [Required]
        [Display(Name = "Datum publikace")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime PublishDate { get; set; }
    }
}
