using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class MunicipalityService : IMunicipalityService
    {
        public string LookupMunicipalityCodeFromOrganizationNumber(string organizationNumber)
        {
            return MunicipalityData.MunicipalityFromOrganizationNumberToCode[organizationNumber];
        }

        public string LookupOrganizationNumberFromMunicipalityCode(string municipalityCode)
        {
            return MunicipalityData.MunicipalityFromCodeToOrganizationNumber[municipalityCode];
        }

        public MunicipalityCenterPoint GetMunicipalityCenterPoint(string municipalityCode)
        {
            return MunicipalityData.MunicipalityCenterPoints[municipalityCode];
        }
    }
}