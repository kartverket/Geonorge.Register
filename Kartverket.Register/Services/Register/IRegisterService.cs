using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Services.Register
{
    public interface IRegisterService
    {
        Models.Register FilterRegisterItems(Models.Register register, FilterParameters filter);
        string ContentNegotiation(ControllerContext context);
        Models.Register GetRegisterByName(string name);
        Models.Register GetSubregisterByName(string parentName, string registerName);
        List<Models.Register> GetRegisters();
        List<Models.Register> GetSubregisters();
        List<Models.Register> GetSubregistersOfRegister(Models.Register register);
        Models.Register GetRegisterBySystemId(Guid systemId);
        Models.Register GetRegister(string parentRegister, string register);
        Models.Register SetStatus(Models.Register register, Models.Register originalRegister);
        bool RegisterHasChildren(string parentname, string registername);
        bool validationName(Object model);
        Organization GetOrganizationByUserName();
        Organization GetOrganizationByOrganizationNr(string number);

    }
}
