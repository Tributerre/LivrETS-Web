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
using System.Configuration;
using System;

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
        public ActionResult Index(string sortOrder, string currentFilter, string select_search, 
            string searchString, double Pmin = 1, double Pmax = 100000, int? page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            IEnumerable<Offer> offers = null;
            int max_home_page = 20;

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;
            offers = Repository.GetAllOffers(Pmin, Pmax, select_search, searchString, sortOrder);
            int pageNumber = (page ?? 1);

            return View(offers.ToList().ToPagedList(pageNumber, max_home_page));
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