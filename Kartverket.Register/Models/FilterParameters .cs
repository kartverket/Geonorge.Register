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
        public string filterVertikalt { get; set; }
        public string filterHorisontalt { get; set; }
        public string InspireRequirement { get; set; }
        public string nationalRequirement { get; set; }
        public string nationalSeaRequirement { get; set; }
        


    }
}