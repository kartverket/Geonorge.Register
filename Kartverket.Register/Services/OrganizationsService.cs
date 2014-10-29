using System;
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
            throw new NotImplementedException();
        }

        public Organization GetOrganizationByNumber(string number)
        {
            throw new NotImplementedException();
        }
    }
}