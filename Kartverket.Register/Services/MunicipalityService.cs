using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class MunicipalityService : IMunicipalityService
    {
        public string LookupMunicipalityCodeFromOrganizationNumber(string organizationNumber)
        {
            return "1201"; // bergen
        }

        public string LookupOrganizationNumberFromMunicipalityCode(string municipalityCode)
        {
            return "962276172"; // bø kommune
        }

        public MunicipalityCenterPoint GetMunicipalityCenterPoint(string municipalityCode)
        {
            return new MunicipalityCenterPoint("-29956", "6730321");
        }
    }
}