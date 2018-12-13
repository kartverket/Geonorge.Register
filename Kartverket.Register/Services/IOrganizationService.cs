using Kartverket.Register.Models;
using Kartverket.Register.Models.Translations;
using System.Collections.Generic;

namespace Kartverket.Register.Services
{
    public interface IOrganizationService
    {
        Organization GetOrganizationByName(string name);
        Organization GetOrganizationByNumber(string number, string culture = Culture.NorwegianCode);
        Organization GetOrganization(string organization);
        List<Organization> GetMunicipalityOrganizations();
        Organization GetOrganizationTranslatedByName(string name, string culture);
    }
}
