using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class SearchResult
    {
        public int NumFound { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public List<SearchResultItem> Items { get; set; }
        public List<Facet> Facets { get; set; }
        //public List<Filter> ItemsFilter { get; set; }


        public SearchResult()
        {

        }

        public SearchResult(SearchResult otherResult)
        {
            Items = otherResult.Items;
            //ItemsFilter = otherResult.ItemsFilter;
            NumFound = otherResult.NumFound;
            Limit = otherResult.Limit;
            Offset = otherResult.Offset;
        }
    }
}