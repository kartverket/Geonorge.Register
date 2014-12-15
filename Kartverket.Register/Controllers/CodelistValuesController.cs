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
    public class CodelistValuesController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: CodelistValues
        public ActionResult Index()
        {
            var registerItems = db.CodelistValues.Include(c => c.register).Include(c => c.status).Include(c => c.submitter);
            return View(registerItems.ToList());
        }

        // GET: CodelistValues/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodelistValue codelistValue = db.CodelistValues.Find(id);
            if (codelistValue == null)
            {
                return HttpNotFound();
            }
            return View(codelistValue);
        }

        // GET: CodelistValues/Create
        public ActionResult Create()
        {
            ViewBag.registerId = new SelectList(db.Registers, "systemId", "name");
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name");
            return View();
        }

        // POST: CodelistValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "systemId,name,description,submitterId,dateSubmitted,modified,statusId,dateAccepted,registerId,value")] CodelistValue codelistValue)
        {
            if (ModelState.IsValid)
            {
                codelistValue.systemId = Guid.NewGuid();
                db.RegisterItems.Add(codelistValue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", codelistValue.registerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", codelistValue.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", codelistValue.submitterId);
            return View(codelistValue);
        }

        // GET: CodelistValues/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodelistValue codelistValue = db.CodelistValues.Find(id);
            if (codelistValue == null)
            {
                return HttpNotFound();
            }
            ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", codelistValue.registerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", codelistValue.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", codelistValue.submitterId);
            return View(codelistValue);
        }

        // POST: CodelistValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "systemId,name,description,submitterId,dateSubmitted,modified,statusId,dateAccepted,registerId,value")] CodelistValue codelistValue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(codelistValue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", codelistValue.registerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", codelistValue.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", codelistValue.submitterId);
            return View(codelistValue);
        }

        // GET: CodelistValues/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodelistValue codelistValue = db.CodelistValues.Find(id);
            if (codelistValue == null)
            {
                return HttpNotFound();
            }
            return View(codelistValue);
        }

        // POST: CodelistValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CodelistValue codelistValue = db.CodelistValues.Find(id);
            db.RegisterItems.Remove(codelistValue);
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
