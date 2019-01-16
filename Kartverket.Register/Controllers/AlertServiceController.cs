using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Kartverket.Register.Models;
using Kartverket.Register.Models.Api;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Register;
using Kartverket.Register.App_Start;

namespace Kartverket.Register.Controllers
{
    public class AlertServiceController : ApiController
    {
        private readonly RegisterDbContext _dbContext;
        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;

        public AlertServiceController(RegisterDbContext dbContext, IRegisterItemService registerItemServive, IRegisterService registerService)
        {
            _dbContext = dbContext;
            _registerItemService = registerItemServive;
            _registerService = registerService;
        }

        /// <summary>
        /// Add service alert
        /// </summary>
        // POST: api/ApiServiceAlerts
        [System.Web.Http.Authorize(Roles = AuthConfig.RegisterProviderRole)]
        [ResponseType(typeof(AlertService))]
        public IHttpActionResult PostServiceAlert(AlertService alertService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string parentRegister = null;
            string registerName = "tjenestevarsler";

            Alert serviceAlert = new Alert();
            serviceAlert.name = "navn";
            serviceAlert.AlertType = alertService.AlertType;
            serviceAlert.Note = alertService.Note;
            serviceAlert.UuidExternal = alertService.ServiceUuid;
            if (alertService.AlertDate.HasValue)
                serviceAlert.AlertDate = alertService.AlertDate.Value;

            if (alertService.EffectiveDate.HasValue)
                serviceAlert.EffectiveDate = alertService.EffectiveDate.Value;


            serviceAlert.register = _registerService.GetRegister(parentRegister, registerName);
            if (serviceAlert.register != null)
            {
                    if (ModelState.IsValid)
                    {
                        serviceAlert.GetMetadataByUuid();
                        serviceAlert.submitterId = new Guid("10087020-F17C-45E1-8542-02ACBCF3D8A3");
                        serviceAlert.InitializeNewAlert();
                        serviceAlert.versioningId = _registerItemService.NewVersioningGroup(serviceAlert);
                        serviceAlert.register.modified = System.DateTime.Now;
                        _registerItemService.SaveNewRegisterItem(serviceAlert);
                    }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // GET: api/ApiServiceAlerts
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult GetRegisterItems()
        {
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // GET: api/ApiServiceAlerts/5
        [ResponseType(typeof(AlertService))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult GetServiceAlert(Guid id)
        {
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        // PUT: api/ApiServiceAlerts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutServiceAlert(Guid id, AlertService serviceAlert)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        // DELETE: api/ApiServiceAlerts/5
        [ResponseType(typeof(AlertService))]
        public IHttpActionResult DeleteServiceAlert(Guid id)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
    }
}