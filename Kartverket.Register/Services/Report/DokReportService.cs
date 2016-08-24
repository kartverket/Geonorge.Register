using Kartverket.Register.Models;
using Kartverket.ReportApi;
using System.Linq;
using System.Collections.Generic;

namespace Kartverket.Register.Services.Report
{
    public class DokReportService : IDokReportService
    {
        private readonly RegisterDbContext _dbContext;

        public DokReportService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ReportResult GetSelectedAndAdditionalDatasets()
        {

            ReportResult reportResult = new ReportResult();
            reportResult.Data = new List<ReportResultData>();

            var total = _dbContext.CoverageDatasets.Select(m => m.DatasetId).Distinct().Count();

            reportResult.TotalDataCount = total;

            var results = (from c in _dbContext.CoverageDatasets
                       where c.Coverage == true
                       group c by c.MunicipalityId into grouped
                       select new
                       {
                           MunicipalityId = grouped.Key,
                           Count = grouped.Count()
                       }).OrderByDescending(x => x.Count);


            foreach (var result in results.ToList())
            {
                ReportResultData reportResultData = new ReportResultData();

                List<ReportResultDataValue> reportResultDataValues = new List<ReportResultDataValue>();

                reportResultData.Label = result.MunicipalityId.ToString();

                ReportResultDataValue reportResultDataValue = new ReportResultDataValue();

                reportResultDataValue.Key = "";
                reportResultDataValue.Value = result.Count.ToString();

                reportResultDataValues.Add(reportResultDataValue);

                reportResultData.Values = reportResultDataValues;

                reportResult.Data.Add(reportResultData);
            }

            return reportResult;
        }

    }
}