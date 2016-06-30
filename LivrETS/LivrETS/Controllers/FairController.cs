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
using LivrETS.Repositories;
using LivrETS.Models;

namespace LivrETS.Controllers
{
    [Authorize(Roles = "Administrator,Clerk")]
    public class FairController : Controller
    {
        private const int NUMBER_OF_PHASES = 2;
        private const string CURRENT_PHASE = "CurrentPhase";
        private const string SELLER = "Seller";

        private LivrETSRepository _repository;
        public LivrETSRepository Repository
        {
            get
            {
                if (_repository == null)
                    _repository = new LivrETSRepository();

                return _repository;
            }
        }

        // GET: /Fair/Pick
        [HttpGet]
        public ActionResult Pick()
        {
            var model = new FairViewModel();
            var currentFair = Repository.GetCurrentFair();
            var currentPhase = 0f;

            model.FairYear = currentFair?.StartDate.Year ?? 0;
            model.CurrentPhase = currentPhase;
            Session[CURRENT_PHASE] = currentPhase;
            model.NumberOfPhases = NUMBER_OF_PHASES;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pick(FairViewModel model)
        {
            if (Session[CURRENT_PHASE] == null)
                return RedirectToAction(nameof(Pick));

            if (ModelState.IsValid)
            {
                int currentPhase = (int)Session[CURRENT_PHASE];

                switch (currentPhase)
                {
                    case 0:
                        Session[SELLER] = Repository.GetUserBy(BarCode: model.UserBarCode);
                        Session[CURRENT_PHASE] = 1;
                        model.CurrentPhase = 1;
                        break;

                    case 1:
                        break;

                    case 2:
                        return RedirectToAction(nameof(Pick));
                }
            }

            return View(model);
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