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

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class OrganizationsController : Controller
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly RegisterDbContext _dbContext;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;
        private readonly IAccessControlService _accessControlService;

        public OrganizationsController(RegisterDbContext dbContext, IRegisterService registerService, IRegisterItemService registerItemService, IAccessControlService accessControlService)
        {
            _dbContext = dbContext;
            _registerService = registerService;
            _registerItemService = registerItemService;
            _accessControlService = accessControlService;
        }

        [Authorize]
        public ActionResult Import()
        {
            string role = GetSecurityClaim("role");
            if (role == "nd.metadata_admin")
            {
                return View();
            }
            else 
            return HttpNotFound("Ingen tilgang");
           
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
                
                string organizationLogin = GetSecurityClaim("organization");
                var queryResultsOrganization = from o in _dbContext.Organizations
                                               where o.name == organizationLogin
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
        [Route("organisasjoner/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("organisasjoner/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            Organization organisasjon = new Organization();
            organisasjon.register = _registerService.GetRegister(parentRegister, registername);
            if (organisasjon.register != null)
            {
                if (_accessControlService.Access(organisasjon.register))
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
        [Route("organisasjoner/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("organisasjoner/{registername}/ny")]
        public ActionResult Create(Organization organization, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string registername, string parentRegister, HttpPostedFileBase agreementDocument, HttpPostedFileBase priceformDocument, string registerOwner)
        {
            organization.register = _registerService.GetRegister(parentRegister, registername);
            if (organization.register != null)
            {
                if (_accessControlService.Access(organization.register))
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
                        return Redirect(RegisterUrls.DeatilsRegisterItemUrl(parentRegister, registerOwner, registername, organization.submitter.seoname, organization.seoname));

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
            return _registerItemService.validateName(organization);
        }

        //// GET: Organizations/Edit/5
        [Authorize]
        [Route("organisasjoner/{registerParent}/{registerowner}/{registername}/{itemowner}/{organisasjon}/rediger")]
        [Route("organisasjoner/{registername}/{innsender}/{organisasjon}/rediger")]
        public ActionResult Edit(string registername, string organisasjon, string registerParent)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResultsOrganisasjon = from o in _dbContext.Organizations
                                           where o.seoname == organisasjon && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == registerParent
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.FirstOrDefault();

            var queryResultsRegister = from o in _dbContext.Registers
                                       where o.seoname == registername
                                       && o.parentRegister.seoname == registerParent
                                       select o.systemId;

            Guid regId = queryResultsRegister.FirstOrDefault();
            Models.Register register = _dbContext.Registers.Find(regId);

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization org = _dbContext.Organizations.Find(systId);
            if (org == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && org.register.accessId == 2 && org.submitter.name.ToLower() == user.ToLower()))
            {
                ViewbagsOrganization(org, register);
                return View(org);
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
            ViewBag.statusId = new SelectList(_dbContext.Statuses.OrderBy(s => s.description), "value", "description", organization.statusId);
            ViewBag.submitterId = new SelectList(_dbContext.Organizations.OrderBy(s => s.name), "SystemId", "name", organization.submitterId);
        }


        // POST: Organizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("organisasjoner/{registerParent}/{registerowner}/{registername}/{itemowner}/{organisasjon}/rediger")]
        [Route("organisasjoner/{registername}/{innsender}/{organisasjon}/rediger")]
        public ActionResult Edit(Organization org, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string registername, string organisasjon, string registerParent, HttpPostedFileBase agreementDocument, HttpPostedFileBase priceformDocument)
        {
            var queryResultsRegister = from o in _dbContext.Registers
                                       where o.seoname == registername
                                       && o.parentRegister.seoname == registerParent
                                       select o.systemId;

            Guid regId = queryResultsRegister.First();
            Models.Register register = _dbContext.Registers.Find(regId);
          
            var queryResultsOrganisasjon = from o in _dbContext.Organizations
                                           where o.seoname == organisasjon && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == registerParent
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.FirstOrDefault();
            Organization originalOrganization = _dbContext.Organizations.Find(systId);

            ValidationName(org, register);

            if (ModelState.IsValid)
            {
                if (org.name != null)
                {
                    originalOrganization.name = org.name;
                    originalOrganization.seoname = RegisterUrls.MakeSeoFriendlyString(org.name);
                }
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

                originalOrganization.modified = DateTime.Now;
                _dbContext.Entry(originalOrganization).State = EntityState.Modified;
                _dbContext.SaveChanges();
                ViewbagsOrganization(org, register);

                if (register.parentRegister != null)
                {
                    return Redirect("/subregister/" + register.parentRegister.seoname + "/" + register.parentRegister.owner.seoname + "/" + originalOrganization.register.seoname + "/" + originalOrganization.submitter.seoname + "/" + originalOrganization.seoname);
                }
                else
                {
                    return Redirect("/register/" + originalOrganization.register.seoname + "/" + originalOrganization.submitter.seoname + "/" + originalOrganization.seoname);    
                }
                            
            }
            ViewbagsOrganization(org, register);
            return View(originalOrganization);
        }

        // GET: Organizations/Delete/5
        [Authorize]
        [Route("organisasjoner/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{organisasjon}/slett")]
        [Route("organisasjoner/{registername}/{submitter}/{organisasjon}/slett")]
        public ActionResult Delete(string registername, string submitter, string organisasjon, string parentregister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");
            
            var queryResults = from o in _dbContext.Organizations
                                            where o.seoname == organisasjon && o.register.seoname == registername
                                            && o.register.parentRegister.seoname == parentregister
                                            select o.systemId;

            Guid systId = queryResults.FirstOrDefault();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = _dbContext.Organizations.Find(systId);
            if (organization == null)
            {
                return HttpNotFound();
            }

            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && organization.register.accessId == 2 && organization.submitter.name.ToLower() == user.ToLower()))
            {
                return View(organization);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: Registers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("organisasjoner/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{organisasjon}/slett")]
        [Route("organisasjoner/{registername}/{submitter}/{organisasjon}/slett")]
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

            if (queryResultsDataset.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");
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

