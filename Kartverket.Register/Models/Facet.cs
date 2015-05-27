using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class Facet
    {
        public string FacetField { get; set; }
        public List<FacetValue> FacetResults { get; set; }

        public class FacetValue
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }
    }
}