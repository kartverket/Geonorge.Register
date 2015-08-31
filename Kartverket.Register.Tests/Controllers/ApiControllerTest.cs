using System;
using FluentAssertions;
using Kartverket.Register.Controllers;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Moq;
using NUnit.Framework;
using System.Configuration;
using Kartverket.Register.Services.Register;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http.Results;


namespace Kartverket.Register.Tests.Controllers
{    
    public class ApiControllerTest
    {
        [Test]
        public void GetRegisters()
        {
            Models.Register r1 = NewRegister("Navn1");
            Models.Register r2 = NewRegister("Navn2");
            Models.Register r3 = NewRegister("Navn3");           
            
            var controller = createController(new List<Models.Register>{r1, r2, r3}, "http://register.geonorge.no/api/register");

            var result = controller.GetRegisters() as OkNegotiatedContentResult<List<Models.Api.Register>>;

            List<Models.Api.Register> actualListOfRegisters = result.Content;
            actualListOfRegisters.Count.Should().Be(3); 
        }

        //public void RegisterShouldContainParentRegisterSeonameWhenRegisterIsASubRegister()
        //{
        //    Models.Register r1 = NewRegister("Navn1");
        //    r1.parentRegisterId = new Guid();

        //    var controller = createController(r1, "http://register.geonorge.no/subregister/kodelister/kartverket/metadata-kodelister");
        //    var result = controller.GetRegisterByName(r1.name) as OkNegotiatedContentResult<Models.Api.Register>;

        //    Models.Api.Registeritem
        //}

        //public void RegisterShouldContainParentRegisterSeonameWhenRegisterIsASubRegister()
        //{
        //    Models.Register r1 = NewRegister("Navn1");
        //    r1.parentRegisterId = new Guid();

        //    var controller = createController(r1, "http://register.geonorge.no/subregister/kodelister/kartverket/metadata-kodelister");
        //    var result = controller.GetRegisterByName();
        //}

        public void RegisterUrlShouldOnlyContainNameWhenItIsATopLevelRegister()
        {

        }


        
        
        private Models.Register NewRegister(string name) {
            Models.Register register = new Models.Register();
            register.systemId = new Guid();
            register.name = name;
            register.description = "testbeskrivelse";
            return register;
        }

        private ApiRootController createController(List<Models.Register> registers, string uri)
        {
            var controller = new ApiRootController(null, createRegisterService(registers));
            controller.Request = new HttpRequestMessage(HttpMethod.Get, uri);
            return controller;
        }

        private ApiRootController createController(Models.Register register, string uri)
        {
            var controller = new ApiRootController(null, createRegisterService(register));
            controller.Request = new HttpRequestMessage(HttpMethod.Get, uri);
            return controller;
        }

        private IRegisterService createRegisterService(Models.Register register)
        {
            var registerService = new Mock<IRegisterService>();
            registerService.Setup(r => r.GetRegisterByName(register.name)).Returns(register);

            return registerService.Object;
        }

        private IRegisterService createRegisterService(List<Models.Register> registers)
        {
            var registerService = new Mock<IRegisterService>();
            registerService.Setup(r => r.GetRegisters()).Returns(registers);

            return registerService.Object;
        }
      


    }


}
