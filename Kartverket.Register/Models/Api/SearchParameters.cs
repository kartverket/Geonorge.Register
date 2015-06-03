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
        /// <summary>
        /// Gå til valgt posisjon i resultatet. Default er 1.
        /// </summary>
        public int offset { get; set; }
        /// <summary>
        /// Begrenser hvor mange treff som returneres. Default er 100.
        /// </summary>
        public int limit { get; set; }
        /// <summary>
        /// Begrenser resultat på fasett. Returnerte fasetter er er "theme" , "type", "organization".
        /// </summary>
        public List<FacetInput> facets { get; set; }
        /// <summary>
        /// Verdier for sortering er: "name", "date_updated", "score". Dersom ingen kriterier er oppgitt sorteres den på "name", ellers er default score
        /// </summary>
        public string orderby { get; set; }

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