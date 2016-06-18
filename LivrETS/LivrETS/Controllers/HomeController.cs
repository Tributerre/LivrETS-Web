﻿/*
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
using LivrETS.ViewModels;
using LivrETS.Models;
using LivrETS.Repositories;

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
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Home/Sell
        [HttpGet]
        public ActionResult Sell()
        {
            ArticleViewModel model = new ArticleViewModel();

            model.Courses = Repository.GetAllCourses().ToList();
            return View(model);
        }

        // POST: /Home/Sell
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sell(ArticleViewModel model)
        {
            //if (model.ISBN == null)
            //{
            //    ModelState.AddModelError("ISBN", "Veuillez indiquer le ISBN.");
            //}

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                model.Courses = Repository.GetAllCourses().ToList();
                return View(model);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_repository != null)
                {
                    _repository.Dispose();
                    _repository = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}