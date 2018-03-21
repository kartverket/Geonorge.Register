using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Kartverket.Register.Models
{
    public class InspireDataService : RegisterItemV2
    {
        public string ServiceType { get; set; } // WMS, WFS, REST, Atom feed, metadata --> protocol

        [ForeignKey("InspireDeliveryMetadata"), Required, Display(Name = "Metadata:")]
        public Guid InspireDeliveryMetadataId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadata { get; set; } // Finnes = Brukbar, Valid metadata = God. Status fra editor.

        [ForeignKey("InspireDeliveryMetadataInSearchService"), Required, Display(Name = "Metadatatjeneste:")]
        public Guid InspireDeliveryMetadataInSearchServiceId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadataInSearchService { get; set; } // Godkjent p� alle

        [ForeignKey("InspireDeliveryServiceStatus"), Required, Display(Name = "Metadatatjeneste:")]
        public Guid InspireDeliveryServiceStatusId { get; set; }
        public virtual DatasetDelivery InspireDeliveryServiceStatus { get; set; } // Tjenestestatus for WMS/WFS
        public int Requests { get; set; } // Manuelt
        public string Url { get; set; } // Til tjenesten, finnes i metadataene
        public string Theme { get; set; } // Liste opp alle Annex tjenestene h�rer til..

        [Display(Name = "Uuid:")]
        public string Uuid { get; set; }

        public bool GetSds()
        {
            return ServiceType != "WFS-tjeneste" && ServiceType != "WMS-tjeneste";
        }

        public bool GetNetworkService()
        {
            return ServiceType == "WFS-tjeneste" || ServiceType == "WMS-tjeneste";
        }

    }

}