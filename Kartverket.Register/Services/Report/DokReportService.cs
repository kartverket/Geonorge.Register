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

        public ReportResult GetSelectedAndAdditionalDatasets(ReportQuery param)
        {

            ReportResult reportResult = new ReportResult();
            reportResult.Data = new List<ReportResultData>();

            var total = _dbContext.CoverageDatasets.Select(m => m.DatasetId).Distinct().Count();

            reportResult.TotalDataCount = total;

            var resultsSelected = (from c in _dbContext.CoverageDatasets
                                   where c.ConfirmedDok == true
                                   group c by new { c.Municipality.name, c.Municipality.number } into grouped
                                   select new
                                   {
                                       name = grouped.Key.name,
                                       Count = grouped.Count(),
                                       number = grouped.Key.number
                                   }).OrderByDescending(x => x.Count).ToList();

            var resultsSelectedAdditional = (from c in _dbContext.CoverageDatasets
                                             join ds in _dbContext.Datasets on c.DatasetId equals ds.systemId
                                             where ds.DatasetType == "Kommunalt"
                                             group c by new { c.Municipality.name, c.Municipality.number } into grouped
                                             select new
                                             {
                                                 name = grouped.Key.name,
                                                 Count = grouped.Count(),
                                                 number = grouped.Key.number
                                             }).ToList();


            var municipalityList = (from mun in MunicipalityData.MunicipalityFromOrganizationNumberToCode
                                    select mun.Key).ToList();

            var orgList = (from o in municipalityList
                           join org in _dbContext.Organizations on o equals org.number
                           select new { org.name, org.number }).ToList();

            var coverageList = (from c in _dbContext.CoverageDatasets
                                select c.Municipality.name).ToList();

            int Count = 0;
            var resultsNotSelected = (
                                  from mun in orgList
                                  where !(from c in coverageList
                                          select c)
                                         .Contains(mun.name)
                                  select new { mun.name, Count, mun.number }).ToList();

            var results = resultsSelected.Union(resultsNotSelected).Distinct();


            var areas = param.Parameters.Where(p => p.Name == "area").Select(a => a.Value).ToList();
            if (areas.Any())
            {
                if (areas[0] != "Hele landet")
                {

                    var resultsNr = (from res in results
                                     join munici in MunicipalityData.MunicipalityFromOrganizationNumberToCode on res.number equals munici.Key
                                     select new { res.name, res.Count, res.number, munici.Value }).ToList();

                    results = (from r in resultsNr
                               where (from c in areas
                                      select c)
                                          .Contains(r.Value)
                                 || (from c in areas
                                     select c)
                                          .Contains(r.Value.Substring(0, 2))

                               select new { r.name, r.Count, r.number }).ToList();


                }
            }


            foreach (var result in results)
            {
                ReportResultData reportResultData = new ReportResultData();

                List<ReportResultDataValue> reportResultDataValues = new List<ReportResultDataValue>();

                reportResultData.Label = result.name.ToString();

                ReportResultDataValue reportResultDataValue = new ReportResultDataValue();

                reportResultDataValue.Key = "Det offentlige kartgrunnlaget";
                reportResultDataValue.Value = result.Count.ToString();

                reportResultDataValues.Add(reportResultDataValue);

                //additional start
                var additionalItem = (from ra in resultsSelectedAdditional
                                      where ra.number == result.number
                                      select ra.Count).FirstOrDefault();

                reportResultDataValue = new ReportResultDataValue();
                reportResultDataValue.Key = "Tillegg";
                reportResultDataValue.Value = additionalItem.ToString();
                reportResultDataValues.Add(reportResultDataValue);
                //additional end


                reportResultData.Values = reportResultDataValues;

                reportResult.Data.Add(reportResultData);
            }

            return reportResult;
        }


        public ReportResult GetSelectedDatasetsByTheme(ReportQuery param)
        {

            ReportResult reportResult = new ReportResult();
            reportResult.Data = new List<ReportResultData>();

            var total = MunicipalityData.MunicipalityFromOrganizationNumberToCode.Count();

            reportResult.TotalDataCount = total;

            var results = (from c in _dbContext.CoverageDatasets.DefaultIfEmpty()
                                   join d in _dbContext.Datasets on c.DatasetId equals d.systemId
                                   where c.ConfirmedDok == true
                                   group c by new { d.theme.description, d.name } into grouped
                                   select new
                                   {
                                       theme = grouped.Key.description,
                                       Count = grouped.Count(),
                                       name = grouped.Key.name
                                   }).OrderBy(x => x.theme).ThenBy(y => y.name).ToList();

            foreach (var result in results)
            {
                ReportResultData reportResultData = new ReportResultData();

                List<ReportResultDataValue> reportResultDataValues = new List<ReportResultDataValue>();

                reportResultData.Label = result.theme.ToString();

                ReportResultDataValue reportResultDataValue = new ReportResultDataValue();

                reportResultDataValue.Key = result.name.ToString();
                reportResultDataValue.Value = result.Count.ToString();

                reportResultDataValues.Add(reportResultDataValue);

                //additional start
                reportResultDataValue = new ReportResultDataValue();
                reportResultDataValue.Key = "";
                reportResultDataValue.Value = "";
                reportResultDataValues.Add(reportResultDataValue);
                //additional end

                reportResultData.Values = reportResultDataValues;

                reportResult.Data.Add(reportResultData);
            }

            return reportResult;
        }

    }
}