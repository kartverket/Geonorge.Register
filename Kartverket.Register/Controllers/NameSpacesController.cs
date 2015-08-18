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
    public class NameSpacesController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: NameSpaces
        public ActionResult Index()
        {
            var registerItems = db.RegisterItems.Include(n => n.register).Include(n => n.status).Include(n => n.submitter).Include(n => n.versioning);
            return View(registerItems.ToList());
        }

        // GET: NameSpaces/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NameSpace nameSpace = db.NameSpases.Find(id);
            if (nameSpace == null)
            {
                return HttpNotFound();
            }
            return View(nameSpace);
        }

        // GET: NameSpaces/Create
        [Authorize]
        [Route("navnerom/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("navnerom/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            NameSpace nameSpace = new NameSpace();
            Kartverket.Register.Models.Register register = GetRegister(registername, parentRegister);

            nameSpace.register = register;
            if (register.parentRegisterId != null)
            {
                nameSpace.register.parentRegister = register.parentRegister;
            }

            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (UserHasAccess(role, user, nameSpace, "Create"))
            {
                return View(nameSpace);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: NameSpaces/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("navnerom/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("navnerom/{registername}/ny")]
        public ActionResult Create(NameSpace nameSpace, string registername, string parentRegister)
        {
            Kartverket.Register.Models.Register register = GetRegister(registername, parentRegister);

            string parentRegisterOwner = null;
            nameSpace.register = register;
            if (register.parentRegisterId != null)
            {
                parentRegisterOwner = register.parentRegister.owner.seoname;
            }
            
            ValidationName(nameSpace, register);

            if (ModelState.IsValid)
            {
                nameSpace.systemId = Guid.NewGuid();
                if (nameSpace.name == null) nameSpace.name = "ikke angitt";
                nameSpace.systemId = Guid.NewGuid();
                nameSpace.modified = DateTime.Now;
                nameSpace.dateSubmitted = DateTime.Now;
                nameSpace.registerId = register.systemId;
                nameSpace.statusId = "Submitted";
                nameSpace.submitter = null;
                nameSpace.seoname = Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(nameSpace.name);

                db.RegisterItems.Add(nameSpace);
                db.SaveChanges();
                GetSubmitter(nameSpace);
                db.Entry(nameSpace).State = EntityState.Modified;
                db.SaveChanges();

                if (!String.IsNullOrWhiteSpace(parentRegister))
                {
                    return Redirect("/subregister/" + parentRegister + "/" + parentRegisterOwner + "/" + registername + "/" + "/" + nameSpace.submitter.seoname + "/" + nameSpace.seoname);
                }
                else
                {
                    return Redirect("/register/" + registername + "/" + nameSpace.submitter.seoname + "/" + nameSpace.seoname);
                }
            }
            return View(nameSpace);
        }

        // GET: NameSpaces/Edit/5
        [Route("navnerom/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        [Route("navnerom/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(string registername, string itemname, string parentRegister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            NameSpace nameSpace = GetRegisterItem(registername, itemname, parentRegister);

            if (nameSpace == null)
            {
                return HttpNotFound();
            }

            if (UserHasAccess(role, user, nameSpace, "Edit"))
            {
                Viewbags(nameSpace);
                return View(nameSpace);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: NameSpaces/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("navnerom/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        [Route("navnerom/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(NameSpace nameSpace, string registername, string itemname, string parentRegister)
        {
            NameSpace originalNameSpace = GetRegisterItem(registername, itemname, parentRegister);
            Kartverket.Register.Models.Register register = GetRegister(registername, parentRegister);
            ValidationName(nameSpace, register);

            if (ModelState.IsValid)
            {
                if (nameSpace.name != null) originalNameSpace.name = nameSpace.name; originalNameSpace.seoname = Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(originalNameSpace.name);
                if (nameSpace.description != null) originalNameSpace.description = nameSpace.description;
                if (nameSpace.submitterId != null) originalNameSpace.submitterId = nameSpace.submitterId;
                if (nameSpace.statusId != null) originalNameSpace.statusId = nameSpace.statusId;
                if (nameSpace.serviceUrl != null) originalNameSpace.serviceUrl = nameSpace.serviceUrl;
           
                if (nameSpace.statusId != null)
                {
                    if (nameSpace.statusId == "Valid" && originalNameSpace.statusId != "Valid")
                    {
                        originalNameSpace.dateAccepted = DateTime.Now;
                    }
                    if (originalNameSpace.statusId == "Valid" && nameSpace.statusId != "Valid")
                    {
                        originalNameSpace.dateAccepted = null;
                    }
                    originalNameSpace.statusId = nameSpace.statusId;
                }

                originalNameSpace.modified = DateTime.Now;
                db.Entry(originalNameSpace).State = EntityState.Modified;
                db.SaveChanges();

                Viewbags(originalNameSpace);
                if (!String.IsNullOrWhiteSpace(parentRegister))
                {
                    return Redirect("/subregister/" + originalNameSpace.register.parentRegister.seoname + "/" + originalNameSpace.register.parentRegister.owner.seoname + "/" + registername + "/" + "/" + originalNameSpace.submitter.seoname + "/" + originalNameSpace.seoname);
                }
                else
                {
                    return Redirect("/register/" + registername + "/" + originalNameSpace.submitter.seoname + "/" + originalNameSpace.seoname);
                }
            }
            Viewbags(originalNameSpace);
            return View(originalNameSpace);
        }

        // GET: NameSpaces/Delete/5
        [Route("navnerom/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett")]
        [Route("navnerom/{registername}/{itemowner}/{itemname}/slett")]
        public ActionResult Delete(string itemname, string registername, string parentRegister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            NameSpace nameSpace = GetRegisterItem(registername, itemname, parentRegister);

            if (nameSpace == null)
            {
                return HttpNotFound();
            }
            if (UserHasAccess(role, user, nameSpace, "Delete"))
            {
                Viewbags(nameSpace);
                return View(nameSpace);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: NameSpaces/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("navnerom/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett")]
        [Route("navnerom/{registername}/{itemowner}/{itemname}/slett")]
        public ActionResult DeleteConfirmed(string itemname, string registername, string parentRegister, string registerowner)
        {
            NameSpace nameSpace = GetRegisterItem(registername, itemname, parentRegister);

            string parent = null;
            if (nameSpace.register.parentRegisterId != null)
            {
                parent = nameSpace.register.parentRegister.seoname;
            }

            db.RegisterItems.Remove(nameSpace);
            db.SaveChanges();

            if (parent != null)
            {
                return Redirect("/subregister/" + parentRegister + "/" + registerowner + "/" + registername);
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

        private Models.Register GetRegister(string registername, string parentRegister)
        {
            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername && o.parentRegister.seoname == parentRegister
                                       select o;

            Kartverket.Register.Models.Register register = queryResultsRegister.FirstOrDefault();
            return register;
        }

        private void GetSubmitter(NameSpace nameSpace)
        {
            string organizationLogin = GetSecurityClaim("organization");
            var queryResults = from o in db.Organizations
                               where o.name == organizationLogin
                               select o;

            Organization submitterOrganisasjon = queryResults.FirstOrDefault();

            nameSpace.submitterId = submitterOrganisasjon.systemId;
            nameSpace.submitter = submitterOrganisasjon;
        }

        private NameSpace GetRegisterItem(string registername, string itemname, string parentRegister)
        {
            var queryResultsNS = from o in db.NameSpases
                                 where o.seoname == itemname &&
                                 o.register.seoname == registername &&
                                 o.register.parentRegister.seoname == parentRegister
                                 select o;

            NameSpace nameSpace = queryResultsNS.FirstOrDefault();
            return nameSpace;
        }

        private static bool UserHasAccess(string role, string user, NameSpace nameSpace, string when)
        {
            if (when == "Create")
            {
                return role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && nameSpace.register.accessId == 2 && nameSpace.submitter.name.ToLower() == user.ToLower());
            }
            else if (when == "Edit" || when == "Delete")
            {
                return role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && nameSpace.register.accessId == 2);
            }
            
            return false;
            
        }

        private void Viewbags(NameSpace nameSpace)
        {
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", nameSpace.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", nameSpace.submitterId);
        }

        private void ValidationName(NameSpace nameSpace, Kartverket.Register.Models.Register register)
        {
            var queryResultsDataset = from o in db.NameSpases
                                      where o.name == nameSpace.name &&
                                      o.systemId != nameSpace.systemId &&
                                      o.register.name == register.name &&
                                      o.register.parentRegisterId == register.parentRegisterId
                                      select o.systemId;

            if (queryResultsDataset.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");
            }
        }
    }
}
