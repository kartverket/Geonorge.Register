using System;
using Kartverket.Register.Models;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services.Register;
using Moq;
using Kartverket.Register.Controllers;
using Kartverket.Register.Services.RegisterItem;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentAssertions;
using Kartverket.Register.Services;
using Xunit;
using Kartverket.Register.Services.Translation;

namespace Kartverket.Register.Tests.Controllers
{
    public class DatasetsControllerTest
    {

        // *** CREATE DATASET

        [Fact]
        public void GetCreateShouldReturnView()
        {
            Dataset dataset = new Dataset();
            dataset.register = NewRegister("DOK-statusregisteret", 1);

            var registerService = new Mock<IRegisterService>();
            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();
            registerService.Setup(r => r.GetRegister(null, dataset.register.seoname)).Returns(dataset.register);
            accessControlService.Setup(a => a.HasAccessTo(It.IsAny<Models.Register>())).Returns(true);
            registerItemService.Setup(s => s.GetThemeGroupSelectList("ThemeGroup")).Returns(NewList());

            var controller = CreateController(null, registerService.Object, registerItemService.Object, accessControlService.Object, null, null, null);
            var result = controller.Create(dataset.register.seoname, null) as ViewResult;
            Dataset resultDataset = (Dataset)result.Model;

            result.Should().NotBeNull();
            resultDataset.register.name.Should().Be(dataset.register.name);
        }

        [Fact]
        public void GetCreateShouldReturnHttpNotFoundWhenRegisterIsNull()
        {
            Dataset dataset = new Dataset();

            var registerService = new Mock<IRegisterService>();
            registerService.Setup(r => r.GetRegister(null, null)).Returns(dataset.register);

            var controller = CreateController(null, registerService.Object, null, null, null, null, null);
            var result = controller.Create(null, null) as ViewResult;

            result.Should().BeNull();
        }


        [Fact]
        public void PostCreateShouldReturnRegisterItemDetailsPag()
        {
            Dataset dataset = NewDataset("Navn på datasett");

            var registerService = new Mock<IRegisterService>();
            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();
            registerService.Setup(r => r.GetRegister(null, dataset.register.seoname)).Returns(dataset.register);
            accessControlService.Setup(a => a.HasAccessTo(It.IsAny<Models.Register>())).Returns(true);
            registerItemService.Setup(v => v.ItemNameIsValid(It.IsAny<Dataset>())).Returns(true);
            registerItemService.Setup(o => o.GetRegisterItemBySystemId(dataset.datasetownerId)).Returns(dataset.datasetowner);

            new AuthTestHelper().SetCurrentOrganization(dataset.submitter.seoname).Invoke();
            
            registerService.Setup(o => o.GetOrganizationByUserName()).Returns(dataset.submitter);

            var controller = CreateController(null, registerService.Object, registerItemService.Object, accessControlService.Object, null, null, null);
            var result = controller.Create(dataset.register.seoname, null) as ActionResult;

            result.Should().NotBeNull();
        }


        // *** EDIT DATASET

        [Fact]
        public void GetEditShouldReturnViewWhenUserHaveAccess()
        {
            Dataset dataset = NewDataset("Test Datasett");

            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();

            registerItemService.Setup(r => r.GetRegisterItem(null, dataset.register.seoname, dataset.seoname, 1, dataset.datasetowner.seoname)).Returns(dataset);
            accessControlService.Setup(a => a.HasAccessTo(It.IsAny<Dataset>())).Returns(true);

            var controller = CreateController(null,null, registerItemService.Object, accessControlService.Object, null, null, null);
            var result = controller.Edit(dataset.register.seoname, dataset.datasetowner.seoname, dataset.seoname, null) as ViewResult;
            Dataset resultDataset = (Dataset)result.Model;

            result.Should().NotBeNull();
            resultDataset.register.name.Should().Be(dataset.register.name);
        }

        [Fact]
        public void GetEditShouldReturnHttpNotFoundWhenRegisterIdNull()
        {
            Dataset dataset = null;
            var registerItemService = new Mock<IRegisterItemService>();
            registerItemService.Setup(r => r.GetRegisterItem(null, null, null, 1, null)).Returns(dataset);

            var controller = CreateController(null,null, registerItemService.Object, null, null, null, null);
            var result = controller.Edit(null, null, null, null) as ViewResult;

            result.Should().BeNull();
        }

