using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public interface IAccessControlService
    {
        bool Access(object model);
        string GetSecurityClaim(string type);
    }
}
