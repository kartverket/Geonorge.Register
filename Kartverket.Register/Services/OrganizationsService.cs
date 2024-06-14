using System.Linq;
using Kartverket.Register.Models;
using System.Collections.Generic;
using Kartverket.Register.Models.Translations;

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
            return _dbContext.Organizations.FirstOrDefault(o => o.name == name);
        }

        public Organization GetOrganizationByNumber(string number, string culture = Culture.NorwegianCode)
        {
            Organization organization = null;

            if (culture == Culture.NorwegianCode)
                organization = _dbContext.Organizations.FirstOrDefault(o => o.number == number);
            else
            { 
                organization = _dbContext.Organizations.FirstOrDefault(o => o.number == number && o.Translations.Any(oo => oo.CultureName == culture));

                if (organization != null)
                {
                    var translated = organization.Translations.Where(t => t.CultureName == culture).FirstOrDefault();
                    organization.name = translated.Name;
                    organization.description = translated.Description;
                }
            }

            return organization;
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

        public Organization GetMunicipalityByNumber(string number)
        {
            var queryResults = from o in _dbContext.Organizations
                               where o.OrganizationType == Models.OrganizationType.Municipality
                               && o.MunicipalityCode == number
                               select o;

            Organization organizations = queryResults.FirstOrDefault();
            return organizations;
        }

        public Organization GetOrganizationTranslatedByName(string name, string culture)
        {
            var organization = _dbContext.Organizations.FirstOrDefault(o => o.name == name && o.Translations.Any(oo => oo.CultureName == culture));
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