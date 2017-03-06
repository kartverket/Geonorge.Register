using Kartverket.Register.Models;
using System.Collections.Generic;

namespace Kartverket.Register.Services
{
    public interface IOrganizationService
    {
        Organization GetOrganizationByName(string name);
        Organization GetOrganizationByNumber(string number);
        Organization GetOrganization(string organization);
        List<Organization> GetMunicipalityOrganizations();
    }
}
