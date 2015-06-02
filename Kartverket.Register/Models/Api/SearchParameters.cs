using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class SearchParameters
    {
        /// <summary>
        /// Fritekst søkestrengen
        /// </summary>
        public string text { get; set; }
       
        public int offset { get; set; } 
        public int limit { get; set; } 
        public List<FacetInput> facets { get; set; }
        public string orderby { get; set; }

        public SearchParameters()
        {
            facets = new List<FacetInput>();
            
            limit = 10;
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