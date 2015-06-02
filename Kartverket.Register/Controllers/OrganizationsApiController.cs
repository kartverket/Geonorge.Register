using System.Web.Http;
using System.Web.Http.Cors;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using System.Web.Configuration;
using System.Web.Http.Description;

namespace Kartverket.Register.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods:"*")]
    [ApiExplorerSettings(IgnoreApi = true)]
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
                LogoUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Organization.DataDirectory + organization.logoFilename,
                LogoLargeUrl = WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Organization.DataDirectory + organization.largeLogo
            };
        }
    }
}
