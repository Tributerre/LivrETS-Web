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
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using LivrETS.Models;
using LivrETS.ViewModels;
using System.Net;
using System.Web.Security;
using LivrETS.Repositories;

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
            return View();
        }

        // GET: /Admin/ManageFairs
        [HttpGet]
        public ActionResult ManageFairs()
        {
           
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
        private LivrETSRepository _repository;
        public LivrETSRepository Repository
        {
            get
            {
                if (_repository == null)
                {
                    _repository = new LivrETSRepository();
                }

                return _repository;
            }
            private set
            {
                _repository = value;
            }
        }

        #region Ajax

        // POST: /Admin/ListUsers
        // List all user
        [HttpPost]
        public ActionResult ListUsers()
        {
            /*var db = new ApplicationDbContext();
            var list_user = UserManager.Users.ToList();
            var listRoles = (from role in db.Roles
                             select new { Id = role.Id, Name = role.Name }).ToList();

            var listUser = (from user in db.Users
                            orderby user.FirstName descending
                            select new
                            {
                                user = user,
                                role = user.Roles.Join(db.Roles, userRole => userRole.RoleId, role => role.Id, (userRole, role) => role).Select(role => role.Name)
                            }).ToList();
            db.Dispose();*/
            var listRoles = Repository.GetAllRoles();
            
            var listUser = Repository.GetAllUsers();

            return Json(new { listUser, listRoles, current_id=User.Identity.GetUserId() }, contentType: "application/json");
        }

        // POST: /Admin/ListFairs
        // List all user
        [HttpPost]
        public ActionResult ListFairs()
        {
            var listFairs = Repository.GetAllFairs();

            return Json(new { listFairs }, contentType: "application/json");
        }

        // PUT: /Admin/ChangeUserRole
        // Change the role of a user.
        [HttpPut]
        public ActionResult ChangeUserRole(AjaxManageUsersViewModel model)
        {
            var user = UserManager.FindById(model.UserId);
            bool status = false;
            String message = null;
            
            if (user == null)
            {
                message = "Modifications annulée: l'utilisateur n'existe pas";
            }

            // Remove from all roles just in case.
            UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());

            if (model.NewRole != null)
            {
                UserManager.AddToRole(user.Id, model.NewRole);
                status = true;
                message = "Modifications reusie";
            }

            return Json(new { status=status, message=message }, contentType: "application/json");
        }

        // DELETE: /Admin/Delete
        // Deletes one or multiple users.
        [HttpDelete]
        public ActionResult DeleteUser(AjaxManageUsersViewModel model)
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

        // POST: /Admin/Fair
        // Adds or modifies a new fair to the system. To modify, populate the Id when posting.
        [HttpPost]
        public ActionResult Fair(AjaxFairViewModel model)
        {
            // TODO: Transfert database specifics to a repository.
            using (var db = new ApplicationDbContext())
            {
                Fair fair;
                if (model.Id == null)
                {
                    fair = new Fair();
                }
                else
                {
                    fair = db.Fairs.FirstOrDefault(fairDb => fairDb.Id.ToString() == model.Id);

                    if (fair == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                }

                fair.SetDates(model.StartDate, model.EndDate)
                    .SetDates(model.PickingStartDate, model.PickingEndDate, forPhase: FairPhase.PICKING)
                    .SetDates(model.SaleStartDate, model.SaleEndDate, forPhase: FairPhase.SALE)
                    .SetDates(model.RetrievalStartDate, model.RetrievalEndDate, forPhase: FairPhase.RETRIEVAL);
                fair.Trimester = model.Trimester;

                if (model.Id == null)
                {
                    db.Fairs.Add(fair);
                }

                // Updating all past offers until the fair start date before this fair
                var pastFair = (
                    from dbFair in db.Fairs
                    where dbFair.StartDate < fair.StartDate
                    orderby dbFair.StartDate descending
                    select dbFair
                ).FirstOrDefault();

                var pastFairStartDate = pastFair?.StartDate ?? new DateTime(1970, 01, 01);
                var pastOffers = (
                    from dbOffer in db.Offers
                    where dbOffer.StartDate < fair.StartDate && dbOffer.StartDate >= pastFairStartDate && dbOffer.ManagedByFair
                    select dbOffer
                );

                // Removing the old offers managed by the fair.
                foreach (var offer in fair.Offers)
                {
                    fair.Offers.Remove(offer);
                }

                // Adding the new ones (including the old ones that are still valid for this fair).
                foreach (var offer in pastOffers)
                {
                    fair.Offers.Add(offer);
                }

                db.SaveChanges();
            }

            return Json(new { }, contentType: "application/json");
        }

        // DELETE: /Admin/DeleteFair
        // Deletes one or more fairs.
        [HttpDelete]
        public ActionResult DeleteFair(AjaxFairViewModel model)
        {
            List<Fair> fairs = new List<Fair>();

            using (var db = new ApplicationDbContext())
            {
                if (model.Id != null)
                {
                    var fair = db.Fairs.FirstOrDefault(fairDb => fairDb.Id.ToString() == model.Id);

                    if (fair != null)
                    {
                        fairs.Add(fair);
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                }
                else
                {
                    fairs = model.IdsList.ConvertAll(new Converter<string, Fair>(id => db.Fairs.FirstOrDefault(fair => fair.Id.ToString() == id)));

                    if (fairs.Any(fair => fair == null))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                }

                foreach (var fair in fairs)
                {
                    db.Fairs.Remove(fair);
                }
                db.SaveChanges();
            }
            return Json(new { }, contentType: "application/json");
        }

        // POST: /Admin/GetFairData
        // Gets the data of a fair. POST method is used to avoid verifying
        // an anti-forgery token with GET.
        [HttpPost]
        public ActionResult GetFairData(AjaxFairViewModel model)
        {
            Fair fair = null;
            using (var db = new ApplicationDbContext())
            {
                fair = (
                    from fairdb in db.Fairs
                    where fairdb.Id.ToString() == model.Id
                    select fairdb
                ).FirstOrDefault();
            }

            if (fair == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            else
            {
                return Json(new AjaxFairViewModel
                {
                    Id = fair.Id.ToString(),
                    StartDate = fair.StartDate,
                    EndDate = fair.EndDate,
                    PickingStartDate = fair.PickingStartDate,
                    PickingEndDate = fair.PickingEndDate,
                    SaleStartDate = fair.SaleStartDate,
                    SaleEndDate = fair.SaleEndDate,
                    RetrievalStartDate = fair.RetrievalStartDate,
                    RetrievalEndDate = fair.RetrievalEndDate,
                    Trimester = fair.Trimester
                }, contentType: "application/json");
            }
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