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
            Models.Register r1 = new Models.Register();
            r1.name = "Register 1";
            Models.Register r2 = new Models.Register();
            r2.name = "Register 2";
            Models.Register r3 = new Models.Register();
            r3.name = "Register 3";
            
            var api = createController(new List<Models.Register>{r1, r2, r3}, "http://register.geonorge.no/api/register");

            var result = api.GetRegisters() as OkNegotiatedContentResult<List<Models.Api.Register>>;

            List<Models.Api.Register> actualListOfRegisters = result.Content;
            actualListOfRegisters.Count.Should().Be(3); 
        }


        private ApiRootController createController(List<Models.Register> registerItems, string uri)
        {
            var controller = new ApiRootController(null, createRegisterService(registerItems));
            controller.Request = new HttpRequestMessage(HttpMethod.Get, uri);
            return controller;
        }

        private IRegisterService createRegisterService(List<Models.Register> registerItems)
        {
            var registerService = new Mock<IRegisterService>();
            registerService.Setup(r => r.GetRegisters()).Returns(registerItems);
            return registerService.Object;
        }

        public void RegisterIdShouldContainParentRegisterSeonameWhenRegisterIsASubRegister()
        {

        }

        public void RegisterIdShouldOnlyContainIdAndNameWhenItIsATopLevelRegister()
        {

        }

        


    }


}
