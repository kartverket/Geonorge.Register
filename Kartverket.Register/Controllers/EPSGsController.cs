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
    public class EPSGsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: EPSGs
        public ActionResult Index()
        {
            var registerItems = db.EPSGs.Include(e => e.register).Include(e => e.status).Include(e => e.submitter).Include(e => e.inspireRequirement).Include(e => e.nationalRequirement).Include(e => e.nationalSeasRequirement);
            return View(registerItems.ToList());
        }

        // GET: EPSGs/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EPSG ePSG = db.EPSGs.Find(id);
            if (ePSG == null)
            {
                return HttpNotFound();
            }
            return View(ePSG);
        }

        // GET: EPSGs/Create
        [Route("register/{registerId}/epsg/ny")]
        public ActionResult Create(string registerId)
        {
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name");
            //ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
            //ViewBag.submitterId = new SelectList(db.Organizations, "systemId", "name");
            //ViewBag.inspireRequirementId = new SelectList(db.requirements, "value", "description");
            //ViewBag.nationalRequirementId = new SelectList(db.requirements, "value", "description");
            //ViewBag.nationalSeasRequirementId = new SelectList(db.requirements, "value", "description");
            return View();
        }

        // POST: EPSGs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("register/{registerId}/epsg/ny")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(EPSG epsg, string registerId)
        {           

            if (ModelState.IsValid)
            {

                epsg.systemId = Guid.NewGuid();
                epsg.modified = DateTime.Now;
                epsg.dateSubmitted = DateTime.Now;
                epsg.registerId = Guid.Parse(registerId);
                epsg.statusId = "Submitted";
                epsg.submitter = null;
                epsg.inspireRequirementId = "Optional";
                epsg.nationalRequirementId = "Optional";
                epsg.nationalSeasRequirementId = "Optional";

                db.RegisterItems.Add(epsg);
                db.SaveChanges();

            }

            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", ePSG.registerId);
            //ViewBag.statusId = new SelectList(db.Statuses, "value", "description", ePSG.statusId);
            //ViewBag.submitterId = new SelectList(db.Organizations, "systemId", "name", ePSG.submitterId);
            //ViewBag.inspireRequirementId = new SelectList(db.requirements, "value", "description", ePSG.inspireRequirementId);
            //ViewBag.nationalRequirementId = new SelectList(db.requirements, "value", "description", ePSG.nationalRequirementId);
            //ViewBag.nationalSeasRequirementId = new SelectList(db.requirements, "value", "description", ePSG.nationalSeasRequirementId);
            
            return Redirect("/register/epsg/" + epsg.registerId);
            //return View(epsg);
        }

        // GET: EPSGs/Edit/5
        [Route("epsg/rediger/{name}/{id}")]
        public ActionResult Edit(Guid? id, string name)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EPSG ePSG = db.EPSGs.Find(id);
            if (ePSG == null)
            {
                return HttpNotFound();
            }
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", ePSG.registerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", ePSG.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations, "systemId", "name", ePSG.submitterId);
            ViewBag.inspireRequirementId = new SelectList(db.requirements, "value", "description", ePSG.inspireRequirementId);
            ViewBag.nationalRequirementId = new SelectList(db.requirements, "value", "description", ePSG.nationalRequirementId);
            ViewBag.nationalSeasRequirementId = new SelectList(db.requirements, "value", "description", ePSG.nationalSeasRequirementId);
            return View(ePSG);
        }

        // POST: EPSGs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("epsg/rediger/{name}/{id}")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(EPSG ePSG, string name, string id)
        public ActionResult Edit(EPSG ePSG, string name, string id)
        {
            if (ModelState.IsValid)
            {              
                ePSG.modified = DateTime.Now;
                
                db.Entry(ePSG).State = EntityState.Modified;
                db.SaveChanges();

            }
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", ePSG.registerId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", ePSG.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations, "systemId", "name", ePSG.submitterId);
            ViewBag.inspireRequirementId = new SelectList(db.requirements, "value", "description", ePSG.inspireRequirementId);
            ViewBag.nationalRequirementId = new SelectList(db.requirements, "value", "description", ePSG.nationalRequirementId);
            ViewBag.nationalSeasRequirementId = new SelectList(db.requirements, "value", "description", ePSG.nationalSeasRequirementId);

            return Redirect("/register/epsg/" + ePSG.registerId);
            //return View(ePSG);
        }

        // GET: EPSGs/Delete/5
        [Route("epsg/slett/{name}/{id}")]
        public ActionResult Delete(Guid? id, string name)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EPSG ePSG = db.EPSGs.Find(id);
            if (ePSG == null)
            {
                return HttpNotFound();
            }
            return View(ePSG);
        }

        // POST: EPSGs/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("epsg/slett/{name}/{id}")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id, string name)
        {
            EPSG ePSG = db.EPSGs.Find(id);
            db.RegisterItems.Remove(ePSG);
            db.SaveChanges();
            return Redirect("/register/epsg/" + ePSG.registerId);
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
