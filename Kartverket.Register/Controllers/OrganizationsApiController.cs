using System.Web.Http;
using System.Web.Http.Cors;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Net.Http;
using Kartverket.Register.Models.Translations;
using System.Linq;
using Kartverket.Register.Helpers;
using System.Net.Http.Headers;
using System.Globalization;
using System.Threading;

namespace Kartverket.Register.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods:"*")]
    public class OrganizationsApiController : ApiController
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationsApiController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }
        /// <summary>
        /// Get organization by name
        /// </summary>
        //[Route("api/organisasjon/navn/{name}")]
        public IHttpActionResult GetOrganizationByName(string name)
        {
            Organization organization = _organizationService.GetOrganizationByName(name);

            if (organization == null)
                return NotFound();

            var externalModel = new Models.Api.Organization();
            externalModel.Convert(organization);
            return Ok(externalModel);
        }

        /// <summary>
        /// Get organization by name translated
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Route("api/organisasjon/navn/{name}/{culture}")]
        public IHttpActionResult GetOrganizationTranslatedByName(string name, string culture)
        {
            Organization organization = _organizationService.GetOrganizationTranslatedByName(name, culture);

            if (organization == null)
                return NotFound();

            var externalModel = new Models.Api.Organization();
            externalModel.Convert(organization);
            return Ok(externalModel);
        }


        /// <summary>
        /// Get organization by number
        /// </summary>
        //[Route("api/organisasjon/orgnr/{number}")]
        public IHttpActionResult GetOrganizationByNumber(string number)
        {
            Organization organization = _organizationService.GetOrganizationByNumber(number, CultureHelper.GetCurrentCulture());

            if (organization == null)
                return NotFound();

            var externalModel = new Models.Api.Organization();
            externalModel.Convert(organization);
            return Ok(externalModel);
        }

        /// <summary>
        /// Get list of municipalities
        /// </summary>
        //[Route("api/v2/organisasjoner/kommuner")]
        public IHttpActionResult GetOrganizationsV2()
        {
            List<Organization> organizations = _organizationService.GetMunicipalityOrganizations();

            if (organizations == null)
                return NotFound();

            List<Models.Api.OrganizationV2> apiModels = Models.Api.OrganizationV2.Convert(organizations);

            return Ok(apiModels);
        }

        /// <summary>
        /// Get  municipality by kommunenummer
        /// </summary>
        public IHttpActionResult GetOrganizationsByCodeV2(string kommunenummer)
        {
            Organization organizations = _organizationService.GetMunicipalityByNumber(kommunenummer);

            if (organizations == null)
                return NotFound();

            Models.Api.OrganizationV2 apiModels = Models.Api.OrganizationV2.Convert(organizations);

            return Ok(apiModels);
        }

        /// <summary>
        /// Get organization by name
        /// </summary>
        //[Route("api/v2/organisasjon/navn/{name}")]
        public IHttpActionResult GetOrganizationByNameV2(string name)
        {
            Organization organization = _organizationService.GetOrganizationByName(name);

            if (organization == null)
                return NotFound();

            Models.Api.OrganizationV2 apiModel = Models.Api.OrganizationV2.Convert(organization);
            return Ok(apiModel);
        }

        /// <summary>
        /// Get organization by number
        /// </summary>
        //[Route("api/v2/organisasjon/orgnr/{number}")]
        public IHttpActionResult GetOrganizationByNumberV2(string number)
        {
            Organization organization = _organizationService.GetOrganizationByNumber(number);

            if (organization == null)
                return NotFound();

            Models.Api.OrganizationV2 apiModel = Models.Api.OrganizationV2.Convert(organization);

            return Ok(apiModel);
        }
    }
}
