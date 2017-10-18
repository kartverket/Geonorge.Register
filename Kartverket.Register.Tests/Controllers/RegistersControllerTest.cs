using System.Collections.Generic;
using System.Web.Mvc;
using FluentAssertions;
using Helpers;
using Kartverket.Register.Controllers;
using Kartverket.Register.Services.Register;
using Moq;
using Xunit;

namespace Kartverket.Register.Tests.Controllers
{
    public class RegistersControllerTest
    {
        /*
        private static Mock<IRegisterService> CreateRegisterServiceMock()
        {
            var registerServiceMock = new Mock<IRegisterService>();
            var registerList = new List<Models.Register>
            {
                new Models.Register()
            };
            registerServiceMock.Setup(m => m.GetRegisters()).Returns(registerList);
            return registerServiceMock;
        }

        [Fact]
        public void ShouldRemoveSessionSearchParamsOnIndexPage()
        {
            Mock<IRegisterService> registerServiceMock = CreateRegisterServiceMock();
            var controller = new RegistersController(null, registerServiceMock.Object, null);

            new MvcMockHelper().For(controller);

            controller.Session["sortingType"] = "blabla";

            controller.Index();

            controller.Session["sortingType"].Should().BeNull();
        }

        [Fact]
        public void ShouldReturnListOfRegistersOnIndexPage()
        {
            Mock<IRegisterService> registerServiceMock = CreateRegisterServiceMock();

            var controller = new RegistersController(null, registerServiceMock.Object, null);
            new MvcMockHelper().For(controller);

            var result = controller.Index() as ViewResult;

            var viewModel = result.Model as RegisterViewModel;

            viewModel.Items.Count.Should().Be(1);
        }
        */
    }
}