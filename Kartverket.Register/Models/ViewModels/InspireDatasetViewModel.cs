using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireDatasetViewModel : DatasetViewModel
    {
        [Display(Name = "Metadata:")]
        public string InspireDeliveryMetadataStatus { get; set; }
        public string InspireDeliveryMetadataNote { get; set; }

        [Display(Name = "Metadatatjeneste:")]
        public string InspireDeliveryMetadataServiceStatus { get; set; }
        public string InspireDeliveryMetadataServiceNote { get; set; }

        [Display(Name = "Deling av data:")]
        public string InspireDeliveryDistributionStatus { get; set; }
        public string InspireDeliveryDistributionNote { get; set; }

        [Display(Name = "WMS:")]
        public string InspireDeliveryWmsStatus { get; set; }
        public string InspireDeliveryWmsNote { get; set; }

        [Display(Name = "Nedlastingstjeneste WFS:")]
        public string InspireDeliveryWfsStatus { get; set; }
        public string InspireDeliveryWfsNote { get; set; }

        [Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public string InspireDeliveryAtomFeedStatus { get; set; }
        public string InspireDeliveryAtomFeedNote { get; set; }

        [Display(Name = "Nedlastingstjeneste WFS eller Atom-feed:")]
        public string InspireDeliveryWfsOrAtomStatus { get; set; }
        public string InspireDeliveryWfsOrAtomNote { get; set; }

        [Display(Name = "Harmoniserte data:")]
        public string InspireDeliveryHarmonizedDataStatus { get; set; }
        public string InspireDeliveryHarmonizedDataNote { get; set; }

        [Display(Name = "Spatial data service:")]
        public string InspireDeliverySpatialDataServiceStatus { get; set; }
        public string InspireDeliverySpatialDataServiceNote { get; set; }
    }
}