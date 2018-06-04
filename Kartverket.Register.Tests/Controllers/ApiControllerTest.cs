﻿using System;
using FluentAssertions;
using Kartverket.Register.Controllers;
using Kartverket.Register.Models;
using Moq;
using Kartverket.Register.Services.Register;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Results;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services.RegisterItem;
using Xunit;
using Kartverket.Register.Models.Translations;

namespace Kartverket.Register.Tests.Controllers
{    
    public class ApiControllerTest
    {
        string url = "https://register.geonorge.no/";

        [Fact]
        public void GetRegisters()
        {
            Models.Register r1 = NewRegister("Navn1");
            Models.Register r2 = NewRegister("Navn2");
            Models.Register r3 = NewRegister("Navn3");
            
            List<Models.Register> registers = new List<Models.Register> { r1, r2, r3 };

            var registerService = new Mock<IRegisterService>();           
            registerService.Setup(r => r.GetRegisters()).Returns(registers);
            var controller = createController(url, registerService.Object, null);

            var result = controller.GetRegisters() as OkNegotiatedContentResult<List<Models.Api.Register>>;

            List<Models.Api.Register> actualListOfRegisters = result.Content;
            actualListOfRegisters.Count.Should().Be(3); 
        }

        [Fact]
        public void GetRegistersByName()
        {
            //Testdata
            Models.Register r1 = NewRegister("Navn");
            Models.Register r2 = NewRegister("Navn2");
            Models.Register r3 = NewRegister("Navn3");

            List<Models.Register> registers = new List<Models.Register> { r1, r2, r3 };

            var registerService = new Mock<IRegisterService>();
            registerService.Setup(s => s.GetRegisterByName(r1.seoname)).Returns(r1);

            var controller = createController(url, registerService.Object, null);
            var result = controller.GetRegisterByName(r1.seoname) as OkNegotiatedContentResult<Models.Api.Register>;

            Models.Api.Register registerApi = result.Content;
            registerApi.label.Should().Be("Navn");
        }

        [Fact]
        public void GetSubregisterByName()
        {
            //Testdata
            Models.Register r1 = NewRegister("RegisterName");
            Models.Register r2 = NewRegister("ParentName");
            Models.Organization organization = NewOrganization("Kartverket");
            r2.owner = organization;
            r1.parentRegisterId = r2.systemId;
            r1.parentRegister = r2;

            List<Models.Register> registers = new List<Models.Register> { r1, r2 };
            Models.Register register = new Models.Register();

            var registerService = new Mock<IRegisterService>();
            registerService.Setup(s => s.GetSubregisterByName(r1.parentRegister.seoname, r1.seoname)).Returns(r1);

            var controller = createController(url, registerService.Object, null);
            var result = controller.GetSubregisterByName(r1.parentRegister.seoname, r1.seoname) as OkNegotiatedContentResult<Models.Api.Register>;

            Models.Api.Register registerApi = result.Content;
            registerApi.label.Should().Be("RegisterName");
        }

        [Fact]
        public void GetRegistersBySystemId()
        {
            //Testdata
            Models.Register r1 = NewRegister("Navn");

            List<Models.Register> registers = new List<Models.Register> { r1 };

            var registerService = new Mock<IRegisterService>();
            registerService.Setup(s => s.GetRegisterBySystemId(r1.systemId)).Returns(r1);

            var controller = createController(url, registerService.Object, null);
            var result = controller.GetRegisterBySystemId(r1.systemId.ToString()) as OkNegotiatedContentResult<Models.Api.Register>;

            Models.Api.Register registerApi = result.Content;
            registerApi.label.Should().Be("Navn");
        }

        [Fact]
        public void GetRegisterItemByName() {
            Models.Register register = NewRegister("Register name");
            List<RegisterItem> versions = GetListOfVersions("itemName", register, "Kartverket");
            var registerService = new Mock<IRegisterService>();
            var registerItemService = new Mock<IRegisterItemService>();

            registerItemService.Setup(s => s.GetAllVersionsOfItem(null, register.seoname, versions[0].seoname)).Returns(versions);
            registerService.Setup(r => r.GetRegister(null, register.seoname)).Returns(register);

            var controller = createController(url, registerService.Object, registerItemService.Object);
            var result = controller.GetRegisterItemByName(register.seoname, versions[0].submitter.seoname, versions[0].seoname ) as OkNegotiatedContentResult<Models.Api.Registeritem>;

            Models.Api.Registeritem actualVersions = result.Content;
            actualVersions.label.Should().Be("itemName");
        }

