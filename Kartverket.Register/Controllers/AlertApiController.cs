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
    public class AlertApiController : ApiController
    {
        private readonly RegisterDbContext _dbContext;
        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;

        public AlertApiController(RegisterDbContext dbContext, IRegisterItemService registerItemServive, IRegisterService registerService)
        {
            _dbContext = dbContext;
            _registerItemService = registerItemServive;
            _registerService = registerService;
        }

        /// <summary>
        /// Add service alert
        /// </summary>
        [System.Web.Http.Authorize(Roles = AuthConfig.RegisterProviderRole)]
        [ResponseType(typeof(AlertAdd))]
        [ApiExplorerSettings(IgnoreApi = false)]
        [System.Web.Http.HttpPost]
        public IHttpActionResult PostServiceAlert(AlertAdd alertService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string parentRegister = null;
            string registerName = "varsler";

            Alert serviceAlert = new Alert();
            serviceAlert.name = "navn";
            serviceAlert.AlertCategory = Constants.AlertCategoryService;
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
                        if(string.IsNullOrEmpty(serviceAlert.Owner))
                            serviceAlert.Owner = "Kartverket";
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

        /// <summary>
        /// Get all service alerts
        /// </summary>
        [ResponseType(typeof(List<AlertView>))]
        [ApiExplorerSettings(IgnoreApi = false)]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Get()
        {
            var alertRegister = _registerService.GetRegister(null, "varsler");
            List<AlertView> alerts = new List<AlertView>();

            foreach (Alert alert in alertRegister.items)
            {
                alerts.Add(
                    new AlertView {
                        SystemId = alert.systemId.ToString(),
                        AlertDate = alert.AlertDate,
                        AlertType = alert.AlertType,
                        EffectiveDate = alert.EffectiveDate,
                        Note = alert.Note
                    });
            }

            return Ok(alerts);
        }

        /// <summary>
        /// Get service alert
        /// </summary>
        [ResponseType(typeof(AlertView))]
        [ApiExplorerSettings(IgnoreApi = false)]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetId(string id)
        {
            var alert = _dbContext.Alerts.Where(a => a.systemId.ToString() == id).FirstOrDefault();
            if (alert == null)
                return NotFound();

            var alertView = new AlertView();
            alertView.SystemId = alert.systemId.ToString();
            alertView.AlertCategory = alert.AlertCategory;
            alertView.AlertDate = alert.AlertDate;
            alertView.AlertType = alert.AlertType;
            alertView.DateResolved = alert.DateResolved;
            alertView.EffectiveDate = alert.EffectiveDate;
            alertView.Department = alert.departmentId;
            alertView.Image1 = alert.Image1;
            alertView.Image1Thumbnail = alert.Image1Thumbnail;
            alertView.Image2 = alert.Image2;
            alertView.Image2Thumbnail = alert.Image2Thumbnail;
            alertView.Link = alert.Link;
            alertView.Note = alert.Note;
            alertView.Owner = alert.Owner;
            alertView.StationName = alert.StationName;
            alertView.StationType = alert.StationType;
            alertView.Summary = alert.Summary;
            alertView.Tags = alert.Tags.Select(t => t.value).ToList();
            alertView.Type = alert.Type;
            alertView.UrlExternal = alert.UrlExternal;
            alertView.UuidExternal = alert.UuidExternal;


            return Ok(alertView);
        }

        /// <summary>
        /// Update service alert
        /// </summary>
        [System.Web.Http.Authorize(Roles = AuthConfig.RegisterProviderRole)]
        [ApiExplorerSettings(IgnoreApi = false)]
        [ResponseType(typeof(void))]
        [System.Web.Http.HttpPut]
        public IHttpActionResult PutServiceAlert(string id, AlertUpdate alert)
        {
            var originalAlert = _dbContext.Alerts.Where(a => a.systemId.ToString() == id).FirstOrDefault();

            if (originalAlert == null)
                return NotFound();

            originalAlert.Summary = alert.Summary;
            originalAlert.DateResolved = alert.DateResolved;

            originalAlert.register.modified = System.DateTime.Now;
            _dbContext.Entry(originalAlert).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ResponseType(typeof(AlertAdd))]
        [System.Web.Http.HttpDelete]
        public IHttpActionResult DeleteServiceAlert(Guid id)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
    }
}