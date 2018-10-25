using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels.StatusReportViewModels
{
    public class GeodatalovLineChart
    {
        public List<string> Labels { get; set; }

        public List<int> Metadata { get; set; }
        public List<int> ProductSpecification { get; set; }
        public List<int> SosiRequirements { get; set; }
        public List<int> GmlRequirements { get; set; }
        public List<int> Wms { get; set; }
        public List<int> Wfs { get; set; }
        public List<int> AtomFeed { get; set; }
        public List<int> Common { get; set; }
        public List<int> InspireTheme { get; set; }
        public List<int> Dok { get; set; }
        public List<int> NationalDataset { get; set; }
        public List<int> Plan { get; set; }
        public List<int> Geodatalov { get; set; }
        

        public List<int> PointSize { get; set; }

        public GeodatalovLineChart(List<StatusReport> statusReports, StatusReport selectedStatusReport, string statusType = "Metadata")
        {
            Metadata = new List<int>();
            ProductSpecification = new List<int>();
            SosiRequirements = new List<int>();
            GmlRequirements = new List<int>();
            Wms = new List<int>();
            Wfs = new List<int>();
            AtomFeed = new List<int>();
            Common = new List<int>();
            Labels = new List<string>();
            PointSize = new List<int>();
            InspireTheme = new List<int>();
            Dok = new List<int>();
            NationalDataset = new List<int>();
            Plan = new List<int>();
            Geodatalov = new List<int>();

            if (statusReports != null)
            {
                foreach (var statusReport in statusReports)
                {
                    var xName = statusReport.Date.ToString("d MMMM yyyy",
                        CultureInfo.CreateSpecificCulture("nb-NO"));
                    PointSize.Add(
                        selectedStatusReport != null && statusReport.Id == selectedStatusReport.Id ? 7 : 3);

                    Labels.Add(xName);
                    Metadata.Add(statusReport.NumberOfGeodatalovDatasetsWithMetadata("good"));
                    ProductSpecification.Add(statusReport.NumberOfGeodatalovDatasetsWithProductSpecification("good"));
                    SosiRequirements.Add(statusReport.NumberOfGeodatalovDatasetsWithSosiRequirements("good"));
                    GmlRequirements.Add(statusReport.NumberOfGeodatalovDatasetsWithGmlRequirements("good"));
                    Wms.Add(statusReport.NumberOfGeodatalovDatasetsWithWms("good"));
                    Wfs.Add(statusReport.NumberOfGeodatalovDatasetsWithWfs("good"));
                    AtomFeed.Add(statusReport.NumberOfGeodatalovDatasetsWithAtomFeed("good"));
                    Common.Add(statusReport.NumberOfItemsWithCommon("good"));
                    InspireTheme.Add(statusReport.NumberOfGeodatalovDatasetsWithInspireTheme());
                    Dok.Add(statusReport.NumberOfGeodatalovDatasetsWithDok());
                    NationalDataset.Add(statusReport.NumberOfGeodatalovDatasetsWithNationalDataset());
                    Plan.Add(statusReport.NumberOfGeodatalovDatasetsWithPlan());
                    Geodatalov.Add(statusReport.NumberOfGeodatalovDatasetsWithGeodatalov());
                }
            }
        }
    }
}