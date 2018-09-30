using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class EditArticleViewModel
    {
       
        public int Id { get; set; }
                
        [Required(ErrorMessage = "Fill in the name of the article.")]       
        [Display(Name="Name of the article")]
        public string Name { get; set; }

        [Display(Name = "Article content")]
        public string HtmlContent { get; set; }

        [Required(ErrorMessage ="Fill in the date of the creation")]
        [Display(Name = "Date of creation")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? PublishDate { get; set; }

       
        [Display(Name ="Category")]
        public List<Category> Categories{ get; set; }

        public int SelectedCategoryId { get; set; }

        [Display(Name="Publish the article")]
        public bool Publish { get; set; }

        [Display(Name="Keywords")]
        public string Keywords { get; set; }
      
        public string Url { get; set; }

        public List<string> AllUrl { get; set; }

        public string ArticlesRoute { get; set; }
    }
}
