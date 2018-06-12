using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Models;
using JasperSiteCore.Areas.Admin.ViewModels;
using JasperSiteCore.Models.Security;
using Microsoft.AspNetCore.Authorization;

namespace JasperSiteCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly DbHelper _dbHelper;
        public UsersController(DatabaseContext dbContext)
        {
            _databaseContext = dbContext;
            _dbHelper = new DbHelper(dbContext);
        }

        [HttpGet]
        public IActionResult Index()
        {          
            return View(UpdatePage());
        }

        public UsersViewModel UpdatePage()
        {
            List<User> users = _dbHelper.GetAllUsers();
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
            User userToEdit =_dbHelper.GetUserById(id);
            EditUserViewModel model = new EditUserViewModel();

            model.AllRoles = _dbHelper.GetAllRoles();
            model.Nickname = userToEdit.Nickname;
            model.Id = userToEdit.Id;
            model.Username = userToEdit.Username;
            model.RoleId = userToEdit.RoleId;
            model.NewPasswordPlainText = string.Empty;
            model.NewPasswordPlainTextAgain = string.Empty;
            return model;
        }

        [HttpPost]        
        public IActionResult EditUser(EditUserViewModel model)
        {
           
                // Get current user
                User user = _dbHelper.GetUserById(model.Id);
                user.Nickname = model.Nickname;
                user.Username = model.Username;
                user.RoleId = model.RoleId;


                 // Password will be changed only if both fields are filled in
                if (!string.IsNullOrWhiteSpace(model.NewPasswordPlainText) && !string.IsNullOrWhiteSpace(model.NewPasswordPlainTextAgain))
                {                   
                        string salt;
                        string hashedNewPaswd;
                        Authentication.HashPassword(model.NewPasswordPlainTextAgain, out salt, out hashedNewPaswd);
                        //Configuration.DbHelper.ChangePassword(user.Id, hashedNewPaswd, salt);

                        user.Password = hashedNewPaswd;
                        user.Salt = salt;
                    
                }

              if (ModelState.IsValid)
            {
                _dbHelper.SaveChanges();
            }

            

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                EditUserViewModel cleanModel = UpdateEditUserPage(model.Id);   
                return PartialView("EditUserPartialView",cleanModel);
            }
            else
            {
                return RedirectToAction("EditUser",new { id=model.Id});
            }
            
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            AddUserViewModel model = new AddUserViewModel();
            model.AllRoles = _dbHelper.GetAllRoles();
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

           _dbHelper.AddNewUser(u);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
           _dbHelper.DeleteUserById(id);

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