using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Kartverket.Register.Models.Api;
using SearchParameters = Kartverket.Register.Models.SearchParameters;
using SearchResult = Kartverket.Register.Models.SearchResult;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.Translations;
using System.Globalization;
using System.Threading;
using System.Net.Http.Headers;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Formatter;
using System.Web;
using Kartverket.Register.App_Start;
using StatusReport = Kartverket.Register.Models.StatusReport;

namespace Kartverket.Register.Controllers
{
    public class ApiRootController : ApiController
    {
        private RegisterDbContext db;

        private readonly ISearchService _searchService;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;
        private readonly IInspireDatasetService _inspireDatasetService;
        private readonly IInspireMonitoringService _inspireMonitoringService;
        private readonly IAccessControlService _accessControlService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IStatusReportService _statusReportService;

        public ApiRootController(RegisterDbContext dbContext, ISearchService searchService, IRegisterService registerService, IRegisterItemService registerItemService, IInspireDatasetService inspireDatasetService, IInspireMonitoringService inspireMonitoringService, IAccessControlService accessControlService, ISynchronizationService synchronizationService, IStatusReportService statusReportService)
        {
            _registerItemService = registerItemService;
            _inspireDatasetService = inspireDatasetService;
            _searchService = searchService;
            _registerService = registerService;
            _inspireMonitoringService = inspireMonitoringService;
            _accessControlService = accessControlService;
            _synchronizationService = synchronizationService;
            _statusReportService = statusReportService;
            db = dbContext;
        }

        /// <summary>
        /// List top level registers. Use id in response to navigate.
        /// </summary>
        [Route("api/register")]
        [Route("api/register.{ext}")]
        [HttpGet]
        public IHttpActionResult GetRegisters()
        {
            SetLanguage(Request);
            var list = new List<Models.Api.Register>();
            List<Models.Register> registers = _registerService.GetRegisters();
            foreach (Models.Register register in registers)
            {
                list.Add(ConvertRegister(register));
            }
            return Ok(list);
        }





        /// <summary>
        /// Gets register by name
        /// </summary>
        /// <param name="registerName">The search engine optimized name of the register</param>
        [Route("api/register/{registerName}.{ext}")]
        [Route("api/register/{registerName}")]
        [HttpGet]
        public IHttpActionResult GetRegisterByName(string registerName, [FromUri] FilterParameters filter = null)
        {
            SetLanguage(Request);
            var register = _registerService.GetRegisterByName(registerName);
            if (filter != null || register.IsDokMunicipal())
            {
                register = RegisterItems(register, filter);
            }

            return Ok(ConvertRegisterAndNextLevel(register, filter));
        }

        /// <summary>
        /// Gets inspiremonitoring xml
        /// </summary>
        [Route("api/register/inspire-statusregister/monitoring-report")]
        [HttpGet]
        public IHttpActionResult InspireMonitoring()
        {
            SetLanguage(Request);
            var inspireStatusRegister = _registerService.GetInspireStatusRegister();
            Monitoring inspireMonitoring = _inspireMonitoringService.GetInspireMonitoringReport(inspireStatusRegister);

            return Content(System.Net.HttpStatusCode.OK, inspireMonitoring, new XMLFormatter(), new MediaTypeHeaderValue("application/xml"));
        }


        /// <summary>
        /// Save dok status report to db
        /// </summary>
        [Authorize(Roles = AuthConfig.RegisterProviderRole)]
        [Route("api/register/{registerName}/report/save")]
        [HttpGet]
        public IHttpActionResult NewStatusReport(string registerName)
        {
            try
            {
                var register = _registerService.GetRegisterByName(registerName);
                _statusReportService.CreateStatusReport(register);
                return Ok("Saved");
            }
            catch(Exception e)
            {
                return Ok(e);
            }
        }

