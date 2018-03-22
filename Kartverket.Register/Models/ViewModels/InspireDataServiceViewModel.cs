using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireDataServiceViewModel : RegisterItemV2ViewModel
    {
        public string ServiceType { get; set; } // WMS, WFS, REST, Atom feed, metadata --> protocol

        public Guid InspireDeliveryMetadataId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadata { get; set; } // Finnes = Brukbar, Valid metadata = God. Status fra editor.

        public Guid InspireDeliveryMetadataInSearchServiceId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadataInSearchService { get; set; } // Godkjent på alle

        public Guid InspireDeliveryServiceStatusId { get; set; }
        public virtual DatasetDelivery InspireDeliveryServiceStatus { get; set; } // Tjenestestatus for WMS/WFS

        public int Requests { get; set; } // Manuelt
        public bool NetworkService { get; set; } // view og download = true
        public bool Sds { get; set; } // tjenester som ikke er WMS eller WFS
        public string Url { get; set; } // Til tjenesten, finnes i metadataene
        public string Theme { get; set; } // Liste opp alle Annex tjenestene hører til..



        public InspireDataServiceViewModel(InspireDataService item)
        {
            Update(item);
        }

        public InspireDataServiceViewModel(ICollection<InspireDataService> collection)
        {
        }

        public void Update(InspireDataService inspireDataService)
        {
            if (inspireDataService != null)
            {
                if (inspireDataService.InspireDeliveryMetadata != null)
                {
                    InspireDeliveryMetadata = inspireDataService.InspireDeliveryMetadata;
                    InspireDeliveryMetadataId = inspireDataService.InspireDeliveryMetadataId;
                }
                if (inspireDataService.InspireDeliveryMetadataInSearchService != null)
                {
                    InspireDeliveryMetadataInSearchService = inspireDataService.InspireDeliveryMetadataInSearchService;
                    InspireDeliveryMetadataInSearchServiceId = inspireDataService.InspireDeliveryMetadataInSearchServiceId;
                }
                if (inspireDataService.InspireDeliveryServiceStatus != null)
                {
                    InspireDeliveryServiceStatus = inspireDataService.InspireDeliveryServiceStatus;
                    InspireDeliveryServiceStatusId = inspireDataService.InspireDeliveryServiceStatusId;
                }

                ServiceType = inspireDataService.ServiceType;
                Requests = inspireDataService.Requests;
                NetworkService = inspireDataService.NetworkService;
                Sds = inspireDataService.GetSds();
                Url = inspireDataService.Url;
                Theme = inspireDataService.Theme;

                UpdateRegisterItem(inspireDataService);
            }
        }

    }
}