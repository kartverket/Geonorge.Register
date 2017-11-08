using System.Linq;
using Kartverket.Register.Models;
using System.Collections.Generic;

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

        public Organization GetOrganization(string organizationName)
        {
            var queryResults = from o in _dbContext.Organizations
                               where o.name == organizationName
                               select o;

            Organization organization = queryResults.FirstOrDefault();
            return organization;
        }

        public List<Organization> GetMunicipalityOrganizations()
        {
            var queryResults = from o in _dbContext.Organizations
                               where o.OrganizationType == Models.OrganizationType.Municipality
                               select o;

            List<Organization> organizations = queryResults.ToList();
            return organizations;
        }

        public Organization GetOrganizationTranslatedByName(string name, string culture)
        {
            var organization = _dbContext.Organizations.SingleOrDefault(o => o.name == name && o.Translations.Any(oo => oo.CultureName == culture));
            if (organization != null)
            {
                var translated = organization.Translations.Where(t => t.CultureName == culture).FirstOrDefault();
                organization.name = translated.Name;
                organization.description = translated.Description;
            }

            return organization;
        }
    }
}