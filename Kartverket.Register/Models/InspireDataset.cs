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
        [Display(Name = "Metadata:")]
        public Guid InspireDeliveryMetadataId { get; set; }
        public virtual DeliveryStatus InspireDeliveryMetadata { get; set; }

        //Metadat service
        [ForeignKey("InspireDeliveryMetadataService")]
        [Required]
        [Display(Name = "Metadatatjeneste:")]
        public Guid InspireDeliveryMetadataServiceId { get; set; }
        public virtual DeliveryStatus InspireDeliveryMetadataService { get; set; }

        //Distribution
        [ForeignKey("InspireDeliveryDistribution")]
        [Required]
        [Display(Name = "Deling av data:")]
        public Guid InspireDeliveryDistributionId { get; set; }
        public virtual DeliveryStatus InspireDeliveryDistribution { get; set; }

        //View service (Visningstjeneste)
        [ForeignKey("InspireDeliveryWms")]
        [Required]
        [Display(Name = "WMS:")]
        public Guid InspireDeliveryWmsId { get; set; }
        public virtual DeliveryStatus InspireDeliveryWms { get; set; }

        //WFS
        [ForeignKey("InspireDeliveryWfs")]
        [Required]
        [Display(Name = "Nedlastingstjeneste WFS:")]
        public Guid InspireDeliveryWfsId { get; set; }
        public virtual DeliveryStatus InspireDeliveryWfs { get; set; }

        //Atom-feed
        [ForeignKey("InspireDeliveryAtomFeed")]
        [Required]
        [Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public Guid InspireDeliveryAtomFeedId { get; set; }
        public DeliveryStatus InspireDeliveryAtomFeed { get; set; }

        //Atom or WFS
        [ForeignKey("InspireDeliveryWfsOrAtom")]
        [Required]
        [Display(Name = "Nedlastingstjeneste WFS eller Atom-feed:")]
        public Guid InspireDeliveryWfsOrAtomId { get; set; }
        public DeliveryStatus InspireDeliveryWfsOrAtom { get; set; }

        //Harmonized data
        [ForeignKey("InspireDeliveryHarmonizedData")]
        [Required]
        [Display(Name = "Harmoniserte data:")]
        public Guid InspireDeliveryHarmonizedDataId { get; set; }
        public DeliveryStatus InspireDeliveryHarmonizedData { get; set; }

        //Spatial data service
        [ForeignKey("InspireDeliverySpatialDataService")]
        [Required]
        [Display(Name = "Spatial data service:")]
        public Guid InspireDeliverySpatialDataServiceId { get; set; }
        public DeliveryStatus InspireDeliverySpatialDataService { get; set; }

        public string LogoSrc()
        {
            if (Owner != null) return "~/data/organizations/" + Owner.logoFilename;
            return "";
        }

        public string GetDescriptionAsSubstring()
        {
            if(!string.IsNullOrWhiteSpace(Description))
            {
                if (Description.Length < 80)
                {
                    return Description.Substring(0, Description.Length);
                }
                else
                {
                    return Description.Substring(0, 80) + "...";
                }
            }
            else
            {
                return "";
            }
        }
    }

}//end namespace Datamodell