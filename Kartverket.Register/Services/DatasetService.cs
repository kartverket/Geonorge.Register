using System.Linq;
using System.Web.Mvc;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class DatasetService : IDatasetService
    {
        private readonly RegisterDbContext _context;

        public DatasetService(RegisterDbContext context)
        {
            _context = context;
        }

        public Dataset GetDatasetByUuid(string uuid)
        {
            return _context.Datasets.First(d => d.Uuid == uuid);
        }

        public SelectList GetDokStatusSelectList(string statusId)
        {
            return new SelectList(_context.DokStatuses, "value", "description", statusId);
        }
    }
}