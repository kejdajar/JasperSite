using JasperSite.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;
using JasperSite.Areas.Admin.Controllers;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class EditBlockViewModel
    {
       
       public EditTextBlock TextBlock { get; set; }
        
       public AddedAndLooseHoldersViewModel HolderManagement { get; set; }
      
    }

   

   
}
