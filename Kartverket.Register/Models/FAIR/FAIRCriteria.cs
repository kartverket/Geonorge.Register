using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.FAIR
{
    public class FAIRCriteria
    {
        public string Id { get; set; }

        public bool F1 { get; set; } = true;
        public bool F2Keywords { get; set; }
        public bool F2Title { get; set; }
        public bool F2Description { get; set; }
        public bool F3DatasetId { get; set; }
        public bool F4 { get; set; } = true;

        public bool A1_1 { get; set; }
        public bool A1_2 { get; set; }
        public bool A2 { get; set; } = false;

        public bool I1Metadata { get; set; }
        public bool I1Dataset { get; set; }
        public bool I2NationalTheme { get; set; }
        public bool I2TopicCategory { get; set; }
        public bool I3Concepts { get; set; }
        public bool I3ApplicationSchemaInformation { get; set; }


        public bool R1_1 { get; set; }
        public bool R1_2ProcessHistory { get; set; }
        public bool R1_2MaintenanceAndUpdateFrequency { get; set; }
        public bool R1_2ProductSpesification { get; set; }
        public bool R1_2ResolutionScale { get; set; }
        public bool R1_2CoverageMap { get; set; }
        public bool R1_2CoverageMapComplete { get; set; }

        public bool R1_3Standards { get; set; }
        public bool R1_3OpenFormats { get; set; }
    }
}