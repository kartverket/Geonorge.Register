using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Services.RegisterItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Kartverket.Register.Services.Register
{
    public class RegisterService : IRegisterService
    {
        private readonly RegisterDbContext _dbContext;
        private IRegisterItemService _registerItemService;

        public RegisterService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerItemService = new RegisterItemService(_dbContext);
        }

        public Models.Register FilterRegisterItems(Models.Register register, FilterParameters filter)
        {
            List<Models.RegisterItem> registerItems = new List<Models.RegisterItem>();

            if (register.containedItemClass == "EPSG")
            {
                FilterEPSGkode(register, filter, registerItems);
            }
            else if (register.containedItemClass == "Document")
            {
                FilterDocument(register, filter, registerItems);
            }
            else if (register.containedItemClass == "Dataset")
            {
                FilterDataset(register, filter, registerItems);

            }
            else if (register.containedItemClass == "CodelistValue")
            {
                foreach (CodelistValue item in register.items)
                {
                    registerItems.Add(item);
                }
            }
            else if (register.containedItemClass == "Organization")
            {
                foreach (Organization item in register.items)
                {
                    registerItems.Add(item);
                }
            }
            else if (register.containedItemClass == "NameSpace")
            {
                foreach (NameSpace item in register.items)
                {
                    registerItems.Add(item);
                }
            }
            else
            {
                registerItems = register.items.ToList();
            }

            register.items = registerItems;
            return register;
        }

        private void FilterDataset(Models.Register register, FilterParameters filter, List<Models.RegisterItem> registerItems)
        {
            AccessControlService access = new AccessControlService();
            if (register.name == "Det offentlige kartgrunnlaget - Kommunalt")
            {
                if (!string.IsNullOrWhiteSpace(filter.municipality))
                {
                    AddNationalDatasets(registerItems);
                    Models.RegisterItem municipal = _registerItemService.GetMunicipalOrganizationByNr(filter.municipality);

                    if (municipal != null)
                    {
                        GetMunicipalDatasetBySelectedMunicipality(register, registerItems, municipal);
                    }
                }
                else if (access.IsMunicipalUser())
                {
                    AddNationalDatasets(registerItems);
                    GetMunicipalDatasetsByUser(register, registerItems);
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
            AccessControlService access = new AccessControlService();
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
            AddNationalDatasets(datasets);

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

        private void AddNationalDatasets(List<Models.RegisterItem> registerItems)
        {
            Models.Register DOK = GetRegisterByName("DOK-statusregisteret");
            foreach (Models.RegisterItem item in DOK.items)
            {
                registerItems.Add(item);
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
                    string role = HtmlHelperExtensions.GetSecurityClaim("role");
                    string user = HtmlHelperExtensions.GetSecurityClaim("organization");

                    if (item.isCurrentVersion())
                    {
                        if ((item.statusId != "Submitted") || HtmlHelperExtensions.accessRegisterItem(item))
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
                item.dokDeliveryMetadataStatusId = GetMetadataStatus(item.Uuid, item.dokDeliveryMetadataStatusAutoUpdate, item.dokDeliveryMetadataStatusId);
                item.dokDeliveryProductSheetStatusId = GetDOKStatus(item.ProductSheetUrl, item.dokDeliveryProductSheetStatusAutoUpdate, item.dokDeliveryProductSheetStatusId);
                item.dokDeliveryPresentationRulesStatusId = GetDOKStatus(item.PresentationRulesUrl, item.dokDeliveryPresentationRulesStatusAutoUpdate, item.dokDeliveryPresentationRulesStatusId);
                item.dokDeliveryProductSpecificationStatusId = GetDOKStatus(item.ProductSpecificationUrl, item.dokDeliveryProductSpecificationStatusAutoUpdate, item.dokDeliveryProductSpecificationStatusId);
                item.dokDeliveryDistributionStatusId = GetDeliveryDistributionStatus(item);
                item.dokDeliverySosiRequirementsStatusId = GetSosiRequirements(item.Uuid, item.GetProductSpecificationUrl(), item.dokDeliverySosiStatusAutoUpdate, item.dokDeliverySosiRequirementsStatusId);
                item.dokDeliveryGmlRequirementsStatusId = GetGmlRequirements(item.Uuid, item.dokDeliveryGmlRequirementsStatusAutoUpdate, item.dokDeliveryGmlRequirementsStatusId);
                item.dokDeliveryWmsStatusId = GetDokDeliveryServiceStatus(item);
            }
            _dbContext.SaveChanges();
        }

        public string GetMetadataStatus(string uuid, bool autoUpdate, string currentStatus)
        {
            string statusValue = currentStatus;
            if (autoUpdate) { 
            try
            {
                if (!string.IsNullOrEmpty(uuid))
                {
                    System.Net.WebClient c = new System.Net.WebClient();
                    c.Encoding = System.Text.Encoding.UTF8;
                    string url = System.Web.Configuration.WebConfigurationManager.AppSettings["EditorUrl"] + "api/validatemetadata/" + uuid;
                    var data = c.DownloadString(url);
                    var response = Newtonsoft.Json.Linq.JObject.Parse(data);
                    var status = response["Status"];
                    if (status != null)
                    {
                        string statusvalue = status?.ToString();

                        if (statusvalue == "OK")
                            return "good";
                    }
                }

                return "deficient";

            }
            catch (Exception ex)
            {
                return "deficient";
            }

            }

            return statusValue;
        }

        public string GetDOKStatus(string url, bool autoUpdate, string currentStatus)
        {
            string statusValue = currentStatus;

            if (autoUpdate)
            { 

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

        public string GetDokDeliveryServiceStatus(Dataset item)
        {
            string status = (!string.IsNullOrEmpty(item.dokDeliveryWmsStatusId) ? item.dokDeliveryWmsStatusId : "notset");

            if (item.dokDeliveryWmsStatusAutoUpdate)
            { 
                string statusUrl = WebConfigurationManager.AppSettings["StatusApiUrl"] + "monitorApi/serviceDetail?uuid=";
                statusUrl = statusUrl + item.UuidService;
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

                            if(data.connect.vurdering == "yes")
                                resposeGetCapabilities = true;

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

                            //Grønn på WMS:
                            //Respons fra GetCapabilities
                            //Svartid innen 4 sekunder
                            //Støtter CORS
                            //EPSG: 25832, 25833, 25835
                            //Støtter egenskapsspørringer
                            //Støtter tegnforklaring
                            //Oppgir dekningsområde
                            if (resposeGetCapabilities && responseTime <= 4
                                && supportCors && epsgSupport && featuresSupport
                                && hasLegend && hasCoverage)
                                status = "good";
                            //Gul:
                            //Respons fra GetCapabilities
                            //Svartid innen 10 sekunder
                            //Støtter CORS
                            //EPSG: 25833, 25835 eller 32633
                            //Støtter tegnforklaring
                            //Oppgir dekningsområde
                            else if (resposeGetCapabilities && responseTime <= 10
                                && supportCors && epsgSupport && hasLegend && hasCoverage)
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

        public string GetDeliveryDistributionStatus(Dataset item)
        {
            string status = item.dokDeliveryDistributionStatusId;

            if (item.dokDeliveryDistributionStatusAutoUpdate)
            {
                //Regler:
                //Hvis enten WFS eller Atom - feed er grønn blir "Nedlastingsstatus" grønn
                //HVis enten WFS eller Atom Feed er gul blir "Nedlastingsstatus" gul" 
                //Hvis både WFS og Atom - feed er rød blir "Nedlastingsstatus" rød
                if (item.dokDeliveryWfsStatusId == "good" || item.dokDeliveryAtomFeedStatusId == "good")
                    status = "good";
                else if (item.dokDeliveryWfsStatusId == "useable" || item.dokDeliveryAtomFeedStatusId == "useable")
                    status = "useable";
                else if (item.dokDeliveryWfsStatusId == "deficient" && item.dokDeliveryAtomFeedStatusId == "deficient")
                    status = "deficient";
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
                    else if(sosiDistributionFormat && (distributionProtocol == "GEONORGE:DOWNLOAD" || distributionProtocol == "WWW:DOWNLOAD-1.0-http--download"))
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
                string role = HtmlHelperExtensions.GetSecurityClaim("role");
                string user = HtmlHelperExtensions.GetSecurityClaim("organization");

                if (item.isCurrentVersion())
                {
                    if ((item.statusId != "Submitted") || HtmlHelperExtensions.accessRegisterItem(item))
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

                //if (filterInspire == null)
                //{
                //    filterInspire = item.inspireRequirement.value;
                //}
                //if (filterNational == null)
                //{
                //    filterNational = item.nationalRequirement.value;
                //}
                //if (filterNationalSea == null)
                //{
                //    filterNationalSea = item.nationalSeasRequirement.value;
                //}

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
                filterInspire = filter.InspireRequirement;
                filterNational = filter.nationalRequirement;
                filterNationalSea = filter.nationalSeaRequirement;
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
                               where organizationLogin.Contains(o.name)
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

                return queryResults.FirstOrDefault();
            }
            else
            {
                var queryResults = from r in _dbContext.Registers
                                   where r.seoname == registerName &&
                                   r.parentRegister.seoname == parentRegisterName
                                   select r;

                return queryResults.FirstOrDefault();
            }
            throw new NotImplementedException();
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
            AccessControlService access = new AccessControlService();
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
    }
}