using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Areas.Admin.ViewModels;
using JasperSite.Models;
using Microsoft.AspNetCore.Mvc;
using JasperSite.Models.Database;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

namespace JasperSite.Areas.Admin.Controllers
{  [Authorize]
    [Area("Admin")]
    public class BlocksController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {           
            return View(UpdatePage());
        }

        private readonly DatabaseContext _databaseContext;
        private readonly DbHelper _dbHelper;
        private readonly IStringLocalizer _localizer;
       

        public BlocksController(DatabaseContext dbContext, IStringLocalizer<BlocksController> localizer)
        {
            this._databaseContext = dbContext;
            this._dbHelper = new DbHelper(dbContext);
            this._localizer = localizer;
        }

        public BlockViewModel UpdatePage()
        {
            try
            {
                var all_Themes = _dbHelper.GetAllThemes();
                var all_Holder_Block = _dbHelper.GetAllHolder_Blocks();
                var all_BlockHolder = _dbHelper.GetAllBlockHolders();
                var all_TextBlocks = _dbHelper.GetAllTextBlocks();

                BlockViewModel model = new BlockViewModel();

               int currentThemeId = _dbHelper.GetCurrentThemeIdFromDb();

                foreach (TextBlock block in all_TextBlocks)
                {
                    List<BlockHolderWithTheme> blockHolders = (from theme in all_Themes
                                                               from holder_block in all_Holder_Block
                                                               from blockHolder in all_BlockHolder
                                                               where block.Id == holder_block.TextBlockId && holder_block.BlockHolderId == blockHolder.Id && theme.Id == blockHolder.ThemeId && theme.Id == currentThemeId // last condition on this row limits the records only to currently activated theme
                                                               select new BlockHolderWithTheme { BlockHolderName = blockHolder.Name, ThemeName = theme.Name, Order = holder_block.Order }).ToList();


                    BlocksViewModelData bvmd = new BlocksViewModelData() { TextBlock = block, BlocHolderWithTheme = blockHolders };
                    model.Blocks.Add(bvmd);
                }

                model.AllBlockHolders = _dbHelper.GetAllBlockHolders();
                model.AllThemes = _dbHelper.GetAllThemes();
                model.CurrentThemeId = _dbHelper.GetCurrentThemeIdFromDb();
                return model;
            }
            catch
            {
                BlockViewModel emptyModel = new BlockViewModel();
                emptyModel.AllBlockHolders = null;
                emptyModel.AllThemes = null;
                emptyModel.Blocks = null;
                emptyModel.NewTextBlock = null;
                emptyModel.CurrentThemeId = -1; // theme id not found
                return emptyModel;
        }

    }

        [HttpPost]
        public IActionResult AddNewBlock(BlockViewModel model)
        {
            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            try
            {               
                if (ModelState.IsValid)
                {
                    string newBlockName = model.NewTextBlock.Name;
                    int newBlock_BlockHolderId = model.NewTextBlock.BlockHolderId;
                    string newBlockContent = model.NewTextBlock.Content;

                    TextBlock tb = new TextBlock() { Content = newBlockContent, Name = newBlockName };
                    TextBlock addedBlock = _dbHelper.AddNewBlock(tb);

                    Holder_Block hb = new Holder_Block();
                    hb.BlockHolderId = newBlock_BlockHolderId;
                    hb.TextBlockId = addedBlock.Id;
                    _dbHelper.AddHolder_Block(hb);
                    TempData["Success"] = true;
                }
                else
                {
                    throw new Exception();
                }                
            }
            catch
            {
                TempData["ErrorMessage"] = _localizer["An error occured during the creation of the text block."];
            }

            if(isAjaxCall)
            {
                ModelState.Clear();
                return PartialView("BlockFormPartialView", UpdatePage());
            }
            else
            {
                return RedirectToAction("Index");
            }           
        }

