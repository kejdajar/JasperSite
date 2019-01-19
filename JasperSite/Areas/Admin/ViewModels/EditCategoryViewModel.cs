using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class EditCategoryViewModel
    {
       
        public int CategoryId { get; set; }

        [Display(Name = "Name of the category")]
        [Required(ErrorMessage ="Fill in the name of the category")]
        public string CategoryName { get; set; }        
    }
}
