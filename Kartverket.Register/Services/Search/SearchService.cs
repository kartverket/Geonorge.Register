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
                               from ri in _dbContext.RegisterItems
                               where r.name == parameters.Text
                               || r.description == parameters.Text
                               || ri.name == parameters.Text
                               || ri.description == parameters.Text
                               orderby r.name
                               select new SearchResultItem { Name =ri.name, Description=ri.description };

            int NumFound = queryResults.Count();
            List<SearchResultItem> items = new List<SearchResultItem>();
            int skip = parameters.Offset;
            skip = skip - 1;
            queryResults = queryResults.Skip(skip).Take(parameters.Limit);

            foreach (SearchResultItem register in queryResults)
            {
                var item = new SearchResultItem
                {
                    Name = register.Name,
                    Description = register.Description

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