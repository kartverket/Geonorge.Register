using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Models.StatusReports;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Search;
using Kartverket.Register.Services.Translation;
using Kartverket.Register.Services.Versioning;
using Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Xml;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class RegistersController : Controller
    {
        private readonly RegisterDbContext _db;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
        private readonly IMareanoDatasetService _mareanoDatasetService;
        private readonly IFairDatasetService _fairDatasetService;

        public RegistersController(ITranslationService translationService,
            RegisterDbContext dbContext, IRegisterItemService registerItemService, ISearchService searchService, IVersioningService versioningService,
            IRegisterService registerService, IAccessControlService accessControlService, IInspireDatasetService inspireDatasetService, IGeodatalovDatasetService geodatalovService, IInspireMonitoringService inspireMonitoringService, ISynchronizationService synchronizationService, IStatusReportService statusReportService, IMareanoDatasetService mareanoDatasetService, IFairDatasetService fairDatasetService)
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
            _mareanoDatasetService = mareanoDatasetService;
            _fairDatasetService = fairDatasetService;
        }

        // GET: Registers
        public ActionResult Index()
        {
            RemoveSessionSearchParams();

            return View(_registerService.GetRegistersGrouped());
        }

        [AllowCrossSiteJson]
        [Route("gettoken")]
        public JsonResult SessionInfo()
        {
            var token = "";

            if (Session["access_token"] != null)
                token = Session["access_token"].ToString();

            return Json(new { access_token = token }, JsonRequestBehavior.AllowGet);
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
                //cookie.SameSite = SameSiteMode.Lax;
                if (!Request.IsLocal)
                    cookie.Domain = ".geonorge.no";
            }
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
                //cookie.SameSite = SameSiteMode.Lax;

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
        [Route("inspire-statusregister")]
        [Route("register/inspire-statusregister")]
        public ActionResult DetailsInspireStatusRegistry(string sorting, int? page, string format, FilterParameters filter)
        {
            RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer();
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var register = _registerService.GetInspireStatusRegister();
            if (register == null) return HttpNotFound();

            if (register.RedirectToNewPath(HttpContext.Request.Path))
            {
                return RedirectPermanent(register.GetObjectUrl());
            }

            filter.InspireRegisteryType = GetInspireRegistryType(filter.InspireRegisteryType);
            register = FilterRegisterItems(register, filter);

            List<StatusReport> inspireStatusReports = _statusReportService.GetInspireStatusReports(12);
            StatusReport statusReport = filter.SelectedReport != null ? _statusReportService.GetStatusReportById(filter.SelectedReport) : inspireStatusReports.FirstOrDefault();


            var viewModel = new RegisterV2ViewModel(register, filter, page, statusReport, inspireStatusReports);
            viewModel.AccessRegister = _accessControlService.AccessViewModel(viewModel);
            viewModel.SelectedInspireRegisteryType = filter.InspireRegisteryType;

            if (viewModel.SelectedInspireRegisteryTypeIsInspireReport())
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
        [Route("inspire-statusregister")]
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
        [Route("dok-statusregisteret")]
        [Route("register/dok-statusregisteret")]
        public ActionResult DetailsDokStatusRegistry(string sorting, int? page, string format, FilterParameters filter)
        {
            RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer();
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var register = _registerService.GetDokStatusRegister();
            if (register == null) return HttpNotFound();
            if (register.RedirectToNewPath(HttpContext.Request.Path))
            {
                return RedirectPermanent(register.GetObjectUrl());
            }

            register = FilterRegisterItems(register, filter);

            List<StatusReport> dokStatusReports = _statusReportService.GetDokStatusReports(12);
            StatusReport statusReport = filter.SelectedReport != null ? _statusReportService.GetStatusReportById(filter.SelectedReport) : dokStatusReports.FirstOrDefault();


            var viewModel = new RegisterV2ViewModel(register, filter, null, statusReport, dokStatusReports);
            viewModel.SelectedDokTab = filter.DokSelectedTab;
            viewModel.AccessRegister = _accessControlService.AccessViewModel(viewModel);

            ItemsOrderBy(sorting, viewModel);
            ViewBagOrganizationMunizipality(filter.municipality);
            ViewbagsRegisterDetails(sorting, page, filter, viewModel);
            return View(viewModel);
        }

        // GET: Registers/Details DOK/5
        [Route("geodatalov-statusregister")]
        [Route("register/geodatalov-statusregister")]
        public ActionResult DetailsGeodatalovStatusRegistry(string sorting, int? page, string format, FilterParameters filter)
        {
            RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer();
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var register = _registerService.GetGeodatalovDatasetRegister();
            if (register == null) return HttpNotFound();
            if (register.RedirectToNewPath(HttpContext.Request.Path))
            {
                return RedirectPermanent(register.GetObjectUrl());
            }

            register = FilterRegisterItems(register, filter);

            List<StatusReport> geodatalovStatusReports = _statusReportService.GetStatusReportsByRegister(register, 12);
            StatusReport statusReport = filter.SelectedReport != null ? _statusReportService.GetStatusReportById(filter.SelectedReport) : geodatalovStatusReports.FirstOrDefault();

            var viewModel = new RegisterV2ViewModel(register, filter, null, statusReport, geodatalovStatusReports);
            viewModel.SelectedGeodatalovTab = filter.GeodatalovSelectedTab;
            viewModel.AccessRegister = _accessControlService.AccessViewModel(viewModel);

            ItemsOrderBy(sorting, viewModel);
            ViewbagsRegisterDetails(sorting, page, filter, viewModel);
            return View(viewModel);
        }

        [Route("mareano-statusregister")]
        [Route("register/mareano-statusregister")]
        public ActionResult DetailsMareanoStatusRegistry(string sorting, int? page, string format, FilterParameters filter)
        {
            RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer();
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var register = _registerService.GetMareanoDatasetRegister();
            if (register == null) return HttpNotFound();
            if (register.RedirectToNewPath(HttpContext.Request.Path))
            {
                return RedirectPermanent(register.GetObjectUrl());
            }

            var orgList = register.RegisterItems.OrderBy(o => o.Owner.name).Select(s => new { name = s.Owner.name, seoname = s.Owner.seoname }).Distinct().ToList();

            register = FilterRegisterItems(register, filter);

            List<StatusReport> mareanoStatusReports = _statusReportService.GetStatusReportsByRegister(register, 12);
            mareanoStatusReports = mareanoStatusReports.OrderBy(o => o.Date).ToList();
            StatusReport statusReport = filter.SelectedReport != null ? _statusReportService.GetStatusReportById(filter.SelectedReport) : mareanoStatusReports.OrderByDescending(o => o.Date).FirstOrDefault();

            if (!string.IsNullOrEmpty(filter.filterOrganization)) 
            {
                List<RegisterItemStatusReport> reportItems = new List<RegisterItemStatusReport>();
                foreach(MareanoDatasetStatusReport item in statusReport.StatusRegisterItems) 
                {
                    if(item.OrganizationSeoName == filter.filterOrganization)
                        reportItems.Add(item);
                }

                statusReport.StatusRegisterItems = reportItems;

                List<StatusReport> mareanoStatusReportsOrganization = new List<StatusReport>();

                foreach (var mareanoStatusReport in mareanoStatusReports) 
                {
                    StatusReport statusReportOrg = mareanoStatusReport;
                    List<RegisterItemStatusReport> reportItemsOrg = new List<RegisterItemStatusReport>();
                    foreach (MareanoDatasetStatusReport itemOrg in mareanoStatusReport.StatusRegisterItems)
                    {
                        if (itemOrg.OrganizationSeoName == filter.filterOrganization)
                            reportItemsOrg.Add(itemOrg);
                    }

                    statusReportOrg.StatusRegisterItems = reportItemsOrg;

                    mareanoStatusReportsOrganization.Add(statusReportOrg);
                }

                mareanoStatusReports = mareanoStatusReportsOrganization;

            }

            var viewModel = new RegisterV2ViewModel(register, filter, null, statusReport, mareanoStatusReports);
            viewModel.SelectedMareanoTab = filter.MareanoSelectedTab;
            var organizations = orgList.Select(s => new { name = s.name, seoname = s.seoname });
            organizations = organizations.Prepend(new { name = "Alle", seoname = "" });
            ViewBag.filterOrganization = organizations;
            viewModel.AccessRegister = _accessControlService.AccessViewModel(viewModel);

            ItemsOrderBy(sorting, viewModel);
            ViewbagsRegisterDetails(sorting, page, filter, viewModel);
            return View(viewModel);
        }

        [Route("fair-register")]
        public ActionResult DetailsFairStatusRegistry(string sorting, int? page, string format, FilterParameters filter)
        {
            RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer();
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var register = _registerService.GetFairDatasetRegister();
            if (register == null) return HttpNotFound();
            if (register.RedirectToNewPath(HttpContext.Request.Path))
            {
                return RedirectPermanent(register.GetObjectUrl());
            }

            var orgList = register.RegisterItems.OrderBy(o => o.Owner.name).Select(s => new { name = s.Owner.name, seoname = s.Owner.seoname }).Distinct().ToList();

            register = FilterRegisterItems(register, filter);

            List<StatusReport> fairStatusReports = _statusReportService.GetStatusReportsByRegister(register, 12);
            fairStatusReports = fairStatusReports.OrderBy(o => o.Date).ToList();
            StatusReport statusReport = filter.SelectedReport != null ? _statusReportService.GetStatusReportById(filter.SelectedReport) : fairStatusReports.OrderByDescending(o => o.Date).FirstOrDefault();

            if (!string.IsNullOrEmpty(filter.filterOrganization))
            {
                List<RegisterItemStatusReport> reportItems = new List<RegisterItemStatusReport>();
                foreach (FairDatasetStatusReport item in statusReport.StatusRegisterItems)
                {
                    if (item.OrganizationSeoName == filter.filterOrganization)
                        reportItems.Add(item);
                }

                statusReport.StatusRegisterItems = reportItems;

                List<StatusReport> fairStatusReportsOrganization = new List<StatusReport>();

                foreach (var fairStatusReport in fairStatusReports)
                {
                    StatusReport statusReportOrg = fairStatusReport;
                    List<RegisterItemStatusReport> reportItemsOrg = new List<RegisterItemStatusReport>();
                    foreach (FairDatasetStatusReport itemOrg in fairStatusReport.StatusRegisterItems)
                    {
                        if (itemOrg.OrganizationSeoName == filter.filterOrganization)
                            reportItemsOrg.Add(itemOrg);
                    }

                    statusReportOrg.StatusRegisterItems = reportItemsOrg;

                    fairStatusReportsOrganization.Add(statusReportOrg);
                }

                fairStatusReports = fairStatusReportsOrganization;

            }

            var viewModel = new RegisterV2ViewModel(register, filter, null, statusReport, fairStatusReports);
            viewModel.SelectedFairTab = filter.FairSelectedTab;
            var organizations = orgList.Select(s => new { name = s.name, seoname = s.seoname });
            organizations = organizations.Prepend(new { name = "Alle", seoname = "" });
            ViewBag.filterOrganization = organizations;
            viewModel.AccessRegister = _accessControlService.AccessViewModel(viewModel);

            ItemsOrderBy(sorting, viewModel);
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


        //// GET: Registers/Details/5
        //[Route("{registername}")]
        //[Route("{parentRegister}/{registername}/")]
        //[Route("{registername}.{format}")]
        //[Route("{parentRegister}/{registername}.{format}/")]
        //[Route("register/{registername}")]
        //[Route("register/{registername}.{format}")]
        //[Route("subregister/{parentRegister}/{owner}/{registername}.{format}")]
        //[Route("subregister/{parentRegister}/{owner}/{registername}")]
        //public ActionResult Details(string parentRegister, string owner, string registername, string sorting, int? page, string format, FilterParameters filter)
        //{
        //    RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer();
        //    var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
        //    if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

        //    var register = _registerService.GetRegister(parentRegister, registername);
        //    if (register == null) return HttpNotFound();

        //    if (register.RedirectToNewPath(HttpContext.Request.Path))
        //    {
        //        return RedirectPermanent(register.GetObjectUrl());
        //    }

        //    //List<StatusReport> statusReports = _statusReportService.GetStatusReportsByRegister(register, 12);
        //    //StatusReport statusReport = filter.SelectedReport != null ? _statusReportService.GetStatusReportById(filter.SelectedReport) : statusReports.FirstOrDefault();

        //    register = FilterRegisterItems(register, filter);
        //    var viewModel = new RegisterV2ViewModel(register, filter, null, null, null);
        //    viewModel.MunicipalityCode = filter.municipality;
        //    viewModel.Municipality = _registerItemService.GetMunicipalityOrganizationByNr(viewModel.MunicipalityCode);
        //    viewModel.AccessRegister = _accessControlService.AccessViewModel(viewModel);

        //    ItemsOrderBy(sorting, viewModel);
        //    ViewBagOrganizationMunizipality(filter.municipality);
        //    ViewBagOrganizationTypes(viewModel);
        //    ViewbagsRegisterDetails(sorting, page, filter, viewModel);
        //    return View(viewModel);
        //}

        private void ViewBagOrganizationTypes(RegisterV2ViewModel viewModel)
        {
            ViewBag.SelectedOrganizationType = new SelectList(OrganizationType.OrganizationTypes(), "Value", "Text");
            
        }


        [Route("{registername}/{*subregisters}")]
        public ActionResult DetailsAll(string registername, string sorting, int? page, FilterParameters filter,string subregisters = null, string InspireRegisteryType = null)
        {
            if (registername == "sosi-kodelister" && (subregisters == "kommunenummer" || (!string.IsNullOrEmpty(subregisters) && subregisters.StartsWith("kommunenummer.")))) { 
                            
                if (!string.IsNullOrEmpty(subregisters) && subregisters.StartsWith("kommunenummer."))
                {
                    var fileExt = subregisters.Split('.').Last();
                    subregisters = "inndelinger/inndelingsbase/kommunenummer";
                    subregisters = subregisters + "." + fileExt;
                }
                else
                {
                    subregisters = "inndelinger/inndelingsbase/kommunenummer";
                }
            }
            if (registername == "sosi-kodelister" && (subregisters == "fylkesnummer" || (!string.IsNullOrEmpty(subregisters) && subregisters.StartsWith("fylkesnummer."))))
            {
                
                if (!string.IsNullOrEmpty(subregisters) && subregisters.StartsWith("fylkesnummer."))
                {
                    var fileExt = subregisters.Split('.').Last();
                    subregisters = "inndelinger/inndelingsbase/fylkesnummer";
                    subregisters = subregisters + "." + fileExt;
                }
                else
                {
                    subregisters = "inndelinger/inndelingsbase/fylkesnummer";
                }
            }

            var path = RegisterUrls.GetPath(registername, subregisters);
            var originalPath = path;
            string systemId = RegisterUrls.GetSystemIdFromPath(registername + "/" + subregisters);
            bool isRegisterItem = false;

            RemoveSessionsParamsIfCurrentRegisterIsNotTheSameAsReferer();
            var registerPath = registername;
            if (!string.IsNullOrEmpty(subregisters))
                registerPath = registerPath + "/" + subregisters;
            string format = RegisterUrls.GetFileExtension(registerPath);
            if(string.IsNullOrEmpty(format) && Request.Headers["Accept"] == null || Request.Headers["Accept"] == "*/*") 
            {
                format = "json";
            }
            var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            var register = _registerService.GetRegisterByPath(path);

            if (register == null) 
            {
                var value = subregisters.Split('/').Last();

                bool urlInSubregister = false;

                if(subregisters.Contains("/http:/") || subregisters.Contains("/https:/")) 
                {
                    var pathArray = subregisters.Split(':'); 
                    var pathNew = pathArray[0];
                    if (subregisters.Contains("/http:/"))
                        pathNew = pathNew.Replace("/http", "");
                    if (subregisters.Contains("/https:/"))
                        pathNew = pathNew.Replace("/https", "");
                    path = pathNew;

                    value = pathArray[1];
                    if (subregisters.Contains("/http:/"))
                        value = "http:/" + value;
                    if (subregisters.Contains("/https:/"))
                        value = "https:/" + value;

                    path = registername + "/" + path;

                    urlInSubregister = true;
                }

                    if (path.Contains('/') && !urlInSubregister)
                    path = path.Substring(0, path.LastIndexOf('/'));
                //check codevalue
                var codevalue = _db.RegisterItems.OfType<CodelistValue>().Where(s => (s.value == value || s.seoname == value )  && s.register.path == path).FirstOrDefault();
                if(codevalue != null)
                {
                    systemId = codevalue.systemId.ToString();
                    register = codevalue.register;
                    isRegisterItem = true;
                }
            }

            if (register == null)
            {
                var value2 = subregisters.Split('/').Last();
                if (path.Contains('/'))
                    path = path.Substring(0, originalPath.LastIndexOf('/'));
                //check value
                var doc = _db.RegisterItems.OfType<Document>().Where(s => (s.seoname == value2) && s.register.path == path).FirstOrDefault();
                if (doc != null)
                {
                    systemId = doc.systemId.ToString();
                    register = doc.register;
                    isRegisterItem = true;
                }
            }


            if (register == null)
            {
                var value = subregisters.Split('/').Last();
                if (path.Contains('/'))
                    path = path.Substring(0, path.LastIndexOf('/'));
                //check register
                var registerItem = _db.RegisterItems.Where(s => (s.seoname == value) && s.register.path == path).FirstOrDefault();
                if (registerItem != null)
                {
                    systemId = registerItem.systemId.ToString();
                    register = registerItem.register;
                }
            }

            if (register == null && originalPath.Contains('/'))
            {
                register = _registerService.GetRegisterByPath(originalPath.Substring(0, originalPath.LastIndexOf('/')));
            }
            if (register == null)
            {
                var itemArray = subregisters.Split('/');
                var itemName = itemArray[itemArray.Length - 2];
                var itemVersion= itemArray[itemArray.Length -1 ];

                RegisterItemV2ViewModel view = new DocumentViewModel((Document)_registerItemService.GetRegisterItemByPath(path, itemName, itemVersion));
                if (view != null) { 
                    view.AccessRegisterItem = _accessControlService.HasAccessTo(view);
                    return View("DetailsRegisterItem", view);
                }
            }

            if (register == null)    
                return HttpNotFound();

            if (register.ContainedItemClassIsDocument() && !register.items.Any()) 
            {
                register.items = _db.RegisterItems.Where(d => d.register.parentRegisterId == register.systemId).ToList();
            }

            if (register.ContainedItemClassIsDocument() && !string.IsNullOrEmpty(subregisters) && isRegisterItem) 
            {
                string registerName = "";
                string parentRegister = null;
                string itemName = "";

                var items = originalPath.Split('/');

                if(items.Count() == 2) 
                {
                    registerName = items[0];
                    itemName = items[1];
                }
                else
                {
                    registerName = items[items.Count()-2];
                    parentRegister = items[items.Count() - 3];
                    itemName = items[items.Count()-1];
                }

                return DetailsRegisterItemVersions(registerName, parentRegister, itemName, register.owner.seoname, format);
            }

               
            register = FilterRegisterItems(register, filter);
            RegisterV2ViewModel viewModel;
            try
            {
                if (!string.IsNullOrEmpty(systemId))
                {
                    var viewModelDetail = GetRegisterItemById(register, systemId, InspireRegisteryType);
                    viewModelDetail.AccessRegisterItem = _accessControlService.HasAccessTo(viewModelDetail);

                    return View("DetailsRegisterItem", viewModelDetail);
                }
                else
                {
                    viewModel = new RegisterV2ViewModel(register, filter, null, null, null);
                }
            }
            catch (Exception ex) { return HttpNotFound(); }


            viewModel.MunicipalityCode = filter.municipality;
            viewModel.Municipality = _registerItemService.GetMunicipalityOrganizationByNr(viewModel.MunicipalityCode);
            viewModel.AccessRegister = _accessControlService.AccessViewModel(viewModel);

            ItemsOrderBy(sorting, viewModel);
            ViewBagOrganizationMunizipality(filter.municipality);
            ViewBagOrganizationTypes(viewModel);
            ViewbagsRegisterDetails(sorting, page, filter, viewModel);
            return View("Details", viewModel);

        }

        private string GetInspireRegistryType(string filter)
        {
            if (filter == "inspirereport")
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

        //[Route("{registername}/{itemname}/{systemId}")]
        //[Route("{registername}/{itemname}/{systemId}.{format}")]
        //[Route("{parentRegister}/{registername}/{itemname}/{systemId}")]
        //[Route("{parentRegister}/{registername}/{itemname}/{systemId}.{format}")]
        //[Route("register/{registername}/{itemowner}/{itemname}.{format}")]
        //[Route("register/{registername}/{itemowner}/{itemname}")]
        //[Route("register/{registername}/{itemowner}/{itemname}/{systemId}")]
        //[Route("subregister/{parentRegister}/{owner}/{registername}/{submitter}/{itemname}.{format}")]
        //[Route("subregister/{parentRegister}/{owner}/{registername}/{submitter}/{itemname}")]
        //public ActionResult DetailsRegisterItem(string parentRegister, string registername, string itemowner, string itemname, string format, string systemId, string InspireRegisteryType = null)
        //{
        //    var redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
        //    if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

        //    RegisterItemV2ViewModel viewModel;
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(systemId))
        //        {
        //            viewModel = GetRegisterItemByName(parentRegister, registername, itemowner, itemname, InspireRegisteryType);
        //        }
        //        else
        //        {
        //            viewModel = GetRegisterItemById(parentRegister, registername, systemId, InspireRegisteryType);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return HttpNotFound();
        //    }
        //    if (viewModel.RedirectToNewPath(HttpContext.Request.Path))
        //    {
        //        return RedirectPermanent(viewModel.DetailPageUrl());
        //    }
        //    viewModel.AccessRegisterItem = _accessControlService.HasAccessTo(viewModel);
        //    if (string.IsNullOrWhiteSpace(viewModel.Name))
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(viewModel);
        //}

        public ActionResult DetailsRegisterItemFramework(string versionnumber, string registername, string codevalue)
        {
            string path = $"styrendedokumenter/nasjonalt-rammeverk-for-geografisk-informasjon/{versionnumber}/{registername}";

            var registerFromPath = _db.Registers.Where(p => p.path == path).FirstOrDefault();

            if(registerFromPath == null)
                return HttpNotFound();

            var registerItem = _db.CodelistValues.Where(x => x.registerId == registerFromPath.systemId && x.value == codevalue).FirstOrDefault();
            if (registerItem == null)
            {
                return HttpNotFound();
            }

            var register = _registerService.GetRegisterBySystemId(registerFromPath.systemId);

            RegisterItemV2ViewModel viewModel = GetRegisterItemById(register, registerItem.systemId.ToString(), null);

            return View("DetailsRegisterItem", viewModel);

        }


        [Route("subregister/versjoner/{parentregister}/{parentowner}/{registername}/{itemowner}/{itemname}/{version}/no.{format}")]
        [Route("subregister/versjoner/{parentregister}/{parentowner}/{registername}/{itemowner}/{itemname}/{version}/no")]
        [Route("register/versjoner/{registername}/{itemowner}/{itemname}/{version}/no.{format}")]
        [Route("register/versjoner/{registername}/{itemowner}/{itemname}/{version}/no")]
        public ActionResult DetailsRegisterItem(string registername, string itemowner, string itemname, int version, string format, string parentregister)
        {
            string redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);
            var viewModel = GetRegisterItemByName(parentregister, registername, itemowner, itemname, null, version);
            viewModel.AccessRegisterItem = _accessControlService.HasAccessTo(viewModel);

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

            if (!string.IsNullOrEmpty(format))
                itemname = itemname + "." + format;

            var versionsItem = _versioningService.Versions(registername, parentRegister, itemname);
            var model = new VersionsViewModel(versionsItem);
            model.HistoricalVersions = model.HistoricalVersions.OrderByDescending(o => o.VersionNumber).ToList();
            model.SuggestedVersions = model.SuggestedVersions.OrderBy(o => o.VersionNumber).ToList();
            model.AccessCreateNewVersions = _accessControlService.AccessCreateNewVersion(model.CurrentVersion);
            model.CurrentVersion.AccessRegisterItem = _accessControlService.HasAccessTo(model.CurrentVersion);

            ViewBag.registerItemOwner = registerItemOwner;

            if (model.CurrentVersion?.Name == null)
                return HttpNotFound();

            return View("DetailsRegisterItemVersions", model);
        }


        // GET: Register/Create
        [Authorize]
        //[Route("ny")]
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
        //[Route("ny")]
        public ActionResult Create(Models.Register register)
        {
            if (IsAdmin())
            {
                if (!_registerService.RegisterNameIsValid(register)) 
                    ModelState.AddModelError("ErrorMessage", Registers.ErrorMessageValidationName);

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
        //[Route("rediger/{registername}")]
        public ActionResult Edit(string systemid)
        {
            var register = _registerService.GetRegisterBySystemId(Guid.Parse(systemid));

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
        //[Route("rediger/{registername}")]
        [Authorize]
        public ActionResult Edit(Models.Register register)
        {
            if (IsAdmin())
            {
                Models.Register originalRegister = _registerService.GetRegisterBySystemId(register.systemId);

                if (_registerService.validationName(register, originalRegister)) ModelState.AddModelError("ErrorMessage", Registers.ErrorMessageValidationName);

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
                    originalRegister.path = RegisterUrls.GetNewPath(originalRegister.path, originalRegister.seoname);
                    originalRegister.modified = DateTime.Now;
                    if (register.statusId != null) originalRegister = _registerService.SetStatus(register, originalRegister);
                    _translationService.UpdateTranslations(register, originalRegister);
                    _db.Entry(originalRegister).State = EntityState.Modified;
                    _db.SaveChanges();
                    Viewbags(register);

                    return Redirect("/" + originalRegister.path);
                }
                Viewbags(register);
                return View(originalRegister);
            }
            return HttpNotFound();
        }




        // GET: Edit DOK-Municipal-Dataset
        [Authorize]
        //[Route("dok/kommunalt/{municipalityCode}/rediger")]
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
                    ViewBag.MeasureStatuses = _db.DokMeasureStatuses.OrderBy(o => o.sortorder);
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
        //[Route("dok/kommunalt/{municipalityCode}/rediger")]
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
                        bool? coverageFound = originalCoverage?.Coverage ?? false;
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
                                "CoverageId, MunicipalityId, DatasetId, CoverageDOKStatusId, ZoningPlan,ImpactAssessmentPlanningBuildingAct,RiskVulnerabilityAnalysisPlanningBuildingAct,MeasureDOKStatusId  ) " +
                                " VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17, @p18, @p19 " +
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
                                coverageNew.CoverageDOKStatusId,
                                item.ZoningPlan,
                                item.ImpactAssessmentPlanningBuildingAct,
                                item.RiskVulnerabilityAnalysisPlanningBuildingAct,
                                item.Measure
                                );
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
                                "SuitabilityAssessmentText = @p11, " +
                                "ZoningPlan = @p12, " +
                                "ImpactAssessmentPlanningBuildingAct = @p13, " +
                                "RiskVulnerabilityAnalysisPlanningBuildingAct = @p14, " +
                                "MeasureDOKStatusId = @p15 " +
                                "WHERE CoverageId = @p16",
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
                                item.ZoningPlan,
                                item.ImpactAssessmentPlanningBuildingAct,
                                item.RiskVulnerabilityAnalysisPlanningBuildingAct,
                                item.Measure,
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
        //[Route("slett/{registername}")]
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
        //[Route("slett/{registername}")]
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

        private CoverageDataset CreateNewCoverage(DokMunicipalEdit item, Dataset originalDataset, string municipalityCode, bool? coverageFound)
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
            if (Request != null && Request.FilePath != null && (Request.FilePath.Contains("/register/tjenestevarsler") || Request.FilePath.Contains("geonorge.no/varsler.atom")))
                return WebConfigurationManager.AppSettings["RegistryUrl"] + "varsler.atom";

            if (!string.IsNullOrWhiteSpace(format) && RegisterUrls.AllowedExtension(format))
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
            DocumentAccessByUser(register);

            return register;
        }

        private static void DocumentAccessByUser(Models.Register register)
        {
            if (register != null && register.ContainedItemClassIsDocument())
            {
                var registeItems = new List<RegisterItem>();
                foreach (Document document in register.items)
                {
                    if ((document.statusId != "Submitted") || HtmlHelperExtensions.AccessRegisterItem(document))
                    {
                        registeItems.Add(document);
                    }
                }

                register.items = registeItems;
            }
        }

        private RegisterItemV2ViewModel GetRegisterItemByName(string parentregister, string registername, string itemowner, string itemname, string inspireRegistryType, int version = 1)
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

        private RegisterItemV2ViewModel GetRegisterItemById(Models.Register register, string systemId, string inspireRegistryType)
        {
            //var register = _registerService.GetRegister(parentregister, registername);

            if (register.IsInspireStatusRegister())
            {
                RegisterItemV2ViewModel viewModel = null;
                if (string.IsNullOrWhiteSpace(inspireRegistryType) || inspireRegistryType == "dataset")
                {
                    var inspireDataset = _inspireDatasetService.GetInspireDatasetById(systemId);
                    if (inspireDataset != null)
                    {
                        viewModel = new InspireDatasetViewModel(inspireDataset);
                    }
                    else
                    {
                        viewModel = new InspireDataServiceViewModel(_inspireDatasetService.GetInspireDataServiceById(systemId));
                    }
                }
                else
                {
                    viewModel = new InspireDataServiceViewModel(_inspireDatasetService.GetInspireDataServiceById(systemId));
                }
                return viewModel;
            }
            if (register.IsGeodatalovStatusRegister())
            {
                return new GeodatalovDatasetViewModel(_geodatalovDatasetService.GetGeodatalovDatasetById(systemId));
            }
            if (register.IsMareanoStatusRegister())
            {
                return new MareanoDatasetViewModel(_mareanoDatasetService.GetMareanoDatasetById(systemId));
            }
            if (register.IsFairStatusRegister())
            {
                return new FairDatasetViewModel(_fairDatasetService.GetFairDatasetById(systemId));
            }
            if (register.IsDokMunicipal())
            {
                return new RegisterItemV2ViewModel(_registerItemService.GetRegisterItemBySystemId(Guid.Parse(systemId)));
            }

            if (register.ContainedItemClassIsDataset())
            {
                return new DokDatasetViewModel(_registerItemService.GetDatasetById(Guid.Parse(systemId), register.systemId));
            }
            if (register.ContainedItemClassIsDocument())
            {
                return new DocumentViewModel((Document)_registerItemService.GetRegisterItemBySystemId(Guid.Parse(systemId)));
            }

            return new RegisterItemV2ViewModel(_registerItemService.GetRegisterItemBySystemId(Guid.Parse(systemId)));

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
