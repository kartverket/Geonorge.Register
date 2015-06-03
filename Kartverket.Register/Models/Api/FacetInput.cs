using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class FacetInput
    {
        /// <summary>
        /// The name of the facet
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The value of the facet
        /// </summary>
        public string value { get; set; }
    }
}