        [HttpGet]
        public IActionResult DeleteBlock(int id)
        {
            try
            {
                _dbHelper.DeleteTextBlockById(id);
            }
            catch(Exception ex)
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = ex.Message;
            }
          

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("BlockFormPartialView",UpdatePage());
            }
            else
            {
                return View("Index",UpdatePage());
            }

                
        }

        [HttpGet]
        public IActionResult DeleteBlockHolder(int id)
        {
            try
            {
                _dbHelper.DeleteBlockHolderById(id);
            }
            catch
            {
                // TODO: error
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
         public IActionResult EditBlock(int blockId)
        {
            try
            {
                TextBlock tbToEdit = _dbHelper.GetAllTextBlocks().Where(tb => tb.Id == blockId).Single();

                List<BlockHolder> correspondingBlockHolders = GetCorrespondingBlockHolders(blockId);

                List<BlockHolder> allBlockHolders = _dbHelper.GetAllBlockHolders().ToList();

                EditBlockViewModel model = new EditBlockViewModel();

                model.TextBlock = new EditTextBlock() { Name = tbToEdit.Name, Content = tbToEdit.Content, Id = tbToEdit.Id };

                model.HolderManagement = GetBlockManagementModel(blockId);
                return View(model);
            }
            catch 
            {
                // TODO: error
                return RedirectToAction("Index");
            }

        
        }

        [HttpPost]
        public IActionResult SaveTextBlockOrder(int textBlockId, int holderId, int order)
        {
            try
            {
                _dbHelper.SaveTextBlockOrderNumberInHolder(textBlockId, holderId, order);
                TempData["Success"] = true;
            }
            catch
            {
                TempData["ErrorMessage"] = _localizer["The order of the text block could not be saved."];
            }
          

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
            try
            {
                List<BlockHolder> all_blockHolders = _dbHelper.GetAllBlockHolders().ToList();
                List<Holder_Block> all_holder_block = _dbHelper.GetAllHolder_Blocks().ToList();

                return (from bh in all_blockHolders
                        from h_b in all_holder_block
                        where h_b.TextBlockId == blockId && h_b.BlockHolderId == bh.Id
                        select bh).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<Holder_Block> GetCorrespondingHolder_Blocks(int blockId)
        {
            try
            {
                List<BlockHolder> all_blockHolders = _dbHelper.GetAllBlockHolders().ToList();
                List<Holder_Block> all_holder_block = _dbHelper.GetAllHolder_Blocks().ToList();

                return (from bh in all_blockHolders
                        from h_b in all_holder_block
                        where h_b.TextBlockId == blockId && h_b.BlockHolderId == bh.Id
                        select h_b).ToList();
            }
            catch 
            {
                return null;
             
            }
        }

        // No partial view result = too slow and laggy for tinyMCE
        [HttpPost]
        public IActionResult SaveBlock(TextBlock model)
        {
            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            try
            {

                if (ModelState.IsValid)
                {
                    TextBlock changedData = new TextBlock() { Id = model.Id, Name = model.Name, Content = model.Content };
                    TextBlock tbFromDb = _dbHelper.GetAllTextBlocks().Where(tb => tb.Id == model.Id).Single();

                    tbFromDb.Name = changedData.Name;
                    tbFromDb.Content = changedData.Content;
                    _databaseContext.SaveChanges();                  
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                Response.StatusCode = 500; // ajax failure function will be executed 
            }
            

            if (isAjaxCall)
            {
                return Content(model.Name);
            }
            else
            {
                return RedirectToAction("EditBlock", new { blockId = model.Id });
            }
        }

        [HttpGet]
        public IActionResult AddHolderToBlock(int holderId, int blockId)
        {
            try
            {
                _dbHelper.AddHolderToBlock(holderId, blockId);
                TempData["Success"] = true;
            }
            catch
            {
                TempData["ErrorMessage"] = _localizer["The container could not be assigned to the text block."];
            }
          

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
            try
            {
                AddedAndLooseHoldersViewModel model = new AddedAndLooseHoldersViewModel();
                var correspondingBlockHolders = GetCorrespondingBlockHolders(blockId);
                var allBlockHolders = _dbHelper.GetAllBlockHolders().ToList();
                model.CurrentTextBoxId = blockId;

                // Display items only relevant to currently acctivated theme
                string currentThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
                var filteredCorrespondingBlockHolders = correspondingBlockHolders.Where(bh => bh.Theme.Name == currentThemeName).ToList();
                var filteredAllBlockHolders = allBlockHolders.Where(bh => bh.Theme.Name == currentThemeName).ToList();

                model.CorrespondingBlockHolders = filteredCorrespondingBlockHolders;
                model.AllBlockHolders = filteredAllBlockHolders;

                return model;
            }
            catch 
            {

                return null;
            }
        }

        [HttpGet]
        public IActionResult RemoveHolderFromBlock(int holderId,int blockId)
        {
            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            try
            {
                _dbHelper.RemoveHolderFromBlock(holderId, blockId);                
                TempData["Success"] = true;
            }
            catch
            {
                TempData["ErrorMessage"] = _localizer["The container could not be removed."];
            }


            if (isAjaxCall)
            {
                return PartialView("AddedAndLooseHoldersPartialView", GetBlockManagementModel(blockId));
            }

            else
            {
                return RedirectToAction("EditBlock", new { blockId = blockId });
            }

        }


        [HttpGet]
        public IActionResult UpdateAllThemesData()
        {
            try
            {
                IRequestCultureFeature culture = Request.HttpContext.Features.Get<IRequestCultureFeature>();
                Configuration.ThemeHelper.UpdateAllThemeRelatedData(_databaseContext,culture);
                TempData["Success"] = true;

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = _localizer["The changes could not be completed."];
            }

            return RedirectToAction("Index");
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