using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlocksController : Controller
    {
        public IActionResult Index()
        {

            var all_Themes = Configuration.DbHelper.GetAllThemes();
            var all_Holder_Block = Configuration.DbHelper.GetAllHolder_Blocks();
            var all_BlockHolder = Configuration.DbHelper.GetAllBlockHolders();
            var all_TextBlocks = Configuration.DbHelper.GetAllTextBlocks();

            BlockViewModel model = new BlockViewModel();
          
           
            foreach (TextBlock block in all_TextBlocks)
            {
               List<BlockHolderWithTheme> blockHolders = (from theme in all_Themes
                                    from holder_block in all_Holder_Block
                                    from blockHolder in all_BlockHolder
                                    where block.Id == holder_block.TextBlockId && holder_block.BlockHolderId == blockHolder.Id && theme.Id == blockHolder.ThemeId
                                    select new BlockHolderWithTheme{ BlockHolderName = blockHolder.Name, ThemeName = theme.Name, Order = holder_block.Order }).ToList();

                
                BlocksViewModelData bvmd = new BlocksViewModelData() { TextBlock = block, BlocHolderWithTheme =blockHolders};
                model.Blocks.Add(bvmd);
            }
            

            //List<BlocksViewModelData> result = (from themes in all_Themes
            //             from holder_block in all_Holder_Block
            //             from blockHolder in all_BlockHolder
            //             from textBlock in all_TextBlocks
            //             where textBlock.Id == holder_block.TextBlockId && holder_block.BlockHolderId == blockHolder.Id && themes.Id == blockHolder.ThemeId
            //             select new BlocksViewModelData { TextBlockName = textBlock.Name,BlockHolderName=blockHolder.Name }).ToList();

            
            return View(model);
        }
    }

    public class BlocksViewModelData
    {
        public TextBlock TextBlock{ get; set; }
        public List<BlockHolderWithTheme> BlocHolderWithTheme { get; set; }
    }

    public class BlockHolderWithTheme
    {
        public string BlockHolderName { get; set; }
        public string ThemeName { get; set; }
        public int Order { get; set; }
    }

}