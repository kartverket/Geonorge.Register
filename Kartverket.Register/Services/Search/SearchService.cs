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
                                        Submitter = o.submitter.name,
                                        Shortname = o.shortname,
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
                        RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "/register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DocumentOwner) + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
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
                                        DocumentOwner = null,
                                        RegisterItemUpdated = d.modified,
                                        RegisterItemStatus = d.statusId,
                                        Submitter = d.submitter.name,
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
                        RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "/register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DocumentOwner) + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname
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
                                        Submitter = d.submitter.name,
                                        DatasetOwner = d.datasetowner.name
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
                        RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "/register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DocumentOwner) + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
                        DatasetOwner = register.DatasetOwner
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
                                        Submitter = e.submitter.name,
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
                        RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "/register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DocumentOwner) + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname
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
                                        RegisterItemUpdated = d.modified,
                                        RegisterItemStatus = d.statusId,
                                        Submitter = null,
                                        Shortname = null,
                                        CodelistValue = null

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
                                         RegisterItemUpdated = o.modified, 
                                         RegisterItemStatus = o.statusId, 
                                         Submitter = o.submitter.name, 
                                         Shortname = o.shortname,
                                         CodelistValue = null
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
                                        RegisterItemUpdated = d.modified, 
                                        RegisterItemStatus = d.statusId, 
                                        Submitter = d.submitter.name, 
                                        Shortname = null,
                                        CodelistValue = null
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
                                         DocumentOwner = d.datasetowner.name, 
                                         RegisterItemUpdated = d.modified, 
                                         RegisterItemStatus = d.statusId, 
                                         Submitter = d.submitter.name, 
                                         Shortname = null,
                                         CodelistValue = null
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
                                         RegisterItemUpdated = d.modified, 
                                         RegisterItemStatus = d.statusId, 
                                         Submitter = d.submitter.name, 
                                         Shortname = null,
                                         CodelistValue = d.value
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
                                         RegisterItemUpdated = e.modified, 
                                         RegisterItemStatus = e.statusId, 
                                         Submitter = e.submitter.name, 
                                         Shortname = null,
                                         CodelistValue = null
                                     })
                                  )))));

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
                        RegisterItemUpdated = register.RegisterItemUpdated,
                        RegisterItemStatus = register.RegisterItemStatus,
                        SubregisterUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "/subregister/" + register.ParentRegisterSeoname + "/" + register.ParentregisterOwner + "/" + register.RegisterName,
                        //RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "/register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DocumentOwner) + "/" + register.RegisterItemSeoname,
                        subregisterItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "/subregister/" + register.ParentRegisterSeoname + "/" + register.ParentregisterOwner + "/" + register.RegisterItemSeoname + "/" + register.Submitter + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
                        CodelistValue = register.CodelistValue
                        
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
                var queryResults = (from d in _dbContext.Registers
                                    where d.parentRegister.containedItemClass == "Register"
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
                                        RegisterItemUpdated = d.modified,
                                        RegisterItemStatus = d.statusId,
                                        Submitter = null,
                                        Shortname = null,
                                        CodelistValue = null

                                    }).Union(
                                    (from d in _dbContext.CodelistValues
                                     where d.register.name.Contains(parameters.Text)
                                     || d.name.Contains(parameters.Text)
                                     || d.description.Contains(parameters.Text)
                                     || d.value.Contains(parameters.Text)
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
                                         RegisterItemUpdated = d.modified,
                                         RegisterItemStatus = d.statusId,
                                         Submitter = d.submitter.name,
                                         Shortname = null,
                                         CodelistValue = d.value
                                    }).Union(
                                    (from o in _dbContext.Organizations
                                    where o.register.name.Contains(parameters.Text)
                                    || o.register.description.Contains(parameters.Text)
                                    || o.register.name.Contains(parameters.Text)
                                    || o.name.Contains(parameters.Text)
                                    || o.description.Contains(parameters.Text)
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
                                         RegisterItemUpdated = o.modified,
                                         RegisterItemStatus = o.statusId,
                                         Submitter = o.submitter.name,
                                         Shortname = o.shortname,
                                         CodelistValue = null
                                    }).Union(
                                   (from d in _dbContext.Documents
                                    where d.register.name.Contains(parameters.Text)
                                    || d.name.Contains(parameters.Text)
                                    || d.description.Contains(parameters.Text)
                                    || d.documentowner.name.Contains(parameters.Text)
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
                                        RegisterItemUpdated = d.modified,
                                        RegisterItemStatus = d.statusId,
                                        Submitter = d.submitter.name,
                                        Shortname = null,
                                        CodelistValue = null
                                    }).Union(
                                    (from d in _dbContext.Datasets
                                     where d.register.name.Contains(parameters.Text)
                                     || d.name.Contains(parameters.Text)
                                     || d.description.Contains(parameters.Text)
                                     || d.datasetowner.name.Contains(parameters.Text)
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
                                         DocumentOwner = d.datasetowner.name,
                                         RegisterItemUpdated = d.modified,
                                         RegisterItemStatus = d.statusId,
                                         Submitter = d.submitter.name,
                                         Shortname = null,
                                         CodelistValue = null
                                     }).Union(
                                    (from e in _dbContext.EPSGs
                                     where e.register.name.Contains(parameters.Text)
                                    || e.name.Contains(parameters.Text)
                                    || e.description.Contains(parameters.Text)
                                    || e.epsgcode.Contains(parameters.Text)
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
                                         RegisterItemUpdated = e.modified,
                                         RegisterItemStatus = e.statusId,
                                         Submitter = e.submitter.name,
                                         Shortname = null,
                                         CodelistValue = null
                                     })
                                  )))));

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
                        RegisterItemUpdated = register.RegisterItemUpdated,
                        RegisterItemStatus = register.RegisterItemStatus,
                        SubregisterUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "subregister/" + register.ParentRegisterSeoname + "/" + register.ParentregisterOwner + "/" + register.RegisterSeoname,
                        RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DocumentOwner) + "/" + register.RegisterItemSeoname,
                        subregisterItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "subregister/" + register.ParentRegisterSeoname + "/" + register.ParentregisterOwner + "/" + register.RegisterItemSeoname + "/" + register.Submitter + "/" + register.RegisterItemSeoname,
                        Submitter = register.Submitter,
                        Shortname = register.Shortname,
                        CodelistValue = register.CodelistValue
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
    }
}