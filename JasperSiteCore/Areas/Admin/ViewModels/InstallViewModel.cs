using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Areas.Admin.Controllers;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class InstallViewModel
    {
     
        public int SelectedDatabase { get; set; }

        [Required(ErrorMessage = "Zadejte připojovací řetězec")]
        public string ConnectionString { get; set; }

        public List<DatabaseListItem> AllDatabases { get; set; }
    }
}
