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
using LivrETS.Service;
using System.Net;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Web.Script.Serialization;

namespace LivrETS.Controllers
{
    [Authorize(Roles = "Administrator,Clerk")]
    public class FairController : Controller
    {
        private const int NUMBER_OF_STEPS = 2;
        private const int MAX_NUMBER_OF_STICKERS_PER_SHEET = 14;
        private const string CURRENT_STEP = "CurrentPhase";
        private const string SELLER = "Seller";
        private const string SELLER_PICKED_ARTICLES = "SellerPickedArticles";
        private const string LAST_NUMBER_OF_STICKERS_LEFT_ON_SHEET = "LastNumberOfStickersLeftOnSheet";

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
            var currentStep = 0;

            model.Fair            = currentFair;
            model.CurrentStep     = currentStep;
            model.NumberOfPhases  = NUMBER_OF_STEPS;
            Session[CURRENT_STEP] = currentStep;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pick(FairViewModel model)
        {
            if (Session[CURRENT_STEP] == null)
                return RedirectToAction(nameof(Pick));

            int currentStep = (int)Session[CURRENT_STEP];
            Fair currentFair = Repository.GetCurrentFair();

            model.Fair = currentFair;
            model.NumberOfPhases = NUMBER_OF_STEPS;

            if (string.IsNullOrEmpty(model.UserBarCode) && currentStep == 0)
            {
                ModelState.AddModelError(nameof(FairViewModel.UserBarCode), "Veuillez indiquez le code à barres du vendeur.");
            }

            if (ModelState.IsValid)
            {
                switch (currentStep)
                {
                    case 0:
                        ApplicationUser seller = Repository.GetUserBy(BarCode: model.UserBarCode.Trim().ToUpper());

                        if (seller == null) return RedirectToAction(nameof(Pick));

                        Session[CURRENT_STEP] = 1;
                        Session[SELLER]       = seller.Id;
                        model.CurrentStep     = 1;
                        model.User            = seller;
                        model.UserOffers      = seller.Offers;
                        model.FairOffers      = currentFair.Offers;
                        break;

                    case 1:
                        if (Session[SELLER] == null) return RedirectToAction(nameof(Pick));
                        ApplicationUser seller1 = Repository.GetUserBy(Id: Session[SELLER] as string);
                        Session[SELLER_PICKED_ARTICLES] = 
                            currentFair.Offers.Intersect(seller1.Offers)
                            .Where(offer => offer.Article.FairState == ArticleFairState.PICKED)
                            .ToList()
                            .ConvertAll(new Converter<Offer, string>(offer => offer.Id.ToString()));

                        Session[CURRENT_STEP] = 2;
                        model.CurrentStep     = 2;
                        model.User            = seller1;
                        break;

                    case 2:
                        return RedirectToAction(nameof(Pick));
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Sell()
        {
            SellViewModel model = new SellViewModel()
            {
                Fair = Repository.GetCurrentFair()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Retrieve()
        {
            var model = new RetrieveViewModel()
            {
                Fair = Repository.GetCurrentFair()
            };

            return View(model);
        }

        public ActionResult Program()
        {
            Fair curentFair = Repository.GetCurrentFair();
            Fair nextFair = Repository.GetNextFair();

            return View((curentFair != null) ? curentFair : nextFair);
                
        }

        #region Ajax
        
        [HttpPost]
        public ActionResult CheckStatusFair()
        {

            return Json(new
            {
                status = Fair.CheckStatusFair()
            },contentType: "application/json"
            );
        }

        [HttpPost]
        public ActionResult OffersNotSold(string UserBarCode)
        {
            var barCode = UserBarCode.ToUpper().Trim();
            var user = Repository.GetUserBy(BarCode: barCode);
            var currentFair = Repository.GetCurrentFair();

            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var offersNotSold = currentFair.Offers
                .Where(offer => !offer.Sold)
                .Intersect(user.Offers);

            return Json(
                from offer in offersNotSold
                select new
                {
                    id = TRIBSTD01Helper.LivrETSIDOf(user: user, article: offer.Article, fair: currentFair),
                    title = offer.Article.Title,
                    userFullName = user.FullName,
                    price = offer.Price
                },
                contentType: "application/json"
            );
        }

        [HttpPost]
        public ActionResult RetrieveArticles(ICollection<string> ids)
        {
            foreach (var id in ids)
            {
                TRIBSTD01Helper helper;
                try
                {
                    helper = new TRIBSTD01Helper(id.ToUpper().Trim());
                }
                catch (RegexNoMatchException ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }

                var offer = helper.GetOffer();
                Repository.AttachToContext(offer);
                offer.Article.MarkAsRetrieved();
                Repository.Update();

                //send notification mail
                NotificationManager.getInstance().sendNotification(
                    new Notification(NotificationOptions.ARTICLERETREIVEDCONFIRMATION,
                                Repository.GetOfferByUser(offer))
                    );
            }

            return Json(new { }, contentType: "application/json");
        }

        public ActionResult ConcludeSell(ICollection<string> ids)
        {
            bool noMatchExists = false;
            var fair = Repository.GetCurrentFair();
            var seller = Repository.GetUserBy(Id: User.Identity.GetUserId());
            var sale = new Sale()
            {
                Date = DateTime.Now,
            };

            var helpers = ids.ToList().ConvertAll(new Converter<string, TRIBSTD01Helper>(id =>
            {
                TRIBSTD01Helper helper = null;
                try
                {
                    helper = new TRIBSTD01Helper(id);
                }
                catch (RegexNoMatchException ex)
                {
                    noMatchExists = true;
                    return null;
                }

                return helper;
            }));

            if (noMatchExists)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            foreach (var helper in helpers)
            {
                var offer = helper.GetOffer();
                Repository.AttachToContext(offer);
                offer.Article.MarkAsSold();
                offer.MarkedSoldOn = DateTime.Now;
                sale.SaleItems.Add(new SaleItem()
                {
                    Offer = offer
                });

                NotificationManager.getInstance().sendNotification(
                    new Notification(NotificationOptions.ARTICLEMARKEDASSOLDDURINGFAIR,
                        Repository.GetOfferByUser(offer))
                );
            }

            seller.Sales.Add(sale);
            fair.Sales.Add(sale);
            Repository.Update();
            return Json(new { }, contentType: "application/json");
        }

        [HttpPost]
        public ActionResult ConcludeSellTest(ICollection<string> offerIds, string fairId = null)
        {
            bool noMatchExists = false, status=false;
            string message = null;

            Fair fair = (fairId != null)?Repository.GetFairById(fairId):
                Repository.GetCurrentFair();

            if(fair == null)
                return Json(new {
                    status = status,
                    message = "La foire concernée n'a pas été retrouvé"
                }, contentType: "application/json");

            var seller = Repository.GetUserBy(Id: User.Identity.GetUserId());
            var sale = new Sale()
            {
                Date = DateTime.Now,
                Seller = seller,
                Fair = fair
            };

            var helpers = offerIds.ToList().ConvertAll(new Converter<string, TRIBSTD01Helper>(id =>
            {
                TRIBSTD01Helper helper = null;
                
                try
                {
                    Offer offer = Repository.GetOfferBy(id);
                    string LivrETSID = fair.LivrETSID + "-" + seller.LivrETSID + "-" + offer.Article.LivrETSID;
   
                    helper = new TRIBSTD01Helper(LivrETSID);
                }
                catch (RegexNoMatchException ex)
                {
                    noMatchExists = true;
                    return null;
                }

                return helper;
            }));

            if (noMatchExists)
                return Json(new
                {
                    status = status,
                    message = "erreur"
                }, contentType: "application/json");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //foreach (var helper in helpers)
            foreach(string id in offerIds)
            {
                //var currentOffer = helper.GetOffer();
                Offer currentOffer = Repository.GetOfferBy(id);
                Repository.AttachToContext(currentOffer);
                currentOffer.Article.MarkAsSold();
                currentOffer.MarkedSoldOn = DateTime.Now;
                sale.SaleItems.Add(new SaleItem()
                {
                    Offer = currentOffer
                });

                NotificationManager.getInstance().sendNotification(
                    new Notification(NotificationOptions.ARTICLEMARKEDASSOLDDURINGFAIR,
                        Repository.GetOfferByUser(currentOffer))
                );
            }

            seller.Sales.Add(sale);
            fair.Sales.Add(sale);
            Repository.Update();
            status = true;

            return Json(new {
                status = status
            }, contentType: "application/json");
        }

        [HttpPost]
        public ActionResult OfferInfo(string LivrETSID)
        {
            var cleanLivrETSID = LivrETSID.Trim().ToUpper();
            TRIBSTD01Helper helper = null;
            try
            {
                helper = new TRIBSTD01Helper(cleanLivrETSID);
            }
            catch (RegexNoMatchException ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }

            var seller = helper.GetSeller();
            var offer = helper.GetOffer();

            if (offer.Sold)
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            else
                return Json(new
                {
                    id = LivrETSID,
                    sellerFullName = $"{seller.FullName} ({seller.LivrETSID})",
                    articleTitle = offer.Article.Title,
                    offerPrice = offer.Price
                }, contentType: "application/json");
        }

        [HttpPost]
        public ActionResult CalculatePrices(ICollection<string> LivrETSIDs)
        {
            double subtotal = 0;
            double total = 0;
            double commission = 0;

            foreach (var id in LivrETSIDs)
            {
                TRIBSTD01Helper helper;
                try
                {
                    helper = new TRIBSTD01Helper(id);
                }
                catch (RegexNoMatchException ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
                }

                subtotal += helper.GetOffer().Price;
            }

            var currentFair = Repository.GetCurrentFair();
            commission = subtotal * currentFair.CommissionOnSale;
            total = subtotal + commission;

            return Json(new
            {
                commission = commission,
                subtotal = subtotal,
                total = total
            }, contentType: "application/json");
        }

        [HttpPost]
        public ActionResult MarkAsPicked(string ArticleId)
        {
            Article article = Repository.GetArticleBy(Id: ArticleId);

            if (article == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            article.MarkAsPicked();
            Repository.Update();

            //send notification mail
            NotificationManager.getInstance().sendNotification(
                new Notification(NotificationOptions.ARTICLEPICKEDCONFIRMATION, 
                        Repository.GetOfferByUser(article: article))
                );
            return Json(new { }, contentType: "application/json");
        }

        [HttpPost]
        public ActionResult GeneratePreview(int NumberOfStickersLeft)
        {
            if (Session[SELLER_PICKED_ARTICLES] == null || Session[SELLER] == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var offersPicked = (Session[SELLER_PICKED_ARTICLES] as IEnumerable<string>).ToList();
            var currentFair = Repository.GetCurrentFair();
            var seller = Repository.GetUserBy(Id: Session[SELLER] as string);

            // Re-attaching the offers to a context...
            foreach (var offer in offersPicked)
            {
                Repository.AttachToContext(offer);
            }

            var relativePdfPath = PrintManager.GeneratePreview(
                Server.MapPath("~/Content/PDF"),
                NumberOfStickersLeft,
                offersPicked.ConvertAll(new Converter<string, PrintManager.StickerInfo>(offerId => 
                {
                    var offer = Repository.GetOfferBy(Id: offerId);
                    return new PrintManager.StickerInfo()
                    {
                        ArticleLivrETSID = offer.Article.LivrETSID,
                        ArticleTitle = offer.Article.Title,
                        OfferPrice = offer.Price,
                        FairLivrETSID = currentFair.LivrETSID,
                        UserLivrETSID = seller.LivrETSID
                    };
                }))
            );
            Session[LAST_NUMBER_OF_STICKERS_LEFT_ON_SHEET] = NumberOfStickersLeft;

            return Json(new { PdfStickerPath = relativePdfPath }, contentType: "application/json");
        }

        [HttpPost]
        public ActionResult ConfirmPrint()
        {
            if (Session[SELLER_PICKED_ARTICLES] == null || Session[SELLER] == null || 
                Session[LAST_NUMBER_OF_STICKERS_LEFT_ON_SHEET] == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var offersPicked = Session[SELLER_PICKED_ARTICLES] as IEnumerable<string>;
            var lastNumberOfStickersLeft = (int)Session[LAST_NUMBER_OF_STICKERS_LEFT_ON_SHEET];
            int numberOfOffersRemaining = 0;

            if (lastNumberOfStickersLeft >= offersPicked.Count())
            {
                Session[SELLER_PICKED_ARTICLES] = null;
            }
            else
            {
                var remainingOffers = offersPicked.Skip(lastNumberOfStickersLeft);
                numberOfOffersRemaining = remainingOffers.Count();
                Session[SELLER_PICKED_ARTICLES] = remainingOffers;
            }

            return Json(new { RemainingOffersCount = numberOfOffersRemaining }, 
                contentType: "application/json");
        }

        [HttpPost]
        public ActionResult GetTotalSalesAmountByArticleType(string fairId)
        {
            FairStatistics fairStats = new FairStatistics(
                Repository.GetFairById(fairId));

            return Json(fairStats.GetTotalSalesAmountByArticleType().ToArray(), 
                contentType: "application/json");
        }

        [HttpPost]
        public ActionResult GetTotalSalesByArticleType(string fairId)
        {
            FairStatistics fairStats = new FairStatistics(
                Repository.GetFairById(fairId));

            return Json(fairStats.GetTotalSalesByArticleType().ToArray(),
                contentType: "application/json");
        }

        public ActionResult GetTotalSalesBySeller(string fairId)
        {
            Fair fair = Repository.GetFairById(fairId);
            FairStatistics fairStats = new FairStatistics(fair);

            List<TabStatistics> dataList_2 = fairStats.GetTotalSalesBySeller();


            return Json(dataList_2, contentType: "application/json");
        }

        public ActionResult GetTotalSalesByCourse(string fairId)
        {
            Fair fair = Repository.GetFairById(fairId);
            FairStatistics fairStats = new FairStatistics(fair);

            return Json(fairStats.GetTotalSalesByCourse(), 
                contentType: "application/json");
        }

        // POST: /Fair/GetStatsFairs
        [HttpPost]
        public string GetStatsFairs()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<MorrisFairsStatistic> results = new FairStatistics().GetStatsFairs();
            return jss.Serialize(results);
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