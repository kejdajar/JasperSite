using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class CategoriesViewModel
    {
       public List<Category> Categories { get; set; }

        [Required(ErrorMessage = "Fill in the name of the category")]
        public string NewCategoryName { get; set; }


    }

}
