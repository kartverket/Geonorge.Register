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
        CodelistValue GetMunicipality();
        /// <summary>
        /// Checks if the user has access to create or edit DOK Municipal for selected municipality
        /// </summary>
        /// <param name="municipalityCode">Selected municipality code</param>
        /// <returns>bool</returns>
        bool AccessEditOrCreateDOKMunicipalBySelectedMunicipality(string municipalityCode);
        bool IsOwner(string owner, string user);
    }
}
