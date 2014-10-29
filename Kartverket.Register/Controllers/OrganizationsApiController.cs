using System.Web.Http;
using Kartverket.Register.Models.Api;
using Kartverket.Register.Services;

namespace Kartverket.Register.Controllers
{
    public class OrganizationsApiController : ApiController
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationsApiController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [Route("api/organisasjon/navn/{name}")]
        public IHttpActionResult GetOrganizationByName(string name)
        {
            Models.Organization organization = _organizationService.GetOrganizationByName(name);

            if (organization == null)
                return NotFound();

            return Ok(Convert(organization));
        }

        private Organization Convert(Models.Organization organization)
        {
            return new Organization
            {
                Name = organization.Name,
                Number = organization.Number,
                LogoUrl = Url.Content("~/data/organizations/") + organization.LogoFilename
            };
        }
    }
}
