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

        [Required(ErrorMessage = "Fill in the connection string")]
        public string ConnectionString { get; set; }

        public List<DatabaseListItem> AllDatabases { get; set; }
    }
}
