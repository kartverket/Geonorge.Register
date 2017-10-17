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
using Kartverket.Register.Services.Translation;
using System.Web;
using Resources;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class RegistersController : Controller
    {
        private readonly RegisterDbContext db;

        private IVersioningService _versioningService;
        private IRegisterService _registerService;
        private ISearchService _searchService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _accessControlService;
        private ITranslationService _translationService;
        private IInspireDatasetService _inspireDatasetService;
        private IGeodatalovDatasetService _geodatalovDatasetService;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RegistersController(ITranslationService translationService, RegisterDbContext dbContext)
        {
            db = dbContext;
            _registerItemService = new RegisterItemService(db);
            _searchService = new SearchService(db);
            _versioningService = new VersioningService(db);
            _registerService = new RegisterService(db);
            _accessControlService = new AccessControlService();
            _translationService = translationService;
            _inspireDatasetService = new InspireDatasetService(db);
            _geodatalovDatasetService = new GeodatalovDatasetService(db);
        }

        // GET: Registers
        public ActionResult Index()
        {
            removeSessionSearchParams();

            return View(db.Registers.ToList().OrderBy(r => r.NameTranslated()).ToList());
        }

        [Route("setculture/{culture}")]
        public ActionResult SetCulture(string culture, string ReturnUrl)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);

            if (!string.IsNullOrEmpty(ReturnUrl))
                return Redirect(ReturnUrl);
            else
            return RedirectToAction("Index");
        }


        // GET: Registers/Details/5
        [Route("register/{registername}")]
        [Route("register/{registername}.{format}")]
        [Route("register/{registername}/{filterOrganization}")]
        [Route("subregister/{parentRegister}/{owner}/{registername}.{format}")]
        [Route("subregister/{parentRegister}/{owner}/{registername}")]
        public ActionResult Details(string parentRegister, string owner, string registername, string sorting, int? page, string format, FilterParameters filter)
        {
            CheckReferrer();
            DokOrderBy(sorting);
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var register = _registerService.GetRegister(parentRegister, registername);
            if (register != null)
            {
                register = RegisterItems(register, filter, page);
                var viewModel = new RegisterV2ViewModel(register);
                if (viewModel.IsDokMunicipal() && string.IsNullOrEmpty(filter.municipality))
                {
                    var municipality = _accessControlService.GetMunicipality();
                    if (municipality != null)
                    {
                        ViewBagOrganizationMunizipality(municipality.value);
                        return Redirect("/register/det-offentlige-kartgrunnlaget-kommunalt?municipality=" + municipality.value);
                    }
                }
                if (viewModel.IsServiceAlertRegister() && string.IsNullOrWhiteSpace(sorting))
                {
                    sorting = "dateSubmitted_desc";
                }
                viewModel.RegisterItemsV2 = _registerItemService.OrderBy(viewModel.RegisterItemsV2, sorting); // Todo flytte sortering av register.registeritem 
                viewModel.RegisterItems = _registerItemService.OrderBy(viewModel.RegisterItems, sorting); // Todo midlertidig.. 
                viewModel.Subregisters = _registerService.OrderBy(viewModel.Subregisters, sorting); // Todo midlertidig.. 
                ViewBagOrganizationMunizipality(filter.municipality);
                ViewbagsRegisterDetails(owner, sorting, page, filter, viewModel);
                return View(viewModel);
            }
                return HttpNotFound();
        }


        [Route("register/{registername}/{itemowner}/{itemname}.{format}")]
        [Route("register/{registername}/{itemowner}/{itemname}")]
        [Route("register/{registername}/{itemowner}/{itemname}/{systemId}")]
        public ActionResult DetailsRegisterItem(string registername, string itemowner, string itemname, string format, string systemId)
        {
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            RegisterItemV2ViewModel viewModel;

            if (string.IsNullOrWhiteSpace(systemId))
            {
                viewModel = GetRegisterItem(null, registername, itemowner, itemname);
            }
            else {
                viewModel = new RegisterItemV2ViewModel(_registerItemService.GetRegisterItemBySystemId(Guid.Parse(systemId)));
            }
            return View(viewModel);
        }


        [Route("register/versjoner/{registername}/{itemowner}/{itemname}/{version}/no.{format}")]
        [Route("register/versjoner/{registername}/{itemowner}/{itemname}/{version}/no")]
        public ActionResult DetailsRegisterItem(string registername, string itemowner, string itemname, int version, string format)
        {
            string redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);
            var viewModel = new RegisterItemV2ViewModel(_registerItemService.GetRegisterItem(null, registername, itemname, version));

            return View(viewModel);
        }


        [Route("subregister/versjoner/{parentRegister}/{owner}/{registername}/{registerItemOwner}/{itemname}.{format}")]
        [Route("register/versjoner/{registername}/{registerItemOwner}/{itemname}.{format}")]
        [Route("subregister/versjoner/{parentRegister}/{owner}/{registername}/{registerItemOwner}/{itemname}")]
        [Route("register/versjoner/{registername}/{registerItemOwner}/{itemname}")]
        public ActionResult DetailsRegisterItemVersions(string registername, string parentRegister, string itemname, string registerItemOwner, string format)
        {
            string redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);
            VersionsItem versionsItem = _versioningService.Versions(registername, parentRegister, itemname);
            RegisterItemVeiwModel model = new RegisterItemVeiwModel(versionsItem);

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
                ViewBag.containedItemClass = new SelectList(db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);
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
                if (_registerService.validationName(register)) ModelState.AddModelError("ErrorMessage", Registers.ErrorMessageValidationName);

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

                    db.Registers.Add(register);
                    db.SaveChanges();

                    Organization submitterOrganisasjon = _registerService.GetOrganizationByUserName();
                    register.ownerId = submitterOrganisasjon.systemId;
                    register.managerId = submitterOrganisasjon.systemId;

                    db.Entry(register).State = EntityState.Modified;
                    db.SaveChanges();
                    return Redirect("/");
                }
                ViewBag.containedItemClass = new SelectList(db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);
                return View();
            }
            return HttpNotFound();
        }


        [Authorize]
        [Route("rediger/{registername}")]
        public ActionResult Edit(string registername)
        {
            Models.Register register = _registerService.GetRegister(null, registername);

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
                    db.Entry(originalRegister).State = EntityState.Modified;
                    db.SaveChanges();
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
                Organization municipality = _registerItemService.GetMunicipalityOrganizationByNr(municipalityCode);
                Models.Register dokMunicipalRegister = _registerService.GetDokMunicipalRegister();
                List<RegisterItem> municipalDatasets = _registerService.GetDatasetBySelectedMunicipality(dokMunicipalRegister, municipality);

                if (municipality != null)
                {
                    List<DokMunicipalEdit> dokMunicipalEditList = new List<DokMunicipalEdit>();
                    foreach (Dataset dataset in municipalDatasets)
                    {
                        DokMunicipalEdit row = new DokMunicipalEdit(dataset, municipality);
                        dokMunicipalEditList.Add(row);
                    }
                    ViewBag.selectedMunicipality = municipality.name;
                    ViewBag.selectedMunicipalityCode = municipalityCode;
                    List<Status> statusDOKMunicipalList = CreateStatusDOKMunicipalList();

                    ViewBag.statusDOKMunicipal = new SelectList(statusDOKMunicipalList, "value", "description", DOKmunicipalStatus(municipality));
                    return View(dokMunicipalEditList);
                }
                else {
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
        public ActionResult EditDokMunicipal(List<DokMunicipalEdit> dokMunicipalList, string municipalityCode, string statusDOKMunicipal)
        {
            if (_accessControlService.AccessEditOrCreateDOKMunicipalBySelectedMunicipality(municipalityCode))
            {
                Organization municipality = _registerItemService.GetMunicipalityOrganizationByNr(municipalityCode);
                if (municipality != null)
                {
                    municipality.UpdateDOKMunicipalStatus(statusDOKMunicipal);
                    _registerItemService.SaveEditedRegisterItem(municipality);
                }

                CoverageService coverage = new CoverageService(db);
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
                    else {
                        bool coverageFound = (originalCoverage != null) ? originalCoverage.Coverage : false;
                        try
                        {
                            coverageFound = coverage.GetCoverage(originalDataset.Uuid);
                        }
                        catch (System.Net.WebException webex)
                        {
                            TempData["failure"] = "Tjenesten som henter dekning feilet";
                        }
                        if (originalCoverage == null)
                        {
                            originalDataset.Coverage.Add(CreateNewCoverage(item, originalDataset, municipalityCode, coverageFound));
                        }
                        else
                        {
                            db.Database.ExecuteSqlCommand(
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

                db.Database.ExecuteSqlCommand("update Registers set modified = GETDATE() where systemid='E807439B-2BFC-4DA5-87C0-B40E7B0CDFB8'");

                db.SaveChanges();

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
                    db.Registers.Remove(register);
                    db.SaveChanges();
                    return Redirect("/");
                }
            }
            return HttpNotFound();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void removeSessionSearchParams()
        {
            Session["sortingType"] = null;
            Session["text"] = null;
            Session["filterVertikalt"] = null;
            Session["filterHorisontalt"] = null;
            Session["InspireRequirement"] = null;
            Session["nationalRequirement"] = null;
            Session["nationalSeaRequirement"] = null;
        }

        private void Viewbags(Models.Register register)
        {
            ViewBag.statusId = new SelectList(db.Statuses.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }).OrderBy(o => o.description), "value", "description", register.statusId);
            ViewBag.ownerId = new SelectList(db.Organizations.ToList().Select(s => new { systemId = s.systemId, name = s.NameTranslated() } ).OrderBy(s => s.name), "systemId", "name", register.ownerId);
            ViewBag.parentRegisterId = new SelectList(db.Registers.ToList().Select(s => new { systemId = s.systemId, name = s.NameTranslated(), containedItemClass = s.containedItemClass }).Where(r => r.containedItemClass == "Register" && r.name != register.name).OrderBy(s => s.name), "systemId", "name", register.parentRegisterId);
            ViewBag.containedItemClass = new SelectList(db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);
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
            db.CoverageDatasets.Add(coverage);
            return coverage;
        }

        private string GetOwner(RegisterItem registerItem)
        {
            if (registerItem.register.containedItemClass == "Document")
            {
                Document document = db.Documents.Find(registerItem.systemId);
                return document.documentowner.name;
            }
            else if (registerItem.register.containedItemClass == "Dataset")
            {
                Dataset dataset = db.Datasets.Find(registerItem.systemId);
                return dataset.datasetowner.name;
            }
            else if (registerItem.register.containedItemClass == "ServiceAlert")
            {
                ServiceAlert serviceAlert = db.ServiceAlerts.Find(registerItem.systemId);
                return serviceAlert.Owner;
            }
            else
            {
                return registerItem.submitter.seoname;
            }
        }

        private string RedirectToApiIfFormatIsNotNull(string format)
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

        private Models.Register RegisterItems(Models.Register register, FilterParameters filter, int? page)
        {
            if (Search(filter)) register = _searchService.Search(register, filter.text);
            register = _registerService.FilterRegisterItems(register, filter);
            return register;
        }

        private RegisterItemV2ViewModel GetRegisterItem(string parentregister, string registername, string itemowner, string itemname)
        {
            var register = _registerService.GetRegister(parentregister, registername);

            if (register.IsInspireStatusRegister())
            {
                return new InspireDatasetViewModel(_inspireDatasetService.GetInspireDatasetByName(registername, itemname));
            }
            if (register.IsGeodatalovStatusRegister())
            {
                return new GeodatalovDatasetViewModel(_geodatalovDatasetService.GetGeodatalovDatasetByName(registername, itemname));
            }
            if (register.IsDokMunicipal())
            {
                return new RegisterItemV2ViewModel(_registerItemService.GetRegisterItem(parentregister, registername, itemname, 1, itemowner));
            }

            return new RegisterItemV2ViewModel(_registerItemService.GetCurrentRegisterItem(parentregister, registername, itemname));
            
        }

        private void ViewbagsRegisterDetails(string owner, string sorting, int? page, FilterParameters filter, RegisterV2ViewModel register)
        {
            ViewBag.search = filter.text;
            ViewBag.page = page;
            ViewBag.SortOrder = sorting != null ? sorting : "";
            ViewBag.selectedMunicipalityCode = filter.municipality;
            ViewBag.sorting = new SelectList(db.Sorting.ToList(), "value", "description");
            ViewBag.municipality = _registerItemService.GetMunicipalityList();
            ViewBag.registerId = register.SystemId;
            ViewBag.register = register.Name;
            ViewBag.registerSEO = register.Seoname;
            ViewBag.InspireRequirement = new SelectList(db.requirements, "value", "description", null);
            ViewBag.nationalRequirement = new SelectList(db.requirements, "value", "description", null);
            ViewBag.nationalSeaRequirement = new SelectList(db.requirements, "value", "description", null);
            ViewBag.ownerSEO = owner;
        }

        private static bool Search(FilterParameters filter)
        {
            return !string.IsNullOrWhiteSpace(filter.text);
        }

        private bool IsAdmin()
        {
            return _accessControlService.IsAdmin();
        }

        private static string DefaultSortingServiceAlertRegister(string sorting, Models.Register register)
        {
            if (register.IsServiceAlertRegister())
            {
                if (string.IsNullOrWhiteSpace(sorting))
                {
                    sorting = "dateSubmitted_desc";
                }
            }

            return sorting;
        }

        private void DokOrderBy(string sorting)
        {
            if (sorting != null)
            {
                if (sorting.Contains("_desc"))
                {
                    ViewBag.DokOrderBy = "";
                }
                else {
                    ViewBag.DokOrderBy = "_desc";
                }
            }
        }

        /// <summary>
        /// Creates a list of statuses for DOK Municipal register. 
        /// </summary>
        /// <returns></returns>
        private static List<Status> CreateStatusDOKMunicipalList()
        {
            List<Status> statusDOKMunicipal = new List<Status>();
            Status s1 = new Status();
            s1.value = "draft";
            s1.description = "I prosess";
            Status s2 = new Status();
            s2.value = "valid";
            s2.description = "Utført";
            statusDOKMunicipal.Add(s1);
            statusDOKMunicipal.Add(s2);
            return statusDOKMunicipal;
        }

        private string DOKmunicipalStatus(Organization municipality)
        {
            if (municipality.DateConfirmedMunicipalDOK != null)
            {
                if (lastDateConfirmedIsNotFromThisYear(municipality.DateConfirmedMunicipalDOK))
                {
                    return null;
                }
                return municipality.StatusConfirmationMunicipalDOK;
            }
            return null;
        }

        private static bool lastDateConfirmedIsNotFromThisYear(DateTime? dateConfirmedMunicipalDOK)
        {
            if (dateConfirmedMunicipalDOK != null)
            {
                return dateConfirmedMunicipalDOK.Value.Year != DateTime.Now.Year;
            }
            return false;
        }

        private void CheckReferrer()
        {
            if (Request.UrlReferrer != null)
            {
                string registerNameReferer = "";
                var pathReferer = Request.UrlReferrer.AbsolutePath;
                if (pathReferer.Contains("/"))
                {
                    var registerNameRefererObject = pathReferer.Split('/');
                    if(registerNameRefererObject.Count() > 2)
                        registerNameReferer = registerNameRefererObject[2].ToString();
                }
                    

                string registerNameCurrent = "";
                var pathCurrent = Request.Url.AbsolutePath;
                if (pathCurrent.Contains("/"))
                {
                    var registerNameCurrentObject = pathCurrent.Split('/');
                    if (registerNameCurrentObject.Count() > 2)
                        registerNameCurrent = registerNameCurrentObject[2].ToString();
                }

                if (Request.UrlReferrer.Host != null && (Request.UrlReferrer.Host != Request.Url.Host))
                    removeSessionSearchParams();

                if (!registerNameReferer.StartsWith("search") && registerNameReferer != registerNameCurrent)
                    removeSessionSearchParams();
            }
        }
    }
}
