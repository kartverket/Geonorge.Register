using System.Collections.Generic;
using System.Globalization;

namespace Kartverket.Register.Models.ViewModels.StatusReportViewModels
{
    public class InspireDatasetLineChart
    {
        public List<string> Labels { get; set; }

        public List<int> Metadata { get; set; }
        public List<int> MetadataService { get; set; }
        public List<int> Distribution { get; set; }
        public List<int> Wms { get; set; }
        public List<int> Wfs { get; set; }
        public List<int> WfsOrAtomFeed { get; set; }
        public List<int> AtomFeed { get; set; }
        public List<int> HarmonizedData { get; set; }
        public List<int> SpatialDataService { get; set; }

        public List<int> PointSize { get; set; }

        public InspireDatasetLineChart(List<StatusReport> statusReports, StatusReport selectedStatusReport)
        {
            Metadata = new List<int>();
            MetadataService = new List<int>();
            Distribution = new List<int>();
            Wms = new List<int>();
            Wfs = new List<int>();
            WfsOrAtomFeed = new List<int>();
            AtomFeed = new List<int>();
            HarmonizedData = new List<int>();
            SpatialDataService = new List<int>();

            Labels = new List<string>();
            PointSize = new List<int>();

            if (statusReports != null)
            {
                foreach (var statusReport in statusReports)
                {
                    var xName = statusReport.Date.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("nb-NO"));
                    PointSize.Add(selectedStatusReport != null && statusReport.Id == selectedStatusReport.Id ? 7 : 3);
                    Labels.Add(xName);

                    Metadata.Add(statusReport.NumberOfInspireDatasetsWithMetadata("good"));
                    MetadataService.Add(statusReport.NumberOfItemsWithMetadataService("good"));
                    Distribution.Add(statusReport.NumberOfInspireDatasetsWithDistribution("good"));
                    Wms.Add(statusReport.NumberOfInspireDatasetsWithWms("good"));
                    Wfs.Add(statusReport.NumberOfInspireDatasetsWithWfs("good"));
                    WfsOrAtomFeed.Add(statusReport.NumberOfItemsWithWfsOrAtom("good"));
                    AtomFeed.Add(statusReport.NumberOfInspireDatasetsWithAtomFeed("good"));
                    HarmonizedData.Add(statusReport.NumberOfItemsWithHarmonizedData("good"));
                    SpatialDataService.Add(statusReport.NumberOfItemsWithSpatialDataService("good"));
                }
            }
        }
    }
}