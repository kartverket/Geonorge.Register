using Kartverket.ReportApi;

namespace Kartverket.Register.Services.Report
{
    public interface IDokReportService
    {
        ReportResult GetSelectedAndAdditionalDatasets(ReportQuery q);
    }
}