using System.Web.Http.Results;
using System.Web.Http.Routing;
using FluentAssertions;
using Kartverket.Register.Controllers;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Moq;
using NUnit.Framework;

namespace Kartverket.Register.Tests.Controllers
{
    class OrganizationsApiControllerTest
    {
        private const string OrganizationName = "dummy organization";
        private const string OrganizationNumber = "123456";
        private const string OrganizationLogoFilename = "test.png";
        private const string LocationUrl = "http://example.com/data/";

        private readonly Organization _organization = new Organization { name = OrganizationName, number = OrganizationNumber, logoFilename = OrganizationLogoFilename };

        [Test]
        public void ShouldReturnHttpNotFoundWhenOrganizationIsNotFoundByName()
        {
            var service = new Mock<IOrganizationService>();

            var controller = new OrganizationsApiController(service.Object);

            var result = controller.GetOrganizationByName("does not exist") as NotFoundResult;

            result.Should().NotBeNull();
        }

        [Test]
        public void ShouldReturnOrganizationByName()
        {
            var service = new Mock<IOrganizationService>();
            service.Setup(s => s.GetOrganizationByName(OrganizationName)).Returns(_organization);
            var controller = new OrganizationsApiController(service.Object);
            controller.Url = CreateMockUrlHelper();

            var result = controller.GetOrganizationByName(OrganizationName) as OkNegotiatedContentResult<Models.Api.Organization>;

            ShouldReturnOrganization(result);
        }

        [Test]
        public void ShouldReturnHttpNotFoundWhenOrganizationIsNotFoundByNumber()
        {
            var service = new Mock<IOrganizationService>();

            var controller = new OrganizationsApiController(service.Object);

            var result = controller.GetOrganizationByNumber("12345678") as NotFoundResult;

            result.Should().NotBeNull();
        }

        [Test]
        public void ShouldReturnOrganizationByNumber()
        {
            var service = new Mock<IOrganizationService>();
            service.Setup(s => s.GetOrganizationByNumber(OrganizationNumber)).Returns(_organization);
            var controller = new OrganizationsApiController(service.Object);
            controller.Url = CreateMockUrlHelper();

            var result = controller.GetOrganizationByNumber(OrganizationNumber) as OkNegotiatedContentResult<Models.Api.Organization>;

            ShouldReturnOrganization(result);
        }
        
        private static void ShouldReturnOrganization(OkNegotiatedContentResult<Models.Api.Organization> result)
        {
            result.Should().NotBeNull();
            result.Content.Should().NotBeNull();
            result.Content.Name.Should().Be(OrganizationName);
            result.Content.Number.Should().Be(OrganizationNumber);
            result.Content.LogoUrl.Should().Be(LocationUrl + Organization.DataDirectory + OrganizationLogoFilename);
        }

        private static UrlHelper CreateMockUrlHelper()
        {
            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Content(It.IsAny<string>())).Returns(LocationUrl);
            return mockUrlHelper.Object;
        }
    }
}
