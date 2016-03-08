using Kartverket.Register.Models;
using System.Collections.Generic;

namespace Kartverket.Register.Services
{
    public interface IAccessControlService
    {
        bool Access(object model);
        List<string> GetSecurityClaim(string type);
        bool IsAdmin();
        bool EditDOK(Dataset dataset);
        bool IsMunicipalUser();
        Organization MunicipalUserOrganization();
        string GetUserName();
        CodelistValue Municipality();
        bool AccessEditOrCreateDOKMunicipalBySelectedMunicipality(string municipalityCode);
    }
}
