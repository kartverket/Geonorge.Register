using System;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.DOK.Service;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services;
using System.Web;
using Kartverket.Register.Helpers;
using System.Collections.Generic;
using www.opengis.net;
using Kartverket.Register.Models.ViewModels;
using System.Linq;
using Kartverket.Register.Services.Translation;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class DatasetsController : Controller
    {
        private readonly RegisterDbContext db;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _accessControlService;
        private IDatasetDeliveryService _datasetDeliveryService;
        private ITranslationService _translationService;
        private IDatasetService _datasetService;
        private IStatusReportService _statusReportService;

        public DatasetsController(RegisterDbContext dbContext, IRegisterItemService registerItemService, IRegisterService registerService, IAccessControlService accessControllService, IDatasetDeliveryService datasetDeliveryService, ITranslationService translationService, IDatasetService datasetService, IStatusReportService statusReportService)
        {
             db = dbContext;
            _registerItemService = registerItemService;
            _registerService = registerService;
            _accessControlService = accessControllService;
            _datasetDeliveryService = datasetDeliveryService;
            _translationService = translationService;
            _datasetService = datasetService;
            _statusReportService = statusReportService;
        }


        // GET: Datasets/Create
        [Authorize]
        //[Route("dataset/{parentRegister}/{registerowner}/{registername}/ny")]
        //[Route("dataset/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            Dataset dataset = new Dataset();
            dataset.register = _registerService.GetRegister(parentRegister, registername);
            if (dataset.register != null)
            {
                dataset.DatasetType = GetDatasetType(dataset.register.name);
                Viewbags(dataset);
                if (_accessControlService.Access(dataset.register))
                {
                    return View(dataset);
                }

                throw new HttpException(401, "Access Denied");
            }
            return HttpNotFound("Finner ikke registeret");
        }

        // POST: Datasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        //[Route("dataset/{parentRegister}/{registerowner}/{registername}/ny")]
        //[Route("dataset/{registername}/ny")]
        public ActionResult Create(Dataset dataset, string registername, string metadataUuid, string parentRegister, string registerowner, string searchString)
        {
            dataset.register = _registerService.GetRegister(parentRegister, registername);
            if (dataset.register != null)
            {
                if (_accessControlService.Access(dataset.register))
                {
                    dataset.systemId = dataset.GetSystemId();
                    dataset.registerId = dataset.register.GetSystemId();
                    dataset.DatasetType = dataset.GetDatasetType();

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        SearchResultsType result = SearchMetadataFromKartkatalogen(searchString);
                        var resList = ParseSearchResult(result);
                        if (resList.Count == 0)
                            ViewBag.Message = "Søket gav ingen treff";
                        ViewBag.SearchResultList = resList;
                    }
                    else if (!string.IsNullOrEmpty(metadataUuid))
                    {
                        Dataset model = GetMetadataFromKartkatalogen(dataset, metadataUuid);
                        Viewbags(dataset);
                        return View(model);
                    }
                    else if (!string.IsNullOrWhiteSpace(dataset.name))
                    {
                        if (!NameIsValid(dataset))
                        {
                            ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationDataset());
                            Viewbags(dataset);
                            return View(dataset);
                        }
                        if (ModelState.IsValid)
                        {
                            dataset = GetMetadataFromKartkatalogen(dataset, dataset.Uuid);
                            dataset = _datasetService.UpdateDataset(dataset);
                            dataset.StatusHistories = _statusReportService.GetStatusHistoriesByDataset(dataset);
                            _registerItemService.SaveNewRegisterItem(dataset);
                            return Redirect(dataset.GetObjectUrl());
                        }
                    }
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            Viewbags(dataset);
            return View(dataset);
        }


        // GET: Datasets/Create
        [Authorize]
        //[Route("dataset/{parentRegister}/{registerowner}/{registername}/{municipality}/ny")]
        //[Route("dataset/{registername}/{municipality}/ny")]
        public ActionResult CreateMunicipalDataset(string municipality)
        {
            if (municipality != null)
            {
                CreateDokMunicipalViewModel model = new CreateDokMunicipalViewModel();
                model.Register = _registerService.GetDokMunicipalRegister();
                model.MunicipalityCode = municipality;
                model.DatasetOwner = _registerItemService.GetMunicipalityOrganizationByNr(municipality);

                if (_accessControlService.AccessEditOrCreateDOKMunicipalBySelectedMunicipality(model.MunicipalityCode))
                {
                    return View(model);
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound("Finner ikke kommunenr.");
        }


        // POST: Datasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        //[Route("dataset/{parentRegister}/{registerowner}/{registername}/{municipality}/ny")]
        //[Route("dataset/{registername}/{municipality}/ny")]
        public ActionResult CreateMunicipalDataset(CreateDokMunicipalViewModel model, string searchString, bool save = false)
        {
            model.Register = _registerService.GetDokMunicipalRegister();
            if (_accessControlService.AccessEditOrCreateDOKMunicipalBySelectedMunicipality(model.MunicipalityCode))
            {
                ViewBag.SearchString = searchString;
                model.DatasetOwner = _registerItemService.GetMunicipalityOrganizationByNr(model.MunicipalityCode);

                if (model.SelectedList != null)
                {
                    List<MetadataItemViewModel> items = new List<MetadataItemViewModel>();
                    foreach (var metaItem in model.SelectedList)
                        items.Add(metaItem);

                    foreach (var item in items)
                    {
                        if (!item.Selected)
                            model.SelectedList.Remove(item);
                    }
                }

                if (model.SearchResult != null)
                {
                    foreach (var item in model.SearchResult)
                    {
                        if (item.Selected)
                        {
                            if (model.SelectedList == null)
                            {
                                model.SelectedList = new List<MetadataItemViewModel>();
                            }
                            if(model.SelectedList.All(metaItem => metaItem.Uuid != item.Uuid))
                                model.SelectedList.Add(item);
                        }
                    }
                    model.SearchResult = null;
                }
                if (!string.IsNullOrEmpty(searchString) && !save)
                {
                    SearchResultsType result = SearchMetadataFromKartkatalogen(searchString);
                    model.SearchResult = ParseSearchResult(result);
                    return View(model);
                }
                if (save)
                {
                    foreach (var item in model.SelectedList)
                    {
                            Dataset dataset = new Dataset();
                            dataset = GetMetadataFromKartkatalogen(dataset, item.Uuid);
                            dataset.register = model.Register;
                            dataset.datasetowner = model.DatasetOwner;
                            dataset.datasetownerId = model.DatasetOwner.systemId;
                            dataset = _datasetService.UpdateDataset(dataset);
                            _registerItemService.SaveNewRegisterItem(dataset);
                    }
                    return Redirect(model.Register.GetObjectUrl() + "?municipality=" + model.MunicipalityCode);
                }
                return View(model);
            }
            else
            {
                throw new HttpException(401, "Access Denied");
            }
        }


        // GET: Datasets/Edit/5
        [Authorize]
        //[Route("dataset/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        //[Route("dataset/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(string registername, string itemowner, string itemname, string parentRegister)
        {
            Dataset dataset = (Dataset)_registerItemService.GetRegisterItem(parentRegister, registername, itemname, 1, itemowner);
            if (dataset != null)
            {
                if (_accessControlService.Access(dataset))
                {
                    if (!string.IsNullOrEmpty(dataset.Uuid))
                    {
                        Dataset model = GetMetadataFromKartkatalogen(dataset, dataset.Uuid);
                        Viewbags(model);
                        return View(model);
                    }
                    Viewbags(dataset);
                    return View(dataset);
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound("Finner ikke datasettet");
        }


        // POST: Dataset/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Route("dataset/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        //[Route("dataset/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(Dataset dataset, CoverageDataset coverage, string registername, string itemname, string metadataUuid, string parentRegister, string registerowner, string itemowner, bool dontUpdateDescription = false)
        {
            Dataset originalDataset = (Dataset)_registerItemService.GetRegisterItem(parentRegister, registername, itemname, 1, itemowner);
            if (originalDataset != null)
            {
                if (metadataUuid != null)
                {
                    Dataset model = GetMetadataFromKartkatalogen(originalDataset, metadataUuid, dontUpdateDescription);
                    Viewbags(model);
                    return View(model);
                }

                if (originalDataset.IsNationalDataset())
                {
                    if (_accessControlService.IsAdmin())
                    {
                        return EditDataset(dataset, originalDataset);
                    }
                    if (_accessControlService.IsMunicipalUser())
                    {
                        return EditCoverageDataset(coverage, originalDataset);
                    }
                }
                else if (_accessControlService.Access(originalDataset))
                {
                    if (ModelState.IsValid)
                    {
                        return EditDataset(dataset, originalDataset, coverage);
                    }
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            Viewbags(originalDataset);
            return View(originalDataset);
        }


        // GET: Dataset/Delete/5
        [Authorize]
        //[Route("dataset/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{itemname}/slett")]
        //[Route("dataset/{registername}/{itemowner}/{itemname}/slett")]
        public ActionResult Delete(string registername, string itemname, string parentregister, string parentregisterowner, string itemowner)
        {
            Dataset dataset = (Dataset)_registerItemService.GetRegisterItem(parentregister, registername, itemname, 1, itemowner);
            if (dataset != null)
            {
                if (_accessControlService.Access(dataset))
                {
                    Viewbags(dataset);
                    return View(dataset);
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound("Finner ikke datasettet");
        }


        // POST: Dataset/Delete/5
        [HttpPost, ActionName("Delete")]
        //[Route("dataset/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett")]
        //[Route("dataset/{registername}/{itemowner}/{itemname}/slett")]        //[Route("dataset/{registername}/{itemowner}/{itemname}/slett")]
        public ActionResult DeleteConfirmed(string registername, string itemname, string parentregister, string registerowner, string itemowner)
        {
            Dataset dataset = (Dataset)_registerItemService.GetCurrentRegisterItem(parentregister, registername, itemname);
            _registerItemService.DeleteCoverageByDatasetId(dataset.systemId);
            dataset.StatusHistories.Clear();
            _registerItemService.SaveDeleteRegisterItem(dataset);
            return Redirect(RegisterUrls.registerUrl(parentregister, registerowner, registername));
        }


        // *** HJELPEMETODER

        private List<MetadataItemViewModel> ParseSearchResult(SearchResultsType res)
        {
            List<MetadataItemViewModel> result = new List<MetadataItemViewModel>();

            if (res.numberOfRecordsMatched != "0")
            {
                for (int s = 0; s < res.Items.Length; s++)
                {
                    MetadataItemViewModel m = new MetadataItemViewModel();
                    m.Uuid = ((DCMIRecordType)(res.Items[s])).Items[0].Text[0];
                    m.Title = ((DCMIRecordType)(res.Items[s])).Items[2].Text[0];
                    result.Add(m);
                }
            }

            return result;
        }

        private SearchResultsType SearchMetadataFromKartkatalogen(string searchString)
        {
            SearchResultsType result = new MetadataService(db).SearchMetadata(searchString);
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }

        private string GetDatasetType(string registerName)
        {
            if (DokMunicipalDataset(registerName))
            {
                return "Kommunalt";
            }
            else
            {
                return "Nasjonalt";
            }
        }

        private static bool DokMunicipalDataset(string registerName)
        {
            return registerName == "Det offentlige kartgrunnlaget - Kommunalt";
        }

        private bool NameIsValid(Dataset dataset)
        {
            return _registerItemService.ItemNameIsValid(dataset);
        }

        private Dataset GetMetadataFromKartkatalogen(Dataset dataset, string uuid, bool dontUpdateDescription = false)
        {
            var model = new Dataset();
            try
            {
                model = new MetadataService(db).UpdateDatasetWithMetadata(dataset, uuid, dontUpdateDescription);
            }
            catch (Exception e)
            {
                TempData["error"] = "Det oppstod en feil ved henting av metadata: " + e.Message;
            }
            return model;
        }

        private void Viewbags(Dataset dataset)
        {
            ViewBag.registerId = _registerItemService.GetRegisterSelectList(dataset.registerId);
            ViewBag.dokStatusId = _registerItemService.GetDokStatusSelectList(dataset.dokStatusId);
            ViewBag.dokDeliveryMetadataStatusId = _registerItemService.GetDokDeliveryStatusSelectList(dataset.dokDeliveryMetadataStatusId);
            ViewBag.dokDeliveryProductSheetStatusId = _registerItemService.GetDokDeliveryStatusSelectList(dataset.dokDeliveryProductSheetStatusId);
            ViewBag.dokDeliveryPresentationRulesStatusId = _registerItemService.GetDokDeliveryStatusSelectList(dataset.dokDeliveryPresentationRulesStatusId);
            ViewBag.dokDeliveryProductSpecificationStatusId = _registerItemService.GetDokDeliveryStatusSelectList(dataset.dokDeliveryProductSpecificationStatusId);
            ViewBag.dokDeliveryWmsStatusId = _registerItemService.GetDokDeliveryStatusSelectList(dataset.dokDeliveryWmsStatusId);
            ViewBag.dokDeliveryWfsStatusId = _registerItemService.GetDokDeliveryStatusSelectList(dataset.dokDeliveryWfsStatusId);
            ViewBag.dokDeliverySosiRequirementsStatusId = _registerItemService.GetDokDeliveryStatusSelectList(dataset.dokDeliverySosiRequirementsStatusId);
            ViewBag.dokDeliveryDistributionStatusId = _registerItemService.GetDokDeliveryStatusSelectList(dataset.dokDeliveryDistributionStatusId);
            ViewBag.dokDeliveryGmlRequirementsStatusId = _registerItemService.GetDokDeliveryStatusSelectList(dataset.dokDeliveryGmlRequirementsStatusId);
            ViewBag.dokDeliveryAtomFeedStatusId = _registerItemService.GetDokDeliveryStatusSelectList(dataset.dokDeliveryAtomFeedStatusId);
            ViewBag.CoverageDOKStatusId = _registerItemService.GetDokStatusSelectList(null);
            ViewBag.submitterId = _registerItemService.GetSubmitterSelectList(dataset.submitterId);
            ViewBag.datasetownerId = _registerItemService.GetOwnerSelectList(dataset.datasetownerId);
            ViewBag.ThemeGroupId = _registerItemService.GetThemeGroupSelectList(dataset.ThemeGroupId);

            ViewBag.RegionalPlan = new SelectList(dataset.SuitabilityScale(), "Value", "Text", dataset.RegionalPlan);
            ViewBag.MunicipalSocialPlan = new SelectList(dataset.SuitabilityScale(), "Value", "Text", dataset.MunicipalSocialPlan);
            ViewBag.MunicipalLandUseElementPlan = new SelectList(dataset.SuitabilityScale(), "Value", "Text", dataset.MunicipalLandUseElementPlan);
            ViewBag.ZoningPlanArea = new SelectList(dataset.SuitabilityScale(), "Value", "Text", dataset.ZoningPlanArea);
            ViewBag.ZoningPlanDetails = new SelectList(dataset.SuitabilityScale(), "Value", "Text", dataset.ZoningPlanDetails);
            ViewBag.BuildingMatter = new SelectList(dataset.SuitabilityScale(), "Value", "Text", dataset.BuildingMatter);
            ViewBag.PartitionOff = new SelectList(dataset.SuitabilityScale(), "Value", "Text", dataset.PartitionOff);
            ViewBag.EenvironmentalImpactAssessment = new SelectList(dataset.SuitabilityScale(), "Value", "Text", dataset.EenvironmentalImpactAssessment);
        }

        private ActionResult EditDataset(Dataset dataset, Dataset originalDataset, CoverageDataset coverage = null)
        {
            dataset.register = originalDataset.register;
            if (!NameIsValid(dataset))
            {
                ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                Viewbags(originalDataset);
                return View(originalDataset);
            }

            dataset = _datasetService.UpdateDataset(dataset, originalDataset, coverage);
            _translationService.UpdateTranslations(dataset, originalDataset);
            _registerItemService.SaveEditedRegisterItem(originalDataset);
            return Redirect(originalDataset.GetObjectUrl());
        }

        private ActionResult EditCoverageDataset(CoverageDataset coverage, Dataset originalDataset)
        {
            originalDataset.Coverage = _datasetService.EditDatasetCoverage(coverage, originalDataset);
            _registerItemService.SaveEditedRegisterItem(originalDataset);
            return Redirect(originalDataset.register.GetDokMunicipalityUrl());
        }
    }
}
