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
            foreach (var item in register.items)
            {
                if (item is Dataset dataset)
                {
                    var datasetStatuses = new DatasetStatusHistory(dataset);
                    statusReport.StatusHistories.Add(datasetStatuses);
                    //dataset.StatusHistories.Add(datasetStatuses);
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
    }
}