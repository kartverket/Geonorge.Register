using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class DatasetStatusHistory : StatusHistory
    {
        public string Metadata { get; set; }
        public string ProductSheet { get; set; }
        public string PresentationRules { get; set; }
        public string ProductSpecification { get; set; }
        public string Wms { get; set; }
        public string Wfs { get; set; }
        public string Distribution { get; set; }
        public string SosiRequirements { get; set; }
        public string GmlRequirements { get; set; }
        public string AtomFeed { get; set; }
    }
}