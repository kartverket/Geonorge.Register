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

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("register/{registerId}/dokument/ny")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Document document, string registerId, HttpPostedFileBase documentfile)
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

                if (document.name == null || document.name.Length < 0)
                {
                    document.name = "ikke angitt";
                }
                if (document.description == null || document.description.Length < 0)
                {
                    document.description = "ikke angitt";
                }
                //if (document.documentUrl == null || document.documentUrl.Length < 0) 
                //{
                //    document.documentUrl = "ikke angitt";
                //}
                if (documentfile != null)
                {
                    document.documentUrl = SaveFileToDisk(documentfile, document.name);
                }
                

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
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Organization.DataDirectory), filename);
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
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", document.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations, "submitterId", "name", document.submitterId);
            ViewBag.documentownerId = new SelectList(db.Organizations, "submitterId", "name", document.documentownerId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("dokument/rediger/{name}/{id}")]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Document document)
        {
            if (ModelState.IsValid)
            {
                document.modified = DateTime.Now;
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
            }
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", document.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations, "submitterId", "name", document.submitterId);
            ViewBag.documentownerId = new SelectList(db.Organizations, "submitterId", "name", document.documentownerId);

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
