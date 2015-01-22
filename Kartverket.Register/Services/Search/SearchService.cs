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
            var queryResults = (from o in _dbContext.Organizations
                                where o.register.name.Contains(parameters.Text)
                                || o.register.description.Contains(parameters.Text)
                                || o.register.name.Contains(parameters.Text)
                                || o.name.Contains(parameters.Text)
                                || o.description.Contains(parameters.Text)
                                select new SearchResultItem { RegisterName = o.register.name, RegisterDescription = o.register.description, RegisterItemName = o.name, RegisterItemDescription = o.description, RegisterID = o.registerId, SystemID = o.systemId, Discriminator = o.register.containedItemClass, RegisterSeoname = o.register.seoname, RegisterItemSeoname = o.seoname, DocumentOwner = null, RegisterItemUpdated = o.modified, RegisterItemStatus = o.statusId }).Union(
                               (from d in _dbContext.Documents
                                where d.register.name.Contains(parameters.Text)
                                || d.name.Contains(parameters.Text)
                                || d.description.Contains(parameters.Text)
                                || d.documentowner.name.Contains(parameters.Text)
                                select new SearchResultItem { RegisterName = d.register.name, RegisterDescription = d.register.description, RegisterItemName = d.name, RegisterItemDescription = d.description, RegisterID = d.registerId, SystemID = d.systemId, Discriminator = d.register.containedItemClass, RegisterSeoname = d.register.seoname, RegisterItemSeoname = d.seoname, DocumentOwner = d.documentowner.name, RegisterItemUpdated = d.modified, RegisterItemStatus = d.statusId }).Union(
                                (from d in _dbContext.Datasets
                                where d.register.name.Contains(parameters.Text)
                                || d.name.Contains(parameters.Text)
                                || d.description.Contains(parameters.Text)
                                || d.datasetowner.name.Contains(parameters.Text)
                                 select new SearchResultItem { RegisterName = d.register.name, RegisterDescription = d.register.description, RegisterItemName = d.name, RegisterItemDescription = d.description, RegisterID = d.registerId, SystemID = d.systemId, Discriminator = d.register.containedItemClass, RegisterSeoname = d.register.seoname, RegisterItemSeoname = d.seoname, DocumentOwner = d.datasetowner.name, RegisterItemUpdated = d.modified, RegisterItemStatus = d.statusId }).Union(
                                (from e in _dbContext.EPSGs
                                 where e.register.name.Contains(parameters.Text)
                                || e.name.Contains(parameters.Text)
                                || e.description.Contains(parameters.Text)
                                || e.epsgcode.Contains(parameters.Text)
                                 select new SearchResultItem { RegisterName = e.register.name, RegisterDescription = e.register.description, RegisterItemName = e.name, RegisterItemDescription = e.description, RegisterID = e.registerId, SystemID = e.systemId, Discriminator = e.register.containedItemClass, RegisterSeoname = e.register.seoname, RegisterItemSeoname = e.seoname, DocumentOwner = null, RegisterItemUpdated = e.modified, RegisterItemStatus = e.statusId })
                               )));



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
                    RegisteItemUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "/register/" + register.RegisterSeoname + "/" + HtmlHelperExtensions.MakeSeoFriendlyString(register.DocumentOwner) + "/" + register.RegisterItemSeoname

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

    }
}