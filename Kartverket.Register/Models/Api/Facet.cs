using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class Facet
    {
        public string FacetField { get; set; }
        public List<FacetValue> FacetResults { get; set; }

        private Facet(Models.Facet item)
        {
            FacetField = item.FacetField;
            FacetResults = FacetValue.CreateFromList(item.FacetResults);
        }

        public static List<Facet> CreateFromList(IEnumerable<Models.Facet> facets)
        {
            return facets.Select(item => new Facet(item)).ToList();
        }

        public class FacetValue
        {
            
            public string Name { get; set; }
            public int Count { get; set; }

            private FacetValue(Models.Facet.FacetValue item)
            {
                Name = item.Name;
                Count = item.Count;
            }

            public static List<FacetValue> CreateFromList(IEnumerable<Models.Facet.FacetValue> facetResults)
            {
                return facetResults.Select(item => new FacetValue(item)).ToList();
            }
        }

    }
}