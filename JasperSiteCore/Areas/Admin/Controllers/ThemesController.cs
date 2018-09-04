using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Areas.Admin.Models;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Helpers;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ThemesController : Controller
    {

        private readonly DatabaseContext databaseContext;
        private readonly DbHelper dbHelper;

        public ThemesController(DatabaseContext dbContext)
        {
            this.databaseContext = dbContext;
            this.dbHelper = new DbHelper(dbContext);
        }

        [HttpGet]
        public ActionResult Index()
        {            
            return View(UpdatePage());
        }

        [HttpPost]
        public ActionResult Index(ThemesViewModel model, IFormCollection collection)
        {

            ThemesViewModel model2 = new ThemesViewModel();

            try
            {
                int itemsPerPage = 6;
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

                
                model2.SelectedThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
                model2.ThemeFolder = Configuration.ThemeFolder;
                model2.PageNumber = paging.CurrentPageNumber;
                model2.ItemsPerPage = paging.ItemsPerPage;
                model2.TotalNumberOfPages = paging.NumberOfPagesNeeded;
                model2.ThemeInfoList = paging.GetCurrentPageItems();               
               
            }
            catch
            {
                TempData["ErrorMessage"] = "Při provádění pořadavku došlo k chybě";
            }

            bool isAjaxRequest = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxRequest)
            {
                ModelState.Clear();
                return PartialView("ThemesPartialView", model2);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        /// <summary>
        /// If the user has modified Theme folder directly on the server, new themes will be added to the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddNewAddedThemesToDb()
        {
            try
            {
                dbHelper.AddThemesFromFolderToDatabase(dbHelper.CheckThemeFolderAndDatabaseIntegrity());
                TempData["Success"] = true;
            }
            catch
            {
                TempData["ErrorMessage"] = "Nebylo možné přidat nové vzhledy do databáze";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult RemoveManuallyDeletedThemesFromDb()
        {
            try
            {
                List<string> themesToDelete = dbHelper.FindManuallyDeletedThemes();
                foreach (string name in themesToDelete)
                {
                    dbHelper.DeleteThemeByName(name);
                }
                TempData["Success"] = true;
            }
            catch
            {
                TempData["ErrorMessage"] = "Nebylo možné odstranit z databáze již fyzicky neexistující vzhledy";
            }
            return View("Index", UpdatePage());
        }


        public ThemesViewModel UpdatePage()
        {
            try
            {
                int itemsPerPage = 6;
                int currentPage = 1;

                List<ThemeInfo> themeInfoList = Configuration.ThemeHelper.GetInstalledThemesInfoByNameAndActive();

                JasperPaging<ThemeInfo> paging = new JasperPaging<ThemeInfo>(themeInfoList, currentPage, itemsPerPage);

                ThemesViewModel model = new ThemesViewModel();
                model.SelectedThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
                model.ThemeFolder = Configuration.ThemeFolder;
                model.PageNumber = paging.CurrentPageNumber;
                model.ItemsPerPage = paging.ItemsPerPage;
                model.TotalNumberOfPages = paging.NumberOfPagesNeeded;

                model.ThemeInfoList = paging.GetCurrentPageItems();

                // Not registered themes check
                model.NotRegisteredThemeNames = dbHelper.CheckThemeFolderAndDatabaseIntegrity();
                model.ManuallyDeletedThemeNames = dbHelper.FindManuallyDeletedThemes();

                return model;
            }
            catch(ThemeNotExistsException ex) // specified theme of jasper.json property "themeName" does not exist
            {
                ThemesViewModel model = new ThemesViewModel();
                model.ItemsPerPage = 3;
                model.ManuallyDeletedThemeNames = null;
                model.NotRegisteredThemeNames = null;
                model.PageNumber = 1;
                model.SelectedThemeName = "Globální soubor jasper.json uvádí jako vzhled: " + ex.MissingThemeName + " který ale ve složce "+ Configuration.ThemeFolder+" neexistuje";
                model.ThemeFolder = Configuration.ThemeFolder;
                model.ThemeInfoList = null;
                model.TotalNumberOfPages = default(int);
                return model;
            }
            catch
            {
                ThemesViewModel model = new ThemesViewModel();
                model.ItemsPerPage = 3;
                model.ManuallyDeletedThemeNames = null;
                model.NotRegisteredThemeNames = null;
                model.PageNumber = 1;
                model.SelectedThemeName = null;
                model.ThemeFolder = null;
                model.ThemeInfoList = null;
                model.TotalNumberOfPages = default(int);
                return model;
            }

        }

        [HttpGet]
        public ActionResult DeleteTheme(string themeName)
        {
            try
            {
                Configuration.ThemeHelper.DeleteThemeByNameFromDbAndFolder(themeName, databaseContext);              
                TempData["Success"] = true;
            }
            catch(Exception)
            {
                TempData["ErrorMessage"] = "Vzhled nebylo možné odstranit";
            }


            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxCall)
            {
                return PartialView("ThemesPartialView", UpdatePage());
            }
            else
            {
                return View("Index",UpdatePage());
            }


        }

        [HttpGet]
        public IActionResult UpdateAllThemesData()
        {
            try
            {
                Configuration.ThemeHelper.UpdateAllThemeRelatedData(databaseContext);
                TempData["Success"] = true;
              
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Změny nebylo možné provést";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ActivateTheme(string themeName)
        {
            try
            {
                string themeNameBeforeChanged = Configuration.GlobalWebsiteConfig.ThemeName;

                // Property ThemeName writes data automatically to the .json file
                Configuration.GlobalWebsiteConfig.ThemeName = themeName;

                UpdateConfiguration(); // All cached settings will be reset
                
                // Find assigned blocks for the prior theme
                Theme priorTheme = dbHelper.Database.Themes.Where(t => t.Name == themeNameBeforeChanged).Single();
                Theme newTheme = dbHelper.Database.Themes.Where(t => t.Name == themeName).Single();

                var oldRelationships = (from old_holder in dbHelper.GetAllBlockHolders()
                                        from old_joinTable in dbHelper.GetAllHolder_Blocks()
                                        where old_joinTable.BlockHolderId == old_holder.Id && old_holder.ThemeId == priorTheme.Id
                                        select new { JoinTableId = old_joinTable.Id, HolderName = old_holder.Name }).ToList();

                List<string> newHolders = Configuration.WebsiteConfig.BlockHolders;
                foreach (string holderNameNewTheme in newHolders)
                {
                    var collectionOfCorrespondingObjects = oldRelationships.Where(r => r.HolderName == holderNameNewTheme).ToList();
                    foreach (var item in collectionOfCorrespondingObjects)
                    {
                        var holdersToEdit = dbHelper.GetAllHolder_Blocks().Where(h => h.Id == item.JoinTableId);
                        foreach (var editedHolder in holdersToEdit)
                        {
                            var join = (from allThemes in dbHelper.GetAllThemes()
                                        from allBlockHolders in dbHelper.GetAllBlockHolders()
                                        where allThemes.Id == newTheme.Id && allBlockHolders.Name == holderNameNewTheme && allBlockHolders.ThemeId == newTheme.Id
                                        select allBlockHolders).Single();

                            editedHolder.BlockHolderId = join.Id;
                            dbHelper.Database.SaveChanges();
                        }
                    }
                }
                TempData["Success"] = true;
            }
            catch
            {
                TempData["ErrorMessage"] = "Vzhled nebylo možné aktivovat";
            }

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (isAjaxCall)
            {
                return PartialView("ThemesPartialView", UpdatePage());
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
                

        [HttpGet]
        public ActionResult UpdateConfiguration()
        {
            try
            {
                Configuration.Initialize();
                TempData["Success"] = true;
            }
            catch(Exception)
            {
                TempData["ErrorMessage"] = "Nebylo možné aktualizovat nastavení systému";                
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult UploadTheme(ICollection<IFormFile> files)
        {
            try
            {
                string themeFolderPath = System.IO.Path.Combine("./", Configuration.ThemeFolder).Replace('\\', '/');
                bool atLeastOneThemeAlreadyExists = false;

                foreach (IFormFile file in files)
                {
               


                    // ZipArchive sometimes throws errors even if the process was successfull ...
                    
                        using (var memoryStream = new MemoryStream())
                        {
                            file.OpenReadStream().CopyTo(memoryStream);

                            using (ZipArchive archive = new ZipArchive(memoryStream))
                            {
                                // Begin check for existing theme
                                string rootFolderName = archive.Entries[0].FullName.Replace("/", string.Empty);
                                
                                    List<string> allThemesNames = dbHelper.GetAllThemes().Select(t => t.Name).ToList();
                                    foreach (string themeName in allThemesNames)
                                    {
                                        if (themeName.ToLower().Trim() == rootFolderName.ToLower().Trim())
                                        {
                                        atLeastOneThemeAlreadyExists = true;
                                       
                                        }
                                    }

                            if (atLeastOneThemeAlreadyExists) continue;

                                  // End check for existing theme
                                
                                archive.ExtractToDirectory(themeFolderPath);                                
                            }
                        }                    
                }

                if (atLeastOneThemeAlreadyExists)
                {
                    ThemeAlreadyExistsException exception = new ThemeAlreadyExistsException() { };
                    throw exception;
                }

                TempData["Success"] = true;
            }         
            catch(ThemeAlreadyExistsException)
            {              
                TempData["ErrorMessage"] = $"Při pokusu nahrát vzhled(y) došlo k chybě. Alespoň jeden přidávaný vzhled již existuje a byl proto přeskočen." ;
            }
            catch (Exception)
            {
              
                TempData["ErrorMessage"] = "Při nahrávání došlo k neočekávané chybě.";
            }
            finally
            {
                dbHelper.AddThemesFromFolderToDatabase(dbHelper.CheckThemeFolderAndDatabaseIntegrity()); // commit physicall changes in Theme folder to DB
            }
            
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult ReconstructAllThemesCorrespondingDatabaseTables()
        {
            try
            {
                dbHelper.ReconstructAndClearThemeData();
                TempData["Success"] = true;
            }
            catch
            {
                TempData["ErrorMessage"] = "Nepodařilo se synchronizovat a rekonstruovat databázi vzhledů";
            }

            return RedirectToAction("Index");
        }

    }

}