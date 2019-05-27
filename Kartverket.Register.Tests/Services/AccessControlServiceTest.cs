using System;
using Kartverket.Register.Services;
using FluentAssertions;
using System.Security.Claims;
using System.Threading;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Geonorge.AuthLib.Common;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using Moq;
using Xunit;

namespace Kartverket.Register.Tests.Services
{
    public class AccessControlServiceTest
    {
        private const string Editor = GeonorgeRoles.MetadataEditor;
        private const string Admin = GeonorgeRoles.MetadataAdmin;
        private const string DokEditor = GeonorgeRoles.DokEditor;
        private const string DokAdmin = GeonorgeRoles.DokAdmin;
        private const string Role = "role";
        private const string Orgnr = GeonorgeClaims.OrganizationOrgnr;
        private const string Org = GeonorgeClaims.OrganizationName;
        private readonly Models.Register _register;
        private readonly Document _document;
        private readonly Dataset _dataset;
        private readonly FilterParameters _filter = null;
        private readonly Organization _organization;
        private readonly Organization _municipality;
        private readonly AccessControlService _accessControlService;

        public AccessControlServiceTest()
        {
            var dbContextMock = CreateTestDbContext(OrganizationList());
            _accessControlService = new AccessControlService(dbContextMock);

            Thread.CurrentPrincipal = null;
            _register = new Models.Register();
            _organization = CreateOrganization();
            _document = CreateDocument();
            _dataset = CreateDataset();
            _municipality = CreateMunicipality();
        }


        // Test Role

