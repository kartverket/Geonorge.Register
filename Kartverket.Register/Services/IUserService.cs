using System;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    interface IUserService
    {
        Guid GetUserOrganizationId();
        Organization GetUserOrganization();
    }
}
