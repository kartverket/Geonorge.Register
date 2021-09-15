using Kartverket.Register.Models;
using Kartverket.ReportApi;
using System.Linq;
using System.Collections.Generic;
using Eu.Europa.Ec.Jrc.Inspire;
using DateTime = System.DateTime;

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

            _dbContext.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            var total = (from  dd in _dbContext.Datasets 
                         where dd.DatasetType != "Kommunalt"
                         select dd.systemId).Distinct().Count();

            reportResult.TotalDataCount = 0;

            var resultsSelected = (from c in _dbContext.CoverageDatasets
                                   join ds in _dbContext.Datasets on c.DatasetId equals ds.systemId
                                   where c.ConfirmedDok == true && ds.DatasetType != "Kommunalt"
                                   group c by new { c.Municipality.name, c.Municipality.number, c.Municipality.DateConfirmedMunicipalDOK, c.Municipality.MunicipalityCode, c.Municipality.StatusConfirmationMunicipalDOK } into grouped
                                   select new
                                   {
                                       name = grouped.Key.name,
                                       Count = grouped.Distinct().Count(),
                                       number = grouped.Key.number,
                                       DateConfirmedMunicipalDOK = grouped.Key.DateConfirmedMunicipalDOK,
                                       MunicipalityCode = grouped.Key.MunicipalityCode,
                                       DOKStatus = grouped.Key.StatusConfirmationMunicipalDOK
                                   }).OrderByDescending(x => x.Count).ToList();

            var resultsSelectedAdditional = (from c in _dbContext.CoverageDatasets
                                             join ds in _dbContext.Datasets on c.DatasetId equals ds.systemId
                                             where ds.DatasetType == "Kommunalt"
                                             group c by new { c.Municipality.name, c.Municipality.number, c.Municipality.DateConfirmedMunicipalDOK, c.Municipality.MunicipalityCode, c.Municipality.StatusConfirmationMunicipalDOK } into grouped
                                             select new
                                             {
                                                 name = grouped.Key.name,
                                                 Count = grouped.Count(),
                                                 number = grouped.Key.number,
                                                 DateConfirmedMunicipalDOK = grouped.Key.DateConfirmedMunicipalDOK,
                                                 MunicipalityCode = grouped.Key.MunicipalityCode,
                                                 DOKStatus = grouped.Key.StatusConfirmationMunicipalDOK
                                             }).ToList();


            var resultsNotSelected = (from c in _dbContext.CoverageDatasets
                                   join ds in _dbContext.Datasets on c.DatasetId equals ds.systemId
                                   where c.ConfirmedDok == false && ds.DatasetType != "Kommunalt"
                                   group c by new { c.Municipality.name, c.Municipality.number, c.Municipality.DateConfirmedMunicipalDOK, c.Municipality.MunicipalityCode, c.Municipality.StatusConfirmationMunicipalDOK } into grouped
                                   select new
                                   {
                                       name = grouped.Key.name,
                                       Count = 0,
                                       number = grouped.Key.number,
                                       DateConfirmedMunicipalDOK = grouped.Key.DateConfirmedMunicipalDOK,
                                       MunicipalityCode = grouped.Key.MunicipalityCode,
                                       DOKStatus = grouped.Key.StatusConfirmationMunicipalDOK
                                   })
                                   .OrderByDescending(x => x.Count)
                                   .ToList();

                resultsNotSelected = (from cc in resultsNotSelected
                                      where !resultsSelected.Any(s => s.number == cc.number)
                                      select cc)
                                     .ToList();

            var results = resultsSelected.Union(resultsNotSelected).Distinct();

            var areas = param.Parameters.Where(p => p.Name == "area").Select(a => a.Value).ToList();
            if (areas.Any())
            {
                if (areas[0] != "Hele landet")
                {

                    var resultsNr = (from res in results
                                     join munici in _dbContext.Organizations on res.number equals munici.number
                                     select new { res.name, res.Count, res.number, munici.MunicipalityCode, munici.DateConfirmedMunicipalDOK, res.DOKStatus }).ToList();

                    results = (from r in resultsNr
                               where (from c in areas
                                      select c)
                                          .Contains(r.MunicipalityCode)
                                 || (from c in areas
                                     select c)
                                          .Contains(r.MunicipalityCode.Substring(0, 2))

                               select new { r.name, r.Count, r.number, r.DateConfirmedMunicipalDOK, r.MunicipalityCode, r.DOKStatus }).ToList();
                }
            }


            foreach (var result in results.OrderByDescending(s => s.Count).ThenBy(ss => ss.name))
            {
                ReportResultData reportResultData = new ReportResultData();

                List<ReportResultDataValue> reportResultDataValues = new List<ReportResultDataValue>();

                reportResultData.Label = result.name.ToString();
                reportResultData.TotalDataCount = total;

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

                //additional DateConfirmedMunicipalDOK start
                reportResultDataValue = new ReportResultDataValue();
                reportResultDataValue.Key = "Bekreftet";
                reportResultDataValue.Value = result.DateConfirmedMunicipalDOK.HasValue ? result.DateConfirmedMunicipalDOK.Value.ToString("dd.MM.yyyy") : "";
                reportResultDataValues.Add(reportResultDataValue);
                //additional DateConfirmedMunicipalDOK end

                //additional Number start
                reportResultDataValue = new ReportResultDataValue();
                reportResultDataValue.Key = "Number";
                reportResultDataValue.Value = result.MunicipalityCode;
                reportResultDataValues.Add(reportResultDataValue);
                //additional Number end

                //additional Status start
                reportResultDataValue = new ReportResultDataValue();
                reportResultDataValue.Key = "DOKStatus";
                reportResultDataValue.Value = result.DOKStatus;
                reportResultDataValues.Add(reportResultDataValue);
                //additional Status end

                reportResultData.Values = reportResultDataValues;

                reportResult.Data.Add(reportResultData);
            }

            return reportResult;
        }


        public ReportResult GetSelectedMeasureDatasets(ReportQuery param)
        {
            ReportResult reportResult = new ReportResult();

            reportResult.Data = new List<ReportResultData>();

            _dbContext.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            var total = (from dd in _dbContext.Datasets
                         where dd.DatasetType != "Kommunalt"
                         select dd.systemId).Distinct().Count();

            reportResult.TotalDataCount = 0;

            var resultsSelected = (from c in _dbContext.CoverageDatasets
                                   join ds in _dbContext.Datasets on c.DatasetId equals ds.systemId
                                   where !string.IsNullOrEmpty(c.MeasureDOKStatusId) && ds.DatasetType != "Kommunalt"
                                   group c by new { c.Municipality.name, c.Municipality.number, c.Municipality.DateConfirmedMunicipalDOK, c.Municipality.MunicipalityCode, c.Municipality.StatusConfirmationMunicipalDOK } into grouped
                                   select new
                                   {
                                       name = grouped.Key.name,
                                       Count = grouped.Distinct().Count(),
                                       number = grouped.Key.number,
                                       DateConfirmedMunicipalDOK = grouped.Key.DateConfirmedMunicipalDOK,
                                       MunicipalityCode = grouped.Key.MunicipalityCode,
                                       DOKStatus = grouped.Key.StatusConfirmationMunicipalDOK
                                   }).OrderByDescending(x => x.Count).ToList();



            var resultsNotSelected = (from c in _dbContext.CoverageDatasets
                                      join ds in _dbContext.Datasets on c.DatasetId equals ds.systemId
                                      where string.IsNullOrEmpty(c.MeasureDOKStatusId) && ds.DatasetType != "Kommunalt"
                                      group c by new { c.Municipality.name, c.Municipality.number, c.Municipality.DateConfirmedMunicipalDOK, c.Municipality.MunicipalityCode, c.Municipality.StatusConfirmationMunicipalDOK } into grouped
                                      select new
                                      {
                                          name = grouped.Key.name,
                                          Count = 0,
                                          number = grouped.Key.number,
                                          DateConfirmedMunicipalDOK = grouped.Key.DateConfirmedMunicipalDOK,
                                          MunicipalityCode = grouped.Key.MunicipalityCode,
                                          DOKStatus = grouped.Key.StatusConfirmationMunicipalDOK
                                      })
                                   .OrderByDescending(x => x.Count)
                                   .ToList();

            resultsNotSelected = (from cc in resultsNotSelected
                                  where !resultsSelected.Any(s => s.number == cc.number)
                                  select cc)
                                 .ToList();

            var results = resultsSelected.Union(resultsNotSelected).Distinct();

            var areas = param.Parameters.Where(p => p.Name == "area").Select(a => a.Value).ToList();
            if (areas.Any())
            {
                if (areas[0] != "Hele landet")
                {

                    var resultsNr = (from res in results
                                     join munici in _dbContext.Organizations on res.number equals munici.number
                                     select new { res.name, res.Count, res.number, munici.MunicipalityCode, munici.DateConfirmedMunicipalDOK, res.DOKStatus }).ToList();

                    results = (from r in resultsNr
                               where (from c in areas
                                      select c)
                                          .Contains(r.MunicipalityCode)
                                 || (from c in areas
                                     select c)
                                          .Contains(r.MunicipalityCode.Substring(0, 2))

                               select new { r.name, r.Count, r.number, r.DateConfirmedMunicipalDOK, r.MunicipalityCode, r.DOKStatus }).ToList();
                }
            }


            foreach (var result in results.OrderByDescending(s => s.Count).ThenBy(ss => ss.name))
            {
                ReportResultData reportResultData = new ReportResultData();

                List<ReportResultDataValue> reportResultDataValues = new List<ReportResultDataValue>();

                reportResultData.Label = result.name.ToString();
                reportResultData.TotalDataCount = total;

                ReportResultDataValue reportResultDataValue = new ReportResultDataValue();

                reportResultDataValue.Key = "Det offentlige kartgrunnlaget";
                reportResultDataValue.Value = result.Count.ToString();

                reportResultDataValues.Add(reportResultDataValue);

                reportResultData.Values = reportResultDataValues;

                reportResult.Data.Add(reportResultData);
            }

            return reportResult;
        }

        public ReportResult GetSelectedDatasetsByTheme(ReportQuery param)
        {

            ReportResult reportResult = new ReportResult();
            reportResult.Data = new List<ReportResultData>();

            var total = _dbContext.Organizations.Where(m => m.OrganizationType == Models.OrganizationType.Municipality).Count();

            reportResult.TotalDataCount = 0;

            var results = (from c in _dbContext.CoverageDatasets.DefaultIfEmpty()
                                   join d in _dbContext.Datasets on c.DatasetId equals d.systemId
                                   where c.ConfirmedDok == true && d.DatasetType != "Kommunalt"
                           group c by new { d.theme.description, d.name } into grouped
                                   select new
                                   {
                                       theme = grouped.Key.description,
                                       Count = grouped.Count(),
                                       name = grouped.Key.name
                                   }).OrderByDescending(x => x.Count).ToList();

            foreach (var result in results)
            {
                ReportResultData reportResultData = new ReportResultData();

                List<ReportResultDataValue> reportResultDataValues = new List<ReportResultDataValue>();

                reportResultData.Label = result.theme.ToString();
                reportResultData.TotalDataCount = total;

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


        public ReportResult GetSelectedDatasetsCoverage(ReportQuery param)
        {

            ReportResult reportResult = new ReportResult();
            reportResult.Data = new List<ReportResultData>();

            var total = 0;

            reportResult.TotalDataCount = total;

            var results = (from c in _dbContext.CoverageDatasets
                           join d in _dbContext.Datasets on c.DatasetId equals d.systemId
                           let datasetName = d.name
                           select new { c.Municipality.name, datasetName, c.Coverage, c.ConfirmedDok,c.Municipality.number, c.Municipality.MunicipalityCode }).Distinct().OrderBy(o=>o.name).ThenBy(o2=>o2.datasetName).ToList();

            var orgList = (from org in _dbContext.Organizations
                           where org.OrganizationType == Models.OrganizationType.Municipality
                           select new { org.name, org.number }).ToList();

            var coverageList = (from c in _dbContext.CoverageDatasets
                                select c.Municipality.name).ToList();



            var areas = param.Parameters.Where(p => p.Name == "area").Select(a => a.Value).ToList();
            if (areas.Any())
            {
                if (areas[0] != "Hele landet")
                {

                    var resultsNr = (from res in results
                                     join munici in _dbContext.Organizations on res.number equals munici.number
                                     select new { res.name, res.datasetName, res.Coverage, res.ConfirmedDok, res.number, munici.MunicipalityCode }).ToList();

                    results = (from r in resultsNr
                               where (from c in areas
                                      select c)
                                          .Contains(r.MunicipalityCode)
                                 || (from c in areas
                                     select c)
                                          .Contains(r.MunicipalityCode.Substring(0, 2))

                               select new { r.name, r.datasetName, r.Coverage, r.ConfirmedDok, r.number, r.MunicipalityCode }).ToList();
                }
            }


            foreach (var result in results)
            {
                ReportResultData reportResultData = new ReportResultData();

                List<ReportResultDataValue> reportResultDataValues = new List<ReportResultDataValue>();

                reportResultData.Label = result.name.ToString();
                reportResultData.TotalDataCount = total;

                ReportResultDataValue reportResultDataValue = new ReportResultDataValue();

                reportResultDataValue.Key = result.datasetName;
                reportResultDataValue.Value = !result.Coverage.HasValue ? "Ukjent" : result.Coverage == true ? "JA" : "NEI";

                reportResultDataValues.Add(reportResultDataValue);


                reportResultDataValue = new ReportResultDataValue();
                reportResultDataValue.Key = "Valgt";
                reportResultDataValue.Value = result.ConfirmedDok ? "JA" : "NEI";
                reportResultDataValues.Add(reportResultDataValue);

                //additional Number start
                reportResultDataValue = new ReportResultDataValue();
                reportResultDataValue.Key = "Number";
                reportResultDataValue.Value = result.MunicipalityCode;
                reportResultDataValues.Add(reportResultDataValue);
                //additional Number end

                reportResultData.Values = reportResultDataValues;

                reportResult.Data.Add(reportResultData);
            }

            return reportResult;
        }
    }
}