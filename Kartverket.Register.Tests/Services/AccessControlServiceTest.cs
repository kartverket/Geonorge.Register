using Kartverket.Register.Services;
using FluentAssertions;
using System.Security.Claims;
using System.Threading;
using System.Collections.Generic;
using Xunit;

namespace Kartverket.Register.Tests.Services
{
    public class AccessControlServiceTest
    {
        private readonly AccessControlService _accessControlService = new AccessControlService();

        public AccessControlServiceTest()
        {
            Thread.CurrentPrincipal = null;
        }

       [Fact]
        public void GetOrganizationNumberShouldReturnOrgnrFromClaim()
        {
            SetClaims("orgnr", "123456789");

            _accessControlService.GetOrganizationNumber().Should().Be("123456789");
        }

       [Fact]
        public void GetOrganizationNumberShouldReturnNullIfNotSet()
        {
            _accessControlService.GetOrganizationNumber().Should().BeNull();
        }

       [Fact]
        public void IsMunicipalUserShouldReturnFalseIfUserHasNoOrgnrClaim()
        {
            _accessControlService.IsMunicipalUser().Should().BeFalse();
        }

       [Fact]
        public void IsMunicipalUserShouldReturnFalseIfOrganizationNumberDoesNotHaveMunicipalCode()
        {
            SetClaims("orgnr", "123456789");

            _accessControlService.IsMunicipalUser().Should().BeFalse();
        }

       [Fact]
        public void IsMunicipalUserShouldReturnTrueIfOrganizationNumberHasMunicipalCode()
        {
            SetClaims("orgnr", "940330408");

            _accessControlService.IsMunicipalUser().Should().BeTrue();
        }

       [Fact]
        public void MunicipalityShouldReturnMunicipalCode()
        {
            SetClaims("orgnr", "940330408");

            _accessControlService.IsMunicipalUser().Should().BeTrue();
        }


        private static void SetClaims(params string[] typeAndValues)
        {
            var claims = new List<Claim>();
            for(int i =0; i<typeAndValues.Length; i+=2)
            {
                claims.Add(new Claim(typeAndValues[i], typeAndValues[i + 1]));
            }

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            Thread.CurrentPrincipal = claimsPrincipal;
        }
    }
}