        [Fact]
        public void GetRegisterItemByNameAndVersion()
        {
            Models.Register register = NewRegister("Register name");
            List<RegisterItem> versions = GetListOfVersions("itemName", register, "Kartverket");

            var registerItemService = new Mock<IRegisterItemService>();
            registerItemService.Setup(s => s.GetRegisterItem(null, "register_name", "itemname", 2, null)).Returns(versions[2]);
            var controller = createController(url, null, registerItemService.Object);
            var result = controller.GetRegisterItemByVersionNr("register_name", "itemname", 2) as OkNegotiatedContentResult<Models.Api.Registeritem>;

            Models.Api.Registeritem actualVersions = result.Content;
            actualVersions.versionNumber.Should().Be(2);
        }

        [Fact]
        public void GetSubregisterItemByName()
        {
            Models.Register register = NewRegister("Register name");
            register.parentRegister = NewRegister("parent name");
            List<Models.RegisterItem> versions = GetListOfVersions("itemName", register, "Kartverket");

            var registerItemService = new Mock<IRegisterItemService>();
            registerItemService.Setup(s => s.GetAllVersionsOfItem(register.parentRegister.seoname, register.seoname, "itemname")).Returns(versions);
            var controller = createController(url, null, registerItemService.Object);
            var result = controller.GetSubregisterItemByName(register.parentRegister.seoname, register.seoname, "itemname") as OkNegotiatedContentResult<Models.Api.Registeritem>;

            Models.Api.Registeritem actualVersions = result.Content;
            actualVersions.label.Should().Be("itemName");
        }

        [Fact]
        public void GetRegisterItemsByOrganization()
        {
            Models.Register register = NewRegister("Register name");
            register.parentRegister = NewRegister("parent name");
            List<Models.RegisterItem> versions = GetListOfVersions("itemName", register, "Kartverket");
            List<Models.RegisterItem> versionsWithOtherOwner = GetListOfVersions("itemName", register, "Norges geologiske undersøkelse");

            var registerItemService = new Mock<IRegisterItemService>();
            registerItemService.Setup(s => s.GetRegisterItemsFromOrganization(register.parentRegister.seoname, register.seoname, "kartverket")).Returns(versions);
            var controller = createController(url, null, registerItemService.Object);
            var result = controller.GetRegisterItemsByOrganization(register.parentRegister.seoname, register.seoname, "kartverket") as OkNegotiatedContentResult<List<Models.Api.Registeritem>>;

            List<Models.Api.Registeritem> actualVersions = result.Content;
            actualVersions.Count.Should().Be(5);
        }

        [Fact]
        public void GetCurrentAndOtherVersions()
        {
            Models.Register register = NewRegister("Register name");
            List<RegisterItem> versions = GetListOfVersions("itemName", register, "Kartverket");

            var registerItemService = new Mock<IRegisterItemService>();
            var registerService = new Mock<IRegisterService>();

            registerItemService.Setup(s => s.GetAllVersionsOfItem(null, register.seoname, versions[0].seoname)).Returns(versions);
            registerService.Setup(r => r.GetRegister(null, register.seoname)).Returns(register);

            var controller = createController(url, registerService.Object, registerItemService.Object);
            var result = controller.GetRegisterItemByName(register.seoname, "kartverket", "itemname") as OkNegotiatedContentResult<Models.Api.Registeritem>;

            Models.Api.Registeritem actualCurrentVersion = result.Content;
            actualCurrentVersion.versions.Count.Should().Be(4);
        }

        [Fact]
        public void RegisterShouldContainParentRegisterWhenRegisterIsASubRegister()
        {
            Models.Register r1 = NewRegister("TestRegister");
            Models.Register parentRegister = NewRegister("Parent");
            r1.parentRegister = parentRegister;
            r1.parentRegisterId = parentRegister.systemId;
            r1.parentRegister.owner = NewOrganization("Kartverket");

            List<Models.Register> registers = new List<Models.Register> { r1, parentRegister };

            var registerService = new Mock<IRegisterService>();
            registerService.Setup(s => s.GetRegisters()).Returns(registers);
            var controller = createController(url, registerService.Object, null);
            var result = controller.GetRegisters() as OkNegotiatedContentResult<List<Models.Api.Register>>;
            List<Models.Api.Register> actualListOfRegisters = result.Content;
            actualListOfRegisters.Count.Should().Be(2);

            Models.Api.Register apiRegister = actualListOfRegisters[0];

            apiRegister.id.Should().Be("https://register.geonorge.no/subregister/parent/testorg/testregister");
        }

