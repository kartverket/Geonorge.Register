using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services.Translation;
using Resources;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class OrganizationsController : BaseController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly RegisterDbContext _dbContext;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;
        private readonly IAccessControlService _accessControlService;
        private ITranslationService _translationService;

        public OrganizationsController(RegisterDbContext dbContext, IRegisterService registerService, IRegisterItemService registerItemService, IAccessControlService accessControlService, ITranslationService translationService)
        {
            _dbContext = dbContext;
            _registerService = registerService;
            _registerItemService = registerItemService;
            _accessControlService = accessControlService;
            _translationService = translationService;
        }

        [Authorize]
        public ActionResult Import()
        {
            if (!IsAdmin())
                return HttpNotFound("Ingen tilgang");

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Import(HttpPostedFileBase csvfile)
        {

            string filename =  "import_" + Path.GetFileName(csvfile.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Organization.DataDirectory), filename);
            csvfile.SaveAs(path);

            var lines = System.IO.File.ReadAllLines(path).Select(a => a.Split(';')).Skip(1);
            foreach (var org in lines)
            {
                //orgnr, navn, beskrivelse, logo
                Organization organization = new Organization();
                organization.systemId = Guid.NewGuid();
                organization.number = org[0];
                organization.name = org[1];
                organization.description = org[2];
                if (organization.name == null)
                {
                    organization.name = "ikke angitt";
                }

                string currentUserOrganizationName = CurrentUserOrganizationName();

                var queryResultsOrganization = from o in _dbContext.Organizations
                                               where o.name == currentUserOrganizationName
                                               select o.systemId;
                Guid orgId = queryResultsOrganization.FirstOrDefault();
                Organization submitterOrganisasjon = _dbContext.Organizations.Find(orgId);

                organization.submitterId = orgId;
                organization.submitter = submitterOrganisasjon;
                var queryResultsRegister = from o in _dbContext.Registers
                                           where o.name == "Organisasjoner"
                                           select o.systemId;
                Guid regId = queryResultsRegister.FirstOrDefault();

                organization.modified = DateTime.Now;
                organization.dateSubmitted = DateTime.Now;
                organization.registerId = regId;
                organization.statusId = "Submitted";
                organization.seoname = RegisterUrls.MakeSeoFriendlyString(organization.name);

                organization.logoFilename = org[3];
                organization.largeLogo = org[3];

                _dbContext.RegisterItems.Add(organization);
                _dbContext.SaveChanges(); 
            }
           
            return Redirect("/register/organisasjoner");
        }
        

        //// GET: Organizations/Create
        [Authorize]
        //[Route("organisasjoner/{parentRegister}/{registerowner}/{registername}/ny")]
        //[Route("organisasjoner/{registername}/ny")]
        public ActionResult Create(string systemid)
        {
            Organization organisasjon = new Organization();
            organisasjon.AddMissingTranslations();
            organisasjon.register = _registerService.GetRegisterBySystemId(Guid.Parse(systemid));
            if (organisasjon.register != null)
            {
                if (_accessControlService.HasAccessTo(organisasjon.register))
                {
                    ViewBag.dimensionId = new SelectList(_dbContext.Dimensions.OrderBy(s => s.description), "value", "description", string.Empty);
                    return View(organisasjon);
                }
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: Organizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        //[Route("organisasjoner/{parentRegister}/{registerowner}/{registername}/ny")]
        //[Route("organisasjoner/{registername}/ny")]
        public ActionResult Create(Organization organization, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string systemid, HttpPostedFileBase agreementDocument, HttpPostedFileBase priceformDocument, string registerOwner)
        {
            organization.register = _registerService.GetRegisterBySystemId(Guid.Parse(systemid));
            if (organization.register != null)
            {
                if (_accessControlService.HasAccessTo(organization.register))
                {
                    if (ModelState.IsValid)
                    {
                        InitializeOrganization(organization, fileSmal, fileLarge, agreementDocument, priceformDocument);
                        if (!NameIsValid(organization))
                        {
                            ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                            return View(organization);
                        }
                        _registerItemService.SaveNewRegisterItem(organization);
                        return Redirect("/" + organization.register.path + "/" + organization.seoname + "/" + organization.systemId);
                        //return Redirect(RegisterUrls.DeatilsRegisterItemUrl(parentRegister, registerOwner, registername, organization.submitter.seoname, organization.seoname));

                    }
                    else
                    {
                        throw new HttpException(401, "Access Denied");
                    }
                }
            }
            return View(organization);
        }

        private bool NameIsValid(Organization organization)
        {
            return _registerItemService.ItemNameIsValid(organization);
        }

        //// GET: Organizations/Edit/5
        [Authorize]
        //[Route("organisasjoner/{registerParent}/{registerowner}/{registername}/{itemowner}/{organisasjon}/rediger")]
        //[Route("organisasjoner/{registername}/{itemowner}/{organisasjon}/rediger")]
        public ActionResult Edit(string registername, string organisasjon, string registerParent, string itemowner)
        {
            Organization organization = (Organization)_registerItemService.GetRegisterItem(registerParent, registername, organisasjon, 1, itemowner);


            var register = _registerService.GetRegister(registerParent, registername);
            if (organisasjon == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (_accessControlService.HasAccessTo(organisasjon))
            {
                organization.AddMissingTranslations();
                ViewbagsOrganization(organization, register);
                return View(organization);
            }
            return HttpNotFound("Ingen tilgang");
        }

        

        private void ViewbagsOrganization(Organization organization, Models.Register register)
        {
            if (register.parentRegisterId != null)
            {
                ViewBag.registerOwner = register.parentRegister.owner.seoname;
                ViewBag.parentRegister = register.parentRegister.seoname;
            }
            ViewBag.registername = register.name;
            ViewBag.registerSEO = register.seoname;
            ViewBag.statusId = new SelectList(_dbContext.Statuses.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }).OrderBy(o => o.description), "value", "description", organization.statusId);
            ViewBag.submitterId = new SelectList(_dbContext.Organizations.ToList().Select(s => new { systemId = s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", organization.submitterId);
        }


        // POST: Organizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        //[Route("organisasjoner/{registerParent}/{registerowner}/{registername}/{itemowner}/{organisasjon}/rediger")]
        //[Route("organisasjoner/{registername}/{innsender}/{organisasjon}/rediger")]
        public ActionResult Edit(Organization org, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string registername, string organisasjon, string registerParent, HttpPostedFileBase agreementDocument, HttpPostedFileBase priceformDocument)
        {
            var register = _registerService.GetRegister(registerParent, registername);
            Organization originalOrganization = (Organization)_registerItemService.GetRegisterItem(registerParent, registername, organisasjon, 1);
            
            ValidationName(org, register);

            if (ModelState.IsValid)
            {
                if (org.name != null)
                {
                    originalOrganization.name = org.name;
                    originalOrganization.seoname = RegisterUrls.MakeSeoFriendlyString(org.name);
                }

                originalOrganization.shortname = org.shortname;

                if (org.submitterId != null)
                {
                    originalOrganization.submitterId = org.submitterId;
                }
                if (!string.IsNullOrEmpty(org.number))
                {
                    originalOrganization.number = org.number;
                }
                if (!string.IsNullOrEmpty(org.description))
                {
                    originalOrganization.description = org.description;
                }
                if (!string.IsNullOrEmpty(org.contact))
                {
                    originalOrganization.contact = org.contact;
                }
                if (org.statusId != null)
                {
                    originalOrganization.statusId = org.statusId;
                }
                if (fileSmal != null && fileSmal.ContentLength > 0)
                {
                    originalOrganization.logoFilename = SaveLogoOptimizedToDisk(fileSmal, org.number);
                }
                if (fileLarge != null && fileLarge.ContentLength > 0)
                {
                    originalOrganization.largeLogo = SaveLogoToDisk(fileLarge, org.number);
                }
                if (org.agreementYear != null)
                {
                    originalOrganization.agreementYear = org.agreementYear;
                }
                if (org.epost != null)
                {
                    originalOrganization.epost = org.epost;
                }
                if (org.member != null)
                {
                    originalOrganization.member = org.member;
                }
                if (org.priceFormDocument != null)
                {
                    originalOrganization.priceFormDocument = org.priceFormDocument;
                }
                if (org.statusId != null)
                {
                    if (org.statusId == "Valid" && originalOrganization.statusId != "Valid")
                    {
                        originalOrganization.dateAccepted = DateTime.Now;
                    }
                    if (originalOrganization.statusId == "Valid" && org.statusId != "Valid")
                    {
                        originalOrganization.dateAccepted = null;
                    }
                    originalOrganization.statusId = org.statusId;
                }

                string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;
                if (agreementDocument != null)
                {
                    originalOrganization.agreementDocumentUrl = url + SaveFileToDisk(agreementDocument, originalOrganization);
                }

                if (priceformDocument != null)
                {
                    originalOrganization.priceFormDocument = url + SaveFileToDisk(priceformDocument, originalOrganization);
                }

                if (org.OrganizationType != null)
                {
                    originalOrganization.OrganizationType = org.OrganizationType;
                }

                if (org.IsMunicipality())
                {
                    originalOrganization.OrganizationType = org.OrganizationType;
                    originalOrganization.MunicipalityCode = org.MunicipalityCode;
                    originalOrganization.BoundingBoxEast = org.BoundingBoxEast;
                    originalOrganization.BoundingBoxNorth = org.BoundingBoxNorth;
                    originalOrganization.BoundingBoxSouth = org.BoundingBoxSouth;
                    originalOrganization.BoundingBoxWest = org.BoundingBoxWest;
                    originalOrganization.GeographicCenterX = org.GeographicCenterX;
                    originalOrganization.GeographicCenterY = org.GeographicCenterY;
                }

                originalOrganization.modified = DateTime.Now;
                _translationService.UpdateTranslations(org, originalOrganization);
                _dbContext.Entry(originalOrganization).State = EntityState.Modified;
                _dbContext.SaveChanges();
                ViewbagsOrganization(org, register);

                return Redirect(originalOrganization.GetObjectUrl());
                            
            }
            ViewbagsOrganization(org, register);
            return View(originalOrganization);
        }

        // GET: Organizations/Delete/5
        [Authorize]
        //[Route("organisasjoner/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{organisasjon}/slett")]
        //[Route("organisasjoner/{registername}/{submitter}/{organisasjon}/slett")]
        public ActionResult Delete(string registername, string submitter, string organisasjon, string parentregister)
        {
            var queryResults = from o in _dbContext.Organizations
                                            where o.seoname == organisasjon && o.register.seoname == registername
                                            && o.register.parentRegister.seoname == parentregister
                                            select o.systemId;

            Guid systId = queryResults.FirstOrDefault();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organizationRegisterItem = _dbContext.Organizations.Find(systId);
            if (organizationRegisterItem == null)
            {
                return HttpNotFound();
            }

            if (IsAdmin() || 
                IsEditor() 
                && organizationRegisterItem.register.accessId == 2 
                && organizationRegisterItem.BelongsTo(CurrentUserOrganizationName()))
            {
                return View(organizationRegisterItem);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: Registers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        //[Route("organisasjoner/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{organisasjon}/slett")]
        //[Route("organisasjoner/{registername}/{submitter}/{organisasjon}/slett")]
        public ActionResult DeleteConfirmed(Organization organization, string registername, string organisasjon, string parentregister, string parentregisterowner)
        {
            var queryResultsOrganisasjon = from o in _dbContext.Organizations
                                           where o.seoname == organisasjon
                                           && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == parentregister
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.FirstOrDefault();

            Organization originalOrganization = _dbContext.Organizations.Find(systId);            
            
            _dbContext.Organizations.Remove(originalOrganization);
            _dbContext.SaveChanges();

            if (parentregister != null)
            {
                return Redirect("/subregister/" + parentregister + "/" + parentregisterowner + "/" + registername);
            }
            else
            {
                return Redirect("/register/" + registername);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private string SaveFileToDisk(HttpPostedFileBase file, Organization organization)
        {
            string filtype;
            string seofilename;
            MakeSeoFriendlyDocumentName(file, out filtype, out seofilename);

            string filename = organization.register.seoname + "_" + organization.seoname + "_v1_" + seofilename + "." + filtype;
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Document.DataDirectory), filename);
            file.SaveAs(path);
            return filename;
        }

        private static void MakeSeoFriendlyDocumentName(HttpPostedFileBase file, out string filtype, out string seofilename)
        {
            string[] documentfilename = file.FileName.Split('.');
            filtype = documentfilename.Last();
            seofilename = null;
            foreach (string item in documentfilename)
            {
                if (item == filtype)
                {
                    break;
                }
                seofilename += RegisterUrls.MakeSeoFriendlyString(item) + "_";
            }
        }

        private string SaveLogoToDisk(HttpPostedFileBase file, string organizationNumber)
        {
            string filename = organizationNumber + "_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Organization.DataDirectory), filename);
            file.SaveAs(path);
            return filename;
        }

        private string SaveLogoOptimizedToDisk(HttpPostedFileBase file, string organizationNumber)
        {
            string filename = organizationNumber + "_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Organization.DataDirectory), filename);

            ImageResizer.ImageJob newImage =
               new ImageResizer.ImageJob(file, path,
               new ImageResizer.Instructions("maxwidth=215;maxheight=215;quality=75"));

            newImage.Build();

            return filename;
        }

        private void ValidationName(Organization organization, Models.Register register)
        {
            var queryResultsDataset = from o in _dbContext.Organizations
                                      where o.name == organization.name 
                                      && o.systemId != organization.systemId 
                                      && o.register.name == register.name
                                      && o.register.parentRegisterId == register.parentRegisterId
                                      select o.systemId;

            if (queryResultsDataset.Any())
            {
                ModelState.AddModelError("ErrorMessage", Registers.ErrorMessageValidationName);
            }
        }

        private void InitializeOrganization(Organization organization, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, HttpPostedFileBase agreementDocument, HttpPostedFileBase priceformDocument)
        {

            organization.systemId = Guid.NewGuid();
            organization.name = OrganizationName(organization.name);
            organization.seoname = OrganizationSeoName(organization.name);
            organization.modified = DateTime.Now;
            organization.dateSubmitted = DateTime.Now;
            organization.registerId = organization.register.systemId;
            organization.statusId = "Submitted";
            organization.versionNumber = GetVersionNr(organization.versionNumber);
            organization.versioningId = _registerItemService.NewVersioningGroup(organization);

            SetOrganizationOwnerAndSubmitter(organization);

            FileSmal(organization, fileSmal);
            FileLarge(organization, fileLarge);
            organization.agreementDocumentUrl = GetAgreementDocumentUrl(agreementDocument, organization);
            organization.priceFormDocument = GetPriceFormDocument(priceformDocument, organization);
        }

        private string GetPriceFormDocument(HttpPostedFileBase priceformDocument, Organization organization)
        {
            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;
            if (priceformDocument != null)
            {
                organization.priceFormDocument = url + SaveFileToDisk(priceformDocument, organization);
            }
            return null;
        }

        private string GetAgreementDocumentUrl(HttpPostedFileBase agreementDocument, Organization organization)
        {
            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;
            if (agreementDocument != null)
            {
                return url + SaveFileToDisk(agreementDocument, organization);
            }
            return null;
        }

        private void FileLarge(Organization organization, HttpPostedFileBase fileLarge)
        {
            if (fileLarge != null && fileLarge.ContentLength > 0)
            {
                organization.largeLogo = SaveLogoToDisk(fileLarge, organization.number);
            }
        }

        private void FileSmal(Organization organization, HttpPostedFileBase fileSmal)
        {
            if (fileSmal != null && fileSmal.ContentLength > 0)
            {
                organization.logoFilename = SaveLogoOptimizedToDisk(fileSmal, organization.number);
            }
        }

        private void SetOrganizationOwnerAndSubmitter(Organization organization)
        {
            Organization submitterOrganisasjon = _registerService.GetOrganizationByUserName();
            organization.submitterId = submitterOrganisasjon.systemId;
            organization.submitter = submitterOrganisasjon;
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

        private string OrganizationSeoName(string name)
        {
            return RegisterUrls.MakeSeoFriendlyString(name);
        }

        private string OrganizationName(string name)
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


        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }
    }
}

