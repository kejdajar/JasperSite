﻿using System;
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
       
       public NewCategoryViewModel NewCategory { get; set; }

        

    }

    public class NewCategoryViewModel
    {
        
       [Required( ErrorMessage ="Vyplňte jméno rubriky")]
       public string NewCategoryName { get; set; }
    }
}