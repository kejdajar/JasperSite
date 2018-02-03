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

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [AllowAnonymous]
    [Area("Admin")]
    public class LoginController : Controller
    {
        // Firstly, the installation must have already been completed before accesing administration panel
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(!Configuration.InstallationCompleted())
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                { "Controller", "Install" },
                { "Action", "Index" },
                        {"Area","Admin" }
                    });
            }

            base.OnActionExecuting(filterContext);
        }

        // GET: Admin/Login
        [HttpGet]
        public ActionResult Index()
        {
            LoginViewModel model = new LoginViewModel();
            model.Username = "admin";
            model.Password = "admin";           
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel model, string returnUrl)
        {

            if (ModelState.IsValid && model.Username == "admin" && model.Password == "admin")
            {
                

                List<Claim> claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.Name,model.Username )
                    };
                ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                HttpContext.SignInAsync(principal, new AuthenticationProperties { IsPersistent = model.Remember });

               if(string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
               else
                {
                    return Redirect(returnUrl);
                }
               

            }
            else
                return Content("error");
           
                  //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, model.Username, null));
                //identity.AddClaim(new Claim(ClaimTypes.Name, model.Username, null));
                //var principal = new ClaimsPrincipal(identity);
                //HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = model.Remember });
                //return RedirectToAction("index", "home");
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
            //return View("Index");
        }
    }
}