using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Kartverket.Register.Models
{
    public class InspireDataset : DatasetV2
    {
        //Inspire delivery statuses
        public string InspireTheme { get; set; }

        //Metadata
        [ForeignKey("InspireDeliveryMetadata"), Required, Display(Name = "Metadata:")]
        public Guid InspireDeliveryMetadataId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadata { get; set; }

        //Metadat service
        [ForeignKey("InspireDeliveryMetadataService"), Required, Display(Name = "Metadatatjeneste:")]
        public Guid InspireDeliveryMetadataServiceId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadataService { get; set; }

        //Distribution
        [ForeignKey("InspireDeliveryDistribution"), Required, Display(Name = "Deling av data:")]
        public Guid InspireDeliveryDistributionId { get; set; }
        public virtual DatasetDelivery InspireDeliveryDistribution { get; set; }

        //View service (Visningstjeneste)
        [ForeignKey("InspireDeliveryWms"), Required, Display(Name = "WMS:")]
        public Guid InspireDeliveryWmsId { get; set; }
        public virtual DatasetDelivery InspireDeliveryWms { get; set; }

        //WFS
        [ForeignKey("InspireDeliveryWfs"), Required, Display(Name = "Nedlastingstjeneste WFS:")]
        public Guid InspireDeliveryWfsId { get; set; }
        public virtual DatasetDelivery InspireDeliveryWfs { get; set; }

        //Atom-feed
        [ForeignKey("InspireDeliveryAtomFeed"), Required, Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public Guid InspireDeliveryAtomFeedId { get; set; }
        public virtual DatasetDelivery InspireDeliveryAtomFeed { get; set; }

        //Atom or WFS
        [ForeignKey("InspireDeliveryWfsOrAtom"), Required, Display(Name = "Nedlastingstjeneste WFS eller Atom-feed:")]
        public Guid InspireDeliveryWfsOrAtomId { get; set; }
        public virtual DatasetDelivery InspireDeliveryWfsOrAtom { get; set; }

        //Harmonized data
        [ForeignKey("InspireDeliveryHarmonizedData"), Required, Display(Name = "Harmoniserte data:")]
        public Guid InspireDeliveryHarmonizedDataId { get; set; }
        public virtual DatasetDelivery InspireDeliveryHarmonizedData { get; set; }

        //Spatial data service
        [ForeignKey("InspireDeliverySpatialDataService"), Required, Display(Name = "Spatial data service:")]
        public Guid InspireDeliverySpatialDataServiceId { get; set; }
        public virtual DatasetDelivery InspireDeliverySpatialDataService { get; set; }

        public bool HaveMetadata()
        {
            return InspireDeliveryMetadata.IsSet();
        }
    }

}//end namespace Datamodell