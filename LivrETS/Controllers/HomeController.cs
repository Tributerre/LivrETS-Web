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
using LivrETS.ViewModels;
using LivrETS.Models;
using LivrETS.Service.IO;
using LivrETS.Repositories;
using Microsoft.AspNet.Identity;
using System.Threading;
using System.Net;

namespace LivrETS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private const string UPLOADS_PATH = "~/Content/Uploads";
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
            ViewBag.ListOffer = Repository.GetAllOffers();
            return View();
        }

        // GET: /Home/Sell
        [HttpGet]
        public ActionResult Sell()
        {
            var model = new ArticleViewModel();

            Session["images"] = null;
            model.Courses = Repository.GetAllCourses().ToList();
            ThreadPool.QueueUserWorkItem(state => 
            {
                var arguments = state as Tuple<string, string>;
                FileSystemFacade.CleanTempFolder(uploadsPath: arguments.Item1, userId: arguments.Item2);
            }, state: new Tuple<string, string>(Server.MapPath(UPLOADS_PATH), User.Identity.GetUserId()));

            return View(model);
        }

        public ActionResult DetailOffer(string id)
        {
            Offer offer = Repository.GetOfferBy(id);
            return View(offer);
        }

        // POST: /Home/Sell
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sell(ArticleViewModel model)
        {
            var course = Repository.GetCourseByAcronym(model.Acronym);

            // Validating the model
            if (model.Type != Article.CALCULATOR_CODE && string.IsNullOrEmpty(model.ISBN))
            {
                ModelState.AddModelError(nameof(ArticleViewModel.ISBN), "Le type choisi requiert un ISBN ou un code.");
            }

            if (course == null && model.Type != Article.CALCULATOR_CODE)
            {
                ModelState.AddModelError(nameof(ArticleViewModel.Acronym), "Le type choisi requiert un cours de la liste.");
            }

            // Proceeding to add the new offer.
            if (ModelState.IsValid)
            {
                Article newArticle = null;
                var uploadsPath = Server.MapPath(UPLOADS_PATH);

                switch (model.Type)
                {
                    case Article.BOOK_CODE:
                        newArticle = new Book()
                        {
                            Course = course,
                            Title = model.Title,
                            ISBN = model.ISBN
                        };
                        break;

                    case Article.COURSE_NOTES_CODE:
                        newArticle = new CourseNotes()
                        {
                            Course = course,
                            Title = model.Title,
                            SubTitle = "Sample Subtitle",  // FIXME: Inconsistent with Title in Article and there's no Title for Offer.
                            BarCode = model.ISBN
                        };
                        break;

                    case Article.CALCULATOR_CODE:
                        newArticle = new Calculator()
                        {
                            Title = model.Title,
                            Model = model.CalculatorModel
                        };
                        break;
                }

                var now = DateTime.Now;
                Offer offer = new Offer()
                {
                    StartDate = now,
                    MarkedSoldOn = now,
                    Price = model.Price,  // FIXME: No elements for this in the view. Weird.
                    Condition = model.Condition,
                    Article = newArticle,
                    ManagedByFair = false,
                    Title = model.Title  // FIXME: No elements for this in the view. Weird.
                };

                if (model.ForNextFair)
                {
                    var nextFair = Repository.GetNextFair();

                    offer.ManagedByFair = true;
                    nextFair?.Offers.Add(offer);
                }
                
                Repository.AddOffer(offer, toUser: User.Identity.GetUserId());

                if (Session["images"] != null)
                {
                    List<OfferImage> images = Session["images"] as List<OfferImage>;
                    foreach (var image in images)
                    {
                        image.MovePermanently(uploadsPath);
                        offer.AddImage(image);
                    }
                }

                Repository.Update();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                model.Courses = Repository.GetAllCourses().ToList();
                return View(model);
            }
        }

        #region Ajax
        [HttpPost]
        public JsonResult AddImage(HttpPostedFileBase image)
        {
            var uploadsPath = Server.MapPath(UPLOADS_PATH);
            var offerImage = new OfferImage();
            offerImage.SaveImageTemporarily(image, User.Identity.GetUserId(), uploadsPath);

            if (Session["images"] == null)
            {
                Session["images"] = new List<OfferImage>();
            }

            //offerImage.Id = Guid.NewGuid();
            (Session["images"] as List<OfferImage>).Add(offerImage);
            return Json(new { thumbPath = offerImage.RelativeThumbnailPath }, contentType: "application/json");
        }

        [HttpPost]
        public ActionResult AddNewCourse(string acronym)
        {
            if (Repository.GetCourseByAcronym(acronym) != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }

            Repository.AddNewCourse(acronym, title: "Sample Title");  // FIXME: Temporary. Eventually, it must be handled correctly
            var recentlyAddedCourse = Repository.GetCourseByAcronym(acronym);
            return Json(new
            {
                courseId = recentlyAddedCourse.Id,
                acronym = recentlyAddedCourse.Acronym
            }, contentType: "application/json");
        }
        #endregion

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