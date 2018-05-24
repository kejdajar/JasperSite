using JasperSiteCore.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.Controllers;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class BlockViewModel
    {
        public List<BlocksViewModelData> Blocks { get; set; } = new List<BlocksViewModelData>();
    }
}
