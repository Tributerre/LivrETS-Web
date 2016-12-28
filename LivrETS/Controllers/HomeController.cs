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
using System.Configuration;
using System.Net.Mail;
using System.Net;
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
            string searchString, double Pmin = 1, double Pmax = 1000, int? page = 1)
        {
            ViewBag.CurrentSort = sortOrder;
            IEnumerable<Offer> offers = null;

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            
            ViewBag.CurrentFilter = searchString;
            offers = Repository.GetAllOffers(Pmin, Pmax, select_search, searchString, sortOrder);

            int pageSize = int.Parse(ConfigurationManager.AppSettings["MAX_HOME_PAGE"]);
            int pageNumber = (page ?? 1);

            return View(offers.ToList().ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult sendMailForOffer(string to_name, string to_message, string to_address, string to_offer)
        {
            Offer offer = Repository.GetOfferBy(to_offer);
            ApplicationUser userOffer = Repository.GetOfferByUser(offer);

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTP_CLIENT"]);
            mail.From = new MailAddress(userOffer.Email);
            mail.Subject = "Un utilisateur est intéssé par votre article ";

            mail.IsBodyHtml = true;

            string header_mail = "<div style='background:#629c49;padding:3px 10px;color:black;'>" +
                                    "<div style='float:left;'><h1>TRIBUTERRE</h1></div>" +
                                    "<div style='margin-left:70%;'><h1 style='color:white;'>" +
                                    "Notification de LivrÈTS</h1></div></div></div>";
            string footer_mail = "<br><div style='background:#629c49;padding:3px 10px;color:black;'>" +
                                   "<h1>MERCI</h1></div>";
            string footer_message = "<div><a href='/Offer/Details/" + offer.Id + "'>Article concernée</a></div>";
            string infos_article = "<div><ul>" +
                                    "<li><b>Titre: </b> " + offer.Title + "</li>" +
                                    "<li><b>Cours: </b> " + offer.Article.Course.Acronym + "</li>" +
                                    "<li><b>Poster le: </b> " + offer.StartDate + "</li>" +
                                    "<li><b>Prix: </b> " + offer.Price.ToString("00.00") + "</li>" +
                                    "</ul></div>";

            //mail.Body = header_mail + message + infos_article + footer_message + footer_mail;

            mail.Body = string.Format("<div>{0}<div><div>{1}</div>{2}{3}</div>{4}" +
                "<div>", header_mail, to_message, infos_article, footer_message, footer_mail);

            SmtpServer.Port = Int32.Parse(ConfigurationManager.AppSettings["EMAIL_PORT"]);
            SmtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EMAIL_USERNAME"],
                ConfigurationManager.AppSettings["EMAIL_PWD"]);
            SmtpServer.EnableSsl = true;

            mail.To.Add(to_address);

            SmtpServer.Send(mail);

            return RedirectToAction("Details", "Offer", new { id = offer.Id });
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