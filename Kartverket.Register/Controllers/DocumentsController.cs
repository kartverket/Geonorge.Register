using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;

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
        public ActionResult Create()
        {
            ViewBag.registerId = new SelectList(db.Registers, "systemId", "name");
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
            ViewBag.submitterId = new SelectList(db.RegisterItems, "systemId", "name");
            ViewBag.documentownerId = new SelectList(db.RegisterItems, "systemId", "name");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "systemId,name,description,submitterId,dateSubmitted,modified,statusId,dateAccepted,registerId,thumbnail,documentownerId,document")] Document document)
        {
            if (ModelState.IsValid)
            {
                document.systemId = Guid.NewGuid();
                db.RegisterItems.Add(document);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", document.statusId);
            ViewBag.submitterId = new SelectList(db.RegisterItems, "systemId", "name", document.submitterId);
            ViewBag.documentownerId = new SelectList(db.RegisterItems, "systemId", "name", document.documentownerId);
            return View(document);
        }

        // GET: Documents/Edit/5
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
            ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", document.statusId);
            ViewBag.submitterId = new SelectList(db.RegisterItems, "systemId", "name", document.submitterId);
            ViewBag.documentownerId = new SelectList(db.RegisterItems, "systemId", "name", document.documentownerId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "systemId,name,description,submitterId,dateSubmitted,modified,statusId,dateAccepted,registerId,thumbnail,documentownerId,document")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", document.statusId);
            ViewBag.submitterId = new SelectList(db.RegisterItems, "systemId", "name", document.submitterId);
            ViewBag.documentownerId = new SelectList(db.RegisterItems, "systemId", "name", document.documentownerId);
            return View(document);
        }

        // GET: Documents/Delete/5
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
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Document document = db.Documents.Find(id);
            db.RegisterItems.Remove(document);
            db.SaveChanges();
            return RedirectToAction("Index");
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
