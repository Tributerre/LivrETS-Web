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

        // GET: Offer
        public ActionResult Index()
        {
            //if current fair is finish, then take next fair 
            Fair currentFair = Repository.GetCurrentFair();
            Fair nextFair = Repository.GetNextFair();

            ViewBag.currentFair = (currentFair != null)?currentFair:nextFair;
            ViewData["whatFair"] = currentFair;
            ViewData["users"] = Repository.GetAllUsers().Count();
            ViewData["fairs"] = Repository.GetAllFairs().ToList().Count();
            ViewData["offers"] = Repository.GetAllOffers().Count();
            ViewData["saleitems"] = Repository.GetAllOffers().
                Where(offer => offer.MarkedSoldOn != offer.StartDate).Count();

            return View();
        }

        // GET: /Admin/CreateFair
        [HttpGet]
        public ActionResult CreateFair()
        {
            return View();
        }

        // GET: /Admin/EditFair
        [HttpGet]
        public ActionResult EditFair(string Id)
        {
            return View(Repository.GetFairById(Id));
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

        // GET: /Admin/ManageDetailsFair/5
        public ActionResult ManageDetailsFair(string id)
        {
            if (id == null)
                throw new HttpException(404, "Page not Found");

            Fair fair = Repository.GetFairById(id);
            FairStatistics statics = new FairStatistics(fair);
            ViewData["TotalSalesAmount"] = statics.GetTotalSalesAmount();
            ViewData["TotalSales"] = statics.GetTotalSales();
            ViewData["TotalAmountForLateRetreivals"] = statics.GetTotalAmountForLateRetreivals();

            return View(fair);
        }

        // GET: /Admin/ManageOffers
        [HttpGet]
        public ActionResult ManageOffers()
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
            var listRoles = Repository.GetAllRoles().ToList();
            
            var listUser = Repository.GetAllUsersForAdmin().ToList();

            return Json(new { listUser, listRoles, current_id=User.Identity.GetUserId() }, 
                contentType: "application/json");
        }

        // POST: /Admin/ListFairs
        // List all Fairs
        [HttpPost]
        public ActionResult ListFairs()
        {
            var listFairs = Repository.GetAllFairs().ToList();

            return Json(new { listFairs }, contentType: "application/json");
        }

        // POST: /Admin/ListOffersFair
        // List all Fairs
        [HttpPost]
        public ActionResult ListOffersFair(string id)
        {
            var currentFair = Repository.GetFairById(id);

            return Json(new {
                currentFair.Offers
            }, contentType: "application/json");
        }

        // POST: /Admin/ListOffers
        // List all Offers
        [HttpPost]
        public ActionResult ListOffers()
        {
            List<Offer> listOffers = Repository.GetAllAdminOffers().ToList();

            return Json(new { listOffers }, contentType: "application/json");
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
                message = "Modifications annulée: l'utilisateur n'existe pas";
            

            // Remove from all roles just in case.
            UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());

            if (model.NewRole.Equals("anonymous"))
            {
                status = true;
                message = "Modifications reusie";
            }


            if (model.NewRole != null && !model.NewRole.Equals("anonymous"))
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
                users = model.UserIdsList.ConvertAll(new Converter<string, 
                    ApplicationUser>(userId => UserManager.FindById(userId)));
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

                /*fair.SetDates(model.StartDate, model.EndDate.AddSeconds(model.EndDate.Second * -1))
                    .SetDates(model.PickingStartDate, model.PickingEndDate.AddSeconds(model.EndDate.Second * -1), 
                                forPhase: FairPhase.PICKING)
                    .SetDates(model.SaleStartDate, model.SaleEndDate.AddSeconds(model.EndDate.Second * -1), 
                                forPhase: FairPhase.SALE)
                    .SetDates(model.RetrievalStartDate, model.RetrievalEndDate.AddSeconds(model.EndDate.Second * -1), 
                                forPhase: FairPhase.RETRIEVAL);*/
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
                    where dbOffer.StartDate < fair.StartDate && dbOffer.StartDate >= 
                        pastFairStartDate && dbOffer.ManagedByFair
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
        public ActionResult DeleteFair(string id)
        {
            bool status = Repository.DeleteFair(id);
            return Json(new {
                status = status,
                message = (status) ? "suppression reussit" : "Erreur"
            }, contentType: "application/json");
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
                /*return Json(new AjaxFairViewModel
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
                }, contentType: "application/json");*/
                return Json(new AjaxFairViewModel
                {}, contentType: "application/json");
            }
        }


        // DELETE: /Admin/DeleteFairStep
        // Deletes one or more fairSteps.
        [HttpDelete]
        public ActionResult DeleteFairStep(string id)
        {
            bool status = Repository.DeleteFairStep(id);
            string message = (status) ? "suppression de l'etape" : "Erreur";

            return Json(new
            {
                status = status,
                message = message
            }, contentType: "application/json");
        }
        // POST: /Admin/CreateFairSubmit
        // Adds Fair
        [HttpPost]
        public ActionResult CreateFairSubmit(string session, string startDate,
            string endDate, string[] steps, string Id)
        {
            string message = "Erreur de dates";
            using (var db = new ApplicationDbContext())
            {
                Fair fair = null;
                FairStep fs = null;

                if (Id != null)
                {
                    fair = db.Fairs.FirstOrDefault(fairDb => fairDb.Id.ToString() == Id);
                    db.Fairs.Attach(fair as Fair);
                }
                else
                    fair = new Fair();


                fair.Trimester = session;
                DateTime startDateTmp = DateTime.Parse(startDate);
                DateTime endDateTmp = DateTime.Parse(endDate);

                if (DateTime.Compare(startDateTmp, endDateTmp) > 0)
                    return Json(new
                    {
                        status = false,
                        message = message
                    }, contentType: "application/json");

                fair.StartDate = startDateTmp;
                fair.EndDate = endDateTmp;

                if (Id == null)
                    db.Fairs.Add(fair);

                if (steps != null)
                {
                    for (int i = 0; i < steps.Count(); i++)
                    {
                        dynamic dynamicObject = new System.Web.Script.Serialization
                            .JavaScriptSerializer().Deserialize<dynamic>(steps[i]);

                        if(dynamicObject["StartDateTime"] == "" || dynamicObject["EndDateTime"] == "" ||
                            dynamicObject["Place"] == "" || dynamicObject["CodeStep"] == "")
                            return Json(new
                            {
                                status = false,
                                message = "Des informations sont manquantes"
                            }, contentType: "application/json");

                        DateTime start = DateTime.Parse(dynamicObject["StartDateTime"]);
                        DateTime end = DateTime.Parse(dynamicObject["EndDateTime"]);

                        if (DateTime.Compare(start, end) > 0 ||
                            (DateTime.Compare(start, startDateTmp) < 0 || DateTime.Compare(start, endDateTmp) > 0) ||
                                (DateTime.Compare(end, startDateTmp) < 0 && DateTime.Compare(end, endDateTmp) > 0))
                            return Json(new
                            {
                                status = false,
                                message = message
                            }, contentType: "application/json");

                        if (dynamicObject["Id"].Equals("-1"))
                        {
                            fs = new FairStep(Convert.ToString(dynamicObject["CodeStep"]),
                                Convert.ToString(dynamicObject["Place"]));
                            db.FairSteps.Attach(fs as FairStep);
                        }
                        else
                        {
                            string id = Convert.ToString(dynamicObject["Id"]);
                            fs = db.FairSteps.FirstOrDefault(fsdb => fsdb.Id.ToString().Equals(id));
                        }

                        fs.StartDateTime = start;
                        fs.EndDateTime = end;
                        fair.FairSteps.Add(fs);

                        if (dynamicObject["Id"].Equals("-1"))
                            db.FairSteps.Add(fs);
                    }
                }


                db.SaveChanges();
            }

            return Json(new
            {
                status = true,
                message = "Foire enregistrée"
            }, contentType: "application/json");
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