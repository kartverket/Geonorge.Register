using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Services.RegisterItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;
using Kartverket.Register.Migrations;

namespace Kartverket.Register.Services.Register
{
    public class RegisterService : IRegisterService
    {
        private readonly RegisterDbContext _dbContext;
        private IRegisterItemService _registerItemService;
        private IDatasetDeliveryService _datasetDeliveryService;

        public RegisterService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerItemService = new RegisterItemService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
        }

        public Models.Register FilterRegisterItems(Models.Register register, FilterParameters filter)
        {
            var registerItems = new List<Models.RegisterItem>();
            var registerItemsv2 = new List<RegisterItemV2>();

            if (register.containedItemClass == "Document")
            {
                FilterDocument(register, filter, registerItems);
            }
            else if (register.containedItemClass == "Dataset")
            {
                FilterDataset(register, filter, registerItems);
            }
            else
            {
                foreach (var item in register.items)
                {
                    registerItems.Add(item);
                }
            }

            if (register.RegisterItems.Any())
            {
                    foreach (var item in register.RegisterItems)
                    {
                        if (filter.filterOrganization != null)
                        {
                            if (item.Owner.seoname == filter.filterOrganization)
                            {
                                registerItemsv2.Add(item);
                            }
                        }
                        else registerItemsv2.Add(item);
                }
            }

            register.items = registerItems;
            register.RegisterItems = registerItemsv2;
            return register;
        }

        private void FilterDataset(Models.Register register, FilterParameters filter, List<Models.RegisterItem> registerItems)
        {
            AccessControlService access = new AccessControlService(_dbContext);
            if (register.IsDokMunicipal())
            {
                if (!string.IsNullOrWhiteSpace(filter.municipality))
                {
                    GetNationalDatasets(registerItems);
                    Models.RegisterItem municipal = _registerItemService.GetMunicipalityOrganizationByNr(filter.municipality);

                    if (municipal != null)
                    {
                        GetMunicipalDatasetBySelectedMunicipality(register, registerItems, municipal);
                    }
                }
                else if (access.IsMunicipalUser())
                {
                    GetNationalDatasets(registerItems);
                    GetMunicipalDatasetsByUser(register, registerItems);
                    filter.municipality = GetOrganizationByUserName().MunicipalityCode;
                }

                register.items.Clear();
            }

            if (!string.IsNullOrWhiteSpace(filter.filterOrganization))
            {
                FilterOrganisasjonDataset(register, filter, registerItems);
            }
            else
            {
                foreach (Dataset item in register.items)
                {
                    registerItems.Add(item);
                }
            }
        }

        private void GetMunicipalDatasetsByUser(Models.Register register, List<Models.RegisterItem> registerItems)
        {
            Organization municipalityOrganization = GetOrganizationByUserName();
            GetMunicipalDatasetBySelectedMunicipality(register, registerItems, municipalityOrganization);
        }

        private static void GetMunicipalDatasetBySelectedMunicipality(Models.Register register, List<Models.RegisterItem> registerItems, Models.RegisterItem municipal)
        {
            //Gå gjennom alle datasett i registeret
            foreach (Dataset item in register.items)
            {
                //Gå gjennom dekningslisten for datasettet
                foreach (CoverageDataset coverage in item.Coverage)
                {
                    //Er det registrert dekning av datasett for valgt kommune...
                    if (coverage.Municipality.systemId == municipal.systemId)
                    {
                        registerItems.Add(item);
                    }
                }
            }
        }

        public List<Models.RegisterItem> GetDatasetBySelectedMunicipality(Models.Register register, Models.RegisterItem municipal)
        {
            List<Models.RegisterItem> datasets = new List<Models.RegisterItem>();
            GetNationalDatasets(datasets);

            //Gå gjennom alle datasett i registeret
            foreach (Dataset item in register.items)
            {
                //Gå gjennom dekningslisten for datasettet
                foreach (CoverageDataset coverage in item.Coverage)
                {
                    //Er det registrert dekning av datasett for valgt kommune...
                    if (coverage.Municipality.systemId == municipal.systemId)
                    {
                        datasets.Add(item);
                    }
                }
            }
            return datasets.OrderBy(n => n.name).ToList();
        }

        private void GetNationalDatasets(List<Models.RegisterItem> registerItems)
        {
            Models.Register DOK = GetRegisterByName("DOK-statusregisteret");
            foreach (Models.RegisterItem item in DOK.items)
            {
                registerItems.Add(item);
            }
        }

