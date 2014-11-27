using System.Web.Http;
using Kartverket.Register.Models;
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
            Organization organization = _organizationService.GetOrganizationByName(name);

            if (organization == null)
                return NotFound();

            return Ok(Convert(organization));
        }

        [Route("api/organisasjon/orgnr/{number}")]
        public IHttpActionResult GetOrganizationByNumber(string number)
        {
            Organization organization = _organizationService.GetOrganizationByNumber(number);

            if (organization == null)
                return NotFound();

            return Ok(Convert(organization));
        }

        private Models.Api.Organization Convert(Organization organization)
        {
            return new Models.Api.Organization
            {
                Name = organization.name,
                Number = organization.number,
                LogoUrl = Url.Content(Constants.DataDirectory) + Organization.DataDirectory + organization.logoFilename
            };
        }
    }
}
