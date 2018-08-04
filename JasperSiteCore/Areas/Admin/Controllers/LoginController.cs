using JasperSiteCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using JasperSiteCore.Areas.Admin.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [AllowAnonymous]
    [Area("Admin")]
    public class LoginController : Controller
    {
        
        private readonly DatabaseContext _databaseContext;
        private readonly DbHelper _dbHelper;

        public LoginController(DatabaseContext dbContext)
        {
            this._databaseContext = dbContext;
            this._dbHelper = new DbHelper(dbContext);
        }

        // GET: Admin/Login
        [HttpGet]
        public ActionResult Index()
        {
           

            LoginViewModel model = new LoginViewModel();           
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel model, string returnUrl)
        {
            // Input data cleansing: username IS CASE-INSENSITIVE, with no leading and trailing whitespaces
            model.Username = model.Username.ToLower().Trim();

            try
            {
                User user = _dbHelper.GetUserWithUsername(model.Username);
                string filledInPassword = model.Password;
                bool isPswdCorrect = user.ComparePassword(filledInPassword);


                if (ModelState.IsValid && isPswdCorrect)
                {


                    List<Claim> claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.Name,model.Username),
                       new Claim (ClaimTypes.Role,user.Role.Name)
                    };
                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    HttpContext.SignInAsync(principal, new AuthenticationProperties { IsPersistent = model.Remember });

                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }

                }
                else
                {
                    ViewBag.Error = "1";
                    ViewBag.ErrorMessage = "Bylo zadáno špatné uživatelské jméno nebo heslo";
                    return View("Index");
                }

            }
            catch
            {
                ViewBag.Error = "1";
                //ViewBag.ErrorMessage = $"Při přihlašování došlo k neočekávané chybě.{((ex.InnerException!=null)? ("Popis chyby:"+ex.InnerException.Message):" ")}";
                ViewBag.ErrorMessage = "Bylo zadáno špatné uživatelské jméno nebo heslo";
                return View("Index");
            }                 
        }

        [HttpGet]
        public ActionResult UnauthorizedUser()
        {
           
            return View("_Unauthorized");
 
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
            
        }
    }
}