using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class FilterParameters
    {
        public FilterParameters()
        {
            
        }
        public string text { get; set; }
        public bool filterVertikalt { get; set; }
        public bool filterHorisontalt { get; set; }
        public string InspireRequirement { get; set; }
        public string nationalRequirement { get; set; }
        public string nationalSeaRequirement { get; set; }
        public string filterOrganization { get; set; }
        


    }
}