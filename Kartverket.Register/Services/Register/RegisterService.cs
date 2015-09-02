using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
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

        public RegisterService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Kartverket.Register.Models.Register Filter(Kartverket.Register.Models.Register register, FilterParameters filter)
        {
            List<Kartverket.Register.Models.RegisterItem> registerItems = new List<Kartverket.Register.Models.RegisterItem>();

            if (register.containedItemClass == "EPSG")
            {
                FilterEPSGkode(register, filter, registerItems);
            }
            else if (register.containedItemClass == "Document")
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

                        if ((item.statusId != "Submitted" && item.statusId != "NotAccepted") || HtmlHelperExtensions.accessRegisterItem(item))
                        {
                            registerItems.Add(item);
                        }

                    }
                }
            }
            else if (register.containedItemClass == "Dataset")
            {
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

            return new Kartverket.Register.Models.Register
            {
                systemId = register.systemId,
                name = register.name,
                containedItemClass = register.containedItemClass,
                dateAccepted = register.dateAccepted,
                dateSubmitted = register.dateSubmitted,
                description = register.description,
                items = registerItems,
                manager = register.manager,
                managerId = register.managerId,
                modified = register.modified,
                owner = register.owner,
                ownerId = register.ownerId,
                parentRegister = register.parentRegister,
                parentRegisterId = register.parentRegisterId,
                seoname = register.seoname,
                status = register.status,
                statusId = register.statusId,
                subregisters = register.subregisters,
                replaces = register.replaces,
                targetNamespace = register.targetNamespace,
                versioning = register.versioning,
                versioningId = register.versioningId,
                versionNumber = register.versionNumber,
                accessId = register.accessId,
            };
        }

        private void FilterOrganisasjonDocument(Models.Register register, FilterParameters filter, List<Kartverket.Register.Models.RegisterItem> filterRegisterItems)
        {
            foreach (Document item in register.items)
            {
                string role = HtmlHelperExtensions.GetSecurityClaim("role");
                string user = HtmlHelperExtensions.GetSecurityClaim("organization");

                if ((item.statusId != "Submitted" && item.statusId != "NotAccepted") || HtmlHelperExtensions.accessRegisterItem(item))
                {
                    if (item.documentowner.seoname == filter.filterOrganization)
                    {
                        filterRegisterItems.Add(item);
                    }
                }
            }
        }

        private void FilterOrganisasjonDataset(Kartverket.Register.Models.Register register, FilterParameters filter, List<Kartverket.Register.Models.RegisterItem> filterRegisterItems)
        {
            foreach (Dataset item in register.items)
            {
                if (item.datasetowner.seoname == filter.filterOrganization)
                {
                    filterRegisterItems.Add(item);
                }
            }
        }

        private void FilterEPSGkode(Kartverket.Register.Models.Register register, FilterParameters filter, List<Kartverket.Register.Models.RegisterItem> filterRegisterItems)
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

        public Kartverket.Register.Models.Register GetRegisterByName(string registerName)
        {
            var queryResults = from r in _dbContext.Registers
                               where r.name == registerName || r.seoname == registerName
                               select r;

            Kartverket.Register.Models.Register register = queryResults.FirstOrDefault();
            return register;
        }

        public Kartverket.Register.Models.Register GetSubregisterByName(string parentName, string registerName)
        {
            var queryResultsSubregister = from r in _dbContext.Registers
                                          where r.seoname == registerName && r.parentRegister.seoname == parentName
                                          select r;

            Kartverket.Register.Models.Register register = queryResultsSubregister.FirstOrDefault();
            return register;
        }

        public Organization GetOrganization(string organizationName)
        {
            var queryResults = from o in _dbContext.Organizations
                               where o.name == organizationName
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
    }
}