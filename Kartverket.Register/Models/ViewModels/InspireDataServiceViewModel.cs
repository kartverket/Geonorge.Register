using Resources;
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
        public string MetadataStatusId { get; set; }
        public string MetadataStatus { get; set; } // Finnes = Brukbar, Valid metadata = God. Status fra editor.
        public string MetadataNote { get; set; }
        public bool MetadataAutoUpdate { get; set; }


        [Display(Name = "MetadataInSearchService", ResourceType = typeof(InspireDataSet))]
        public Guid InspireDeliveryMetadataInSearchServiceId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadataInSearchService { get; set; } // Godkjent på alle
        public string MetadataInSearchServiceStatusId { get; set; }
        public string MetadataInSearchServiceStatus { get; set; }
        public string MetadataInSearchServiceNote { get; set; }
        public bool MetadataInSearchAutoUpdate { get; set; }

        [Display(Name = "ServiceStatus", ResourceType = typeof(InspireDataSet))]
        public Guid InspireDeliveryServiceStatusId { get; set; }
        public virtual DatasetDelivery InspireDeliveryServiceStatus { get; set; } // Tjenestestatus for WMS/WFS
        public string ServiceStatusId { get; set; }
        public string ServiceStatus { get; set; } 
        public string ServiceStatusNote { get; set; }
        public bool ServiceStatusAutoUpdate { get; set; }

        [Display(Name = "Requests")]
        public int Requests { get; set; } // Manuelt

        [Display(Name = "Nettverkstjeneste")]
        public bool NetworkService { get; set; } // view og download = true

        [Display(Name = "Sds")]
        public bool Sds { get; set; } // tjenester som ikke er WMS eller WFS

        [Display(Name = "Url")]
        public string Url { get; set; } // Til tjenesten, finnes i metadataene

        [Display(Name = "InspireTheme", ResourceType = typeof(InspireDataSet))]
        public ICollection<CodelistValue> InspireThemes { get; set; }

        [Display(Name = "Metadata url")]
        public string MetadataUrl { get; set; }

        [Display(Name = "UUid")]
        public string Uuid { get; set; }

        public string ServiceType { get; set; }


        public InspireDataServiceViewModel(InspireDataService item)
        {
            Update(item);
        }

        public InspireDataServiceViewModel()
        {
        }

        public void Update(InspireDataService inspireDataService)
        {
            if (inspireDataService != null)
            {
                if (inspireDataService.InspireDeliveryMetadata != null)
                {
                    MetadataAutoUpdate = inspireDataService.InspireDeliveryMetadata.AutoUpdate;
                    MetadataNote = inspireDataService.InspireDeliveryMetadata.Note;
                    MetadataStatusId = inspireDataService.InspireDeliveryMetadata.Status.value;
                    MetadataStatus = inspireDataService.InspireDeliveryMetadata.Status.DescriptionTranslated();
                    InspireDeliveryMetadataId = inspireDataService.InspireDeliveryMetadataId;
                }
                if (inspireDataService.InspireDeliveryMetadataInSearchService != null)
                {
                    //InspireDeliveryMetadataInSearchService = inspireDataService.InspireDeliveryMetadataInSearchService;
                    InspireDeliveryMetadataInSearchServiceId = inspireDataService.InspireDeliveryMetadataInSearchServiceId;
                    MetadataInSearchAutoUpdate = inspireDataService.InspireDeliveryMetadataInSearchService.AutoUpdate;
                    MetadataInSearchServiceNote = inspireDataService.InspireDeliveryMetadataInSearchService.Note;
                    MetadataInSearchServiceStatusId = inspireDataService.InspireDeliveryMetadataInSearchService.Status.value;
                    MetadataInSearchServiceStatus = inspireDataService.InspireDeliveryMetadataInSearchService.Status.DescriptionTranslated();
                }
                    if (inspireDataService.InspireDeliveryServiceStatus != null)
                {
                    //InspireDeliveryServiceStatus = inspireDataService.InspireDeliveryServiceStatus;
                    InspireDeliveryServiceStatusId = inspireDataService.InspireDeliveryServiceStatusId;
                    ServiceStatusAutoUpdate = inspireDataService.InspireDeliveryServiceStatus.AutoUpdate;
                    ServiceStatusNote = inspireDataService.InspireDeliveryServiceStatus.Note;
                    ServiceStatusId = inspireDataService.InspireDeliveryServiceStatus.Status.value;
                    ServiceStatus = inspireDataService.InspireDeliveryServiceStatus.Status.DescriptionTranslated();
                }

                InspireDataType = inspireDataService.InspireDataType;
                Requests = inspireDataService.Requests;
                NetworkService = inspireDataService.IsNetworkService();
                Sds = inspireDataService.IsSds();
                Url = inspireDataService.Url;
                InspireThemes = inspireDataService.InspireThemes;
                Uuid = inspireDataService.Uuid;
                MetadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "metadata/uuid/" + Uuid;
                ServiceType = inspireDataService.ServiceType;

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

        public string InspireThemsAsString()
        {
            string inspireTeamsString = null;
            foreach (var item in InspireThemes)
            {
                if (inspireTeamsString == null)
                {
                    inspireTeamsString += item.name;
                }
                else
                {
                    inspireTeamsString += ", " + item.name;
                }
            }
            return inspireTeamsString;
        }

        public string GetInspireDataServiceDeleteUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/inspire-data-service/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
            }
            return "/inspire-data-service/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
        }
    }
}