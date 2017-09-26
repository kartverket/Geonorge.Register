using Kartverket.Register.Models;
using System.Linq;

namespace Kartverket.Register.Services
{
    public class GeodatalovDatasetService : IGeodatalovDatasetService
    {
        private readonly RegisterDbContext _dbContext;

        public GeodatalovDatasetService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public GeodatalovDataset GetGeodatalovDatasetByName(string registerSeoName, string itemSeoName)
        {
            var queryResult = from i in _dbContext.GeodatalovDatasets
                              where i.Seoname == itemSeoName &&
                              i.Register.seoname == registerSeoName
                              select i;

            return queryResult.FirstOrDefault();
        }
    }
}