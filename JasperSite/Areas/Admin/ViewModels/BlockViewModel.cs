using JasperSite.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;
using JasperSite.Areas.Admin.Controllers;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class BlockViewModel
    {
        // Blocks section
        public List<BlocksViewModelData> Blocks { get; set; } = new List<BlocksViewModelData>();

        // Add new block section
        public NewTextBlock NewTextBlock { get; set; } = new NewTextBlock();
        public List<BlockHolder> AllBlockHolders { get; set; }

        // Themes section
        public List<Theme> AllThemes { get; set; }

        public int CurrentThemeId { get; set; }
    }

   

   
}
