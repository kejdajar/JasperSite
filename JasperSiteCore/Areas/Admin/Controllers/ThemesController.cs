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
            try
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
            catch
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = "Při daném požadavku nastala chyba.";
                return PartialView("ThemesPartialView", UpdatePage());
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
            }
            catch
            {
                // TODO: Error
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
            }
            catch
            {
                // TODO: ERROR
            }
            return View("Index", UpdatePage());
        }


        public ThemesViewModel UpdatePage()
        {
            try
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
            catch(ThemeNotExistsException ex) // specified theme of jasper.json property "themeName" does not exist
            {
                ThemesViewModel model = new ThemesViewModel();
                model.ItemsPerPage = 3;
                model.ManuallyDeletedThemeNames = null;
                model.NotRegisteredThemeNames = null;
                model.PageNumber = 1;
                model.SelectedThemeName = "Globální soubor jasper.json uvádí jako vzhled: " + ex.MissingThemeName + " který ale ve složce "+ Configuration.GlobalWebsiteConfig.ThemeFolder+" neexistuje";
                model.ThemeFolder = Configuration.GlobalWebsiteConfig.ThemeFolder;
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
            }
            catch(Exception ex)
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = ex.Message;
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
        public ActionResult ActivateTheme(string themeName)
        {
            try
            {
                string themeNameBeforeChanged = Configuration.GlobalWebsiteConfig.ThemeName;


                // Property ThemeName writes data automatically to the .json file
                JasperSiteCore.Models.Configuration.GlobalWebsiteConfig.ThemeName = themeName;

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

            }
            catch
            {
                // TODO : Error
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
        public ActionResult UpdateConfigurationWithThemeReset()
        {
            try
            {
                JasperSiteCore.Models.Configuration.Initialize();
              //  Configuration.GlobalWebsiteConfig.ThemeName = "Default";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "1";
                ViewBag.ErrorMessage = ex.Message;
                return View("Index", UpdatePage());
            }

            return RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult UpdateConfiguration()
        {
            try
            {
                JasperSiteCore.Models.Configuration.Initialize();
            }
            catch(Exception ex)
            {
                ViewBag.Error = "1";
                ViewBag.ErrorMessage = ex.Message;
                return View("Index", UpdatePage());
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult UploadTheme(ICollection<IFormFile> files)
        {
            try
            {
                string themeFolderPath = System.IO.Path.Combine("./", Configuration.GlobalWebsiteConfig.ThemeFolder).Replace('\\', '/');


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

                                string rootFolderName = archive.Entries[0].FullName.Replace("/", string.Empty);
                                
                                    List<string> allThemesNames = dbHelper.GetAllThemes().Select(t => t.Name).ToList();
                                    foreach (string themeName in allThemesNames)
                                    {
                                        if (themeName.ToLower().Trim() == rootFolderName.ToLower().Trim())
                                        {
                                            ThemeAlreadyExistsException exception = new ThemeAlreadyExistsException() { DuplicateTheme = rootFolderName };
                                            throw exception;
                                        }
                                    }
                                  
                                

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
                    }
                    catch(ThemeAlreadyExistsException)
                    {
                        // rethrow
                        throw new ThemeAlreadyExistsException();
                    }
                }
            }
            catch(ThemeAlreadyExistsException ex)
            {
                //Todo: error

                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = $"Při pokusu nahrát vzhled došlo k chybě. Vzhled {ex.DuplicateTheme} již existuje. Při nahrávání více vzhledů nebyly další vzhledy přidány." ;
            }

            return View("Index", UpdatePage());

        }

        [HttpGet]
        public IActionResult ReconstructAllThemesCorrespondingDatabaseTables()
        {
            try
            {
                dbHelper.Reconstruct_Theme_TextBlock_BlockHolder_HolderBlockDatabase();
            }
            catch
            {
                // TODO: Error
            }

            return RedirectToAction("Index");
        }

    }

}