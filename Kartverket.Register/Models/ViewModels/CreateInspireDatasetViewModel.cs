using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class CreateInspireDatasetViewModel : CreateDatasetViewModel
    {
        public string InspireDeliveryMetadataStatus { get; set; }
        public string InspireDeliveryMetadataNote { get; set; }
        public string InspireDeliveryMetadataServiceStatus { get; set; }
        public string InspireDeliveryMetadataServiceNote { get; set; }
        public string InspireDeliveryDistributionStatus { get; set; }
        public string InspireDeliveryDistributionNote { get; set; }
        public string InspireDeliveryWmsStatus { get; set; }
        public string InspireDeliveryWmsNote { get; set; }
        public string InspireDeliveryWfsStatus { get; set; }
        public string InspireDeliveryWfsNote { get; set; }
        public string InspireDeliveryAtomFeedStatus { get; set; }
        public string InspireDeliveryAtomFeedNote { get; set; }
        public string InspireDeliveryWfsOrAtomStatus { get; set; }
        public string InspireDeliveryWfsOrAtomNote { get; set; }
        public string InspireDeliveryHarmonizedDataStatus { get; set; }
        public string InspireDeliveryHarmonizedDataNote { get; set; }
        public string InspireDeliverySpatialDataServiceStatus { get; set; }
        public string InspireDeliverySpatialDataServiceNote { get; set; }
    }
}