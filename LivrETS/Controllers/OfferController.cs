using LivrETS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LivrETS.Controllers
{
    public class OfferController : Controller
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

        #endregion
    }
}