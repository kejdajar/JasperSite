using JasperSite.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;
using Microsoft.AspNetCore.Mvc;

namespace JasperSite.Areas.Admin.ViewModels
{
    
    public class AddUserViewModel
    {
       public int Id { get; set; }

        [Display(Name = "Nickname")]
        [Required(ErrorMessage ="Fill in the nickname")]
        public string Nickname { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Fill in the username")]
        public string Username { get; set; }

        [Display(Name = "Fill in the new password")]
        [Required(ErrorMessage = "Fill in the new password")]
        public string NewPasswordPlainText { get; set; }

        [Display(Name="Fill in the new password again")]
        [Required(ErrorMessage = "Fill in the new password again")]
        public string NewPasswordPlainTextAgain { get; set; }

        public int RoleId { get; set; }       

        [Display(Name = "Choose the user role")] 
        public List<Role> AllRoles { get; set; }
     
    }
}
