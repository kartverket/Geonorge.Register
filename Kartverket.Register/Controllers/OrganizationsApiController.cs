using System.Web.Http;

namespace Kartverket.Register.Controllers
{
    public class OrganizationsApiController : ApiController
    {

        public IHttpActionResult GetOrganizationByName(string name)
        {
            return Ok();
        }

    }
}
