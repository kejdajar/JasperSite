using JasperSiteCore.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models.Database;
using Microsoft.AspNetCore.Mvc;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    
    public class EditUserViewModel
    {
       public int Id { get; set; }

        [Display(Name = "Přezdívka")]
        [Required(ErrorMessage ="Vyplňte přezdívku")]
        public string Nickname { get; set; }

        [Display(Name = "Uživatelské jméno")]
        [Required(ErrorMessage = "Vyplňte uživatelské jméno")]
        public string Username { get; set; }

        [Display(Name = "Zadejte vaše nové heslo")]
        [Required(ErrorMessage = "Vyplňte vaše nové heslo")]
        public string NewPasswordPlainText { get; set; }

        [Display(Name="Zadejte vaše staré heslo")]
        [Required(ErrorMessage = "Vyplňte vaše staré heslo")]
        public string OldPasswordPlainText { get; set; }

        public int RoleId { get; set; }       

        [Display(Name = "Zvolte roli")] 
        public List<Role> AllRoles { get; set; }
     
    }
}
