using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models.Database;
using Microsoft.AspNetCore.Mvc;

namespace JasperSiteCore.Areas.Admin.Controllers
{
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
            model.Categories = _dbHelper.GetAllCategories();
            return model;
        }

        [HttpPost]
        // If JS is enabled - data will be passed in ajaxData, otherwise in the model
        public IActionResult CreateNewCategory(CategoriesViewModel model,string ajaxData)
        {
            

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

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
            else {
                // Categories are not passed back from view, so they need to be filled into model again
                model.Categories = _dbHelper.GetAllCategories();
                return View("Index", model);
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
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                _dbHelper.DeleteCategory(id);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = ex.Message;              
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