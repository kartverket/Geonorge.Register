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
    public class GeodatalovDatasetsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: GeodatalovDatasets/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = new SelectList(db.RegisterItems, "systemId", "name");
            ViewBag.RegisterId = new SelectList(db.Registers, "systemId", "name");
            ViewBag.StatusId = new SelectList(db.Statuses, "value", "description");
            ViewBag.SubmitterId = new SelectList(db.RegisterItems, "systemId", "name");
            ViewBag.VersioningId = new SelectList(db.Versions, "systemId", "containedItemClass");
            ViewBag.DokStatusId = new SelectList(db.DokStatuses, "value", "description");
            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description");
            ViewBag.AtomFeedStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId");
            ViewBag.CommonStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId");
            ViewBag.GmlDataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId");
            ViewBag.MetadataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId");
            ViewBag.ProductSpesificationStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId");
            ViewBag.SosiDataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId");
            ViewBag.WfsStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId");
            ViewBag.WmsStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId");
            return View();
        }

        // POST: GeodatalovDatasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SystemId,Name,Seoname,Description,SubmitterId,OwnerId,DateSubmitted,Modified,StatusId,RegisterId,DateAccepted,DateNotAccepted,DateSuperseded,DateRetired,VersionNumber,VersionName,VersioningId,Uuid,Notes,SpecificUsage,ProductSheetUrl,PresentationRulesUrl,ProductSpecificationUrl,MetadataUrl,DistributionFormat,DistributionUrl,DistributionArea,WmsUrl,ThemeGroupId,DatasetThumbnail,DokStatusId,DokStatusDateAccepted,UuidService,InspireTheme,Dok,NationalDataset,Plan,Geodatalov,MetadataStatusId,ProductSpesificationStatusId,SosiDataStatusId,GmlDataStatusId,WmsStatusId,WfsStatusId,AtomFeedStatusId,CommonStatusId")] GeodatalovDataset geodatalovDataset)
        {
            if (ModelState.IsValid)
            {
                geodatalovDataset.SystemId = Guid.NewGuid();
                db.GeodatalovDatasets.Add(geodatalovDataset);
                db.SaveChanges();
                return View();
            }

            ViewBag.OwnerId = new SelectList(db.RegisterItems, "systemId", "name", geodatalovDataset.OwnerId);
            ViewBag.RegisterId = new SelectList(db.Registers, "systemId", "name", geodatalovDataset.RegisterId);
            ViewBag.StatusId = new SelectList(db.Statuses, "value", "description", geodatalovDataset.StatusId);
            ViewBag.SubmitterId = new SelectList(db.RegisterItems, "systemId", "name", geodatalovDataset.SubmitterId);
            ViewBag.VersioningId = new SelectList(db.Versions, "systemId", "containedItemClass", geodatalovDataset.VersioningId);
            ViewBag.DokStatusId = new SelectList(db.DokStatuses, "value", "description", geodatalovDataset.DokStatusId);
            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", geodatalovDataset.ThemeGroupId);
            ViewBag.AtomFeedStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.AtomFeedStatusId);
            ViewBag.CommonStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.CommonStatusId);
            ViewBag.GmlDataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.GmlDataStatusId);
            ViewBag.MetadataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.MetadataStatusId);
            ViewBag.ProductSpesificationStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.ProductSpesificationStatusId);
            ViewBag.SosiDataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.SosiDataStatusId);
            ViewBag.WfsStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.WfsStatusId);
            ViewBag.WmsStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.WmsStatusId);
            return View(geodatalovDataset);
        }

        // GET: GeodatalovDatasets/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeodatalovDataset geodatalovDataset = db.GeodatalovDatasets.Find(id);
            if (geodatalovDataset == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.RegisterItems, "systemId", "name", geodatalovDataset.OwnerId);
            ViewBag.RegisterId = new SelectList(db.Registers, "systemId", "name", geodatalovDataset.RegisterId);
            ViewBag.StatusId = new SelectList(db.Statuses, "value", "description", geodatalovDataset.StatusId);
            ViewBag.SubmitterId = new SelectList(db.RegisterItems, "systemId", "name", geodatalovDataset.SubmitterId);
            ViewBag.VersioningId = new SelectList(db.Versions, "systemId", "containedItemClass", geodatalovDataset.VersioningId);
            ViewBag.DokStatusId = new SelectList(db.DokStatuses, "value", "description", geodatalovDataset.DokStatusId);
            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", geodatalovDataset.ThemeGroupId);
            ViewBag.AtomFeedStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.AtomFeedStatusId);
            ViewBag.CommonStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.CommonStatusId);
            ViewBag.GmlDataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.GmlDataStatusId);
            ViewBag.MetadataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.MetadataStatusId);
            ViewBag.ProductSpesificationStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.ProductSpesificationStatusId);
            ViewBag.SosiDataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.SosiDataStatusId);
            ViewBag.WfsStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.WfsStatusId);
            ViewBag.WmsStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.WmsStatusId);
            return View(geodatalovDataset);
        }

        // POST: GeodatalovDatasets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SystemId,Name,Seoname,Description,SubmitterId,OwnerId,DateSubmitted,Modified,StatusId,RegisterId,DateAccepted,DateNotAccepted,DateSuperseded,DateRetired,VersionNumber,VersionName,VersioningId,Uuid,Notes,SpecificUsage,ProductSheetUrl,PresentationRulesUrl,ProductSpecificationUrl,MetadataUrl,DistributionFormat,DistributionUrl,DistributionArea,WmsUrl,ThemeGroupId,DatasetThumbnail,DokStatusId,DokStatusDateAccepted,UuidService,InspireTheme,Dok,NationalDataset,Plan,Geodatalov,MetadataStatusId,ProductSpesificationStatusId,SosiDataStatusId,GmlDataStatusId,WmsStatusId,WfsStatusId,AtomFeedStatusId,CommonStatusId")] GeodatalovDataset geodatalovDataset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(geodatalovDataset).State = EntityState.Modified;
                db.SaveChanges();
                return View();
            }
            ViewBag.OwnerId = new SelectList(db.RegisterItems, "systemId", "name", geodatalovDataset.OwnerId);
            ViewBag.RegisterId = new SelectList(db.Registers, "systemId", "name", geodatalovDataset.RegisterId);
            ViewBag.StatusId = new SelectList(db.Statuses, "value", "description", geodatalovDataset.StatusId);
            ViewBag.SubmitterId = new SelectList(db.RegisterItems, "systemId", "name", geodatalovDataset.SubmitterId);
            ViewBag.VersioningId = new SelectList(db.Versions, "systemId", "containedItemClass", geodatalovDataset.VersioningId);
            ViewBag.DokStatusId = new SelectList(db.DokStatuses, "value", "description", geodatalovDataset.DokStatusId);
            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", geodatalovDataset.ThemeGroupId);
            ViewBag.AtomFeedStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.AtomFeedStatusId);
            ViewBag.CommonStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.CommonStatusId);
            ViewBag.GmlDataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.GmlDataStatusId);
            ViewBag.MetadataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.MetadataStatusId);
            ViewBag.ProductSpesificationStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.ProductSpesificationStatusId);
            ViewBag.SosiDataStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.SosiDataStatusId);
            ViewBag.WfsStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.WfsStatusId);
            ViewBag.WmsStatusId = new SelectList(db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.WmsStatusId);
            return View(geodatalovDataset);
        }

        // GET: GeodatalovDatasets/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeodatalovDataset geodatalovDataset = db.GeodatalovDatasets.Find(id);
            if (geodatalovDataset == null)
            {
                return HttpNotFound();
            }
            return View(geodatalovDataset);
        }

        // POST: GeodatalovDatasets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            GeodatalovDataset geodatalovDataset = db.GeodatalovDatasets.Find(id);
            db.GeodatalovDatasets.Remove(geodatalovDataset);
            db.SaveChanges();
            return View();
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
