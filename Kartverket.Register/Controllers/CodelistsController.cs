using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kartverket.Register.Controllers
{
    public class CodelistsController : Controller
    {
        // GET: Codelists
        public ActionResult Index()
        {
            return View();
        }

        [Route("Codelists/{kodeliste}")]
        public ActionResult Values(string kodeliste)
        {
            return View();
        }

        [Route("Codelists/{kodeliste}/{id}")]
        public ActionResult ValueDetails(string kodeliste, string id)
        {
            return View();
        }
    }
}