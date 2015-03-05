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
    public class CodelistValuesController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: CodelistValues
        public ActionResult Index()
        {
            var registerItems = db.RegisterItems.Include(c => c.register).Include(c => c.status).Include(c => c.submitter);
            return View(registerItems.ToList());
        }

        // GET: CodelistValues/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodelistValue codelistValue = db.CodelistValues.Find(id);
            if (codelistValue == null)
            {
                return HttpNotFound();
            }
            return View(codelistValue);
        }

        // GET: CodelistValues/Create
        [Authorize]
        [Route("kodeliste/{registername}/ny")]
        public ActionResult Create(string registername)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);

            if(register.parentRegisterId != null){
                ViewBag.registerOwner = register.parentRegister.owner.seoname;
                ViewBag.parentregister = register.parentRegister.seoname;
            }
            ViewBag.registername = registername;
                        string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin" || role == "nd.metadata" || role == "nd.metadata_editor")
            {
                return View();
            }
            return HttpNotFound();
        }

        // POST: CodelistValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("kodeliste/{registername}/ny")]
        public ActionResult Create(CodelistValue codelistValue, string registername)
        {
            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername
                                       select o.systemId;

            Guid regId = queryResultsRegister.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);
            string parentRegister = null;
            string parentRegisterOwner = null;
            if (register.parentRegisterId != null)
            {
                parentRegister = register.parentRegister.seoname;
                parentRegisterOwner = register.parentRegister.owner.seoname;
            }
            ValidationName(codelistValue, registername);

            if (ModelState.IsValid)
            {
                codelistValue.systemId = Guid.NewGuid();
                codelistValue.modified = DateTime.Now;
                codelistValue.dateSubmitted = DateTime.Now;
                codelistValue.registerId = regId;
                codelistValue.statusId = "Submitted";

                if (codelistValue.name == null || codelistValue.name.Length == 0)
                {
                    codelistValue.name = "ikke angitt";
                    codelistValue.seoname = codelistValue.systemId.ToString();
                }

                if (codelistValue.description == null || codelistValue.description.Length == 0)
                {
                    codelistValue.description = "ikke angitt";
                }

                codelistValue.seoname = MakeSeoFriendlyString(codelistValue.name);

                string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;

                string organizationLogin = GetSecurityClaim("organization");

                var queryResults = from o in db.Organizations
                                   where o.name == organizationLogin
                                   select o.systemId;

                Guid orgId = queryResults.First();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                codelistValue.submitterId = orgId;
                codelistValue.submitter = submitterOrganisasjon;

                db.Entry(codelistValue).State = EntityState.Modified;
                //db.SaveChanges();

                db.RegisterItems.Add(codelistValue);
                db.SaveChanges();

                return Redirect("/subregister/" + parentRegister + "/" + parentRegisterOwner + "/" + registername);
            }

            return View(codelistValue);
        }

        // GET: CodelistValues/Edit/5
        [Authorize]
        [Route("kodeliste/{register}/{submitter}/{itemname}/rediger")]
        public ActionResult Edit(string register, string itemname)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResults = from o in db.CodelistValues
                               where o.seoname == itemname && o.register.seoname == register
                               select o.systemId;

            Guid systId = queryResults.First();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodelistValue codelistValue = db.CodelistValues.Find(systId);

            if (codelistValue == null)
            {
                return HttpNotFound();
            }

            if (role == "nd.metadata_admin" || user.ToLower() == codelistValue.submitter.name.ToLower())
            {
                Viewbags(codelistValue);
                return View(codelistValue);
            }
            return HttpNotFound();
        }

        // POST: CodelistValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("kodeliste/{register}/{submitter}/{itemname}/rediger")]
        public ActionResult Edit(CodelistValue codelistValue, string submitter, string register, string itemname)
        {
            var queryResults = from o in db.CodelistValues
                               where o.seoname == itemname && o.register.seoname == register
                               select o.systemId;

            Guid systId = queryResults.First();
            CodelistValue originalCodelistValue = db.CodelistValues.Find(systId);

            ValidationName(codelistValue, register);

            if (ModelState.IsValid)
            {
                if (codelistValue.name != null) originalCodelistValue.name = codelistValue.name; originalCodelistValue.seoname = MakeSeoFriendlyString(originalCodelistValue.name);
                if (codelistValue.description != null) originalCodelistValue.description = codelistValue.description;
                if (codelistValue.statusId != null) originalCodelistValue.statusId = codelistValue.statusId;
                if (codelistValue.submitterId != null) originalCodelistValue.submitterId = codelistValue.submitterId;
                if (codelistValue.value != null) originalCodelistValue.value = codelistValue.value;

                originalCodelistValue.modified = DateTime.Now;
                db.Entry(originalCodelistValue).State = EntityState.Modified;
                db.SaveChanges();
                Viewbags(codelistValue);


                if(originalCodelistValue.register.parentRegisterId != null)
                {
                    return Redirect("/subregister/" + originalCodelistValue.register.parentRegister.seoname + "/" + originalCodelistValue.register.owner.seoname + "/" + submitter + "/" + register);
                }
                
                return Redirect("/register/" + originalCodelistValue.register.seoname);
            }
            Viewbags(originalCodelistValue);
            return View(originalCodelistValue);
        }

        // GET: CodelistValues/Delete/5
        [Authorize]
        [Route("kodeliste/{register}/{submitter}/{itemname}/slett")]
        public ActionResult Delete(string register, string itemname)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResults = from o in db.CodelistValues
                               where o.seoname == itemname && o.register.seoname == register
                               select o.systemId;

            Guid systId = queryResults.First();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodelistValue codelistValue = db.CodelistValues.Find(systId);
            if (codelistValue == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin" || user.ToLower() == codelistValue.submitter.name.ToLower())
            {
                return View(codelistValue);
            }
            return HttpNotFound();
        }

        // POST: CodelistValues/Delete/5
        [Authorize]
        [Route("kodeliste/{register}/{submitter}/{itemname}/slett")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string register, string itemname, string submitter)
        {
            var queryResults = from o in db.CodelistValues
                               where o.seoname == itemname && o.register.seoname == register
                               select o.systemId;

            Guid systId = queryResults.First();

            CodelistValue codelistValue = db.CodelistValues.Find(systId);
            string parent = codelistValue.register.parentRegister.seoname;
           
            db.RegisterItems.Remove(codelistValue);
            db.SaveChanges();
            if (parent != null)
            {
                return Redirect("/subregister/" + parent + "/" + submitter + "/" + register);
            }
            return Redirect("/register/" + register);
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

        private void ValidationName(CodelistValue codelistValue, string registername)
        {
            var queryResultsDataset = from o in db.CodelistValues
                                      where o.name == codelistValue.name && o.systemId != codelistValue.systemId && o.register.seoname == registername
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

        private void Viewbags(CodelistValue codelistValue)
        {
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", codelistValue.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", codelistValue.submitterId);
        }
    }
}
