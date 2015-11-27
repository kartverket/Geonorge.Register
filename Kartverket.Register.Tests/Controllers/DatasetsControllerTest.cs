using System;
using Kartverket.Register.Models;
using NUnit.Framework;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services.Register;
using Moq;
using System.Web.Http.Results;
using Kartverket.Register.Controllers;
using Kartverket.Register.Services.RegisterItem;
using System.Net.Http;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentAssertions;
using Kartverket.Register.Services;

namespace Kartverket.Register.Tests.Controllers
{
    class DatasetsControllerTest
    {

        [Test]
        public void CreateShouldReturnViewWhenUserHaveAccess() {   
            Dataset dataset = new Dataset();
            dataset.register = NewRegister("Det offentlige kartgrunnlaget");

            var registerService = new Mock<IRegisterService>();
            var accessControlService = new Mock<IAccessControlService>();
            registerService.Setup(r => r.GetRegister(null, dataset.register.seoname)).Returns(dataset.register);
            accessControlService.Setup(a => a.Access(Moq.It.IsAny<Dataset>())).Returns(true);

            var controller = createController(registerService.Object, null, accessControlService.Object);
            var result = controller.Create(dataset.register.seoname, null) as ViewResult;
            Dataset resultDataset = (Dataset)result.Model;
            
            result.Should().NotBeNull();
            resultDataset.register.name.Should().Be(dataset.register.name);
        }

        [Test]
        public void CreateShouldReturnHttpNotFoundWhenRegisterIdNull()
        {
            Dataset dataset = new Dataset();
            //dataset.register = NewRegister("Det offentlige kartgrunnlaget");

            var registerService = new Mock<IRegisterService>();
            var accessControlService = new Mock<IAccessControlService>();
            registerService.Setup(r => r.GetRegister(null, null)).Returns(dataset.register);
            accessControlService.Setup(a => a.Access(Moq.It.IsAny<Dataset>())).Returns(false);

            var controller = createController(registerService.Object, null, accessControlService.Object);
            var result = controller.Create(null, null) as ViewResult;

            result.Should().BeNull();
        }

        private DatasetsController createController(IRegisterService registerService, IRegisterItemService registerItemService, IAccessControlService accessControlService)
        {
            var controller = new DatasetsController(registerItemService, registerService, accessControlService);
            return controller;
        }

        private Models.Register NewRegister(string name, int accessId = 2)
        {
            Models.Register register = new Models.Register();
            register.systemId = Guid.NewGuid();
            register.name = name;
            register.seoname = RegisterUrls.MakeSeoFriendlyString(register.name);
            register.description = "testbeskrivelse";
            register.owner = NewOrganization("Testorg");
            register.accessId = accessId;
            return register;
        }
        private Models.Organization NewOrganization(string name)
        {
            Models.Organization organization = new Models.Organization();
            organization.systemId = Guid.NewGuid();
            organization.name = name;
            organization.seoname = RegisterUrls.MakeSeoFriendlyString(organization.name);
            organization.description = "beskrivelse av organisasjon";
            return organization;
        }
    }
}
