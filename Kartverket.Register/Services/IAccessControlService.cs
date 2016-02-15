using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public interface IAccessControlService
    {
        bool Access(object model);
        string GetSecurityClaim(string type);
        bool IsAdmin();
        bool EditDOK(Dataset dataset);
        bool IsMunicipalUser();
        Organization MunicipalUserOrganization();
        string GetUserName();
        CodelistValue Municipality();
        bool AccessEditDOKMunicipalBySelectedMunicipality(string municipalityCode);
    }
}
