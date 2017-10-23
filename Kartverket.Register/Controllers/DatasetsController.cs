﻿using System;
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

        public DatasetsController(RegisterDbContext dbContext, IRegisterItemService registerItemService, IRegisterService registerService, IAccessControlService accessControllService, IDatasetDeliveryService datasetDeliveryService, ITranslationService translationService)
        {
             db = dbContext;
            _registerItemService = registerItemService;
            _registerService = registerService;
            _accessControlService = accessControllService;
            _datasetDeliveryService = datasetDeliveryService;
            _translationService = translationService;
        }

        // GET: Datasets/Create
        [Authorize]
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("dataset/{registername}/ny")]
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
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound("Finner ikke registeret");
        }

        // POST: Datasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("dataset/{registername}/ny")]
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
                        // TODO fikse validering... 
                        if (!NameIsValid(dataset))
                        {
                            ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationDataset());
                            Viewbags(dataset);
                            return View(dataset);
                        }
                        if (ModelState.IsValid)
                        {
                            dataset = initialisationDataset(dataset);
                            dataset = GetMetadataFromKartkatalogen(dataset, dataset.Uuid);
                            dataset.datasetowner = (Organization)_registerItemService.GetRegisterItemBySystemId(dataset.datasetownerId);
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
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/{municipality}/ny")]
        [Route("dataset/{registername}/{municipality}/ny")]
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
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/{municipality}/ny")]
        [Route("dataset/{registername}/{municipality}/ny")]
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
                            if(!model.SelectedList.Any(metaItem => metaItem.Uuid == item.Uuid))
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
                            dataset = initialisationDataset(dataset);
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
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        [Route("dataset/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(string registername, string itemowner, string itemname, string parentRegister)
        {
            Dataset dataset = (Dataset)_registerItemService.GetRegisterItem(parentRegister, registername, itemname, 1, itemowner);
            if (dataset != null)
            {
                if (_accessControlService.Access(dataset))
                {
                    if (!string.IsNullOrEmpty(dataset.Uuid))
                    {
                        Dataset model = GetMetadataFromKartkatalogen(dataset, dataset.Uuid, false);
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
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        [Route("dataset/{registername}/{itemowner}/{itemname}/rediger")]
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
                        return EditDataset(dataset, registername, parentRegister, registerowner, originalDataset);
                    }
                    if (_accessControlService.IsMunicipalUser())
                    {
                        return EditCoverageDataset(coverage, registername, parentRegister, registerowner, originalDataset);
                    }
                }
                else if (_accessControlService.Access(originalDataset))
                {
                    if (ModelState.IsValid)
                    {
                        return EditDataset(dataset, registername, parentRegister, registerowner, originalDataset, coverage);
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
        [Route("dataset/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{itemname}/slett")]
        [Route("dataset/{registername}/{itemowner}/{itemname}/slett")]
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
        [Route("dataset/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett")]
        [Route("dataset/{registername}/{itemowner}/{itemname}/slett")]
        public ActionResult DeleteConfirmed(string registername, string itemname, string parentregister, string registerowner, string itemowner)
        {
            Dataset dataset = (Dataset)_registerItemService.GetCurrentRegisterItem(parentregister, registername, itemname);
            DeleteCoverageDataset(dataset);
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
                    m.Uuid = ((www.opengis.net.DCMIRecordType)(res.Items[s])).Items[0].Text[0];
                    m.Title = ((www.opengis.net.DCMIRecordType)(res.Items[s])).Items[2].Text[0];
                    result.Add(m);
                }
            }

            return result;
        }

        private SearchResultsType SearchMetadataFromKartkatalogen(string searchString)
        {
            SearchResultsType result = new MetadataService().SearchMetadata(searchString);
            return result;
        }

        private Guid GetDatasetOwnerId(Guid datasetownerId)
        {
            if (datasetownerId == null || datasetownerId == Guid.Empty)
            {
                Organization datasetOwner = _registerService.GetOrganizationByUserName();
                return datasetOwner.systemId;
            }
            return datasetownerId;
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

        private bool GetConfirmedDok(CoverageDataset inputCoverage)
        {
            if (inputCoverage != null)
            {
                return inputCoverage.ConfirmedDok;
            }
            else {
                return false;
            }
        }

        private Guid GetVersioningId(Dataset dataset)
        {
            if (dataset.versioningId == null || dataset.versioningId == Guid.Empty)
            {
                return _registerItemService.NewVersioningGroup(dataset);
            }
            else
            {
                return dataset.GetVersioningId();
            }
        }


        private bool NameIsValid(Dataset dataset)
        {
            return _registerItemService.ItemNameAlredyExist(dataset);
        }

        private Dataset GetMetadataFromKartkatalogen(Dataset dataset, string uuid, bool dontUpdateDescription = false)
        {
            var model = new Dataset();
            try
            {
                model = new MetadataService().UpdateDatasetWithMetadata(dataset, uuid, dataset, dontUpdateDescription);
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

        private ActionResult EditDataset(Dataset dataset, string registername, string parentRegister, string registerowner, Dataset originalDataset, CoverageDataset coverage = null)
        {
            dataset.register = originalDataset.register;
            if (!NameIsValid(dataset))
            {
                ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                Viewbags(originalDataset);
                return View(originalDataset);
            }
            initialisationDataset(dataset, originalDataset, coverage);
            _translationService.UpdateTranslations(dataset, originalDataset);
            _registerItemService.SaveEditedRegisterItem(originalDataset);
            return Redirect(originalDataset.GetObjectUrl());
        }

        private ActionResult EditCoverageDataset(CoverageDataset coverage, string registername, string parentRegister, string registerowner, Dataset originalDataset)
        {
            initialisationCoverageDataset(coverage, originalDataset);
            _registerItemService.SaveEditedRegisterItem(originalDataset);
            return Redirect(originalDataset.register.GetDokMunicipalityUrl());
        }

        private Guid SetMunicipality()
        {
            Organization municipality = _registerService.GetOrganizationByUserName();
            return municipality.systemId;
        }

        private void DeleteCoverageDataset(Dataset dataset)
        {
            if (dataset.Coverage != null)
            {
                for (int i = 0; i < dataset.Coverage.Count; i++)
                {
                    dataset.Coverage[i].DatasetId = Guid.Empty;
                    dataset.Coverage[i].dataset = null;
                    _registerItemService.DeleteCoverage(dataset.Coverage[i]);
                }
                dataset.Coverage.Clear();
            }
        }

        private Dataset initialisationDataset(Dataset inputDataset, Dataset originalDataset = null, CoverageDataset inputCoverage = null)
        {
            Dataset dataset = GetDataset(originalDataset);
            dataset.systemId = inputDataset.GetSystemId();
            dataset.modified = dataset.GetDateModified();
            dataset.dateSubmitted = dataset.GetDateSubmbitted();
            dataset.registerId = inputDataset.register.GetSystemId();
            dataset.register = GetRegister(inputDataset.register, dataset.register);
            dataset.DatasetType = dataset.GetDatasetType();
            dataset.statusId = dataset.SetStatusId();
            dataset.dokStatusId = inputDataset.GetDokStatus();
            dataset.dokStatusDateAccepted = inputDataset.GetDokStatusDateAccepted();
            dataset.Kandidatdato = inputDataset.Kandidatdato;
            dataset.versionNumber = dataset.GetVersionNr();
            dataset.name = inputDataset.GetName();
            dataset.seoname = RegisterUrls.MakeSeoFriendlyString(dataset.name);
            dataset.description = inputDataset.GetDescription();
            dataset.versioningId = GetVersioningId(dataset);
            Guid originalDatasetownerId = GetDatasetOriginalOwnerId(originalDataset);
            dataset.datasetownerId = GetDatasetOwnerId(inputDataset.datasetownerId);
            dataset.datasetowner = (Organization)_registerItemService.GetRegisterItemBySystemId(dataset.datasetownerId);
            dataset.submitterId = GetSubmitterId(inputDataset.submitterId);
            dataset.submitter = (Organization)_registerItemService.GetRegisterItemBySystemId(dataset.submitterId);
            dataset.DistributionUrl = inputDataset.GetDistributionUrl();
            dataset.MetadataUrl = inputDataset.GetMetadataUrl();
            dataset.PresentationRulesUrl = inputDataset.GetPresentationRulesUrl();
            dataset.ProductSheetUrl = inputDataset.GetProductSheetUrl();
            dataset.ProductSpecificationUrl = inputDataset.GetProductSpecificationUrl();
            dataset.UuidService = inputDataset.UuidService;
            dataset.WmsUrl = inputDataset.GetWmsUrl();
            dataset.DistributionFormat = inputDataset.GetDistributionFormat();
            dataset.DistributionArea = inputDataset.GetDistributionArea();
            dataset.Notes = inputDataset.GetNotes();
            dataset.ThemeGroupId = inputDataset.GetThemeGroupId();
            dataset.datasetthumbnail = inputDataset.Getdatasetthumbnail();
            dataset.Uuid = inputDataset.Uuid;
            dataset.dokDeliveryMetadataStatusId = _datasetDeliveryService.GetMetadataStatus(inputDataset.Uuid, inputDataset.dokDeliveryMetadataStatusAutoUpdate, inputDataset.dokDeliveryMetadataStatusId);
            dataset.dokDeliveryMetadataStatusNote = inputDataset.dokDeliveryMetadataStatusNote;
            dataset.dokDeliveryMetadataStatusAutoUpdate = inputDataset.dokDeliveryMetadataStatusAutoUpdate;
            dataset.dokDeliveryProductSheetStatusId = _registerService.GetDOKStatus(inputDataset.GetProductSheetUrl(), inputDataset.dokDeliveryProductSheetStatusAutoUpdate, inputDataset.dokDeliveryProductSheetStatusId);
            dataset.dokDeliveryProductSheetStatusNote = inputDataset.dokDeliveryProductSheetStatusNote;
            dataset.dokDeliveryProductSheetStatusAutoUpdate = inputDataset.dokDeliveryProductSheetStatusAutoUpdate;
            dataset.dokDeliveryPresentationRulesStatusId = _registerService.GetDOKStatus(inputDataset.GetPresentationRulesUrl(), inputDataset.dokDeliveryPresentationRulesStatusAutoUpdate, inputDataset.dokDeliveryPresentationRulesStatusId);
            dataset.dokDeliveryPresentationRulesStatusNote = inputDataset.dokDeliveryPresentationRulesStatusNote;
            dataset.dokDeliveryPresentationRulesStatusAutoUpdate = inputDataset.dokDeliveryPresentationRulesStatusAutoUpdate;
            dataset.dokDeliveryProductSpecificationStatusId = _registerService.GetDOKStatus(inputDataset.GetProductSpecificationUrl(), inputDataset.dokDeliveryProductSpecificationStatusAutoUpdate, inputDataset.dokDeliveryProductSpecificationStatusId);
            dataset.dokDeliveryProductSpecificationStatusNote = inputDataset.dokDeliveryProductSpecificationStatusNote;
            dataset.dokDeliveryProductSpecificationStatusAutoUpdate = inputDataset.dokDeliveryProductSpecificationStatusAutoUpdate;
            dataset.dokDeliveryWmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(inputDataset.Uuid, inputDataset.dokDeliveryWmsStatusAutoUpdate, inputDataset.dokDeliveryWmsStatusId, inputDataset.UuidService);
            dataset.dokDeliveryWmsStatusNote = inputDataset.dokDeliveryWmsStatusNote;
            dataset.dokDeliveryWmsStatusAutoUpdate = inputDataset.dokDeliveryWmsStatusAutoUpdate;
            dataset.dokDeliveryWfsStatusId = _datasetDeliveryService.GetWfsStatus(inputDataset.Uuid, inputDataset.dokDeliveryWfsStatusAutoUpdate, inputDataset.dokDeliveryWfsStatusId);
            dataset.dokDeliveryWfsStatusNote = inputDataset.dokDeliveryWfsStatusNote;
            dataset.dokDeliveryWfsStatusAutoUpdate = inputDataset.dokDeliveryWfsStatusAutoUpdate;
            dataset.dokDeliverySosiRequirementsStatusId = _registerService.GetSosiRequirements(inputDataset.Uuid, inputDataset.GetProductSpecificationUrl(), inputDataset.dokDeliverySosiStatusAutoUpdate, inputDataset.dokDeliverySosiRequirementsStatusId);
            dataset.dokDeliverySosiRequirementsStatusNote = inputDataset.dokDeliverySosiRequirementsStatusNote;
            dataset.dokDeliverySosiStatusAutoUpdate = inputDataset.dokDeliverySosiStatusAutoUpdate;
            dataset.dokDeliveryGmlRequirementsStatusId = _registerService.GetGmlRequirements(inputDataset.Uuid, inputDataset.dokDeliveryGmlRequirementsStatusAutoUpdate, inputDataset.dokDeliveryGmlRequirementsStatusId);
            dataset.dokDeliveryGmlRequirementsStatusNote = inputDataset.dokDeliveryGmlRequirementsStatusNote;
            dataset.dokDeliveryGmlRequirementsStatusAutoUpdate = inputDataset.dokDeliveryGmlRequirementsStatusAutoUpdate;
            dataset.dokDeliveryAtomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(inputDataset.Uuid, inputDataset.dokDeliveryAtomFeedStatusAutoUpdate, inputDataset.dokDeliveryAtomFeedStatusId);
            dataset.dokDeliveryAtomFeedStatusNote = inputDataset.dokDeliveryAtomFeedStatusNote;
            dataset.dokDeliveryAtomFeedStatusAutoUpdate = inputDataset.dokDeliveryAtomFeedStatusAutoUpdate;
            dataset.SpecificUsage = inputDataset.SpecificUsage;
            dataset.restricted = inputDataset.restricted;
            dataset.dokDeliveryDistributionStatusId = _registerService.GetDeliveryDownloadStatus(dataset.Uuid, dataset.dokDeliveryDistributionStatusAutoUpdate, dataset.dokDeliveryDistributionStatusId);
            initialisationCoverageDataset(inputCoverage, dataset, originalDatasetownerId);
            dataset.dokDeliveryDistributionStatusNote = inputDataset.dokDeliveryDistributionStatusNote;
            dataset.dokDeliveryDistributionStatusAutoUpdate = inputDataset.dokDeliveryDistributionStatusAutoUpdate;
            dataset.dokDeliveryDistributionStatusId = inputDataset.dokDeliveryDistributionStatusId;
            dataset.dokDeliveryDistributionStatusId = _registerService.GetDeliveryDownloadStatus(dataset.Uuid, dataset.dokDeliveryDistributionStatusAutoUpdate, dataset.dokDeliveryDistributionStatusId);

            dataset.RegionalPlan = inputDataset.RegionalPlan;
            dataset.RegionalPlanNote = inputDataset.RegionalPlanNote;
            dataset.MunicipalSocialPlan = inputDataset.MunicipalSocialPlan;
            dataset.MunicipalSocialPlanNote = inputDataset.MunicipalSocialPlanNote;
            dataset.MunicipalLandUseElementPlan = inputDataset.MunicipalLandUseElementPlan;
            dataset.MunicipalLandUseElementPlanNote = inputDataset.MunicipalLandUseElementPlanNote;
            dataset.ZoningPlanArea = inputDataset.ZoningPlanArea;
            dataset.ZoningPlanAreaNote = inputDataset.ZoningPlanAreaNote;
            dataset.ZoningPlanDetails = inputDataset.ZoningPlanDetails;
            dataset.ZoningPlanDetailsNote = inputDataset.ZoningPlanDetailsNote;
            dataset.BuildingMatter = inputDataset.BuildingMatter;
            dataset.BuildingMatterNote = inputDataset.BuildingMatterNote;
            dataset.PartitionOff = inputDataset.PartitionOff;
            dataset.PartitionOffNote = inputDataset.PartitionOffNote;
            dataset.EenvironmentalImpactAssessment = inputDataset.EenvironmentalImpactAssessment;
            dataset.EenvironmentalImpactAssessmentNote = inputDataset.EenvironmentalImpactAssessmentNote;

            return dataset;
        }

        private Guid GetSubmitterId(Guid submitterId)
        {
            if (submitterId == null || submitterId == Guid.Empty)
            {
                Organization submitter = _registerService.GetOrganizationByUserName();
                return submitter.systemId;
            }
            return submitterId;
        }

        private Guid GetDatasetOriginalOwnerId(Dataset originalDataset)
        {
            if (originalDataset != null)
            {
                return originalDataset.datasetownerId;
            }
            else {
                return Guid.Empty;
            }
        }

        private Models.Register GetRegister(Models.Register inputRegister, Models.Register register)
        {
            if (register == null)
            {
                return inputRegister;
            }
            else {
                return register;
            }
        }

        private void initialisationCoverageDataset(CoverageDataset coverage, Dataset dataset, Guid? originalDatasetOwnerId = null)
        {
            if (coverage != null)
            {
                CoverageDataset originalCoverage = _registerItemService.GetMunicipalityCoverage(dataset, originalDatasetOwnerId);

                if (dataset.IsMunicipalDataset())
                {
                    if (originalCoverage != null)
                    {
                        originalCoverage.MunicipalityId = dataset.datasetownerId;
                        originalCoverage.CoverageDOKStatusId = dataset.dokStatusId;
                        originalCoverage.ConfirmedDok = GetConfirmedDok(coverage);
                        _registerItemService.Save();
                    }
                }
                else
                {
                    if (originalCoverage == null)
                    {
                        CoverageDataset newCoverage = new CoverageDataset()
                        {
                            CoverageId = Guid.NewGuid(),
                            CoverageDOKStatus = coverage.CoverageDOKStatus,
                            CoverageDOKStatusId = coverage.CoverageDOKStatusId,
                            ConfirmedDok = coverage.ConfirmedDok,
                            DatasetId = dataset.systemId,
                            MunicipalityId = SetMunicipality(),
                            Note = coverage.Note,
                        };
                        _registerItemService.SaveNewCoverage(newCoverage);
                        dataset.Coverage.Add(newCoverage);
                    }
                    else {
                        originalCoverage.ConfirmedDok = coverage.ConfirmedDok;
                        originalCoverage.CoverageDOKStatusId = coverage.CoverageDOKStatusId;
                        originalCoverage.Note = coverage.Note;
                    }
                }
            }
            else if (coverage == null && dataset.IsMunicipalDataset())
            {
                dataset.Coverage.Add(_registerItemService.NewCoverage(dataset));
            }
        }


        private Dataset GetDataset(Dataset originalDataset)
        {
            if (originalDataset != null)
            {
                return originalDataset;
            }
            else {
                return new Dataset();
            }
        }
    }
}
