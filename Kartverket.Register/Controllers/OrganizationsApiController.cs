using System.Web.Http;

namespace Kartverket.Register.Controllers
{
    public class OrganizationsApiController : ApiController
    {

        public IHttpActionResult GetProduct(int id)
        {
            var product = products.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

    }
}
