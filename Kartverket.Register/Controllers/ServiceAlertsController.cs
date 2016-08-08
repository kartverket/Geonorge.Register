using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services;
using Kartverket.Register.Helpers;
using System.Collections.Generic;
using System.Net;
using Kartverket.Register.Services.Versioning;

namespace Kartverket.Register.Controllers
{
    public class ServiceAlertsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();
        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _accessControlService;
        private IVersioningService _versioningService;

        public ServiceAlertsController(IRegisterItemService registerItemServive, IRegisterService registerService, IAccessControlService accessControlService, IVersioningService versioningService)
        {
            _registerItemService = registerItemServive;
            _registerService = registerService;
            _accessControlService = accessControlService;
            _versioningService = versioningService;
        }

        // GET: ServiceAlerts/Create
        [Authorize]
        [Route("tjenestevarsler/{parentregister}/{registerowner}/{registerName}/ny")]
        [Route("tjenestevarsler/{registerName}/ny")]
        public ActionResult Create(string parentRegister, string registerName)
        {
            ServiceAlert serviceAlert = new ServiceAlert();
            serviceAlert.register = _registerService.GetRegister(parentRegister, registerName);
            ViewBags(serviceAlert);

            if (serviceAlert.register != null)
            {
                if (_accessControlService.Access(serviceAlert.register))
                {
                    return View(serviceAlert);
                }
            }
            return HttpNotFound();
        }


        // POST: ServiceAlerts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("tjenestevarsler/{parentregister}/{registerowner}/{registerName}/ny")]
        [Route("tjenestevarsler/{registerName}/ny")]
        public ActionResult Create(ServiceAlert serviceAlert, string parentRegister, string registerName)
        {
            serviceAlert.register = _registerService.GetRegister(parentRegister, registerName);
            if (serviceAlert.register != null)
            {
                if (_accessControlService.Access(serviceAlert.register))
                {
                    if (!_registerItemService.validateName(serviceAlert))
                    {
                        ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                        return View(serviceAlert);
                    }
                    if (ModelState.IsValid)
                    {
                        serviceAlert.GetMetadataByUuid();
                        serviceAlert.submitter = _registerService.GetOrganizationByUserName();
                        serviceAlert.InitializeNewServiceAlert();
                        serviceAlert.versioningId = _registerItemService.NewVersioningGroup(serviceAlert);
                        serviceAlert.register.modified = System.DateTime.Now;
                        _registerItemService.SaveNewRegisterItem(serviceAlert);
                        return Redirect(serviceAlert.GetObjectUrl());
                    }
                }
            }
            ViewBags(serviceAlert);
            return View(serviceAlert);
        }


        private void ViewBags(ServiceAlert serviceAlert)
        {
            ViewBag.AlertType = new SelectList(serviceAlert.GetAlertTypes(), serviceAlert.AlertType);
            ViewBag.ServiceUuid = new SelectList(GetServicesFromKartkatalogen(), "Key", "Value", serviceAlert.ServiceUuid);
        }

        public Dictionary<string, string> GetServicesFromKartkatalogen()
        {
            Dictionary<string, string> ServiceList = new Dictionary<string, string>();
            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/search/?facets[0]name=type&facets[0]value=service&limit=1000&orderby=title";
            WebClient c = new WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var data = c.DownloadString(url);
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);

            var result = response["Results"];

            foreach (var service in result)
            {
                var ServiceUuid = service["Uuid"].ToString();
                if (string.IsNullOrWhiteSpace(ServiceUuid))
                    ServiceUuid = service["Uuid"].ToString();

                var Organization = service["Organization"] != null ? service["Organization"].ToString() : "";

                if (!ServiceList.ContainsKey(ServiceUuid))
                {
                    if(_accessControlService.IsAdmin() || _accessControlService.IsOwner(Organization, _accessControlService.GetSecurityClaim("organization")[0]))
                    ServiceList.Add(ServiceUuid, service["Title"].ToString());
                }
            }
            return ServiceList;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
