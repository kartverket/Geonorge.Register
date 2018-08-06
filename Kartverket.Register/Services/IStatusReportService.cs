using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public interface IStatusReportService
    {
        void CreateStatusReport(Models.Register register);
        ICollection<DatasetStatusHistory> GetStatusHistoriesByDataset(Dataset dataset);
        StatusReport GetLatestReport();
        List<StatusReport> GetStatusReports(int numberOfReports = 0);
        StatusReport GetStatusReportById(string statusReportId);
    }
}
