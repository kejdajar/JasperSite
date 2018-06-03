using JasperSiteCore.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.Controllers;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class EditBlockViewModel
    {
       
       public EditTextBlock TextBlock { get; set; }
        
       public AddedAndLooseHoldersViewModel HolderManagement { get; set; }
      
    }

   

   
}
