using System;
using Kartverket.Register.Services;
using FluentAssertions;
using System.Security.Claims;
using System.Threading;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using Moq;
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
        private const string Org = "organization";
        private readonly Models.Register _register;
        private Document _document;
        private Dataset _dataset;
        private readonly Organization _organization;
        private readonly Organization _municipality;
        private readonly AccessControlService _accessControlService;


        public AccessControlServiceTest()
        {
            var dbContextMock = CreateTestDbContext(OrganizationList());
            _accessControlService = new AccessControlService(dbContextMock);

            Thread.CurrentPrincipal = null;
            _register = new Models.Register();
            _organization = new Organization { name = "Kartverket" };
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
            _accessControlService.AccessRegister(_register).Should().BeTrue();
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
            SetClaims(Role, Editor, "organization", "Kartverket");

            _register.accessId = 2;
            _register.containedItemClass = "CodelistValue";
            _register.owner = _organization;

            _accessControlService.AccessRegister(_register).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterIfRegisterAccessLevelIsMunicipalUserDokEditorAndDokAdminAndUserIsMunicipality()
        {
            SetClaims(Orgnr, "840098222");
            
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

        [Fact]
        public void AccessEditRegisterWhenUserIsAdminAndRegisterIsNotServiceAlertRegister()
        {
            SetClaims(Role, Admin);
            var registerViewModel = new RegisterV2ViewModel(_register);
            _accessControlService.EditRegister(registerViewModel).Should().BeTrue();
        }

        [Fact]
        public void NotAccessEditRegisterWhenUserIsAdminAndRegisterIsServiceAlertRegister()
        {
            SetClaims(Role, Admin);
            _register.systemId = Guid.Parse("0f428034-0b2d-4fb7-84ea-c547b872b418");
            var registerViewModel = new RegisterV2ViewModel(_register);
            _accessControlService.EditRegister(registerViewModel).Should().BeFalse();
        }

        [Fact]
        public void AccessAddToRegisterWhenRegisterIsDokMunicipalAndMunicipalityIsNotNull()
        {
            SetClaims(Role, Admin);
            _register.name = "Det offentlige kartgrunnlaget - Kommunalt";
            var registerViewModel = new RegisterV2ViewModel(_register);
            registerViewModel.Municipality = _municipality;
            _accessControlService.AddToRegister(registerViewModel).Should().BeTrue();
        }
        [Fact]
        public void NotAccessAddToRegisterWhenRegisterIsDokMunicipalAndMunicipalityIsNull()
        {
            SetClaims(Role, Admin);
            _register.name = "Det offentlige kartgrunnlaget - Kommunalt";
            var registerViewModel = new RegisterV2ViewModel(_register);
            _accessControlService.AddToRegister(registerViewModel).Should().BeFalse();
        }

        [Fact]
        public void AccessAddToRegisterWhenRegisterIsNotDokMunicipal()
        {
            SetClaims(Role, Admin);
            var registerViewModel = new RegisterV2ViewModel(_register);
            _accessControlService.AddToRegister(registerViewModel).Should().BeTrue();
        }

        [Fact]
        public void AccessEditRegisterItemListWhenMunicipalityIsNotNull()
        {
            SetClaims(Role, Admin);
            var registerViewModel = new RegisterV2ViewModel(_register);
            registerViewModel.Municipality = _municipality;
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
            var registerItem = new CodelistValue();
            registerItem.register = _register;
            registerItem.submitter = _organization;
            _accessControlService.AccessRegisterItem(registerItem).Should().BeTrue();
        }

        [Fact]
        public void AccessRegisterItemIfUserIsRegisterOwnerAndNotItemOwner()
        {
            SetClaims(Role, Editor, Orgnr, "971040238", Org, "Kartverket");
            var registerItem = new CodelistValue();
            registerItem.register = _register;
            registerItem.register.owner = _organization;
            registerItem.submitter = new Organization(){ name = "Norges geologiske undersøkelse"};

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
                claims.Add(new Claim(typeAndValues[i], typeAndValues[i + 1]));
            }

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            Thread.CurrentPrincipal = claimsPrincipal;
        }

        private Document CreateDocument()
        {
            var document = new Document();
            document.register = _register;
            document.register.accessId = 2;
            document.documentowner = _organization;
            return document;
        }

        private Dataset CreateDataset()
        {
            var dataset = new Dataset();
            dataset.register = _register;
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
