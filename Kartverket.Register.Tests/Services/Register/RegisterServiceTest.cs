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

namespace Kartverket.Register.Tests.Services.Register
{
    public class RegisterServiceTest
    {
        [Test]
        public void GetTopLevelRegisters()
        {            
            Models.Register r1 = new Models.Register();
            r1.name = "Register 1";
            Models.Register r2 = new Models.Register();
            r2.name = "Register 2";
            r2.parentRegisterId = Guid.NewGuid();
            Models.Register r3 = new Models.Register();
            r3.name = "Register 3";
            List<Models.Register> registerList = new List<Models.Register>() { r1, r2, r3};

            var registerService = new RegisterService(CreateTestDbContext(registerList));

            List<Models.Register> actualListOfRegisters = registerService.GetRegisters();

            Assert.AreEqual(2, actualListOfRegisters.Count);
        }

        [Test]
        public void GetTopLevelRegisterByName()
        {
            Models.Register r1 = NewRegister("Register 1");
            Models.Register r2 = NewRegister("Register 2");
            Models.Register r3 = NewRegister("Register 3");

            List<Models.Register> registerList = new List<Models.Register>() { r1, r2, r3 };

            var registerService = new RegisterService(CreateTestDbContext(registerList));

            Models.Register actualRegister = registerService.GetRegisterByName(r1.seoname);

            actualRegister.Should().Be(r1);

        }

        [Test]
        public void GetSubregisterByName()
        {
            Models.Register r1 = NewRegister("Register 1");
            Models.Register r2 = NewRegister("Register 2");
            Models.Register r3 = NewRegister("Register 3");

            r1.parentRegister = r2;

            List<Models.Register> registerList = new List<Models.Register>() { r1, r2, r3 };

            var registerService = new RegisterService(CreateTestDbContext(registerList));

            Models.Register actualRegister = registerService.GetSubregisterByName(r2.seoname, r1.seoname);

            actualRegister.Should().Be(r1);
        }

        [Test]
        public void GetSubregisterBysystemId()
        {
            Models.Register r1 = NewRegister("Register 1");
            Models.Register r2 = NewRegister("Register 2");
            Models.Register r3 = NewRegister("Register 3");

            List<Models.Register> registerList = new List<Models.Register>() { r1, r2, r3 };

            var registerService = new RegisterService(CreateTestDbContext(registerList));

            Models.Register actualRegister = registerService.GetRegisterBySystemId(r1.systemId);

            actualRegister.Should().Be(r1);
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

        private Models.Organization NewOrganization(string name)
        {
            Models.Organization organization = new Models.Organization();
            organization.systemId = Guid.NewGuid();
            organization.name = name;
            organization.seoname = RegisterUrls.MakeSeoFriendlyString(organization.name);
            organization.description = "beskrivelse av organisasjon";
            return organization;
        }

        private RegisterDbContext CreateTestDbContext(IEnumerable<Models.Register> registerList)
        {
            var data = registerList.AsQueryable();

            var mockSet = new Mock<DbSet<Models.Register>>();
            mockSet.As<IQueryable<Models.Register>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Models.Register>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Models.Register>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Models.Register>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<RegisterDbContext>();
            mockContext.Setup(c => c.Registers).Returns(mockSet.Object);

            return mockContext.Object;
        }

    }
}
