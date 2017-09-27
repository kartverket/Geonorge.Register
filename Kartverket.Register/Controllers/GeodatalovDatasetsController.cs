using System;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.DOK.Service;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;

namespace Kartverket.Register.Controllers
{
    public class GeodatalovDatasetsController : Controller
    {
        private readonly RegisterDbContext _db = new RegisterDbContext();
        private readonly MetadataService _metadataService;
        private readonly IAccessControlService _accessControlService;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;
        private readonly IGeodatalovDatasetService _geodatalovDatasetService;

        public GeodatalovDatasetsController(IGeodatalovDatasetService geodatalovDatasetService, IAccessControlService accessControllService, IRegisterService registerService, IRegisterItemService registerItemService)
        {
            _geodatalovDatasetService = geodatalovDatasetService;
            _accessControlService = accessControllService;
            _metadataService = new MetadataService();
            _registerService = registerService;
            _registerItemService = registerItemService;
        }

        // GET: InspireDatasets/Create
        [Authorize]
        [Route("geodatalov/{registername}/ny")]
        [Route("geodatalov/{parentregister}/{registerowner}/{registername}/ny")]
        public ActionResult Create(string registername, string parentregister)
        {
            var viewModel = _geodatalovDatasetService.NewGeodatalovDatasetViewModel(parentregister, registername);
            if (_accessControlService.Access(viewModel.Register))
                return View(viewModel);
            throw new HttpException(401, "Access Denied");
        }

        // POST: GeodatalovDatasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("geodatalov/{registername}/ny")]
        [Route("geodatalov/{parentregister}/{registerowner}/{registername}/ny")]
        public ActionResult Create(GeodatalovDatasetViewModel viewModel, string parentregister, string registername, string metadataUuid)
        {
            viewModel.Register = _registerService.GetRegisterBySystemId(viewModel.RegisterId);
            if (!_accessControlService.Access(viewModel.Register)) throw new HttpException(401, "Access Denied");

            if (viewModel.SearchString != null)
            {
                viewModel.SearchResultList = _metadataService.SearchMetadataFromKartkatalogen(viewModel.SearchString);
                return View(viewModel);
            }

            if (metadataUuid != null)
            {
                viewModel.Update(_metadataService.FetchGeodatalovDatasetFromKartkatalogen(metadataUuid));
                if (viewModel.Name == null)
                {
                    ModelState.AddModelError("ErrorMessage", "Det har oppstått en feil ved henting av metadata...");
                }
            }
            if (_registerItemService.ItemNameAlredyExist(viewModel))
            {
                ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationDataset());
                return View(viewModel);
            }
            if (!ModelState.IsValid) return View(viewModel);
            var inspireDataset = _geodatalovDatasetService.CreateNewGeodatalovDataset(viewModel, parentregister, registername);
            return Redirect(inspireDataset.Register.GetObjectUrl());
        }

        // GET: GeodatalovDatasets/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeodatalovDataset geodatalovDataset = _db.GeodatalovDatasets.Find(id);
            if (geodatalovDataset == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(_db.RegisterItems, "systemId", "name", geodatalovDataset.OwnerId);
            ViewBag.RegisterId = new SelectList(_db.Registers, "systemId", "name", geodatalovDataset.RegisterId);
            ViewBag.StatusId = new SelectList(_db.Statuses, "value", "description", geodatalovDataset.StatusId);
            ViewBag.SubmitterId = new SelectList(_db.RegisterItems, "systemId", "name", geodatalovDataset.SubmitterId);
            ViewBag.VersioningId = new SelectList(_db.Versions, "systemId", "containedItemClass", geodatalovDataset.VersioningId);
            ViewBag.DokStatusId = new SelectList(_db.DokStatuses, "value", "description", geodatalovDataset.DokStatusId);
            ViewBag.ThemeGroupId = new SelectList(_db.DOKThemes, "value", "description", geodatalovDataset.ThemeGroupId);
            ViewBag.AtomFeedStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.AtomFeedStatusId);
            ViewBag.CommonStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.CommonStatusId);
            ViewBag.GmlDataStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.GmlDataStatusId);
            ViewBag.MetadataStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.MetadataStatusId);
            ViewBag.ProductSpesificationStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.ProductSpesificationStatusId);
            ViewBag.SosiDataStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.SosiDataStatusId);
            ViewBag.WfsStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.WfsStatusId);
            ViewBag.WmsStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.WmsStatusId);
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
                _db.Entry(geodatalovDataset).State = EntityState.Modified;
                _db.SaveChanges();
                return View();
            }
            ViewBag.OwnerId = new SelectList(_db.RegisterItems, "systemId", "name", geodatalovDataset.OwnerId);
            ViewBag.RegisterId = new SelectList(_db.Registers, "systemId", "name", geodatalovDataset.RegisterId);
            ViewBag.StatusId = new SelectList(_db.Statuses, "value", "description", geodatalovDataset.StatusId);
            ViewBag.SubmitterId = new SelectList(_db.RegisterItems, "systemId", "name", geodatalovDataset.SubmitterId);
            ViewBag.VersioningId = new SelectList(_db.Versions, "systemId", "containedItemClass", geodatalovDataset.VersioningId);
            ViewBag.DokStatusId = new SelectList(_db.DokStatuses, "value", "description", geodatalovDataset.DokStatusId);
            ViewBag.ThemeGroupId = new SelectList(_db.DOKThemes, "value", "description", geodatalovDataset.ThemeGroupId);
            ViewBag.AtomFeedStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.AtomFeedStatusId);
            ViewBag.CommonStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.CommonStatusId);
            ViewBag.GmlDataStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.GmlDataStatusId);
            ViewBag.MetadataStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.MetadataStatusId);
            ViewBag.ProductSpesificationStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.ProductSpesificationStatusId);
            ViewBag.SosiDataStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.SosiDataStatusId);
            ViewBag.WfsStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.WfsStatusId);
            ViewBag.WmsStatusId = new SelectList(_db.DatasetDeliveries, "DatasetDeliveryId", "StatusId", geodatalovDataset.WmsStatusId);
            return View(geodatalovDataset);
        }

        // GET: GeodatalovDatasets/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeodatalovDataset geodatalovDataset = _db.GeodatalovDatasets.Find(id);
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
            GeodatalovDataset geodatalovDataset = _db.GeodatalovDatasets.Find(id);
            _db.GeodatalovDatasets.Remove(geodatalovDataset);
            _db.SaveChanges();
            return View();
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
