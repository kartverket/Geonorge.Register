using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class InspireDataserviceStatusReport : RegisterItemStatusReport
    {
        public InspireDataserviceStatusReport()
        {
        }

        public InspireDataserviceStatusReport(InspireDataService inspireDataservice)
        {
            //InspireDataservice = inspireDataservice;
            UuidInspireDataService = inspireDataservice.Uuid;
            MetadataInspireDataService = inspireDataservice.InspireDeliveryMetadata.StatusId;
            MetadataInSearchServiceInspireDataService = inspireDataservice.InspireDeliveryMetadataInSearchService.StatusId;
            ServiceStatusInspireDataService = inspireDataservice.InspireDeliveryServiceStatus.StatusId;
            Sds = inspireDataservice.IsSds();
            NetworkService = inspireDataservice.IsNetworkService();
        }

        //public virtual InspireDataService InspireDataservice { get; set; }
        public string UuidInspireDataService { get; set; }
        public string MetadataInspireDataService { get; set; }
        public string MetadataInSearchServiceInspireDataService { get; set; }
        public string ServiceStatusInspireDataService { get; set; }
        public Boolean Sds { get; set; }   
        public Boolean NetworkService { get; set; }   
    }
}