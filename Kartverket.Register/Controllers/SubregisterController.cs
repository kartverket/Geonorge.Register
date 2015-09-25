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
using System.Text;
using System.Xml.Linq;
using System.IO;
using Kartverket.Register.Services.Versioning;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.Search;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class SubregisterController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IRegisterService _registerService;
        private ISearchService _searchService;

        public SubregisterController()
        {
            _registerService = new RegisterService(db);
            _searchService = new SearchService(db);
        }

        // GET: Subregister
        public ActionResult Index()
        {
            var registers = db.Registers.Include(r => r.manager).Include(r => r.owner).Include(r => r.parentRegister).Include(r => r.status);
            return View(registers.ToList());
        }

        // GET: Registers/Details/5
        [Route("subregister/{parentRegister}/{owner}/{subregister}.{format}")]
        [Route("subregister/{parentRegister}/{owner}/{subregister}")]
        public ActionResult Details(string parentRegister, string owner, string subregister, string sorting, int? page, string export, string format, FilterParameters filter)
        {
            string ApiRedirectUrl = GetApiUrl(ref format);
            if (!string.IsNullOrWhiteSpace(ApiRedirectUrl))
            {
                return Redirect(ApiRedirectUrl);
            }

            Kartverket.Register.Models.Register register = _registerService.GetSubregisterByName(parentRegister, subregister);
            if (register != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.text))
                {
                    register = _searchService.Search(register, filter.text);
                }
                register = _registerService.FilterRegisterItems(register, filter);

                ViewBag.page = page;
                ViewBag.SortOrder = sorting;
                ViewBag.sorting = new SelectList(db.Sorting.ToList(), "value", "description");
                //Kartverket.Register.Models.Register parent = register.parentRegister;
                //while (parent.parentRegisterId != null)
                //{
                //    parent = parent.parentRegister;
                //}
                ViewBag.register = register.name;

                ViewBag.registerSEO = register.parentRegister.seoname;
                ViewBag.ownerSEO = owner;
                ViewBag.subregister = subregister;
                ViewBag.search = filter.text;

                if (register == null)
                {
                    return HttpNotFound();
                }

                return View(register);
            }
            return HttpNotFound();
        }


        [Route("subregister/{registername}/{owner}/{subregister}/{submitter}/{itemname}.{format}")]
        [Route("subregister/{registername}/{owner}/{subregister}/{submitter}/{itemname}")]
        public ActionResult DetailsSubregisterItem(string registername, string owner, string subregister, string itemname, string format)
        {
            string ApiRedirectUrl = GetApiUrl(ref format);
            if (!string.IsNullOrWhiteSpace(ApiRedirectUrl))
            {
                return Redirect(ApiRedirectUrl);
            }

            var queryResults = from o in db.RegisterItems
                               where o.seoname == itemname && o.register.seoname == subregister && o.register.parentRegister.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();
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
        [Route("subregister/{parentregister}/{parentRegisterOwner}/{registername}/ny")]
        [Route("subregister/{registername}/ny")]
        public ActionResult Create(string registername, string parentregister)
        {
            Kartverket.Register.Models.Register nyttRegister = new Kartverket.Register.Models.Register();
            Kartverket.Register.Models.Register register = _registerService.GetSubregisterByName(parentregister, registername);
            nyttRegister.parentRegister = register;

            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");
            ViewBagSubregister(register, parentregister);

            if (register.parentRegister != null)
            {
                nyttRegister.parentRegister.parentRegister = register.parentRegister;
            }

            if (role == "nd.metadata_admin")
            {
                return View(nyttRegister);
            }

            if ((role == "nd.metadata" || role == "nd.metadata_editor") && register.accessId == 2)
            {
                return View(nyttRegister);
            }
            return HttpNotFound();
        }

        

        // POST: subregister/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [Route("subregister/{registerparant}/{parentRegisterOwner}/{registerName}/ny")]
        [Route("subregister/{registerName}/ny")]
        public ActionResult Create(Kartverket.Register.Models.Register subRegister, string registerName, string registerparant)
        {
            var errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();

            ValidationName(subRegister, registerName);

            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registerName && o.parentRegister.seoname == registerparant
                                       select o.systemId;
            Guid regId = queryResultsRegister.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);

            if (ModelState.IsValid)
            {
                subRegister.systemId = Guid.NewGuid();
                if (subRegister.name == null)
                {
                    subRegister.name = "ikke angitt";
                }
                subRegister.systemId = Guid.NewGuid();
                subRegister.modified = DateTime.Now;
                subRegister.dateSubmitted = DateTime.Now;
                subRegister.statusId = "Submitted";
                subRegister.seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(subRegister.name);
                subRegister.parentRegisterId = regId;

                db.Registers.Add(subRegister);
                db.SaveChanges();

                string organizationLogin = GetSecurityClaim("organization");

                var queryResults = from o in db.Organizations
                                   where o.name == organizationLogin
                                   select o.systemId;

                Guid orgId = queryResults.FirstOrDefault();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                subRegister.ownerId = submitterOrganisasjon.systemId;
                subRegister.managerId = submitterOrganisasjon.systemId;

                db.Entry(subRegister).State = EntityState.Modified;

                db.SaveChanges();
                ViewBagSubregister(register, registerparant);

                return Redirect("/subregister/" + subRegister.parentRegister.seoname + "/" + subRegister.parentRegister.owner.seoname + "/" + subRegister.seoname);

            }
            ViewBagSubregister(register, registerparant);
            
            
            return View(subRegister);
        }

        // GET: Subregister/Edit/5
        [Authorize]
        [Route("subregister/{registername}/{owner}/{subregister}/rediger")]
        public ActionResult Edit(string registername, string subregister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResults = from o in db.Registers
                               where o.seoname == subregister && o.parentRegister.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);

            Viewbags(register);
            if (register == null)
            {
                return HttpNotFound();
            }

            if (role == "nd.metadata_admin")
            {
                return View(register);
            }

            if ((role == "nd.metadata" || user == register.owner.name) && register.accessId == 2)
            {
                return View(register);
            }
            return HttpNotFound();
        }

        // POST: Subregister/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("subregister/{registername}/{ownerSubregister}/{subregister}/rediger")]
        public ActionResult Edit(Kartverket.Register.Models.Register register, string registername, string subregister)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == subregister && o.parentRegister.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();
            Kartverket.Register.Models.Register originalRegister = db.Registers.Find(systId);

            ValidationName(register, registername);


            if (ModelState.IsValid)
            {
                if (register.name != null) originalRegister.name = register.name; originalRegister.seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(originalRegister.name);
                if (register.description != null) originalRegister.description = register.description;
                if (register.ownerId != null) originalRegister.ownerId = register.ownerId;
                if (register.managerId != null) originalRegister.managerId = register.managerId;
                if (register.targetNamespace != null) originalRegister.targetNamespace = register.targetNamespace;
                originalRegister.accessId = register.accessId;
                originalRegister.parentRegisterId = register.parentRegisterId;


                originalRegister.modified = DateTime.Now;
                if (register.statusId != null)
                {
                    originalRegister.statusId = register.statusId;
                    if (originalRegister.statusId != "Valid" && register.statusId == "Valid")
                    {
                        originalRegister.dateAccepted = DateTime.Now;
                    }
                    if (originalRegister.statusId == "Valid" && register.statusId != "Valid")
                    {
                        originalRegister.dateAccepted = null;
                    }
                }

                db.Entry(originalRegister).State = EntityState.Modified;
                db.SaveChanges();
                Viewbags(register);

                if (originalRegister.parentRegisterId == null)
                {
                    return Redirect("/register/" + originalRegister.seoname);
                }
                else
                {
                    return Redirect("/subregister/" + originalRegister.parentRegister.seoname + "/" + originalRegister.parentRegister.owner.seoname + "/" + originalRegister.seoname);
                }

            }
            Viewbags(register);
            return View(originalRegister);
        }

        // GET: Subregister/Delete/5
        [Authorize]
        [Route("subregister/{registername}/{owner}/{subregister}/slett")]
        public ActionResult Delete(string registername, string owner, string subregister)
        {

            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");
            var queryResults = from o in db.Registers
                               where o.seoname == subregister && o.parentRegister.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);

            if (register == null)
            {
                return HttpNotFound();
            }

            if (role == "nd.metadata_admin")
            {
                return View(register);
            }

            if ((role == "nd.metadata" || user == register.owner.name) && register.accessId == 2)
            {
                return View(register);
            }
            return HttpNotFound();
        }

        // POST: Subregister/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("subregister/{registername}/{owner}/{subregister}/slett")]
        public ActionResult DeleteConfirmed(string registername, string owner, string subregister)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == subregister && o.parentRegister.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);

            var queryResultsRegisterItem = ((from o in db.RegisterItems
                                             where (o.register.seoname == subregister && o.register.parentRegister.seoname == registername)
                                             || (o.register.parentRegister.seoname == subregister && o.register.parentRegister.parentRegister.seoname == registername)
                                             select o.systemId).Union(
                                           from r in db.Registers
                                           where r.parentRegister.seoname == subregister && r.parentRegister.parentRegister.seoname == registername
                                           select r.systemId));

            if (queryResultsRegisterItem.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessageDelete", "Registeret kan ikke slettes fordi det inneholder elementer som må slettes først!");
                return View(register);
            }
            else
            {
                string parentParentRegisterName = null;
                string parentParentRegisterOwner = null;

                if (register.parentRegister.parentRegisterId != null)
                {
                    parentParentRegisterName = register.parentRegister.parentRegister.seoname;
                    parentParentRegisterOwner = register.parentRegister.parentRegister.owner.seoname;
                }

                db.Registers.Remove(register);
                db.SaveChanges();

                if (parentParentRegisterName != null)
                {
                    return Redirect("/subregister/" + parentParentRegisterName + "/" + parentParentRegisterOwner + "/" + registername);
                }

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

        private void ViewBagSubregister(Kartverket.Register.Models.Register register, string parentRegister)
        {
            ViewBag.containedItemClass = new SelectList(db.ContainedItemClass.OrderBy(s => s.description), "value", "description", String.Empty);
            if (parentRegister != null)
            {
                ViewBag.parentRegister = register.parentRegister.name;
                ViewBag.parentRegisterSEO = register.parentRegister.seoname;
                ViewBag.parentRegisterOwner = register.parentRegister.owner.seoname;
            }
            ViewBag.register = register.name;
            ViewBag.registerSEO = register.seoname;
        }
        
        private void ValidationName(Kartverket.Register.Models.Register subRegister, string register)
        {
            var queryResultsDataset = from o in db.Registers
                                      where o.name == subRegister.name && o.systemId != subRegister.systemId && o.parentRegister.seoname == register
                                      select o.systemId;

            if (queryResultsDataset.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");
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

        private void Viewbags(Kartverket.Register.Models.Register register)
        {
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", register.statusId);
            ViewBag.ownerId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", register.ownerId);
            ViewBag.parentRegisterId = new SelectList(db.Registers.Where(r => r.containedItemClass == "Register" && r.name != register.name).OrderBy(s => s.name), "systemId", "name", register.parentRegisterId);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }

        private string GetApiUrl(ref string format)
        {
            string ApiRedirectUrl = null;
            if (!string.IsNullOrWhiteSpace(format))
            {
                ApiRedirectUrl = "/api/" + Request.FilePath;
            }
            else
            {
                format = _registerService.ContentNegotiation(ControllerContext);
                if (!string.IsNullOrWhiteSpace(format))
                {
                    ApiRedirectUrl = "/api/" + Request.FilePath + "." + format;
                }
            }
            return ApiRedirectUrl;
        }
    }
}
