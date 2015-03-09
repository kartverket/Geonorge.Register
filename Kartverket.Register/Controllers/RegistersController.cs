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
using PagedList;

namespace Kartverket.Register.Controllers
{
    public class RegistersController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        
        // GET: Registers
        public ActionResult Index()
        {
            setAccessRole();

            return View(db.Registers.OrderBy(r => r.name).ToList());
        }

        // GET: Registers/Details/5
        [Route("register/{name}")]
        public ActionResult Details(string name, string sorting, int? page)
        {
            var queryResults = from o in db.Registers
                               where o.name == name || o.seoname == name
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);
            ViewBag.page = page;
            ViewBag.SortOrder = sorting;            
            ViewBag.sorting = new SelectList(db.Sorting.ToList(), "value", "description");
            ViewBag.register = register.name;
            ViewBag.registerSEO = register.seoname;

            if(register.parentRegisterId != null)
            {
                ViewBag.parentRegister = register.parentRegister.name;
            }

            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        [Route("register/{registername}/{submitter}/{itemname}/")]
        public ActionResult DetailsRegisterItem(string registername, string itemname)
        {
            
            var queryResultsRegisterItem = from o in db.RegisterItems
                                            where o.seoname == itemname && o.register.seoname == registername
                                            select o.systemId;
            
            Guid systId = queryResultsRegisterItem.First();
            Kartverket.Register.Models.RegisterItem registerItem = db.RegisterItems.Find(systId);

            if (registerItem.register.containedItemClass == "Document") {
                Kartverket.Register.Models.Document document = db.Documents.Find(systId);
                ViewBag.documentOwner = document.documentowner.name;
            }
                return View(registerItem);    
        }


        // GET: subregister/Create
        [Authorize]
        [Route("ny/")]
        public ActionResult Create()
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");
            ViewBag.containedItemClass = new SelectList(db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);

            if (role == "nd.metadata_admin")
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
        [Route("ny/")]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Kartverket.Register.Models.Register register)
        {
            ValidationName(register);

            if (ModelState.IsValid)
            {
                register.systemId = Guid.NewGuid();
                if (register.name == null)
                {
                    register.name = "ikke angitt";
                }

                register.systemId = Guid.NewGuid();
                register.modified = DateTime.Now;
                register.dateSubmitted = DateTime.Now;
                register.statusId = "Submitted";
                register.seoname = MakeSeoFriendlyString(register.name);

                db.Registers.Add(register);
                db.SaveChanges();

                string organizationLogin = GetSecurityClaim("organization");

                var queryResults = from o in db.Organizations
                                   where o.name == organizationLogin
                                   select o.systemId;

                Guid orgId = queryResults.First();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                register.ownerId = submitterOrganisasjon.systemId;
                register.managerId = submitterOrganisasjon.systemId;

                db.Entry(register).State = EntityState.Modified;

                db.SaveChanges();
                return Redirect("/");
            }

            return Redirect("/");
        }


        [Authorize]
        [Route("endre/{registername}")]
        public ActionResult Edit(string registername)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResults = from o in db.Registers
                               where o.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);

            if (register == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin")
            {
                Viewbags(register);
                return View(register);
            }
            return HttpNotFound();

        }


        // POST: Registers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("endre/{registername}")]
        [Authorize]
        public ActionResult Edit(Kartverket.Register.Models.Register register, string registername)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();
            ValidationName(register);
            Kartverket.Register.Models.Register originalRegister = db.Registers.Find(systId);
            if (ModelState.IsValid)
            {
                if (register.name != null) originalRegister.name = register.name; originalRegister.seoname = MakeSeoFriendlyString(originalRegister.name);
                if (register.description != null) originalRegister.description = register.description;
                if (register.owner != null) originalRegister.ownerId = register.ownerId;
                if (register.managerId != null) originalRegister.managerId = register.managerId;

                originalRegister.modified = DateTime.Now;
                if (register.statusId != null)
                {
                    originalRegister.statusId = register.statusId;
                    if (originalRegister.statusId != "Accepted" && register.statusId == "Accepted")
                    {
                        originalRegister.dateAccepted = DateTime.Now;
                    }
                    if (originalRegister.statusId == "Accepted" && register.statusId != "Accepted")
                    {
                        originalRegister.dateAccepted = null;
                    }
                }

                db.Entry(originalRegister).State = EntityState.Modified;
                db.SaveChanges();
                Viewbags(register);

                return Redirect("/register/" + register.name);
            }
            Viewbags(register);
            return View(originalRegister);
        }

        // GET: Registers/Delete/5
        [Authorize]
        [Route("slett/{registername}")]
        public ActionResult Delete(string registername)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);

            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        // POST: Registers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("slett/{registername}")]
        [Authorize]
        public ActionResult DeleteConfirmed(string registername)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);

            var queryResultsRegisterItem = from o in db.RegisterItems
                                           where o.register.seoname == registername
                                           select o.systemId;

            if (queryResultsRegisterItem.Count() > 0)
            {
                return View(register);
                //skriv ut feilmelding på at registeret inneholder registeritems.. kan ikke slettes...
            }
            else
            {
                db.Registers.Remove(register);
                db.SaveChanges();
                return Redirect("/");
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

        private string HasAccessToRegister()
        {
            string role = GetSecurityClaim("role");
                        
            bool isAdmin = !string.IsNullOrWhiteSpace(role) && role.Equals("nd.metadata_admin");
            bool isEditor = !string.IsNullOrWhiteSpace(role) && role.Equals("nd.metadata"); //nd.metadata_editor

            if (isAdmin)
            {
                return "admin";
            }
            else if (isEditor)
            {
                return "editor";
            }
            else {
                return "guest";
            }
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

        private void setAccessRole()
        {
            string organization = GetSecurityClaim("organization");

            string role = HasAccessToRegister();
            if (role == "admin")
            {
                Session["role"] = "admin";
                Session["user"] = organization;
            }
            else if (role == "editor")
            {
                Session["role"] = "editor";
                Session["user"] = organization;
            }
            else
            {
                Session["role"] = "guest";
                Session["user"] = "guest";
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

        private void ValidationName(Kartverket.Register.Models.Register register)
        {
            var queryResultsDataset = from o in db.Registers
                                      where o.name == register.name && o.systemId != register.systemId
                                      select o.systemId;

            if (queryResultsDataset.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");
            }
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

        private void Viewbags(Kartverket.Register.Models.Register register)
        {
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", register.statusId);
            ViewBag.ownerId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", register.ownerId);
        }
    }

}
