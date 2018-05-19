using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Models;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models.Security;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            List<User> users = Configuration.DbHelper.GetAllUsers();  
            UsersViewModel model = new UsersViewModel();
            model.Users = users;
            return View(model);
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            User userToEdit = Configuration.DbHelper.GetUserById(id);
            EditUserViewModel model = new EditUserViewModel();

            model.AllRoles = Configuration.DbHelper.GetAllRoles();
            model.Nickname = userToEdit.Nickname;
            model.Id = userToEdit.Id;
            model.Username = userToEdit.Username;          
            model.RoleId = userToEdit.RoleId;
           
            return View(model);
        }

        [HttpPost]
       
        public IActionResult EditUser(EditUserViewModel model)
        {
           if (ModelState.IsValid)
            {
                // Get current user
                User user = Configuration.DbHelper.GetUserById(model.Id);
           

           
                // Check password
                if(user.ComparePassword(model.OldPasswordPlainText))
                {
                    string salt;
                    string hashedNewPaswd;
                    Authentication.HashPassword(model.NewPasswordPlainText, out salt, out hashedNewPaswd);
                    //Configuration.DbHelper.ChangePassword(user.Id, hashedNewPaswd, salt);

                    user.Nickname = model.Nickname;
                    user.Username = model.Username;
                    user.RoleId = model.RoleId;
                    user.Password = hashedNewPaswd;
                    user.Salt = salt;
                    Configuration.DbHelper.SaveChanges();
                }
            }
            return View(model);
        }
    }
}