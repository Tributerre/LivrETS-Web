using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using LivrETS.Models;
using System.Net;

namespace LivrETS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;

        public AdminController() { }

        public AdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: /Admin/ManageUsers
        [HttpGet]
        public ActionResult ManageUsers()
        {
            var db = new ApplicationDbContext();

            ViewBag.users = UserManager.Users.ToList();
            ViewBag.roles = (from role in db.Roles select role).ToList();
            return View();
        }

        // PUT: /Admin/ChangeUserRole
        // Change the role of a user.
        [HttpPut]
        public ActionResult ChangeUserRole(AjaxManageUsersViewModel model)
        {
            var user = UserManager.FindById(model.UserId);
            
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Remove from all roles just in case.
            UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());

            if (model.NewRole != null)
            {
                UserManager.AddToRole(user.Id, model.NewRole);
            }

            return Json(new { }, contentType: "application/json");
        }

        // DELETE: /Admin/Delete
        // Deletes one or multiple users.
        [HttpDelete]
        public ActionResult Delete(AjaxManageUsersViewModel model)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            
            if (model.UserId != null)  // Only one user to delete
            {
                users.Add(UserManager.FindById(model.UserId));
            }
            else  // Multiple users to delete
            {
                users = model.UserIdsList.ConvertAll(new Converter<string, ApplicationUser>(userId => UserManager.FindById(userId)));
            }

            foreach (var user in users)
            {
                if (user != null)
                {
                    Unregister(user);
                }
            }

            return Json(new { }, contentType: "application/json");
        }

        /// <summary>
        /// Removes a user from the system completely.
        /// </summary>
        /// <param name="user">The user to unregister</param>
        private void Unregister(ApplicationUser user)
        {
            var logins = UserManager.GetLogins(user.Id);
            var roles = UserManager.GetRoles(user.Id);

            foreach (var login in logins)
            {
                UserManager.RemoveLogin(user.Id, login);
            }

            if (roles.Count > 0)
            {
                UserManager.RemoveFromRoles(user.Id, roles.ToArray());
            }

            UserManager.Delete(user);
        }
    }
}