﻿using System;
using Kartverket.Register.Models;
using NUnit.Framework;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services.Register;
using Moq;
using Kartverket.Register.Controllers;
using Kartverket.Register.Services.RegisterItem;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentAssertions;
using Kartverket.Register.Services;

namespace Kartverket.Register.Tests.Controllers
{
    class DatasetsControllerTest
    {

        // *** CREATE DATASET

        [Test]
        public void GetCreateShouldReturnViewWhenUserHaveAccess()
        {
            Dataset dataset = new Dataset();
            dataset.register = NewRegister("Det offentlige kartgrunnlaget");

            var registerService = new Mock<IRegisterService>();
            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();
            registerService.Setup(r => r.GetRegister(null, dataset.register.seoname)).Returns(dataset.register);
            accessControlService.Setup(a => a.Access(It.IsAny<Dataset>())).Returns(true);
            registerItemService.Setup(s => s.GetThemeGroupSelectList("ThemeGroup")).Returns(NewList());

            var controller = CreateController(registerService.Object, registerItemService.Object, accessControlService.Object);
            var result = controller.Create(dataset.register.seoname, null) as ViewResult;
            Dataset resultDataset = (Dataset)result.Model;

            result.Should().NotBeNull();
            resultDataset.register.name.Should().Be(dataset.register.name);
        }

        [Test]
        public void GetCreateShouldReturnHttpNotFoundWhenRegisterIsNull()
        {
            Dataset dataset = new Dataset();

            var registerService = new Mock<IRegisterService>();
            registerService.Setup(r => r.GetRegister(null, null)).Returns(dataset.register);

            var controller = CreateController(registerService.Object, null, null);
            var result = controller.Create(null, null) as ViewResult;

            result.Should().BeNull();
        }

        [Test]
        public void PostCreateShouldReturnViewWhenUuidIsNotNull()
        {
            Dataset dataset = NewDataset("datasett Navn");
            var registerService = new Mock<IRegisterService>();
            var registerItemService = new Mock<IRegisterItemService>();
            registerService.Setup(r => r.GetRegister(null, dataset.register.seoname)).Returns(dataset.register);
            registerItemService.Setup(s => s.GetThemeGroupSelectList("ThemeGroup")).Returns(NewList());
            registerItemService.Setup(s => s.GetDokStatusSelectList("ThemeGroup")).Returns(NewList());
            registerItemService.Setup(s => s.GetOwnerSelectList(Guid.NewGuid())).Returns(NewList());
            registerItemService.Setup(s => s.GetRegisterSelectList(Guid.NewGuid())).Returns(NewList());
            registerItemService.Setup(s => s.GetSubmitterSelectList(Guid.NewGuid())).Returns(NewList());
            var controller = CreateController(registerService.Object, registerItemService.Object, null);
            var result = controller.Create(dataset, dataset.register.seoname, "123", null, null) as ViewResult;

            result.Should().NotBeNull();
        }

        [Test]
        public void PostCreateShouldReturnRegisterItemDetailsPage()
        {
            Dataset dataset = NewDataset("Navn på datasett");

            var registerService = new Mock<IRegisterService>();
            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();
            registerService.Setup(r => r.GetRegister(null, dataset.register.seoname)).Returns(dataset.register);
            accessControlService.Setup(a => a.Access(It.IsAny<Dataset>())).Returns(true);
            registerItemService.Setup(v => v.validateName(It.IsAny<Dataset>())).Returns(true);
            accessControlService.Setup(a => a.GetSecurityClaim("organization")).Returns(dataset.submitter.seoname);
            registerService.Setup(o => o.GetOrganization(dataset.submitter.seoname)).Returns(dataset.submitter);

            var controller = CreateController(registerService.Object, registerItemService.Object, accessControlService.Object);
            var result = controller.Create(dataset, dataset.register.seoname, null, null, null) as ActionResult;
            
            result.Should().NotBeNull();
        }


        // *** EDIT DATASET

