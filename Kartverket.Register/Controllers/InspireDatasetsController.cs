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
    public class InspireDatasetsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: InspireDatasets
        public ActionResult Index()
        {
            var inspireDatasets = db.InspireDatasets.Include(i => i.DokStatus).Include(i => i.InspireDeliveryAtomFeed).Include(i => i.InspireDeliveryDistribution).Include(i => i.InspireDeliveryHarmonizedData).Include(i => i.InspireDeliveryMetadata).Include(i => i.InspireDeliveryMetadataService).Include(i => i.InspireDeliverySpatialDataService).Include(i => i.InspireDeliveryWfs).Include(i => i.InspireDeliveryWfsOrAtom).Include(i => i.InspireDeliveryWms).Include(i => i.Owner).Include(i => i.Register).Include(i => i.Status).Include(i => i.Submitter).Include(i => i.Theme);
            return View(inspireDatasets.ToList());
        }

        // GET: InspireDatasets/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspireDataset inspireDataset = db.InspireDatasets.Find(id);
            if (inspireDataset == null)
            {
                return HttpNotFound();
            }
            return View(inspireDataset);
        }

        // GET: InspireDatasets/Create
        public ActionResult Create()
        {
            ViewBag.DokStatusId = new SelectList(db.DokStatuses, "value", "description");
            ViewBag.InspireDeliveryAtomFeedId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId");
            ViewBag.InspireDeliveryDistributionId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId");
            ViewBag.InspireDeliveryHarmonizedDataId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId");
            ViewBag.InspireDeliveryMetadataId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId");
            ViewBag.InspireDeliveryMetadataServiceId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId");
            ViewBag.InspireDeliverySpatialDataServiceId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId");
            ViewBag.InspireDeliveryWfsId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId");
            ViewBag.InspireDeliveryWfsOrAtomId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId");
            ViewBag.InspireDeliveryWmsId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId");
            ViewBag.OwnerId = new SelectList(db.RegisterItems, "systemId", "name");
            ViewBag.RegisterId = new SelectList(db.Registers, "systemId", "name");
            ViewBag.StatusId = new SelectList(db.Statuses, "value", "description");
            ViewBag.SubmitterId = new SelectList(db.RegisterItems, "systemId", "name");
            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description");
            return View();
        }

        // POST: InspireDatasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SystemId,InspireDeliveryMetadataId,InspireDeliveryMetadataServiceId,InspireDeliveryDistributionId,InspireDeliveryWmsId,InspireDeliveryWfsId,InspireDeliveryAtomFeedId,InspireDeliveryWfsOrAtomId,InspireDeliveryHarmonizedDataId,InspireDeliverySpatialDataServiceId,Uuid,Notes,SpecificUsage,ProductSheetUrl,PresentationRulesUrl,ProductSpecificationUrl,MetadataUrl,DistributionFormat,DistributionUrl,DistributionArea,WmsUrl,ThemeGroupId,DatasetThumbnail,DokStatusId,DokStatusDateAccepted,Name,Seoname,Description,SubmitterId,OwnerId,DateSubmitted,Modified,StatusId,RegisterId")] InspireDataset inspireDataset)
        {
            if (ModelState.IsValid)
            {
                inspireDataset.SystemId = Guid.NewGuid();
                db.InspireDatasets.Add(inspireDataset);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DokStatusId = new SelectList(db.DokStatuses, "value", "description", inspireDataset.DokStatusId);
            ViewBag.InspireDeliveryAtomFeedId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryAtomFeedId);
            ViewBag.InspireDeliveryDistributionId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryDistributionId);
            ViewBag.InspireDeliveryHarmonizedDataId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryHarmonizedDataId);
            ViewBag.InspireDeliveryMetadataId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryMetadataId);
            ViewBag.InspireDeliveryMetadataServiceId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryMetadataServiceId);
            ViewBag.InspireDeliverySpatialDataServiceId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliverySpatialDataServiceId);
            ViewBag.InspireDeliveryWfsId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWfsId);
            ViewBag.InspireDeliveryWfsOrAtomId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWfsOrAtomId);
            ViewBag.InspireDeliveryWmsId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWmsId);
            ViewBag.OwnerId = new SelectList(db.RegisterItems, "systemId", "name", inspireDataset.OwnerId);
            ViewBag.RegisterId = new SelectList(db.Registers, "systemId", "name", inspireDataset.RegisterId);
            ViewBag.StatusId = new SelectList(db.Statuses, "value", "description", inspireDataset.StatusId);
            ViewBag.SubmitterId = new SelectList(db.RegisterItems, "systemId", "name", inspireDataset.SubmitterId);
            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", inspireDataset.ThemeGroupId);
            return View(inspireDataset);
        }

        // GET: InspireDatasets/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspireDataset inspireDataset = db.InspireDatasets.Find(id);
            if (inspireDataset == null)
            {
                return HttpNotFound();
            }
            ViewBag.DokStatusId = new SelectList(db.DokStatuses, "value", "description", inspireDataset.DokStatusId);
            ViewBag.InspireDeliveryAtomFeedId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryAtomFeedId);
            ViewBag.InspireDeliveryDistributionId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryDistributionId);
            ViewBag.InspireDeliveryHarmonizedDataId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryHarmonizedDataId);
            ViewBag.InspireDeliveryMetadataId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryMetadataId);
            ViewBag.InspireDeliveryMetadataServiceId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryMetadataServiceId);
            ViewBag.InspireDeliverySpatialDataServiceId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliverySpatialDataServiceId);
            ViewBag.InspireDeliveryWfsId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWfsId);
            ViewBag.InspireDeliveryWfsOrAtomId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWfsOrAtomId);
            ViewBag.InspireDeliveryWmsId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWmsId);
            ViewBag.OwnerId = new SelectList(db.RegisterItems, "systemId", "name", inspireDataset.OwnerId);
            ViewBag.RegisterId = new SelectList(db.Registers, "systemId", "name", inspireDataset.RegisterId);
            ViewBag.StatusId = new SelectList(db.Statuses, "value", "description", inspireDataset.StatusId);
            ViewBag.SubmitterId = new SelectList(db.RegisterItems, "systemId", "name", inspireDataset.SubmitterId);
            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", inspireDataset.ThemeGroupId);
            return View(inspireDataset);
        }

        // POST: InspireDatasets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SystemId,InspireDeliveryMetadataId,InspireDeliveryMetadataServiceId,InspireDeliveryDistributionId,InspireDeliveryWmsId,InspireDeliveryWfsId,InspireDeliveryAtomFeedId,InspireDeliveryWfsOrAtomId,InspireDeliveryHarmonizedDataId,InspireDeliverySpatialDataServiceId,Uuid,Notes,SpecificUsage,ProductSheetUrl,PresentationRulesUrl,ProductSpecificationUrl,MetadataUrl,DistributionFormat,DistributionUrl,DistributionArea,WmsUrl,ThemeGroupId,DatasetThumbnail,DokStatusId,DokStatusDateAccepted,Name,Seoname,Description,SubmitterId,OwnerId,DateSubmitted,Modified,StatusId,RegisterId")] InspireDataset inspireDataset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inspireDataset).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DokStatusId = new SelectList(db.DokStatuses, "value", "description", inspireDataset.DokStatusId);
            ViewBag.InspireDeliveryAtomFeedId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryAtomFeedId);
            ViewBag.InspireDeliveryDistributionId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryDistributionId);
            ViewBag.InspireDeliveryHarmonizedDataId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryHarmonizedDataId);
            ViewBag.InspireDeliveryMetadataId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryMetadataId);
            ViewBag.InspireDeliveryMetadataServiceId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryMetadataServiceId);
            ViewBag.InspireDeliverySpatialDataServiceId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliverySpatialDataServiceId);
            ViewBag.InspireDeliveryWfsId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWfsId);
            ViewBag.InspireDeliveryWfsOrAtomId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWfsOrAtomId);
            ViewBag.InspireDeliveryWmsId = new SelectList(db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWmsId);
            ViewBag.OwnerId = new SelectList(db.RegisterItems, "systemId", "name", inspireDataset.OwnerId);
            ViewBag.RegisterId = new SelectList(db.Registers, "systemId", "name", inspireDataset.RegisterId);
            ViewBag.StatusId = new SelectList(db.Statuses, "value", "description", inspireDataset.StatusId);
            ViewBag.SubmitterId = new SelectList(db.RegisterItems, "systemId", "name", inspireDataset.SubmitterId);
            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", inspireDataset.ThemeGroupId);
            return View(inspireDataset);
        }

        // GET: InspireDatasets/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspireDataset inspireDataset = db.InspireDatasets.Find(id);
            if (inspireDataset == null)
            {
                return HttpNotFound();
            }
            return View(inspireDataset);
        }

        // POST: InspireDatasets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            InspireDataset inspireDataset = db.InspireDatasets.Find(id);
            db.InspireDatasets.Remove(inspireDataset);
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
