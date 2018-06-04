using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;
using FluentAssertions;
using Helpers;
using Kartverket.Register.Controllers;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Moq;
using Xunit;

namespace Kartverket.Register.Tests.Controllers
{
    public class RegistersControllerTest
    {
        public static Models.Register _register = new Models.Register { name = "RegisterName" };
        public static FilterParameters _filter = new FilterParameters();
        public static RegisterV2ViewModel _viewModel = new RegisterV2ViewModel(_register);


        private static Mock<IRegisterService> CreateRegisterServiceMock()
        {
            var registerServiceMock = new Mock<IRegisterService>();
            var registerList = new List<Models.Register>
            {
                new Models.Register()
            };
            
            registerServiceMock.Setup(m => m.GetRegisters()).Returns(registerList);
            registerServiceMock.Setup(c => c.ContentNegotiation(null));
            return registerServiceMock;
            
        }

        private static Mock<IRegisterItemService> CreateRegisterItemMock()
        {
            var registerItemServiceMock = new Mock<IRegisterItemService>();
            var registerItems = new List<RegisterItem>();
            var registerItemsV2ViewModel = new List<RegisterItemV2ViewModel>();
            registerItemServiceMock.Setup(m => m.OrderBy(registerItems, null)).Returns(registerItems);
            registerItemServiceMock.Setup(m => m.OrderBy(registerItemsV2ViewModel, null)).Returns(registerItemsV2ViewModel);
            return registerItemServiceMock;
        }

        private static Mock<IAccessControlService> CreateAccessControlServiceMock()
        {
            var accessControlServiceMock = new Mock<IAccessControlService>();
            accessControlServiceMock.Setup(m => m.AccessViewModel(_viewModel)).Returns(new AccessViewModel());
            return accessControlServiceMock;
        }

        /*
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

        [Fact]
        public void RegisterDetailsShouldReturnHttpNotFoundWhenRegisterIsNull()
        {
            var registerService = CreateRegisterServiceMock();
            registerService.Setup(r => r.GetRegister(null, null));

            var controller = new RegistersController(null, null, null, null, null, registerService.Object, null, null, null, null, null);

            var result = controller.Details(null, null, null, null, null, null, _filter) as ViewResult;
            result.Should().BeNull();
        }

        [Fact]
        public void RegisterDetailsShouldReturnDetailPageIfRegisterIsNotNull()
        {
            var registerService = CreateRegisterServiceMock();
            var registerItemService = CreateRegisterItemMock();
            var accessControlService = CreateAccessControlServiceMock();
            registerService.Setup(r => r.GetRegister(null, "RegisterName")).Returns(_register);
            registerItemService.Setup(m => m.GetMunicipalityOrganizationByNr(_viewModel.MunicipalityCode));


            var controller = new RegistersController(null, null, registerItemService.Object, null, null, registerService.Object, accessControlService.Object, null, null, null, null);

            var result = controller.Details(null,null, "RegisterName", null, null, null, _filter) as ViewResult;
            result.Should().NotBeNull();
        }


    }
}