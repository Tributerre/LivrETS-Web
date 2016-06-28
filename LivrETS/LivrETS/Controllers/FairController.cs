using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LivrETS.Controllers
{
    [Authorize(Roles = "Administrator,Clerk")]
    public class FairController : Controller
    {
        // GET: /Fair/Pick
        [HttpGet]
        public ActionResult Pick()
        {
            return View();
        }
    }
}