using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services;

namespace Kartverket.Register.Controllers
{
    public class InspireDatasetsController : Controller
    {
        private readonly RegisterDbContext _db = new RegisterDbContext();
        private readonly IInspireDatasetService _inspireDatasetService;

        public InspireDatasetsController(IInspireDatasetService inspireDatasetService) {
            _inspireDatasetService = inspireDatasetService;
        }

        // GET: InspireDatasets
        public ActionResult Index()
        {
            var inspireDatasets = _db.InspireDatasets.Include(i => i.DokStatus).Include(i => i.InspireDeliveryAtomFeed).Include(i => i.InspireDeliveryDistribution).Include(i => i.InspireDeliveryHarmonizedData).Include(i => i.InspireDeliveryMetadata).Include(i => i.InspireDeliveryMetadataService).Include(i => i.InspireDeliverySpatialDataService).Include(i => i.InspireDeliveryWfs).Include(i => i.InspireDeliveryWfsOrAtom).Include(i => i.InspireDeliveryWms).Include(i => i.Owner).Include(i => i.Register).Include(i => i.Status).Include(i => i.Submitter).Include(i => i.Theme);
            return View(inspireDatasets.ToList());
        }

        // GET: InspireDatasets/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspireDataset inspireDataset = _db.InspireDatasets.Find(id);
            if (inspireDataset == null)
            {
                return HttpNotFound();
            }
            return View(inspireDataset);
        }

        // GET: InspireDatasets/Create
        //[Authorize]
        [Route("inspire/{registername}/ny")]
        [Route("inspire/{parentregister}/{registerowner}/{registername}/ny")]
        public ActionResult Create(string registername, string parentregister, string registerowner)
        {
            ViewBag.DokStatusId = new SelectList(_db.DokStatuses, "value", "description");
            ViewBag.InspireDeliveryAtomFeedStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description", null);
            ViewBag.InspireDeliveryDistributionStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description");
            ViewBag.InspireDeliveryHarmonizedDataStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description");
            ViewBag.InspireDeliveryMetadataStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description");
            ViewBag.InspireDeliveryMetadataServiceStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description");
            ViewBag.InspireDeliverySpatialDataServiceStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description");
            ViewBag.InspireDeliveryWfsStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description");
            ViewBag.InspireDeliveryWfsOrAtomStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description");
            ViewBag.InspireDeliveryWmsStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description");
            ViewBag.OwnerId = new SelectList(_db.RegisterItems, "systemId", "name");
            ViewBag.RegisterId = new SelectList(_db.Registers, "systemId", "name");
            ViewBag.StatusId = new SelectList(_db.Statuses, "value", "description");
            ViewBag.SubmitterId = new SelectList(_db.RegisterItems, "systemId", "name");
            ViewBag.ThemeGroupId = new SelectList(_db.DOKThemes, "value", "description");
            return View();
        }

        // POST: InspireDatasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("inspire/{registername}/ny")]
        [Route("inspire/{parentregister}/{registerowner}/{registername}/ny")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InspireDatasetViewModel inspireDatasetViewModel, string parentregister, string registername)
        {
            if (ModelState.IsValid)
            {
                _inspireDatasetService.CreateNewInspireDataset(inspireDatasetViewModel, parentregister, registername);
                return RedirectToAction("Details");
            }

            ViewBag.DokStatusId = new SelectList(_db.DokStatuses, "value", "description", inspireDatasetViewModel.DokStatusId);
            ViewBag.InspireDeliveryAtomFeedStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description", inspireDatasetViewModel.InspireDeliveryAtomFeedStatus);
            ViewBag.InspireDeliveryDistributionStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description", inspireDatasetViewModel.InspireDeliveryDistributionStatus);
            ViewBag.InspireDeliveryHarmonizedDataStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description", inspireDatasetViewModel.InspireDeliveryHarmonizedDataStatus);
            ViewBag.InspireDeliveryMetadataStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description", inspireDatasetViewModel.InspireDeliveryMetadataStatus);
            ViewBag.InspireDeliveryMetadataServiceStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description", inspireDatasetViewModel.InspireDeliveryMetadataServiceStatus);
            ViewBag.InspireDeliverySpatialDataServiceStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description", inspireDatasetViewModel.InspireDeliverySpatialDataServiceStatus);
            ViewBag.InspireDeliveryWfsStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description", inspireDatasetViewModel.InspireDeliveryWfsStatus);
            ViewBag.InspireDeliveryWfsOrAtomStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description", inspireDatasetViewModel.InspireDeliveryWfsOrAtomStatus);
            ViewBag.InspireDeliveryWmsStatus = new SelectList(_db.DokDeliveryStatuses, "value", "description", inspireDatasetViewModel.InspireDeliveryWmsStatus);
            ViewBag.OwnerId = new SelectList(_db.RegisterItems, "systemId", "name", inspireDatasetViewModel.OwnerId);
            ViewBag.RegisterId = new SelectList(_db.Registers, "systemId", "name", inspireDatasetViewModel.RegisterId);
            ViewBag.StatusId = new SelectList(_db.Statuses, "value", "description", inspireDatasetViewModel.StatusId);
            ViewBag.SubmitterId = new SelectList(_db.RegisterItems, "systemId", "name", inspireDatasetViewModel.SubmitterId);
            ViewBag.ThemeGroupId = new SelectList(_db.DOKThemes, "value", "description", inspireDatasetViewModel.ThemeGroupId);
            return View(inspireDatasetViewModel);
        }

