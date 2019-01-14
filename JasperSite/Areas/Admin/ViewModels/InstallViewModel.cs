using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Areas.Admin.Controllers;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class InstallViewModel
    {
     
        public int SelectedDatabase { get; set; }

        public bool RecreateDatabase { get; set; }

        [Required(ErrorMessage = "Fill in the connection string")]
        public string ConnectionString { get; set; }

        public List<DatabaseListItem> AllDatabases { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username can't be an empty string.")]
        public string Username { get; set; }

        [Display(Name = "Password")]      
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Password can't be an empty string.")]
        public string Password { get; set; }

        [Display(Name = "Password check")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]       
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password can't be an empty string.")]
        public string PasswordAgain { get; set; }
    }
}
