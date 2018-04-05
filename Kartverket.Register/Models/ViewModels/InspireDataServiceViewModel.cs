using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Configuration;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireDataServiceViewModel : RegisterItemV2ViewModel
    {
        public string InspireDataType { get; set; } // WMS, WFS, REST, Atom feed, metadata --> protocol

        [Display(Name = "Metadata:")]
        public Guid InspireDeliveryMetadataId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadata { get; set; } // Finnes = Brukbar, Valid metadata = God. Status fra editor.

        [Display(Name = "Metadata i søketjenesten:")]
        public Guid InspireDeliveryMetadataInSearchServiceId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadataInSearchService { get; set; } // Godkjent på alle

        [Display(Name = "Tjenestestatus")]
        public Guid InspireDeliveryServiceStatusId { get; set; }
        public virtual DatasetDelivery InspireDeliveryServiceStatus { get; set; } // Tjenestestatus for WMS/WFS

        [Display(Name = "Requests")]
        public int Requests { get; set; } // Manuelt

        [Display(Name = "Nettverkstjeneste")]
        public bool NetworkService { get; set; } // view og download = true

        [Display(Name = "Sds")]
        public bool Sds { get; set; } // tjenester som ikke er WMS eller WFS

        [Display(Name = "Url")]
        public string Url { get; set; } // Til tjenesten, finnes i metadataene

        [Display(Name = "Tema")]
        public string Theme { get; set; } // Liste opp alle Annex tjenestene hører til..

        [Display(Name = "Metadata url")]
        public string MetadataUrl { get; set; }

        [Display(Name = "UUid")]
        public string Uuid { get; set; }


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

                InspireDataType = inspireDataService.InspireDataType;
                Requests = inspireDataService.Requests;
                NetworkService = inspireDataService.IsNetworkService();
                Sds = inspireDataService.GetSds();
                Url = inspireDataService.Url;
                Theme = inspireDataService.Theme;
                Uuid = inspireDataService.Uuid;
                MetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + Uuid;
                UpdateRegisterItem(inspireDataService);
            }
        }

        public string GetInspireDataServiceEditUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/inspire-data-service/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
            }
            return "/inspire-data-service/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";

        }
    }
}