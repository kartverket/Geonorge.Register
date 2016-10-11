using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class MunicipalityService : IMunicipalityService
    {
        public string LookupMunicipalityCodeFromOrganizationNumber(string organizationNumber)
        {
            string municipalityCode = null;
            MunicipalityData.MunicipalityFromOrganizationNumberToCode.TryGetValue(organizationNumber, out municipalityCode);
            return municipalityCode;
        }

        public string LookupOrganizationNumberFromMunicipalityCode(string municipalityCode)
        {
            return MunicipalityData.MunicipalityFromCodeToOrganizationNumber[municipalityCode];
        }
    }
}