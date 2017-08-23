using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class InspireDataset : DatasetV2
    {
        //Inspire delivery statuses

        //Metadata
        [ForeignKey("InspireDeliveryMetadata")]
        [Required]
        public Guid InspireDeliveryMetadataId { get; set; }
        [Display(Name = "Metadata:")]
        public virtual DeliveryStatus InspireDeliveryMetadata { get; set; }

        //Metadat service
        [ForeignKey("InspireDeliveryMetadataService")]
        [Required]
        public Guid InspireDeliveryMetadataServiceId { get; set; }
        [Display(Name = "Metadatatjeneste:")]
        public virtual DeliveryStatus InspireDeliveryMetadataService { get; set; }

        //Distribution
        [ForeignKey("InspireDeliveryDistribution")]
        [Required]
        public Guid InspireDeliveryDistributionId { get; set; }
        [Display(Name = "Deling av data:")]
        public virtual DeliveryStatus InspireDeliveryDistribution { get; set; }

        //View service (Visningstjeneste)
        [ForeignKey("InspireDeliveryWms")]
        [Required]
        public Guid InspireDeliveryWmsId { get; set; }
        [Display(Name = "WMS:")]
        public virtual DeliveryStatus InspireDeliveryWms { get; set; }

        //WFS
        [ForeignKey("InspireDeliveryWfs")]
        [Required]
        public Guid InspireDeliveryWfsId { get; set; }
        [Display(Name = "Nedlastingstjeneste WFS:")]
        public virtual DeliveryStatus InspireDeliveryWfs { get; set; }

        //Atom-feed
        [ForeignKey("InspireDeliveryAtomFeed")]
        [Required]
        public Guid InspireDeliveryAtomFeedId { get; set; }
        [Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public DeliveryStatus InspireDeliveryAtomFeed { get; set; }

        //Atom or WFS
        [ForeignKey("InspireDeliveryWfsOrAtom")]
        [Required]
        public Guid InspireDeliveryWfsOrAtomId { get; set; }
        [Display(Name = "Nedlastingstjeneste WFS eller Atom-feed:")]
        public DeliveryStatus InspireDeliveryWfsOrAtom { get; set; }

        //Harmonized data
        [ForeignKey("InspireDeliveryHarmonizedData")]
        [Required]
        public Guid InspireDeliveryHarmonizedDataId { get; set; }
        [Display(Name = "Harmoniserte data:")]
        public DeliveryStatus InspireDeliveryHarmonizedData { get; set; }

        //Spatial data service
        [ForeignKey("InspireDeliverySpatialDataService")]
        [Required]
        public Guid InspireDeliverySpatialDataServiceId { get; set; }
        [Display(Name = "Spatial data service:")]
        public DeliveryStatus InspireDeliverySpatialDataService { get; set; }
    }

}//end namespace Datamodell