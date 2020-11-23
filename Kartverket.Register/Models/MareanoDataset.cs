using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Kartverket.Register.Models.FAIR;

namespace Kartverket.Register.Models
{
    public class MareanoDataset : DatasetV2
    {
        //FAIR delivery status

        //Findable
        [ForeignKey("FindableStatus"), Required, Display(Name = "Findable-status:")]
        public Guid FindableStatusId { get; set; }
        public double FindableStatusPerCent { get; set; }
        public virtual FAIRDelivery FindableStatus { get; set; }

        //Accesible 
        [ForeignKey("AccesibleStatus"), Required, Display(Name = "Accesible-status:")]
        public Guid AccesibleStatusId { get; set; }
        public double AccesibleStatusPerCent { get; set; }
        public virtual FAIRDelivery AccesibleStatus { get; set; }

        //Interoperable 
        [ForeignKey("InteroperableStatus"), Required, Display(Name = "Interoperable-status:")]
        public Guid InteroperableStatusId { get; set; }
        public double InteroperableStatusPerCent { get; set; }
        public virtual FAIRDelivery InteroperableStatus { get; set; }

        //ReUseable 
        [ForeignKey("ReUseableStatus"), Required, Display(Name = "Re-useable-status:")]
        public Guid ReUseableStatusId { get; set; }
        public double ReUseableStatusPerCent { get; set; }
        public virtual FAIRDelivery ReUseableStatus { get; set; }

        //Total FAIR 
        [ForeignKey("FAIRStatus"), Required, Display(Name = "FAIR-status:")]
        public Guid FAIRStatusId { get; set; }
        public double FAIRStatusPerCent { get; set; }
        public virtual FAIRDelivery FAIRStatus { get; set; }

        public virtual FAIRCriteria Criterias { get; set; }

        //Mareano delivery statuses

        //Metadata
        [ForeignKey("MetadataStatus"), Required, Display(Name = "Metadata:")]
        public Guid MetadataStatusId { get; set; }
        public virtual DatasetDelivery MetadataStatus { get; set; }

        //Produktspesifikasjon
        [ForeignKey("ProductSpesificationStatus"), Required, Display(Name = "Produktspesifikasjon:")]
        public Guid ProductSpesificationStatusId { get; set; }
        public virtual DatasetDelivery ProductSpesificationStatus { get; set; }

        //SOSI-data i hht nasjonal produkstspesifikasjon
        [ForeignKey("SosiDataStatus"), Required, Display(Name = "SOSI-data i hht nasjonal produkstspesifikasjon :")]
        public Guid SosiDataStatusId { get; set; }
        public virtual DatasetDelivery SosiDataStatus { get; set; }

        //GML-data i hht nasjonal produkstspesifikasjon
        [ForeignKey("GmlDataStatus"), Required, Display(Name = "GML-data i hht nasjonal produkstspesifikasjon :")]
        public Guid GmlDataStatusId { get; set; }
        public virtual DatasetDelivery GmlDataStatus { get; set; }

        //View service (Visningstjeneste)
        [ForeignKey("WmsStatus"), Required, Display(Name = "Visningstjeneste WMS/WMTS:")]
        public Guid WmsStatusId { get; set; }
        public virtual DatasetDelivery WmsStatus { get; set; }

        //WFS
        [ForeignKey("WfsStatus"), Required, Display(Name = "Nedlastingstjeneste WFS:")]
        public Guid WfsStatusId { get; set; }
        public virtual DatasetDelivery WfsStatus { get; set; }

        //Atom-feed
        [ForeignKey("AtomFeedStatus"), Required, Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public Guid AtomFeedStatusId { get; set; }
        public virtual DatasetDelivery AtomFeedStatus { get; set; }

        //Nedlastingstjeneste - felles/kombi
        [ForeignKey("CommonStatus"), Required, Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public Guid CommonStatusId { get; set; }
        public virtual DatasetDelivery CommonStatus { get; set; }


        public string DetailPageUrl()
        {
            return Register.GetObjectUrl() + "/" + Seoname + "/" + Uuid;
        }
    }
}