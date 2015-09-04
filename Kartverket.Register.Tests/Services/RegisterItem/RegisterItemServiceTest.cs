using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services.RegisterItem;

namespace Kartverket.Register.Tests.Services.RegisterItem
{
    public class RegisterItemServiceTest
    {
        [Test]
        public void getCurrentRegisterItem()
        {            
            Models.Register register = NewRegister("Register name");
            //register.parentRegister = NewRegister("Parentregister name");
            List<Models.RegisterItem> versions = GetListOfVersions("itemName", register);

            var registerItemService = new RegisterItemService(CreateTestDbContext(versions));
            Models.RegisterItem actualCurrentVersion = registerItemService.GetCurrentRegisterItem(register.seoname, versions[1].seoname);

            actualCurrentVersion.Should().Be(versions[0]);
        }

        [Test]
        public void getCurrentSubregisterItem()
        {
            Models.Register register = NewRegister("Register name");
            register.parentRegister = NewRegister("Parentregister name");
            List<Models.RegisterItem> versions = GetListOfVersions("itemName", register);

            var registerItemService = new RegisterItemService(CreateTestDbContext(versions));
            Models.RegisterItem actualCurrentVersion = registerItemService.GetCurrentSubregisterItem(register.parentRegister.seoname, register.seoname, versions[1].seoname);

            actualCurrentVersion.Should().Be(versions[0]);
        }

        [Test]
        public void getVersionsOfItem()
        {
            Models.Register register = NewRegister("Register name");
            register.parentRegister = NewRegister("Parentregister name");
            List<Models.RegisterItem> versions = GetListOfVersions("itemName", register);

            var registerItemService = new RegisterItemService(CreateTestDbContext(versions));
            List<Models.RegisterItem> actualListOfVersions = registerItemService.GetAllVersionsOfItem(register.seoname, versions[1].seoname);

            actualListOfVersions.Count.Should().Be(5);
        }

        [Test]
        public void GetRegisterItemByVersionNr()
        {
            Models.Register register = NewRegister("Register name");
            List<Models.RegisterItem> versions = GetListOfVersions("itemName", register);

            var registerItemService = new RegisterItemService(CreateTestDbContext(versions));
            Models.RegisterItem actualVersion = registerItemService.GetRegisterItemByVersionNr(register.seoname, "itemname", 2);

            actualVersion.Should().Be(versions[2]);
        }

        [Test]
        public void GetSubregisterItemByVersionNr()
        {
            Models.Register register = NewRegister("Register name");
            register.parentRegister = NewRegister("Parent name");
            List<Models.RegisterItem> versions = GetListOfVersions("itemName", register);

            var registerItemService = new RegisterItemService(CreateTestDbContext(versions));
            Models.RegisterItem actualVersion = registerItemService.GetSubregisterItemByVersionNr(register.parentRegister.seoname, register.seoname, "itemname", 2);

            actualVersion.Should().Be(versions[2]);
        }



        // **** HJELPEMETODER ****

        private Models.Register NewRegister(string name)
        {
            Models.Register register = new Models.Register();
            register.systemId = new Guid();
            register.name = name;
            register.seoname = HtmlHelperExtensions.MakeSeoFriendlyString(register.name);
            register.description = "testbeskrivelse";
            return register;
        }

        private List<Models.RegisterItem> GetListOfVersions(string versionName, Models.Register register)
        {
            Models.Version versionGroup = new Models.Version();
            versionGroup.systemId = new Guid();
            versionGroup.lastVersionNumber = 0;
            List<Models.RegisterItem> versions = new List<Models.RegisterItem>();

            for (int i = 0; i < 5; i++)
            {
                Models.Document document = new Document();
                document.systemId = new Guid();
                document.name = versionName;
                document.seoname = HtmlHelperExtensions.MakeSeoFriendlyString(document.name);
                document.documentowner = NewOrganization("Kartverket");
                document.versionNumber = versionGroup.lastVersionNumber;
                document.versioning = versionGroup;
                document.versioningId = versionGroup.systemId;
                document.register = register;

                versions.Add(document);
                versionGroup.lastVersionNumber++;
            }

            foreach (Document doc in versions)
            {
                doc.versioning.currentVersion = versions[0].systemId;
            }                    
            return versions;
        }

        private Models.Organization NewOrganization(string name)
        {
            Models.Organization organization = new Models.Organization();
            organization.systemId = new Guid();
            organization.name = name;
            organization.seoname = HtmlHelperExtensions.MakeSeoFriendlyString(organization.name);
            organization.description = "beskrivelse av organisasjon";
            return organization;
        }

        private RegisterDbContext CreateTestDbContext(IEnumerable<Models.RegisterItem> versions)
        {
            var data = versions.AsQueryable();

            var mockSet = new Mock<DbSet<Models.RegisterItem>>();
            mockSet.As<IQueryable<Models.RegisterItem>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Models.RegisterItem>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Models.RegisterItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Models.RegisterItem>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<RegisterDbContext>();
            mockContext.Setup(c => c.RegisterItems).Returns(mockSet.Object);

            return mockContext.Object;
        }
    }
}
