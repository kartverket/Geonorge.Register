using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Resources;

namespace Kartverket.Register.Models
{
    public class InspireDataService : RegisterItemV2
    {
        public string InspireDataType { get; set; } // WMS, WFS, REST, Atom feed, metadata --> protocol

        [ForeignKey("InspireDeliveryMetadata"), Required, Display(Name = "Metadata", ResourceType = typeof(InspireDataSet))]
        public Guid InspireDeliveryMetadataId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadata { get; set; } // Finnes = Brukbar, Valid metadata = God. Status fra editor.

        [ForeignKey("InspireDeliveryMetadataInSearchService"), Required, Display(Name = "MetadataInSearchService", ResourceType = typeof(InspireDataSet))]
        public Guid InspireDeliveryMetadataInSearchServiceId { get; set; }
        public virtual DatasetDelivery InspireDeliveryMetadataInSearchService { get; set; } // Godkjent p� alle

        [ForeignKey("InspireDeliveryServiceStatus"), Required, Display(Name = "ServiceStatus", ResourceType = typeof(InspireDataSet))]
        public Guid InspireDeliveryServiceStatusId { get; set; }
        public virtual DatasetDelivery InspireDeliveryServiceStatus { get; set; } // Tjenestestatus for WMS/WFS

        public int Requests { get; set; } // Manuelt
        public string Url { get; set; } // Til tjenesten, finnes i metadataene

        [Display(Name = "InspireTheme", ResourceType = typeof(InspireDataSet))]
        public virtual ICollection<CodelistValue> InspireThemes { get; set; } /// Liste opp alle Annex tjenestene h�rer til..

        public string ServiceType { get; set; } 
        public string Uuid { get; set; }

        public bool IsSds()
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


        internal bool MetadataIsGoodOrDeficent()
        {
            if (InspireDeliveryMetadata != null)
            {
                return InspireDeliveryMetadata.IsSet();
            }
            return false;
        }

        public void UpdateInspireTheme(ICollection<CodelistValue> inspireThemes)
        {
            RemoveInspireTheme(inspireThemes);
            AddToList(inspireThemes);
        }

        private void AddToList(ICollection<CodelistValue> inspireThemesUpdated)
        {
            foreach (var inspireTheme in inspireThemesUpdated)
            {
                if (!InspireThemes.Any(i => i.systemId == inspireTheme.systemId))
                {
                    InspireThemes.Add(inspireTheme);
                }
            }
        }

        private void RemoveInspireTheme(ICollection<CodelistValue> inspireThemesToUpdate)
        {
            var exists = false;
            var removeDatasets = new List<CodelistValue>();

            foreach (var inspireTheme in InspireThemes)
            {
                if (inspireThemesToUpdate.Any(i => i.systemId == inspireTheme.systemId))
                {
                    exists = true;
                }
                if (!exists)
                {
                    removeDatasets.Add(inspireTheme);
                }
                exists = false;
            }
            foreach (var inspireTheme in removeDatasets)
            {
                InspireThemes.Remove(inspireTheme);
            }
        }

        internal bool MetadataIsGood()
        {
            if (InspireDeliveryMetadata != null)
            {
                return InspireDeliveryMetadata.IsGood();
            }
            return false;
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

        internal bool ServiceStatusIsGood()
        {
            if (InspireDeliveryServiceStatus != null)
            {
                return InspireDeliveryServiceStatus.IsGood();
            }
            return false;
        }

        public void GetServiceType(string serviceType)
        {
            ServiceType = serviceType;
            if (serviceType == "other")
            {
                ServiceType = "invoke";
            }
        }
    }

}