        [Fact]
        public void PostEditShouldReturnViewWhenUuidIsNotNull()
        {
            Dataset dataset = NewDataset("Test Datasett");
            var registerItemService = new Mock<IRegisterItemService>();
            registerItemService.Setup(r => r.GetRegisterItem(null, null, null, 1, null)).Returns(dataset);

            var controller = CreateController(null, null, registerItemService.Object, null, null, null, null);
            var result = controller.Edit(dataset, null, null, null, "123", null, null, null) as ViewResult;

            result.Should().NotBeNull();
        }

        [Fact]
        public void PostEditShouldReturnRegisterItemDetailsPageWhenOriginalDatasetIsNotNull()
        {
            Dataset dataset = NewDataset("Navn på datasett");

            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();
            var registerService = new Mock<IRegisterService>();
            registerItemService.Setup(v => v.GetRegisterItem(null, dataset.register.seoname, dataset.seoname, 1, null)).Returns(dataset);
            accessControlService.Setup(a => a.HasAccessTo(It.IsAny<Dataset>())).Returns(true);
            registerItemService.Setup(v => v.ItemNameIsValid(It.IsAny<Dataset>())).Returns(true);

            new AuthTestHelper().SetCurrentOrganization(dataset.submitter.seoname).Invoke();

            registerItemService.Setup(a => a.GetRegisterItemBySystemId(dataset.datasetownerId)).Returns(dataset.datasetowner);
            registerService.Setup(o => o.GetOrganizationByUserName()).Returns(dataset.submitter);

            var datasetServiceMock = new Mock<IDatasetService>();
            datasetServiceMock.Setup(m => m.UpdateDataset(It.IsAny<Dataset>(), It.IsAny<Dataset>(), null)).Returns(dataset);

            var translationServiceMock = new Mock<ITranslationService>();

            var controller = CreateController(null, registerService.Object, registerItemService.Object, accessControlService.Object, null, translationServiceMock.Object, datasetServiceMock.Object);
            var result = controller.Edit(dataset, null, dataset.register.seoname, dataset.seoname, null, null, null, null) as ActionResult;

            result.Should().NotBeNull();
        }

        // *** DELETE DATASET

        [Fact]
        public void GetDeleteShouldReturnViewWhenUserHaveAccess()
        {
            Dataset dataset = NewDataset("Test Datasett");

            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();

            registerItemService.Setup(r => r.GetRegisterItem(null, dataset.register.seoname, dataset.seoname, dataset.versionNumber, null)).Returns(dataset);
            accessControlService.Setup(a => a.HasAccessTo(It.IsAny<Dataset>())).Returns(true);

            var controller = CreateController(null, null, registerItemService.Object, accessControlService.Object, null, null, null);
            var result = controller.Delete(dataset.register.seoname, dataset.seoname, null, null, null) as ViewResult;
            Dataset resultDataset = (Dataset)result.Model;

            result.Should().NotBeNull();
            resultDataset.register.name.Should().Be(dataset.register.name);
        }

        [Fact]
        public void GetDeleteShouldReturnHttpNotFoundWhenRegisterIdNull()
        {
            Dataset dataset = null;
            var registerItemService = new Mock<IRegisterItemService>();
            registerItemService.Setup(r => r.GetRegisterItem(null, null, null, 1, null)).Returns(dataset);

            var controller = CreateController(null,null, registerItemService.Object, null, null, null, null);
            var result = controller.Delete(null, null, null, null, null) as ViewResult;

            result.Should().BeNull();
        }


        // *** HJELPEMETODER
        private DatasetsController CreateController(RegisterDbContext dbContext, IRegisterService registerService,
            IRegisterItemService registerItemService, IAccessControlService accessControlService,
            IDatasetDeliveryService datasetDeliveryService, ITranslationService translationService,
            IDatasetService datasetService)
        {
            var controller = new DatasetsController(dbContext, registerItemService, registerService, accessControlService, datasetDeliveryService, translationService, datasetService, null);
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
            dataset.versionNumber = 1;
            dataset.register = NewRegister("DOK-statusregisteret");
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

        private SelectList NewList() {
            List<string> list = new List<string>();
            return new SelectList(list);
        }
    }
}