        private void GetNationalDatasetsConfirmdByMunicipality(List<Models.RegisterItem> registerItems, Organization municipality)
        {
            Models.Register DOK = GetRegisterByName("DOK-statusregisteret");
            foreach (Dataset dataset in DOK.items)
            {
                foreach (CoverageDataset coverage in dataset.Coverage)
                {
                    if (coverage.Municipality. systemId == municipality.systemId)
                    {
                        registerItems.Add(dataset);
                    }
                }
            }
        }

        private void FilterDocument(Models.Register register, FilterParameters filter, List<Models.RegisterItem> registerItems)
        {
            if (!string.IsNullOrWhiteSpace(filter.filterOrganization))
            {
                FilterOrganisasjonDocument(register, filter, registerItems);
            }
            else
            {
                foreach (Document item in register.items)
                {
                    if (item.isCurrentVersion())
                    {
                        if ((item.statusId != "Submitted") || HtmlHelperExtensions.AccessRegisterItem(item))
                        {
                            registerItems.Add(item);
                        }
                    }
                }
            }
        }

        public void UpdateDOKStatus()
        {
            Models.Register DOK = GetRegisterByName("DOK-statusregisteret");
            foreach (Models.Dataset item in DOK.items)
            {
                item.dokDeliveryMetadataStatusId = _datasetDeliveryService.GetMetadataStatus(item.Uuid, item.dokDeliveryMetadataStatusAutoUpdate, item.dokDeliveryMetadataStatusId);
                item.dokDeliveryProductSheetStatusId = GetDOKStatus(item.ProductSheetUrl, item.dokDeliveryProductSheetStatusAutoUpdate, item.dokDeliveryProductSheetStatusId);
                item.dokDeliveryPresentationRulesStatusId = GetDOKStatus(item.PresentationRulesUrl, item.dokDeliveryPresentationRulesStatusAutoUpdate, item.dokDeliveryPresentationRulesStatusId);
                item.dokDeliveryProductSpecificationStatusId = GetDOKStatus(item.ProductSpecificationUrl, item.dokDeliveryProductSpecificationStatusAutoUpdate, item.dokDeliveryProductSpecificationStatusId);
                item.dokDeliverySosiRequirementsStatusId = GetSosiRequirements(item.Uuid, item.GetProductSpecificationUrl(), item.dokDeliverySosiStatusAutoUpdate, item.dokDeliverySosiRequirementsStatusId);
                item.dokDeliveryGmlRequirementsStatusId = GetGmlRequirements(item.Uuid, item.dokDeliveryGmlRequirementsStatusAutoUpdate, item.dokDeliveryGmlRequirementsStatusId);
                item.dokDeliveryWmsStatusId = GetDokDeliveryServiceStatus(item.Uuid, item.dokDeliveryWmsStatusAutoUpdate, item.dokDeliveryWmsStatusId, item.UuidService);
                item.dokDeliveryAtomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(item.Uuid, item.dokDeliveryAtomFeedStatusAutoUpdate, item.dokDeliveryAtomFeedStatusId);
                item.dokDeliveryWfsStatusId = _datasetDeliveryService.GetWfsStatus(item.Uuid, item.dokDeliveryWfsStatusAutoUpdate, item.dokDeliveryWfsStatusId);
                item.dokDeliveryDistributionStatusId = GetDeliveryDownloadStatus(item.Uuid, item.dokDeliveryDistributionStatusAutoUpdate, item.dokDeliveryDistributionStatusId);
            }
            _dbContext.SaveChanges();
        }

