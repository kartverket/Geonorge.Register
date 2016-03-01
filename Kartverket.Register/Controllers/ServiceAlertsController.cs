using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Controllers
{
    public class ServiceAlertsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();
        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _AccessControlService;

        public ServiceAlertsController(IRegisterItemService registerItemServive, IRegisterService registerService, IAccessControlService accessControlService) {
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
                    if (ModelState.IsValid)
                    {
                        serviceAlert.InitializeNewServiceAlert();
                        serviceAlert.submitter = _registerService.GetOrganizationByUserName();
                        serviceAlert.submitterId = serviceAlert.submitter.systemId;
                        serviceAlert.versioningId = _registerItemService.NewVersioningGroup(serviceAlert);
                    }
                    if (!_registerItemService.validateName(serviceAlert))
                    {
                        ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                        return View(serviceAlert);
                    }
                    _registerItemService.SaveNewRegisterItem(serviceAlert);
                    return Redirect(serviceAlert.GetObjectUrl());
                }
            }
            ViewBag.OwnerId = _registerItemService.GetOwnerSelectList(serviceAlert.OwnerId);
            ViewBag.AlertType = new SelectList(serviceAlert.GetAlertTypes());
            return View(serviceAlert);
        }

        //// GET: ServiceAlerts/Edit/5
        //public ActionResult Edit(Guid? id)
        //{

        //}

        //// POST: ServiceAlerts/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "systemId,name,description,submitterId,dateSubmitted,modified,statusId,dateAccepted,dateNotAccepted,dateSuperseded,DateRetired,registerId,seoname,versioningId,versionNumber,versionName,documentUrl,approvalDocument,approvalReference,Accepted,AlertDate,EffectiveDate,AlertType,ServiceType,OwnerId,Note,ServiceMetadataUrl,ServiceUuid")] ServiceAlert serviceAlert)
        //{

        //}

        //// GET: ServiceAlerts/Delete/5
        //public ActionResult Delete(Guid? id)
        //{

        //}

        //// POST: ServiceAlerts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{

        //}

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
