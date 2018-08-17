using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class EditArticleViewModel
    {
       
        public int Id { get; set; }

        [RegularExpression(@"[^\s]+",ErrorMessage ="Název nemůže obsahovat pouze prázdné znaky.")]
        [Required(ErrorMessage = "Vyplňte prosím název článku")]       
        [Display(Name="Název článku")]
        public string Name { get; set; }

        [Display(Name = "Text článku")]
        public string HtmlContent { get; set; }

        [Required(ErrorMessage ="Vyplňte prosím datum publikace")]
        [Display(Name = "Datum publikace")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? PublishDate { get; set; }

       
        [Display(Name ="Kategorie")]
        public List<Category> Categories{ get; set; }

        public int SelectedCategoryId { get; set; }
    }
}
