using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class InstallViewModel
    {
        public string SelectedDatabase { get; set; }
        public string ConnectionString { get; set; }
    }
}
