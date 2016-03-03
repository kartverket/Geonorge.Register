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

namespace Kartverket.Register.Controllers
{
    public class ServiceAlertsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();
        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _AccessControlService;

        public ServiceAlertsController(IRegisterItemService registerItemServive, IRegisterService registerService, IAccessControlService accessControlService)
        {
            _registerItemService = registerItemServive;
            _registerService = registerService;
            _AccessControlService = accessControlService;
        }


        // GET: ServiceAlerts/Create
        [Route("tjenestevarsler/{parentregister}/{registerowner}/{registerName}/ny")]
        [Route("tjenestevarsler/{registerName}/ny")]
        public ActionResult Create(string parentRegister, string registerName)
        {
            ServiceAlert serviceAlert = new ServiceAlert();
            serviceAlert.register = _registerService.GetRegister(parentRegister, registerName);
            ViewBag.OwnerId = _registerItemService.GetOwnerSelectList(serviceAlert.OwnerId);
            ViewBag.AlertType = new SelectList(serviceAlert.GetAlertTypes());
            ViewBag.ServiceUuid = GetServicesFromKartkatalogen();

            if (serviceAlert.register != null)
            {
                if (_AccessControlService.Access(serviceAlert.register))
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
                if (_AccessControlService.Access(serviceAlert.register))
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
            ViewBag.OwnerId = _registerItemService.GetOwnerSelectList(serviceAlert.OwnerId);
            ViewBag.AlertType = new SelectList(serviceAlert.GetAlertTypes());
            ViewBag.ServiceUuid = GetServicesFromKartkatalogen();
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
                if (_AccessControlService.Access(serviceAlert))
                {
                    ViewBag.OwnerId = _registerItemService.GetOwnerSelectList(serviceAlert.OwnerId);
                    ViewBag.AlertType = new SelectList(serviceAlert.GetAlertTypes(), serviceAlert.AlertType);
                    ViewBag.ServiceUuid = GetServicesFromKartkatalogen(serviceAlert.ServiceUuid);
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
                if (_AccessControlService.Access(originalServiceAlert))
                {
                    if (!_registerItemService.validateName(serviceAlert))
                    {
                        ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                        ViewBag.OwnerId = _registerItemService.GetOwnerSelectList(serviceAlert.OwnerId);
                        ViewBag.AlertType = new SelectList(serviceAlert.GetAlertTypes(), serviceAlert.AlertType);
                        ViewBag.ServiceUuid = GetServicesFromKartkatalogen(originalServiceAlert.ServiceUuid);
                        return View(originalServiceAlert);
                    }
                    if (ModelState.IsValid)
                    {
                        originalServiceAlert.UpdateServiceAlert(serviceAlert);
                        _registerItemService.SaveEditedRegisterItem(originalServiceAlert);
                        return Redirect(originalServiceAlert.register.GetObjectUrl());
                    }
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            ViewBag.OwnerId = _registerItemService.GetOwnerSelectList(serviceAlert.OwnerId);
            ViewBag.AlertType = new SelectList(serviceAlert.GetAlertTypes(), serviceAlert.AlertType);
            ViewBag.ServiceUuid = GetServicesFromKartkatalogen(originalServiceAlert.ServiceUuid);
            return View(originalServiceAlert);
        }



        // ***** Hjelpemetoder *****

        private SelectList GetServicesFromKartkatalogen(string serviceUuid = null)
        {
            var servicesFromKartkatalogen = new MetadataService().GetMetadataServices();
            List<MetadataItemViewModel> result = new List<MetadataItemViewModel>();

            if (servicesFromKartkatalogen.numberOfRecordsMatched != "0")
            {
                for (int s = 0; s < servicesFromKartkatalogen.Items.Length; s++)
                {
                    MetadataItemViewModel m = new MetadataItemViewModel();
                    m.Uuid = ((www.opengis.net.DCMIRecordType)(servicesFromKartkatalogen.Items[s])).Items[0].Text[0];
                    m.Title = ((www.opengis.net.DCMIRecordType)(servicesFromKartkatalogen.Items[s])).Items[2].Text[0];
                    result.Add(m);
                }
            }
            return new SelectList(result, "Uuid", "Title", serviceUuid);
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
