using System.Web.Http.Results;
using FluentAssertions;
using Kartverket.Register.Controllers;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Moq;
using Xunit;

namespace Kartverket.Register.Tests.Controllers
{
    class OrganizationsApiControllerTest
    {
        private const string OrganizationName = "dummy organization";
        private const string OrganizationNumber = "123456";
        private const string OrganizationLogoFilename = "test.png";
        private const string OrganizationLogoLargeFilename = "testLarge.png";
        private const string OrganizationShortName = "IFSL";
        //private string LocationUrl = System.Configuration.ConfigurationSettings.AppSettings["RegistryUrl"] + "data/";

        private readonly Organization _organization = new Organization { name = OrganizationName, number = OrganizationNumber, logoFilename = OrganizationLogoFilename, largeLogo = OrganizationLogoLargeFilename, shortname = OrganizationShortName };

        [Fact]
        public void ShouldReturnHttpNotFoundWhenOrganizationIsNotFoundByName()
        {
            var service = new Mock<IOrganizationService>();

            var controller = new OrganizationsApiController(service.Object);

            var result = controller.GetOrganizationByName("does not exist") as NotFoundResult;

            result.Should().NotBeNull();
        }

        [Fact]
        public void ShouldReturnOrganizationByName()
        {
            var service = new Mock<IOrganizationService>();
            service.Setup(s => s.GetOrganizationByName(OrganizationName)).Returns(_organization);
            var controller = new OrganizationsApiController(service.Object);
            //controller.Url = CreateMockUrlHelper();

            var result = controller.GetOrganizationByName(OrganizationName) as OkNegotiatedContentResult<Models.Api.Organization>;

            ShouldReturnOrganization(result);
        }

        [Fact]
        public void ShouldReturnHttpNotFoundWhenOrganizationIsNotFoundByNumber()
        {
            var service = new Mock<IOrganizationService>();

            var controller = new OrganizationsApiController(service.Object);

            var result = controller.GetOrganizationByNumber("12345678") as NotFoundResult;

            result.Should().NotBeNull();
        }

        [Fact]
        public void ShouldReturnOrganizationByNumber()
        {
            var service = new Mock<IOrganizationService>();
            service.Setup(s => s.GetOrganizationByNumber(OrganizationNumber)).Returns(_organization);
            var controller = new OrganizationsApiController(service.Object);
            //controller.Url = CreateMockUrlHelper();

            var result = controller.GetOrganizationByNumber(OrganizationNumber) as OkNegotiatedContentResult<Models.Api.Organization>;

            ShouldReturnOrganization(result);
        }


        
        private static void ShouldReturnOrganization(OkNegotiatedContentResult<Models.Api.Organization> result)
        {
            result.Should().NotBeNull();
            result.Content.Should().NotBeNull();
            result.Content.Name.Should().Be(OrganizationName);
            result.Content.Number.Should().Be(OrganizationNumber);
            result.Content.ShortName.Should().Be(OrganizationShortName);
            //result.Content.LogoUrl.Should().Be(LocationUrl + Organization.DataDirectory + OrganizationLogoFilename);
            //result.Content.LogoLargeUrl.Should().Be(LocationUrl + Organization.DataDirectory + OrganizationLogoLargeFilename);
        }

        //private static UrlHelper CreateMockUrlHelper()
        //{
        //    var mockUrlHelper = new Mock<UrlHelper>();
        //    mockUrlHelper.Setup(x => x.Content(It.IsAny<string>())).Returns(LocationUrl);
        //    return mockUrlHelper.Object;
        //}
    }
}
