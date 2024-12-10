using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels.StatusReportViewModels
{
    public class FairLineChart
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

        public FairLineChart(List<StatusReport> statusReports, StatusReport selectedStatusReport, string statusType = "Metadata")
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
                    Metadata.Add(statusReport.NumberOfFairDatasetsWithMetadata("good"));
                    ProductSpecification.Add(statusReport.NumberOfFairDatasetsWithProductSpecification("good"));
                    ProductSheet.Add(statusReport.NumberOfFairDatasetsWithProductSheet("good"));
                    PresentationRules.Add(statusReport.NumberOfFairDatasetsWithPresentationRules("good"));
                    SosiRequirements.Add(statusReport.NumberOfFairDatasetsWithSosiRequirements("good"));
                    GmlRequirements.Add(statusReport.NumberOfFairDatasetsWithGmlRequirements("good"));
                    Wms.Add(statusReport.NumberOfFairDatasetsWithWms("good"));
                    Wfs.Add(statusReport.NumberOfFairDatasetsWithWfs("good"));
                    AtomFeed.Add(statusReport.NumberOfFairDatasetsWithAtomFeed("good"));
                    Common.Add(statusReport.NumberOfItemsWithCommon("good"));

                    var numberOfFairDatasetsWithFindable = (int) statusReport.FairDatasetsFindablePercent() / 4;
                    var numberOfFairDatasetsWithAccesible = (int)statusReport.FairDatasetsAccessiblePercent() / 4;
                    var numberOfFairDatasetsWithInteroperable = (int)statusReport.FairDatasetsInteroperablePercent() / 4;
                    var numberOfFairDatasetsWithReUsable = (int)statusReport.FairDatasetsReuseablePercent() / 4;


                    Findable.Add(numberOfFairDatasetsWithFindable);
                    Accesible.Add(numberOfFairDatasetsWithAccesible);
                    Interoperable.Add(numberOfFairDatasetsWithInteroperable);
                    ReUsable.Add(numberOfFairDatasetsWithReUsable);



                }
            }
        }
    }
}