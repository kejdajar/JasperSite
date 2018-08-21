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
            try
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
            catch
            {
                UsersViewModel model = new UsersViewModel();
                model.Users = null;
                return model;
            }
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            return View(UpdateEditUserPage(id));
        }

        public EditUserViewModel UpdateEditUserPage(int id)
        {
            try
            {
                User userToEdit = _dbHelper.GetUserById(id);
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
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public IActionResult EditUser(EditUserViewModel model)
        {
            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            try
            {
                if (ModelState.IsValid)
                {
                    // Get current user
                    User user = _dbHelper.GetUserById(model.Id);
                    user.Nickname = model.Nickname;
                    user.Username = model.Username;
                    user.RoleId = model.RoleId;


                    // Check whether current user is not the last admin and his role being changed to non-admin role
                    string currentRole = user.Role.Name;
                    string newRole = _dbHelper.GetAllRoles().Where(r => r.Id == model.RoleId).Single().Name;
                    int numberOfAdministrators = _dbHelper.GetAllAdministrators().Count();
                    if (numberOfAdministrators == 1 && currentRole == "Admin" && newRole != "Admin") // this can't be allowed
                    {
                        throw new NoRemainingAdminException("There must be at least one administrator among users.");
                    }

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

                    _dbHelper.SaveChanges();
                    TempData["Success"] = true;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (NoRemainingAdminException)
            {
                TempData["ErrorMessage"] = "V systému musí být alespoň jeden administrátor.";

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Změny nebylo možné uložit";

            }


            if (isAjaxCall)
            {
                ModelState.Clear();
                return PartialView("EditUserPartialView", UpdateEditUserPage(model.Id));
            }
            else
            {
                return RedirectToAction("EditUser", new { id = model.Id });
            }

        }

        [HttpGet]
        public IActionResult AddUser()
        {
            try
            {
                AddUserViewModel model = new AddUserViewModel();
                model.AllRoles = _dbHelper.GetAllRoles();               
                return View(model);
            }
            catch 
            {
                return View("_Error");
            }
           
        }

        [HttpPost]
        public IActionResult SaveNewUser(AddUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
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
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                // Error message will be shown & previously entered data will remain the same
                TempData["ErrorMessage"] = "Nebylo možné provést dané změny. Zkontrolujte prosím všechna pole.";
                model.AllRoles = _dbHelper.GetAllRoles(); // list of roles is not posted back
                return View("AddUser",model);
            }            

        }

        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                User userToBeDeleted = _dbHelper.GetUserById(id);
                // Check whether current user is not the last admin and his role being changed to non-admin role
                string currentRole = userToBeDeleted.Role.Name;
                int numberOfAdministrators = _dbHelper.GetAllAdministrators().Count();

                if (numberOfAdministrators == 1 && currentRole == "Admin") // this  is the last admin and thus can't be deleted
                {
                    throw new NoRemainingAdminException("There must be at least one administrator among users.");
                }
              
                _dbHelper.DeleteUserById(id);
                TempData["Success"] = true;
            }
            catch (NoRemainingAdminException)
            {
                TempData["ErrorMessage"] = "V systému musí být alespoň jeden administrátor.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Daný uživatel nemohl být smazán.";
            }

            bool isAjaxCall = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjaxCall)
            {
                return PartialView("UserListPartialView", UpdatePage());
            }
            else
            {
                return View("Index", UpdatePage());
            }


        }
    }
}