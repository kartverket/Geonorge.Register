using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Versioning;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services.Search;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Kartverket.Register.Services.Translation;
using System.Web;
using System.Web.Configuration;
using Resources;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class RegistersController : Controller
    {
        private readonly RegisterDbContext _db;

        private readonly IVersioningService _versioningService;
        private readonly IRegisterService _registerService;
        private readonly ISearchService _searchService;
        private readonly IRegisterItemService _registerItemService;
        private readonly IAccessControlService _accessControlService;
        private readonly ITranslationService _translationService;
        private readonly IInspireDatasetService _inspireDatasetService;
        private readonly IGeodatalovDatasetService _geodatalovDatasetService;
        private readonly IInspireMonitoringService _inspireMonitoringService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IStatusReportService _statusReportService;

        public RegistersController(ITranslationService translationService,
            RegisterDbContext dbContext, IRegisterItemService registerItemService, ISearchService searchService, IVersioningService versioningService,
            IRegisterService registerService, IAccessControlService accessControlService, IInspireDatasetService inspireDatasetService, IGeodatalovDatasetService geodatalovService, IInspireMonitoringService inspireMonitoringService, ISynchronizationService synchronizationService, IStatusReportService statusReportService)
        {
            _db = dbContext;
            _registerItemService = registerItemService;
            _searchService = searchService;
            _versioningService = versioningService;
            _registerService = registerService;
            _accessControlService = accessControlService;
            _translationService = translationService;
            _inspireDatasetService = inspireDatasetService;
            _geodatalovDatasetService = geodatalovService;
            _inspireMonitoringService = inspireMonitoringService;
            _synchronizationService = synchronizationService;
            _statusReportService = statusReportService;
        }

        // GET: Registers
        public ActionResult Index()
        {
            RemoveSessionSearchParams();

            return View(_registerService.GetRegistersGrouped());
        }

        [Route("setculture/{culture}")]
        public ActionResult SetCulture(string culture, string returnUrl)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];

            if (cookie != null)
            {
                if (cookie.Domain != ".geonorge.no")
                {
                    HttpCookie oldCookie = new HttpCookie("_culture");
                    oldCookie.Domain = cookie.Domain;
                    oldCookie.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(oldCookie);
                }
            }

            if (cookie != null)
            {
                cookie.Value = culture;   // update cookie value
                cookie.Expires = DateTime.Now.AddYears(1);
                if (!Request.IsLocal)
                    cookie.Domain = ".geonorge.no";
            }
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
                if (!Request.IsLocal)
                    cookie.Domain = ".geonorge.no";
            }
            Response.Cookies.Add(cookie);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index");
        }


        // GET: Registers/Details Inspire registry/5
        [Route("register/inspire-statusregister")]
        [Route("register/inspire-statusregister/{filterOrganization}")]
        public ActionResult DetailsInspireStatusRegistry(string sorting, int? page, string format, FilterParameters filter)
        {
            RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer();
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var register = _registerService.GetInspireStatusRegister();
            if (register == null) return HttpNotFound();

            filter.InspireRegisteryType = GetInspireRegistryType(filter.InspireRegisteryType);
            register = FilterRegisterItems(register, filter);

            var viewModel = new RegisterV2ViewModel(register, page);
            viewModel.AccessRegister = _accessControlService.AccessViewModel(viewModel);
            viewModel.SelectedInspireRegisteryType = filter.InspireRegisteryType;

            if (viewModel.SelectedInspireRegisteryTypeIsReport())
            {
                viewModel.InspireReport = _inspireMonitoringService.GetInspireReportViewModel(register, filter);
            }

            ItemsOrderBy(sorting, viewModel);
            ViewbagsRegisterDetails(sorting, page, filter, viewModel);

            return View(viewModel);
        }

        // POST: Registers/Details/5
        [Authorize]
        [HttpPost]
        [Route("register/inspire-statusregister")]
        public ActionResult DetailsInspireStatusRegistry(FilterParameters filter, string dataset, string service)
        {
            if (IsAdmin())
            {
                if (dataset != null)
                {
                    StartSynchronizationDataset();
                }
                else if (service != null)
                {
                    StartSynchronizationService();
                }

                return Redirect(Request.RawUrl);
            }

            return HttpNotFound();
        }

        // GET: Registers/Details DOK/5
        [Route("register/det-offentlige-kartgrunnlaget")]
        [Route("register/det-offentlige-kartgrunnlagetr/{filterOrganization}")]
        public ActionResult DetailsDokStatusRegistry(string sorting, int? page, string format, FilterParameters filter)
        {
            RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer();
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var register = _registerService.GetDokStatusRegister();
            if (register == null) return HttpNotFound();

            register = FilterRegisterItems(register, filter);

            StatusReport statusReport = filter.SelectedDokReport != null ? _statusReportService.GetStatusReportById(filter.SelectedDokReport) : _statusReportService.GetLatestReport();
            

            var viewModel = new RegisterV2ViewModel(register, null, statusReport, _statusReportService.GetStatusReports(12), filter.StatusType);
            viewModel.SelectedDokTab = filter.DokSelectedTab;
            viewModel.AccessRegister = _accessControlService.AccessViewModel(viewModel);

            ItemsOrderBy(sorting, viewModel);
            ViewBagOrganizationMunizipality(filter.municipality);
            ViewbagsRegisterDetails(sorting, page, filter, viewModel);
            return View(viewModel);
        }

        private void StartSynchronizationDataset()
        {
            HttpClient hc = new HttpClient();
            hc.GetAsync(WebConfigurationManager.AppSettings["RegistryUrl"] + "api/metadata/synchronize/inspire-statusregister/dataset");
        }

        private void StartSynchronizationService()
        {
            HttpClient hc = new HttpClient();
            hc.GetAsync(WebConfigurationManager.AppSettings["RegistryUrl"] + "api/metadata/synchronize/inspire-statusregister/dataservices");
        }


        // GET: Registers/Details/5
        [Route("register/{registername}")]
        [Route("register/{registername}.{format}")]
        [Route("register/{registername}/{filterOrganization}")]
        [Route("subregister/{parentRegister}/{owner}/{registername}.{format}")]
        [Route("subregister/{parentRegister}/{owner}/{registername}")]
        public ActionResult Details(string parentRegister, string owner, string registername, string sorting, int? page, string format, FilterParameters filter)
        {
            RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer();
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var register = _registerService.GetRegister(parentRegister, registername);
            if (register == null) return HttpNotFound();

            register = FilterRegisterItems(register, filter);
            var viewModel = new RegisterV2ViewModel(register);
            viewModel.MunicipalityCode = filter.municipality;
            viewModel.Municipality = _registerItemService.GetMunicipalityOrganizationByNr(viewModel.MunicipalityCode);
            viewModel.AccessRegister = _accessControlService.AccessViewModel(viewModel);

            ItemsOrderBy(sorting, viewModel);
            ViewBagOrganizationMunizipality(filter.municipality);
            ViewbagsRegisterDetails(sorting, page, filter, viewModel);
            return View(viewModel);
        }


        private string GetInspireRegistryType(string filter)
        {
            if (filter == "report")
            {
                if (!IsAdmin())
                {
                    return "dataset";
                }
            }
            return filter;
        }

        private void ItemsOrderBy(string sorting, RegisterV2ViewModel viewModel)
        {
            viewModel.RegisterItemsV2 = _registerItemService.OrderBy(viewModel.RegisterItemsV2, sorting);
            viewModel.RegisterItems = _registerItemService.OrderBy(viewModel.RegisterItems, sorting);
            viewModel.Subregisters = _registerService.OrderBy(viewModel.Subregisters, sorting);
        }

        [Route("register/{registername}/{itemowner}/{itemname}.{format}")]
        [Route("register/{registername}/{itemowner}/{itemname}")]
        [Route("register/{registername}/{itemowner}/{itemname}/{systemId}")]
        public ActionResult DetailsRegisterItem(string registername, string itemowner, string itemname, string format, string systemId, string InspireRegisteryType = null)
        {
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            RegisterItemV2ViewModel viewModel;
            try
            {
                if (string.IsNullOrWhiteSpace(systemId))
                {
                    viewModel = GetRegisterItem(null, registername, itemowner, itemname, InspireRegisteryType);
                }
                else
                {
                    viewModel = new RegisterItemV2ViewModel(_registerItemService.GetRegisterItemBySystemId(Guid.Parse(systemId)));
                }
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }
            viewModel.AccessRegisterItem = _accessControlService.Access(viewModel);
            if (string.IsNullOrWhiteSpace(viewModel.Name))
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }



        [Route("subregister/versjoner/{parentregister}/{parentowner}/{registername}/{itemowner}/{itemname}/{version}/no.{format}")]
        [Route("subregister/versjoner/{parentregister}/{parentowner}/{registername}/{itemowner}/{itemname}/{version}/no")]
        [Route("register/versjoner/{registername}/{itemowner}/{itemname}/{version}/no.{format}")]
        [Route("register/versjoner/{registername}/{itemowner}/{itemname}/{version}/no")]
        public ActionResult DetailsRegisterItem(string registername, string itemowner, string itemname, int version, string format, string parentregister)
        {
            string redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);
            var viewModel = GetRegisterItem(parentregister, registername, itemowner, itemname, null, version);
            viewModel.AccessRegisterItem = _accessControlService.Access(viewModel);

            if (viewModel.SystemId == Guid.Empty)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }


        [Route("subregister/versjoner/{parentRegister}/{owner}/{registername}/{registerItemOwner}/{itemname}.{format}")]
        [Route("register/versjoner/{registername}/{registerItemOwner}/{itemname}.{format}")]
        [Route("subregister/versjoner/{parentRegister}/{owner}/{registername}/{registerItemOwner}/{itemname}")]
        [Route("register/versjoner/{registername}/{registerItemOwner}/{itemname}")]
        public ActionResult DetailsRegisterItemVersions(string registername, string parentRegister, string itemname, string registerItemOwner, string format)
        {
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var versionsItem = _versioningService.Versions(registername, parentRegister, itemname);
            var model = new VersionsViewModel(versionsItem);
            model.AccessCreateNewVersions = _accessControlService.AccessCreateNewVersion(model.CurrentVersion);
            model.CurrentVersion.AccessRegisterItem = _accessControlService.Access(model.CurrentVersion);

            ViewBag.registerItemOwner = registerItemOwner;
            return View(model);
        }


        // GET: Register/Create
        [Authorize]
        [Route("ny")]
        public ActionResult Create()
        {
            if (IsAdmin())
            {
                ViewBag.containedItemClass = new SelectList(_db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);
                return View();
            }
            return HttpNotFound();
        }


        // POST: Register/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [Route("ny")]
        public ActionResult Create(Models.Register register)
        {
            if (IsAdmin())
            {
                if (_registerService.RegisterNameAlredyExist(register)) ModelState.AddModelError("ErrorMessage", Registers.ErrorMessageValidationName);

                if (ModelState.IsValid)
                {
                    register.systemId = Guid.NewGuid();
                    if (register.name == null) register.name = "ikke angitt";
                    register.systemId = Guid.NewGuid();
                    register.modified = DateTime.Now;
                    register.dateSubmitted = DateTime.Now;
                    register.statusId = "Submitted";
                    register.seoname = RegisterUrls.MakeSeoFriendlyString(register.name);
                    register.containedItemClass = register.containedItemClass;

                    _db.Registers.Add(register);
                    _db.SaveChanges();

                    Organization submitterOrganisasjon = _registerService.GetOrganizationByUserName();
                    register.ownerId = submitterOrganisasjon.systemId;
                    register.managerId = submitterOrganisasjon.systemId;

                    _db.Entry(register).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Redirect("/");
                }
                ViewBag.containedItemClass = new SelectList(_db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);
                return View();
            }
            return HttpNotFound();
        }


        [Authorize]
        [Route("rediger/{registername}")]
        public ActionResult Edit(string registername)
        {
            var register = _registerService.GetRegister(null, registername);

            if (register == null) return HttpNotFound();

            if (IsAdmin())
            {
                Viewbags(register);
                return View(register);
            }
            return HttpNotFound();

        }

        // POST: Registers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("rediger/{registername}")]
        [Authorize]
        public ActionResult Edit(Models.Register register, string registername, string accessRegister)
        {
            if (IsAdmin())
            {
                if (_registerService.validationName(register)) ModelState.AddModelError("ErrorMessage", Registers.ErrorMessageValidationName);
                Models.Register originalRegister = _registerService.GetRegister(null, registername);

                if (ModelState.IsValid)
                {
                    if (register.name != null) originalRegister.name = register.name;

                    if (register.description != null) originalRegister.description = register.description;
                    if (register.ownerId != null) originalRegister.ownerId = register.ownerId;
                    if (register.managerId != null) originalRegister.managerId = register.managerId;
                    if (register.targetNamespace != null) originalRegister.targetNamespace = register.targetNamespace;
                    originalRegister.accessId = register.accessId;
                    originalRegister.parentRegisterId = register.parentRegisterId;
                    originalRegister.seoname = RegisterUrls.MakeSeoFriendlyString(originalRegister.name);
                    originalRegister.modified = DateTime.Now;
                    if (register.statusId != null) originalRegister = _registerService.SetStatus(register, originalRegister);
                    _translationService.UpdateTranslations(register, originalRegister);
                    _db.Entry(originalRegister).State = EntityState.Modified;
                    _db.SaveChanges();
                    Viewbags(register);

                    return Redirect(RegisterUrls.registerUrl(null, null, registername));
                }
                Viewbags(register);
                return View(originalRegister);
            }
            return HttpNotFound();
        }




        // GET: Edit DOK-Municipal-Dataset
        [Authorize]
        [Route("dok/kommunalt/{municipalityCode}/rediger")]
        public ActionResult EditDokMunicipal(string municipalityCode)
        {
            if (_accessControlService.AccessEditOrCreateDOKMunicipalBySelectedMunicipality(municipalityCode))
            {
                var municipality = _registerItemService.GetMunicipalityOrganizationByNr(municipalityCode);
                Models.Register dokMunicipalRegister = _registerService.GetDokMunicipalRegister();
                var municipalDatasets = _registerService.GetDatasetBySelectedMunicipality(dokMunicipalRegister, municipality);

                if (municipality != null)
                {
                    var dokMunicipalEditList = new List<DokMunicipalEdit>();
                    foreach (Dataset dataset in municipalDatasets)
                    {
                        var row = new DokMunicipalEdit(dataset, municipality);
                        dokMunicipalEditList.Add(row);
                    }
                    ViewBag.selectedMunicipality = municipality.name;
                    ViewBag.selectedMunicipalityCode = municipalityCode;
                    var statusDokMunicipalList = CreateStatusDokMunicipalList();

                    ViewBag.statusDOKMunicipal = new SelectList(statusDokMunicipalList, "value", "description", DOKmunicipalStatus(municipality));
                    return View(dokMunicipalEditList);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return HttpNotFound();
        }


        // POST: DOK-Municipal-Dataset
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("dok/kommunalt/{municipalityCode}/rediger")]
        [Authorize]
        public ActionResult EditDokMunicipal(List<DokMunicipalEdit> dokMunicipalList, string municipalityCode, string statusDokMunicipal)
        {
            if (_accessControlService.AccessEditOrCreateDOKMunicipalBySelectedMunicipality(municipalityCode))
            {
                Organization municipality = _registerItemService.GetMunicipalityOrganizationByNr(municipalityCode);
                if (municipality != null)
                {
                    municipality.UpdateDOKMunicipalStatus(statusDokMunicipal);
                    _registerItemService.SaveEditedRegisterItem(municipality);
                }

                CoverageService coverage = new CoverageService(_db);
                coverage.SetCoverage(municipalityCode);

                foreach (DokMunicipalEdit item in dokMunicipalList)
                {
                    Dataset originalDataset = (Dataset)_registerItemService.GetRegisterItemBySystemId(item.Id);
                    originalDataset.modified = DateTime.Now;
                    CoverageDataset originalCoverage = originalDataset.GetCoverageByOwner(item.MunicipalityId);
                    if (item.Delete)
                    {
                        if (originalCoverage != null)
                        {
                            _registerItemService.DeleteCoverage(originalCoverage);
                        }
                        _registerItemService.SaveDeleteRegisterItem(originalDataset);
                    }
                    else
                    {
                        var coverageFound = originalCoverage?.Coverage ?? false;
                        try
                        {
                            coverageFound = coverage.GetCoverage(originalDataset.Uuid);
                        }
                        catch (System.Net.WebException)
                        {
                            TempData["failure"] = "Tjenesten som henter dekning feilet";
                        }
                        if (originalCoverage == null)
                        {
                            var coverageNew = CreateNewCoverage(item, originalDataset, municipalityCode, coverageFound);
                            _db.Database.ExecuteSqlCommand(
                                "INSERT INTO CoverageDatasets (ConfirmedDok, Coverage, Note," +
                                "RegionalPlan, MunicipalSocialPlan, MunicipalLandUseElementPlan, ZoningPlanArea, ZoningPlanDetails, " +
                                "BuildingMatter, PartitionOff, EenvironmentalImpactAssessment, SuitabilityAssessmentText, " +
                                "CoverageId, MunicipalityId, DatasetId, CoverageDOKStatusId  ) " +
                                " VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15 " +
                                ")",
                                coverageNew.ConfirmedDok,
                                coverageFound,
                                coverageNew.Note,
                                item.RegionalPlan,
                                item.MunicipalSocialPlan,
                                item.MunicipalLandUseElementPlan,
                                item.ZoningPlanArea,
                                item.ZoningPlanDetails,
                                item.BuildingMatter,
                                item.PartitionOff,
                                item.EnvironmentalImpactAssessment,
                                item.SuitabilityAssessmentText,
                                coverageNew.CoverageId,
                                coverageNew.MunicipalityId,
                                originalDataset.systemId,
                                coverageNew.CoverageDOKStatusId);
                        }
                        else
                        {
                            _db.Database.ExecuteSqlCommand(
                                "UPDATE CoverageDatasets SET ConfirmedDok = @p0 , " +
                                "Coverage = @p1, " +
                                "Note = @p2, " +
                                "RegionalPlan = @p3, " +
                                "MunicipalSocialPlan = @p4, " +
                                "MunicipalLandUseElementPlan = @p5, " +
                                "ZoningPlanArea = @p6, " +
                                "ZoningPlanDetails = @p7, " +
                                "BuildingMatter = @p8, " +
                                "PartitionOff = @p9, " +
                                "EenvironmentalImpactAssessment = @p10, " +
                                "SuitabilityAssessmentText = @p11 " +
                                "WHERE CoverageId = @p12",
                                item.Confirmed,
                                coverageFound,
                                item.Note,
                                item.RegionalPlan,
                                item.MunicipalSocialPlan,
                                item.MunicipalLandUseElementPlan,
                                item.ZoningPlanArea,
                                item.ZoningPlanDetails,
                                item.BuildingMatter,
                                item.PartitionOff,
                                item.EnvironmentalImpactAssessment,
                                item.SuitabilityAssessmentText,
                                originalCoverage.CoverageId);
                        }
                    }
                }

                _db.Database.ExecuteSqlCommand("update Registers set modified = GETDATE() where systemid='E807439B-2BFC-4DA5-87C0-B40E7B0CDFB8'");

                return Redirect("/register/det-offentlige-kartgrunnlaget-kommunalt?municipality=" + municipalityCode);
            }
            return HttpNotFound();
        }


        // GET: Registers/Delete/5
        [Authorize]
        [Route("slett/{registername}")]
        public ActionResult Delete(string registername)
        {
            if (IsAdmin())
            {
                Models.Register register = _registerService.GetRegister(null, registername);
                if (register == null) return HttpNotFound();
                return View(register);
            }
            return HttpNotFound();
        }


        // POST: Registers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("slett/{registername}")]
        [Authorize]
        public ActionResult DeleteConfirmed(string registername)
        {
            if (IsAdmin())
            {
                Models.Register register = _registerService.GetRegister(null, registername);

                if (_registerService.RegisterHasChildren(null, registername))
                {
                    ModelState.AddModelError("ErrorMessageDelete", Registers.ErrorMessageDelete);
                    return View(register);
                }
                else
                {
                    _db.Registers.Remove(register);
                    _db.SaveChanges();
                    return Redirect("/");
                }
            }
            return HttpNotFound();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void RemoveSessionSearchParams()
        {
            Session["sortingType"] = null;
            Session["text"] = null;
            Session["filterVertikalt"] = null;
            Session["filterHorisontalt"] = null;
            Session["InspireRequirement"] = null;
            Session["nationalRequirement"] = null;
            Session["nationalSeaRequirement"] = null;
            Session["inspireRegistryTab"] = null;
        }

        private void Viewbags(Models.Register register)
        {
            ViewBag.statusId = new SelectList(_db.Statuses.ToList().Select(s => new { s.value, description = s.DescriptionTranslated() }).OrderBy(o => o.description), "value", "description", register.statusId);
            ViewBag.ownerId = new SelectList(_db.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", register.ownerId);
            ViewBag.parentRegisterId = new SelectList(_db.Registers.ToList().Select(s => new { s.systemId, name = s.NameTranslated(), s.containedItemClass }).Where(r => r.containedItemClass == "Register" && r.name != register.name).OrderBy(s => s.name), "systemId", "name", register.parentRegisterId);
            ViewBag.containedItemClass = new SelectList(_db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);
        }

        private void ViewBagOrganizationMunizipality(string municipalityCode)
        {
            ViewBag.organizationMunicipality = _registerItemService.GetMunicipalityOrganizationByNr(municipalityCode);
        }

        private CoverageDataset CreateNewCoverage(DokMunicipalEdit item, Dataset originalDataset, string municipalityCode, bool coverageFound)
        {
            Organization municipality = _registerItemService.GetMunicipalityOrganizationByNr(municipalityCode);
            CoverageDataset coverage = new CoverageDataset
            {
                CoverageId = Guid.NewGuid(),
                CoverageDOKStatusId = "Accepted",
                ConfirmedDok = item.Confirmed,
                Coverage = coverageFound,
                dataset = originalDataset,
                DatasetId = originalDataset.systemId,
                MunicipalityId = municipality.systemId,
                Note = item.Note
            };
            return coverage;
        }


        public string RedirectToApiIfFormatIsNotNull(string format)
        {
            if (!string.IsNullOrWhiteSpace(format))
            {
                return "/api/" + Request.FilePath + "?" + Request.QueryString;
            }
            else
            {
                format = _registerService.ContentNegotiation(ControllerContext);
                if (!string.IsNullOrWhiteSpace(format))
                {
                    return "/api/" + Request.FilePath + "." + format + "?" + Request.QueryString;
                }
            }
            return null;
        }

        private Models.Register FilterRegisterItems(Models.Register register, FilterParameters filter)
        {
            if (Search(filter)) register = _searchService.Search(register, filter.text);
            register = _registerService.FilterRegisterItems(register, filter);
            return register;
        }

        private RegisterItemV2ViewModel GetRegisterItem(string parentregister, string registername, string itemowner, string itemname, string inspireRegistryType, int version = 1)
        {
            var register = _registerService.GetRegister(parentregister, registername);

            if (register.IsInspireStatusRegister())
            {
                RegisterItemV2ViewModel viewModel = null;
                if (string.IsNullOrWhiteSpace(inspireRegistryType) || inspireRegistryType == "dataset")
                {
                    var inspireDataset = _inspireDatasetService.GetInspireDatasetByName(registername, itemname);
                    if (inspireDataset != null)
                    {
                        viewModel = new InspireDatasetViewModel(inspireDataset);
                    }
                    else
                    {
                        viewModel = new InspireDataServiceViewModel(_inspireDatasetService.GetInspireDataServiceByName(registername, itemname));
                    }
                }
                else
                {
                    viewModel = new InspireDataServiceViewModel(_inspireDatasetService.GetInspireDataServiceByName(registername, itemname));
                }
                return viewModel;
            }
            if (register.IsGeodatalovStatusRegister())
            {
                return new GeodatalovDatasetViewModel(_geodatalovDatasetService.GetGeodatalovDatasetByName(registername, itemname));
            }
            if (register.IsDokMunicipal())
            {
                return new RegisterItemV2ViewModel(_registerItemService.GetRegisterItem(parentregister, registername, itemname, version, itemowner));
            }

            if (register.ContainedItemClassIsDataset())
            {
                return new DokDatasetViewModel((Dataset)_registerItemService.GetRegisterItem(parentregister, registername, itemname, version, itemowner));
            }
            if (register.ContainedItemClassIsDocument())
            {
                return new DocumentViewModel((Document)_registerItemService.GetRegisterItem(parentregister, registername, itemname, version, itemowner));
            }

            return new RegisterItemV2ViewModel(_registerItemService.GetRegisterItem(parentregister, registername, itemname, version, itemowner));

        }

        private void ViewbagsRegisterDetails(string sorting, int? page, FilterParameters filter, RegisterV2ViewModel register)
        {
            ViewBag.search = filter.text;
            ViewBag.page = page;
            ViewBag.SortOrder = sorting ?? "";
            ViewBag.selectedMunicipalityCode = filter.municipality;
            ViewBag.municipality = _registerItemService.GetMunicipalityList();
            ViewBag.registerId = register.SystemId;
            ViewBag.register = register.Name;
            ViewBag.registerSEO = register.Seoname;
            ViewBag.InspireRegisteryType = filter.InspireRegisteryType;
            ViewBag.text = filter.text;
        }

        private static bool Search(FilterParameters filter)
        {
            return !string.IsNullOrWhiteSpace(filter.text);
        }

        private bool IsAdmin()
        {
            return _accessControlService.IsAdmin();
        }


        /// <summary>
        /// Creates a list of statuses for DOK Municipal register. 
        /// </summary>
        /// <returns></returns>
        private static List<Status> CreateStatusDokMunicipalList()
        {
            var statusDokMunicipal = new List<Status>();
            var s1 = new Status();
            s1.value = "draft";
            s1.description = "I prosess";
            var s2 = new Status();
            s2.value = "valid";
            s2.description = "Utført";
            statusDokMunicipal.Add(s1);
            statusDokMunicipal.Add(s2);
            return statusDokMunicipal;
        }

        private string DOKmunicipalStatus(Organization municipality)
        {
            if (municipality.DateConfirmedMunicipalDOK != null)
            {
                if (LastDateConfirmedIsNotFromThisYear(municipality.DateConfirmedMunicipalDOK))
                {
                    return null;
                }
                return municipality.StatusConfirmationMunicipalDOK;
            }
            return null;
        }

        private static bool LastDateConfirmedIsNotFromThisYear(DateTime? dateConfirmedMunicipalDok)
        {
            if (dateConfirmedMunicipalDok != null)
            {
                return dateConfirmedMunicipalDok.Value.Year != DateTime.Now.Year;
            }
            return false;
        }

        public void RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer()
        {
            if (Request?.UrlReferrer != null)
            {
                var registerNameReferer = "";
                var pathReferer = Request.UrlReferrer.AbsolutePath;
                if (pathReferer.Contains("/"))
                {
                    var registerNameRefererObject = pathReferer.Split('/');
                    if (registerNameRefererObject.Count() > 2)
                        registerNameReferer = registerNameRefererObject[2];
                }

                var registerNameCurrent = "";
                if (Request.Url != null)
                {
                    var pathCurrent = Request.Url.AbsolutePath;
                    if (pathCurrent.Contains("/"))
                    {
                        var registerNameCurrentObject = pathCurrent.Split('/');
                        if (registerNameCurrentObject.Count() > 2)
                            registerNameCurrent = registerNameCurrentObject[2];
                    }
                }

                if (Request.Url != null && (Request.UrlReferrer.Host != Request.Url.Host))
                    RemoveSessionSearchParams();

                if (!registerNameReferer.StartsWith("search") && registerNameReferer != registerNameCurrent && registerNameReferer != "versjoner")
                    RemoveSessionSearchParams();
            }
        }
    }
}
