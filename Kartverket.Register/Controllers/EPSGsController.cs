using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services;
using Kartverket.Register.Helpers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Kartverket.Register.Services.Translation;
using Resources;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class EPSGsController : BaseController
    {
        private readonly RegisterDbContext db;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _accessControlService;
        private ITranslationService _translationService;

        public EPSGsController(ITranslationService translationService, RegisterDbContext dbContext, IRegisterItemService registerItemService, IRegisterService registerService, IAccessControlService accessControlService)
        {
            db = dbContext;
            _registerItemService = registerItemService;
            _registerService = registerService;
            _accessControlService = accessControlService;
            _translationService = translationService;
        }


        // GET: EPSGs/Create
        [System.Web.Mvc.Authorize]
        //[Route("epsg/{registername}/ny")]
        //[Route("epsg/{parentRegister}/{registerowner}/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            EPSG ePSg = new EPSG();
            ePSg.AddMissingTranslations();
            ePSg.register = _registerService.GetRegister(parentRegister, registername);
            if (ePSg.register != null)
            {
                if (_accessControlService.HasAccessTo(ePSg.register))
                {
                    ViewBag.dimensionId = new SelectList(db.Dimensions.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }).OrderBy(s => s.description), "value", "description", string.Empty);
                    return View(ePSg);
                }
            }
            return HttpNotFound("Ingen tilgang");
        }


        // POST: EPSG/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        //[Route("epsg/{parentRegister}/{registerowner}/{registername}/ny")]
        //[Route("epsg/{registername}/ny")]
        public ActionResult Create(EPSG epsgKode, string registername, string parentRegister, string registerowner)
        {
            epsgKode.register = _registerService.GetRegister(parentRegister, registername);
            if (epsgKode.register != null)
            {
                if (_accessControlService.HasAccessTo(epsgKode.register))
                {
                    if (ModelState.IsValid)
                    {
                        initialisationEPSG(epsgKode);
                        if (!NameIsValid(epsgKode))
                        {
                            ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                            ViewBag.dimensionId = new SelectList(db.Dimensions.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }).OrderBy(s => s.description), "value", "description", string.Empty);
                            return View(epsgKode);
                        }
                        _registerItemService.SaveNewRegisterItem(epsgKode);
                        return Redirect(RegisterUrls.DeatilsRegisterItemUrl(parentRegister, registerowner, registername, epsgKode.submitter.seoname, epsgKode.seoname));

                    }
                    else
                    {
                        throw new HttpException(401, "Access Denied");
                    }
                }
            }
            ViewBag.dimensionId = new SelectList(db.Dimensions.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }).OrderBy(s => s.description), "value", "description", string.Empty);
            return View(epsgKode);
        }


        // GET: EPSGs/Edit/5
        [System.Web.Mvc.Authorize]
        //[Route("epsg/{parentRegister}/{registerowner}/{registername}/{itemowner}/{epsgname}/rediger")]
        //[Route("epsg/{registername}/{organization}/{epsgname}/rediger")]
        public ActionResult Edit(string registername, string epsgname, int? vnr, string parentRegister)
        {
            string currentUserOrganizationName = CurrentUserOrganizationName();

            var queryResultsEpsg = from o in db.EPSGs
                                   where o.seoname == epsgname &&
                                   o.register.seoname == registername &&
                                   o.register.parentRegister.seoname == parentRegister
                                   select o.systemId;
            Guid systId = queryResultsEpsg.FirstOrDefault();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EPSG ePSG = db.EPSGs.Find(systId);
            if (ePSG == null)
            {
                return HttpNotFound();
            }
            if (CanEditOrDeleteEpsgItem(ePSG))
            {
                ePSG.AddMissingTranslations();
                Viewbags(ePSG);
                return View(ePSG);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: EPSGs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        //[Route("epsg/{parentRegister}/{registerowner}/{registername}/{itemowner}/{epsgname}/rediger")]
        //[Route("epsg/{registername}/{organization}/{epsgname}/rediger")]
        //public ActionResult Edit(EPSG ePSG, string name, string id)
        public ActionResult Edit(EPSG ePSG, string epsgname, string parentRegister, string registername)
        {
            var queryResultsOrganisasjon = from o in db.EPSGs
                                           where o.seoname == epsgname
                                           && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == parentRegister
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.FirstOrDefault();
            EPSG epsg = db.EPSGs.Find(systId);

            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername && o.parentRegister.seoname == parentRegister
                                       select o.systemId;

            Guid regId = queryResultsRegister.FirstOrDefault();
            Models.Register register = db.Registers.Find(regId);

            ValidationName(ePSG, register);

            if (ModelState.IsValid)
            {
                EPSG originalEPSG = db.EPSGs.Find(systId);

                if (ePSG.name != null) originalEPSG.name = ePSG.name; originalEPSG.seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(originalEPSG.name);
                if (ePSG.description != null) originalEPSG.description = ePSG.description;
                if (ePSG.submitterId != null) originalEPSG.submitterId = ePSG.submitterId;
                if (ePSG.statusId != null) originalEPSG.statusId = ePSG.statusId;
                if (ePSG.epsgcode != null) originalEPSG.epsgcode = ePSG.epsgcode;
                if (ePSG.verticalReferenceSystem != null) originalEPSG.verticalReferenceSystem = ePSG.verticalReferenceSystem;
                if (ePSG.horizontalReferenceSystem != null) originalEPSG.horizontalReferenceSystem = ePSG.horizontalReferenceSystem;
                if (ePSG.dimensionId != null) originalEPSG.dimensionId = ePSG.dimensionId;
                if (ePSG.sosiReferencesystem != null) originalEPSG.sosiReferencesystem = ePSG.sosiReferencesystem;
                if (ePSG.externalReference != null) originalEPSG.externalReference = ePSG.externalReference;
                if (ePSG.inspireRequirementId != null) originalEPSG.inspireRequirementId = ePSG.inspireRequirementId;
                if (ePSG.inspireRequirementDescription != null) originalEPSG.inspireRequirementDescription = ePSG.inspireRequirementDescription;
                if (ePSG.nationalRequirementId != null) originalEPSG.nationalRequirementId = ePSG.nationalRequirementId;
                if (ePSG.nationalRequirementDescription != null) originalEPSG.nationalRequirementDescription = ePSG.nationalRequirementDescription;
                if (ePSG.nationalSeasRequirementId != null) originalEPSG.nationalSeasRequirementId = ePSG.nationalSeasRequirementId;
                if (ePSG.nationalSeasRequirementDescription != null) originalEPSG.nationalSeasRequirementDescription = ePSG.nationalSeasRequirementDescription;
                if (ePSG.statusId != null)
                {
                    if (ePSG.statusId == "Valid" && originalEPSG.statusId != "Valid")
                    {
                        originalEPSG.dateAccepted = DateTime.Now;
                    }
                    if (originalEPSG.statusId == "Valid" && ePSG.statusId != "Valid")
                    {
                        originalEPSG.dateAccepted = null;
                    }
                    originalEPSG.statusId = ePSG.statusId;
                }
                _translationService.UpdateTranslations(ePSG, originalEPSG);
                originalEPSG.modified = DateTime.Now;
                db.Entry(originalEPSG).State = EntityState.Modified;
                db.SaveChanges();

                Viewbags(ePSG);
                if (!String.IsNullOrWhiteSpace(parentRegister))
                {
                    return Redirect("/subregister/" + originalEPSG.register.parentRegister.seoname + "/" + originalEPSG.register.parentRegister.owner.seoname + "/" + registername + "/" + "/" + originalEPSG.submitter.seoname + "/" + originalEPSG.seoname);
                }
                else
                {
                    return Redirect("/register/" + registername + "/" + originalEPSG.submitter.seoname + "/" + originalEPSG.seoname);
                }

            }
            Viewbags(ePSG);
            return View(epsg);
        }


        // GET: EPSGs/Delete/5
        [System.Web.Mvc.Authorize]
        //[Route("epsg/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{epsgname}/slett")]
        //[Route("epsg/{registername}/{organization}/{epsgname}/slett")]
        public ActionResult Delete(string epsgname, string registername, string parentregister)
        {
            var queryResultsOrganisasjon = from o in db.EPSGs
                                           where o.seoname == epsgname
                                           && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == parentregister
                                           select o.systemId;
            Guid systId = queryResultsOrganisasjon.FirstOrDefault();

            EPSG ePSG = db.EPSGs.Find(systId);
            if (ePSG == null)
            {
                return HttpNotFound();
            }
            if (CanEditOrDeleteEpsgItem(ePSG))
            {
                Viewbags(ePSG);
                return View(ePSG);
            }
            return HttpNotFound("Ingen tilgang");
        }

        private bool CanEditOrDeleteEpsgItem(EPSG item)
        {
            return IsAdmin()
                   ||
                   IsEditor() 
                        && item.register.accessId == 2 
                        && item.submitter.name.ToLower() == CurrentUserOrganizationName().ToLower();
        }

        // POST: EPSGs/Delete/5
        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost, System.Web.Mvc.ActionName("Delete")]
        //[Route("epsg/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{epsgname}/slett")]
        //[Route("epsg/{registername}/{organization}/{epsgname}/slett")]
        public ActionResult DeleteConfirmed(string epsgname, string registername, string parentregister, string parentregisterowner)
        {
            var queryResultsOrganisasjon = from o in db.EPSGs
                                           where o.seoname == epsgname
                                           && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == parentregister
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.FirstOrDefault();
            EPSG ePSG = db.EPSGs.Find(systId);

            if (!CanEditOrDeleteEpsgItem(ePSG))
                return new HttpUnauthorizedResult("Ingen tilgang");

            string parent = null;
            if (ePSG.register.parentRegisterId != null)
            {
                parent = ePSG.register.parentRegister.seoname;
            }

            db.RegisterItems.Remove(ePSG);
            db.SaveChanges();

            if (parent != null)
            {
                return Redirect("/subregister/" + parentregister + "/" + parentregisterowner + "/" + registername);
            }
            return Redirect("/register/" + registername);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Viewbags(EPSG ePSG)
        {
            ViewBag.statusId = new SelectList(db.Statuses.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }).OrderBy(o => o.description), "value", "description", ePSG.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.ToList().Select(s => new { systemId = s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", ePSG.submitterId);
            ViewBag.inspireRequirementId = new SelectList(db.requirements.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }), "value", "description", ePSG.inspireRequirementId);
            ViewBag.nationalRequirementId = new SelectList(db.requirements.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }), "value", "description", ePSG.nationalRequirementId);
            ViewBag.nationalSeasRequirementId = new SelectList(db.requirements.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }), "value", "description", ePSG.nationalSeasRequirementId);
            ViewBag.dimensionId = new SelectList(db.Dimensions.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }).OrderBy(s => s.description), "value", "description", ePSG.dimensionId);
        }

        private void ValidationName(EPSG epsg, Models.Register register)
        {
            var queryResultsDataset = from o in db.EPSGs
                                      where o.name == epsg.name &&
                                      o.systemId != epsg.systemId &&
                                      o.register.name == register.name &&
                                      o.register.parentRegisterId == register.parentRegisterId
                                      select o.systemId;

            if (queryResultsDataset.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessage", Registers.ErrorMessageValidationName);
            }
        }

        private bool NameIsValid(EPSG epsgKode)
        {
            return _registerItemService.ItemNameIsValid(epsgKode);
        }

        private string EPSGName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "ikke angitt";
            }
            else
            {
                return name;
            }
        }

        private int GetVersionNr(int versionNumber)
        {
            if (versionNumber == 0)
            {
                versionNumber = 1;
            }
            else
            {
                versionNumber++;
            }
            return versionNumber;
        }

        private void initialisationEPSG(EPSG epsgKode)
        {
            epsgKode.systemId = Guid.NewGuid();
            epsgKode.modified = DateTime.Now;
            epsgKode.dateSubmitted = DateTime.Now;
            epsgKode.registerId = epsgKode.register.systemId;
            epsgKode.statusId = "Submitted";
            epsgKode.versionNumber = GetVersionNr(epsgKode.versionNumber);
            epsgKode.name = EPSGName(epsgKode.name);
            epsgKode.seoname = EPSGSeoName(epsgKode.name);
            epsgKode.versioningId = _registerItemService.NewVersioningGroup(epsgKode);
            SetEPSGOwnerAndSubmitter(epsgKode);
            epsgKode.inspireRequirementId = "Notset";
            epsgKode.nationalRequirementId = "Notset";
            epsgKode.nationalSeasRequirementId = "Notset";
        }

        private void SetEPSGOwnerAndSubmitter(EPSG epsgKode)
        {
            Organization submitterOrganisasjon = _registerService.GetOrganizationByUserName();
            epsgKode.submitterId = submitterOrganisasjon.systemId;
            epsgKode.submitter = submitterOrganisasjon;
        }

        private string EPSGSeoName(string name)
        {
            return Helpers.RegisterUrls.MakeSeoFriendlyString(name);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }
    }
}
