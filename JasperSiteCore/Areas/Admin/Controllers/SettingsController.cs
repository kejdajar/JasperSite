using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly DbHelper _dbHelper;
        public SettingsController(DatabaseContext dbContext)
        {
            this._databaseContext = dbContext;
            this._dbHelper = new DbHelper(dbContext);
        }

        [HttpGet]
        public IActionResult Index()
        {
            SettingsViewModel model = new SettingsViewModel();
            model.WebsiteName = _dbHelper.GetWebsiteName();
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveSettings(SettingsViewModel model)
        {
            if(ModelState.IsValid)
            {
                _dbHelper.SetWebsiteName(model.WebsiteName);
            }
           
            return View(model);
        }
    }
}