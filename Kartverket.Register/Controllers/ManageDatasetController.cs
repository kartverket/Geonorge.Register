//using System;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web.Mvc;
//using Kartverket.DOK.Models;
//using Kartverket.DOK.Service;

//namespace Kartverket.DOK.Controllers
//{
//    [Authorize]
//    public class ManageDatasetController : Controller
//    {
//        private Kartverket.Register.Models.RegisterDbContext db = new Kartverket.Register.Models.RegisterDbContext();

//        // GET: Dataset
//        public ActionResult Index()
//        {
//            var dataset = db.DokDatasets.Include(d => d.ThemeGroup).Include(s => s.status);
//            return View(dataset.ToList());
//        }

//        // GET: Dataset/Create
//        public ActionResult Create()
//        {
//            ViewBag.ThemeGroupId = new SelectList(db.ThemeGroup, "Id", "Name");
//            ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
//            return View();
//        }

//        // POST: Dataset/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(DokDataset dataset)
//        {
//            if (ModelState.IsValid)
//            {
//                db.DokDatasets.Add(dataset);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.ThemeGroupId = new SelectList(db.ThemeGroup, "Id", "Name", dataset.ThemeGroupId);
//            ViewBag.statusId = new SelectList(db.Statuses, "value", "description",dataset.statusId);
//            return View(dataset);
//        }

//        // GET: Dataset/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            DokDataset dataset = db.DokDatasets.Find(id);
//            if (dataset == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.ThemeGroupId = new SelectList(db.ThemeGroup, "Id", "Name", dataset.ThemeGroupId);
//            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", dataset.statusId);
//            return View(dataset);
//        }

//        // POST: Dataset/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(DokDataset dataset)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(dataset).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.ThemeGroupId = new SelectList(db.ThemeGroup, "Id", "Name", dataset.ThemeGroupId);
//            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", dataset.statusId);
//            return View(dataset);
//        }

//        // GET: Dataset/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            DokDataset dataset = db.DokDatasets.Find(id);
//            if (dataset == null)
//            {
//                return HttpNotFound();
//            }
//            return View(dataset);
//        }

//        // POST: Dataset/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            DokDataset dataset = db.DokDatasets.Find(id);
//            db.DokDatasets.Remove(dataset);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        public ActionResult CreateFromMetadata(string uuid)
//        {
//            var model = new DokDataset();
//            try
//            {
//                new MetadataService().UpdateDatasetWithMetadata(model, uuid);
//            }
//            catch (Exception e)
//            {
//                TempData["error"] = "Det oppstod en feil ved henting av metadata: " + e.Message;
//            }

//            ViewBag.ThemeGroupId = new SelectList(db.ThemeGroup, "Id", "Name");
//            ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
//            return View("Create", model);
//        }

//        public ActionResult UpdateFromMetadata(int id, string uuid, bool dontUpdateDescription)
//        {
//            var model = db.DokDatasets.Find(id);

//            string originalDescription = model.Description;
            
//            try
//            {
//                new MetadataService().UpdateDatasetWithMetadata(model, uuid);
//            }
//            catch (Exception e)
//            {
//                TempData["error"] = "Det oppstod en feil ved henting av metadata: " + e.Message;
//            }

//            if (dontUpdateDescription) model.Description = originalDescription;

//            ViewBag.ThemeGroupId = new SelectList(db.ThemeGroup, "Id", "Name");
//            ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
//            return View("Edit", model);
//        }
//    }
//}
