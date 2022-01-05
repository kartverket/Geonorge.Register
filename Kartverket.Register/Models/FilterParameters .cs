using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class FilterParameters
    {
        private string Dataset = "dataset";
        private string Service = "service";
        private string InspireReport = "inspirereport";

        public FilterParameters()
        {
            InspireRegisteryType = Dataset;
            InspireDatasetStatusType = "all";
            InspireDataServiceStatusType = "all";
            StatusType = "all";
        }

        public int Offset { get; set; }
        public int Limit { get; set; }
        public string OrderBy { get; set; } = string.Empty;
        public string text { get; set; }
        public string Category { get; set; }
        public bool filterVertikalt { get; set; }
        public bool filterHorisontalt { get; set; }
        public string InspireRequirement { get; set; }
        public string nationalRequirement { get; set; }
        public string nationalSeaRequirement { get; set; }
        public string filterOrganization { get; set; }
        public string municipality { get; set; }
        public string InspireRegisteryType { get; set; }
        public bool ShowCurrentInspireMonitoringReport { get; set; }
        public string SelectedInspireMonitoringReport { get; set; }
        public string SelectedComparableCandidate { get; set; }
        public string SelectedOrganizationType { get; set; }
        public bool Compare { get; set; }
        public string DokSelectedTab { get; set; }
        public string GeodatalovSelectedTab { get; set; }
        public string MareanoSelectedTab { get; set; }
        public string GeodataType { get; set; }
        public string SelectedReport { get; set; }
        public string StatusType { get; set; }
        public string InspireDatasetStatusType { get; set; }
        public string InspireDataServiceStatusType { get; set; }
        public string InspireAnnex { get; set; }
        public string filterTheme { get; set; }
        public string[] tag { get; set; }
        public string[] department { get; set; }
        public string stationName { get; set; }
        public string stationType { get; set; }
        public DateTime? effectivedate_from { get; set; }

    public bool InspireRegistertTypeIsDataset()
        {
            return InspireRegisteryType == Dataset;
        }

        public bool InspireRegistertTypeIsService()
        {
            return InspireRegisteryType == Service;
        }

        public bool InspireRegisteryTypeIsisInspireReport()
        {
            return InspireRegisteryType == InspireReport;
        }
    }
}