        /// <summary>
        /// Gets selected status report
        /// </summary>
        [Route("api/register/{registerName}/report/{id}.{ext}")]
        [Route("api/register/{registerName}/report/{id}")]
        [HttpGet]
        public IHttpActionResult StatusReport(string id, bool dataset = true, bool service = true)
        {
            SetLanguage(Request);
            var statusReport = _statusReportService.GetStatusReportById(id);

            if (statusReport.IsDokReport())
            {
                return Ok(new DokStatusReport(statusReport));
            }

            if (statusReport.IsInspireRegistryReport())
            {
                if (dataset && service)
                {
                return Ok(new InspireRegistryStatusReport(statusReport));
                }
                if (dataset)
                {
                return Ok(new Models.Api.InspireDataSetStatusReport(statusReport));
                }
                if (service)
                {
                    return Ok(new InspireServiceStatusReport(statusReport));
                }

            }
            return Ok();

        }

        /// <summary>
        /// Gets selected status report
        /// </summary>
        [Route("api/register/{registerName}/report.{ext}")]
        [Route("api/register/{registerName}/report")]
        [HttpGet]
        public IHttpActionResult StatusReports(string registerName)
        {
            SetLanguage(Request);
            var register = _registerService.GetRegisterByName(registerName);
            List<StatusReport> statusReports = new List<StatusReport>();
            if (register.IsInspireStatusRegister())
            {
                statusReports = _statusReportService.GetInspireStatusReports();
                //foreach (var report in statusReports)
                //{
                //    statusReportsApi.Add(new Models.Api.InspireStatusReport(report));
                //}
            }
            else if (register.IsDokStatusRegister())
            {
                List<DokStatusReport> dokStatusReportsApi = new List<DokStatusReport>();
                statusReports = _statusReportService.GetDokStatusReports();

                foreach (var report in statusReports)
                {
                    dokStatusReportsApi.Add(new DokStatusReport(report));
                }
                return Ok(dokStatusReportsApi);
            }

            return Ok();
        }



        /// <summary>
        /// Save inspire monitoring report data to database.
        /// </summary>
        [Authorize(Roles = AuthConfig.RegisterProviderRole)]
        [Route("api/register/inspire-statusregister/monitoring-report/save")]
        [HttpGet]
        public IHttpActionResult SaveInspireMonitoringData()
        {
            try
            {
                var inspireStatusRegister = _registerService.GetInspireStatusRegister();
                _inspireMonitoringService.SaveInspireMonitoring(inspireStatusRegister);

                return Ok("Saved");
            }
            catch
            {
                return Ok("Error");
            }
        }



        private Models.Register RegisterItems(Models.Register register, FilterParameters filter)
        {
            if (Search(filter)) register = _searchService.Search(register, filter.text);
            if (register.IsInspireStatusRegister())
            {
                register = _registerItemService.GetInspireStatusRegisterItems(register);
            }
            else
            {
                register = _registerService.FilterRegisterItems(register, filter);
            }
            return register;
        }

        private static bool Search(FilterParameters filter)
        {
            return !string.IsNullOrWhiteSpace(filter.text);
        }


        /// <summary>
        /// Gets subregister by name
        /// </summary>
        /// <param name="register">The search engine optimized name of the register</param>
        /// <param name="parentregister">The search engine optimized name of the parentregister</param>
        [Route("api/subregister/{parentregister}/{parentregisterOwner}/{register}.{ext}")]
        [Route("api/subregister/{parentregister}/{parentregisterOwner}/{register}")]
        [HttpGet]
        public IHttpActionResult GetSubregisterByName(string parentregister, string register)
        {
            SetLanguage(Request);
            var it = _registerService.GetSubregisterByName(parentregister, register);
            return Ok(ConvertRegisterAndNextLevel(it));
        }


        /// <summary>
        /// Gets codelist by systemid
        /// </summary>
        /// <param name="systemid">The uniqueidentifier for the register</param>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/kodelister/{systemid}")]
        [HttpGet]
        public IHttpActionResult GetRegisterBySystemId(string systemid)
        {
            SetLanguage(Request);
            var it = _registerService.GetRegisterBySystemId(Guid.Parse(systemid));
            return Ok(ConvertRegisterAndNextLevel(it));
        }


