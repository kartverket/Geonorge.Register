using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public interface IOrganizationService
    {
        Organization GetOrganizationByName(string name);
        Organization GetOrganizationByNumber(string number);
        Organization GetOrganization(string organization);
    }
}