        [Test]
        public void GetEditShouldReturnViewWhenUserHaveAccess()
        {
            Dataset dataset = NewDataset("Test Datasett");

            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();

            registerItemService.Setup(r => r.GetRegisterItem(null, dataset.register.seoname, dataset.seoname, dataset.versionNumber)).Returns(dataset);
            accessControlService.Setup(a => a.Access(It.IsAny<Dataset>())).Returns(true);

            var controller = CreateController(null, registerItemService.Object, accessControlService.Object);
            var result = controller.Edit(dataset.register.seoname, dataset.seoname, null) as ViewResult;
            Dataset resultDataset = (Dataset)result.Model;

            result.Should().NotBeNull();
            resultDataset.register.name.Should().Be(dataset.register.name);
        }

        [Test]
        public void GetEditShouldReturnHttpNotFoundWhenRegisterIdNull()
        {
            Dataset dataset = null;
            var registerItemService = new Mock<IRegisterItemService>();
            registerItemService.Setup(r => r.GetRegisterItem(null, null, null, 1)).Returns(dataset);

            var controller = CreateController(null, registerItemService.Object, null);
            var result = controller.Edit(null, null, null) as ViewResult;

            result.Should().BeNull();
        }

        [Test]
        public void PostEditShouldReturnViewWhenUuidIsNotNull()
        {
            Dataset dataset = NewDataset("Test Datasett");
            var registerItemService = new Mock<IRegisterItemService>();
            registerItemService.Setup(r => r.GetRegisterItem(null, null, null, 1)).Returns(dataset);

            var controller = CreateController(null, registerItemService.Object, null);
            var result = controller.Edit(dataset, null, null, "123", false, null, null) as ViewResult;

            result.Should().NotBeNull();
        }

        [Test]
        public void PostEditShouldReturnRegisterItemDetailsPageWhenOriginalDatasetIsNotNull()
        {
            Dataset dataset = NewDataset("Navn på datasett");

            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();
            var registerService = new Mock<IRegisterService>();
            registerItemService.Setup(v => v.GetRegisterItem(null, dataset.register.seoname, dataset.seoname, dataset.versionNumber)).Returns(dataset);
            accessControlService.Setup(a => a.Access(It.IsAny<Dataset>())).Returns(true);
            registerItemService.Setup(v => v.validateName(It.IsAny<Dataset>())).Returns(true);
            accessControlService.Setup(a => a.GetSecurityClaim("organization")).Returns(dataset.submitter.seoname);
            registerService.Setup(o => o.GetOrganization(dataset.submitter.seoname)).Returns(dataset.submitter);

            var controller = CreateController(registerService.Object, registerItemService.Object, accessControlService.Object);
            var result = controller.Edit(dataset, dataset.register.seoname, dataset.seoname, null, false, null, null) as ActionResult;

            result.Should().NotBeNull();
        }

        // *** DELETE DATASET

        [Test]
        public void GetDeleteShouldReturnViewWhenUserHaveAccess()
        {
            Dataset dataset = NewDataset("Test Datasett");

            var accessControlService = new Mock<IAccessControlService>();
            var registerItemService = new Mock<IRegisterItemService>();

            registerItemService.Setup(r => r.GetRegisterItem(null, dataset.register.seoname, dataset.seoname, dataset.versionNumber)).Returns(dataset);
            accessControlService.Setup(a => a.Access(It.IsAny<Dataset>())).Returns(true);

            var controller = CreateController(null, registerItemService.Object, accessControlService.Object);
            var result = controller.Delete(dataset.register.seoname, dataset.seoname, null, null) as ViewResult;
            Dataset resultDataset = (Dataset)result.Model;

            result.Should().NotBeNull();
            resultDataset.register.name.Should().Be(dataset.register.name);
        }

        [Test]
        public void GetDeleteShouldReturnHttpNotFoundWhenRegisterIdNull()
        {
            Dataset dataset = null;
            var registerItemService = new Mock<IRegisterItemService>();
            registerItemService.Setup(r => r.GetRegisterItem(null, null, null, 1)).Returns(dataset);

            var controller = CreateController(null, registerItemService.Object, null);
            var result = controller.Delete(null, null, null, null) as ViewResult;

            result.Should().BeNull();
        }


        // *** HJELPEMETODER
        private DatasetsController CreateController(IRegisterService registerService, IRegisterItemService registerItemService, IAccessControlService accessControlService)
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
            dataset.versionNumber = 1;
            dataset.register = NewRegister("Det offentlige kartgrunnlaget");
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