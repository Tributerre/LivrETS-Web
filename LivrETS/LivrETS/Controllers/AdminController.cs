/*
LivrETS - Centralized system that manages selling of pre-owned ETS goods.
Copyright (C) 2016  TribuTerre

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
 */
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
            using (var db = new ApplicationDbContext())
            {
                ViewBag.users = UserManager.Users.ToList();
                ViewBag.roles = (from role in db.Roles select role).ToList();
            }
            return View();
        }

        // GET: /Admin/ManageFairs
        [HttpGet]
        public ActionResult ManageFairs()
        {
            using (var db = new ApplicationDbContext())
            {
                ViewBag.fairs = (from fair in db.Fairs select fair).ToList();
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userManager?.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Ajax

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

        #endregion

        #region Helpers

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

        #endregion
    }
}