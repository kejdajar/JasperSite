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
                
        [Required(ErrorMessage = "Vyplňte prosím název článku")]       
        [Display(Name="Název článku")]
        public string Name { get; set; }

        [Display(Name = "Text článku")]
        public string HtmlContent { get; set; }

        [Required(ErrorMessage ="Vyplňte prosím datum vytvoření")]
        [Display(Name = "Datum vytvoření")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? PublishDate { get; set; }

       
        [Display(Name ="Kategorie")]
        public List<Category> Categories{ get; set; }

        public int SelectedCategoryId { get; set; }

        [Display(Name="Publikovat článek")]
        public bool Publish { get; set; }
      
        public string Url { get; set; }

        public List<string> AllUrl { get; set; }

        public string ArticlesRoute { get; set; }
    }
}