        public string GetDOKStatus(string url, bool autoUpdate, string currentStatus)
        {
            string statusValue = currentStatus;

            if (autoUpdate)
            {
                statusValue = "deficient";
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    System.Net.WebClient c = new System.Net.WebClient();
                    c.Encoding = System.Text.Encoding.UTF8;

                    var data = c.DownloadString(url + ".json");
                    var response = Newtonsoft.Json.Linq.JObject.Parse(data);
                    var status = response["status"];
                    if (status != null)
                    {
                        string statusvalue = status?.ToString();

                        if (statusvalue == "Gyldig" || statusvalue == "SOSI godkjent")
                            return "good";
                        else
                            return "useable";
                    }
                }


                }
                catch (Exception ex)
                {
                    return "deficient";
                }

            }

            return statusValue;
        }

        public string GetDokDeliveryServiceStatus(string uuid, bool autoUpdate, string currentStatus, string uuidService)
        {
            bool hasServiceUrl = false;

            string status = (!string.IsNullOrEmpty(currentStatus) ? currentStatus : "notset");

            if (autoUpdate)
            {
                try
                { 
                    string metadataUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + uuid;
                    System.Net.WebClient c = new System.Net.WebClient();
                    c.Encoding = System.Text.Encoding.UTF8;

                    var json = c.DownloadString(metadataUrl);

                    dynamic metadata = Newtonsoft.Json.Linq.JObject.Parse(json);

                    if (metadata != null)
                    {
    
                        if (metadata.Related != null)
                        {
                            foreach (var service in metadata.Related)
                            {
                                if (service.DistributionDetails != null)
                                {
                                    if (service.DistributionDetails.Protocol.Value == "OGC:WMS")
                                        hasServiceUrl = true;
                                }
                            }
                        }

                    }

                }
                catch (Exception e)
                {

                }



                string statusUrl = WebConfigurationManager.AppSettings["StatusApiUrl"] + "monitorApi/serviceDetail?uuid=";
                statusUrl = statusUrl + uuidService;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    try
                    {
                        HttpResponseMessage response = client.GetAsync(new Uri(statusUrl)).Result;

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var text = response.Content.ReadAsStringAsync().Result;
                            dynamic data = Newtonsoft.Json.Linq.JObject.Parse(text);

                            var responseTime = (double)data.svartid;
                            var resposeGetCapabilities = false;
                            var supportCors = false;
                            var epsgSupport = false;
                            var featuresSupport = false;
                            var hasLegend = false;
                            var hasCoverage = false;
                            var connectSoso = false;

                            if(data.connect.vurdering == "yes")
                                resposeGetCapabilities = true;

                            if (data.connect.vurdering == "soso")
                                connectSoso = true;

                            if (data.cors.vurdering == "yes")
                                supportCors = true;

                            if (data.epsgSupported.vurdering == "yes")
                                epsgSupport = true; //Todo check epsg code

                            if (data.hasGFI.vurdering == "yes")
                                featuresSupport = true;

                            if (data.hasLegend.vurdering == "yes")
                                hasLegend = true;

                            if (data.bbox.vurdering == "yes")
                                hasCoverage = true;

                            if(!hasServiceUrl)
                                status = "deficient";
                            //Grønn på WMS:
                            //Respons fra GetCapabilities
                            //Svartid innen 4 sekunder
                            //Støtter CORS
                            //EPSG: 25832, 25833, 25835
                            //Støtter egenskapsspørringer
                            //Støtter tegnforklaring
                            //Oppgir dekningsområde
                            else if ((resposeGetCapabilities && responseTime <= 4
                                && supportCors && epsgSupport && featuresSupport
                                && hasLegend && hasCoverage) || connectSoso)
                                status = "good";
                            //Gul:
                            //Respons fra GetCapabilities
                            //Svartid innen 10 sekunder
                            //Støtter CORS
                            //EPSG: 25833, 25835 eller 32633
                            //Støtter tegnforklaring
                            //Oppgir dekningsområde
                            else if ((resposeGetCapabilities && responseTime <= 10
                                && epsgSupport && hasLegend && hasCoverage) || connectSoso)
                                status = "useable";
                            //Rød:
                            //Feiler på en av testene til gul
                            else
                                status = "deficient";
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return status;
        }

        public ICollection<Models.Register> OrderBy(ICollection<Models.Register> registers, string orderBy)
        {
            if (!registers.Any()) return registers;
            var text = HttpContext.Current.Request.QueryString["text"] != null ? HttpContext.Current.Request.QueryString["text"].ToString() : "";
            var filterVertikalt = HttpContext.Current.Request.QueryString["filterVertikalt"] != null ? HttpContext.Current.Request.QueryString["filterVertikalt"].ToString() : "";
            var municipality = HttpContext.Current.Request.QueryString["municipality"] != null ? HttpContext.Current.Request.QueryString["municipality"].ToString() : "";
            var filterHorisontalt = HttpContext.Current.Request.QueryString["filterHorisontalt"] != null ? HttpContext.Current.Request.QueryString["filterHorisontalt"].ToString() : "";
            var InspireRequirementParam = HttpContext.Current.Request.QueryString["InspireRequirement"] != null ? HttpContext.Current.Request.QueryString["InspireRequirement"].ToString() : "";
            var nationalRequirementParam = HttpContext.Current.Request.QueryString["nationalRequirement"] != null ? HttpContext.Current.Request.QueryString["nationalRequirement"].ToString() : "";
            var nationalSeaRequirementParam = HttpContext.Current.Request.QueryString["nationalSeaRequirement"] != null ? HttpContext.Current.Request.QueryString["nationalSeaRequirement"].ToString() : "";

            if (HttpContext.Current.Request.QueryString.Count < 1)
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["sortingType"] != null && string.IsNullOrEmpty(orderBy))
                        orderBy = HttpContext.Current.Session["sortingType"].ToString();

                    if (HttpContext.Current.Session["text"] != null && string.IsNullOrEmpty(text))
                        text = HttpContext.Current.Session["text"].ToString();

                    if (HttpContext.Current.Session["filterVertikalt"] != null && string.IsNullOrEmpty(filterVertikalt))
                        filterVertikalt = HttpContext.Current.Session["filterVertikalt"].ToString();

                    if (HttpContext.Current.Session["filterHorisontalt"] != null && string.IsNullOrEmpty(filterHorisontalt))
                        filterHorisontalt = HttpContext.Current.Session["filterHorisontalt"].ToString();

                    if (HttpContext.Current.Session["InspireRequirement"] != null && string.IsNullOrEmpty(InspireRequirementParam))
                        InspireRequirementParam = HttpContext.Current.Session["InspireRequirement"].ToString();

                    if (HttpContext.Current.Session["nationalRequirement"] != null && string.IsNullOrEmpty(nationalRequirementParam))
                        nationalRequirementParam = HttpContext.Current.Session["nationalRequirement"].ToString();

                    if (HttpContext.Current.Session["nationalSeaRequirement"] != null && string.IsNullOrEmpty(nationalSeaRequirementParam))
                        nationalSeaRequirementParam = HttpContext.Current.Session["nationalSeaRequirement"].ToString();

                    if (HttpContext.Current.Session["municipality"] != null && string.IsNullOrEmpty(municipality))
                        municipality = HttpContext.Current.Session["municipality"].ToString();

                    string redirect = HttpContext.Current.Request.Path + "?sorting=" + orderBy;
                    bool shallRedirect = false;

                    if (text != "")
                    {
                        redirect = redirect + "&text=" + text;
                        shallRedirect = true;
                    }

                    if (filterVertikalt != "")
                    {
                        if (filterVertikalt.Contains(","))
                            filterVertikalt = filterVertikalt.Replace(",false", "");
                        redirect = redirect + "&filterVertikalt=" + filterVertikalt;
                        shallRedirect = true;
                    }

                    if (filterHorisontalt != "")
                    {
                        if (filterHorisontalt.Contains(","))
                            filterHorisontalt = filterHorisontalt.Replace(",false", "");
                        redirect = redirect + "&filterHorisontalt=" + filterHorisontalt;
                        shallRedirect = true;
                    }

                    if (InspireRequirementParam != "")
                    {
                        redirect = redirect + "&inspireRequirement=" + InspireRequirementParam;
                        shallRedirect = true;
                    }

                    if (nationalRequirementParam != "")
                    {
                        redirect = redirect + "&nationalRequirement=" + nationalRequirementParam;
                        shallRedirect = true;
                    }

                    if (nationalSeaRequirementParam != "")
                    {
                        redirect = redirect + "&nationalSeaRequirement=" + nationalSeaRequirementParam;
                        shallRedirect = true;
                    }

                    if (nationalSeaRequirementParam != "")
                    {
                        redirect = redirect + "&municipality=" + municipality;
                        shallRedirect = true;
                    }

                    if (shallRedirect)
                    {
                        HttpContext.Current.Response.Redirect(redirect);
                    }

                }
            }
            HttpContext.Current.Session["sortingType"] = orderBy;
            HttpContext.Current.Session["municipality"] = municipality;
            HttpContext.Current.Session["text"] = text;
            HttpContext.Current.Session["filterVertikalt"] = filterVertikalt;
            HttpContext.Current.Session["filterHorisontalt"] = filterHorisontalt;
            HttpContext.Current.Session["InspireRequirement"] = InspireRequirementParam;
            HttpContext.Current.Session["nationalRequirement"] = nationalRequirementParam;
            HttpContext.Current.Session["nationalSeaRequirement"] = nationalSeaRequirementParam;


            var sortedList = registers.OrderBy(o => o.name).ToList();
            switch (orderBy)
            {
                case "name_desc":
                    sortedList = registers.OrderByDescending(o => o.name).ToList();
                    break;
                case "submitter":
                    sortedList = registers.OrderBy(o => o.owner.name).ToList();
                    break;
                case "submitter_desc":
                    sortedList = registers.OrderByDescending(o => o.owner.name).ToList();
                    break;
                case "status":
                    sortedList = registers.OrderBy(o => o.status.description).ToList();
                    break;
                case "status_desc":
                    sortedList = registers.OrderByDescending(o => o.status.description).ToList();
                    break;
                case "dateSubmitted":
                    sortedList = registers.OrderBy(o => o.dateSubmitted).ToList();
                    break;
                case "dateSubmitted_desc":
                    sortedList = registers.OrderByDescending(o => o.dateSubmitted).ToList();
                    break;
                case "modified":
                    sortedList = registers.OrderBy(o => o.modified).ToList();
                    break;
                case "modified_desc":
                    sortedList = registers.OrderByDescending(o => o.modified).ToList();
                    break;
                case "dateAccepted":
                    sortedList = registers.OrderBy(o => o.dateAccepted).ToList();
                    break;
                case "dateAccepted_desc":
                    sortedList = registers.OrderByDescending(o => o.dateAccepted).ToList();
                    break;
                case "description":
                    sortedList = registers.OrderBy(o => o.description).ToList();
                    break;
                case "description_desc":
                    sortedList = registers.OrderByDescending(o => o.description).ToList();
                    break;
                case "owner":
                    sortedList = registers.OrderBy(o => o.owner.NameTranslated()).ToList();
                    break;
                case "owner_desc":
                    sortedList = registers.OrderByDescending(o => o.owner.NameTranslated()).ToList();
                    break;
            }

            return sortedList;
        }

        public string GetDeliveryDownloadStatus(string uuid, bool autoUpdate, string currentStatus)
        {
            string status = currentStatus;

            try
            {
                string metadataUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + uuid;
                System.Net.WebClient c = new System.Net.WebClient();
                c.Encoding = System.Text.Encoding.UTF8;

                var json = c.DownloadString(metadataUrl);

                dynamic metadata = Newtonsoft.Json.Linq.JObject.Parse(json);

                bool hasDistributionUrl = false;

                if (metadata != null)
                {
                    var distribution = metadata.DistributionDetails;
                    if (distribution != null && distribution.Protocol != null && distribution.URL != null)
                    {
                        if(Uri.IsWellFormedUriString(distribution.URL.Value, UriKind.Absolute) 
                            && (distribution.Protocol.Value == "WWW:DOWNLOAD-1.0-http--download" 
                            || distribution.Protocol.Value == "GEONORGE:FILEDOWNLOAD" || distribution.Protocol.Value == "GEONORGE:DOWNLOAD"))
                            hasDistributionUrl = true;
                    }

                }

                if (autoUpdate)
                {
                    if (currentStatus == "good" || currentStatus == "good")
                        status = "good";
                    else if (currentStatus == "useable" || currentStatus == "useable" || hasDistributionUrl)
                        status = "useable";
                    else
                        status = "deficient";
                }
            }

            catch (Exception ex)
            {
                return "deficient";
            }


            return status;
        }

        public string GetSosiRequirements(string uuid ,string url, bool autoUpdate, string currentStatus)
        {
 
            string statusValue = currentStatus;
            bool qualitySpecificationsResult = false;
            bool sosiDistributionFormat = false;
            string distributionProtocol = "";

            if (autoUpdate)
            {
                try
                {
                    string metadataUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + uuid;
                    System.Net.WebClient c = new System.Net.WebClient();
                    c.Encoding = System.Text.Encoding.UTF8;

                    var json = c.DownloadString(metadataUrl);

                    dynamic metadata = Newtonsoft.Json.Linq.JObject.Parse(json);

                    if (metadata != null)
                    {
                        var qualitySpecifications = metadata.QualitySpecifications;
                        if(qualitySpecifications != null && qualitySpecifications.Count > 0)
                        {
                            for(int q=0;q < qualitySpecifications.Count; q++)
                            { 
                                var qualitySpecification = qualitySpecifications[q];
                                string explanation = qualitySpecification.Explanation.Value;
                                if (explanation.StartsWith("SOSI-filer"))
                                {
                                    qualitySpecificationsResult = true;
                                    break;
                                }
                            }
                        }

                        var distributionFormats = metadata.DistributionFormats;
                        if (distributionFormats != null && distributionFormats.Count > 0)
                        {
                            foreach(var format in distributionFormats)
                            {
                                if (format.Name == "SOSI")
                                    sosiDistributionFormat = true;
                            }
                        }

                        var distributionDetails = metadata.DistributionDetails;
                        if (distributionDetails != null && distributionDetails.Count > 0)
                        {
                            distributionProtocol = distributionDetails.Protocol;
                        }

                    }

                    if (qualitySpecificationsResult)
                        statusValue = "good";
                    else if(sosiDistributionFormat)
                        statusValue = "useable";
                    else
                        statusValue = "deficient";
                }

                catch (Exception ex)
                {
                    return "deficient";
                }
            }

            return statusValue;

        }

        public string GetGmlRequirements(string uuid, bool autoUpdate, string currentStatus)
        {

            string statusValue = currentStatus;
            bool qualitySpecificationsResult = false;
            bool gmlDistributionFormat = false;
            string distributionProtocol = "";

            if (autoUpdate)
            {
                try
                {
                    string metadataUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/getdata/" + uuid;
                    System.Net.WebClient c = new System.Net.WebClient();
                    c.Encoding = System.Text.Encoding.UTF8;

                    var json = c.DownloadString(metadataUrl);

                    dynamic metadata = Newtonsoft.Json.Linq.JObject.Parse(json);

                    if (metadata != null)
                    {
                        var qualitySpecifications = metadata.QualitySpecifications;
                        if (qualitySpecifications != null && qualitySpecifications.Count > 0)
                        {
                            for (int q = 0; q < qualitySpecifications.Count; q++)
                            {
                                var qualitySpecification = qualitySpecifications[q];
                                string explanation = qualitySpecification.Explanation.Value;
                                if (explanation.StartsWith("GML-filer"))
                                {
                                    qualitySpecificationsResult = true;
                                    break;
                                }
                            }
                        }

                        var distributionFormats = metadata.DistributionFormats;
                        if (distributionFormats != null && distributionFormats.Count > 0)
                        {
                            foreach (var format in distributionFormats)
                            {
                                if (format.Name == "GML")
                                    gmlDistributionFormat = true;
                            }
                        }

                        var distributionDetails = metadata.DistributionDetails;
                        if (distributionDetails != null && distributionDetails.Count > 0)
                        {
                            distributionProtocol = distributionDetails.Protocol;
                        }

                    }

                    if (qualitySpecificationsResult)
                        statusValue = "good";
                    else if (gmlDistributionFormat)
                        statusValue = "useable";
                    else
                        statusValue = "deficient";
                }

                catch (Exception ex)
                {
                    return "deficient";
                }
            }

            return statusValue;

        }

        private void FilterOrganisasjonDocument(Models.Register register, FilterParameters filter, List<Models.RegisterItem> filterRegisterItems)
        {
            foreach (Document item in register.items)
            {
                if (item.isCurrentVersion())
                {
                    if ((item.statusId != "Submitted") || HtmlHelperExtensions.AccessRegisterItem(item))
                    {
                        if (item.documentowner.seoname == filter.filterOrganization)
                        {
                            filterRegisterItems.Add(item);
                        }
                    }
                }
            }
        }

        private void FilterOrganisasjonDataset(Models.Register register, FilterParameters filter, List<Models.RegisterItem> filterRegisterItems)
        {
            foreach (Dataset item in register.items)
            {
                if (item.datasetowner.seoname == filter.filterOrganization)
                {
                    filterRegisterItems.Add(item);
                }
            }
        }

        private void FilterEPSGkode(Models.Register register, FilterParameters filter, List<Models.RegisterItem> filterRegisterItems)
        {
            bool filterHorisontalt = filter.filterHorisontalt;
            bool filterVertikalt = filter.filterVertikalt;
            string filterDimensjon;
            string filterInspire = filter.InspireRequirement;
            string filterNational = filter.nationalRequirement;
            string filterNationalSea = filter.nationalSeaRequirement;

            foreach (EPSG item in register.items)
            {
                if (filterHorisontalt && filterVertikalt)
                {
                    filterDimensjon = "compound";
                }
                else
                {
                    if (filterHorisontalt)
                    {
                        filterDimensjon = "horizontal";
                    }
                    else if (filterVertikalt)
                    {
                        filterDimensjon = "vertical";
                    }
                    else
                    {
                        filterDimensjon = item.dimensionId;
                    }
                }

                var queryResult = from e in _dbContext.EPSGs
                                  where e.dimensionId == filterDimensjon
                                  && e.systemId == item.systemId
                                  select e;

                if (queryResult.Count() > 0)
                {
                    Kartverket.Register.Models.EPSG epsgkode = queryResult.First();
                    filterRegisterItems.Add(epsgkode);
                }
                filterHorisontalt = filter.filterHorisontalt;
                filterVertikalt = filter.filterVertikalt;
            }
        }

        public string ContentNegotiation(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;
            HttpRequestBase request = context.HttpContext.Request;

            if (request.AcceptTypes.Contains("application/json"))
            {
                response.ContentType = "application/json";
                return "json";
            }
            if (request.AcceptTypes.Contains("application/xml"))
            {
                response.ContentType = "application/xml";
                return "gml";
            }
            if (request.AcceptTypes.Contains("application/rdf+xml"))
            {
                response.ContentType = "application/xml+rdf";
                return "skos";
            }
            if (request.AcceptTypes.Contains("application/atom+xml"))
            {
                response.ContentType = "application/atom+xml";
                return "atom";
            }
            if (request.AcceptTypes.Contains("application/rss+xml"))
            {
                response.ContentType = "application/rss+xml";
                return "rss";
            }
            if (request.AcceptTypes.Contains("text/csv"))
            {
                response.ContentType = "text/csv";
                return "csv";
            }
            return null;
        }

        public Models.Register GetRegisterByName(string registerName)
        {
            var queryResults = from r in _dbContext.Registers
                               where r.name == registerName || r.seoname == registerName
                               select r;

            Models.Register register = queryResults.FirstOrDefault();
            if (register != null)
                register.AddMissingTranslations();
            return register;
        }

        public Models.Register GetSubregisterByName(string parentName, string registerName)
        {
            var queryResultsSubregister = from r in _dbContext.Registers
                                          where r.seoname == registerName && r.parentRegister.seoname == parentName
                                          select r;

            Models.Register register = queryResultsSubregister.FirstOrDefault();
            return register;
        }

        private Organization GetOrganization()
        {
            string organizationLogin = HtmlHelperExtensions.GetSecurityClaim("organization");
            var queryResults = from o in _dbContext.Organizations
                               where organizationLogin == o.name
                               select o;

            Organization organization = queryResults.FirstOrDefault();
            return organization;
        }

        public List<Models.Register> GetRegisters()
        {
            var queryResult = from r in _dbContext.Registers
                              where r.parentRegisterId == null
                              select r;

            return queryResult.ToList();

        }

        public List<Models.Register> GetSubregisters()
        {
            var queryResult = from r in _dbContext.Registers
                              where r.parentRegisterId != null
                              select r;

            return queryResult.ToList();

        }

        public List<Models.Register> GetSubregistersOfRegister(Models.Register register)
        {
            var queryResult = from r in _dbContext.Registers
                              where r.parentRegisterId == register.systemId
                              select r;

            return queryResult.ToList();

        }

        public Models.Register GetRegisterBySystemId(Guid systemId)
        {
            var queryResult = from r in _dbContext.Registers
                              where r.systemId == systemId
                              select r;

            return queryResult.FirstOrDefault();
        }

        public Models.Register GetRegister(string parentRegisterName, string registerName)
        {
            if (string.IsNullOrWhiteSpace(parentRegisterName))
            {
                var queryResults = from r in _dbContext.Registers
                                   where r.seoname == registerName &&
                                   r.parentRegister == null
                                   select r;

                var result = queryResults.FirstOrDefault();
                if(result != null)
                    result.AddMissingTranslations();
                
                return result;
            }
            else
            {
                var queryResults = from r in _dbContext.Registers
                                   where r.seoname == registerName &&
                                   r.parentRegister.seoname == parentRegisterName
                                   select r;

                var result = queryResults.FirstOrDefault();
                if (result != null)
                    result.AddMissingTranslations();
                
                return result;
            }
        }

        public Guid GetRegisterId(string parentRegisterName, string registerName)
        {
            if (string.IsNullOrWhiteSpace(parentRegisterName))
            {
                var queryResults = from r in _dbContext.Registers
                    where r.seoname == registerName &&
                          r.parentRegister == null
                    select r.systemId;

                var result = queryResults.FirstOrDefault();
                return result;
            }
            else
            {
                var queryResults = from r in _dbContext.Registers
                    where r.seoname == registerName &&
                          r.parentRegister.seoname == parentRegisterName
                    select r.systemId;

                var result = queryResults.FirstOrDefault();
                return result;
            }
        }

        public Models.Register SetStatus(Models.Register register, Models.Register originalRegister)
        {
            originalRegister.statusId = register.statusId;
            if (originalRegister.statusId != "Valid" && register.statusId == "Valid")
            {
                originalRegister.dateAccepted = DateTime.Now;
            }
            if (originalRegister.statusId == "Valid" && register.statusId != "Valid")
            {
                originalRegister.dateAccepted = null;
            }
            return originalRegister;
        }

        public bool RegisterHasChildren(string parentname, string registername)
        {
            var queryResultsRegisterItem = ((from o in _dbContext.RegisterItems
                                             where o.register.seoname == registername
                                             || o.register.parentRegister.seoname == registername
                                             select o.systemId).Union(
                                               from r in _dbContext.Registers
                                               where r.parentRegister.seoname == registername
                                               select r.systemId));

            if (queryResultsRegisterItem.Count() > 0) return true;
            else return false;
        }

        public bool validationName(object model)
        {
            if (model is Models.Register)
            {
                Models.Register register = (Models.Register)model;
                var queryResults = from o in _dbContext.Registers
                                   where o.name == register.name && o.systemId != register.systemId
                                   select o.systemId;

                if (queryResults.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public Organization GetOrganizationByUserName()
        {
            AccessControlService access = new AccessControlService(_dbContext);
            if (access.IsMunicipalUser())
            {
                string organizationNr = access.GetOrganizationNumber();
                return GetOrganizationByOrganizationNr(organizationNr);
            }
            else
            {
                return GetOrganization();
            }
        }

        public Organization GetOrganizationByOrganizationNr(string number)
        {
            var queryResults = from o in _dbContext.Organizations
                               where o.number == number
                               select o;

            return queryResults.FirstOrDefault();
        }

        public Organization GetOrganizationByMunicipalityCode(string municipalityCode)
        {
            var queryResults = from o in _dbContext.Organizations
                               where o.MunicipalityCode == municipalityCode
                               select o;

            return queryResults.FirstOrDefault();
        }

        public Models.Register GetDokMunicipalRegister()
        {
            var queryResult = from r in _dbContext.Registers
                              where r.name == "Det offentlige kartgrunnlaget - Kommunalt"
                              select r;
            return queryResult.FirstOrDefault();
        }

        public List<Models.RegisterItem> GetConfirmdDatasetBySelectedMunicipality(Models.Register dokMunicipalRegister, Organization municipality)
        {
            List<Models.RegisterItem> datasets = new List<Models.RegisterItem>();
            GetNationalDatasetsConfirmdByMunicipality(datasets, municipality);

            //Gå gjennom alle datasett i registeret
            foreach (Dataset item in dokMunicipalRegister.items)
            {
                //Gå gjennom dekningslisten for datasettet
                foreach (CoverageDataset coverage in item.Coverage)
                {
                    //Er det registrert dekning av datasett for valgt kommune...
                    if (coverage.Municipality.systemId == municipality.systemId)
                    {
                        datasets.Add(item);
                    }
                }
            }
            return datasets;
        }

        public Guid GetOrganizationIdByUserName()
        {
            var user = GetOrganizationByUserName();
            return user?.systemId ?? GetOrganizationKartverket();
        }

        private Guid GetOrganizationKartverket()
        {
            var queryResults = from o in _dbContext.Organizations
                where o.name == "Kartverket"
                select o.systemId;

            return queryResults.FirstOrDefault();
        }

        public Guid GetInspireStatusRegisterId()
        {
            var queryResults = from o in _dbContext.Registers
                where o.name == "Inspire statusregister"
                select o.systemId;

            return queryResults.FirstOrDefault();
        }

        public Guid GetGeodatalovStatusRegisterId()
        {
            var queryResults = from o in _dbContext.Registers
                where o.name == "Geodatalov statusregister"
                select o.systemId;

            return queryResults.FirstOrDefault();
        }

        public List<Models.Register> GetCodelistRegisters()
        {
            var queryResults = from o in _dbContext.Registers
                where o.containedItemClass == "CodelistValue"
                select o;

            var codelistRegisters = new List<Models.Register>();
            foreach (var item in queryResults)
            {
                codelistRegisters.Add(item);
            }
            codelistRegisters.OrderBy(r => r.NameTranslated());

            return codelistRegisters;
        }

        public void DeleteRegister(Models.Register register)
        {
            _registerItemService.DeleteRegisterItems(register.items);
            DeleteSubregisters(register.subregisters);
            _dbContext.Registers.Remove(register);
            _dbContext.SaveChanges();
        }

        private void DeleteSubregisters(ICollection<Models.Register> subregisters)
        {
            foreach (var subregister in subregisters.ToList())
            {
                DeleteRegister(subregister);
            }       
        }
    }
}