        /// <summary>
        /// Gets current and historical versions of register item by register- organization- and registeritem-name 
        /// </summary>
        /// <param name="registerName">The search engine optimized name of the register</param>
        /// <param name="itemowner">The search engine optimized name of the register item owner</param>
        /// <param name="item">The search engine optimized name of the register item</param>
        [Route("api/register/{registerName}/{itemowner}/{item}.{ext}")]
        [Route("api/register/{registerName}/{itemowner}/{item}")]
        [Route("api/register/versjoner/{registerName}/{itemowner}/{item}.{ext}")]
        [Route("api/register/versjoner/{registerName}/{itemowner}/{item}")]
        [HttpGet]
        public IHttpActionResult GetRegisterItemByName(string registerName, string itemowner, string item)
        {
            SetLanguage(Request);
            var register = _registerService.GetRegister(null, registerName);

            Registeritem currentVersion;

            if (register.IsInspireStatusRegister())
            {
                currentVersion = ConvertInspireRegister(registerName, item);
            }
            else
            {
                currentVersion = ConvertCurrentAndVersions(null, registerName, item);
            }

            return Ok(currentVersion);
        }

        /// <summary>
        /// Gets current and historical versions of register item by register- organization- and registeritem-name 
        /// </summary>
        /// <param name="registerName">The search engine optimized name of the register</param>
        /// <param name="itemowner">The search engine optimized name of the register item owner</param>
        /// <param name="item">The search engine optimized name of the register item</param>
        [Route("api/register/{registerName}/{itemowner}/{item}/monitoring-report")]
        [HttpGet]
        public IHttpActionResult InspireDatasetMonitoring(string registerName, string itemowner, string item)
        {
            SetLanguage(Request);
            Models.InspireDataset inspireDataset = _inspireDatasetService.GetInspireDatasetByName(registerName, item);
            SpatialDataSet inspireDatasetMonitoring = _inspireMonitoringService.MappingSpatialDataSet(inspireDataset);

            return Ok(inspireDatasetMonitoring);
        }

        private Registeritem ConvertInspireRegister(string registerName, string item)
        {
            var inspireDataset = _inspireDatasetService.GetInspireDatasetByName(registerName, item);
            return ConvertRegisterItem(inspireDataset);
        }


        /// <summary>
        /// Gets register item by register- organization- registeritem-name  and version-id
        /// </summary>
        /// <param name="register">The search engine optimized name of the register</param>
        /// <param name="item">The search engine optimized name of the register item</param>
        /// <param name="version">The version id of the registeritem</param>
        [Route("api/register/versjoner/{register}/{itemowner}/{item}/{version}/no.{ext}")]
        [Route("api/register/versjoner/{register}/{itemowner}/{item}/{version}/no")]
        [HttpGet]
        public IHttpActionResult GetRegisterItemByVersionNr(string register, string item, int version)
        {
            SetLanguage(Request);
            var registerItem = _registerItemService.GetRegisterItem(null, register, item, version);
            return Ok(ConvertRegisterItem(registerItem));
        }


        /// <summary>
        /// Gets register item by parent-register, register- organization- and registeritem-name 
        /// </summary>
        /// <param name="parentregister">The search engine optimized name of the parent register</param>
        /// <param name="register">The search engine optimized name of the register</param>
        /// <param name="item">The search engine optimized name of the register item</param>
        [Route("api/subregister/{parentregister}/{registerowner}/{register}/{itemowner}/{item}")]
        [Route("api/subregister/{parentregister}/{registerowner}/{register}/{itemowner}/{item}.{ext}")]
        [HttpGet]
        public IHttpActionResult GetSubregisterItemByName(string parentregister, string register, string item)
        {
            SetLanguage(Request);
            Models.Api.Registeritem currentVersion = ConvertCurrentAndVersions(parentregister, register, item);
            return Ok(currentVersion);
        }

        /// <summary>
        /// List items for specific organization 
        /// </summary>
        /// <param name="name">The name of the organization</param>
        [Route("api/register/organisasjon/{name}")]
        [HttpGet]
        public IHttpActionResult SearchByOrganizationName(string name)
        {
            List<Models.Api.Item> resultat = new List<Models.Api.Item>();
            SearchParameters parameters = new SearchParameters();
            parameters.Text = name;
            SearchResult searchResult = _searchService.Search(parameters);
            foreach (var it in searchResult.Items)
            {
                resultat.Add(Convert(it));
            }
            return Ok(resultat);
        }

