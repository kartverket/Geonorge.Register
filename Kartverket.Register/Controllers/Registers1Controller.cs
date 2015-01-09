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
    public class Registers1Controller : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: Registers1
        public ActionResult Index()
        {
            var registers = db.Registers.Include(r => r.manager).Include(r => r.owner).Include(r => r.status);
            return View(registers.ToList());
        }

        // GET: Registers1/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Register register = db.Registers.Find(id);
            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        // GET: Registers1/Create
        public ActionResult Create()
        {
            ViewBag.managerId = new SelectList(db.RegisterItems, "systemId", "name");
            ViewBag.ownerId = new SelectList(db.RegisterItems, "systemId", "name");
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
            return View();
        }

        // POST: Registers1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "systemId,ownerId,managerId,name,description,statusId,dateSubmitted,modified,dateAccepted,containedItemClass")] Register register)
        {
            if (ModelState.IsValid)
            {
                register.systemId = Guid.NewGuid();
                db.Registers.Add(register);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.managerId = new SelectList(db.RegisterItems, "systemId", "name", register.managerId);
            ViewBag.ownerId = new SelectList(db.RegisterItems, "systemId", "name", register.ownerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", register.statusId);
            return View(register);
        }

        // GET: Registers1/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Register register = db.Registers.Find(id);
            if (register == null)
            {
                return HttpNotFound();
            }
            ViewBag.managerId = new SelectList(db.RegisterItems, "systemId", "name", register.managerId);
            ViewBag.ownerId = new SelectList(db.RegisterItems, "systemId", "name", register.ownerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", register.statusId);
            return View(register);
        }

        // POST: Registers1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "systemId,ownerId,managerId,name,description,statusId,dateSubmitted,modified,dateAccepted,containedItemClass")] Register register)
        {
            if (ModelState.IsValid)
            {
                db.Entry(register).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.managerId = new SelectList(db.RegisterItems, "systemId", "name", register.managerId);
            ViewBag.ownerId = new SelectList(db.RegisterItems, "systemId", "name", register.ownerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", register.statusId);
            return View(register);
        }

        // GET: Registers1/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Register register = db.Registers.Find(id);
            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        // POST: Registers1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Register register = db.Registers.Find(id);
            db.Registers.Remove(register);
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
