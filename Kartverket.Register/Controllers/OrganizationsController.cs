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
                Guid orgId = queryResultsOrganization.First();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                organization.submitterId = orgId;
                organization.submitter = submitterOrganisasjon;
                var queryResultsRegister = from o in db.Registers
                                           where o.name == "Organisasjoner"
                                           select o.systemId;
                Guid regId = queryResultsRegister.First();

                organization.modified = DateTime.Now;
                organization.dateSubmitted = DateTime.Now;
                organization.registerId = regId;
                organization.statusId = "Submitted";
                organization.seoname = MakeSeoFriendlyString(organization.name);

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
            //organization.seoname = MakeSeoFriendlyString(organization.name);
            db.SaveChanges();
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }


        //// GET: Organizations/Create
        [Authorize]
        [Route("organisasjoner/{registername}/ny")]
        public ActionResult Create(string registername)
        {
            ViewBag.registerSEO = registername;
            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername || o.name == registername
                                       select o.systemId;

            Guid regId = queryResultsRegister.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);
            ViewBag.registername = register.name;

            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin")
            {
                return View(); 
            }
            return HttpNotFound();
        }

        // POST: Organizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("organisasjoner/{registername}/ny")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Organization organization, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string registername)
        {
            ValidationName(organization, registername);
            
            if (ModelState.IsValid)
            {
                var queryResultsRegister = from o in db.Registers
                                           where o.name == registername
                                           select o.systemId;
                Guid regId = queryResultsRegister.First();
                Kartverket.Register.Models.Register register = db.Registers.Find(regId);

                organization.systemId = Guid.NewGuid();
                if (organization.name == null) { organization.name = "ikke angitt"; }
                organization.modified = DateTime.Now;
                organization.dateSubmitted = DateTime.Now;
                organization.registerId = regId;
                organization.statusId = "Submitted";
                organization.seoname = MakeSeoFriendlyString(organization.name);

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
                Guid orgId = queryResultsOrganization.First();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                organization.submitterId = orgId;
                organization.submitter = submitterOrganisasjon;

                db.Entry(organization).State = EntityState.Modified;
                db.SaveChanges();

                if (register.parentRegister != null)
                {
                    return Redirect("/subregister/" + register.parentRegister.seoname + "/" + register.parentRegister.owner.seoname + "/" + registername);
                }
                else
                {
                    return Redirect("/register/" + registername);
                }
            }

            return View(organization);
        }


        //// GET: Organizations/Edit/5
        [Authorize]
        [Route("organisasjoner/{registername}/{submitter}/{organisasjon}/rediger")]
        public ActionResult Edit(string registername, string submitter, string organisasjon)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResultsOrganisasjon = from o in db.Organizations
                                           where o.seoname == organisasjon && o.register.seoname == registername
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.First();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kartverket.Register.Models.Organization organization = db.Organizations.Find(systId);
            if (organization == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin")
            {
                Viewbags(organization);
                return View(organization);
            }
            return HttpNotFound();
        }

        private void Viewbags(Kartverket.Register.Models.Organization organization)
        {
            ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", organization.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "SystemId", "name", organization.submitterId);
        }


        // POST: Organizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("organisasjoner/{registernameOrganization}/{submitterOrganization}/{organisasjon}/rediger")]
        public ActionResult Edit(Organization organization, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string registernameOrganization, string organisasjon)
        {
            var queryResultsRegister = from o in db.Registers
                                       where o.name == registernameOrganization
                                       select o.systemId;
            Guid regId = queryResultsRegister.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);
          
            var queryResultsOrganisasjon = from o in db.Organizations
                                           where o.seoname == organisasjon && o.register.seoname == registernameOrganization
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.First();
            Organization originalOrganization = db.Organizations.Find(systId);

            ValidationName(organization, registernameOrganization);

            var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();

            if (ModelState.IsValid)
            {
                if (organization.name != null)
                {
                    originalOrganization.name = organization.name;
                    originalOrganization.seoname = MakeSeoFriendlyString(organization.name);
                }
                if (organization.submitterId != null)
                {
                    originalOrganization.submitterId = organization.submitterId;
                }
                if (organization.number != null && organization.number.Length > 0)
                {
                    originalOrganization.number = organization.number;
                }
                if (organization.description != null && organization.description.Length > 0)
                {
                    originalOrganization.description = organization.description;
                }
                if (organization.contact != null && organization.contact.Length > 0)
                {
                    originalOrganization.contact = organization.contact;
                }
                if (organization.statusId != null)
                {
                    originalOrganization.statusId = organization.statusId;
                }
                if (fileSmal != null && fileSmal.ContentLength > 0)
                {
                    originalOrganization.logoFilename = SaveLogoToDisk(fileSmal, organization.number);
                }
                if (fileLarge != null && fileLarge.ContentLength > 0)
                {
                    originalOrganization.largeLogo = SaveLogoToDisk(fileLarge, organization.number);
                }
                if (organization.statusId != null)
                {
                    if (organization.statusId == "Accepted" && originalOrganization.statusId != "Accepted")
                    {
                        originalOrganization.dateAccepted = DateTime.Now;
                    }
                    if (originalOrganization.statusId == "Accepted" && organization.statusId != "Accepted")
                    {
                        originalOrganization.dateAccepted = null;
                    }
                    originalOrganization.statusId = organization.statusId;
                }

                originalOrganization.modified = DateTime.Now;
                db.Entry(originalOrganization).State = EntityState.Modified;
                db.SaveChanges();
                Viewbags(organization);


                if (register.parentRegister != null)
                {
                    return Redirect("/subregister/" + register.parentRegister.seoname + "/" + register.parentRegister.owner.seoname + "/" + registernameOrganization + "/" + originalOrganization.submitter.seoname + "/" + originalOrganization.seoname);
                }
                else
                {
                    return Redirect("/register/" + registernameOrganization + "/" + originalOrganization.submitter.seoname + "/" + originalOrganization.seoname);    
                }
                            
            }
            Viewbags(organization);
            return View(originalOrganization);
        }

        // GET: Organizations/Delete/5
        [Authorize]
        [Route("organisasjoner/{registername}/{submitter}/{organisasjon}/slett")]
        public ActionResult Delete(string registername, string submitter, string organisasjon)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");
            
            var queryResults = from o in db.Organizations
                                            where o.seoname == organisasjon && o.register.seoname == registername
                                            select o.systemId;

            Guid systId = queryResults.First();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(systId);
            if (organization == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin")
            {
                return View(organization);
            }
            return HttpNotFound();
        }

        // POST: Registers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("organisasjoner/{registername}/{submitter}/{organisasjon}/slett")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Organization organization, string registername, string organisasjon)
        {
            var queryResultsRegister = from o in db.Registers
                                       where o.name == registername
                                       select o.systemId;
            Guid regId = queryResultsRegister.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);

            var queryResultsOrganisasjon = from o in db.Organizations
                                           where o.seoname == organisasjon
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.First();

            Organization originalOrganization = db.Organizations.Find(systId);
            db.Organizations.Remove(originalOrganization);
            db.SaveChanges();

            if (register.parentRegister != null)
            {
                return Redirect("/subregister/" + register.parentRegister.seoname + "/" + register.parentRegister.owner.seoname + "/" + registername);
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

        private static string MakeSeoFriendlyString(string input)
        {
            string encodedUrl = (input ?? "").ToLower();

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // replace norwegian characters
            encodedUrl = encodedUrl.Replace("å", "a").Replace("æ", "ae").Replace("ø", "o");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }

        private string SaveLogoToDisk(HttpPostedFileBase file, string organizationNumber)
        {
            string filename = organizationNumber + "_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Organization.DataDirectory), filename);
            file.SaveAs(path);
            return filename;
        }

        private void ValidationName(Organization organization, string registername)
        {
            var queryResultsDataset = from o in db.Organizations
                                      where o.name == organization.name && o.systemId != organization.systemId && o.register.name == registername
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

