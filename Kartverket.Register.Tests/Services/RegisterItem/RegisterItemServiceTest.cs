using Kartverket.Register.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using FluentAssertions;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services.RegisterItem;
using Xunit;

namespace Kartverket.Register.Tests.Services.RegisterItem
{
    public class RegisterItemServiceTest
    {
        [Fact]
        public void GetCurrentRegisterItem()
        {            
            var register = NewRegister("Register name");
            var versions = GetListOfVersions("itemName", register, "Kartverket");

            var registerItemService = new RegisterItemService(CreateTestDbContext(versions));
            var actualCurrentVersion = registerItemService.GetCurrentRegisterItem(null, register.seoname, versions[1].seoname);

            actualCurrentVersion.Should().Be(versions[0]);
        }

        [Fact]
        public void GetVersionsOfItem()
        {
            var register = NewRegister("Register name");
            register.parentRegister = NewRegister("Parentregister name");
            var versions = GetListOfVersions("itemName", register, "Kartverket");

            var registerItemService = new RegisterItemService(CreateTestDbContext(versions));
            var actualListOfVersions = registerItemService.GetAllVersionsOfItem(register.parentRegister.seoname, register.seoname, versions[1].seoname);

            actualListOfVersions.Count.Should().Be(5);
        }

        [Fact]
        public void GetRegisterItemByVersionNr()
        {
            var register = NewRegister("Register name");
            var versions = GetListOfVersions("itemName", register, "kartverket");

            var registerItemService = new RegisterItemService(CreateTestDbContext(versions));
            var actualVersion = registerItemService.GetRegisterItem(null,  register.seoname, "itemname", 2);

            actualVersion.Should().Be(versions[1]);
        }

        [Fact]
        public void GetRegisterItemByOrganization()
        {
            var register = NewRegister("Register name");
            var versions = GetListOfVersions("itemName", register, "Kartverket");
            var versionsFromOtherOrganization = GetListOfVersions("itemName2", register, "Kartverket");

            foreach (var item in versionsFromOtherOrganization)
            {
                versions.Add(item);
            }

            var registerItemService = new RegisterItemService(CreateTestDbContext(versions));
            var actualVersion = registerItemService.GetRegisterItemsFromOrganization(null, register.seoname, "kartverket");

            actualVersion.Count.Should().Be(2);
        }

        [Fact]
        public void GetSubregisterItemByOrganization()
        {
            var register = NewRegister("Register name");
            register.parentRegister = NewRegister("Parent name");
            var versions = GetListOfVersions("itemName", register, "Kartverket");
            var versionsFromOtherOrganization = GetListOfVersions("itemName2", register, "Kartverket");

            foreach (var item in versionsFromOtherOrganization)
            {
                versions.Add(item);
            }

            var registerItemService = new RegisterItemService(CreateTestDbContext(versions));
            var actualVersion = registerItemService.GetRegisterItemsFromOrganization(register.parentRegister.seoname, register.seoname, "kartverket");

            actualVersion.Count.Should().Be(2);
        }



        // **** HJELPEMETODER ****

        private Models.Register NewRegister(string name)
        {
            Models.Register register = new Models.Register();
            register.systemId = Guid.NewGuid();
            register.name = name;
            register.seoname = RegisterUrls.MakeSeoFriendlyString(register.name);
            register.description = "testbeskrivelse";
            return register;
        }

        private List<Models.RegisterItem> GetListOfVersions(string versionName, Models.Register register, string Owner)
        {
            Models.Version versionGroup = new Models.Version();
            versionGroup.systemId = Guid.NewGuid();
            versionGroup.lastVersionNumber = 1;
            List<Models.RegisterItem> versions = new List<Models.RegisterItem>();

            for (int i = 0; i < 5; i++)
            {
                Models.Document document = new Document();
                document.systemId = Guid.Parse("c6056ed8-e040-42ef-b3c8-02f66fbb0ce" + i);
                document.name = versionName;
                document.seoname = RegisterUrls.MakeSeoFriendlyString(document.name);
                Organization organisasjon = NewOrganization(Owner);
                document.documentowner = organisasjon;
                document.submitter = organisasjon;
                document.versionNumber = versionGroup.lastVersionNumber;
                document.versioning = versionGroup;
                document.versioningId = versionGroup.systemId;
                document.register = register;

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

        private Organization NewOrganization(string name)
        {
            Models.Organization organization = new Models.Organization();
            organization.systemId = Guid.NewGuid();
            organization.name = name;
            organization.seoname = RegisterUrls.MakeSeoFriendlyString(organization.name);
            organization.description = "beskrivelse av organisasjon";
            return organization;
        }

        private RegisterDbContext CreateTestDbContext(IEnumerable<Models.RegisterItem> versions = null)
        {
            var mockContext = new Mock<RegisterDbContext>();

            if (versions.Any())
            {
                var data = versions.AsQueryable();
                var mockSet = new Mock<DbSet<Models.RegisterItem>>();
                mockSet.As<IQueryable<Models.RegisterItem>>().Setup(m => m.Provider).Returns(data.Provider);
                mockSet.As<IQueryable<Models.RegisterItem>>().Setup(m => m.Expression).Returns(data.Expression);
                mockSet.As<IQueryable<Models.RegisterItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
                mockSet.As<IQueryable<Models.RegisterItem>>().Setup(m => m.GetEnumerator())
                    .Returns(data.GetEnumerator());
                mockContext.Setup(c => c.RegisterItems).Returns(mockSet.Object);
            }

            return mockContext.Object;
        }
    }
}
