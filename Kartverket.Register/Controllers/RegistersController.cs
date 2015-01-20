using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System.Text.RegularExpressions;

namespace Kartverket.Register.Controllers
{
    public class RegistersController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();


        // GET: Registers
        public ActionResult Index()
        {
            setAccessRole();
            return View(db.Registers.ToList());
        }


        // GET: Registers/Details/5
        [Route("register/{name}")]
        public ActionResult Details(string name)
        {

            var queryResults = from o in db.Registers
                               where o.seoname == name
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);

            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        [Route("organisasjoner/{name}")]
        public ActionResult DetailsOrganization(string name)
        {

            var queryResults = from o in db.Organizations
                               where o.seoname == name
                               select o.systemId;

            Guid systID = queryResults.First();

            Kartverket.Register.Models.Organization organization = db.Organizations.Find(systID);

            if (organization == null)
            {
                return HttpNotFound();
            }

            return View(organization);
        }


        [Route("{registername}/{documentowner}/{documentname}/")]
        public ActionResult DetailsDocument(string registername, string documentname)
        {

            var queryResultsRegisterId = from o in db.Documents
                               where o.seoname == documentname
                               select o.systemId;

            Guid systId = queryResultsRegisterId.First();
            
            Kartverket.Register.Models.Document document = db.Documents.Find(systId);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        [Route("epsg-koder/{name}")]
        public ActionResult DetailsEPSG(string name)
        {
            
            var queryResults = from o in db.EPSGs
                               where o.seoname == name
                               select o.systemId;

            Guid systID = queryResults.First();

            Kartverket.Register.Models.EPSG epsg = db.EPSGs.Find(systID);
            if (epsg == null)
            {
                return HttpNotFound();
            }
            return View(epsg);
        }

        // GET: Registers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Registers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "systemId,name,description,dateSubmitted,modified,dateAccepted,containedItemClass")] Kartverket.Register.Models.Register register)
        {
            if (ModelState.IsValid)
            {
                register.systemId = Guid.NewGuid();
                db.Registers.Add(register);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(register);
        }

        // GET: Registers/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kartverket.Register.Models.Register register = db.Registers.Find(id);
            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);

        }



        // POST: Registers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        public ActionResult Edit([Bind(Include = "systemId,name,description,dateSubmitted,modified,dateAccepted,containedItemClass,url")] Kartverket.Register.Models.Register register)
        {
            if (ModelState.IsValid)
            {
                db.Entry(register).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(register);
        }

        // GET: Registers/Delete/5
        public ActionResult Delete(Guid? id, string name)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kartverket.Register.Models.Register register = db.Registers.Find(id);
            if (name == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        //// POST: Registers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    Kartverket.Register.Models.Register register = db.Registers.Find(id);
        //    db.Registers.Remove(register);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HasAccessToRegister()
        {
            string organization = GetSecurityClaim("organization");
            string role = GetSecurityClaim("role");
            bool isAdmin = !string.IsNullOrWhiteSpace(role) && role.Equals("nd.metadata_admin");
            return isAdmin;
        }

        private string GetSecurityClaim(string type)
        {
            string result = null;
            foreach (var claim in System.Security.Claims.ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    result = claim.Value;
                    break;
                }
            }

            // bad hack, must fix BAAT
            if (!string.IsNullOrWhiteSpace(result) && type.Equals("organization") && result.Equals("Statens kartverk"))
            {
                result = "Kartverket";
            }

            return result;
        }

        private void setAccessRole()
        {
            bool editor = HasAccessToRegister();
            if (editor)
                Session["role"] = "editor";
            else
                Session["role"] = "guest";
        }

    }
}
