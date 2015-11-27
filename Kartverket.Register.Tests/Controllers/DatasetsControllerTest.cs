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
        public void GetCreateShouldReturnViewWhenUserHaveAccess() {   
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
        public void GetCreateShouldReturnHttpNotFoundWhenRegisterIdNull()
        {
            Dataset dataset = new Dataset();

            var registerService = new Mock<IRegisterService>();
            var accessControlService = new Mock<IAccessControlService>();
            registerService.Setup(r => r.GetRegister(null, null)).Returns(dataset.register);
            accessControlService.Setup(a => a.Access(Moq.It.IsAny<Dataset>())).Returns(false);

            var controller = createController(registerService.Object, null, accessControlService.Object);
            var result = controller.Create(null, null) as ViewResult;

            result.Should().BeNull();
        }

        [Test]
        public void PostCreateShouldReturnViewWhenUuidIsNotNull()
        {
            Dataset dataset = new Dataset();
            var controller = createController(null, null, null);
            var result = controller.Create(dataset, null, "123", null, null) as ViewResult;

            result.Should().NotBeNull();
        }

        [Test]
        public void PostCreateShouldReturnRegisterItemDetails()
        {
            Dataset dataset = NewDataset("Navn på datasett");
            dataset.name = "navn på datasett";
            dataset.register = NewRegister("Det offentlige kartgrunnlaget");

            var registerService = new Mock<IRegisterService>();
            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();
            registerService.Setup(r => r.GetRegister(null, dataset.register.seoname)).Returns(dataset.register);
            accessControlService.Setup(a => a.Access(It.IsAny<Dataset>())).Returns(true);
            registerItemService.Setup(v => v.validateName(It.IsAny<Dataset>())).Returns(true);
            accessControlService.Setup(a => a.GetSecurityClaim("organization")).Returns(dataset.submitter.seoname);
            registerService.Setup(o => o.GetOrganization(dataset.submitter.seoname)).Returns(dataset.submitter);

            var controller = createController(registerService.Object, registerItemService.Object, accessControlService.Object);
            var result = controller.Create(dataset, dataset.register.seoname, null, null, null) as ActionResult;
           
            result.Should().NotBeNull();
        }

        private DatasetsController createController(IRegisterService registerService, IRegisterItemService registerItemService, IAccessControlService accessControlService)
        {
            var controller = new DatasetsController(registerItemService, registerService, accessControlService);
            return controller;
        }

        private Dataset NewDataset(string name)
        {
            Dataset dataset = new Dataset();
            dataset.systemId = Guid.NewGuid();
            dataset.name = name;
            dataset.seoname = RegisterUrls.MakeSeoFriendlyString(dataset.name);
            dataset.description = "testbeskrivelse";
            dataset.datasetowner = NewOrganization("Testorg");
            dataset.datasetownerId = dataset.datasetowner.systemId;
            dataset.submitter = dataset.datasetowner;
            dataset.submitterId = dataset.submitterId;
            return dataset;
        }

        private Models.Register NewRegister(string name, int accessId = 2)
        {
            Models.Register register = new Models.Register();
            register.systemId = Guid.NewGuid();
            register.name = name;
            register.seoname = RegisterUrls.MakeSeoFriendlyString(register.name);
            register.description = "testbeskrivelse";
            register.owner = NewOrganization("Kartverket");
            register.accessId = accessId;
            return register;
        }
        private Organization NewOrganization(string name)
        {
            Organization organization = new Organization();
            organization.systemId = Guid.NewGuid();
            organization.name = name;
            organization.seoname = RegisterUrls.MakeSeoFriendlyString(organization.name);
            organization.description = "beskrivelse av organisasjon";
            return organization;
        }
    }
}
