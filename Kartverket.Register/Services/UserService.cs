using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class UserService : IUserService
    {
        private readonly RegisterDbContext _dbContext;

        public UserService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Guid GetUserOrganizationId()
        {
            var user = GetUserOrganization();
            return user?.systemId ?? GetOrganizationKartverket();
        }

        private Guid GetOrganizationKartverket()
        {
            var queryResults = from o in _dbContext.Organizations
                where o.name == "Kartverket"
                select o.systemId;

            return queryResults.FirstOrDefault();
        }

        public Organization GetUserOrganization()
        {
            return GetOrganizationByUserOrganizationNr() ?? GetOrganizationByUserName();
        }

        private Organization GetOrganizationByUserOrganizationNr()
        {
            var orgnr = GetSecurityClaim("orgnr");

            if (orgnr == null) return null;
            var queryResults = from o in _dbContext.Organizations
                where o.number == orgnr
                select o;

            return queryResults.FirstOrDefault();
        }

        private Organization GetOrganizationByUserName()
        {
            var name = GetSecurityClaim("organization");

            if (name == null) return null;
            var queryResults = from o in _dbContext.Organizations
                where o.name == name
                select o;

            return queryResults.FirstOrDefault();
        }

        public string GetSecurityClaim(string type)
        {
            foreach (var claim in ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    return claim.Value;
                }
            }
            return null;
        }
    }
}