        /// <summary>
        /// List items for specific organization 
        /// </summary>
        /// <param name="register">The name of the register</param>
        /// <param name="itemowner">The name of the organization</param>
        [Route("api/register/{register}/{itemowner}.{ext}", Order = 1)]
        [Route("api/register/{register}/{itemowner}", Order = 1)]
        [HttpGet]
        public IHttpActionResult GetRegisterItemsByOrganization(string register, string itemowner)
        {
            List<Models.RegisterItem> itemsByOwner = _registerItemService.GetRegisterItemsFromOrganization(null, register, itemowner);
            List<Models.Api.Registeritem> ConverteditemsByOwner = new List<Models.Api.Registeritem>();

            foreach (Models.RegisterItem item in itemsByOwner)
            {
                ConverteditemsByOwner.Add(ConvertRegisterItem(item));
            }

            return Ok(ConverteditemsByOwner);
        }

        /// <summary>
        /// List items for specific organization in subregister
        /// </summary>
        /// <param name="parent">The name of the parentregister</param>
        /// <param name="register">The name of the register</param>
        /// <param name="itemowner">The name of the organization</param>
        [Route("api/subregister/{parent}/{registerOwner}/{register}/{itemowner}.{ext}")]
        [Route("api/subregister/{parent}/{registerOwner}/{register}/{itemowner}")]
        [HttpGet]
        public IHttpActionResult GetRegisterItemsByOrganization(string parent, string register, string itemowner)
        {
            SetLanguage(Request);
            List<RegisterItem> itemsByOwner = _registerItemService.GetRegisterItemsFromOrganization(parent, register, itemowner);
            List<Registeritem> ConverteditemsByOwner = new List<Registeritem>();

            foreach (Models.RegisterItem item in itemsByOwner)
            {
                ConverteditemsByOwner.Add(ConvertRegisterItem(item));
            }

            return Ok(ConverteditemsByOwner);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/metadata/synchronize")]
        [HttpGet]
        public IHttpActionResult SynchronizeDokMetadata()
        {
            new CoverageService(db).UpdateDatasetsWithCoverage();
            new DOK.Service.MetadataService(db).UpdateDatasetsWithMetadata();
            _registerService.UpdateDOKStatus();
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/metadata/synchronize/inspire-statusregister")]
        [HttpGet]
        public IHttpActionResult SynchronizeInspireStatusregister()
        {
            var register = _registerService.GetInspireStatusRegister();
            if (register.TooManySynchronizationJobsDataset())
            {
                return Ok("Can not start synchronization. Wait for other synchronization jobs to stop");
            }

            var synchronizationJobDataset = _synchronizationService.StartSynchronizationJob(register, "Datasett");
            new InspireDatasetService(db).SynchronizeInspireDatasets(synchronizationJobDataset);
            _synchronizationService.StopSynchronizationJob(synchronizationJobDataset);

            if (register.TooManySynchronizationJobsServices())
            {
                return Ok("Can not start synchronization. Wait for other synchronization jobs to stop");
            }

            var synchronizationJobServices = _synchronizationService.StartSynchronizationJob(register, "Tjenester");
            new InspireDatasetService(db).SynchronizeInspireDataServices(synchronizationJobServices);
            _synchronizationService.StopSynchronizationJob(synchronizationJobServices);
            _registerService.UpdateDateModified(register);

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/metadata/synchronize/inspire-statusregister/dataservices")]
        [HttpGet]
        public IHttpActionResult SynchronizeInspireDataServices()
        {
            var register = _registerService.GetInspireStatusRegister();
            if (register.TooManySynchronizationJobsServices())
            {
                return Ok("Can not start synchronization. Wait for other synchronization jobs to stop");
            }
            var synchronizationJob = _synchronizationService.StartSynchronizationJob(register, "Tjenester");

            new InspireDatasetService(db).SynchronizeInspireDataServices(synchronizationJob);

            _synchronizationService.StopSynchronizationJob(synchronizationJob);
            _registerService.UpdateDateModified(register);

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/metadata/synchronize/inspire-statusregister/dataset")]
        [HttpGet]
        public IHttpActionResult SynchronizeInspireDataset()
        {
            var register = _registerService.GetInspireStatusRegister();
            if (register.TooManySynchronizationJobsDataset())
            {
                return Ok("Can not start synchronization. Wait for other synchronization jobs to stop");
            }

            var synchronizationJobDataset = _synchronizationService.StartSynchronizationJob(register, "Datasett");
            new InspireDatasetService(db).SynchronizeInspireDatasets(synchronizationJobDataset);
            _synchronizationService.StopSynchronizationJob(synchronizationJobDataset);

            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/metadata/synchronize/geodatalov-statusregister")]
        [HttpGet]
        public IHttpActionResult SynchronizeGeodatalovStatusregister()
        {
            new GeodatalovDatasetService(db).SynchronizeGeodatalovDatasets();
            return Ok();
        }

        /// <summary>
        /// DokCoverageMapping
        /// </summary>
        /// <param name="name">DokCoverageMapping</param>
        [Route("api/metadata/DokCoverageMapping")]
        [HttpGet]
        public IHttpActionResult DokCoverageMapping()
        {
            return Ok(DokCoverageWmsMapping.DatasetUuidToWmsLayerMapping);
        }



        // **** HJELPEMETODER ****

        private Registeritem ConvertCurrentAndVersions(string parent, string register, string item)
        {
            Registeritem currentVersion = null;
            var versjoner = _registerItemService.GetAllVersionsOfItem(parent, register, item);
            foreach (var v in versjoner)
            {
                if (v.versioning.currentVersion == v.systemId)
                {
                    currentVersion = ConvertRegisterItem(v);

                    foreach (var ve in versjoner)
                    {
                        if (v.versionNumber != ve.versionNumber)
                        {
                            currentVersion.versions.Add(ConvertRegisterItem(ve));
                        }
                    }
                }
            }
            return currentVersion;
        }

        private Models.Api.Register ConvertRegister(Models.Register item, FilterParameters filter = null)
        {
            string registerId = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"]; //uri.Scheme + "://" + uri.Authority;
            if (registerId.Substring(registerId.Length - 1, 1) == "/") registerId = registerId.Remove(registerId.Length - 1);
            string selectedDOKMunicipality = "";
            if (filter != null && !string.IsNullOrEmpty(filter.municipality))
            {
                Models.Organization org = _registerItemService.GetMunicipalityOrganizationByNr(filter.municipality);
                if (org != null)
                {
                    selectedDOKMunicipality = org.name;
                }
            }
            var tmp = new Models.Api.Register(item, registerId, selectedDOKMunicipality);
            return tmp;
        }



        private Models.Api.Register ConvertRegisterAndNextLevel(Models.Register item, FilterParameters filter = null)
        {
            var tmp = ConvertRegister(item, filter);
            tmp.containeditems = new List<Registeritem>();
            tmp.lang = CultureHelper.GetCurrentCulture();
            if (!item.items.Any())
            {
                foreach (var registerItem in item.RegisterItems)
                {
                    tmp.containeditems.Add(ConvertRegisterItem(registerItem, filter));
                }
            }

            if (item.items.Any())
            {
                foreach (var d in item.items)
                {
                    if (d.register.containedItemClass == "Document")
                    {
                        if (d.statusId != "Submitted" && d.versioning.currentVersion == d.systemId)
                        {
                            tmp.containeditems.Add(ConvertRegisterItem(d, filter));
                        }
                    }
                    else
                    {
                        tmp.containeditems.Add(ConvertRegisterItem(d, filter));
                    }
                }
            }
            else
            {
                tmp.containedSubRegisters = new List<Models.Api.Register>();
                var subregisters = _registerService.GetSubregistersOfRegister(item);
                if (subregisters != null)
                {
                    foreach (var reg in subregisters)
                    {
                        tmp.containedSubRegisters.Add(ConvertRegister(reg));
                    }
                }
            }

            return tmp;
        }

        private Registeritem ConvertRegisterItem(RegisterItem item, FilterParameters filter = null)
        {
            string registerId = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"];  //uri.Scheme + "://" + uri.Authority;
            if (registerId.Substring(registerId.Length - 1, 1) == "/") registerId = registerId.Remove(registerId.Length - 1);
            var tmp = new Registeritem(item, registerId, filter);
            return tmp;
        }
        private Registeritem ConvertRegisterItem(RegisterItemV2 item, FilterParameters filter = null)
        {
            string registerId = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"];  //uri.Scheme + "://" + uri.Authority;
            if (registerId.Substring(registerId.Length - 1, 1) == "/") registerId = registerId.Remove(registerId.Length - 1);
            var tmp = new Registeritem(item, registerId, filter);
            return tmp;
        }


        private Models.Api.Item Convert(SearchResultItem searchitem)
        {
            var tmp = new Models.Api.Item
            {
                name = searchitem.RegisterItemName,
                description = searchitem.RegisterItemDescription,
                status = searchitem.RegisterItemStatus,
                updated = searchitem.RegisterItemUpdated,
                author = searchitem.DocumentOwner,
                showUrl = searchitem.RegisteItemUrl,
                editUrl = null

            };

            return tmp;
        }

        private RegisterItem GetCurrentVersion(string parent, string register, string item)
        {
            List<RegisterItem> itemList = new List<RegisterItem>();
            var queryresult = from d in db.RegisterItems
                              where d.register.seoname == register
                              && (d.register.parentRegister == null || d.register.parentRegister.seoname == parent)
                              && d.seoname == item
                              select d;


            foreach (var ri in queryresult.ToList())
            {
                if (ri.statusId != "Submitted")
                {
                    itemList.Add(ri);
                }
            }
            if (itemList.Count() > 1)
            {
                foreach (RegisterItem version in itemList)
                {
                    if (version.systemId == version.versioning.currentVersion)
                    {
                        return version;
                    }
                }
            }
            return queryresult.FirstOrDefault();
        }

        private Models.Register GetRegister(string parent, string register)
        {
            var queryresult = from d in db.Registers
                              where d.seoname == register
                              && (d.parentRegister == null || d.parentRegister.seoname == parent)
                              select d;

            return queryresult.FirstOrDefault();
        }

        private Models.RegisterItem GetVersion(string parent, string register, string item, int version)
        {
            var queryresult = from d in db.RegisterItems
                              where d.seoname == item
                              && d.register.seoname == register
                              && (d.register.parentRegister == null || d.register.parentRegister.seoname == parent)
                              && d.versionNumber == version
                              select d;

            return queryresult.FirstOrDefault();
        }

        private List<Models.Api.Registeritem> GetVersions(RegisterItem rit)
        {
            List<Models.Api.Registeritem> versions = new List<Models.Api.Registeritem>();
            var queryResult = from ri in db.RegisterItems
                              where ri.versioningId == rit.versioningId
                              select ri;

            foreach (var item in queryResult.ToList())
            {
                versions.Add(ConvertRegisterItem(item));
            }
            versions.OrderBy(o => o.status);
            return versions;
        }

        private void SetLanguage(HttpRequestMessage request)
        {
            string language = Culture.NorwegianCode;

            IEnumerable<string> headerValues;
            if (request.Headers.TryGetValues("Accept-Language", out headerValues))
            {
                language = headerValues.FirstOrDefault();
                if (CultureHelper.IsNorwegian(language))
                    language = Culture.NorwegianCode;
                else
                    language = Culture.EnglishCode;
            }
            else
            {
                CookieHeaderValue cookie = request.Headers.GetCookies("_culture").FirstOrDefault();
                if (cookie != null && !string.IsNullOrEmpty(cookie["_culture"].Value))
                {
                    language = cookie["_culture"].Value;
                }
            }

            var culture = new CultureInfo(language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

        }
    }
}
