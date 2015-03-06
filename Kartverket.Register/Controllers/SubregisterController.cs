using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System.Text.RegularExpressions;

namespace Kartverket.Register.Controllers
{
    public class SubregisterController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: Subregister
        public ActionResult Index()
        {
            var registers = db.Registers.Include(r => r.manager).Include(r => r.owner).Include(r => r.parentRegister).Include(r => r.status);
            return View(registers.ToList());
        }

        // GET: Registers/Details/5
        [Route("subregister/{registername}/{owner}/{subregister}")]
        public ActionResult Details(string registername, string owner, string subregister, string sorting, int? page)
        {
            var queryResultsSubregister = from r in db.Registers
                                          where r.seoname == subregister && r.parentRegister.seoname == registername
                                          select r.systemId;

            if (queryResultsSubregister.Count() > 0)
            {
                Guid systId = queryResultsSubregister.First();
                Kartverket.Register.Models.Register register = db.Registers.Find(systId);
                ViewBag.page = page;
                ViewBag.SortOrder = sorting;
                ViewBag.sorting = new SelectList(db.Sorting.ToList(), "value", "description");
                ViewBag.register = register.parentRegister.name;
                ViewBag.registerSEO = register.parentRegister.seoname;
                ViewBag.ownerSEO = owner;
                ViewBag.subregister = subregister;

                if (register == null)
                {
                    return HttpNotFound();
                }
                return View(register);
            }
            return HttpNotFound();
        }

        [Route("subregister/{registername}/{owner}/{subregister}/{submitter}/{itemname}")]
        public ActionResult DetailsSubregisterItem(string registername, string owner, string subregister, string itemname)
        {

            var queryResults = from o in db.RegisterItems
                               where o.seoname == itemname && o.register.seoname == subregister && o.register.parentRegister.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.RegisterItem registerItem = db.RegisterItems.Find(systId);

            if (registerItem.register.containedItemClass == "Document")
            {
                Kartverket.Register.Models.Document document = db.Documents.Find(systId);
                ViewBag.documentOwner = document.documentowner.name;
            }
            return View(registerItem);
        }

        // GET: subregister/Create
        [Authorize]
        [Route("subregister/ny")]
        public ActionResult Create()
        {
            string registerOwner = FindRegisterOwner("kodelister");
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");
            ViewBag.containedItemClass = new SelectList(db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);

            if (role == "nd.metadata_admin" || role == "nd.metadata" || role == "nd.metadata_editor")
            {
                return View();
            }
            return HttpNotFound();
        }

        // POST: subregister/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [Route("subregister/ny")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Kartverket.Register.Models.Register subregister)
        {
            ValidationName(subregister);

            if (ModelState.IsValid)
            {
                subregister.systemId = Guid.NewGuid();
                if (subregister.name == null)
                {
                    subregister.name = "ikke angitt";
                }

                var queryResultsRegister = from o in db.Registers
                                           where o.name == "Kodelister"
                                           select o.systemId;
                Guid regId = queryResultsRegister.First();

                subregister.systemId = Guid.NewGuid();
                subregister.modified = DateTime.Now;
                subregister.dateSubmitted = DateTime.Now;
                subregister.statusId = "Submitted";
                subregister.seoname = MakeSeoFriendlyString(subregister.name);
                subregister.parentRegisterId = regId;

                db.Registers.Add(subregister);
                db.SaveChanges();

                string organizationLogin = GetSecurityClaim("organization");

                var queryResults = from o in db.Organizations
                                   where o.name == organizationLogin
                                   select o.systemId;

                Guid orgId = queryResults.First();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                subregister.ownerId = submitterOrganisasjon.systemId;
                subregister.managerId = submitterOrganisasjon.systemId;

                db.Entry(subregister).State = EntityState.Modified;

                db.SaveChanges();
                return Redirect("/register/kodelister");
            }

            return View(subregister);
        }

        // GET: Subregister/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kartverket.Register.Models.Register register = db.Registers.Find(id);
            if (register == null)
            {
                return HttpNotFound();
            }
            ViewBag.managerId = new SelectList(db.RegisterItems, "systemId", "name", register.managerId);
            ViewBag.ownerId = new SelectList(db.RegisterItems, "systemId", "name", register.ownerId);
            ViewBag.parentRegisterId = new SelectList(db.Registers, "systemId", "name", register.parentRegisterId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", register.statusId);
            return View(register);
        }

        // POST: Subregister/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "systemId,ownerId,managerId,name,description,statusId,dateSubmitted,modified,dateAccepted,containedItemClass,parentRegisterId,seoname")] Kartverket.Register.Models.Register register)
        {
            if (ModelState.IsValid)
            {
                db.Entry(register).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.managerId = new SelectList(db.RegisterItems, "systemId", "name", register.managerId);
            ViewBag.ownerId = new SelectList(db.RegisterItems, "systemId", "name", register.ownerId);
            ViewBag.parentRegisterId = new SelectList(db.Registers, "systemId", "name", register.parentRegisterId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", register.statusId);
            return View(register);
        }

        // GET: Subregister/Delete/5
        [Authorize]
        [Route("subregister/{registername}/{owner}/{subregister}/slett")]
        public ActionResult Delete(string registername, string owner, string subregister)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == subregister && o.parentRegister.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);

            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        // POST: Subregister/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("subregister/{registername}/{owner}/{subregister}/slett")]
        public ActionResult DeleteConfirmed(string registername, string owner, string subregister)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == subregister && o.parentRegister.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();        
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);

            var queryResultsRegisterItem = from o in db.RegisterItems
                               where o.register.seoname == subregister && o.register.parentRegister.seoname == registername
                               select o.systemId;

            if (queryResultsRegisterItem.Count() > 0)
            {
                return View(register);
                //skriv ut feilmelding på at registeret inneholder registeritems.. kan ikke slettes...
            }
            else { 
                db.Registers.Remove(register);
                db.SaveChanges();
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



        // *********************** Hjelpemetoder

        private void ValidationName(Kartverket.Register.Models.Register kodelisteregister)
        {
            var queryResultsDataset = from o in db.Registers
                                      where o.name == kodelisteregister.name && o.systemId != kodelisteregister.systemId && o.parentRegister.name == "Kodelister"
                                      select o.systemId;

            if (queryResultsDataset.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");
            }
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
    }
}
