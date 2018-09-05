using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Areas.Admin.Controllers;
using JasperSite.Models.Database;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class HomeViewModel
    {     
        public List<Article> Articles { get; set; }
        public List<Category> Categories { get; set; }
        public User CurrentUser { get; set; }
    }
}
