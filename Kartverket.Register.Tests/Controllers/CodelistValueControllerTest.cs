using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using FluentAssertions;
using Kartverket.Register.Controllers;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Moq;
using Xunit;
using RedirectResult = System.Web.Mvc.RedirectResult;

namespace Kartverket.Register.Tests.Controllers
{
    public class CodelistValueControllerTest
    {
        public static Models.Register Register;
        public Mock<IRegisterService> RegisterService;
        public Mock<IRegisterItemService> RegisterItemService;
        public Mock<IAccessControlService> AccessControlService;

        public CodelistValueControllerTest()
        {
            Register = CreateRegister();
            RegisterService = CreateRegisterServiceMock(); ;
            RegisterItemService = CreateRegisterItemServiceMock(); ;
            AccessControlService = CreateAccessControlServiceMock();
        }

        

        // IMPORT

        [Fact]
        public void GetHttpNotFoundIfRegisterIsNullWhenTryingToGetImportView()
        {
            RegisterService.Setup(r => r.GetRegister(null, "null")).Returns(Register);

            var controller = new CodelistValuesController(null, RegisterService.Object, null, null, null);
            var result = controller.Import("", null) as HttpNotFoundResult;
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public void GetImportViewIfRegisterIsNotNull()
        {
            RegisterService.Setup(r => r.GetRegister(null, "testregister")).Returns(Register);

            var controller = new CodelistValuesController(null, RegisterService.Object, AccessControlService.Object, null, null);
            var result = controller.Import("testregister", null) as ViewResult;
            result.Should().NotBeNull();
        }

        [Fact]
        public void ImportCodelistShouldReturnHttpNotFoundIfRegisterIsNull()
        {
            var controller = new CodelistValuesController(null, RegisterService.Object, AccessControlService.Object, null, null);
            var result = controller.Import(null, null, null, null) as HttpNotFoundResult;
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public void ImportCodelistShouldReturnToViewIfCsvFileIsNull()
        {
            var controller = new CodelistValuesController(RegisterItemService.Object, RegisterService.Object, AccessControlService.Object, null, null);
            var result = controller.Import(null, "testregister", null, null) as ViewResult;
            result.Should().NotBeNull();
        }

        [Fact]
        public void ImportCodelistShouldSetModelErrorIfFileContentIsNotcsvContent()
        {
            var file = new Mock<HttpPostedFileBase>();
            file.Setup(f => f.ContentType).Returns("WrongContentType");
            var controller = new CodelistValuesController(RegisterItemService.Object, RegisterService.Object, AccessControlService.Object, null, null);
            controller.Import(file.Object, "testregister", null, null);
            var modelstate = controller.ModelState;
            modelstate.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage.Should().Be("Filen har feil innhold!");
        }

        [Fact]
        public void ImportCodelistShouldReturnToRegisterUrlAfterImport()
        {
            var file = new Mock<HttpPostedFileBase>();
            file.Setup(f => f.ContentType).Returns("text/csv");
            var controller = new CodelistValuesController(RegisterItemService.Object, RegisterService.Object, AccessControlService.Object, null, null);
            var result = controller.Import(file.Object, "testregister", null, null) as RedirectResult;
            result.Url.Should().Be(Register.GetObjectUrl());
        }




        // *** HJELPEMETODER

        private static Mock<IAccessControlService> CreateAccessControlServiceMock()
        {
            var accessControlServiceMock = new Mock<IAccessControlService>();
            accessControlServiceMock.Setup(m => m.Access(Register)).Returns(true);
            return accessControlServiceMock;
        }

        private Mock<IRegisterService> CreateRegisterServiceMock()
        {
            var registerServiceMock = new Mock<IRegisterService>();
            registerServiceMock.Setup(r => r.GetRegister(null, "testregister")).Returns(Register);
            return registerServiceMock;
        }

        private Mock<IRegisterItemService> CreateRegisterItemServiceMock()
        {
            var registerItemServiceMock = new Mock<IRegisterItemService>();
            return registerItemServiceMock;
        }

        private Models.Register CreateRegister(string name = "testregister")
        {
            return new Models.Register()
            {
                name = name,
                seoname = RegisterUrls.MakeSeoFriendlyString(name)
            };
        }
    }
}
