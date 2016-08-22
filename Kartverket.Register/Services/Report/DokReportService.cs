using Kartverket.Register.Models;
using Kartverket.ReportApi;
using System.Linq;

namespace Kartverket.Register.Services.Report
{
    public class DokReportService : IDokReportService
    {
        private readonly RegisterDbContext _dbContext;

        public DokReportService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ReportResult GetSelectedDatasets()
        {
            var results = (from c in _dbContext.CoverageDatasets
                       where c.Coverage == true
                       group c by c.MunicipalityId into grouped
                       select new
                       {
                           MunicipalityId = grouped.Key,
                           Count = grouped.Count()
                       }).OrderByDescending(x => x.Count);


            return new ReportResult();
        }

    }
}