        [Fact]
        public void MunicipalityShouldReturnMunicipalCode()
        {
            SetClaims(Orgnr, "840098222");

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


        // Test Access register

        [Fact]
        public void AccessRegisterIfUserIsAdmin()
        {
            SetClaims(Role, Admin);
            _accessControlService.HasAccessToRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterIfContainedItemClassIsNotCodelistValue()
        {
            SetClaims(Role, Editor);

            _register.accessId = 2;
            _register.containedItemClass = "Organization";

            _accessControlService.HasAccessToRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterIfUserIsRegisterOwner()
        {
            SetClaims(Role, Editor, Org, "Kartverket");

            _register.accessId = 2;
            _register.containedItemClass = "CodelistValue";
            _register.owner = _organization;

            _accessControlService.HasAccessToRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterIfRegisterAccessLevelIsMunicipalUserDokEditorAndDokAdminAndUserIsMunicipality()
        {
            SetClaims(Orgnr, "840098222");

            _register.accessId = 4;

            _accessControlService.HasAccessToRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterIfRegisterAccessLevelIsMunicipalUserDokEditorAndDokAdminAndUserRoleIsDokEditor()
        {
            SetClaims(Role, DokEditor);

            _register.accessId = 4;

            _accessControlService.HasAccessToRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterIfRegisterAccessLevelIsMunicipalUserDokEditorAndDokAdminAndUserRoleIsDokAdmin()
        {
            SetClaims(Role, DokAdmin);

            _register.accessId = 4;

            _accessControlService.HasAccessToRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessEditRegisterWhenUserIsAdminAndRegisterIsNotAlertRegister()
        {
            SetClaims(Role, Admin);
            var registerViewModel = new RegisterV2ViewModel(_register, _filter);
            _accessControlService.EditRegister(registerViewModel).Should().BeTrue();
        }

        [Fact]
        public void NotAccessEditRegisterWhenUserIsAdminAndRegisterIsAlertRegister()
        {
            SetClaims(Role, Admin);
            _register.systemId = Guid.Parse("0f428034-0b2d-4fb7-84ea-c547b872b418");
            var registerViewModel = new RegisterV2ViewModel(_register, _filter);
            _accessControlService.EditRegister(registerViewModel).Should().BeFalse();
        }

        [Fact]
        public void AccessAddToRegisterWhenRegisterIsDokMunicipalAndMunicipalityIsNotNull()
        {
            SetClaims(Role, Admin);
            _register.systemId = Guid.Parse(GlobalVariables.DokMunicipalRegistryId);
            var registerViewModel = new RegisterV2ViewModel(_register, _filter) {Municipality = _municipality};
            _accessControlService.AddToRegister(registerViewModel).Should().BeTrue();
        }
        [Fact]
        public void NotAccessAddToRegisterWhenRegisterIsDokMunicipalAndMunicipalityIsNull()
        {
            SetClaims(Role, Admin);
            _register.systemId = Guid.Parse(GlobalVariables.DokMunicipalRegistryId);
            var registerViewModel = new RegisterV2ViewModel(_register, _filter);
            _accessControlService.AddToRegister(registerViewModel).Should().BeFalse();
        }

        [Fact]
        public void AccessAddToRegisterWhenRegisterIsNotDokMunicipal()
        {
            SetClaims(Role, Admin);
            var registerViewModel = new RegisterV2ViewModel(_register, _filter);
            _accessControlService.AddToRegister(registerViewModel).Should().BeTrue();
        }

        [Fact]
        public void AccessEditRegisterItemListWhenMunicipalityIsNotNull()
        {
            SetClaims(Role, Admin);
            var registerViewModel = new RegisterV2ViewModel(_register, _filter) {Municipality = _municipality};
            _accessControlService.EditRegisterItemsList(registerViewModel).Should().BeTrue();
        }

        // Test access registerItem
        [Fact]
        public void AccessRegisterItemIfUserIsAdmin()
        {
            SetClaims(Role, Admin);
            _accessControlService.AccessRegisterItem(_document).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterItemIfUserIsItemOwner()
        {
            SetClaims(Role, Editor, Orgnr, "971040238", Org, "Kartverket");
            var registerItem = new CodelistValue {register = _register, submitter = _organization};
            _accessControlService.AccessRegisterItem(registerItem).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterItemIfUserIsRegisterOwnerAndNotItemOwner()
        {
            SetClaims(Role, Editor, Orgnr, "971040238", Org, "Kartverket");
            var registerItem = new CodelistValue {register = _register};
            registerItem.register.owner = _organization;
            registerItem.submitter = new Organization() { name = "Norges geologiske undersøkelse" };

            _accessControlService.AccessRegisterItem(registerItem).Should().BeTrue();
        }

        // Test access registerItem document
        [Fact]
        public void AccessRegisterItemDocumentIfUserIsItemOwner()
        {
            SetClaims(Role, Editor, Orgnr, "971040238", Org, "Kartverket");
            _accessControlService.AccessRegisterItem(_document).Should().BeTrue();
        }

        [Fact]
        public void NotAccessRegisterItemDocumentIfUserIsNotItemOwner()
        {
            SetClaims(Role, Editor, Orgnr, "970188290", Org, "Norges geologiske undersøkelse");
            _accessControlService.AccessRegisterItem(_document).Should().BeFalse();
        }


        // Access register item dataset
        [Fact]
        public void AccessRegisterItemDatasetIfDatasetTypeIsKommunaltAndUserIsItemOwner()
        {
            SetClaims(Role, DokEditor, Orgnr, "971040238", Org, "Kartverket");
            _register.accessId = 4;
            _accessControlService.AccessRegisterItem(_dataset).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterItemDatasetIfDatasetTypeIsKommunaltAndUserIsDokAdminAndNotOwner()
        {
            SetClaims(Role, DokAdmin, Orgnr, "970188290", Org, "Norges geologiske undersøkelse");
            _register.accessId = 4;
            _accessControlService.AccessRegisterItem(_dataset).Should().BeTrue();
        }

        [Fact]
        public void NotAccessRegisterItemDatasetIfDatasetTypeIsNotKommunalt()
        {
            SetClaims(Role, DokEditor, Orgnr, "971040238", Org, "Kartverket");
            _dataset.DatasetType = "";
            _register.accessId = 2;
            _accessControlService.AccessRegisterItem(_dataset).Should().BeFalse();
        }


        [Fact]
        public void AccessRegisterItemIfUserIsNotItemOwnerAndUserIsRegisterOwner()
        {
            SetClaims(Role, Editor, Orgnr, "970188290", Org, "Norges geologiske undersøkelse");
            var codelistValue = CreateCodelistValue();
            codelistValue.Register.owner = CreateOrganization("Norges geologiske undersøkelse");
            codelistValue.Register.accessId = 2;
            _accessControlService.HasAccessTo(codelistValue).Should().BeTrue();
        }

        // User name and claims

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
            SetClaims(Orgnr, "840098222");

            _accessControlService.IsMunicipalUser().Should().BeTrue();
        }

        [Fact]
        public void ReturnTrueIfUserIsSelectedMunicipality()
        {
            SetClaims(Orgnr, "840098222");
            _accessControlService.UserIsSelectedMunicipality("1622").Should().BeTrue();
        }

        [Fact]
        public void ReturnFalseIfUserIsNotSelectedMunicipality()
        {
            SetClaims(Orgnr, "840098222");
            _accessControlService.UserIsSelectedMunicipality("1212").Should().BeFalse();
        }


        // Help methods

        private static void SetClaims(params string[] typeAndValues)
        {
            var claims = new List<Claim>();
            for (int i = 0; i < typeAndValues.Length; i += 2)
            {
                var name = typeAndValues[i];
                var value = typeAndValues[i + 1];

                if (name == Role)
                {
                    name = GeonorgeAuthorizationService.ClaimIdentifierRole; // use claim name for roles from auth lib to enable use of IsInRole()-method
                }

                claims.Add(new Claim(name, value));
            }

            


            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            Thread.CurrentPrincipal = claimsPrincipal;
        }

        private Document CreateDocument()
        {
            var document = new Document {register = _register};
            document.register.accessId = 2;
            document.documentowner = _organization;
            document.statusId = "Submitted";
            return document;
        }

        private Dataset CreateDataset()
        {
            var dataset = new Dataset {register = _register};
            dataset.register.accessId = 2;
            dataset.datasetowner = _organization;
            dataset.DatasetType = "Kommunalt";
            return dataset;
        }

        private Organization CreateMunicipality()
        {
            return new Organization
            {
                name = "Agdenes kommune",
                MunicipalityCode = "1622",
                number = "840098222"
            };
        }

        private RegisterItemV2ViewModel CreateCodelistValue()
        {
            var codelistValue = new RegisterItemV2ViewModel {Register = CreateCodelist(), Owner = _organization};
            return codelistValue;
        }

        private Models.Register CreateCodelist(Organization owner = null)
        {
            return new Models.Register
            {
                owner = owner != null ? _organization : owner,
                containedItemClass = "CodelistValue"
            };
        }

        private Organization CreateOrganization(string ownerName = "Kartverket")
        {
            return new Organization { name = ownerName };
        }

        private IEnumerable<Organization> OrganizationList()
        {
            return new List<Organization>
            {
                new Organization {name = "Kartverket", number = "971040238"},
                new Organization {name = "Norges geologiske undersøkelse", number = "970188290"},
                new Organization {name = "Agdenes kommune", number = "840098222", MunicipalityCode = "1622"}
            };
        }

        private RegisterDbContext CreateTestDbContext(IEnumerable<Organization> organizationList)
        {
            var dataOrganization = organizationList.AsQueryable();

            var mockSetOrganizations = new Mock<DbSet<Organization>>();
            mockSetOrganizations.As<IQueryable<Organization>>().Setup(m => m.Provider).Returns(dataOrganization.Provider);
            mockSetOrganizations.As<IQueryable<Organization>>().Setup(m => m.Expression).Returns(dataOrganization.Expression);
            mockSetOrganizations.As<IQueryable<Organization>>().Setup(m => m.ElementType).Returns(dataOrganization.ElementType);
            mockSetOrganizations.As<IQueryable<Organization>>().Setup(m => m.GetEnumerator()).Returns(dataOrganization.GetEnumerator());

            var mockContext = new Mock<RegisterDbContext>();
            mockContext.Setup(c => c.Organizations).Returns(mockSetOrganizations.Object);

            return mockContext.Object;
        }
    }
}
