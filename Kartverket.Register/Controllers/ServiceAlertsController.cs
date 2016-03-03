using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services;
using Kartverket.Register.Helpers;
using Kartverket.DOK.Service;
using Kartverket.Register.Models.ViewModels;
using System.Collections.Generic;
using System;
using System.Net;
using System.Web;
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
                        serviceAlert.InitializeNewServiceAlert();
                        serviceAlert.submitter = _registerService.GetOrganizationByUserName();
                        serviceAlert.submitterId = serviceAlert.submitter.systemId;
                        serviceAlert.versioningId = _registerItemService.NewVersioningGroup(serviceAlert);
                        _registerItemService.SaveNewRegisterItem(serviceAlert);
                        return Redirect(serviceAlert.GetObjectUrl());
                    }
                }
            }
            ViewBags(serviceAlert);
            return View(serviceAlert);
        }


        // GET: ServiceAlerts/Edit
        [Route("tjenestevarsler/{parentregister}/{registerowner}/{registerName}/{itemowner}/{item}/rediger")]
        [Route("tjenestevarsler/{registerName}/{itemowner}/{item}/rediger")]
        public ActionResult Edit(string parentRegister, string registerName, string item)
        {
            ServiceAlert serviceAlert = (ServiceAlert)_registerItemService.GetRegisterItem(parentRegister, registerName, item, 1);
            if (serviceAlert != null)
            {
                if (_accessControlService.Access(serviceAlert))
                {
                    ViewBags(serviceAlert);
                    return View(serviceAlert);
                }
                return HttpNotFound("Ingen tilgang");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        // POST: ServiceAlerts/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("tjenestevarsler/{parentregister}/{registerowner}/{registerName}/{itemowner}/{item}/rediger")]
        [Route("tjenestevarsler/{registerName}/{itemowner}/{item}/rediger")]
        public ActionResult Edit(ServiceAlert serviceAlert, string parentRegister, string registerName, string item)
        {
            ServiceAlert originalServiceAlert = (ServiceAlert)_registerItemService.GetRegisterItem(parentRegister, registerName, item, 1);
            if (originalServiceAlert != null)
            {
                if (_accessControlService.Access(originalServiceAlert))
                {
                    if (!_registerItemService.validateName(serviceAlert))
                    {
                        ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                        ViewBags(originalServiceAlert);
                        return View(originalServiceAlert);
                    }
                    if (ModelState.IsValid)
                    {
                        originalServiceAlert.UpdateServiceAlert(serviceAlert);
                        _registerItemService.SaveEditedRegisterItem(originalServiceAlert);
                        return Redirect(originalServiceAlert.GetObjectUrl());
                    }
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            ViewBags(originalServiceAlert);
            return View(originalServiceAlert);
        }


        // GET: ServiceAlerts/Delete
        [Route("tjenestevarsler/{parentregister}/{registerowner}/{registerName}/{itemowner}/{item}/slett")]
        [Route("tjenestevarsler/{registerName}/{itemowner}/{item}/slett")]
        public ActionResult Delete(string parentRegister, string registerName, string item)
        {
            ServiceAlert serviceAlert = (ServiceAlert)_registerItemService.GetRegisterItem(parentRegister, registerName, item, 1);
            if (serviceAlert != null)
            {
                if (_accessControlService.Access(serviceAlert))
                {
                    return View(serviceAlert);
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound("Finner ikke tjenestevarselet");
        }


        // POST: ServiceAlerts/Delete
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("tjenestevarsler/{parentregister}/{registerowner}/{registerName}/{itemowner}/{item}/slett")]
        [Route("tjenestevarsler/{registerName}/{itemowner}/{item}/slett")]
        public ActionResult DeleteConfirmed(string parentRegister, string registerName, string item)
        {
            ServiceAlert serviceAlert = (ServiceAlert)_registerItemService.GetCurrentRegisterItem(parentRegister, registerName, item);
            string registerUrl = serviceAlert.register.GetObjectUrl();
            DeleteServiceAlert(serviceAlert);
            return Redirect(registerUrl);
        }

        private void DeleteServiceAlert(ServiceAlert serviceAlert)
        {
            Guid versioningId = serviceAlert.versioningId;
            _registerItemService.SaveDeleteRegisterItem(serviceAlert);
            _versioningService.DeleteVersionGroup(versioningId);
        }


        private void ViewBags(ServiceAlert serviceAlert)
        {
            ViewBag.OwnerId = _registerItemService.GetOwnerSelectList(serviceAlert.OwnerId);
            ViewBag.AlertType = new SelectList(serviceAlert.GetAlertTypes(), serviceAlert.AlertType);
            ViewBag.ServiceUuid = new SelectList(GetServicesFromKartkatalogen(), "Key", "Value", serviceAlert.ServiceUuid);
        }

        public Dictionary<string, string> GetServicesFromKartkatalogen()
        {
            Dictionary<string, string> ServiceList = new Dictionary<string, string>();
            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/search/?facets[0]name=type&facets[0]value=service&limit=1000";
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

                if (!ServiceList.ContainsKey(ServiceUuid))
                {
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
