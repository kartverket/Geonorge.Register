using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class SearchResult
    {
        public SearchResult(Models.SearchResult searchResult)
        {
            Limit = searchResult.Limit;
            Offset = searchResult.Offset;
            NumFound = searchResult.NumFound;
            Results = RegisterData.CreateFromList(searchResult.Items);
            Facets = Facet.CreateFromList(searchResult.Facets);
        }

        public int NumFound { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public List<RegisterData> Results { get; set; }
        public List<Facet> Facets { get; set; }
    }
}