        [Fact]
        public void RegisterShouldNotContainParentRegisterWhenItIsATopLevelRegister()
        {
            Models.Register r1 = NewRegister("TestRegister");
            List<Models.Register> registers = new List<Models.Register> { r1 };

            var registerService = new Mock<IRegisterService>();
            registerService.Setup(s => s.GetRegisters()).Returns(registers);
            var controller = createController(url, registerService.Object, null);
            var result = controller.GetRegisters() as OkNegotiatedContentResult<List<Models.Api.Register>>;
            List<Models.Api.Register> actualListOfRegisters = result.Content;
            actualListOfRegisters.Count.Should().Be(1);

            Models.Api.Register apiRegister = actualListOfRegisters[0];

            apiRegister.id.Should().Be("https://register.geonorge.no/register/testregister");
        }

        [Fact]
        public void RegisterOwnerShouldNotBeNull()
        {
            Models.Register r1 = NewRegister("Navn");
            r1.owner = NewOrganization("Kartverket");

            List<Models.Register> registers = new List<Models.Register> { r1 };

            var registerService = new Mock<IRegisterService>();
            registerService.Setup(s => s.GetRegisters()).Returns(registers);
            var controller = createController(url, registerService.Object, null);
            var result = controller.GetRegisters() as OkNegotiatedContentResult<List<Models.Api.Register>>;
            List<Models.Api.Register> actualListOfRegisters = result.Content;
            actualListOfRegisters.Count.Should().Be(1);

            Models.Api.Register apiRegister = actualListOfRegisters[0];

            apiRegister.owner.Should().Be("kartverket");
        }

        [Fact]
        public void RegisterManagerShouldNotBeNull()
        {
            Models.Register r1 = NewRegister("Navn");
            r1.manager = NewOrganization("Kartverket");

            List<Models.Register> registers = new List<Models.Register> { r1 };

            var registerService = new Mock<IRegisterService>();
            registerService.Setup(s => s.GetRegisters()).Returns(registers);
            var controller = createController(url, registerService.Object, null);
            var result = controller.GetRegisters() as OkNegotiatedContentResult<List<Models.Api.Register>>;
            List<Models.Api.Register> actualListOfRegisters = result.Content;
            actualListOfRegisters.Count.Should().Be(1);

            Models.Api.Register apiRegister = actualListOfRegisters[0];

            apiRegister.manager.Should().Be("kartverket");
        }



        // **** HJELPEMETODER ****

        private Models.Register NewRegister(string name)
        {
            Models.Register register = new Models.Register();
            register.systemId = Guid.NewGuid();
            register.name = name;
            register.seoname = RegisterUrls.MakeSeoFriendlyString(register.name);
            register.description = "testbeskrivelse";
            register.owner = NewOrganization("Testorg");
            register.Translations.Add(new Models.Translations.RegisterTranslation { CultureName = Culture.NorwegianCode });
            register.AddMissingTranslations();
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

        private ApiRootController createController(string uri, IRegisterService registerService, IRegisterItemService registerItemService)
        {
            var controller = new ApiRootController(null, null, registerService, registerItemService, null, null, null, null);
            controller.Request = new HttpRequestMessage(HttpMethod.Get, uri);
            return controller;
        }

        private List<RegisterItem> GetListOfVersions(string versionName, Models.Register register, string Owner)
        {
            Models.Version versionGroup = new Models.Version();
            versionGroup.systemId = Guid.NewGuid();
            versionGroup.lastVersionNumber = 0;
            List<Models.RegisterItem> versions = new List<Models.RegisterItem>();

            for (int i = 0; i < 5; i++)
            {
                Document document = new Document();
                document.systemId = Guid.NewGuid();
                document.name = versionName;
                document.seoname = RegisterUrls.MakeSeoFriendlyString(document.name);
                Organization organisasjon = NewOrganization(Owner);
                document.documentowner = organisasjon;
                document.submitter = organisasjon;
                document.versionNumber = versionGroup.lastVersionNumber;
                document.versioning = versionGroup;
                document.versioningId = versionGroup.systemId;
                document.register = register;
                document.Translations.Add( new Models.Translations.DocumentTranslation { CultureName = Culture.NorwegianCode });
                document.AddMissingTranslations();

                versions.Add(document);
                versionGroup.lastVersionNumber++;
            }

            versionGroup.currentVersion = versions[0].systemId;
            foreach (Models.RegisterItem doc in versions)
            {
                doc.versioning.currentVersion = versionGroup.currentVersion;
            }
            return versions;
        }
    }
}
