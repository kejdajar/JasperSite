using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Fill in the username")]
       public string Username { get; set; }

        [Required(ErrorMessage ="Fill in the password")]
        public string Password { get; set; }

        public bool Remember { get; set; }


        
    }
}
