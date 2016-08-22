using System.Diagnostics;
using System.Web.Http;
using Kartverket.Register.Services.Report;
using Kartverket.ReportApi;

namespace Kartverket.Register.Controllers
{
    public class ReportController : ApiController
    {
        private readonly IDokReportService _dokReportService;

        public ReportController(IDokReportService dokReportService)
        {
            _dokReportService = dokReportService;
        }

        public ReportResult Post(ReportQuery query)
        {
            Trace.WriteLine("QueryName: " + query.QueryName);


            _dokReportService.GetSelectedDatasets();


            return new ReportResult() { TotalDataCount = 1 };
        }


    }
}