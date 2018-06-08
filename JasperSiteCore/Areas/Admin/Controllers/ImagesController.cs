using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Areas.Admin.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ImagesController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly DbHelper _dbHelper;
        public ImagesController(DatabaseContext dbContext)
        {
            this._databaseContext = dbContext;
            this._dbHelper = new DbHelper(dbContext);
        }


        [HttpGet]
        public IActionResult Index()
        {

            return View(UpdatePage());
        }

        public ImagesViewModel UpdatePage()
        {
            ImagesViewModel model = new ImagesViewModel();
            model.ImagesFromDatabase = _dbHelper.GetAllImages();
            return model;
        }

        [HttpPost]
        public IActionResult PostImage(ICollection<IFormFile> files)
        {
            foreach(IFormFile file in files)
            {
                if(file.Length >0)
                {
                    MemoryStream ms = new MemoryStream();
                    file.CopyTo(ms);
                    byte[] imageInBytes = ms.ToArray();

                    Image dbImageEntity = new Image();
                    dbImageEntity.Name = file.FileName;
                    dbImageEntity.ImageData = new ImageData() { Data = imageInBytes };
                    _databaseContext.Images.Add(dbImageEntity);
                    _databaseContext.SaveChanges();
                }
            }

            ImagesViewModel model = new ImagesViewModel();
            model.ImagesFromDatabase = _dbHelper.GetAllImages();
            return RedirectToAction("Index");
        }

        public FileResult GetImage(int id)
        {                       
           // Query using navigation property + include in DbHelper class
           // Query must be Async in order to display all images one after another as they are being loaded
           Task<Image> image = _databaseContext.Images.Include(i=>i.ImageData).Where(i => i.Id == id).SingleAsync();
            
            return  File(image.Result.ImageData.Data,"image/jpg");
        }

        [HttpGet]
        public IActionResult DeleteImage(int imgId)
        {
            _dbHelper.DeleteImageById(imgId);

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("UploadedImagesPartialView",UpdatePage());
            }
            else
            {
                return RedirectToAction("Index");
            }
              
         
        }
    }
}