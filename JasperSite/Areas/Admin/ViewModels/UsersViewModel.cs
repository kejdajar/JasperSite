using JasperSite.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class UsersViewModel
    {
        public List<User> Users { get; set; }
    }
}
