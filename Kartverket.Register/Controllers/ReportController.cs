using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;
using Kartverket.Register.Services.Report;
using Kartverket.ReportApi;

namespace Kartverket.Register.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ReportController : ApiController
    {
        private readonly IDokReportService _dokReportService;

        public ReportController(IDokReportService dokReportService)
        {
            _dokReportService = dokReportService;
        }

        /// <summary>
        /// Return reports for DOK.
        /// Supported Queries:
        /// QueryName = "register-DOK-selectedAndAdditional".
        /// QueryName = "register-DOK-selectedTheme".
        /// QueryName = "register-DOK-coverage".
        /// </summary>
        [HttpPost]
        [HttpGet]
        public ReportResult Post(ReportQuery query)
        {
            Trace.WriteLine("QueryName: " + query.QueryName);

            ReportResult result = new ReportResult();

            if (query.QueryName == "register-DOK-selectedAndAdditional")
                result = _dokReportService.GetSelectedAndAdditionalDatasets(query);
            else if (query.QueryName == "register-DOK-selectedSuitability")
                result = _dokReportService.GetSelectedSuitabilityDatasets(query);
            else if (query.QueryName == "register-DOK-selectedTheme")
                result = _dokReportService.GetSelectedDatasetsByTheme(query);
            else if (query.QueryName == "register-DOK-coverage")
                result = _dokReportService.GetSelectedDatasetsCoverage(query);


            return result;
        }

        /// <summary>
        /// Return report for DOK-selectedAndAdditional.
        /// </summary>
        [HttpGet]
        public ReportResult DokSelected()
        {
            ReportQuery query = new ReportQuery
            {
                QueryName = "register-DOK-selectedAndAdditional",
                Parameters = new System.Collections.Generic.List<ReportQueryParameter>()
            };

            query.Parameters.Add(new ReportQueryParameter { Name = "area" , Value ="Hele landet" });
            query.Parameters.Add(new ReportQueryParameter { Name = "data", Value = "e4eb3a1d-481e-45a2-8a58-ead15240a9b0" });

            return Post(query);
        }

    }
}