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

    public class NewTextBlock
    {
        [Display(Name = "Name of the text block")]
        [Required(ErrorMessage = "Please fill in the name of the text block")]
        public string Name { get; set; }

        [Display(Name = "Quick block content")]
        public string Content { get; set; }

        public int BlockHolderId { get; set; }
    }

    public class EditTextBlock
    {

        public int Id { get; set; }


        [Display(Name = "Name of the text block")]
        [Required(ErrorMessage = "Fill in the name of the text block")]
        public string Name { get; set; }

        [Display(Name = "Block content")]
        public string Content { get; set; }


    }




}
