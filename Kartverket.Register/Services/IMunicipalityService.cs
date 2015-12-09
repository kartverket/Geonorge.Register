using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public interface IMunicipalityService
    {
        string LookupMunicipalityCodeFromOrganizationNumber(string organizationNumber);
        MunicipalityCenterPoint GetMunicipalityCenterPoint(string municipalityCode);
        string LookupOrganizationNumberFromMunicipalityCode(string municipalityCode);
    }
}
