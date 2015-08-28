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
            r2.parentRegisterId = new Guid();
            Models.Register r3 = new Models.Register();
            r3.name = "Register 3";
            List<Models.Register> registerList = new List<Models.Register>() { r1, r2, r3};

            var registerService = new RegisterService(CreateTestDbContext(registerList));

            List<Models.Register> actualListOfRegisters = registerService.GetRegisters();

            Assert.AreEqual(2, actualListOfRegisters.Count);

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
