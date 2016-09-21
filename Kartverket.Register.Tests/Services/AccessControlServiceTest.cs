using System;
using NUnit.Framework;

using Kartverket.Register.Services;
using FluentAssertions;
using System.Security.Claims;
using System.Threading;
using System.Collections.Generic;

namespace Kartverket.Register.Tests.Services
{
    public class AccessControlServiceTest
    {

        private AccessControlService accessControlService = new AccessControlService();

        [TearDown]
        public void TearDown()
        {
            Thread.CurrentPrincipal = null;
        }

        [Test]
        public void GetOrganizationNumberShouldReturnOrgnrFromClaim()
        {
            SetClaims("orgnr", "123456789");

            accessControlService.GetOrganizationNumber().Should().Be("123456789");
        }

        [Test]
        public void GetOrganizationNumberShouldReturnNullIfNotSet()
        {
            accessControlService.GetOrganizationNumber().Should().BeNull();
        }

        [Test]
        public void IsMunicipalUserShouldReturnFalseIfUserHasNoOrgnrClaim()
        {
            accessControlService.IsMunicipalUser().Should().BeFalse();
        }

        [Test]
        public void IsMunicipalUserShouldReturnFalseIfOrganizationNumberDoesNotHaveMunicipalCode()
        {
            SetClaims("orgnr", "123456789");

            accessControlService.IsMunicipalUser().Should().BeFalse();
        }

        [Test]
        public void IsMunicipalUserShouldReturnTrueIfOrganizationNumberHasMunicipalCode()
        {
            SetClaims("orgnr", "940330408");

            accessControlService.IsMunicipalUser().Should().BeTrue();
        }

        [Test]
        public void MunicipalityShouldReturnMunicipalCode()
        {
            SetClaims("orgnr", "940330408");

            accessControlService.IsMunicipalUser().Should().BeTrue();
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
