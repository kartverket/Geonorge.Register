using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class SearchParameters
    {
        /// <summary>
        /// The text to search for
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// This param indicates an offset into the list of constraints to allow paging. Default is 1.
        /// </summary>
        public int offset { get; set; }
        /// <summary>
        /// This param indicates the maximum number of constraint counts that should be returned. Default is 100.
        /// </summary>
        public int limit { get; set; }
        /// <summary>
        /// Limits the list to facets. Ex. <![CDATA[ &facets[0]name=organization&facets[0]value=Kartverket]]>. Facets result is grouped by  "theme" , "type", "organization".
        /// </summary>
        public List<FacetInput> facets { get; set; }
        /// <summary>
        /// Values to order by is: "name", "date_updated", "score". No search criteria is ordered by "name", else default is "score"
        /// </summary>
        public string orderby { get; set; }
        public bool excludecodelistvalues { get; set; }

        public SearchParameters()
        {
            facets = new List<FacetInput>();
            
            limit = 100;
            offset = 1;
        }

        public void AddDefaultFacetsIfMissing()
        {
            AddDefaultFacetsIfMissing(new List<string>());
        }

        public void AddDefaultFacetsIfMissing(List<string> additionalFacets)
        {
            var defaultFacets = new List<string> { "theme", "Type" };

            if (additionalFacets.Any())
            {
                defaultFacets.AddRange(additionalFacets);
            }

            foreach (var defaultFacet in defaultFacets)
            {
                if (facets.All(f => f.name != defaultFacet))
                {
                    facets.Add(new FacetInput { name = defaultFacet });
                }
            }
        }

    }

    
}