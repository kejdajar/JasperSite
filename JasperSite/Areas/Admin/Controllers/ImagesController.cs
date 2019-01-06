using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using JasperSite.Models;
using JasperSite.Models.Database;
using JasperSite.Areas.Admin.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace JasperSite.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ImagesController : Controller
    {
        // Images can be served after the instalation was completed, otherwise everything is redirected to the install page.
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Configuration.InstallationCompleted())
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                { "Controller", "Install" },
                { "Action", "Index" },
                        {"Area","Admin" }
                });
                base.OnActionExecuting(filterContext);
            }
        }

        private readonly DatabaseContext _databaseContext;
        private readonly DbHelper _dbHelper;
        private readonly IStringLocalizer _localizer;

        public ImagesController(DatabaseContext dbContext, IStringLocalizer<ImagesController> localizer)
        {
            this._databaseContext = dbContext;
            this._dbHelper = new DbHelper(dbContext);
            this._localizer = localizer;
        }

        /// <summary>
        /// Administration Image page
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// New images could be posted only by authorized users.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
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

                        string type = file.ContentType;
                        
                        if (type != "image/bmp" && type != "image/png" && type != "image/jpeg" && type != "image/gif" )
                        {
                            throw new Exception(_localizer["The selected format is not supported."]);
                        }

                        Image dbImageEntity = new Image();
                        dbImageEntity.Name = file.FileName;
                        dbImageEntity.ImageData = new ImageData() { Data = imageInBytes };
                        _databaseContext.Images.Add(dbImageEntity);
                        _databaseContext.SaveChanges();
                        TempData["Success"] = true;
                    }
                    else
                    {
                        throw new InvalidImageFormatException(_localizer["The selected file is not a image or contains no data."]);
                    }
                }
              
            }
            catch (InvalidImageFormatException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = _localizer["There was an error during image upload."] + ex.Message + ((ex.InnerException != null) ? ex.InnerException.Message : "");
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// If the image has been already deleted, deleted image placeholder will be returned instead.
        /// This method is publicly accessible.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>       
        [HttpGet]
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
                try // Image placeholder will be shown in case required image does not exist
                {
                    string missingImagePlaceholderRelativePath = Configuration.WebsiteConfig.MissingImagePath;
                    string absolutePath = Configuration.CustomRouting.RelativeThemePathToRootRelativePath(missingImagePlaceholderRelativePath);
                    byte[] bytes = System.IO.File.ReadAllBytes(absolutePath);
                    return File(bytes, "image/jpg");

                }
                catch
                {
                    return File(new byte[0], "image/jpg"); // Returns empty byte array (= blank page with no image)
                }

            }

        }

        /// <summary>
        /// This method is used only from the Admin section. It is used within the TinyMce plugin for displaying a list of images.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
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

        /// <summary>
        /// Returns list of images id --> used in tinyMCE insert modal window.
        /// This method is used only within the Administration.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public JsonResult GetImagesId()
        {
            try
            {
                List<int> imagesId = (from image in _dbHelper.Database.Images
                                      select image.Id).ToList();

                return Json(imagesId);
            }
            catch
            {
                return Json(null);       
            }
        }


        /// <summary>
        /// Images could be deleted only by authorized users. 
        /// </summary>
        /// <param name="imgId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult DeleteImage(int imgId)
        {
            try
            {
                _dbHelper.DeleteImageById(imgId);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "1"; // Automatically shows error modal
                ViewBag.ErrorMessage = ex.Message;
            }

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("UploadedImagesPartialView",UpdatePage());
            }
            else
            {
                return View("Index", UpdatePage());
            }
              
         
        }
    }
}