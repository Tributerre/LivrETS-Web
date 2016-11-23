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
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LivrETS.Models;
using LivrETS.Repositories;
using PagedList;
using Hangfire;
using Microsoft.AspNet.Identity;

namespace LivrETS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
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

        public HomeController() { }

        public HomeController(LivrETSRepository livretsRepository)
        {
            Repository = livretsRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(string sortOrder, string currentFilter, 
            string searchString, double Pmin = 1, double Pmax = 500, int? page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            IEnumerable<Offer> offers = null;
            var oll = Repository.GetStatsFairs();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            /*List<ApplicationUser> listAllUsers = new List<ApplicationUser>();
            ApplicationUser user = Repository.GetUserBy(null, User.Identity.GetUserId());
            listAllUsers.Add(user);
            NotificationManager.getInstance().sendNotification(
                    new Notification(NotificationOptions.ENDFAIRPICKING, listAllUsers)
                );*/

            ViewBag.CurrentFilter = searchString;
            offers = Repository.GetAllOffers(Pmin, Pmax, searchString, sortOrder);

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return View(offers.ToList().ToPagedList(pageNumber, pageSize));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository?.Dispose();
                _repository = null;
            }

            base.Dispose(disposing);
        }
    }
}