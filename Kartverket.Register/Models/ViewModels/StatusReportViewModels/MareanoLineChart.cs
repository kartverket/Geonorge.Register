using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels.StatusReportViewModels
{
    public class MareanoLineChart
    {
        public List<string> Labels { get; set; }

        public List<int> Metadata { get; set; }
        public List<int> ProductSpecification { get; set; }
        public List<int> ProductSheet { get; set; }
        public List<int> PresentationRules { get; set; }
        public List<int> SosiRequirements { get; set; }
        public List<int> GmlRequirements { get; set; }
        public List<int> Wms { get; set; }
        public List<int> Wfs { get; set; }
        public List<int> AtomFeed { get; set; }
        public List<int> Common { get; set; }

        public List<int> Findable { get; set; }
        public List<int> Accesible { get; set; }
        public List<int> Interoperable { get; set; }
        public List<int> ReUsable { get; set; }


        public List<int> PointSize { get; set; }

        public MareanoLineChart(List<StatusReport> statusReports, StatusReport selectedStatusReport, string statusType = "Metadata")
        {
            Metadata = new List<int>();
            ProductSpecification = new List<int>();
            ProductSheet = new List<int>();
            PresentationRules = new List<int>();
            SosiRequirements = new List<int>();
            GmlRequirements = new List<int>();
            Wms = new List<int>();
            Wfs = new List<int>();
            AtomFeed = new List<int>();
            Common = new List<int>();
            Labels = new List<string>();
            PointSize = new List<int>();
            Findable = new List<int>();
            Accesible = new List<int>();
            Interoperable = new List<int>();
            ReUsable = new List<int>();

            if (statusReports != null)
            {
                foreach (var statusReport in statusReports)
                {
                    var xName = statusReport.Date.ToString("d MMMM yyyy",
                        CultureInfo.CreateSpecificCulture("nb-NO"));
                    PointSize.Add(
                        selectedStatusReport != null && statusReport.Id == selectedStatusReport.Id ? 7 : 3);

                    Labels.Add(xName);
                    Metadata.Add(statusReport.NumberOfMareanoDatasetsWithMetadata("good"));
                    ProductSpecification.Add(statusReport.NumberOfMareanoDatasetsWithProductSpecification("good"));
                    ProductSheet.Add(statusReport.NumberOfMareanoDatasetsWithProductSheet("good"));
                    PresentationRules.Add(statusReport.NumberOfMareanoDatasetsWithPresentationRules("good"));
                    SosiRequirements.Add(statusReport.NumberOfMareanoDatasetsWithSosiRequirements("good"));
                    GmlRequirements.Add(statusReport.NumberOfMareanoDatasetsWithGmlRequirements("good"));
                    Wms.Add(statusReport.NumberOfMareanoDatasetsWithWms("good"));
                    Wfs.Add(statusReport.NumberOfMareanoDatasetsWithWfs("good"));
                    AtomFeed.Add(statusReport.NumberOfMareanoDatasetsWithAtomFeed("good"));
                    Common.Add(statusReport.NumberOfItemsWithCommon("good"));
                    Findable.Add(statusReport.NumberOfMareanoDatasetsWithFindable("good"));
                    Accesible.Add(statusReport.NumberOfMareanoDatasetsWithAccesible("good"));
                    Interoperable.Add(statusReport.NumberOfMareanoDatasetsWithInteroperable("good"));
                    ReUsable.Add(statusReport.NumberOfMareanoDatasetsWithReUsable("good"));
                }
            }
        }
    }
}