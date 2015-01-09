using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System.IO;

namespace Kartverket.Register.Controllers
{
    public class DocumentsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: Documents
        public ActionResult Index()
        {
            var registerItems = db.Documents.Include(d => d.register).Include(d => d.status).Include(d => d.submitter).Include(d => d.documentowner);
            return View(registerItems.ToList());
        }

        // GET: Documents/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Documents/Create
        [Route("register/{registerId}/dokument/ny")]
        public ActionResult Create(string registerId)
        {
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name");
            //ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
            //ViewBag.submitterId = new SelectList(db.Organizations, "systemId", "name");
            //ViewBag.documentownerId = new SelectList(db.Organizations, "systemId", "name");
            return View();
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
                
        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("register/{registerId}/dokument/ny")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Document document, string registerId, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail)
        {
            if (ModelState.IsValid)
            {
                document.systemId = Guid.NewGuid();
                document.modified = DateTime.Now;
                document.dateSubmitted = DateTime.Now;
                document.registerId = Guid.Parse(registerId);
                document.statusId = "Submitted";
                document.submitter = null;
                document.documentowner = null;
                document.documentownerId = null;

                if (document.name == null || document.name.Length == 0)
                {
                    document.name = "ikke angitt";
                }
                if (document.description == null || document.description.Length == 0)
                {
                    document.description = "ikke angitt";
                }

                string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;
                if (documentfile != null)
                {
                    document.documentUrl = url + SaveFileToDisk(documentfile, document.name);
                }

                if (thumbnail != null)
                {
                    document.thumbnail = url + SaveFileToDisk(thumbnail, document.name);
                }

                string organizationLogin = GetSecurityClaim("organization");

                var queryResults = from o in db.Organizations
                                   where o.name == organizationLogin
                                   select o.systemId;

                Guid orgId = queryResults.First();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);
                
                document.submitterId = orgId;
                document.submitter = submitterOrganisasjon;
                document.documentowner = submitterOrganisasjon;
                document.documentownerId = orgId;

                db.Entry(document).State = EntityState.Modified;
                //db.SaveChanges();

                db.RegisterItems.Add(document);
                db.SaveChanges();

                return Redirect("/register/dokument/" + document.registerId);
                //return RedirectToAction("Index");
            }

            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            //ViewBag.statusId = new SelectList(db.Statuses, "value", "description", document.statusId);
            //ViewBag.submitterId = new SelectList(db.Organizations, "systemId", "name", document.submitterId);
            //ViewBag.documentownerId = new SelectList(db.Organizations, "systemId", "name", document.documentownerId);
            return View(document);
        }

        private string SaveFileToDisk(HttpPostedFileBase file, string submitter)
        {
            string filename = submitter + "_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Document.DataDirectory), filename);
            file.SaveAs(path);
            return filename;
        }

        // GET: Documents/Edit/5
        [Route("dokument/rediger/{name}/{id}")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", document.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", document.submitterId);
            ViewBag.documentownerId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", document.documentownerId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("dokument/rediger/{name}/{id}")]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Document document, string name, string id, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail)
        {
            if (ModelState.IsValid)
            {
                Document originalDocument = db.Documents.Find(Guid.Parse(id));
                if (document.name != null) originalDocument.name = document.name;
                if (document.description != null) originalDocument.description = document.description;
                if (document.documentownerId != null) originalDocument.documentownerId = document.documentownerId;
                if (document.documentUrl != null) originalDocument.documentUrl = document.documentUrl;
                if (document.statusId != null) originalDocument.statusId = document.statusId;
                if (document.submitterId != null) originalDocument.submitterId = document.submitterId;

                string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;
                if (documentfile != null)
                {
                    originalDocument.documentUrl = url + SaveFileToDisk(documentfile, originalDocument.name);
                }

                if (thumbnail != null)
                {
                    originalDocument.thumbnail = url + SaveFileToDisk(thumbnail, originalDocument.name);
                }


                originalDocument.modified = DateTime.Now;
                db.Entry(originalDocument).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", originalDocument.statusId);
                ViewBag.submitterId = new SelectList(db.Organizations, "systemId", "name", originalDocument.submitterId);
                ViewBag.documentownerId = new SelectList(db.Organizations, "systemId", "name", originalDocument.documentownerId);
            }
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);

            return Redirect("/register/dokument/" + document.registerId);
            //return View(document);
        }

        // GET: Documents/Delete/5
        [Route("dokument/slett/{name}/{id}")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("dokument/slett/{name}/{id}")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Document document = db.Documents.Find(id);
            db.RegisterItems.Remove(document);
            db.SaveChanges();
            return Redirect("/register/dokument/" + document.registerId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
