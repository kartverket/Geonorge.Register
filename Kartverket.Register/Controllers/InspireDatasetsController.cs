using System;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.DOK.Service;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;

namespace Kartverket.Register.Controllers
{
    public class InspireDatasetsController : Controller
    {
        private readonly RegisterDbContext _db = new RegisterDbContext();
        private readonly IInspireDatasetService _inspireDatasetService;
        private readonly MetadataService _metadataService;
        private readonly IAccessControlService _accessControlService;
        private readonly IDatasetDeliveryService _datasetDeliveryService;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;

        public InspireDatasetsController(IInspireDatasetService inspireDatasetService, IAccessControlService accessControllService, IRegisterService registerService, IDatasetDeliveryService datasetDeliveryService, IRegisterItemService registerItemService) {
            _inspireDatasetService = inspireDatasetService;
            _accessControlService = accessControllService;
            _metadataService = new MetadataService();
            _registerService = registerService;
            _datasetDeliveryService = datasetDeliveryService;
            _registerItemService = registerItemService;
        }
        
        // GET: InspireDatasets/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var inspireDataset = _db.InspireDatasets.Find(id);
            if (inspireDataset == null)
            {
                return HttpNotFound();
            }
            return View(inspireDataset);
        }

        // GET: InspireDatasets/Create
        [Authorize]
        [Route("inspire/{registername}/ny")]
        [Route("inspire/{parentregister}/{registerowner}/{registername}/ny")]
        public ActionResult Create(string registername, string parentregister)
        {
            var model = _inspireDatasetService.NewInspireDataset(parentregister, registername);
            if (_accessControlService.Access(model.Register))
                return View(model);
            throw new HttpException(401, "Access Denied");
        }

        // POST: InspireDatasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("inspire/{registername}/ny")]
        [Route("inspire/{parentregister}/{registerowner}/{registername}/ny")]
        public ActionResult Create(InspireDatasetViewModel viewModel, string parentregister, string registername, string metadataUuid)
        {
            viewModel.Register = _registerService.GetRegisterBySystemId(viewModel.RegisterId);
            if (_accessControlService.Access(viewModel.Register)) { 
                if (viewModel.SearchString != null)
                {
                    viewModel.SearchResultList =
                        _metadataService.SearchMetadataFromKartkatalogen(viewModel.SearchString);
                }
                else if (metadataUuid != null)
                {
                    viewModel.Update(_metadataService.FetchInspireDatasetFromKartkatalogen(metadataUuid));
                }
                else if (ModelState.IsValid)
                {
                    var inspireDataset = _inspireDatasetService.CreateNewInspireDataset(viewModel, parentregister, registername);
                    return Redirect(inspireDataset.DetailPageUrl());
                }
                return View(viewModel);
            }
            throw new HttpException(401, "Access Denied");
        }

        // GET: InspireDatasets/Edit/5
        [Authorize]
        [Route("inspire/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        [Route("inspire/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(string registername, string itemname)
        {
            var inspireDataset = _inspireDatasetService.GetInspireDatasetByName(registername, itemname);
            if (inspireDataset == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (_accessControlService.Access(inspireDataset))
            {
                var viewModel = new InspireDatasetViewModel(inspireDataset);
                ViewBags(viewModel);
                return View(viewModel);
            }
            throw new HttpException(401, "Access Denied");
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
                return RedirectToAction("Details");
            }
            return View();
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
            return RedirectToAction("Details");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ViewBags(InspireDatasetViewModel viewModel)
        {
            ViewBag.DokStatusId = _registerItemService.GetDokStatusSelectList(viewModel.DokStatusId);
            ViewBag.SubmitterId = _registerItemService.GetSubmitterSelectList(viewModel.SubmitterId);
            ViewBag.OwnerId = _registerItemService.GetOwnerSelectList(viewModel.OwnerId);
            ViewBag.MetadataStatus = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataStatus);
            ViewBag.MetadataServiceStatus = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataStatus);
            ViewBag.DistributionStatus = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataStatus);
            ViewBag.WmsStatus = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataStatus);
            ViewBag.WfsStatus = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataStatus);
            ViewBag.AtomFeedStatus = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataStatus);
            ViewBag.WfsOrAtomStatus = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataStatus);
            ViewBag.HarmonizedDataStatus = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataStatus);
            ViewBag.SpatialDataServiceStatus = _datasetDeliveryService.GetDokDeliveryStatusesAsSelectlist(viewModel.MetadataStatus);
        }

    }
}
