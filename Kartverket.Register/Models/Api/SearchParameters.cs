using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class SearchParameters
    {
        public string text { get; set; }
       
        public int offset { get; set; } 
        public int limit { get; set; } 
        public List<FacetParameter> facets { get; set; }

        public SearchParameters()
        {
            facets = new List<FacetParameter>();
            
            limit = 10;
            offset = 1;
        } 
    }

    
}