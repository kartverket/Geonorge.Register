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
        private const string Editor = "nd.metadata";
        private const string Admin = "nd.metadata_admin";
        private const string DokEditor = "nd.dok_editor";
        private const string DokAdmin = "nd.dok_admin";
        private const string Role = "role";
        private const string Orgnr = "orgnr";
        private readonly Models.Register _register;
        private readonly Models.Organization _owner;

        private readonly AccessControlService _accessControlService = new AccessControlService();


        public AccessControlServiceTest()
        {
            Thread.CurrentPrincipal = null;
            _register = new Models.Register();
            _owner = new Models.Organization {name = "Kartverket"};
        }

       [Fact]
        public void GetOrganizationNumberShouldReturnOrgnrFromClaim()
        {
            SetClaims(Orgnr, "123456789");

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
            SetClaims(Orgnr, "123456789");

            _accessControlService.IsMunicipalUser().Should().BeFalse();
        }

       [Fact]
        public void IsMunicipalUserShouldReturnTrueIfOrganizationNumberHasMunicipalCode()
        {
            SetClaims(Orgnr, "940330408");

            _accessControlService.IsMunicipalUser().Should().BeTrue();
        }

       [Fact]
        public void MunicipalityShouldReturnMunicipalCode()
        {
            SetClaims(Orgnr, "940330408");

            _accessControlService.IsMunicipalUser().Should().BeTrue();
        }

        [Fact]
        public void RoleIsEditor()
        {
            SetClaims(Role, Editor);
            _accessControlService.IsEditor().Should().BeTrue();

        }

        [Fact]
        public void RoleIsNotEditor()
        {
            SetClaims(Role, "");
            _accessControlService.IsEditor().Should().BeFalse();

        }

        [Fact]
        public void RoleIsAdmin()
        {
            SetClaims(Role, Admin);
            _accessControlService.IsAdmin().Should().BeTrue();

        }

        [Fact]
        public void RoleIsNotAdmin()
        {
            SetClaims(Role, "");
            _accessControlService.IsAdmin().Should().BeFalse();
        }

        [Fact]
        public void RoleIsDokAdmin()
        {
            SetClaims(Role, DokAdmin);
            _accessControlService.IsDokAdmin().Should().BeTrue();
        }

        [Fact]
        public void RoleIsNotDokAdmin()
        {
            SetClaims(Role, "");
            _accessControlService.IsDokAdmin().Should().BeFalse();
        }

        [Fact]
        public void RoleIsDokEditor()
        {
            SetClaims(Role, DokEditor);
            _accessControlService.IsDokEditor().Should().BeTrue();
        }

        [Fact]
        public void RoleIsNotDokEditor()
        {
            SetClaims(Role, "");
            _accessControlService.IsDokEditor().Should().BeFalse();
        }

        [Fact]
        public void AccessRegisterIfContainedItemClassIsNotCodelistValue()
        {
            SetClaims(Role, Editor);

            _register.accessId = 2;
            _register.containedItemClass = "Organization";

            _accessControlService.AccessRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterIfUserIsRegisterOwner()
        {
            SetClaims(Role, Editor);

            _register.accessId = 2;
            _register.owner = _owner;

            _accessControlService.AccessRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterIfRegisterAccessLevelIsMunicipalUserDokEditorAndDokAdminAndUserIsMunicipality()
        {
            SetClaims(Orgnr, "940330408");

            _register.accessId = 4;

            _accessControlService.AccessRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterIfRegisterAccessLevelIsMunicipalUserDokEditorAndDokAdminAndUserRoleIsDokEditor()
        {
            SetClaims(Role, DokEditor);

            _register.accessId = 4;

            _accessControlService.AccessRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterIfRegisterAccessLevelIsMunicipalUserDokEditorAndDokAdminAndUserRoleIsDokAdmin()
        {
            SetClaims(Role, DokAdmin);

            _register.accessId = 4;

            _accessControlService.AccessRegister(_register).Should().BeTrue();
        }

        // TODO teste at bruker er eier av registere...

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
