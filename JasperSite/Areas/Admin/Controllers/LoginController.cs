using JasperSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using JasperSite.Areas.Admin.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using JasperSite.Models.Database;
using Microsoft.Extensions.Localization;

namespace JasperSite.Areas.Admin.Controllers
{
    [AllowAnonymous]
    [Area("Admin")]
    public class LoginController : Controller
    {

        // Login page is not accessible in case the installation has not been completed yet
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

        public LoginController(DatabaseContext dbContext, IStringLocalizer<LoginController> localizer)
        {
            this._databaseContext = dbContext;
            this._dbHelper = new DbHelper(dbContext);
            this._localizer = localizer;
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
            // Input data cleansing: username IS NOT CASE-INSENSITIVE, with no leading and trailing whitespaces
            model.Username = model.Username.ToLower().Trim();

            // If the user was locked out of the system due to incorrenct connection string, 
            // they can correct it through FTP --> now it has to be reloaded.
            Configuration.GlobalWebsiteConfig.RefreshData();

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

                    // After succesful login - global configuration data are reloaded
                    Configuration.Initialize();

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
                    throw new Exception();
                }

            }
            catch
            {
               TempData["ErrorMessage"]= _localizer["Invalid username or password."];
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