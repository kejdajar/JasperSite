using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using JasperSiteCore.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using JasperSiteCore.Helpers;
using System.IO;
using System.Net.Http.Headers;
using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class HomeController : Controller
    {  
        // Firstly, the installation must have already been completed before accesing administration panel
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {          
            if (!Configuration.InstallationCompleted())
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                { "Controller", "Install" },
                { "Action", "Index" },
                        {"Area","Admin" }
                    });
            }

            base.OnActionExecuting(filterContext);
        }

        private readonly DatabaseContext databaseContext;
        private readonly DbHelper dbHelper;

        public HomeController(DatabaseContext dbContext)
        {
            this.databaseContext = dbContext;
            this.dbHelper = new DbHelper(dbContext);
        }

        // GET: Admin/Home
        public ActionResult Index()
        {          
            return View();
        }

       

        public ActionResult Articles()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Themes()
        {
            //ModelState.Clear();
            
            return View(UpdatePage());
        }

        [HttpPost]
        public ActionResult Themes(ThemesViewModel model,IFormCollection collection)
        {
            int itemsPerPage = 3;
            int currentPage = model.PageNumber;

            string next = collection["next"];
            string prev = collection["prev"];

            if (next != null)
                currentPage++;
            if (prev != null)
                currentPage--;

            List<ThemeInfo> themeInfoList = Configuration.ThemeHelper.GetInstalledThemesInfo();
            themeInfoList.OrderBy(o => o.ThemeName);
            ThemeInfo currentTheme = themeInfoList.Where(i => i.ThemeName == Configuration.GlobalWebsiteConfig.ThemeName).First();
            themeInfoList.Remove(currentTheme);
            themeInfoList.Insert(0, currentTheme);


            JasperPaging<ThemeInfo> paging = new JasperPaging<ThemeInfo>(themeInfoList, currentPage, itemsPerPage);

            ThemesViewModel model2 = new ThemesViewModel();
            model2.SelectedThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
            model2.ThemeFolder = Configuration.GlobalWebsiteConfig.ThemeFolder;
            model2.PageNumber = paging.CurrentPageNumber;
            model2.ItemsPerPage = paging.ItemsPerPage;
            model2.TotalNumberOfPages = paging.NumberOfPagesNeeded;

            model2.ThemeInfoList = paging.GetCurrentPageItems();

            ModelState.Clear();
            return PartialView("ThemesPartialView", model2);

        }

        /// <summary>
        /// If the user has modified Theme folder directly on the server, new themes will be added to the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public IActionResult AddNewAddedThemesToDb()
        {
            dbHelper.AddThemesFromFolderToDatabase(dbHelper.CheckThemeFolderAndDatabaseIntegrity());
            return RedirectToAction("Themes");
        }

        [HttpGet]
        public IActionResult RemoveManuallyDeletedThemesFromDb()
        {
           List<string> themesToDelete= dbHelper.FindManuallyDeletedThemes();
            foreach(string name in themesToDelete)
            {
                dbHelper.DeleteThemeByName(name);
            }
            return View("Themes", UpdatePage());
        }


        public ThemesViewModel UpdatePage()
        {
            int itemsPerPage = 3;
            int currentPage = 1;

            List<ThemeInfo> themeInfoList = Configuration.ThemeHelper.GetInstalledThemesInfoByNameAndActive();


            JasperPaging<ThemeInfo> paging = new JasperPaging<ThemeInfo>(themeInfoList, currentPage, itemsPerPage);

            ThemesViewModel model = new ThemesViewModel();
            model.SelectedThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
            model.ThemeFolder = Configuration.GlobalWebsiteConfig.ThemeFolder;
            model.PageNumber = paging.CurrentPageNumber;
            model.ItemsPerPage = paging.ItemsPerPage;
            model.TotalNumberOfPages = paging.NumberOfPagesNeeded;

            model.ThemeInfoList = paging.GetCurrentPageItems();

            // Not registered themes check
            model.NotRegisteredThemeNames = dbHelper.CheckThemeFolderAndDatabaseIntegrity();
            model.ManuallyDeletedThemeNames = dbHelper.FindManuallyDeletedThemes();
            return model;
        }

        [HttpGet]
        public ActionResult DeleteTheme(string themeName)
        {
            bool success = Configuration.ThemeHelper.DeleteThemeByNameFromDbAndFolder(themeName,databaseContext);

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("ThemesPartialView", UpdatePage());
            }
            else
            {
                return RedirectToAction("Themes");
            }

            
        }

        [HttpGet]
        public ActionResult ActivateTheme(string themeName)
        {
            string themeNameBeforeChanged = Configuration.GlobalWebsiteConfig.ThemeName;
       

            // Property ThemeName writes data automatically to the .json file
            JasperSiteCore.Models.Configuration.GlobalWebsiteConfig.ThemeName = themeName;

                UpdateConfiguration(); // All cached settings will be reset
                                   

            

            // Find assigned blocks for the prior theme
            Theme priorTheme = dbHelper._db.Themes.Where(t => t.Name == themeNameBeforeChanged).Single();
            Theme newTheme = dbHelper._db.Themes.Where(t => t.Name == themeName).Single();

            var oldRelationships = (from old_holder in dbHelper.GetAllBlockHolders()
                                                  from old_joinTable in dbHelper.GetAllHolder_Blocks()
                                                  where old_joinTable.BlockHolderId == old_holder.Id && old_holder.ThemeId==priorTheme.Id
                                                  select new { JoinTableId=old_joinTable.Id, HolderName=old_holder.Name }).ToList();

            List<string> newHolders = Configuration.WebsiteConfig.BlockHolders;
            foreach(string holderNameNewTheme in newHolders)
            {
              var collectionOfCorrespondingObjects= oldRelationships.Where(r => r.HolderName == holderNameNewTheme).ToList();
              foreach(var item in collectionOfCorrespondingObjects)
                {
                   var holdersToEdit= dbHelper.GetAllHolder_Blocks().Where(h => h.Id == item.JoinTableId);
                    foreach(var editedHolder in holdersToEdit)
                    {
                        var join = (from allThemes in dbHelper.GetAllThemes()
                                    from allBlockHolders in dbHelper.GetAllBlockHolders()
                                    where allThemes.Id == newTheme.Id && allBlockHolders.Name == holderNameNewTheme && allBlockHolders.ThemeId==newTheme.Id
                                    select allBlockHolders).Single();
                        
                        editedHolder.BlockHolderId = join.Id;
                        dbHelper._db.SaveChanges();
                    }
                }
            }

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("ThemesPartialView", UpdatePage());
            }
            else
            {
                return RedirectToAction("Themes");
            }

                
            
            

        }

        [HttpGet]
        public ActionResult UpdateConfiguration()
        {
            
                JasperSiteCore.Models.Configuration.Initialize();
                return RedirectToAction("Themes");  

        }

        [HttpPost]
        public IActionResult UploadTheme(ICollection<IFormFile> files)
        {
            string themeFolderPath = System.IO.Path.Combine("./",Configuration.GlobalWebsiteConfig.ThemeFolder).Replace('\\', '/');

            foreach (IFormFile file in files)
                {



                // ZipArchive sometimes throws errors even if the process was successfull ...
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.OpenReadStream().CopyTo(memoryStream);

                        using (ZipArchive archive = new ZipArchive(memoryStream))
                        {

                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (!string.IsNullOrEmpty(Path.GetExtension(entry.FullName))) //make sure it's not a folder
                                {
                                    entry.ExtractToFile(Path.Combine(themeFolderPath, entry.FullName));
                                }
                                else
                                {
                                    Directory.CreateDirectory(Path.Combine(themeFolderPath, entry.FullName));
                                }
                            }
                        }
                    }
                } catch { }





                    //using (var reader = new StreamReader(file.OpenReadStream()))
                    //    {
                    //        var fileContent = reader.ReadToEnd();
                    //        string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    //        System.IO.File.WriteAllText(System.IO.Path.Combine(Environment.CurrentDirectory, fileName), fileContent);
                    //    }

                    }
                
           
            
              return RedirectToAction("Themes");
           
        }

        [HttpGet]
        public ActionResult Categories()
        {            
            return View(UpdateCategoryPage());
        }

        public CategoriesViewModel UpdateCategoryPage()
        {
            CategoriesViewModel model = new CategoriesViewModel();
            model.Categories =dbHelper.GetAllCategories();
            return model;
        }

        [HttpPost]
        public IActionResult CreateNewCategory(string btnCategoryName)
        {
            dbHelper.AddNewCategory(btnCategoryName);

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest"; 
            if(isAjaxCall)
            {

                return PartialView("AddNewCategoryPartialView", UpdateCategoryPage());
            }
            else
            {
                return RedirectToAction("Categories");
            }
            
        }

        [HttpGet]
        public IActionResult DeleteCategory(CategoriesViewModel model,int id)
        {
            dbHelper.DeleteCategory(id);
            CategoriesViewModel viewModel = new CategoriesViewModel();
            viewModel.Categories = dbHelper.GetAllCategories();
            ModelState.Clear();


            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("AddNewCategoryPartialView", viewModel);
            }
            else
            {
                return View("Categories", viewModel);
            }
                
        }


        [HttpGet]
        public IActionResult ReconstructAllThemesCorrespondingDatabaseTables()
        {
            dbHelper.Reconstruct_Theme_TextBlock_BlockHolder_HolderBlockDatabase();
            return RedirectToAction("Themes");
        }

       

    }
}