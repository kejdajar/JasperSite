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
            // Image image= Configuration.DbHelper.GetAllImages().Where(i => i.Id == id).Single();

            var image = (from images in Configuration.DbHelper._db.Images
                               from imagesData in Configuration.DbHelper._db.ImageData
                               where images.ImageDataId == imagesData.Id && images.Id == id
                               select imagesData).FirstOrDefaultAsync();
            
            return  File(image.Result.Data, "image/jpg");
        }
    }
}