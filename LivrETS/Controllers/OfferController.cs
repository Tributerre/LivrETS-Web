using LivrETS.Models;
using LivrETS.Repositories;
using LivrETS.Service.IO;
using LivrETS.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace LivrETS.Controllers
{
    public class OfferController : Controller
    {
        private const string UPLOADS_PATH = "~/Content/Uploads";

        private LivrETSRepository _repository;
        public LivrETSRepository Repository
        {
            get
            {
                if (_repository == null)
                    _repository = new LivrETSRepository();

                return _repository;
            }
            private set
            {
                _repository = value;
            }
        }

        public OfferController(LivrETSRepository livretsRepository)
        {
            Repository = livretsRepository;
        }

        public OfferController() { }

        // GET: Offer
        public ActionResult Index()
        {
            return View();
        }

        // GET: Offer/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
                throw new HttpException(404, "Page not Found");

            Offer offer = Repository.GetOfferBy(id);

            if (offer == null)
                throw new HttpException(404, "Page not Found");

            return View(offer);
        }

        // GET: Offer/Create
        public ActionResult Create()
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

        // POST: Offer/Create
        [HttpPost]
        public ActionResult Create(ArticleViewModel model)
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

                return RedirectToAction("Index", "Home");
            }
            else
            {
                model.Courses = Repository.GetAllCourses().ToList();
                return View(model);
            }
        }

        // GET: Offer/Edit/5
        public ActionResult Edit(string id)
        {
            /*Offer offer = Repository.GetOfferBy(id);
            var model = new ArticleViewModel() {
                Acronym = offer.Article.Course.Acronym,
                Condition = offer.Condition,
                Title = offer.Title,
                Price = offer.Price,
                // je ne peux pas continuer ici car, je n'ai pas 
                //trouver comment acceder au code ISBN
                //je chercherais plus tard 
                Type = offer.Article.TypeName
            };

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


            Session["images"] = null;
            model.Courses = Repository.GetAllCourses().ToList();
            ThreadPool.QueueUserWorkItem(state =>
            {
                var arguments = state as Tuple<string, string>;
                FileSystemFacade.CleanTempFolder(uploadsPath: arguments.Item1, userId: arguments.Item2);
            }, state: new Tuple<string, string>(Server.MapPath(UPLOADS_PATH), User.Identity.GetUserId()));

            return View(model);*/
            return View();
        }

        // POST: Offer/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: Offer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Offer/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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

        #region AJAX

        // DELETE: /Offer/DeleteOffer
        // Deletes one or more offers.
        [HttpDelete]
        public ActionResult DeleteOffer(string[] offerIds)
        {
            if (offerIds == null)
                return Json(new { status = false, message = "no data source" }, contentType: "application/json");

            Repository.DeleteOffer(offerIds);

            return Json(new { status = true, message = "data delete" }, contentType: "application/json");
        }

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
    }
}
