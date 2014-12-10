using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using SearchParameters = Kartverket.Register.Models.SearchParameters;
using SearchResult = Kartverket.Register.Models.SearchResult;
using System.Data.Entity.Infrastructure;
using Kartverket.Register.Models;

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
            var queryResults = from r in _dbContext.Registers
                               join ri in _dbContext.RegisterItems
                               on r.systemId equals ri.registerId
                               where r.name.Contains(parameters.Text)
                               || r.description.Contains(parameters.Text)
                               || ri.name.Contains(parameters.Text)
                               || ri.description.Contains(parameters.Text)
                               orderby r.name
                               select new SearchResultItem { RegisterName = r.name, RegisterDescription= r.description, RegisterItemName=ri.name, RegisterItemDescription= ri.description, RegisterID = ri.registerId, SystemID = ri.systemId, Discriminator=r.containedItemClass };

            int NumFound = queryResults.Count();
            List<SearchResultItem> items = new List<SearchResultItem>();
            int skip = parameters.Offset;
            skip = skip - 1;
            queryResults = queryResults.Skip(skip).Take(parameters.Limit);

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
                   Discriminator = register.Discriminator

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