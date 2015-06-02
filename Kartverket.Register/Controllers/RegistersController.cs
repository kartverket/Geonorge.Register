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
using System.Text;
using System.Xml.Linq;
using Kartverket.Register.Services.Versioning;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services.Search;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class RegistersController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private IVersioningService _versioningService;
        private IRegisterService _registerService;
        private ISearchService _searchService;
        
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Registers
        public ActionResult Index()
        {
            setAccessRole();

            removeSessionSearchParams();

            return View(db.Registers.OrderBy(r => r.name).ToList());
        }

        private ActionResult exportCodelist(Kartverket.Register.Models.Register register, string export)
        {
            if (export == "csv")
            {

                string text = "Kode; Initialverdi; Beskrivelse\n";

                foreach (CodelistValue item in register.items)
                {
                    string description = item.description;
                    string replaceWith = " ";
                    string removedBreaksDescription = description.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);

                    text += item.name + ";" + item.value + ";" + removedBreaksDescription + "\n";
                }

                byte[] data = Encoding.UTF8.GetBytes(text);

                return File(data, "text/csv", register.name + "_kodeliste.csv");
            }
            
            else if (export == "gml")
            {
                //var queryResult = from x in db.CodelistValues
                //                  select x.value;

                string targetNamespace = "";
                string nameSpace = "";
                if (register.targetNamespace != null)
                {
                    nameSpace = register.targetNamespace;
                    if (register.targetNamespace.EndsWith("/"))
                    {
                        targetNamespace = register.targetNamespace + register.seoname;
                    }
                    else
                    {
                        targetNamespace = register.targetNamespace + "/" + register.seoname;
                    }
                }

                XNamespace ns = "http://www.opengis.net/gml/3.2";
                XNamespace xsiNs = "http://www.w3.org/2001/XMLSchema-instance";
                XNamespace gmlNs = "http://www.opengis.net/gml/3.2";
                XElement xdoc =
                    new XElement(gmlNs + "Dictionary", new XAttribute(XNamespace.Xmlns + "xsi", xsiNs),
                        new XAttribute(XNamespace.Xmlns + "gml", gmlNs),
                        new XAttribute(xsiNs + "schemaLocation", "http://www.opengis.net/gml/3.2 http://schemas.opengis.net/gml/3.2.1/gml.xsd"),
                        new XAttribute(gmlNs + "id", register.seoname),
                        new XElement(gmlNs + "description"),
                        new XElement(gmlNs + "identifier",
                            new XAttribute("codeSpace", nameSpace), register.name),

                        from k in db.CodelistValues.ToList()
                        where k.register.name == register.name && k.register.parentRegisterId == register.parentRegisterId
                        select new XElement(gmlNs + "dictionaryEntry", new XElement(gmlNs + "Definition", new XAttribute(gmlNs + "id", "_" + k.value),
                          new XElement(gmlNs + "description", k.description),
                          new XElement(gmlNs + "identifier", new XAttribute("codeSpace", targetNamespace), k.value),
                          new XElement(gmlNs + "name", k.name)
                          )));

                return new XmlResult(xdoc);
            }
            return View(register);

        }        

        // GET: Registers/Details/5
        [Route("register/{name}")]
        public ActionResult Details(string name, string sorting, int? page, string export, FilterParameters filter)
        {
            var queryResults = from o in db.Registers
                               where o.name == name || o.seoname == name
                               select o;

            Kartverket.Register.Models.Register register = queryResults.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(filter.text))
            {
                _searchService = new SearchService(db);
                register = _searchService.Search(register, filter.text);   
            }
            //Hjelpemetode Sjekk om noen av filterparametrene er satt        TODO!    
            _registerService = new RegisterService(db);
            register = _registerService.Filter(register, filter);

            ViewBag.search = filter.text;
            ViewBag.page = page;
            ViewBag.SortOrder = sorting;
            ViewBag.sorting = new SelectList(db.Sorting.ToList(), "value", "description");
            ViewBag.register = register.name;
            ViewBag.registerSEO = register.seoname;
            ViewBag.InspireRequirement = new SelectList(db.requirements, "value", "description", null);
            ViewBag.nationalRequirement = new SelectList(db.requirements, "value", "description", null);
            ViewBag.nationalSeaRequirement = new SelectList(db.requirements, "value", "description", null);

            if (register.parentRegisterId != null)
            {
                ViewBag.parentRegister = register.parentRegister.name;
            }

            if (register == null)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrEmpty(export))
            {
                return exportCodelist(register, export);
            }

            return View(register);
        }


        [Route("register/versjoner/{registername}/{submitter}/{itemname}/{version}/no")]
        [Route("register/{registername}/{submitter}/{itemname}/")]
        public ActionResult DetailsRegisterItem(string registername, string itemname, int? version)
        {
            //Guid? documentId = null;
            Guid? systId = null;
            
            if (version != null)
            {
                var queryResultDocument = from d in db.Documents
                                          where d.seoname == itemname && d.register.seoname == registername && d.versionNumber == version
                                          select d.systemId;

                systId = queryResultDocument.FirstOrDefault();

                Kartverket.Register.Models.Document document = db.Documents.Find(systId);
                ViewBag.documentOwner = document.documentowner.name;
                ViewBag.version = document.versionNumber;
            }
            else { 
                var queryResultsRegisterItem = from o in db.RegisterItems
                                               where o.seoname == itemname && o.register.seoname == registername 
                                               select o.systemId;

                systId = queryResultsRegisterItem.FirstOrDefault();
            }
           
            Kartverket.Register.Models.RegisterItem registerItem = db.RegisterItems.Find(systId);

            if (registerItem.register.containedItemClass == "Document")
            {
                Kartverket.Register.Models.Document document = db.Documents.Find(systId);
                ViewBag.documentOwner = document.documentowner.name;
            }

            if (registerItem.register.containedItemClass == "Dataset")
            {
                Kartverket.Register.Models.Dataset dataset = db.Datasets.Find(systId);
                ViewBag.datasetOwner = dataset.datasetowner.name;
            }
            return View(registerItem);
        }

        [Route("register/versjoner/{registername}/{registerItemOwner}/{itemname}/")]
        public ActionResult DetailsRegisterItemVersions(string registername, string itemname, string registerItemOwner)
        {
            _versioningService = new VersioningService(db);
            VersionsItem versionsItem = _versioningService.Versions(registername, itemname);
            RegisterItemVeiwModel model = new RegisterItemVeiwModel(versionsItem);
            ViewBag.registerItemOwner = registerItemOwner;
            return View(model);           
        }

        // GET: Register/Create
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

        // POST: Register/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [Route("ny/")]
        public ActionResult Create(Kartverket.Register.Models.Register register)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");
            if (role == "nd.metadata_admin")
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
                    register.containedItemClass = register.containedItemClass;

                    db.Registers.Add(register);
                    db.SaveChanges();

                    string organizationLogin = GetSecurityClaim("organization");

                    var queryResults = from o in db.Organizations
                                       where o.name == organizationLogin
                                       select o.systemId;

                    Guid orgId = queryResults.FirstOrDefault();
                    Organization submitterOrganisasjon = db.Organizations.Find(orgId);

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
        [Route("endre/{registername}")]
        public ActionResult Edit(string registername)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResults = from o in db.Registers
                               where o.seoname == registername
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();
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
        public ActionResult Edit(Kartverket.Register.Models.Register register, string registername, string accessRegister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin")
            {

                var queryResults = from o in db.Registers
                                   where o.seoname == registername
                                   select o.systemId;

                Guid systId = queryResults.FirstOrDefault();
                ValidationName(register);
                Kartverket.Register.Models.Register originalRegister = db.Registers.Find(systId);
                if (ModelState.IsValid)
                {
                    if (register.name != null) originalRegister.name = register.name; originalRegister.seoname = MakeSeoFriendlyString(originalRegister.name);
                    if (register.description != null) originalRegister.description = register.description;
                    if (register.owner != null) originalRegister.ownerId = register.ownerId;
                    if (register.managerId != null) originalRegister.managerId = register.managerId;
                    if (register.ownerId != null) originalRegister.ownerId = register.ownerId;
                    originalRegister.accessId = register.accessId;

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
            return HttpNotFound();
        }

        // GET: Registers/Delete/5
        [Authorize]
        [Route("slett/{registername}")]
        public ActionResult Delete(string registername)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin")
            {
                var queryResults = from o in db.Registers
                                   where o.seoname == registername
                                   select o.systemId;

                Guid systId = queryResults.FirstOrDefault();
                Kartverket.Register.Models.Register register = db.Registers.Find(systId);

                if (register == null)
                {
                    return HttpNotFound();
                }
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
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin")
            {
                var queryResults = from o in db.Registers
                                   where o.seoname == registername
                                   select o.systemId;

                Guid systId = queryResults.FirstOrDefault();
                Kartverket.Register.Models.Register register = db.Registers.Find(systId);

                var queryResultsRegisterItem = ((from o in db.RegisterItems
                                                 where o.register.seoname == registername
                                                 || o.register.parentRegister.seoname == registername
                                                 select o.systemId).Union(
                                               from r in db.Registers
                                               where r.parentRegister.seoname == registername
                                               select r.systemId));

                if (queryResultsRegisterItem.Count() > 0)
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
            ViewBag.containedItemClass = new SelectList(db.ContainedItemClass.OrderBy(s => s.description), "value", "description", string.Empty);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }

    }

}
