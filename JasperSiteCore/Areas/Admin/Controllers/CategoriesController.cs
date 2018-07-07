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
        public IActionResult CreateNewCategory(string btnCategoryName)
        {
            _dbHelper.AddNewCategory(btnCategoryName);

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
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
                return View("Index", UpdateCategoryPage());
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