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
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View(UpdatePage());
        }

        public ImagesViewModel UpdatePage()
        {
            try
            {
                ImagesViewModel model = new ImagesViewModel();
                model.ImagesFromDatabase = _dbHelper.GetAllImages();
                return model;
            }
            catch 
            {
                ImagesViewModel model = new ImagesViewModel();
                model.ImagesFromDatabase = null;
                return model;
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult PostImage(ICollection<IFormFile> files)
        {
            try
            {
                foreach (IFormFile file in files)
                {
                    if (file.Length > 0)
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
            }
            catch 
            {

            // TODO: error
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// If the image has been already deleted, deleted image placeholder will be returned instead.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>       
        public FileResult GetImage(int id)
        {                       
           // Query using navigation property + include in DbHelper class
           // Query must be Async in order to display all images one after another as they are being loaded
           try
            {
                Task<Image> image = _databaseContext.Images.Include(i => i.ImageData).Where(i => i.Id == id).SingleAsync();
                return File(image.Result.ImageData.Data, "image/jpg");
            }
            catch
            {
                try {
                    string missingImagePlaceholderRelativePath = Configuration.WebsiteConfig.MissingImagePath;
                    string absolutePath = Configuration.CustomRouting.RelativeThemePathToRootRelativePath(missingImagePlaceholderRelativePath);
                    byte[] bytes = System.IO.File.ReadAllBytes(absolutePath);
                    return File(bytes, "image/jpg");
                }
                catch
                {
                    return null;
                }
               
            }
           
        }

        [HttpGet]
        public JsonResult GetImageForImageList(int id)
        {
            // Query using navigation property + include in DbHelper class
            // Query must be Async in order to display all images one after another as they are being loaded
            try
            {
                Task<Image> image = _databaseContext.Images.Include(i => i.ImageData).Where(i => i.Id == id).SingleAsync();
                string imageName = image.Result.Name;
                byte[] imageData = image.Result.ImageData.Data;
                return Json(new { Name = imageName, Data = imageData, Id=id });
            }
            catch
            {
                return Json(null);       

            }

        }

        [HttpGet]
        public JsonResult GetImagesId()
        {
            List<int> imagesId = (from image in _dbHelper.Database.Images
                                  select image.Id).ToList();

            return Json( imagesId);
        }



        [Authorize]
        [HttpGet]
        public IActionResult DeleteImage(int imgId)
        {
            try
            {
                _dbHelper.DeleteImageById(imgId);
            }
            catch 
            {

               //TODO: error
            }

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