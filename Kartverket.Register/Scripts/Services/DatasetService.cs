using System.Linq;
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
    }
}