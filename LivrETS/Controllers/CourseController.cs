using LivrETS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LivrETS.Controllers
{
    public class CourseController : Controller
    {
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

        public CourseController(LivrETSRepository livretsRepository)
        {
            Repository = livretsRepository;
        }

        public CourseController() { }
        // GET: Course
        public ActionResult Index()
        {
            return View();
        }

        // GET: Course/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Course/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Course/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Course/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Course/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Course/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Course/Delete/5
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

        #region AJAX

        /// <summary>
        /// Delete course
        /// </summary>
        /// <param name="offerIds">Offer Id array</param>
        /// <param name="type">Define the choice between Disable and delete offer</param>
        [HttpPost]
        public ActionResult DeleteCourse(string[] acronym)
        {
            bool status = false;

            if (acronym != null)
                status = Repository.DeleteCourse(acronym);

            return Json(new
            {
                status = status,
                message = (!status) ? "Erreur lors de la suppression" :
                            "cours supprimé",
        }, contentType: "application/json");
        }

        #endregion
    }
}
