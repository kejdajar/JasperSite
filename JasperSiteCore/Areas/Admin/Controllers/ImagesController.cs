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
        [HttpGet]
        public IActionResult Index()
        {
            ImagesViewModel model = new ImagesViewModel();
            model.ImagesFromDatabase = Configuration.DbHelper.GetAllImages();
            return View(model);
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
                    Configuration.DbHelper._db.Images.Add(dbImageEntity);
                    Configuration.DbHelper._db.SaveChanges();
                }
            }

            ImagesViewModel model = new ImagesViewModel();
            model.ImagesFromDatabase = Configuration.DbHelper.GetAllImages();
            return View("Index",model);
        }

        public FileResult GetImage(int id)
        {                       
           // Query using navigation property + include in DbHelper class
           // Query must be Async in order to display all images one after another as they are being loaded
           Task<Image> image = Configuration.DbHelper._db.Images.Where(i => i.Id == id).SingleAsync();
            
            return  File(image.Result.ImageData.Data,"image/jpg");
        }
    }
}