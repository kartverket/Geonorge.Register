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

            
            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        // GET: Registers/Details/5
        [Route("subregister/{registername}/{owner}/{subregister}")]
        public ActionResult DetailsSubregister(string registername, string owner, string subregister, string sorting, int? page)
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
                ViewBag.register = register.name;
                ViewBag.registerSEO = register.seoname;

                if (register == null)
                {
                    return HttpNotFound();
                }
                return View(register);
	        }
                return HttpNotFound();
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

        [Route("subregister/{registername}/{owner}/{subregister}/{itemname}")]
        public ActionResult DetailsSubregisterItem(string registername, string owner, string subregister, string itemname)
        {
            
            var queryResults = from o in db.RegisterItems
                                where o.seoname == itemname && o.register.seoname == subregister && o.register.parentRegister.seoname == registername
                                select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.RegisterItem registerItem = db.RegisterItems.Find(systId);

            if (registerItem.register.containedItemClass == "Document") {
                Kartverket.Register.Models.Document document = db.Documents.Find(systId);
                ViewBag.documentOwner = document.documentowner.name;
            }
                return View(registerItem);    
        }

        //// GET: Registers/Create
        //[Authorize]
        //public ActionResult Create()
        //{
        //    string role = GetSecurityClaim("role");
        //    if (role == "nd.metadata_admin")
        //    {
        //        return View();
        //    }
        //    return HttpNotFound();
        //}

        //// POST: Registers/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "systemId,name,description,dateSubmitted,modified,dateAccepted,containedItemClass")] Kartverket.Register.Models.Register register)
        //{
            
        //    if (ModelState.IsValid)
        //    {
        //        register.systemId = Guid.NewGuid();

        //        var queryResults = from o in db.Organizations
        //                           where o.name == Session["user"]
        //                           select o.systemId;

        //        register.ownerId = queryResults.First();
        //        register.statusId = "Submitted";                
        //        db.Registers.Add(register);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(register);
        //}

        
        // GET: subregister/Create
        [Authorize]
        [Route("subregister/ny")]
        public ActionResult CreateSubregister()
        {
            string registerOwner = FindRegisterOwner("kodelister");
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");
            
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
        public ActionResult CreateSubregister(Kartverket.Register.Models.Register subregister)
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

        [Authorize]
        // GET: Registers/Edit/5
        public ActionResult Edit(Guid? id)
        {
            string role = GetSecurityClaim("role");
            if (role != "nd.metadata_admin")
            {
                return HttpNotFound();
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kartverket.Register.Models.Register register = db.Registers.Find(id);
            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);

        }



        // POST: Registers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        public ActionResult Edit([Bind(Include = "systemId,name,description,dateSubmitted,modified,dateAccepted,containedItemClass,url")] Kartverket.Register.Models.Register register)
        {
            if (ModelState.IsValid)
            {
                db.Entry(register).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(register);
        }

        // GET: Registers/Delete/5
        [Authorize]
        public ActionResult Delete(Guid? id, string name)
        {
            string role = GetSecurityClaim("role");
            if (role != "nd.metadata_admin")
            {
                return HttpNotFound();
            }
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kartverket.Register.Models.Register register = db.Registers.Find(id);
            if (name == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        //// POST: Registers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    Kartverket.Register.Models.Register register = db.Registers.Find(id);
        //    db.Registers.Remove(register);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


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
