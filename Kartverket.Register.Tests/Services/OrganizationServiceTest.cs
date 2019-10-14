using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FluentAssertions;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Moq;
using Xunit;

namespace Kartverket.Register.Tests.Services
{
    
    public class OrganizationServiceTest
    {
        private const string OrganizationName = "Institutt for skog og landskap";
        private const string OrganizationNumber = "1234567890";
        private const string OrganizationLogo = "logo.png";
        private const string OrganizationLogoLarge = "logoLarge.png";
        private const string OrganizationShortName = "IFSL";

        [Fact]
        public void ShouldReturnNullWhenOrganizationIsNotFoundByName()
        {
            var service = CreateService(new List<Organization>());

            Organization organization = service.GetOrganizationByName("skog og landskap");

            organization.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnOrganizationByName()
        {
            var service = CreateService(CreateOrganizationSkogOgLandskap());

            Organization organization = service.GetOrganizationByName(OrganizationName);

            ShouldBeSkogOgLandskap(organization);
        }

        private static void ShouldBeSkogOgLandskap(Organization organization)
        {
            organization.Should().NotBeNull();
            organization.name.Should().Be(OrganizationName);
            organization.number.Should().Be(OrganizationNumber);
            organization.logoFilename.Should().Be(OrganizationLogo);
            organization.largeLogo.Should().Be(OrganizationLogoLarge);
            organization.shortname.Should().Be(OrganizationShortName);
        }

        [Fact]
        public void ShouldReturnOrganizationByNumber()
        {
            var service = CreateService(CreateOrganizationSkogOgLandskap());

            Organization organization = service.GetOrganizationByNumber(OrganizationNumber);

            ShouldBeSkogOgLandskap(organization);
        }

        [Fact]
        public void ShouldReturnNullWhenOrganizationIsNotFoundByNumber()
        {
            var service = CreateService(CreateOrganizationSkogOgLandskap());

            Organization organization = service.GetOrganizationByNumber("56789");

            organization.Should().BeNull();
        }

        private OrganizationsService CreateService(IEnumerable<Organization> listOfOrganizations)
        {
            RegisterDbContext dbContextMock = CreateTestDbContext(listOfOrganizations);
            var service = new OrganizationsService(dbContextMock);
            return service;
        }

        private static IEnumerable<Organization> CreateOrganizationSkogOgLandskap()
        {
            var listOfOrganizations = new List<Organization>
            {
                new Organization
                {
                    name = OrganizationName,
                    number = OrganizationNumber,
                    logoFilename = OrganizationLogo,
                    largeLogo = OrganizationLogoLarge,
                    shortname = OrganizationShortName
                }
            };
            return listOfOrganizations;
        }


        private RegisterDbContext CreateTestDbContext(IEnumerable<Organization> organizationList)
        {
            var data = organizationList.AsQueryable();

            var mockSet = new Mock<DbSet<Organization>>();
            mockSet.As<IQueryable<Organization>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Organization>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Organization>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Organization>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<RegisterDbContext>();
            mockContext.Setup(c => c.Organizations).Returns(mockSet.Object);

            return mockContext.Object;
        }

    }
}
