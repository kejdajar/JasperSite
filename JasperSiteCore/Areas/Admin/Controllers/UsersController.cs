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
            return View(UpdatePage());
        }

        public UsersViewModel UpdatePage()
        {
            List<User> users = Configuration.DbHelper.GetAllUsers();
            UsersViewModel model = new UsersViewModel();
            model.Users = users;

            // Active user will be in the first row
            string activeUserName = User.Identity.Name;
            JasperSiteCore.Models.Database.User currentUser = users.Where(u => u.Username.Trim().ToLower() == activeUserName.Trim().ToLower()).Single();
            users.Remove(currentUser);
            users.Insert(0, currentUser);

            return model;
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {           
            return View(UpdateEditUserPage(id));
        }

        public EditUserViewModel UpdateEditUserPage(int id)
        {
            User userToEdit = Configuration.DbHelper.GetUserById(id);
            EditUserViewModel model = new EditUserViewModel();

            model.AllRoles = Configuration.DbHelper.GetAllRoles();
            model.Nickname = userToEdit.Nickname;
            model.Id = userToEdit.Id;
            model.Username = userToEdit.Username;
            model.RoleId = userToEdit.RoleId;
            return model;
        }

        [HttpPost]       
        public IActionResult EditUser(EditUserViewModel model)
        {
           
                // Get current user
                User user = Configuration.DbHelper.GetUserById(model.Id);
                user.Nickname = model.Nickname;
                user.Username = model.Username;
                user.RoleId = model.RoleId;


                 // Password will be changed only if both fields are filled in
                if (!string.IsNullOrWhiteSpace(model.NewPasswordPlainText) && !string.IsNullOrWhiteSpace(model.NewPasswordPlainTextAgain))
                {
                    if (model.NewPasswordPlainText.Trim()==model.NewPasswordPlainTextAgain.Trim())  // Check password
                    {
                        string salt;
                        string hashedNewPaswd;
                        Authentication.HashPassword(model.NewPasswordPlainTextAgain, out salt, out hashedNewPaswd);
                        //Configuration.DbHelper.ChangePassword(user.Id, hashedNewPaswd, salt);

                        user.Password = hashedNewPaswd;
                        user.Salt = salt;

                    }
                    else
                    {
                        ModelState.AddModelError("NewPasswordPlainTextAgain", "Zadaná hesla se neshodují.");
                    }
                }

              if (ModelState.IsValid)
            {
                Configuration.DbHelper.SaveChanges();
            }

            model.AllRoles = Configuration.DbHelper.GetAllRoles();
            return View(model);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            AddUserViewModel model = new AddUserViewModel();
            model.AllRoles = Configuration.DbHelper.GetAllRoles();
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveNewUser(AddUserViewModel model)
        {
            User u = new User();
            u.Nickname = model.Nickname;            
            u.RoleId = model.RoleId;
            u.Username = model.Username;

            string salt;
            string hashedNewPaswd;
            Authentication.HashPassword(model.NewPasswordPlainTextAgain, out salt, out hashedNewPaswd);
            u.Password = hashedNewPaswd;
            u.Salt = salt;

            Configuration.DbHelper.AddNewUser(u);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            Configuration.DbHelper.DeleteUserById(id);

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("UserListPartialView",UpdatePage());
            }
            else
            {
                return RedirectToAction("Index");
            }

                
        }
    }
}