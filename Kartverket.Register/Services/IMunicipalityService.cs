using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public interface IMunicipalityService
    {
        string LookupMunicipalityCodeFromOrganizationNumber(string organizationNumber);
        string LookupOrganizationNumberFromMunicipalityCode(string municipalityCode);
    }
}
