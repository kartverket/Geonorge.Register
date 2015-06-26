using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SearchParameters = Kartverket.Register.Models.SearchParameters;
using SearchResult = Kartverket.Register.Models.SearchResult;
using System.Data.Entity.Infrastructure;
using Kartverket.Register.Models;
using System.Web.Configuration;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Services.Search
{
    public class SearchService : ISearchService
    {
        private readonly RegisterDbContext _dbContext;

        public SearchService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public FilterResult Filter(SearchParameters parameters)
        {
            string itemClass = "";
            var queryResultsRegister = from o in _dbContext.Registers
                                       where o.name == parameters.Register || o.seoname == parameters.Register
                                       select o.containedItemClass;

            itemClass = queryResultsRegister.FirstOrDefault();

            if (itemClass == "Document")
            {
                var queryResults = (from d in _dbContext.Documents
                                    where (d.register.name == parameters.Register || d.register.seoname == parameters.Register)
                                    && (d.documentowner.name == parameters.Owner || d.documentowner.seoname == parameters.Owner)
                                    select new Filter
                                    {
                                        systemId = d.systemId,
                                        name = d.name,
                                        description = d.description,
                                        submitterId = d.submitterId,
                                        submitter = d.submitter,
                                        dateSubmitted = d.dateSubmitted,
                                        modified = d.modified,
                                        statusId = d.statusId,
                                        status = d.status,
                                        dateAccepted = d.dateAccepted,
                                        register = d.register,
                                        registerId = d.registerId,
                                        seoname = d.seoname,
                                        thumbnail = d.thumbnail,
                                        documentowner = d.documentowner,
                                        documentownerId = d.documentownerId,
                                        documentUrl = d.documentUrl,
                                        currentVersion = d.versioning.currentVersion
                                    });

                int NumFound = queryResults.Count();
                List<Filter> items = new List<Filter>();
                int skip = parameters.Offset;
                skip = skip - 1;

                queryResults = SortingQueryResults(parameters, queryResults, skip);

                foreach (Filter doc in queryResults)
                {
                    var item = new Filter
                    {
                        systemId = doc.systemId,
                        name = doc.name,
                        description = doc.description,
                        submitterId = doc.submitterId,
                        submitter = doc.submitter,
                        dateSubmitted = doc.dateSubmitted,
                        modified = doc.modified,
                        statusId = doc.statusId,
                        status = doc.status,
                        dateAccepted = doc.dateAccepted,
                        register = doc.register,
                        registerId = doc.registerId,
                        seoname = doc.seoname,
                        thumbnail = doc.thumbnail,
                        documentowner = doc.documentowner,
                        documentownerId = doc.documentownerId,
                        documentUrl = doc.documentUrl,
                        currentVersion = doc.currentVersion
                    };

                    items.Add(item);
                }

                return new FilterResult
                {
                    ItemsFilter = items,
                    Limit = parameters.Limit,
                    Offset = parameters.Offset,
                    NumFound = NumFound
                };


            }

            else if (itemClass == "Dataset")
            {
                var queryResults = (from d in _dbContext.Datasets
                                    where (d.register.name == parameters.Register || d.register.seoname == parameters.Register)
                                    && (d.datasetowner.name == parameters.Owner || d.datasetowner.seoname == parameters.Owner)
                                    select new Filter
                                    {
                                        systemId = d.systemId,
                                        name = d.name,
                                        description = d.description,
                                        submitterId = d.submitterId,
                                        submitter = d.submitter,
                                        dateSubmitted = d.dateSubmitted,
                                        modified = d.modified,
                                        statusId = d.statusId,
                                        status = d.status,
                                        dateAccepted = d.dateAccepted,
                                        register = d.register,
                                        registerId = d.registerId,
                                        seoname = d.seoname,
                                        datasetownerId = d.datasetownerId,
                                        datasetowner = d.datasetowner,
                                        datasetthumbnail = d.datasetthumbnail,
                                        DistributionArea = d.DistributionArea,
                                        DistributionFormat = d.DistributionFormat,
                                        DistributionUrl = d.DistributionUrl,
                                        MetadataUrl = d.MetadataUrl,
                                        Notes = d.Notes,
                                        PresentationRulesUrl = d.PresentationRulesUrl,
                                        ProductSheetUrl = d.ProductSheetUrl,
                                        ProductSpecificationUrl = d.ProductSpecificationUrl,
                                        theme = d.theme,
                                        ThemeGroupId = d.ThemeGroupId,
                                        Uuid = d.Uuid,
                                        WmsUrl = d.WmsUrl,
                                        currentVersion = d.versioning.currentVersion
                                    });

                int NumFound = queryResults.Count();
                List<Filter> items = new List<Filter>();
                int skip = parameters.Offset;
                skip = skip - 1;
                queryResults = SortingQueryResults(parameters, queryResults, skip);

                foreach (Filter dat in queryResults)
                {
                    var item = new Filter
                    {
                        systemId = dat.systemId,
                        name = dat.name,
                        description = dat.description,
                        submitterId = dat.submitterId,
                        submitter = dat.submitter,
                        dateSubmitted = dat.dateSubmitted,
                        modified = dat.modified,
                        statusId = dat.statusId,
                        status = dat.status,
                        dateAccepted = dat.dateAccepted,
                        register = dat.register,
                        registerId = dat.registerId,
                        seoname = dat.seoname,
                        datasetownerId = dat.datasetownerId,
                        datasetowner = dat.datasetowner,
                        datasetthumbnail = dat.datasetthumbnail,
                        DistributionArea = dat.DistributionArea,
                        DistributionFormat = dat.DistributionFormat,
                        DistributionUrl = dat.DistributionUrl,
                        MetadataUrl = dat.MetadataUrl,
                        Notes = dat.Notes,
                        PresentationRulesUrl = dat.PresentationRulesUrl,
                        ProductSheetUrl = dat.ProductSheetUrl,
                        ProductSpecificationUrl = dat.ProductSpecificationUrl,
                        theme = dat.theme,
                        ThemeGroupId = dat.ThemeGroupId,
                        Uuid = dat.Uuid,
                        WmsUrl = dat.WmsUrl,
                        currentVersion = dat.currentVersion
                    };

                    items.Add(item);
                }

                return new FilterResult
                {
                    ItemsFilter = items,
                    Limit = parameters.Limit,
                    Offset = parameters.Offset,
                    NumFound = NumFound
                };
            }

            return new FilterResult
            {
                ItemsFilter = null,
                Limit = 0,
                Offset = 0,
                NumFound = 0
            };

        }

        public Kartverket.Register.Models.Register Search(Kartverket.Register.Models.Register register, string text)
        {
            List<Kartverket.Register.Models.RegisterItem> registerItems = new List<Kartverket.Register.Models.RegisterItem>();
            List<Kartverket.Register.Models.Register> subregisters = new List<Kartverket.Register.Models.Register>();

            if (register.containedItemClass == "Organization")
            {
                var queryResults = (from o in _dbContext.Organizations
                                    where o.name.Contains(text)
                                    || o.description.Contains(text)
                                    || o.shortname.Contains(text)
                                    select o);

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResults)
                    {
                        Organization organisasjon = item;
                        registerItems.Add(organisasjon);
                    }
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
                    versionNumber = register.versionNumber
                };
            }

            else if (register.containedItemClass == "Document")
            {
                var queryResults = (from d in _dbContext.Documents
                                    where (d.register.name.Contains(register.name) || d.register.seoname.Contains(register.seoname))
                                    && (d.name.Contains(text)
                                    || d.description.Contains(text)
                                    || d.documentowner.name.Contains(text))
                                    select d);

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResults)
                    {
                        Document document = item;
                        registerItems.Add(document);
                    }
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
                    versionNumber = register.versionNumber
                };
            }

            else if (register.containedItemClass == "NameSpace")
            {
                var queryResults = (from d in _dbContext.NameSpases
                                    where (d.register.name.Contains(register.name) || d.register.seoname.Contains(register.seoname))
                                    && (d.name.Contains(text)
                                    || d.description.Contains(text))
                                    select d);

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResults)
                    {
                        NameSpace nameSpace = item;
                        registerItems.Add(nameSpace);
                    }
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
                    versionNumber = register.versionNumber
                };
            }

            else if (register.containedItemClass == "Dataset")
            {
                var queryResults = (from d in _dbContext.Datasets
                                    where (d.register.name.Contains(register.name) || d.register.seoname.Contains(register.seoname))
                                    && (d.name.Contains(text)
                                    || d.description.Contains(text)
                                    || d.datasetowner.name.Contains(text))
                                    select d);

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResults)
                    {
                        Dataset dataset = item;
                        registerItems.Add(dataset);
                    }
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
                    versionNumber = register.versionNumber
                };
            }

            else if (register.containedItemClass == "EPSG")
            {
                var queryResults = (from e in _dbContext.EPSGs
                                    where e.name.Contains(text)
                                    || e.description.Contains(text)
                                    || e.epsgcode.Contains(text)
                                    select e);

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResults)
                    {
                        EPSG epsg = item;
                        registerItems.Add(epsg);
                    }
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
                    versionNumber = register.versionNumber
                };
            }

            else if (register.containedItemClass == "CodelistValue")
            {
                var queryResults = (from e in _dbContext.CodelistValues
                                    where e.name.Contains(text)
                                    || e.description.Contains(text)
                                    || e.value.Contains(text)
                                    select e);

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResults)
                    {
                        CodelistValue code = item;
                        registerItems.Add(code);
                    }
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
                    versionNumber = register.versionNumber
                };
            }

            else if (register.containedItemClass == "Register")
            {
                var queryResults = from d in _dbContext.Registers
                                   where d.parentRegister.name == register.name
                                   && d.parentRegister.containedItemClass == register.containedItemClass
                                   && (d.name.Contains(text)
                                   || d.description.Contains(text))
                                   select d;

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResults)
                    {
                        Kartverket.Register.Models.Register subregister = item;
                        subregisters.Add(subregister);
                    }

                }

                // Finnes det organisasjoner i dette registeret?
                var queryResultsOrganization = from o in _dbContext.Organizations
                                               where o.register.parentRegister.name.Contains(register.name) && (
                                               o.register.name.Contains(text)
                                               || o.register.description.Contains(text)
                                               || o.register.name.Contains(text)
                                               || o.name.Contains(text)
                                               || o.description.Contains(text))
                                               select o;
                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResultsOrganization)
                    {
                        Organization organization = item;
                        registerItems.Add(organization);
                    }

                }

                // Finnes det Document i dette registeret?
                var queryResultsDocument = from d in _dbContext.Documents
                                           where d.register.parentRegister.name.Contains(register.name) && (
                                               d.register.name.Contains(text)
                                           || d.name.Contains(text)
                                           || d.description.Contains(text)
                                           || d.documentowner.name.Contains(text))
                                           select d;
                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResultsDocument)
                    {
                        Document document = item;
                        registerItems.Add(document);
                    }

                }

                // Finnes det Datasett i dette registeret?
                var queryResultsDataset = from d in _dbContext.Datasets
                                          where d.register.parentRegister.name.Contains(register.name) && (
                                          d.register.name.Contains(text)
                                          || d.name.Contains(text)
                                          || d.description.Contains(text)
                                          || d.datasetowner.name.Contains(text))
                                          select d;

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResultsDataset)
                    {
                        Dataset dataset = item;
                        registerItems.Add(dataset);
                    }

                }

                // Finnes det Koder i dette registeret?
                var queryResultsKodelister = from d in _dbContext.CodelistValues
                                             where d.register.parentRegister.name.Contains(register.name) && (
                                                d.register.name.Contains(text)
                                             || d.name.Contains(text)
                                             || d.description.Contains(text)
                                             || d.value.Contains(text))
                                             select d;

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResultsKodelister)
                    {
                        CodelistValue codelistValue = item;
                        registerItems.Add(codelistValue);
                    }

                }

                // Finnes det navenrom i dette registeret?
                var queryResultsNameSpace = from d in _dbContext.NameSpases
                                             where d.register.parentRegister.name.Contains(register.name) && (
                                                d.register.name.Contains(text)
                                             || d.name.Contains(text)
                                             || d.description.Contains(text))
                                             select d;

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResultsNameSpace)
                    {
                        NameSpace nameSpace = item;
                        registerItems.Add(nameSpace);
                    }

                }

                // Finnes det Epsg-koder i dette registeret?
                var queryResultsEpsg = from e in _dbContext.EPSGs
                                       where e.register.parentRegister.name.Contains(register.name) && (
                                       e.register.name.Contains(text)
                                       || e.name.Contains(text)
                                       || e.description.Contains(text)
                                       || e.epsgcode.Contains(text))
                                       select e;

                if (queryResults.Count() > 0)
                {
                    foreach (var item in queryResultsEpsg)
                    {
                        EPSG epsg = item;
                        registerItems.Add(epsg);
                    }

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
                    subregisters = subregisters,
                    replaces = register.replaces,
                    targetNamespace = register.targetNamespace,
                    versioning = register.versioning,
                    versioningId = register.versioningId,
                    versionNumber = register.versionNumber
                };
            }
            else { return register; }
        }

        public SearchResult Search(SearchParameters parameters)
        {
            string itemClass = "";

            if (parameters.Register != "Alle registre")
            {
                var queryResultsRegister = from o in _dbContext.Registers
                                           where o.name == parameters.Register || o.seoname == parameters.Register
                                           select o.containedItemClass;

                itemClass = queryResultsRegister.FirstOrDefault();
            }


            if (itemClass == "Organization")
            {
                var queryResults = (from o in _dbContext.Organizations
                                    where o.name.Contains(parameters.Text)
                                    || o.description.Contains(parameters.Text)
                                    || o.shortname.Contains(parameters.Text)
                                    select new SearchResultItem
                                    {
                                        RegisterName = o.register.name,
                                        RegisterDescription = o.register.description,
                                        RegisterItemName = o.name,
                                        RegisterItemDescription = o.description,
                                        RegisterID = o.registerId,
                                        SystemID = o.systemId,
                                        Discriminator = o.register.containedItemClass,
                                        RegisterSeoname = o.register.seoname,
                                        RegisterItemSeoname = o.seoname,
                                        DocumentOwner = null,
                                        RegisterItemUpdated = o.modified,
                                        RegisterItemStatus = o.statusId,
                                        Submitter = o.submitter.seoname,
                                        Shortname = o.shortname,
                                        currentVersion = o.versioning.currentVersion,
                                    });

                int NumFound = queryResults.Count();
                List<SearchResultItem> items = new List<SearchResultItem>();
                int skip = parameters.Offset;
                skip = skip - 1;
                queryResults = queryResults.OrderBy(ri => ri.RegisterItemName).Skip(skip).Take(parameters.Limit);

                foreach (SearchResultItem register in queryResults)
                {
                    var item = new SearchResultItem
                    {
                        RegisterName = register.RegisterName,
                        RegisterDescription = register.RegisterDescription,
                        RegisterItemName = register.RegisterItemName,
                        RegisterItemDescription = register.RegisterItemDescription,
                        RegisterID = register.RegisterID,
                        SystemID = register.SystemID,
                        Discriminator = register.Discriminator,
                        RegisterSeoname = register.RegisterSeoname,
                        RegisterItemSeoname = register.RegisterItemSeoname,
                        DocumentOwner = register.DocumentOwner,
                        RegisterItemUpdated = register.RegisterItemUpdated,
                        RegisterItemStatus = register.RegisterItemStatus,
                        RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.Submitter) + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
                        currentVersion = register.currentVersion,
                    };

                    items.Add(item);
                }

                return new SearchResult
                {
                    Items = items,
                    Limit = parameters.Limit,
                    Offset = parameters.Offset,
                    NumFound = NumFound
                };
            }

            else if (itemClass == "Document")
            {
                var queryResults = (from d in _dbContext.Documents
                                    where (d.register.name.Contains(parameters.Register) || d.register.seoname.Contains(parameters.Register))
                                    && (d.name.Contains(parameters.Text)
                                    || d.description.Contains(parameters.Text)
                                    || d.documentowner.name.Contains(parameters.Text))
                                    select new SearchResultItem
                                    {
                                        RegisterName = d.register.name,
                                        RegisterDescription = d.register.description,
                                        RegisterItemName = d.name,
                                        RegisterItemDescription = d.description,
                                        RegisterID = d.registerId,
                                        SystemID = d.systemId,
                                        Discriminator = d.register.containedItemClass,
                                        RegisterSeoname = d.register.seoname,
                                        RegisterItemSeoname = d.seoname,
                                        DocumentOwner = d.documentowner.seoname,
                                        RegisterItemUpdated = d.modified,
                                        RegisterItemStatus = d.statusId,
                                        Submitter = d.submitter.seoname,
                                        currentVersion = d.versioning.currentVersion,
                                    });

                int NumFound = queryResults.Count();
                List<SearchResultItem> items = new List<SearchResultItem>();
                int skip = parameters.Offset;
                skip = skip - 1;
                queryResults = queryResults.OrderBy(ri => ri.RegisterItemName).Skip(skip).Take(parameters.Limit);

                foreach (SearchResultItem register in queryResults)
                {
                    var item = new SearchResultItem
                    {
                        RegisterName = register.RegisterName,
                        RegisterDescription = register.RegisterDescription,
                        RegisterItemName = register.RegisterItemName,
                        RegisterItemDescription = register.RegisterItemDescription,
                        RegisterID = register.RegisterID,
                        SystemID = register.SystemID,
                        Discriminator = register.Discriminator,
                        RegisterSeoname = register.RegisterSeoname,
                        RegisterItemSeoname = register.RegisterItemSeoname,
                        DocumentOwner = register.DocumentOwner,
                        RegisterItemUpdated = register.RegisterItemUpdated,
                        RegisterItemStatus = register.RegisterItemStatus,
                        RegisteItemUrlDocument = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/versjoner/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DocumentOwner) + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
                        currentVersion = register.currentVersion,
                    };

                    items.Add(item);
                }

                return new SearchResult
                {
                    Items = items,
                    Limit = parameters.Limit,
                    Offset = parameters.Offset,
                    NumFound = NumFound
                };
            }

            else if (itemClass == "Dataset")
            {
                var queryResults = (from d in _dbContext.Datasets
                                    where (d.register.name.Contains(parameters.Register) || d.register.seoname.Contains(parameters.Register))
                                    && (d.name.Contains(parameters.Text)
                                    || d.description.Contains(parameters.Text)
                                    || d.datasetowner.name.Contains(parameters.Text))
                                    select new SearchResultItem
                                    {
                                        RegisterName = d.register.name,
                                        RegisterDescription = d.register.description,
                                        RegisterItemName = d.name,
                                        RegisterItemDescription = d.description,
                                        RegisterID = d.registerId,
                                        SystemID = d.systemId,
                                        Discriminator = d.register.containedItemClass,
                                        RegisterSeoname = d.register.seoname,
                                        RegisterItemSeoname = d.seoname,
                                        DocumentOwner = null,
                                        RegisterItemUpdated = d.modified,
                                        RegisterItemStatus = d.statusId,
                                        Submitter = d.submitter.seoname,
                                        DatasetOwner = d.datasetowner.seoname,
                                        currentVersion = d.versioning.currentVersion
                                    });

                int NumFound = queryResults.Count();
                List<SearchResultItem> items = new List<SearchResultItem>();
                int skip = parameters.Offset;
                skip = skip - 1;
                queryResults = queryResults.OrderBy(ri => ri.RegisterItemName).Skip(skip).Take(parameters.Limit);

                foreach (SearchResultItem register in queryResults)
                {
                    var item = new SearchResultItem
                    {
                        RegisterName = register.RegisterName,
                        RegisterDescription = register.RegisterDescription,
                        RegisterItemName = register.RegisterItemName,
                        RegisterItemDescription = register.RegisterItemDescription,
                        RegisterID = register.RegisterID,
                        SystemID = register.SystemID,
                        Discriminator = register.Discriminator,
                        RegisterSeoname = register.RegisterSeoname,
                        RegisterItemSeoname = register.RegisterItemSeoname,
                        DocumentOwner = register.DocumentOwner,
                        RegisterItemUpdated = register.RegisterItemUpdated,
                        RegisterItemStatus = register.RegisterItemStatus,
                        RegisteItemUrlDataset = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DatasetOwner) + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
                        DatasetOwner = register.DatasetOwner,
                        currentVersion = register.currentVersion
                    };

                    items.Add(item);
                }

                return new SearchResult
                {
                    Items = items,
                    Limit = parameters.Limit,
                    Offset = parameters.Offset,
                    NumFound = NumFound
                };
            }


            else if (itemClass == "EPSG")
            {
                var queryResults = (from e in _dbContext.EPSGs
                                    where e.name.Contains(parameters.Text)
                                    || e.description.Contains(parameters.Text)
                                    || e.epsgcode.Contains(parameters.Text)
                                    select new SearchResultItem
                                    {
                                        RegisterName = e.register.name,
                                        RegisterDescription = e.register.description,
                                        RegisterItemName = e.name,
                                        RegisterItemDescription = e.description,
                                        RegisterID = e.registerId,
                                        SystemID = e.systemId,
                                        Discriminator = e.register.containedItemClass,
                                        RegisterSeoname = e.register.seoname,
                                        RegisterItemSeoname = e.seoname,
                                        DocumentOwner = null,
                                        RegisterItemUpdated = e.modified,
                                        RegisterItemStatus = e.statusId,
                                        Submitter = e.submitter.seoname,
                                        currentVersion = e.versioning.currentVersion,
                                    });

                int NumFound = queryResults.Count();
                List<SearchResultItem> items = new List<SearchResultItem>();
                int skip = parameters.Offset;
                skip = skip - 1;
                queryResults = queryResults.OrderBy(ri => ri.RegisterItemName).Skip(skip).Take(parameters.Limit);

                foreach (SearchResultItem register in queryResults)
                {
                    var item = new SearchResultItem
                    {
                        RegisterName = register.RegisterName,
                        RegisterDescription = register.RegisterDescription,
                        RegisterItemName = register.RegisterItemName,
                        RegisterItemDescription = register.RegisterItemDescription,
                        RegisterID = register.RegisterID,
                        SystemID = register.SystemID,
                        Discriminator = register.Discriminator,
                        RegisterSeoname = register.RegisterSeoname,
                        RegisterItemSeoname = register.RegisterItemSeoname,
                        DocumentOwner = register.DocumentOwner,
                        RegisterItemUpdated = register.RegisterItemUpdated,
                        RegisterItemStatus = register.RegisterItemStatus,
                        RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.Submitter) + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
                        currentVersion = register.currentVersion,
                    };

                    items.Add(item);
                }

                return new SearchResult
                {
                    Items = items,
                    Limit = parameters.Limit,
                    Offset = parameters.Offset,
                    NumFound = NumFound
                };
            }

            else if (itemClass == "NameSpace")
            {
                var queryResults = (from e in _dbContext.NameSpases
                                    where e.name.Contains(parameters.Text)
                                    || e.description.Contains(parameters.Text)
                                    select new SearchResultItem
                                    {
                                        RegisterName = e.register.name,
                                        RegisterDescription = e.register.description,
                                        RegisterItemName = e.name,
                                        RegisterItemDescription = e.description,
                                        RegisterID = e.registerId,
                                        SystemID = e.systemId,
                                        Discriminator = e.register.containedItemClass,
                                        RegisterSeoname = e.register.seoname,
                                        RegisterItemSeoname = e.seoname,
                                        DocumentOwner = null,
                                        RegisterItemUpdated = e.modified,
                                        RegisterItemStatus = e.statusId,
                                        Submitter = e.submitter.seoname,
                                        currentVersion = e.versioning.currentVersion,
                                    });

                int NumFound = queryResults.Count();
                List<SearchResultItem> items = new List<SearchResultItem>();
                int skip = parameters.Offset;
                skip = skip - 1;
                queryResults = queryResults.OrderBy(ri => ri.RegisterItemName).Skip(skip).Take(parameters.Limit);

                foreach (SearchResultItem register in queryResults)
                {
                    var item = new SearchResultItem
                    {
                        RegisterName = register.RegisterName,
                        RegisterDescription = register.RegisterDescription,
                        RegisterItemName = register.RegisterItemName,
                        RegisterItemDescription = register.RegisterItemDescription,
                        RegisterID = register.RegisterID,
                        SystemID = register.SystemID,
                        Discriminator = register.Discriminator,
                        RegisterSeoname = register.RegisterSeoname,
                        RegisterItemSeoname = register.RegisterItemSeoname,
                        DocumentOwner = register.DocumentOwner,
                        RegisterItemUpdated = register.RegisterItemUpdated,
                        RegisterItemStatus = register.RegisterItemStatus,
                        RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.Submitter) + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
                        currentVersion = register.currentVersion,
                    };

                    items.Add(item);
                }
                return new SearchResult
                {
                    Items = items,
                    Limit = parameters.Limit,
                    Offset = parameters.Offset,
                    NumFound = NumFound
                };
            }

            else if (itemClass == "Register")
            {
                var queryResults =
                                    (from d in _dbContext.Registers
                                     where d.parentRegister.name == parameters.Register
                                     && d.parentRegister.containedItemClass == itemClass
                                     && (d.name.Contains(parameters.Text)
                                     || d.description.Contains(parameters.Text))
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = d.parentRegisterId,
                                         ParentRegisterName = d.parentRegister.name,
                                         ParentRegisterDescription = d.parentRegister.description,
                                         ParentRegisterSeoname = d.parentRegister.seoname,
                                         ParentregisterOwner = d.parentRegister.owner.seoname,
                                         RegisterName = d.name,
                                         RegisterDescription = d.description,
                                         RegisterItemName = null,
                                         RegisterItemDescription = null,
                                         RegisterID = d.systemId,
                                         SystemID = d.systemId,
                                         Discriminator = d.containedItemClass,
                                         RegisterSeoname = d.seoname,
                                         RegisterItemSeoname = null,
                                         DocumentOwner = null,
                                         DatasetOwner = null,
                                         RegisterItemUpdated = d.modified,
                                         RegisterItemStatus = d.statusId,
                                         Submitter = null,
                                         Shortname = null,
                                         CodelistValue = null,
                                         currentVersion = d.versioning.currentVersion,

                                     }).Union(
                                    (from o in _dbContext.Organizations
                                     where o.register.parentRegister.name.Contains(parameters.Register) && (
                                        o.register.name.Contains(parameters.Text)
                                     || o.register.description.Contains(parameters.Text)
                                     || o.register.name.Contains(parameters.Text)
                                     || o.name.Contains(parameters.Text)
                                     || o.description.Contains(parameters.Text))
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = o.register.parentRegisterId,
                                         ParentRegisterName = o.register.parentRegister.name,
                                         ParentRegisterDescription = o.register.parentRegister.description,
                                         ParentRegisterSeoname = o.register.parentRegister.seoname,
                                         ParentregisterOwner = o.register.parentRegister.owner.seoname,
                                         RegisterName = o.register.name,
                                         RegisterDescription = o.register.description,
                                         RegisterItemName = o.name,
                                         RegisterItemDescription = o.description,
                                         RegisterID = o.registerId,
                                         SystemID = o.systemId,
                                         Discriminator = o.register.containedItemClass,
                                         RegisterSeoname = o.register.seoname,
                                         RegisterItemSeoname = o.seoname,
                                         DocumentOwner = null,
                                         DatasetOwner = null,
                                         RegisterItemUpdated = o.modified,
                                         RegisterItemStatus = o.statusId,
                                         Submitter = o.submitter.seoname,
                                         Shortname = o.shortname,
                                         CodelistValue = null,
                                         currentVersion = o.versioning.currentVersion
                                     }).Union(
                                     (from o in _dbContext.NameSpases
                                     where o.register.parentRegister.name.Contains(parameters.Register) && (
                                        o.register.name.Contains(parameters.Text)
                                     || o.register.description.Contains(parameters.Text)
                                     || o.register.name.Contains(parameters.Text)
                                     || o.name.Contains(parameters.Text)
                                     || o.description.Contains(parameters.Text))
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = o.register.parentRegisterId,
                                         ParentRegisterName = o.register.parentRegister.name,
                                         ParentRegisterDescription = o.register.parentRegister.description,
                                         ParentRegisterSeoname = o.register.parentRegister.seoname,
                                         ParentregisterOwner = o.register.parentRegister.owner.seoname,
                                         RegisterName = o.register.name,
                                         RegisterDescription = o.register.description,
                                         RegisterItemName = o.name,
                                         RegisterItemDescription = o.description,
                                         RegisterID = o.registerId,
                                         SystemID = o.systemId,
                                         Discriminator = o.register.containedItemClass,
                                         RegisterSeoname = o.register.seoname,
                                         RegisterItemSeoname = o.seoname,
                                         DocumentOwner = null,
                                         DatasetOwner = null,
                                         RegisterItemUpdated = o.modified,
                                         RegisterItemStatus = o.statusId,
                                         Submitter = o.submitter.seoname,
                                         Shortname = null,
                                         CodelistValue = null,
                                         currentVersion = o.versioning.currentVersion
                                     }).Union(
                                   (from d in _dbContext.Documents
                                    where d.register.parentRegister.name.Contains(parameters.Register) && (
                                        d.register.name.Contains(parameters.Text)
                                    || d.name.Contains(parameters.Text)
                                    || d.description.Contains(parameters.Text)
                                    || d.documentowner.name.Contains(parameters.Text))
                                    select new SearchResultItem
                                    {
                                        ParentRegisterId = d.register.parentRegisterId,
                                        ParentRegisterName = d.register.parentRegister.name,
                                        ParentRegisterDescription = d.register.parentRegister.description,
                                        ParentRegisterSeoname = d.register.parentRegister.seoname,
                                        ParentregisterOwner = d.register.parentRegister.owner.seoname,
                                        RegisterName = d.register.name,
                                        RegisterDescription = d.register.description,
                                        RegisterItemName = d.name,
                                        RegisterItemDescription = d.description,
                                        RegisterID = d.registerId,
                                        SystemID = d.systemId,
                                        Discriminator = d.register.containedItemClass,
                                        RegisterSeoname = d.register.seoname,
                                        RegisterItemSeoname = d.seoname,
                                        DocumentOwner = d.documentowner.name,
                                        DatasetOwner = null,
                                        RegisterItemUpdated = d.modified,
                                        RegisterItemStatus = d.statusId,
                                        Submitter = d.submitter.seoname,
                                        Shortname = null,
                                        CodelistValue = null,
                                        currentVersion = d.versioning.currentVersion
                                    }).Union(
                                    (from d in _dbContext.Datasets
                                     where d.register.parentRegister.name.Contains(parameters.Register) && (
                                        d.register.name.Contains(parameters.Text)
                                     || d.name.Contains(parameters.Text)
                                     || d.description.Contains(parameters.Text)
                                     || d.datasetowner.name.Contains(parameters.Text))
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = d.register.parentRegisterId,
                                         ParentRegisterName = d.register.parentRegister.name,
                                         ParentRegisterDescription = d.register.parentRegister.description,
                                         ParentRegisterSeoname = d.register.parentRegister.seoname,
                                         ParentregisterOwner = d.register.parentRegister.owner.seoname,
                                         RegisterName = d.register.name,
                                         RegisterDescription = d.register.description,
                                         RegisterItemName = d.name,
                                         RegisterItemDescription = d.description,
                                         RegisterID = d.registerId,
                                         SystemID = d.systemId,
                                         Discriminator = d.register.containedItemClass,
                                         RegisterSeoname = d.register.seoname,
                                         RegisterItemSeoname = d.seoname,
                                         DocumentOwner = null,
                                         DatasetOwner = d.datasetowner.seoname,
                                         RegisterItemUpdated = d.modified,
                                         RegisterItemStatus = d.statusId,
                                         Submitter = d.submitter.seoname,
                                         Shortname = null,
                                         CodelistValue = null,
                                         currentVersion = d.versioning.currentVersion,
                                     }).Union(
                                    (from d in _dbContext.CodelistValues
                                     where d.register.parentRegister.name.Contains(parameters.Register) && (
                                        d.register.name.Contains(parameters.Text)
                                     || d.name.Contains(parameters.Text)
                                     || d.description.Contains(parameters.Text)
                                     || d.value.Contains(parameters.Text))
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = d.register.parentRegisterId,
                                         ParentRegisterName = d.register.parentRegister.name,
                                         ParentRegisterDescription = d.register.parentRegister.description,
                                         ParentRegisterSeoname = d.register.parentRegister.seoname,
                                         ParentregisterOwner = d.register.parentRegister.owner.seoname,
                                         RegisterName = d.register.name,
                                         RegisterDescription = d.register.description,
                                         RegisterItemName = d.name,
                                         RegisterItemDescription = d.description,
                                         RegisterID = d.registerId,
                                         SystemID = d.systemId,
                                         Discriminator = d.register.containedItemClass,
                                         RegisterSeoname = d.register.seoname,
                                         RegisterItemSeoname = d.seoname,
                                         DocumentOwner = null,
                                         DatasetOwner = null,
                                         RegisterItemUpdated = d.modified,
                                         RegisterItemStatus = d.statusId,
                                         Submitter = d.submitter.seoname,
                                         Shortname = null,
                                         CodelistValue = d.value,
                                         currentVersion = d.versioning.currentVersion
                                     }).Union(
                                    (from e in _dbContext.EPSGs
                                     where e.register.parentRegister.name.Contains(parameters.Register) && (
                                        e.register.name.Contains(parameters.Text)
                                    || e.name.Contains(parameters.Text)
                                    || e.description.Contains(parameters.Text)
                                    || e.epsgcode.Contains(parameters.Text))
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = e.register.parentRegisterId,
                                         ParentRegisterName = e.register.parentRegister.name,
                                         ParentRegisterDescription = e.register.parentRegister.description,
                                         ParentRegisterSeoname = e.register.parentRegister.seoname,
                                         ParentregisterOwner = e.register.parentRegister.owner.seoname,
                                         RegisterName = e.register.name,
                                         RegisterDescription = e.register.description,
                                         RegisterItemName = e.name,
                                         RegisterItemDescription = e.description,
                                         RegisterID = e.registerId,
                                         SystemID = e.systemId,
                                         Discriminator = e.register.containedItemClass,
                                         RegisterSeoname = e.register.seoname,
                                         RegisterItemSeoname = e.seoname,
                                         DocumentOwner = null,
                                         DatasetOwner = null,
                                         RegisterItemUpdated = e.modified,
                                         RegisterItemStatus = e.statusId,
                                         Submitter = e.submitter.seoname,
                                         Shortname = null,
                                         CodelistValue = null,
                                         currentVersion = e.versioning.currentVersion,
                                     })
                                  ))))));

                int NumFound = queryResults.Count();
                List<SearchResultItem> items = new List<SearchResultItem>();
                int skip = parameters.Offset;
                skip = skip - 1;
                queryResults = queryResults.OrderBy(ri => ri.RegisterItemName).Skip(skip).Take(parameters.Limit);

                foreach (SearchResultItem register in queryResults)
                {
                    var item = new SearchResultItem
                    {
                        ParentRegisterName = register.ParentRegisterName,
                        ParentRegisterId = register.ParentRegisterId,
                        ParentRegisterDescription = register.ParentRegisterDescription,
                        ParentRegisterSeoname = register.ParentRegisterSeoname,
                        ParentregisterOwner = register.ParentregisterOwner,
                        RegisterName = register.RegisterName,
                        RegisterDescription = register.RegisterDescription,
                        RegisterItemName = register.RegisterItemName,
                        RegisterItemDescription = register.RegisterItemDescription,
                        RegisterID = register.RegisterID,
                        SystemID = register.SystemID,
                        Discriminator = register.Discriminator,
                        RegisterSeoname = register.RegisterSeoname,
                        RegisterItemSeoname = register.RegisterItemSeoname,
                        DocumentOwner = register.DocumentOwner,
                        DatasetOwner = register.DatasetOwner,
                        RegisterItemUpdated = register.RegisterItemUpdated,
                        RegisterItemStatus = register.RegisterItemStatus,
                        SubregisterUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "subregister/" + register.ParentRegisterSeoname + "/" + register.ParentregisterOwner + "/" + register.RegisterSeoname,
                        RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.Submitter) + "/" + register.RegisterItemSeoname,
                        RegisteItemUrlDocument = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/versjoner/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DocumentOwner) + "/" + register.RegisterItemSeoname,
                        RegisteItemUrlDataset = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DatasetOwner) + "/" + register.RegisterItemSeoname,
                        subregisterItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "subregister/" + register.ParentRegisterSeoname + "/" + register.ParentregisterOwner + "/" + register.RegisterSeoname + "/" + register.Submitter + "/" + register.RegisterItemSeoname,
                        ParentRegisterUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.ParentRegisterSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
                        CodelistValue = register.CodelistValue,
                        currentVersion = register.currentVersion,

                    };

                    items.Add(item);
                }

                return new SearchResult
                {
                    Items = items,
                    Limit = parameters.Limit,
                    Offset = parameters.Offset,
                    NumFound = NumFound
                };
            }

            else
            {
                Guid systemIDSearch = new Guid();
                Guid.TryParse(parameters.Text, out systemIDSearch);

                var queryResult = (from d in _dbContext.Registers
                                   where d.parentRegister.containedItemClass == "Register"
                                   && (d.name.Contains(parameters.Text)
                                   || d.description.Contains(parameters.Text)
                                   || d.systemId.Equals(systemIDSearch))
                                   select new SearchResultItem
                                   {
                                       ParentRegisterId = d.parentRegisterId,
                                       ParentRegisterName = d.parentRegister.name,
                                       ParentRegisterDescription = d.parentRegister.description,
                                       ParentRegisterSeoname = d.parentRegister.seoname,
                                       ParentregisterOwner = d.parentRegister.owner.seoname,
                                       RegisterName = d.name,
                                       RegisterDescription = d.description,
                                       RegisterItemName = null,
                                       RegisterItemDescription = null,
                                       RegisterID = d.systemId,
                                       SystemID = d.systemId,
                                       Discriminator = d.containedItemClass,
                                       RegisterSeoname = d.seoname,
                                       RegisterItemSeoname = null,
                                       DocumentOwner = null,
                                       DatasetOwner = null,
                                       RegisterItemUpdated = d.modified,
                                       RegisterItemStatus = d.statusId,
                                       Submitter = null,
                                       Shortname = null,
                                       CodelistValue = null,
                                       ObjektkatalogUrl = null,
                                       Type = null,
                                       currentVersion = null

                                   }).Union(
                                    (from d in _dbContext.CodelistValues
                                     where d.register.name.Contains(parameters.Text)
                                     || d.name.Contains(parameters.Text)
                                     || d.description.Contains(parameters.Text)
                                     || d.value.Contains(parameters.Text)
                                     || d.systemId.Equals(systemIDSearch)
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = d.register.parentRegisterId,
                                         ParentRegisterName = d.register.parentRegister.name,
                                         ParentRegisterDescription = d.register.parentRegister.description,
                                         ParentRegisterSeoname = d.register.parentRegister.seoname,
                                         ParentregisterOwner = d.register.parentRegister.owner.seoname,
                                         RegisterName = d.register.name,
                                         RegisterDescription = d.register.description,
                                         RegisterItemName = d.name,
                                         RegisterItemDescription = d.description,
                                         RegisterID = d.registerId,
                                         SystemID = d.systemId,
                                         Discriminator = d.register.containedItemClass,
                                         RegisterSeoname = d.register.seoname,
                                         RegisterItemSeoname = d.seoname,
                                         DocumentOwner = null,
                                         DatasetOwner = null,
                                         RegisterItemUpdated = d.modified,
                                         RegisterItemStatus = d.statusId,
                                         Submitter = d.submitter.seoname,
                                         Shortname = null,
                                         CodelistValue = d.value,
                                         ObjektkatalogUrl = null,
                                         Type = null,
                                         currentVersion = d.versioning.currentVersion
                                     }).Union(
                                    (from o in _dbContext.Organizations
                                     where o.register.name.Contains(parameters.Text)
                                     || o.register.description.Contains(parameters.Text)
                                     || o.register.name.Contains(parameters.Text)
                                     || o.name.Contains(parameters.Text)
                                     || o.description.Contains(parameters.Text)
                                     || o.systemId.Equals(systemIDSearch)
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = o.register.parentRegisterId,
                                         ParentRegisterName = o.register.parentRegister.name,
                                         ParentRegisterDescription = o.register.parentRegister.description,
                                         ParentRegisterSeoname = o.register.parentRegister.seoname,
                                         ParentregisterOwner = o.register.parentRegister.owner.seoname,
                                         RegisterName = o.register.name,
                                         RegisterDescription = o.register.description,
                                         RegisterItemName = o.name,
                                         RegisterItemDescription = o.description,
                                         RegisterID = o.registerId,
                                         SystemID = o.systemId,
                                         Discriminator = o.register.containedItemClass,
                                         RegisterSeoname = o.register.seoname,
                                         RegisterItemSeoname = o.seoname,
                                         DocumentOwner = null,
                                         DatasetOwner = null,
                                         RegisterItemUpdated = o.modified,
                                         RegisterItemStatus = o.statusId,
                                         Submitter = o.submitter.seoname,
                                         Shortname = o.shortname,
                                         CodelistValue = null,
                                         ObjektkatalogUrl = null,
                                         Type = null,
                                         currentVersion = o.versioning.currentVersion
                                     }).Union(
                                     (from o in _dbContext.NameSpases
                                     where o.register.name.Contains(parameters.Text)
                                     || o.register.description.Contains(parameters.Text)
                                     || o.register.name.Contains(parameters.Text)
                                     || o.name.Contains(parameters.Text)
                                     || o.description.Contains(parameters.Text)
                                     || o.systemId.Equals(systemIDSearch)
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = o.register.parentRegisterId,
                                         ParentRegisterName = o.register.parentRegister.name,
                                         ParentRegisterDescription = o.register.parentRegister.description,
                                         ParentRegisterSeoname = o.register.parentRegister.seoname,
                                         ParentregisterOwner = o.register.parentRegister.owner.seoname,
                                         RegisterName = o.register.name,
                                         RegisterDescription = o.register.description,
                                         RegisterItemName = o.name,
                                         RegisterItemDescription = o.description,
                                         RegisterID = o.registerId,
                                         SystemID = o.systemId,
                                         Discriminator = o.register.containedItemClass,
                                         RegisterSeoname = o.register.seoname,
                                         RegisterItemSeoname = o.seoname,
                                         DocumentOwner = null,
                                         DatasetOwner = null,
                                         RegisterItemUpdated = o.modified,
                                         RegisterItemStatus = o.statusId,
                                         Submitter = o.submitter.seoname,
                                         Shortname = null,
                                         CodelistValue = null,
                                         ObjektkatalogUrl = null,
                                         Type = null,
                                         currentVersion = o.versioning.currentVersion
                                     }).Union(
                                   (from d in _dbContext.Documents
                                    where d.register.name.Contains(parameters.Text)
                                    || d.name.Contains(parameters.Text)
                                    || d.description.Contains(parameters.Text)
                                    || d.documentowner.name.Contains(parameters.Text)
                                    || d.systemId.Equals(systemIDSearch)
                                    select new SearchResultItem
                                    {
                                        ParentRegisterId = d.register.parentRegisterId,
                                        ParentRegisterName = d.register.parentRegister.name,
                                        ParentRegisterDescription = d.register.parentRegister.description,
                                        ParentRegisterSeoname = d.register.parentRegister.seoname,
                                        ParentregisterOwner = d.register.parentRegister.owner.seoname,
                                        RegisterName = d.register.name,
                                        RegisterDescription = d.register.description,
                                        RegisterItemName = d.name,
                                        RegisterItemDescription = d.description,
                                        RegisterID = d.registerId,
                                        SystemID = d.systemId,
                                        Discriminator = d.register.containedItemClass,
                                        RegisterSeoname = d.register.seoname,
                                        RegisterItemSeoname = d.seoname,
                                        DocumentOwner = d.documentowner.seoname,
                                        DatasetOwner = null,
                                        RegisterItemUpdated = d.modified,
                                        RegisterItemStatus = d.statusId,
                                        Submitter = d.submitter.seoname,
                                        Shortname = null,
                                        CodelistValue = null,
                                        ObjektkatalogUrl = null,
                                        Type = null,
                                        currentVersion = d.versioning.currentVersion
                                    }).Union(
                                    (from d in _dbContext.Datasets
                                     where d.register.name.Contains(parameters.Text)
                                     || d.name.Contains(parameters.Text)
                                     || d.description.Contains(parameters.Text)
                                     || d.datasetowner.name.Contains(parameters.Text)
                                     || d.systemId.Equals(systemIDSearch)
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = d.register.parentRegisterId,
                                         ParentRegisterName = d.register.parentRegister.name,
                                         ParentRegisterDescription = d.register.parentRegister.description,
                                         ParentRegisterSeoname = d.register.parentRegister.seoname,
                                         ParentregisterOwner = d.register.parentRegister.owner.seoname,
                                         RegisterName = d.register.name,
                                         RegisterDescription = d.register.description,
                                         RegisterItemName = d.name,
                                         RegisterItemDescription = d.description,
                                         RegisterID = d.registerId,
                                         SystemID = d.systemId,
                                         Discriminator = d.register.containedItemClass,
                                         RegisterSeoname = d.register.seoname,
                                         RegisterItemSeoname = d.seoname,
                                         DocumentOwner = null,
                                         DatasetOwner = d.datasetowner.seoname,
                                         RegisterItemUpdated = d.modified,
                                         RegisterItemStatus = d.statusId,
                                         Submitter = d.submitter.seoname,
                                         Shortname = null,
                                         CodelistValue = null,
                                         ObjektkatalogUrl = null,
                                         Type = null,
                                         currentVersion = d.versioning.currentVersion

                                     }).Union(
                                    (from e in _dbContext.EPSGs
                                     where e.register.name.Contains(parameters.Text)
                                    || e.name.Contains(parameters.Text)
                                    || e.description.Contains(parameters.Text)
                                    || e.epsgcode.Contains(parameters.Text)
                                    || e.systemId.Equals(systemIDSearch)
                                     select new SearchResultItem
                                     {
                                         ParentRegisterId = e.register.parentRegisterId,
                                         ParentRegisterName = e.register.parentRegister.name,
                                         ParentRegisterDescription = e.register.parentRegister.description,
                                         ParentRegisterSeoname = e.register.parentRegister.seoname,
                                         ParentregisterOwner = e.register.parentRegister.owner.seoname,
                                         RegisterName = e.register.name,
                                         RegisterDescription = e.register.description,
                                         RegisterItemName = e.name,
                                         RegisterItemDescription = e.description,
                                         RegisterID = e.registerId,
                                         SystemID = e.systemId,
                                         Discriminator = e.register.containedItemClass,
                                         RegisterSeoname = e.register.seoname,
                                         RegisterItemSeoname = e.seoname,
                                         DocumentOwner = null,
                                         DatasetOwner = null,
                                         RegisterItemUpdated = e.modified,
                                         RegisterItemStatus = e.statusId,
                                         Submitter = e.submitter.name,
                                         Shortname = null,
                                         CodelistValue = null,
                                         ObjektkatalogUrl = null,
                                         Type = null,
                                         currentVersion = e.versioning.currentVersion
                                     })
                                  ))))));


                var queryResultsList = queryResult.ToList();

                IQueryable<SearchResultItem> queryResultsListObjektKat = null;

                if (parameters.IncludeObjektkatalog)
                {
                    System.Net.WebClient c = new System.Net.WebClient();
                    c.Encoding = System.Text.Encoding.UTF8;
                    var data = c.DownloadString(System.Web.Configuration.WebConfigurationManager.AppSettings["ObjektkatalogUrl"] + "api/search/?text=" + parameters.Text + "&limit=1000");
                    var response = Newtonsoft.Json.Linq.JObject.Parse(data);

                    var objectKats = response["Results"];

                    List<SearchResultItem> objList = new List<SearchResultItem>();

                    foreach (var obj in objectKats)
                    {

                        objList.Add(new SearchResultItem
                        {
                            RegisterName = "Objektregister",
                            RegisterItemName = obj["name"] != null ? obj["name"].ToString() : null,
                            RegisterItemDescription = obj["description"] != null ? obj["description"].ToString() : null,
                            Discriminator = "Objektregister",
                            ObjektkatalogUrl = obj["url"] != null ? obj["url"].ToString() : null,
                            Type = obj["type"] != null ? obj["type"].ToString() : null
                        });

                    }

                    var qObjList = objList.AsQueryable();

                    queryResultsListObjektKat =
                    (from o in qObjList
                     select new SearchResultItem
                     {
                         ParentRegisterId = null,
                         ParentRegisterName = null,
                         ParentRegisterDescription = null,
                         ParentRegisterSeoname = null,
                         ParentregisterOwner = null,
                         RegisterName = o.RegisterName,
                         RegisterDescription = null,
                         RegisterItemName = o.RegisterItemName,
                         RegisterItemDescription = o.RegisterItemDescription,
                         RegisterID = new Guid(),
                         SystemID = new Guid(),
                         Discriminator = o.Discriminator,
                         RegisterSeoname = null,
                         RegisterItemSeoname = null,
                         DocumentOwner = null,
                         DatasetOwner = null,
                         RegisterItemUpdated = new System.DateTime(),
                         RegisterItemStatus = null,
                         Submitter = null,
                         Shortname = null,
                         CodelistValue = null,
                         ObjektkatalogUrl = o.ObjektkatalogUrl,
                         Type = o.Type,
                         currentVersion = o.currentVersion
                     }
                    );

                }

                var queryResultsListAll = queryResultsListObjektKat != null ? queryResultsList.Concat(queryResultsListObjektKat.ToList()).ToList() : queryResultsList.ToList();
                var queryResults = queryResultsListAll;

                int NumFound = queryResultsListAll.Count();
                List<SearchResultItem> items = new List<SearchResultItem>();
                int skip = parameters.Offset;
                skip = skip - 1;
                queryResults = queryResults.OrderBy(ri => ri.RegisterItemName).Skip(skip).Take(parameters.Limit).ToList();

                foreach (SearchResultItem register in queryResults)
                {
                    var item = new SearchResultItem
                    {
                        ParentRegisterName = register.ParentRegisterName,
                        ParentRegisterId = register.ParentRegisterId,
                        ParentRegisterDescription = register.ParentRegisterDescription,
                        ParentRegisterSeoname = register.ParentRegisterSeoname,
                        ParentregisterOwner = register.ParentregisterOwner,
                        RegisterName = register.RegisterName,
                        RegisterDescription = register.RegisterDescription,
                        RegisterItemName = register.RegisterItemName,
                        RegisterItemDescription = register.RegisterItemDescription,
                        RegisterID = register.RegisterID,
                        SystemID = register.SystemID,
                        Discriminator = register.Discriminator,
                        RegisterSeoname = register.RegisterSeoname,
                        RegisterItemSeoname = register.RegisterItemSeoname,
                        DocumentOwner = register.DocumentOwner,
                        RegisterItemUpdated = register.RegisterItemUpdated,
                        RegisterItemStatus = register.RegisterItemStatus,
                        SubregisterUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "subregister/" + register.ParentRegisterSeoname + "/" + register.ParentregisterOwner + "/" + register.RegisterSeoname,
                        RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.Submitter) + "/" + register.RegisterItemSeoname,
                        RegisteItemUrlDocument = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/versjoner/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DocumentOwner) + "/" + register.RegisterItemSeoname,
                        RegisteItemUrlDataset = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DatasetOwner) + "/" + register.RegisterItemSeoname,
                        subregisterItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "subregister/" + register.ParentRegisterSeoname + "/" + register.ParentregisterOwner + "/" + register.RegisterSeoname + "/" + register.Submitter + "/" + register.RegisterItemSeoname,
                        ParentRegisterUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.ParentRegisterSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
                        CodelistValue = register.CodelistValue,
                        ObjektkatalogUrl = register.ObjektkatalogUrl,
                        Type = register.Type,
                        currentVersion = register.currentVersion
                    };

                    items.Add(item);
                }

                return new SearchResult
                {
                    Items = items,
                    Limit = parameters.Limit,
                    Offset = parameters.Offset,
                    NumFound = NumFound
                };
            }

            //return new SearchResult();


        }

        private static IQueryable<Models.Filter> SortingQueryResults(SearchParameters parameters, IQueryable<Models.Filter> queryResults, int skip)
        {
            if (parameters.OrderBy != null)
            {
                if (parameters.OrderBy == "name_desc")
                {
                    queryResults = queryResults.OrderByDescending(d => d.name).Skip(skip).Take(parameters.Limit);
                }
                else if (parameters.OrderBy == "submitter")
                {
                    queryResults = queryResults.OrderBy(d => d.submitter.name).Skip(skip).Take(parameters.Limit);
                }
                else if (parameters.OrderBy == "submitter_desc")
                {
                    queryResults = queryResults.OrderByDescending(d => d.submitter.name).Skip(skip).Take(parameters.Limit);
                }
                else if (parameters.OrderBy == "status")
                {
                    queryResults = queryResults.OrderBy(d => d.status.description).Skip(skip).Take(parameters.Limit);
                }
                else if (parameters.OrderBy == "status_desc")
                {
                    queryResults = queryResults.OrderByDescending(d => d.status.description).Skip(skip).Take(parameters.Limit);
                }
                else if (parameters.OrderBy == "dateSubmitted")
                {
                    queryResults = queryResults.OrderBy(d => d.dateSubmitted).Skip(skip).Take(parameters.Limit);
                }
                else if (parameters.OrderBy == "dateSubmitted_desc")
                {
                    queryResults = queryResults.OrderByDescending(d => d.dateSubmitted).Skip(skip).Take(parameters.Limit);
                }
                else if (parameters.OrderBy == "modified")
                {
                    queryResults = queryResults.OrderBy(d => d.modified).Skip(skip).Take(parameters.Limit);
                }
                else if (parameters.OrderBy == "modified_desc")
                {
                    queryResults = queryResults.OrderByDescending(d => d.modified).Skip(skip).Take(parameters.Limit);
                }
                else if (parameters.OrderBy == "dateAccepted")
                {
                    queryResults = queryResults.OrderBy(d => d.dateAccepted).Skip(skip).Take(parameters.Limit);
                }
                else if (parameters.OrderBy == "dateAccepted_desc")
                {
                    queryResults = queryResults.OrderByDescending(d => d.dateAccepted).Skip(skip).Take(parameters.Limit);
                }
                else
                {
                    queryResults = queryResults.OrderBy(d => d.name).Skip(skip).Take(parameters.Limit);
                }
            }
            else
            {
                queryResults = queryResults.OrderBy(d => d.name).Skip(skip).Take(parameters.Limit);
            }
            return queryResults;
        }
    }
}