        // GET: InspireDatasets/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspireDataset inspireDataset = _db.InspireDatasets.Find(id);
            if (inspireDataset == null)
            {
                return HttpNotFound();
            }
            //ViewBag.DokStatusId = new SelectList(_db.DokStatuses, "value", "description", inspireDataset.DokStatusId);
            //ViewBag.InspireDeliveryAtomFeedId = new SelectList(_db.DokDeliveryStatuses, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryAtomFeedId);
            //ViewBag.InspireDeliveryDistributionId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryDistributionId);
            //ViewBag.InspireDeliveryHarmonizedDataId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryHarmonizedDataId);
            //ViewBag.InspireDeliveryMetadataId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryMetadataId);
            //ViewBag.InspireDeliveryMetadataServiceId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryMetadataServiceId);
            //ViewBag.InspireDeliverySpatialDataServiceId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliverySpatialDataServiceId);
            //ViewBag.InspireDeliveryWfsId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWfsId);
            //ViewBag.InspireDeliveryWfsOrAtomId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWfsOrAtomId);
            //ViewBag.InspireDeliveryWmsId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWmsId);
            //ViewBag.OwnerId = new SelectList(_db.RegisterItems, "systemId", "name", inspireDataset.OwnerId);
            //ViewBag.RegisterId = new SelectList(_db.Registers, "systemId", "name", inspireDataset.RegisterId);
            //ViewBag.StatusId = new SelectList(_db.Statuses, "value", "description", inspireDataset.StatusId);
            //ViewBag.SubmitterId = new SelectList(_db.RegisterItems, "systemId", "name", inspireDataset.SubmitterId);
            //ViewBag.ThemeGroupId = new SelectList(_db.DOKThemes, "value", "description", inspireDataset.ThemeGroupId);
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
                _db.Entry(inspireDataset).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.DokStatusId = new SelectList(_db.DokStatuses, "value", "description", inspireDataset.DokStatusId);
            //ViewBag.InspireDeliveryAtomFeedId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryAtomFeedId);
            //ViewBag.InspireDeliveryDistributionId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryDistributionId);
            //ViewBag.InspireDeliveryHarmonizedDataId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryHarmonizedDataId);
            //ViewBag.InspireDeliveryMetadataId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryMetadataId);
            //ViewBag.InspireDeliveryMetadataServiceId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryMetadataServiceId);
            //ViewBag.InspireDeliverySpatialDataServiceId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliverySpatialDataServiceId);
            //ViewBag.InspireDeliveryWfsId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWfsId);
            //ViewBag.InspireDeliveryWfsOrAtomId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWfsOrAtomId);
            //ViewBag.InspireDeliveryWmsId = new SelectList(_db.DeliveryStatus, "DeliveryStatusId", "StatusId", inspireDataset.InspireDeliveryWmsId);
            //ViewBag.OwnerId = new SelectList(_db.RegisterItems, "systemId", "name", inspireDataset.OwnerId);
            //ViewBag.RegisterId = new SelectList(_db.Registers, "systemId", "name", inspireDataset.RegisterId);
            //ViewBag.StatusId = new SelectList(_db.Statuses, "value", "description", inspireDataset.StatusId);
            //ViewBag.SubmitterId = new SelectList(_db.RegisterItems, "systemId", "name", inspireDataset.SubmitterId);
            //ViewBag.ThemeGroupId = new SelectList(_db.DOKThemes, "value", "description", inspireDataset.ThemeGroupId);
            return View(inspireDataset);
        }

        // GET: InspireDatasets/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InspireDataset inspireDataset = _db.InspireDatasets.Find(id);
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
            var inspireDataset = _db.InspireDatasets.Find(id);
            _db.InspireDatasets.Remove(inspireDataset ?? throw new InvalidOperationException());
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
