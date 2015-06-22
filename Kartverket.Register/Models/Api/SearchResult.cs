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
            Results = (searchResult.Items != null) ? RegisterData.CreateFromList(searchResult.Items) : null;
            Facets = (searchResult.Facets != null) ? Facet.CreateFromList(searchResult.Facets) : null;
        }
        /// <summary>
        /// Number of items found
        /// </summary>
        public int NumFound { get; set; }
        /// <summary>
        /// The maximum number of constraint counts that is returned
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// The offset into the list
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// Items in the searchresult
        /// </summary>
        public List<RegisterData> Results { get; set; }
        /// <summary>
        /// Result grouped by facets
        /// </summary>
        public List<Facet> Facets { get; set; }
    }
}