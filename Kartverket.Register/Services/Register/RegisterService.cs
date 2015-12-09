using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Services.RegisterItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Services.Register
{
    public class RegisterService : IRegisterService
    {
        private readonly RegisterDbContext _dbContext;
        private IRegisterItemService _registerItemService;
        private IMunicipalityService _municipalityService;

        public RegisterService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerItemService = new RegisterItemService(_dbContext);
            _municipalityService = new MunicipalityService();
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
            if (register.name == "Det offentlige kartgrunnlaget - Kommunalt")
            {
                if (!string.IsNullOrWhiteSpace(filter.municipality))
                {
                    AddNationalDatasets(registerItems);

                    //Finn valgt kommune
                    Models.RegisterItem municipal = _registerItemService.GetMunicipalByNr(filter.municipality);

                    if (municipal != null)
                    {
                        GetMunicipalDatasetBySelectedMunicipality(register, registerItems, municipal);
                    }
                }
                else
                {
                    AccessControlService access = new AccessControlService();
                    if (access.IsAdmin())
                    {
                        GetMunicipalDatasetAddedByAdmin(register, registerItems, access);
                    }
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

        private static void GetMunicipalDatasetAddedByAdmin(Models.Register register, List<Models.RegisterItem> registerItems, AccessControlService access)
        {
            foreach (Dataset item in register.items)
            {
                if (access.GetSecurityClaim("organization") == item.datasetowner.name)
                {
                    registerItems.Add(item);
                }
            }
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

        private void AddNationalDatasets(List<Models.RegisterItem> registerItems)
        {
            Models.Register DOK = GetRegisterByName("Det offentlige kartgrunnlaget");
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

                    if ((item.statusId != "Submitted") || HtmlHelperExtensions.accessRegisterItem(item))
                    {
                        registerItems.Add(item);
                    }

                }
            }
        }

        private void FilterOrganisasjonDocument(Models.Register register, FilterParameters filter, List<Models.RegisterItem> filterRegisterItems)
        {
            foreach (Document item in register.items)
            {
                string role = HtmlHelperExtensions.GetSecurityClaim("role");
                string user = HtmlHelperExtensions.GetSecurityClaim("organization");

                if ((item.statusId != "Submitted") || HtmlHelperExtensions.accessRegisterItem(item))
                {
                    if (item.documentowner.seoname == filter.filterOrganization)
                    {
                        filterRegisterItems.Add(item);
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

                if (filterInspire == null)
                {
                    filterInspire = item.inspireRequirement.value;
                }
                if (filterNational == null)
                {
                    filterNational = item.nationalRequirement.value;
                }
                if (filterNationalSea == null)
                {
                    filterNationalSea = item.nationalSeasRequirement.value;
                }

                var queryResult = from e in _dbContext.EPSGs
                                  where e.dimensionId == filterDimensjon
                                  && e.inspireRequirement.value == filterInspire
                                  && e.nationalRequirement.value == filterNational
                                  && e.nationalSeasRequirement.value == filterNationalSea
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
                return "xml";
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

        public Organization GetOrganization()
        {
            AccessControlService access = new AccessControlService();
            string organizationLogin = access.GetSecurityClaim("organization");
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
            CodelistValue user = access.Municipality();
            string organizationNr = _municipalityService.LookupOrganizationNumberFromMunicipalityCode(user.value);

            var queryResults = from o in _dbContext.Organizations
                               where o.number == organizationNr
                               select o;

            return queryResults.FirstOrDefault();
        }
    }
}