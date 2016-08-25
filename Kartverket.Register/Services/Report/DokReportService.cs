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

            var resultsSelected = (from c in _dbContext.CoverageDatasets
                       where c.Coverage == true
                       group c by c.Municipality.name into grouped
                       select new
                       {
                           name = grouped.Key,
                           Count = grouped.Count()
                       }).OrderByDescending(x => x.Count).ToList();

            var municipalityList = (from mun in MunicipalityData.MunicipalityFromOrganizationNumberToCode
                    select mun.Key).ToList();

            var orgList = (from o in municipalityList
                          join org in _dbContext.Organizations on o equals org.number
                          select org.name).ToList();

            var coverageList = (from c in _dbContext.CoverageDatasets
                                select c.Municipality.name).ToList();

            int Count = 0;
            var resultsNotSelected =(
                                  from name in orgList
                                  where !(from c in coverageList
                                          select c)
                                         .Contains(name)
                                  select new { name, Count }).ToList();

            var results = resultsSelected.Union(resultsNotSelected).Distinct();


            foreach (var result in results)
            {
                ReportResultData reportResultData = new ReportResultData();

                List<ReportResultDataValue> reportResultDataValues = new List<ReportResultDataValue>();

                reportResultData.Label = result.name.ToString();

                ReportResultDataValue reportResultDataValue = new ReportResultDataValue();

                reportResultDataValue.Key = "Det offentlige kartgrunnlaget";
                reportResultDataValue.Value = result.Count.ToString();

                reportResultDataValues.Add(reportResultDataValue);

                reportResultData.Values = reportResultDataValues;

                reportResult.Data.Add(reportResultData);
            }

            return reportResult;
        }

    }
}