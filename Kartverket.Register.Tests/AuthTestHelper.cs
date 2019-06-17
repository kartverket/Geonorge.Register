using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Geonorge.AuthLib.Common;

namespace Kartverket.Register.Tests
{
    public class AuthTestHelper
    {
        private List<Claim> _claims;

        public AuthTestHelper()
        {
            _claims = new List<Claim>();
        }

        /// <summary>
        /// Add organization claim
        /// </summary>
        /// <param name="organizationName"></param>
        /// <returns></returns>
        public AuthTestHelper SetCurrentOrganization(string organizationName)
        {
            _claims.Add(new Claim(GeonorgeClaims.OrganizationName, organizationName));
            return this;
        }

        /// <summary>
        /// Puts the list of user claims in the current principal on the running thread.
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal Invoke()
        {
            var identity = new ClaimsIdentity(_claims, "UnitTestAuthorization");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            Thread.CurrentPrincipal = claimsPrincipal;

            return claimsPrincipal;
        }
    }
}
