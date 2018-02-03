using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Vyplňte uživatelské jméno")]
       public string Username { get; set; }

        [Required(ErrorMessage ="Vyplňte heslo")]
        public string Password { get; set; }

        public bool Remember { get; set; }
    }
}
