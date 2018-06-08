using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Models.Database;
using System.ComponentModel.DataAnnotations;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlocksController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ModelState.Clear();
            return View(UpdatePage());
        }

        private readonly DatabaseContext _databaseContext;
        private readonly DbHelper _dbHelper;
        public BlocksController(DatabaseContext dbContext)
        {
            this._databaseContext = dbContext;
            this._dbHelper = new DbHelper(dbContext);
        }

        public BlockViewModel UpdatePage()
        {
            var all_Themes = _dbHelper.GetAllThemes();
            var all_Holder_Block = _dbHelper.GetAllHolder_Blocks();
            var all_BlockHolder = _dbHelper.GetAllBlockHolders();
            var all_TextBlocks = _dbHelper.GetAllTextBlocks();

            BlockViewModel model = new BlockViewModel();


            foreach (TextBlock block in all_TextBlocks)
            {
                List<BlockHolderWithTheme> blockHolders = (from theme in all_Themes
                                                           from holder_block in all_Holder_Block
                                                           from blockHolder in all_BlockHolder
                                                           where block.Id == holder_block.TextBlockId && holder_block.BlockHolderId == blockHolder.Id && theme.Id == blockHolder.ThemeId
                                                           select new BlockHolderWithTheme { BlockHolderName = blockHolder.Name, ThemeName = theme.Name, Order = holder_block.Order }).ToList();


                BlocksViewModelData bvmd = new BlocksViewModelData() { TextBlock = block, BlocHolderWithTheme = blockHolders };
                model.Blocks.Add(bvmd);
            }

            model.AllBlockHolders = _dbHelper.GetAllBlockHolders();
            return model;
        }

        [HttpPost]
        public IActionResult AddNewBlock(BlockViewModel model)
        {
            string newBlockName = model.NewTextBlock.Name;
            int newBlock_BlockHolderId = model.NewTextBlock.BlockHolderId;
            string newBlockContent = model.NewTextBlock.Content;

            TextBlock tb = new TextBlock() { Content = newBlockContent, Name = newBlockName };
            TextBlock addedBlock= _dbHelper.AddNewBlock(tb);

            Holder_Block hb = new Holder_Block();
            hb.BlockHolderId = newBlock_BlockHolderId;
            hb.TextBlockId = addedBlock.Id;
            _dbHelper.AddNewHolder_Block(hb);

            ModelState.Clear();

            return PartialView("BlockFormPartialView",UpdatePage());
        }

        [HttpGet]
        public IActionResult DeleteBlock(int id)
        {
            _dbHelper.DeleteTextBlockById(id);

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {

                return PartialView("BlockFormPartialView",UpdatePage());
            }
            else
            {
                return RedirectToAction("Index");
            }

                
        }

        [HttpGet]
        public IActionResult DeleteBlockHolder(int id)
        {
            _dbHelper.DeleteBlockHolderById(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
         public IActionResult EditBlock(int blockId)
        {
            TextBlock tbToEdit = _dbHelper.GetAllTextBlocks().Where(tb => tb.Id == blockId).Single();




            List<BlockHolder> correspondingBlockHolders = GetCorrespondingBlockHolders(blockId);

            List<BlockHolder> allBlockHolders = _dbHelper.GetAllBlockHolders().ToList();

            EditBlockViewModel model = new EditBlockViewModel();
          
            model.HolderManagement = new AddedAndLooseHoldersViewModel();
            model.HolderManagement.CurrentTextBoxId = blockId;
            model.TextBlock = new EditTextBlock() { Name=tbToEdit.Name,Content=tbToEdit.Content,Id=tbToEdit.Id};
            model.HolderManagement.CorrespondingBlockHolders = correspondingBlockHolders;
            model.HolderManagement.AllBlockHolders = _dbHelper.GetAllBlockHolders().ToList();
            model.HolderManagement.CorrespondingHolder_Blocks = GetCorrespondingHolder_Blocks(blockId);
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveTextBlockOrder(int textBlockId, int holderId, int order)
        {
            _dbHelper.SaveTextBlockOrderNumberInHolder(textBlockId, holderId, order);

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("AddedAndLooseHoldersPartialView", GetBlockManagementModel(textBlockId));
            }
            else
            {
                return RedirectToAction("EditBlock", new { blockId = textBlockId });
            }
               
        }
       

        public List<BlockHolder> GetCorrespondingBlockHolders(int blockId)
        {
            List<BlockHolder> all_blockHolders = _dbHelper.GetAllBlockHolders().ToList();
            List<Holder_Block> all_holder_block = _dbHelper.GetAllHolder_Blocks().ToList();

            return                                        (from bh in all_blockHolders
                                                           from h_b in all_holder_block
                                                           where h_b.TextBlockId == blockId && h_b.BlockHolderId == bh.Id
                                                           select bh).ToList();
        }

        public List<Holder_Block> GetCorrespondingHolder_Blocks(int blockId)
        {
            List<BlockHolder> all_blockHolders = _dbHelper.GetAllBlockHolders().ToList();
            List<Holder_Block> all_holder_block = _dbHelper.GetAllHolder_Blocks().ToList();

            return (from bh in all_blockHolders
                    from h_b in all_holder_block
                    where h_b.TextBlockId == blockId && h_b.BlockHolderId == bh.Id
                    select h_b).ToList();
        }

        [HttpPost]
        public IActionResult SaveBlock(TextBlock model)
        {
            
            TextBlock changedData = new TextBlock() { Id = model.Id, Name = model.Name, Content = model.Content };
            TextBlock tbFromDb= _dbHelper.GetAllTextBlocks().Where(tb => tb.Id == model.Id).Single();

            tbFromDb.Name = changedData.Name;
            tbFromDb.Content = changedData.Content;
            _dbHelper.SaveChanges();

            // It is necessary to update other properties of the view or the partial view will not be served
            // because hidden fields does not store complex types

            // model.HolderManagement.CorrespondingBlockHolders = GetCorrespondingBlockHolders(model.TextBlock.Id);
            // model.HolderManagement.AllBlockHolders = Configuration.DbHelper.GetAllBlockHolders().ToList();


            // return PartialView("EditTextBlockPartialView",model);

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
                return Content(model.Name);
            else return RedirectToAction("EditBlock", new { blockId = model.Id });
        }

        [HttpGet]
        public IActionResult AddHolderToBlock(int holderId, int blockId)
        {
            _dbHelper.AddHolderToBlock(holderId, blockId);
            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {                
                return PartialView("AddedAndLooseHoldersPartialView", GetBlockManagementModel(blockId));
            }
            else
            {
                return RedirectToAction("EditBlock", new { blockId = blockId });
            }
                
        }

        public AddedAndLooseHoldersViewModel GetBlockManagementModel(int blockId)
        {
            AddedAndLooseHoldersViewModel model = new AddedAndLooseHoldersViewModel();
            model.CorrespondingBlockHolders = GetCorrespondingBlockHolders(blockId);
            model.AllBlockHolders = _dbHelper.GetAllBlockHolders().ToList();
            model.CurrentTextBoxId = blockId;
            return model;
        }

        [HttpGet]
        public IActionResult RemoveHolderFromBlock(int holderId,int blockId)
        {
            _dbHelper.RemoveHolderFromBlock(holderId, blockId);

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                 AddedAndLooseHoldersViewModel model = new AddedAndLooseHoldersViewModel();
                 model.CorrespondingBlockHolders = GetCorrespondingBlockHolders(blockId);
                 model.AllBlockHolders = _dbHelper.GetAllBlockHolders().ToList();
                 model.CurrentTextBoxId = blockId;
                 return PartialView("AddedAndLooseHoldersPartialView",model);
            }

            else {
return RedirectToAction("EditBlock", new { blockId = blockId });
            }
            
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

    public class NewTextBlock
    {
        [Display(Name ="Název textového bloku")]
        [Required(ErrorMessage ="Uveďte prosím název textového bloku.")]
        public string Name { get; set; }

        [Display(Name="Rychlý obsah bloku")]
        public string Content { get; set; }
        
        public int BlockHolderId { get; set; }
    }

    public class EditTextBlock
    {
        
        public int Id{ get; set; }

        [Display(Name = "Název textového bloku")]
        [Required(ErrorMessage = "Uveďte prosím název textového bloku.")]
        public string Name { get; set; }

        [Display(Name = "Obsah bloku")]
        public string Content { get; set; }

      
    }

}