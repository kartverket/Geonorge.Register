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
    public class OrganizationsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

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
        [Route("organisasjoner/ny")]
        public ActionResult Create()
        {
            string registerOwner = FindRegisterOwner("organisasjoner");
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResults = from o in db.Registers
                               where o.seoname == "organisasjoner"
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);
            string registerStatus = register.statusId;

            if (role == "nd.metadata_admin" || role == "nd.metadata")
            {
                return View(); 
            }
            return HttpNotFound();
        }

        // POST: Organizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("organisasjoner/ny")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Organization organization, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge)
        {
            if (ModelState.IsValid)
            {

                organization.systemId = Guid.NewGuid();
                if (organization.name == null)
                {
                    organization.name = "ikke angitt";
                }

                var queryResultsRegister = from o in db.Registers
                                           where o.name == "Organisasjoner"
                                           select o.systemId;
                Guid regId = queryResultsRegister.First();

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
            }

            return Redirect("/register/organisasjoner");
        }


        //// GET: Organizations/Edit/5
        [Authorize]
        [Route("organisasjoner/{orgnavn}/rediger")]
        public ActionResult Edit(string orgnavn)
        {
            string registerOwner = FindRegisterOwner("organisasjoner");
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin" || user == registerOwner)
            {
                var queryResultsOrganisasjon = from o in db.Organizations
                                               where o.seoname == orgnavn
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
                ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", organization.statusId);
                ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "SystemId", "name", organization.submitterId);
                return View(organization);
            }
            return HttpNotFound();
        }


        // POST: Organizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("organisasjoner/{orgnavn}/rediger")]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Organization organization, string submitterId, string number, string description, string contact, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string statusID, string id, string orgnavn)
        {
          
            var queryResultsOrganisasjon = from o in db.Organizations
                                           where o.seoname == orgnavn
                                           select o.systemId;
            Guid systId = queryResultsOrganisasjon.First();

            if (ModelState.IsValid)
            {
                Organization originalOrganization = db.Organizations.Find(systId);

                if (organization.name != null)
                {
                    originalOrganization.name = organization.name;
                    originalOrganization.seoname = MakeSeoFriendlyString(organization.name);
                }
                if (submitterId != null)
                {
                    originalOrganization.submitterId = organization.submitterId;
                }
                if (number != null && number.Length > 0)
                {
                    originalOrganization.number = organization.number;
                }
                if (description != null && description.Length > 0)
                {
                    originalOrganization.description = organization.description;
                }
                if (contact != null && contact.Length > 0)
                {
                    originalOrganization.contact = organization.contact;
                }
                if (statusID != null)
                {
                    originalOrganization.statusId = statusID;
                }
                if (fileSmal != null && fileSmal.ContentLength > 0)
                {
                    originalOrganization.logoFilename = SaveLogoToDisk(fileSmal, organization.number);
                }
                if (fileLarge != null && fileLarge.ContentLength > 0)
                {
                    originalOrganization.largeLogo = SaveLogoToDisk(fileLarge, organization.number);
                }

                originalOrganization.modified = DateTime.Now;
                db.Entry(originalOrganization).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", organization.statusId);
                ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "SystemId", "name", organization.submitterId);
                
                return Redirect("/register/organisasjoner/" + originalOrganization.submitter.seoname + "/" + originalOrganization.seoname);                
            }
            return RedirectToAction("Edit");
        }

        // GET: Organizations/Delete/5
        //[Route("organisasjoner/slett/{name}/{id}")]
        [Authorize]
        [Route("organisasjoner/{orgname}/slett")]
        public ActionResult Delete(string orgname)
        {
            string registerOwner = FindRegisterOwner("organisasjoner");
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin" || user == registerOwner)
            {
                var queryResultsOrganisasjon = from o in db.Organizations
                                               where o.seoname == orgname
                                               select o.systemId;
                Guid systId = queryResultsOrganisasjon.First();

                if (systId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Organization organization = db.Organizations.Find(systId);
                if (organization == null)
                {
                    return HttpNotFound();
                }
                return View(organization);
            }
            return HttpNotFound();
        }

        // POST: Registers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("organisasjoner/{orgname}/slett")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Organization organization, string orgname)
        {
            var queryResultsOrganisasjon = from o in db.Organizations
                                           where o.seoname == orgname
                                           select o.systemId;

            Guid systId = queryResultsOrganisasjon.First();

            Organization originalOrganization = db.Organizations.Find(systId);

            

            var queryResultsRegisterItems = from o in db.RegisterItems
                                           where o.registerId == originalOrganization.systemId || o.submitterId == originalOrganization.systemId
                                           select o.systemId;
           

            db.Organizations.Remove(originalOrganization);
            db.SaveChanges();


            return Redirect("/register/organisasjoner/");
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

        
        private string FindRegisterOwner(string registername)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == registername
                               select o.systemId;

            Guid regId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);
            string registerOwner = register.owner.name;
            return registerOwner;
        }

        private string SaveLogoToDisk(HttpPostedFileBase file, string organizationNumber)
        {
            string filename = organizationNumber + "_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Organization.DataDirectory), filename);
            file.SaveAs(path);
            return filename;
        }
    }
}

