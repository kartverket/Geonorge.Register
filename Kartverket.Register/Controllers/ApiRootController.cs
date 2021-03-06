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
using System.Web.Mvc;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Formatter;
using Kartverket.Register.App_Start;
using StatusReport = Kartverket.Register.Models.StatusReport;
using Swashbuckle.Examples;
using System.Net;
using System.Net.Http.Formatting;

namespace Kartverket.Register.Controllers
{
    public class ApiRootController : ApiController
    {
        private RegisterDbContext db;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        [System.Web.Http.Route("api/register.{ext}")]
        [System.Web.Http.Route("api/register")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetRegisters()
        {
            var list = new List<Models.Api.Register>();
            List<Models.Register> registers = _registerService.GetRegisters();
            foreach (Models.Register register in registers)
            {
                list.Add(ConvertRegister(register));
            }
            return Ok(list);
        }


        /// <summary>
        /// Gets subregister by id
        /// </summary>
        /// <param name="systemid">The uniqueidentifier for the register</param>
        //[System.Web.Http.Route("api/kodelister/{systemid}")]
        //[System.Web.Http.Route("api/kodelister/{systemid}.{ext}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetRegisterById(string systemid)
        {
            bool isValid = Guid.TryParse(systemid, out var guid);

            var codelist = new Models.Register();

            codelist = isValid ? _registerService.GetRegisterBySystemId(Guid.Parse(systemid)) : _registerService.GetRegister("kodelister", systemid);

            if (codelist == null)
            {
                codelist = _registerService.GetRegisterByName(systemid);
                return codelist == null ? (IHttpActionResult) NotFound() : Ok(ConvertRegisterAndNextLevel(codelist));
            }

            var totalNumberOfItems = codelist.items.Count;
            var result = (ConvertRegisterAndNextLevel(codelist));
            result.ContainedItemsResult.Total = totalNumberOfItems;

            return Ok(result);
        }


        /// <summary>
        /// Gets register by name
        /// </summary>
        /// <param name="registerName">The search engine optimized name of the register</param>
        /// <param name="filter"></param>
        [System.Web.Http.Route("api/{registerName}")]
        [System.Web.Http.Route("api/{registerName}.{ext}")]
        [System.Web.Http.Route("api/register/{registerName}.{ext}")]
        [System.Web.Http.Route("api/register/{registerName}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetRegisterByName(string registerName, [FromUri] FilterParameters filter = null)
        {
            var register = _registerService.GetRegisterByName(registerName);

            if (register != null)
            {
                if (filter != null && !string.IsNullOrEmpty(filter.filterOrganization))
                    filter.filterOrganization = RegisterUrls.MakeSeoFriendlyString(filter.filterOrganization);

                int totalNumberOfItems = GetTotalNumberOfCurrentItemsByOrganization(filter, register);

                if (filter != null && !string.IsNullOrEmpty(filter.filterOrganization) && register.IsDokStatusRegister())
                    totalNumberOfItems = register.items.OfType<Dataset>().Where(o => o.datasetowner.seoname.ToLower() == filter.filterOrganization.ToLower()).Count();

                if (filter != null || register.IsDokMunicipal())
                {
                    register = RegisterItems(register, filter);
                }

                var result = ConvertRegisterAndNextLevel(register, filter);
                result.ContainedItemsResult.Total = totalNumberOfItems;

                if (filter != null)
                { 
                    result.ContainedItemsResult.Limit = filter.Limit;
                    result.ContainedItemsResult.Offset = filter.Offset;
                }

                return Ok(result);
            }

            return NotFound();
        }

        private int GetTotalNumberOfCurrentItemsByOrganization(FilterParameters filter, Models.Register register)
        {
            var totalNumberOfItems = 0;

            if (register.IsInspireStatusRegister())
            { 
                if (!string.IsNullOrEmpty(filter.filterOrganization))
                  return register.RegisterItems.Where(o => o.Owner.seoname.ToLower() == filter.filterOrganization.ToLower()).Count();
                else
                    return register.RegisterItems.Count;
            }

            if (filter?.filterOrganization != null)
            {
                Models.Organization organization = _registerItemService.GetOrganizationByFilterOrganizationParameter(filter.filterOrganization);
                    totalNumberOfItems = register.NumberOfCurrentVersions(organization);
            }
            else
            {
                totalNumberOfItems = register.NumberOfCurrentVersions();
            }

            return totalNumberOfItems;
        }

        /// <summary>
        /// Gets inspiremonitoring xml
        /// </summary>
        [System.Web.Http.Route("api/inspire-statusregister/monitoring-report")]
        [System.Web.Http.Route("api/register/inspire-statusregister/monitoring-report")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult InspireMonitoring()
        {
            var inspireStatusRegister = _registerService.GetInspireStatusRegister();
            Monitoring inspireMonitoring = _inspireMonitoringService.GetInspireMonitoringReport(inspireStatusRegister);

            return Content(System.Net.HttpStatusCode.OK, inspireMonitoring, new XMLFormatter(), new MediaTypeHeaderValue("application/xml"));
        }


        /// <summary>
        /// Save dok status report to db
        /// </summary>
        [System.Web.Http.Authorize(Roles = AuthConfig.RegisterProviderRole)]
        [System.Web.Http.Route("api/{registerName}/report/save")]
        [System.Web.Http.Route("api/register/{registerName}/report/save")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult NewStatusReport(string registerName)
        {
            try
            {
                var register = _registerService.GetRegisterByName(registerName);
                _statusReportService.CreateStatusReport(register);
                return Ok("Saved");
            }
            catch (Exception e)
            {
                return Ok(e);
            }
        }

        /// <summary>
        /// Gets selected status report
        /// </summary>
        [System.Web.Http.Route("api/{registerName}/report/{id}")]
        [System.Web.Http.Route("api/{registerName}/report/{id}.{ext}")]
        [System.Web.Http.Route("api/register/{registerName}/report/{id}.{ext}")]
        [System.Web.Http.Route("api/register/{registerName}/report/{id}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult StatusReport(string id, string ext, bool dataset = true, bool service = true)
        {
            var statusReport = _statusReportService.GetStatusReportById(id);
            if (statusReport == null)
            {
                return NotFound();
            }

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
                    return Ok(new InspireDataSetStatusReport(statusReport));
                }
                if (service)
                {
                    return Ok(new InspireServiceStatusReport(statusReport));
                }

            }

            if (statusReport.IsGeodatalovDatasetReport())
            {
                return Ok(new GeodatalovDatasetStatusReport(statusReport));
            }

            if (statusReport.IsMareanoDatasetReport())
            {
                return Ok(new MareanoDatasetStatusReport(statusReport));
            }

            return Ok();


        }

        /// <summary>
        /// Gets selected status report
        /// </summary>
        //[System.Web.Http.Route("api/{registerName}/report.{ext}")]
        //[System.Web.Http.Route("api/{registerName}/report")]
        //[System.Web.Http.Route("api/register/{registerName}/report.{ext}")]
        //[System.Web.Http.Route("api/register/{registerName}/report")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult StatusReports(string registerName)
        {
            var register = _registerService.GetRegisterByName(registerName);
            List<StatusReport> statusReports;
            if (register.IsInspireStatusRegister())
            {
                List<InspireRegistryStatusReport> inspireStatusReportsApi = new List<InspireRegistryStatusReport>();
                statusReports = _statusReportService.GetInspireStatusReports();
                foreach (var report in statusReports)
                {
                    inspireStatusReportsApi.Add(new InspireRegistryStatusReport(report));
                }
                return Ok(inspireStatusReportsApi);
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
            else if (register.IsGeodatalovStatusRegister())
            {
                List<GeodatalovDatasetStatusReport> geodatalovDatasetStatusReportApi = new List<GeodatalovDatasetStatusReport>();
                statusReports = _statusReportService.GetGeodatalovStatusReports();

                foreach (var report in statusReports)
                {
                    geodatalovDatasetStatusReportApi.Add(new GeodatalovDatasetStatusReport(report));
                }
                return Ok(geodatalovDatasetStatusReportApi);
            }

            else if (register.IsMareanoStatusRegister())
            {
                List<MareanoDatasetStatusReport> mareanoDatasetStatusReportApi = new List<MareanoDatasetStatusReport>();
                statusReports = _statusReportService.GetMareanoStatusReports();

                foreach (var report in statusReports)
                {
                    mareanoDatasetStatusReportApi.Add(new MareanoDatasetStatusReport(report));
                }
                return Ok(mareanoDatasetStatusReportApi);
            }

            return Ok();
        }



        /// <summary>
        /// Save inspire monitoring report data to database.
        /// </summary>
        [System.Web.Http.Authorize(Roles = AuthConfig.RegisterProviderRole)]
        [System.Web.Http.Route("api/inspire-statusregister/monitoring-report/save")]
        [System.Web.Http.Route("api/register/inspire-statusregister/monitoring-report/save")]
        [System.Web.Http.HttpGet]
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
                register = _registerItemService.GetInspireStatusRegisterItems(register, filter);
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
        /// <param name="register">The search engine optimized name or id of the register</param>
        /// <param name="parentregister">The search engine optimized name of the parentregister</param>
        /// <param name="systemid">The uniqueidentifier for the register</param>
        [System.Web.Http.Route("api/{parentregister}/{register}.{ext}")]
        //[System.Web.Http.Route("api/kodelister/{systemid}")]
        [System.Web.Http.Route("api/{parentregister}/{register}")]
        [System.Web.Http.Route("api/subregister/{parentregister}/{parentregisterOwner}/{register}.{ext}")]
        [System.Web.Http.Route("api/subregister/{parentregister}/{parentregisterOwner}/{register}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetSubregisterByName(string parentregister, string register, string systemid = null)
        {
            var it = _registerService.GetRegister(parentregister, register);

            if(it == null && !string.IsNullOrEmpty(systemid))
               it = _registerService.GetRegisterBySystemId(Guid.Parse(systemid));

            if (it == null)
            {
                return NotFound();
            }

            var result = ConvertRegisterAndNextLevel(it);
            return Ok(result);
        }

        /// <summary>
        /// Gets current and historical versions of register
        /// </summary>
        /// <param name="registerName">The search engine optimized name of the register</param>
        /// <param name="subregisters">The path to the register</param>
        [System.Web.Http.Route("api/{registerName}/{*subregisters}")]
        [System.Web.Http.Route("api/register/versjoner/{registerName}/{*subregisters}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetRegisterItemByName(string registerName, string subregisters = null)
        {
            var path = RegisterUrls.GetPath(registerName, subregisters);
            string systemId = RegisterUrls.GetSystemIdFromPath(registerName + "/" + subregisters);
            string format = RegisterUrls.GetFileExtension(registerName + "/" + subregisters);
            path = RegisterUrls.RemoveExtension(path);

            var mediatype = GetFormattingForMediaType(format);

            var register = _registerService.GetRegisterByPath(path);
            if (register == null)
            {
                var currentVersion = ConvertCurrentAndVersions(null, registerName, RegisterUrls.GetItemFromPath(subregisters));

                if (currentVersion != null)
                    return Content(HttpStatusCode.OK, currentVersion, mediatype.Formatter, mediatype.MediaTypeHeader);

                return NotFound();
            }

            if (!string.IsNullOrEmpty(systemId)) { 
                var currentVersion = ConvertCurrentAndVersions(register, systemId);
                return Content(HttpStatusCode.OK, currentVersion, mediatype.Formatter, mediatype.MediaTypeHeader);
            }
            else {
                var result = (ConvertRegisterAndNextLevel(register));
                return Content(HttpStatusCode.OK, result, mediatype.Formatter, mediatype.MediaTypeHeader);
            }
        }


        private MediaType GetFormattingForMediaType(string extension)
        {

            MediaType mediaType = new MediaType();

            if(extension == "csv")
            {
                mediaType.Formatter = new CsvFormatter();
                mediaType.MediaTypeHeader = new MediaTypeHeaderValue("text/csv");
            }
            else if (extension == "gml")
            {
                mediaType.Formatter = new GMLFormatter();
                mediaType.MediaTypeHeader = new MediaTypeHeaderValue("application/gml+xml");
            }
            else if (extension == "rdf")
            {
                mediaType.Formatter = new SKOSFormatter();
                mediaType.MediaTypeHeader = new MediaTypeHeaderValue("application/gml+xml");
            }
            else if (extension == "rss")
            {
                mediaType.Formatter = new SyndicationFeedFormatter();
                mediaType.MediaTypeHeader = new MediaTypeHeaderValue("application/rss+xml");
            }
            else if (extension == "atom")
            {
                mediaType.Formatter = new SyndicationFeedFormatter();
                mediaType.MediaTypeHeader = new MediaTypeHeaderValue("application/atom+xml");
            }
            else if (extension == "xml")
            {
                mediaType.Formatter = new XMLFormatter();
                mediaType.MediaTypeHeader = new MediaTypeHeaderValue("application/xml");
            }
            else
            {
                mediaType.Formatter = new JsonMediaTypeFormatter();
                mediaType.MediaTypeHeader = new MediaTypeHeaderValue("application/json");
            }

            return mediaType;
        }

        class MediaType
        {
            public MediaTypeFormatter Formatter { get; set; }
            public MediaTypeHeaderValue MediaTypeHeader { get; set; }
        }


        ///// <summary>
        ///// Gets current and historical versions of register item by register- organization- and registeritem-name 
        ///// </summary>
        ///// <param name="registerName">The search engine optimized name of the register</param>
        ///// <param name="item">The search engine optimized name of the register item</param>
        ///// <param name="id"></param>
        //[System.Web.Http.Route("api/{registerName}/{item}/{id}.{ext}")]
        //[System.Web.Http.Route("api/{registerName}/{item}/{id}")]
        //[System.Web.Http.Route("api/register/{registerName}/{itemowner}/{item}.{ext}")]
        //[System.Web.Http.Route("api/register/{registerName}/{itemowner}/{item}")]
        //[System.Web.Http.Route("api/register/versjoner/{registerName}/{itemowner}/{item}.{ext}")]
        //[System.Web.Http.Route("api/register/versjoner/{registerName}/{itemowner}/{item}")]
        //[System.Web.Http.HttpGet]
        //public IHttpActionResult GetRegisterItemByName(string registerName, string item, string id = null)
        //{
        //    var register = _registerService.GetRegister(null, registerName);
        //    if (register == null)
        //    {
        //        return NotFound();
        //    }
        //    Registeritem currentVersion;

        //    currentVersion = register.IsInspireStatusRegister() ? ConvertInspireRegister(registerName, item) : ConvertCurrentAndVersions(null, registerName, item);

        //    return Ok(currentVersion);
        //}

        /// <summary>
        /// Gets current and historical versions of register item by register- organization- and registeritem-name 
        /// </summary>
        /// <param name="registerName">The search engine optimized name of the register</param>
        /// <param name="itemowner">The search engine optimized name of the register item owner</param>
        /// <param name="item">The search engine optimized name of the register item</param>
        [System.Web.Http.Route("api/register/{registerName}/{itemowner}/{item}/monitoring-report")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult InspireDatasetMonitoring(string registerName, string itemowner, string item)
        {
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
        [System.Web.Http.Route("api/register/versjoner/{register}/{itemowner}/{item}/{version}/no.{ext}")]
        [System.Web.Http.Route("api/register/versjoner/{register}/{itemowner}/{item}/{version}/no")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetRegisterItemByVersionNr(string register, string item, int version)
        {
            var registerItem = _registerItemService.GetRegisterItem(null, register, item, version);
            return Ok(ConvertRegisterItem(registerItem));
        }


        ///// <summary>
        ///// Gets register item by parent-register, register- organization- and registeritem-name 
        ///// </summary>
        ///// <param name="parentregister">The search engine optimized name of the parent register</param>
        ///// <param name="register">The search engine optimized name of the register</param>
        ///// <param name="item">The search engine optimized name of the register item</param>
        ///// <param name="id"></param>
        //[System.Web.Http.Route("api/{parentregister}/{register}/{item}/{id}")]
        //[System.Web.Http.Route("api/subregister/{parentregister}/{registerowner}/{register}/{itemowner}/{item}")]
        //[System.Web.Http.Route("api/{parentregister}/{register}/{item}/{id}.{ext}")]
        //[System.Web.Http.Route("api/subregister/{parentregister}/{registerowner}/{register}/{itemowner}/{item}.{ext}")]
        //[System.Web.Http.HttpGet]
        //public IHttpActionResult GetSubregisterItemByName(string parentregister, string register, string item, string id = null)
        //{
        //    Models.Api.Registeritem currentVersion = ConvertCurrentAndVersions(parentregister, register, item);
        //    return Ok(currentVersion);
        //}

        ///// <summary>
        ///// Gets sosi-codelist
        ///// </summary>
        ///// <param name="register">The search engine optimized name of the register</param>
        ///// <param name="item">The search engine optimized name of the register item</param>
        //[System.Web.Http.Route("api/sosi-kodelister/{register}/{item}")]
        //[System.Web.Http.HttpGet]
        //public IHttpActionResult GetSubregisterItemByName(string register, string item)
        //{
        //    Models.Api.Registeritem currentVersion = ConvertCurrentAndVersions("sosi-kodelister", register, item);
        //    return Ok(currentVersion);
        //}

        /// <summary>
        /// List items for specific organization 
        /// </summary>
        /// <param name="name">The name of the organization</param>
        [System.Web.Http.Route("api/register/organisasjon/{name}")]
        [System.Web.Http.HttpGet]
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
        [System.Web.Http.Route("api/register/{register}/{itemowner}.{ext}", Order = 1)]
        [System.Web.Http.Route("api/register/{register}/{itemowner}", Order = 1)]
        [System.Web.Http.HttpGet]
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
        [System.Web.Http.Route("api/subregister/{parent}/{registerOwner}/{register}/{itemowner}.{ext}")]
        [System.Web.Http.Route("api/subregister/{parent}/{registerOwner}/{register}/{itemowner}")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetRegisterItemsByOrganization(string parent, string register, string itemowner)
        {
            List<RegisterItem> itemsByOwner = _registerItemService.GetRegisterItemsFromOrganization(parent, register, itemowner);
            List<Registeritem> ConverteditemsByOwner = new List<Registeritem>();

            foreach (Models.RegisterItem item in itemsByOwner)
            {
                ConverteditemsByOwner.Add(ConvertRegisterItem(item));
            }

            return Ok(ConverteditemsByOwner);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [System.Web.Http.Route("api/metadata/synchronize")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult SynchronizeDokMetadata()
        {
            new CoverageService(db).UpdateDatasetsWithCoverage();
            new DOK.Service.MetadataService(db).UpdateDatasetsWithMetadata();
            _registerService.UpdateDOKStatus();
            _registerService.UpdateRegisterItemV2Translations();
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [System.Web.Http.Route("api/metadata/synchronizev2")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult SynchronizeMetadata()
        {
            _registerService.UpdateRegisterItemV2Translations();
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [System.Web.Http.Route("api/metadata/synchronize/inspire-statusregister")]
        [System.Web.Http.HttpGet]
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
        [System.Web.Http.Route("api/metadata/synchronize/inspire-statusregister/dataservices")]
        [System.Web.Http.HttpGet]
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
        [System.Web.Http.Route("api/metadata/synchronize/inspire-statusregister/dataset")]
        [System.Web.Http.HttpGet]
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
        [System.Web.Http.Route("api/metadata/synchronize/geodatalov-statusregister")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult SynchronizeGeodatalovStatusregister()
        {
            new GeodatalovDatasetService(db).SynchronizeGeodatalovDatasets();
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [System.Web.Http.Route("api/metadata/synchronize/mareano-statusregister")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult SynchronizeMareanoStatusregister()
        {
            new MareanoDatasetService(db).SynchronizeMareanoDatasets();
            return Ok();
        }

        /// <summary>
        /// DokCoverageMapping
        /// </summary>
        [System.Web.Http.Route("api/metadata/DokCoverageMapping")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult DokCoverageMapping()
        {
            return Ok(DokCoverageWmsMapping.DatasetUuidToWmsLayerMapping);
        }

        //[System.Web.Http.Authorize(Roles = AuthConfig.RegisterProviderRole)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[System.Web.Http.Route("api/codelist/update/kommunereform-2020")]
        //[System.Web.Http.HttpGet]
        //public IHttpActionResult UpdateMunicipalities()
        //{
        //    try
        //    {
        //        new UpdateCodelistService(db).UpdateMunicipalitiesAllStatus();
        //        new UpdateCodelistService(db).UpdateCountiesAllStatus();
        //        new UpdateCodelistService(db).UpdateMunicipalities();
        //        try
        //        {
        //            new UpdateCodelistService(db).UpdateOrganizationsAll();
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex);
        //        }

        //        new UpdateCodelistService(db).UpdateCenterPoint();
        //        new UpdateCodelistService(db).UpdateBbox();
        //    }
        //    catch (Exception ex){
        //        Log.Error(ex);
        //    }

        //    return Ok();
        //}

        //[System.Web.Http.Authorize(Roles = AuthConfig.RegisterProviderRole)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[System.Web.Http.Route("api/codelist/update/kommunereform-gyldig-dato")]
        //[System.Web.Http.HttpGet]
        //public IHttpActionResult UpdateMunicipalitiesValidDate()
        //{
        //    try
        //    {
        //        new UpdateCodelistService(db).UpdateMunicipalitiesAllValidDate();
        //        new UpdateCodelistService(db).UpdateCountiesAllValidDate();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //    }

        //    return Ok();
        //}

        /// <summary>
        /// Get all Geolett
        /// </summary>
        /// <remarks>
        /// Med GeoLett skal vi forbedre datagrunnlaget for plan- og byggesaksprosessen. Det gjør vi ved å fremme innovasjon av metodebruk og digitale verktøy, legge til rette for effektiv deling av informasjon og støtte kommuner og sektormyndigheter for å forbedre kvaliteten på grunndata.
        /// </remarks>
        /// <returns></returns>
        //[ResponseType(typeof(GeoLett))]
        //[System.Web.Http.Route("api/geolett")]
        //[System.Web.Http.HttpGet]
        //[SwaggerResponseExample(HttpStatusCode.OK, typeof(GeoLettModelExample))]
        //public IHttpActionResult GetGeoLettRegister()
        //{
        //    try
        //    {
        //        var geolettRegister = new GeoLettService().Get();
        //        return Ok(geolettRegister);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //    }

        //    return Ok();
        //}


        // **** HJELPEMETODER ****

        private Registeritem ConvertCurrentAndVersions(Models.Register register, string itemSystemId = null)
        {
            Registeritem currentVersion = null;
            var versjoner = _registerItemService.GetAllVersionsOfRegisterItem(register, itemSystemId);
            if (versjoner != null)
            {
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
                    if (d is Document document)
                    {
                        if (document.isCurrentVersion())
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
                var subregisters = _registerService.GetSubregisters(item);
                if (subregisters != null)
                {
                    foreach (var reg in subregisters)
                    {
                        tmp.containedSubRegisters.Add(ConvertRegister(reg));
                    }
                }
            }

            tmp.ContainedItemsResult = new Result(filter, tmp.containeditems.Count);

            return tmp;
        }

        private Registeritem ConvertCurrentAndVersions(string parent, string register, string item)
        {
            Registeritem currentVersion = null;
            var versjoner = _registerItemService.GetAllVersionsOfItem(parent, register, item);
            if (versjoner != null)
            {
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
            }
            return currentVersion;
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


        private Item Convert(SearchResultItem searchitem)
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
    }

    public class GeoLettModelExample : IExamplesProvider
    {
        public object GetExamples()
        {
            var geolettRegister = new GeoLettService().Get();
            List<GeoLett> geoLetts = new List<GeoLett>();
            var hulEik = geolettRegister.Where(g => g.KontekstType == "Byggesak-treff-biomangfold-utvalgtnaturtype-hul eik").FirstOrDefault();
            geoLetts.Add(hulEik);
            return geoLetts;
        }
    }
}
