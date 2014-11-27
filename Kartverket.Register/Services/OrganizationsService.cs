using System.Linq;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class OrganizationsService : IOrganizationService
    {
        private readonly RegisterDbContext _dbContext;

        public OrganizationsService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Organization GetOrganizationByName(string name)
        {
            return _dbContext.Organizations.SingleOrDefault(o => o.name == name);
        }

        public Organization GetOrganizationByNumber(string number)
        {
            return _dbContext.Organizations.SingleOrDefault(o => o.number == number);
        }
    }
}