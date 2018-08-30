using System.Collections.Generic;
using System.Globalization;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.DOK.Service;

namespace Kartverket.Register.Models.ViewModels.StatusReportViewModels
{
    public class InspireDataServiceLineChart
    {
        public List<string> Labels { get; set; }

        public List<int> Metadata { get; set; }
        public List<int> MetadataSearchService { get; set; }
        public List<int> Servicestatus { get; set; }
        public List<int> Sds { get; set; }
        public List<int> NetworkService { get; set; }

        public List<int> PointSize { get; set; }

        public InspireDataServiceLineChart(List<StatusReport> statusReports, StatusReport selectedStatusReport)
        {
            Metadata = new List<int>();
            MetadataSearchService = new List<int>();
            Servicestatus = new List<int>();
            Sds = new List<int>();
            NetworkService = new List<int>();

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
                    MetadataSearchService.Add(statusReport.NumberOfItemsWithMetadataService("good"));
                    Servicestatus.Add(statusReport.NumberOfItemsWithServiceStatus("good"));
                    Sds.Add(statusReport.NumberOfInspireDataServiceWithSds());
                    NetworkService.Add(statusReport.NumberOfInspireDataServiceWithNetworkService());
                }
            }
        }
    }
}