using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Areas.Admin.ViewModels;
using JasperSite.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JasperSite.Areas.Admin.Controllers
{   [Authorize]
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {              
          return View(UpdateCategoryPage());                       
        }

        private readonly DatabaseContext _databaseContext;
        private readonly DbHelper _dbHelper;

        public CategoriesController(DatabaseContext dbContext)
        {
            this._databaseContext = dbContext;
            this._dbHelper = new DbHelper(dbContext);
        }
       
        public CategoriesViewModel UpdateCategoryPage()
        {
            CategoriesViewModel model = new CategoriesViewModel();
            try
            {
                model.Categories = _dbHelper.GetAllCategories();
                if(model.Categories.Count <= 0)
                {
                    model.Categories = null;
                    return model;
                }
                else
                {
                    return model;
                }
              
            }
            catch
            {
                model.Categories = null;
                return model;
            }

        }

        [HttpPost]        
        public IActionResult CreateNewCategory(CategoriesViewModel model)
        {            

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            try
            {
                if (ModelState.IsValid) // Server check in case JS is disabled
                {

                    //if (model.NewCategoryName != "Uncategorized")
                    //{
                    //    _dbHelper.AddCategory(model.NewCategoryName);
                    //    TempData["Success"] = true;
                    //}
                    //else
                    //{
                    //    TempData["ErrorMessage"] = "Rubriku s tímto názvem není možné vytvořit.";
                    //}

                    // AddCategory checks for 'Uncategorized' category
                    _dbHelper.AddCategory(model.NewCategoryName);
                     TempData["Success"] = true;

                }
                else
                {
                    // Categories are not passed back from view, so they need to be filled into model again
                    // model.Categories = _dbHelper.GetAllCategories();
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message + " " + ((ex.InnerException != null) ? ex.InnerException.Message : string.Empty);
            }

            if (isAjaxCall)
            {
                ModelState.Clear();
                return PartialView("AddNewCategoryPartialView", UpdateCategoryPage());
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public IActionResult DeleteCategory(int? id)
        {
            
            if(id == null || id < 0)
            {
               
                TempData["ErrorMessage"] = "Daná rubrika pro smazání nebyla nalezena.";
                return RedirectToAction("Index");
            }

            try
            {                             
                _dbHelper.DeleteCategory((int)id);
                TempData["Success"] = true;
                
            }
            catch (Exception ex)
            {
               
                TempData["ErrorMessage"] = ex.Message;              
            }           

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                ModelState.Clear();
                return PartialView("AddNewCategoryPartialView", UpdateCategoryPage());
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            EditCategoryViewModel model = new EditCategoryViewModel();

            try
            {
                model.CategoryId = id;
                model.CategoryName = _dbHelper.GetCategoryNameById(id);
            }
            catch
            {
                model = null;
            }
            
            return View(model);
        }

        [HttpPost]
        public IActionResult EditCategory(EditCategoryViewModel model)
        {
            try
            {                
                _dbHelper.RenameCategory(model.CategoryId, model.CategoryName);
                TempData["Success"] = true;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message + " "+((ex.InnerException != null) ? ex.InnerException.Message : string.Empty);
            }

            return View(model);
        }
    }
}