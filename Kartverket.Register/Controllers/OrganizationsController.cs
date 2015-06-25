using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System;
using Kartverket.Register.Helpers;
using System.Text.RegularExpressions;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class OrganizationsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Organizations
        public ActionResult Index(string searchString)
        {
            return View(db.Organizations.OrderBy(o => o.name).ToList());
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
                var queryResultsOrganization = from o in db.Organizations
                                               where o.name == organizationLogin
                                               select o.systemId;
                Guid orgId = queryResultsOrganization.FirstOrDefault();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                organization.submitterId = orgId;
                organization.submitter = submitterOrganisasjon;
                var queryResultsRegister = from o in db.Registers
                                           where o.name == "Organisasjoner"
                                           select o.systemId;
                Guid regId = queryResultsRegister.FirstOrDefault();

                organization.modified = DateTime.Now;
                organization.dateSubmitted = DateTime.Now;
                organization.registerId = regId;
                organization.statusId = "Submitted";
                organization.seoname = Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(organization.name);

                organization.logoFilename = org[3];
                organization.largeLogo = org[3];

                db.RegisterItems.Add(organization);
                db.SaveChanges(); 
            }
           
            return Redirect("/register/organisasjoner");
        }
        // GET: Organizations/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            db.SaveChanges();
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }


        //// GET: Organizations/Create
        [Authorize]
        [Route("organisasjoner/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("organisasjoner/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            Organization organisasjon = new Organization();
            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername || o.name == registername
                                       && o.parentRegister.seoname == parentRegister
                                       select o.systemId;

            Guid regId = queryResultsRegister.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);
            organisasjon.register = register;

            if (register.parentRegisterId != null)
            {
                organisasjon.register.parentRegister = register.parentRegister;
            }
            
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && register.accessId == 2))
            {
                return View(organisasjon);
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
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Organization organization, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string registername, string parentRegister)
        {
            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername
                                       && o.parentRegister.seoname == parentRegister
                                       select o.systemId;

            Guid regId = queryResultsRegister.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);
            string parentRegisterOwner = null;
            if (register.parentRegisterId != null)
            {
                parentRegisterOwner = register.parentRegister.owner.seoname;
            }
            ValidationName(organization, register);
            
            if (ModelState.IsValid)
            {
                organization.systemId = Guid.NewGuid();
                if (organization.name == null) { organization.name = "ikke angitt"; }
                organization.modified = DateTime.Now;
                organization.dateSubmitted = DateTime.Now;
                organization.registerId = regId;
                organization.statusId = "Submitted";
                organization.seoname = Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(organization.name);

                if (fileSmal != null && fileSmal.ContentLength > 0)
                {
                    organization.logoFilename = SaveLogoToDisk(fileSmal, organization.number);
                }
                if (fileLarge != null && fileLarge.ContentLength > 0)
                {
                    organization.largeLogo = SaveLogoToDisk(fileLarge, organization.number);
                }

                db.RegisterItems.Add(organization);
                db.SaveChanges();

                string organizationLogin = GetSecurityClaim("organization");
                var queryResultsOrganization = from o in db.Organizations
                                               where o.name == organizationLogin
                                               select o.systemId;
                Guid orgId = queryResultsOrganization.FirstOrDefault();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                organization.submitterId = orgId;
                organization.submitter = submitterOrganisasjon;

                db.Entry(organization).State = EntityState.Modified;
                db.SaveChanges();

                if (register.parentRegister != null)
                {
                    return Redirect("/subregister/" + register.parentRegister.seoname + "/" + register.parentRegister.owner.seoname + "/" + registername + "/" + organization.submitter.seoname + "/" + organization.seoname);
                }
                else
                {
                    return Redirect("/register/" + registername + "/" + organization.submitter.seoname + "/" + organization.seoname);
                }
            }
            organization.register = register;
            return View(organization);
        }


        //// GET: Organizations/Edit/5
        [Authorize]
        [Route("organisasjoner/{registerParent}/{registerowner}/{registername}/{itemowner}/{organisasjon}/rediger")]
        [Route("organisasjoner/{registername}/{innsender}/{organisasjon}/rediger")]
        public ActionResult Edit(string registername, string organisasjon, string registerParent)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResultsOrganisasjon = from o in db.Organizations
                                           where o.seoname == organisasjon && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == registerParent
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.FirstOrDefault();

            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername
                                       && o.parentRegister.seoname == registerParent
                                       select o.systemId;

            Guid regId = queryResultsRegister.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kartverket.Register.Models.Organization org = db.Organizations.Find(systId);
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

        

        private void ViewbagsOrganization(Kartverket.Register.Models.Organization organization, Kartverket.Register.Models.Register register)
        {
            if (register.parentRegisterId != null)
            {
                ViewBag.registerOwner = register.parentRegister.owner.seoname;
                ViewBag.parentRegister = register.parentRegister.seoname;
            }
            ViewBag.registername = register.name;
            ViewBag.registerSEO = register.seoname;
            ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", organization.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "SystemId", "name", organization.submitterId);
        }


        // POST: Organizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("organisasjoner/{registerParent}/{registerowner}/{registername}/{itemowner}/{organisasjon}/rediger")]
        [Route("organisasjoner/{registername}/{innsender}/{organisasjon}/rediger")]
        public ActionResult Edit(Organization org, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string registername, string organisasjon, string registerParent)
        {
            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername
                                       && o.parentRegister.seoname == registerParent
                                       select o.systemId;

            Guid regId = queryResultsRegister.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);
          
            var queryResultsOrganisasjon = from o in db.Organizations
                                           where o.seoname == organisasjon && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == registerParent
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.FirstOrDefault();
            Organization originalOrganization = db.Organizations.Find(systId);

            ValidationName(org, register);

            var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();

            if (ModelState.IsValid)
            {
                if (org.name != null)
                {
                    originalOrganization.name = org.name;
                    originalOrganization.seoname = Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(org.name);
                }
                if (org.submitterId != null)
                {
                    originalOrganization.submitterId = org.submitterId;
                }
                if (org.number != null && org.number.Length > 0)
                {
                    originalOrganization.number = org.number;
                }
                if (org.description != null && org.description.Length > 0)
                {
                    originalOrganization.description = org.description;
                }
                if (org.contact != null && org.contact.Length > 0)
                {
                    originalOrganization.contact = org.contact;
                }
                if (org.statusId != null)
                {
                    originalOrganization.statusId = org.statusId;
                }
                if (fileSmal != null && fileSmal.ContentLength > 0)
                {
                    originalOrganization.logoFilename = SaveLogoToDisk(fileSmal, org.number);
                }
                if (fileLarge != null && fileLarge.ContentLength > 0)
                {
                    originalOrganization.largeLogo = SaveLogoToDisk(fileLarge, org.number);
                }
                if (org.statusId != null)
                {
                    if (org.statusId == "Accepted" && originalOrganization.statusId != "Accepted")
                    {
                        originalOrganization.dateAccepted = DateTime.Now;
                    }
                    if (originalOrganization.statusId == "Accepted" && org.statusId != "Accepted")
                    {
                        originalOrganization.dateAccepted = null;
                    }
                    originalOrganization.statusId = org.statusId;
                }

                originalOrganization.modified = DateTime.Now;
                db.Entry(originalOrganization).State = EntityState.Modified;
                db.SaveChanges();
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
            
            var queryResults = from o in db.Organizations
                                            where o.seoname == organisasjon && o.register.seoname == registername
                                            && o.register.parentRegister.seoname == parentregister
                                            select o.systemId;

            Guid systId = queryResults.FirstOrDefault();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(systId);
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
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Organization organization, string registername, string organisasjon, string parentregister, string parentregisterowner)
        {
            var queryResultsOrganisasjon = from o in db.Organizations
                                           where o.seoname == organisasjon
                                           && o.register.seoname == registername
                                           && o.register.parentRegister.seoname == parentregister
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.FirstOrDefault();

            Organization originalOrganization = db.Organizations.Find(systId);            
            
            db.Organizations.Remove(originalOrganization);
            db.SaveChanges();

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

        private string SaveLogoToDisk(HttpPostedFileBase file, string organizationNumber)
        {
            string filename = organizationNumber + "_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Organization.DataDirectory), filename);
            file.SaveAs(path);
            return filename;
        }

        private void ValidationName(Organization organization, Kartverket.Register.Models.Register register)
        {
            var queryResultsDataset = from o in db.Organizations
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

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }
    }
}

