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

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class EPSGsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _accessControlService;

        public EPSGsController()
        {
            _registerItemService = new RegisterItemService(db);
            _registerService = new RegisterService(db);
            _accessControlService = new AccessControlService();
        }


        // GET: EPSGs/Create
        [Authorize]
        [Route("epsg/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("epsg/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            EPSG ePSg = new EPSG();
            ePSg.register = _registerService.GetRegister(parentRegister, registername);
            if (ePSg.register != null)
            {
                if (_accessControlService.Access(ePSg.register))
                {
                    ViewBag.dimensionId = new SelectList(db.Dimensions.OrderBy(s => s.description), "value", "description", string.Empty);
                    return View(ePSg);
                }
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: EPSG/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [Route("epsg/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("epsg/{registername}/ny")]
        public ActionResult Create(EPSG epsgKode, string registername, string parentRegister, string registerowner)
        {
            epsgKode.register = _registerService.GetRegister(parentRegister, registername);
            if (epsgKode.register != null)
            {
                if (_accessControlService.Access(epsgKode.register))
                {
                    if (ModelState.IsValid)
                    {
                        initialisationEPSG(epsgKode);
                        if (!NameIsValid(epsgKode))
                        {
                            ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                            ViewBag.dimensionId = new SelectList(db.Dimensions.OrderBy(s => s.description), "value", "description", string.Empty);
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
            ViewBag.dimensionId = new SelectList(db.Dimensions.OrderBy(s => s.description), "value", "description", string.Empty);
            return View(epsgKode);
        }

        private bool NameIsValid(EPSG epsgKode)
        {
            return _registerItemService.validateName(epsgKode);
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

        // GET: EPSGs/Edit/5
        [Authorize]
        [Route("epsg/{parentRegister}/{registerowner}/{registername}/{itemowner}/{epsgname}/rediger")]
        [Route("epsg/{registername}/{organization}/{epsgname}/rediger")]
        public ActionResult Edit(string registername, string epsgname, int? vnr, string parentRegister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

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

            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && ePSG.register.accessId == 2 && ePSG.submitter.name.ToLower() == user.ToLower()))
            {
                Viewbags(ePSG);
                return View(ePSG);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: EPSGs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [Route("epsg/{parentRegister}/{registerowner}/{registername}/{itemowner}/{epsgname}/rediger")]
        [Route("epsg/{registername}/{organization}/{epsgname}/rediger")]
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
        [Authorize]
        [Route("epsg/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{epsgname}/slett")]
        [Route("epsg/{registername}/{organization}/{epsgname}/slett")]
        public ActionResult Delete(string epsgname, string registername, string parentregister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResultsOrganisasjon = from o in db.EPSGs
                                           where o.seoname == epsgname
                                           && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == parentregister
                                           select o.systemId;
            Guid systId = queryResultsOrganisasjon.FirstOrDefault();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EPSG ePSG = db.EPSGs.Find(systId);
            if (ePSG == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && ePSG.register.accessId == 2 && ePSG.submitter.name.ToLower() == user.ToLower()))
            {
                Viewbags(ePSG);
                return View(ePSG);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: EPSGs/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [Route("epsg/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{epsgname}/slett")]
        [Route("epsg/{registername}/{organization}/{epsgname}/slett")]
        public ActionResult DeleteConfirmed(string epsgname, string registername, string parentregister, string parentregisterowner)
        {
            var queryResultsOrganisasjon = from o in db.EPSGs
                                           where o.seoname == epsgname
                                           && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == parentregister
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.FirstOrDefault();
            EPSG ePSG = db.EPSGs.Find(systId);

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

        private string GetSecurityClaim(string type)
        {
            string result = null;
            foreach (var claim in System.Security.Claims.ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    result = claim.Value;
                    break;
                }
            }

            // bad hack, must fix BAAT
            if (!string.IsNullOrWhiteSpace(result) && type.Equals("organization") && result.Equals("Statens kartverk"))
            {
                result = "Kartverket";
            }

            return result;
        }

        private void Viewbags(EPSG ePSG)
        {
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", ePSG.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", ePSG.submitterId);
            ViewBag.inspireRequirementId = new SelectList(db.requirements, "value", "description", ePSG.inspireRequirementId);
            ViewBag.nationalRequirementId = new SelectList(db.requirements, "value", "description", ePSG.nationalRequirementId);
            ViewBag.nationalSeasRequirementId = new SelectList(db.requirements, "value", "description", ePSG.nationalSeasRequirementId);
            ViewBag.dimensionId = new SelectList(db.Dimensions, "value", "description", ePSG.dimensionId);
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
                ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }
    }
}
