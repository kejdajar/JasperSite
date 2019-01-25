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
    public class ImagesViewModel
    {
       public List<Image> ImagesFromDatabase { get; set; }
       public bool SaveToDb { get; set; }
    }

   

   
}
