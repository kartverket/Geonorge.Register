using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Models.StatusReports;

namespace Kartverket.Register.Services
{
    public class StatusReportService : IStatusReportService
    {

        private readonly RegisterDbContext _dbContext;
        public StatusReportService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateStatusReport(Models.Register register, bool latestSavedDataReport = false)
        {

            var statusReport = new StatusReport(latestSavedDataReport);
            statusReport.Register = register;
            foreach (var item in register.items)
            {
                if (item is Dataset dataset)
                {
                    var datasetStatuses = new DatasetStatusHistory(dataset);
                    statusReport.StatusRegisterItems.Add(datasetStatuses);
                }
            }

            foreach (var item in register.RegisterItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    var inspireDatasetStatuses = new InspireDatasetStatusReport(inspireDataset);
                    statusReport.StatusRegisterItems.Add(inspireDatasetStatuses);
                }
                if (item is InspireDataService inspireDataService)
                {
                    var inspireDataserviceStatuses = new InspireDataserviceStatusReport(inspireDataService);
                    statusReport.StatusRegisterItems.Add(inspireDataserviceStatuses);
                }

                if (item is GeodatalovDataset geodatalovDataset)
                {
                    var geodatalovDatasetStatuses = new GeodatalovDatasetStatusReport(geodatalovDataset);
                    statusReport.StatusRegisterItems.Add(geodatalovDatasetStatuses);
                }

                if (item is MareanoDataset mareanoDataset)
                {
                    if (latestSavedDataReport)
                    {
                        var mareanoRegistryId = Guid.Parse(GlobalVariables.MareanoRegistryId);
                        var currentReport = _dbContext.StatusReports.Where(s => s.Register.systemId == mareanoRegistryId && s.LatestSavedDataReport).FirstOrDefault();
                        if (currentReport != null)
                        {
                            _dbContext.Database.ExecuteSqlCommand("DELETE FROM RegisterItemStatusReports Where StatusReport_Id ='" + currentReport.Id + "'; DELETE FROM StatusReports WHERE Id = '" + currentReport.Id + "'");
                        }
                    }
                    var mareanoDatasetStatuses = new MareanoDatasetStatusReport(mareanoDataset);
                    statusReport.StatusRegisterItems.Add(mareanoDatasetStatuses);
                }

                if (item is FairDataset fairDataset)
                {
                    if (latestSavedDataReport)
                    {
                        var fairRegistryId = Guid.Parse(GlobalVariables.FairRegistryId);
                        var currentReport = _dbContext.StatusReports.Where(s => s.Register.systemId == fairRegistryId && s.LatestSavedDataReport).FirstOrDefault();
                        if (currentReport != null)
                        {
                            _dbContext.Database.ExecuteSqlCommand("DELETE FROM RegisterItemStatusReports Where StatusReport_Id ='" + currentReport.Id + "'; DELETE FROM StatusReports WHERE Id = '" + currentReport.Id + "'");
                        }
                    }
                    var fairDatasetStatuses = new FairDatasetStatusReport(fairDataset);
                    statusReport.StatusRegisterItems.Add(fairDatasetStatuses);
                }
            }

            _dbContext.StatusReports.Add(statusReport);
            _dbContext.SaveChanges();
        }

        public ICollection<DatasetStatusHistory> GetStatusHistoriesByDataset(Dataset dataset)
        {
            var queryResult = from d in _dbContext.DatasetStatusHistories
                              where d.DatasetUuid == dataset.Uuid
                              select d;

            return queryResult.ToList();
        }

        public StatusReport GetLatestReport()
        {
            var queryResults = from r in _dbContext.StatusReports
                               select r;

            StatusReport latestReport = queryResults.OrderByDescending(o => o.Date).FirstOrDefault();
            return latestReport;
        }

        public List<StatusReport> GetStatusReports(int numberOfReports = 0)
        {
            var queryResults = from r in _dbContext.StatusReports
                               select r;


            if (numberOfReports > 0)
            {

                var reports = queryResults.OrderByDescending(o => o.Date).Take(numberOfReports);
                return reports.ToList();
            }

            return queryResults.OrderByDescending(o => o.Date).ToList();
        }

        public StatusReport GetStatusReportById(string statusReportId)
        {
            var queryResults = from r in _dbContext.StatusReports
                               where r.Id.ToString() == statusReportId
                               select r;

            return queryResults.FirstOrDefault();
        }

        public List<StatusReport> GetDokStatusReports(int numberOfReports)
        {
            List<StatusReport> statusReports = GetStatusReports();
            List<StatusReport> dokStatusReports = new List<StatusReport>();

            foreach (var report in statusReports)
            {
                if (report.IsDokReport())
                {
                    dokStatusReports.Add(report);
                    if (numberOfReports != 0)
                    {
                        if (dokStatusReports.Count > numberOfReports)
                        {
                            break;
                        }
                    }
                }
            }

            dokStatusReports = dokStatusReports.OrderBy(d => d.Date).ToList();

            return dokStatusReports;
        }

        public List<StatusReport> GetInspireStatusReports(int numberOfReports = 0)
        {
            List<StatusReport> statusReports = GetStatusReports();
            List<StatusReport> inpsireStatusReports = new List<StatusReport>();

            foreach (var report in statusReports)
            {
                if (report.IsInspireDatasetReport())
                {
                    inpsireStatusReports.Add(report);
                    if (numberOfReports != 0)
                    {
                        if (inpsireStatusReports.Count > numberOfReports)
                        {
                            break;
                        }
                    }
                }
            }

            inpsireStatusReports = inpsireStatusReports.OrderBy(d => d.Date).ToList();

            return inpsireStatusReports;
        }

        public List<StatusReport> GetGeodatalovStatusReports(int numberOfReports = 0)
        {
            List<StatusReport> statusReports = GetStatusReports();
            List<StatusReport> geodatalovStatusReports = new List<StatusReport>();

            foreach (var report in statusReports)
            {
                if (report.IsGeodatalovDatasetReport())
                {
                    geodatalovStatusReports.Add(report);
                    if (numberOfReports != 0)
                    {
                        if (geodatalovStatusReports.Count > numberOfReports)
                        {
                            break;
                        }
                    }
                }
            }

            geodatalovStatusReports = geodatalovStatusReports.OrderBy(d => d.Date).ToList();

            return geodatalovStatusReports;
        }

        public List<StatusReport> GetMareanoStatusReports(int numberOfReports = 0)
        {
            List<StatusReport> statusReports = GetStatusReports();
            statusReports = statusReports.OrderByDescending(d => d.Date).ToList();
            List<StatusReport> mareanoStatusReports = new List<StatusReport>();

            foreach (var report in statusReports)
            {
                if (report.IsMareanoDatasetReport())
                {
                    mareanoStatusReports.Add(report);
                    if (numberOfReports != 0)
                    {
                        if (mareanoStatusReports.Count > numberOfReports)
                        {
                            break;
                        }
                    }
                }
            }

            return mareanoStatusReports;
        }

        public List<StatusReport> GetStatusReportsByRegister(Models.Register register, int numberOfReports = 0)
        {
            if (register.IsDokStatusRegister())
            {
                return GetDokStatusReports(numberOfReports);
            }

            if (register.IsInspireStatusRegister())
            {
                return GetInspireStatusReports(numberOfReports);
            }

            if (register.IsGeodatalovStatusRegister())
            {
                return GetGeodatalovStatusReports(numberOfReports);
            }

            if (register.IsMareanoStatusRegister())
            {
                return GetMareanoStatusReports(numberOfReports);
            }

            return new List<StatusReport>();
        }
    }
}