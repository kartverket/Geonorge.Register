using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Kartverket.Register.Models
{
    public class GeodatalovDataset : DatasetV2
    {
        public bool InspireTheme { get; set; }
        public bool Dok { get; set; }
        public bool NationalDataset { get; set; }
        public bool Plan { get; set; }
        public bool Geodatalov { get; set; }
        public bool Mareano { get; set; }
        public bool EcologicalBaseMap { get; set; }

        //Geodatalov delivery statuses

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