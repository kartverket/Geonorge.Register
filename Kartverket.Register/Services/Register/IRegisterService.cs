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
        Kartverket.Register.Models.Register Filter(Kartverket.Register.Models.Register register, FilterParameters filter);
        string ContentNegotiation(ControllerContext context);
        Kartverket.Register.Models.Register GetRegisterByName(string name);
        Kartverket.Register.Models.Register GetSubRegisterByNameAndParent(string registerName, string parentRegisterName);
        Organization GetOrganization(string organization);
    }
}
