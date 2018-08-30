using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Areas.Admin.Controllers;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class HomeViewModel
    {     
        public List<Article> Articles { get; set; }
        public List<Category> Categories { get; set; }
        public User CurrentUser { get; set; }
    }
}
