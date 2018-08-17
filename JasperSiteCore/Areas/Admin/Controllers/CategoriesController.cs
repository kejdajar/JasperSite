using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JasperSiteCore.Areas.Admin.Controllers
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
        // If JS is enabled - data will be passed in ajaxData, otherwise in the model
        public IActionResult CreateNewCategory(CategoriesViewModel model,string ajaxData)
        {            

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            try
            {
                if (ModelState.IsValid) // Server check in case JS is disabled
                {
                    if (isAjaxCall)
                    {
                        _dbHelper.AddNewCategory(ajaxData);
                    }
                    else
                    {
                        _dbHelper.AddNewCategory(model.NewCategory.NewCategoryName);
                    }
                }
                else
                {
                    // Categories are not passed back from view, so they need to be filled into model again
                    model.Categories = _dbHelper.GetAllCategories();
                    return View("Index", model);
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "1";
                TempData["ErrorMessage"] = "Akce nemohla být dokončena.";
               
            }

            if (isAjaxCall)
            {

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
                TempData["Error"] = "1"; // Automatically shows error modal
                TempData["ErrorMessage"] = "Daná rubrika pro smazání nebyla nalezena.";
                return RedirectToAction("Index");
            }

            try
            {                             
                _dbHelper.DeleteCategory((int)id);
                
            }
            catch (Exception ex)
            {
                TempData["Error"] = "1"; // Automatically shows error modal
                TempData["ErrorMessage"] = ex.Message;              
            }           

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("AddNewCategoryPartialView", UpdateCategoryPage());
            }
            else
            {
                return View("Index", UpdateCategoryPage());
            }

        }
    }
}