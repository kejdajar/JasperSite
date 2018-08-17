using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class CategoriesViewModel
    {
       public List<Category> Categories { get; set; }
       
       public NewCategoryViewModel NewCategory { get; set; }

    }

    public class NewCategoryViewModel
    {
        [RegularExpression(@"[^\s]+", ErrorMessage = "Název nemůže obsahovat pouze prázdné znaky.")]
        [Required( ErrorMessage ="Vyplňte jméno rubriky")]
       public string NewCategoryName { get; set; }
    }
}
