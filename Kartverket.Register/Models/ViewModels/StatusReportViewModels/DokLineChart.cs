using System.Collections.Generic;
using System.Globalization;

namespace Kartverket.Register.Models
{
    public class DokLineChart
    {
        public List<string> Labels { get; set; }

        //public List<DataPoint> Metadata { get; set; } 
        public List<int> Metadata { get; set; }
        public List<int> Productsheet { get; set; }
        public List<int> PresentationRules { get; set; }
        public List<int> ProductSpecification { get; set; }
        public List<int> Wms { get; set; }
        public List<int> Wfs { get; set; }
        public List<int> SosiRequirements { get; set; }
        public List<int> GmlRequirements { get; set; }
        public List<int> AtomFeed { get; set; }
        public List<int> Distribution { get; set; }

        public List<int> PointSize { get; set; }

        public DokLineChart(List<StatusReport> statusReports, StatusReport selectedStatusReport, string statusType = "Metadata")
        {
            Metadata = new List<int>();
            Productsheet = new List<int>();
            PresentationRules = new List<int>();
            ProductSpecification = new List<int>();
            Wms = new List<int>();
            Wfs = new List<int>();
            SosiRequirements = new List<int>();
            GmlRequirements = new List<int>();
            AtomFeed = new List<int>();
            Distribution = new List<int>();
            Labels = new List<string>();
            PointSize = new List<int>();

            if (statusReports != null)
            {
                foreach (var statusReport in statusReports)
                {
                        var xName = statusReport.Date.ToString("d MMMM yyyy",
                            CultureInfo.CreateSpecificCulture("nb-NO"));
                        PointSize.Add(
                            selectedStatusReport != null && statusReport.Id == selectedStatusReport.Id ? 7 : 3);

                        Labels.Add(xName);
                        Metadata.Add(statusReport.NumberOfItemsWithMetadata("good"));
                        Productsheet.Add(statusReport.NumberOfItemsWithProductsheet("good"));
                        PresentationRules.Add(statusReport.NumberOfItemsWithPresentationRules("good"));
                        ProductSpecification.Add(statusReport.NumberOfItemsWithProductSpecification("good"));
                        Wms.Add(statusReport.NumberOfItemsWithWms("good"));
                        Wfs.Add(statusReport.NumberOfItemsWithWfs("good"));
                        SosiRequirements.Add(statusReport.NumberOfItemsWithSosiRequirements("good"));
                        GmlRequirements.Add(statusReport.NumberOfItemsWithGmlRequirements("good"));
                        AtomFeed.Add(statusReport.NumberOfItemsWithAtomFeed("good"));
                        Distribution.Add(statusReport.NumberOfItemsWithDistribution("good"));
                }
            }
        }
    }
}