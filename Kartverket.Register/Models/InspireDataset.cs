using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class InspireDataset : DatasetNy
    {
        //Inspire delivery statuses

        //Metadata
        [ForeignKey("InspireDeliveryMetadataStatus")]
        [Display(Name = "Metadata:")]
        public string InspireDeliveryMetadataStatusId { get; set; }
        public virtual DokDeliveryStatus InspireDeliveryMetadataStatus { get; set; }
        public string InspireDeliveryMetadataStatusNote { get; set; }
        public bool InspireDeliveryMetadataStatusAutoUpdate { get; set; } = true;

        //Metadat service
        [ForeignKey("InspireDeliveryMetadataServiceStatus")]
        [Display(Name = "Metadatatjeneste:")]
        public string InspireDeliveryMetadataServiceStatusId { get; set; }
        public virtual DokDeliveryStatus InspireDeliveryMetadataServiceStatus { get; set; }
        public string InspireDeliveryMetadataServiceStatusNote { get; set; }
        public bool InspireDeliveryMetadataServiceStatusAutoUpdate { get; set; } = true;

        //Distribution
        [ForeignKey("InspireDeliveryDistributionStatus")]
        [Display(Name = "Deling av data:")]
        public string InspireDeliveryDistributionStatusId { get; set; }
        public virtual DokDeliveryStatus InspireDeliveryDistributionStatus { get; set; }
        public string InspireDeliveryDistributionStatusNote { get; set; }
        public bool InspireDeliveryDistributionStatusAutoUpdate { get; set; } = true;

        //View service (Visningstjeneste)
        [ForeignKey("InspireDeliveryWmsStatus")]
        [Display(Name = "WMS:")]
        public string InspireDeliveryWmsStatusId { get; set; }
        public virtual DokDeliveryStatus InspireDeliveryWmsStatus { get; set; }
        public string InspireDeliveryWmsStatusNote { get; set; }
        public bool InspireDeliveryWmsStatusAutoUpdate { get; set; } = true;

        //WFS
        [ForeignKey("InspireDeliveryWfsStatus")]
        [Display(Name = "Nedlastingstjeneste WFS:")]
        public string InspireDeliveryWfsStatusId { get; set; }
        public virtual DokDeliveryStatus InspireDeliveryWfsStatus { get; set; }
        public string InspireDeliveryWfsStatusNote { get; set; }
        public bool InspireDeliveryWfsStatusAutoUpdate { get; set; } = true;

        //Atom-feed
        [ForeignKey("InspireDeliveryAtomFeedStatus")]
        [Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public string InspireDeliveryAtomFeedStatusId { get; set; }
        public virtual DokDeliveryStatus InspireDeliveryAtomFeedStatus { get; set; }
        public string InspireDeliveryAtomFeedStatusNote { get; set; }
        public bool InspireDeliveryAtomFeedStatusAutoUpdate { get; set; } = true;

        //Atom or WFS
        [ForeignKey("InspireDeliveryWfsOrAtomStatus")]
        [Display(Name = "Nedlastingstjeneste WFS eller Atom-feed:")]
        public string InspireDeliveryWfsOrAtomStatusId { get; set; }
        public virtual DokDeliveryStatus InspireDeliveryWfsOrAtomStatus { get; set; }
        public string InspireDeliveryWfsOrAtomStatusNote { get; set; }
        public bool InspireDeliveryWfsOrAtomStatusAutoUpdate { get; set; } = true;

        //Harmonized data
        [ForeignKey("InspireDeliveryHarmonizedDataStatus")]
        [Display(Name = "Harmoniserte data:")]
        public string InspireDeliveryHarmonizedDataStatusId { get; set; }
        public virtual DokDeliveryStatus InspireDeliveryHarmonizedDataStatus { get; set; }
        public string InspireDeliveryHarmonizedDataStatusNote { get; set; }
        public bool InspireDeliveryHarmonizedDataStatusAutoUpdate { get; set; } = true;

        //Spatial data service
        [ForeignKey("InspireDeliverySpatialDataServiceStatus")]
        [Display(Name = "Spatial data service:")]
        public string InspireDeliverySpatialDataServiceStatusId { get; set; }
        public virtual DokDeliveryStatus InspireDeliverySpatialDataServiceStatus { get; set; }
        public string InspireDeliverySpatialDataServiceStatusNote { get; set; }
        public bool InspireDeliverySpatialDataServiceStatusAutoUpdate { get; set; } = true;
    }

}//end namespace Datamodell