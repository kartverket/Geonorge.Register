using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Kartverket.Register.Models
{
    public class InspireDataService : RegisterItemV2
    {
        public string InspireDataType { get; set; } // WMS, WFS, REST, Atom feed, metadata --> protocol

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
        public string ServiceType { get; set; } 
        public string Uuid { get; set; }
        [Display(Name = "Areal")]
        public int Area { get; set; }
        [Display(Name = "Relevant areal")]
        public int RelevantArea { get; set; }

        public bool Conformity { get; set; }

        public bool GetSds()
        {
            return InspireDataType != "WFS-tjeneste" && InspireDataType != "WMS-tjeneste";
        }

        public bool IsNetworkService() {
            return ServiceType == "view" || ServiceType == "download";
        }

        public bool IsView()
        {
            return ServiceType == "view";
        }

        public bool IsDownload()
        {
            return ServiceType == "download";
        }

        public bool HaveMetadata()
        {
            return InspireDeliveryMetadata.IsSet();
        }
    }

}