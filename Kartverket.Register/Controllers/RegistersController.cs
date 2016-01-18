using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Versioning;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services.Search;
using System.Web.Routing;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services;
using System.Collections.Generic;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class RegistersController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private IVersioningService _versioningService;
        private IRegisterService _registerService;
        private ISearchService _searchService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _accessControlService;
        //private string role = HtmlHelperExtensions.GetSecurityClaim("role");

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RegistersController()
        {
            _registerItemService = new RegisterItemService(db);
            _searchService = new SearchService(db);
            _versioningService = new VersioningService(db);
            _registerService = new RegisterService(db);
            _accessControlService = new AccessControlService();
        }

        // GET: Registers
        public ActionResult Index()
        {
            removeSessionSearchParams();

            return View(db.Registers.OrderBy(r => r.name).ToList());
        }

        // GET: Registers/Details/5
        //[Route("subregister/{parentRegister}/{owner}/{registername}.{format}")]
        //[Route("subregister/{parentRegister}/{owner}/{registername}")]
        [Route("register/{registername}")]
        [Route("register/{registernamename}.{format}")]
        [Route("register/{registername}/{filterOrganization}")]
        public ActionResult Details(string parentRegister, string owner, string registername, string sorting, int? page, string format, FilterParameters filter)
        {
            string redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            Models.Register register = _registerService.GetRegister(parentRegister, registername);
            if (register != null)
            {
                register = RegisterItems(register, filter, page);
                ViewbagsRegisterDetails(owner, sorting, page, filter, register);
                return View(register);
            }
            else
            {
                return HttpNotFound();
            }
        }


        [Route("register/{registername}/{itemowner}/{itemname}.{format}")]
        [Route("register/{registername}/{itemowner}/{itemname}")]
        public ActionResult DetailsRegisterItem(string registername, string itemowner, string itemname, string format)
        {
            string redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            RegisterItem registerItem = _registerItemService.GetRegisterItem(null, registername, itemname, null, itemowner);
            ViewBag.owner = GetOwner(registerItem);

            return View(registerItem);
        }


        [Route("register/versjoner/{registername}/{itemowner}/{itemname}/{version}/no.{format}")]
        [Route("register/versjoner/{registername}/{itemowner}/{itemname}/{version}/no")]
        public ActionResult DetailsRegisterItem(string registername, string itemowner, string itemname, int version, string format)
        {
            string redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);

            RegisterItem registerItem = _registerItemService.GetRegisterItem(null, registername, itemname, version);
            ViewBag.owner = GetOwner(registerItem);

            return View(registerItem);
        }


        [Route("subregister/versjoner/{parentRegister}/{owner}/{registername}/{registerItemOwner}/{itemname}.{format}")]
        [Route("register/versjoner/{registername}/{registerItemOwner}/{itemname}.{format}")]
        [Route("subregister/versjoner/{parentRegister}/{owner}/{registername}/{registerItemOwner}/{itemname}")]
        [Route("register/versjoner/{registername}/{registerItemOwner}/{itemname}")]
        public ActionResult DetailsRegisterItemVersions(string registername, string parentRegister, string itemname, string registerItemOwner, string format)
        {
            string redirectToApiUrl = RedirectToApiIfFormatIsNotNull(format);
            if (!string.IsNullOrWhiteSpace(redirectToApiUrl)) return Redirect(redirectToApiUrl);
            VersionsItem versionsItem = _versioningService.Versions(registername, parentRegister, itemname);
            RegisterItemVeiwModel model = new RegisterItemVeiwModel(versionsItem);

            ViewBag.registerItemOwner = registerItemOwner;
            return View(model);
        }


        // GET: Register/Create
        [Authorize]
        [Route("ny")]
        public ActionResult Create()
        {
            if (IsAdmin())
            {
                ViewBag.containedItemClass = new SelectList(db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);
                return View();
            }
            return HttpNotFound();
        }


        // POST: Register/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [Route("ny")]
        public ActionResult Create(Models.Register register)
        {
            if (IsAdmin())
            {
                if (_registerService.validationName(register)) ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");

                if (ModelState.IsValid)
                {
                    register.systemId = Guid.NewGuid();
                    if (register.name == null) register.name = "ikke angitt";
                    register.systemId = Guid.NewGuid();
                    register.modified = DateTime.Now;
                    register.dateSubmitted = DateTime.Now;
                    register.statusId = "Submitted";
                    register.seoname = RegisterUrls.MakeSeoFriendlyString(register.name);
                    register.containedItemClass = register.containedItemClass;

                    db.Registers.Add(register);
                    db.SaveChanges();

                    Organization submitterOrganisasjon = _registerService.GetOrganizationByUserName();
                    register.ownerId = submitterOrganisasjon.systemId;
                    register.managerId = submitterOrganisasjon.systemId;

                    db.Entry(register).State = EntityState.Modified;
                    db.SaveChanges();
                    return Redirect("/");
                }
                ViewBag.containedItemClass = new SelectList(db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);
                return View();
            }
            return HttpNotFound();
        }


        [Authorize]
        [Route("rediger/{registername}")]
        public ActionResult Edit(string registername)
        {
            Models.Register register = _registerService.GetRegister(null, registername);

            if (register == null) return HttpNotFound();

            if (IsAdmin())
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
        [Route("rediger/{registername}")]
        [Authorize]
        public ActionResult Edit(Models.Register register, string registername, string accessRegister)
        {
            if (IsAdmin())
            {
                if (_registerService.validationName(register)) ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");
                Models.Register originalRegister = _registerService.GetRegister(null, registername);

                if (ModelState.IsValid)
                {
                    if (register.name != null) originalRegister.name = register.name;

                    if (register.description != null) originalRegister.description = register.description;
                    if (register.ownerId != null) originalRegister.ownerId = register.ownerId;
                    if (register.managerId != null) originalRegister.managerId = register.managerId;
                    if (register.targetNamespace != null) originalRegister.targetNamespace = register.targetNamespace;
                    originalRegister.accessId = register.accessId;
                    originalRegister.parentRegisterId = register.parentRegisterId;
                    originalRegister.seoname = RegisterUrls.MakeSeoFriendlyString(originalRegister.name);
                    originalRegister.modified = DateTime.Now;
                    if (register.statusId != null) originalRegister = _registerService.SetStatus(register, originalRegister);

                    db.Entry(originalRegister).State = EntityState.Modified;
                    db.SaveChanges();
                    Viewbags(register);

                    return Redirect(RegisterUrls.registerUrl(null, null, registername));
                }
                Viewbags(register);
                return View(originalRegister);
            }
            return HttpNotFound();
        }


        // GET: Registers/Delete/5
        [Authorize]
        [Route("slett/{registername}")]
        public ActionResult Delete(string registername)
        {
            if (IsAdmin())
            {
                Models.Register register = _registerService.GetRegister(null, registername);
                if (register == null) return HttpNotFound();
                return View(register);
            }
            return HttpNotFound();
        }

        // POST: Registers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("slett/{registername}")]
        [Authorize]
        public ActionResult DeleteConfirmed(string registername)
        {
            if (IsAdmin())
            {
                Models.Register register = _registerService.GetRegister(null, registername);

                if (_registerService.RegisterHasChildren(null, registername))
                {
                    ModelState.AddModelError("ErrorMessageDelete", "Registeret kan ikke slettes fordi det inneholder elementer som må slettes først!");
                    return View(register);
                }
                else
                {
                    db.Registers.Remove(register);
                    db.SaveChanges();
                    return Redirect("/");
                }
            }
            return HttpNotFound();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void removeSessionSearchParams()
        {
            Session["sortingType"] = null;
            Session["text"] = null;
            Session["filterVertikalt"] = null;
            Session["filterHorisontalt"] = null;
            Session["InspireRequirement"] = null;
            Session["nationalRequirement"] = null;
            Session["nationalSeaRequirement"] = null;
        }

        private void Viewbags(Models.Register register)
        {
            ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", register.statusId);
            ViewBag.ownerId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", register.ownerId);
            ViewBag.parentRegisterId = new SelectList(db.Registers.Where(r => r.containedItemClass == "Register" && r.name != register.name).OrderBy(s => s.name), "systemId", "name", register.parentRegisterId);
            ViewBag.containedItemClass = new SelectList(db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);
        }

        private string GetOwner(RegisterItem registerItem)
        {
            if (registerItem.register.containedItemClass == "Document")
            {
                Document document = db.Documents.Find(registerItem.systemId);
                return document.documentowner.name;
            }
            else if (registerItem.register.containedItemClass == "Dataset")
            {
                Dataset dataset = db.Datasets.Find(registerItem.systemId);
                return dataset.datasetowner.name;
            }
            else
            {
                return registerItem.submitter.seoname;
            }
        }

        private string RedirectToApiIfFormatIsNotNull(string format)
        {
            if (!string.IsNullOrWhiteSpace(format))
            {
                return "/api/" + Request.FilePath;
            }
            else
            {
                format = _registerService.ContentNegotiation(ControllerContext);
                if (!string.IsNullOrWhiteSpace(format))
                {
                    return "/api/" + Request.FilePath + "." + format;
                }
            }
            return null;
        }

        private Models.Register RegisterItems(Models.Register register, FilterParameters filter, int? page)
        {
            if (Search(filter)) register = _searchService.Search(register, filter.text);
            register = _registerService.FilterRegisterItems(register, filter);
            return register;
        }

        private void ViewbagsRegisterDetails(string owner, string sorting, int? page, FilterParameters filter, Models.Register register)
        {
            ViewBag.search = filter.text;
            ViewBag.page = page;
            ViewBag.SortOrder = sorting;
            ViewBag.selectedMunicipality = filter.municipality;
            ViewBag.sorting = new SelectList(db.Sorting.ToList(), "value", "description");
            ViewBag.municipality = db.RegisterItems.Where(ri => ri.register.name == "Kommunenummer").OrderBy(o => o.name).ToList();
            ViewBag.register = register.name;
            ViewBag.registerSEO = register.seoname;
            ViewBag.InspireRequirement = new SelectList(db.requirements, "value", "description", null);
            ViewBag.nationalRequirement = new SelectList(db.requirements, "value", "description", null);
            ViewBag.nationalSeaRequirement = new SelectList(db.requirements, "value", "description", null);
            ViewBag.ownerSEO = owner;
        }

        private static bool Search(FilterParameters filter)
        {
            return !string.IsNullOrWhiteSpace(filter.text);
        }

        private bool IsAdmin()
        {
            return _accessControlService.IsAdmin();
        }

    }

}
