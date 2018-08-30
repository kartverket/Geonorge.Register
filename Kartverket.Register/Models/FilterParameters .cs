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
            StatusType = "all";
        }
        public string text { get; set; }
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
        public bool Compare { get; set; }
        public string DokSelectedTab { get; set; }
        public string SelectedReport { get; set; }
        public string StatusType { get; set; }

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