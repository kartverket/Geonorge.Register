using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class SearchParameters
    {
        public SearchParameters()
        {
            Offset = 1;
            Limit = 50;
            Facets = new List<FacetParameter>();
        }

        public string Owner { get; set; }
        public string Register { get; set; }
        public string Text { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string OrderBy { get; set; }
        public List<FacetParameter> Facets { get; set; }

        bool objektkatalog = true;

        public bool IncludeObjektkatalog
        {
            get { return this.objektkatalog; }
            set { this.objektkatalog = value; }
        }

        public void AddDefaultFacetsIfMissing()
        {
            AddDefaultFacetsIfMissing(new List<string>());
        }

        public void AddDefaultFacetsIfMissing(List<string> additionalFacets)
        {
            var defaultFacets = new List<string> { "theme" , "type", "organization" };

            if (additionalFacets.Any())
            {
                defaultFacets.AddRange(additionalFacets);
            }

            foreach (var defaultFacet in defaultFacets)
            {
                if (Facets.All(f => f.Name != defaultFacet))
                {
                    Facets.Add(new FacetParameter { Name = defaultFacet });
                }
            }
        }
    }
}