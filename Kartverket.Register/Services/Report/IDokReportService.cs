using Kartverket.ReportApi;

namespace Kartverket.Register.Services.Report
{
    public interface IDokReportService
    {
        ReportResult GetSelectedAndAdditionalDatasets(ReportQuery q);
        ReportResult GetSelectedSuitabilityDatasets(ReportQuery q);
        ReportResult GetSelectedDatasetsByTheme(ReportQuery q);
        ReportResult GetSelectedDatasetsCoverage(ReportQuery q); 
    }
}