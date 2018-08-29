using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class StatusReportService : IStatusReportService
    {

        private readonly RegisterDbContext _dbContext;
        public StatusReportService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateStatusReport(Models.Register register)
        {
            var statusReport = new StatusReport();
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
                return reports.OrderBy(o => o.Date).ToList();
            }

            return queryResults.OrderBy(o => o.Date).ToList();
        }

        public StatusReport GetStatusReportById(string statusReportId)
        {
            var queryResults = from r in _dbContext.StatusReports
                               where r.Id.ToString() == statusReportId
                                select r;

            return queryResults.FirstOrDefault();
        }

        public List<StatusReport> GetDokStatusReports(int i)
        {
            List<StatusReport> statusReports = GetStatusReports();
            List<StatusReport> dokStatusReports = new List<StatusReport>();
            
                foreach (var report in statusReports)
                {
                if (report.IsDokReport())
                    {
                        dokStatusReports.Add(report);
                        if (dokStatusReports.Count > i)
                        {
                            break;
                        }
                    }
                }

            return dokStatusReports;
        }

        public List<StatusReport> GetInspireStatusReports(int i, string type)
        {
            List<StatusReport> statusReports = GetStatusReports();
            List<StatusReport> inpsireStatusReports = new List<StatusReport>();

            foreach (var report in statusReports)
            {
                if (report.IsInspireDatasetReport())
                {
                    inpsireStatusReports.Add(report);
                    if (inpsireStatusReports.Count > i)
                    {
                        break;
                    }
                }
            }

            return inpsireStatusReports;
        }
    }
}