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
    public class AddedAndLooseHoldersViewModel
    {
        public int CurrentTextBoxId { get; set; }
        public List<BlockHolder> AllBlockHolders { get; set; } = new List<BlockHolder>();
        public List<BlockHolder> CorrespondingBlockHolders { get; set; } = new List<BlockHolder>();
        public List<Holder_Block> CorrespondingHolder_Blocks { get; set; } = new List<Holder_Block>();
    }

   

   
}
