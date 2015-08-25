using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System.IO;
using Kartverket.Register.Helpers;
using System.Text.RegularExpressions;
using System.Drawing;
using Kartverket.Register.Services.Versioning;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Register;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class DocumentsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        public DocumentsController()
        {
            _registerItemService = new RegisterItemService(db);
            _registerService = new RegisterService(db);
        }

        // GET: Documents
        public ActionResult Index()
        {
            var registerItems = db.Documents.Include(d => d.register).Include(d => d.status).Include(d => d.submitter).Include(d => d.documentowner);
            return View(registerItems.ToList());
        }

        // GET: Documents/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Documents/Create
        [Authorize]
        [Route("dokument/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("dokument/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            Document document = new Document();
            Kartverket.Register.Models.Register register = _registerService.GetSubRegisterByNameAndParent(registername, parentRegister);
            document.register = register;

            if (register.parentRegisterId != null) document.register.parentRegister = register.parentRegister;

            string role = HtmlHelperExtensions.GetSecurityClaim("role");
            string user = HtmlHelperExtensions.GetSecurityClaim("organization");

            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && register.accessId == 2))
            {
                return View(document);
            }
            return HttpNotFound("Ingen tilgang");

        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("dokument/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("dokument/{registername}/ny")]
        public ActionResult Create(Document document, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, string registername, string parentRegister)
        {
            // Finn register         
            Kartverket.Register.Models.Register register = _registerService.GetSubRegisterByNameAndParent(registername, parentRegister);

            string parentRegisterOwner = null;
            if (register.parentRegisterId != null) parentRegisterOwner = register.parentRegister.owner.seoname;
            ValidationName(document, register);

            if (ModelState.IsValid)
            {
                //Tildel verdi til dokumentobjektet
                document.systemId = Guid.NewGuid();
                document.modified = DateTime.Now;
                document.dateSubmitted = DateTime.Now;
                document.registerId = register.systemId;
                document.statusId = "Submitted";
                document.versionNumber = 1;

                if (string.IsNullOrWhiteSpace(document.name)) document.name = "ikke angitt"; document.seoname = Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(document.name);
                if (string.IsNullOrWhiteSpace(document.description)) document.description = "ikke angitt";

                //Dokument og thumbnail
                string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;
                if (documentfile != null)
                {
                    document.documentUrl = url + SaveFileToDisk(documentfile, document.name, register.seoname, document.versionNumber);
                    if (document.documentUrl.Contains(".pdf"))
                    {
                        GenerateThumbnail(document, documentfile, url, registername);
                    }
                }
                if (thumbnail != null) document.thumbnail = url + SaveFileToDisk(thumbnail, document.name, register.seoname, document.versionNumber);
                if (document.documentUrl == null) document.documentUrl = "ikke angitt";

                // Opprette versjonering av et element
                Kartverket.Register.Models.Version versjoneringsGruppe = new Kartverket.Register.Models.Version();
                versjoneringsGruppe.systemId = Guid.NewGuid();
                versjoneringsGruppe.currentVersion = document.systemId;
                versjoneringsGruppe.containedItemClass = "Documents";
                versjoneringsGruppe.lastVersionNumber = document.versionNumber;
                document.versioningId = versjoneringsGruppe.systemId;

                db.Entry(versjoneringsGruppe).State = EntityState.Modified;
                db.Versions.Add(versjoneringsGruppe);

                // Hente innsender og eier ut fra innlogget bruker                
                string organizationLogin = HtmlHelperExtensions.GetSecurityClaim("organization");
                Organization submitterOrganisasjon = _registerService.GetOrganization(organizationLogin);
                document.submitterId = submitterOrganisasjon.systemId;
                document.submitter = submitterOrganisasjon;
                document.documentowner = submitterOrganisasjon;
                document.documentownerId = submitterOrganisasjon.systemId;

                // Legger inn det nye dokumentet i db
                db.Entry(document).State = EntityState.Modified;
                db.RegisterItems.Add(document);
                db.SaveChanges();

                if (!String.IsNullOrWhiteSpace(parentRegister))
                {
                    return Redirect(HtmlHelperExtensions.VersionsInSubregisterURL(parentRegister, parentRegisterOwner, registername, document.documentowner.seoname, document.seoname));
                }
                else
                {
                    return Redirect(HtmlHelperExtensions.VersionsUrl(registername, document.documentowner.seoname, document.seoname));
                }
            }
            document.register = register;
            return View(document);
        }

        // GET: Documents/CreateNewVersion
        [Authorize]
        [Route("dokument/versjon/{parentRegister}/{parentRegisterOwner}/{registername}/{itemOwner}/{itemname}/ny")]
        [Route("dokument/versjon/{registername}/{itemOwner}/{itemname}/ny")]
        public ActionResult CreateNewVersion(string parentRegister, string registername, string itemname)
        {
            string role = HtmlHelperExtensions.GetSecurityClaim("role");
            string user = HtmlHelperExtensions.GetSecurityClaim("organization");

            Document document = (Document)_registerItemService.getCurrentRegisterItem(parentRegister, registername, itemname);
            document.versionName = null;
            if (document == null)
            {
                return HttpNotFound();
            }

            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && document.register.accessId == 2))
            {
                Viewbags(document);
                return View(document);
            }
            return HttpNotFound("Ingen tilgang");

        }

        // POST: Documents/CreateNewVersion
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("dokument/versjon/{parentRegister}/{parentRegisterOwner}/{registername}/{itemOwner}/{itemname}/ny")]
        [Route("dokument/versjon/{registername}/{itemOwner}/{itemname}/ny")]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateNewVersion(Document document, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, string parentRegisterOwner, string parentRegister, string registername, string itemname)
        {
            if (ModelState.IsValid)
            {
                document.systemId = Guid.NewGuid();
                document.modified = DateTime.Now;
                document.dateSubmitted = DateTime.Now;
                document.statusId = "Submitted";
                document.versionNumber++;

                if (string.IsNullOrWhiteSpace(document.description)) document.description = "ikke angitt";
                document.seoname = Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(document.name);
                string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;

                if (documentfile != null)
                {
                    document.documentUrl = url + SaveFileToDisk(documentfile, document.name, registername, document.versionNumber);
                    if (document.documentUrl.Contains(".pdf"))
                    {
                        GenerateThumbnail(document, documentfile, url, registername);
                    }
                }
                if (thumbnail != null)
                {
                    document.thumbnail = url + SaveFileToDisk(thumbnail, document.name, registername, document.versionNumber);
                }
                if (document.documentUrl == null)
                {
                    document.documentUrl = "ikke angitt";
                }

                string organizationLogin = HtmlHelperExtensions.GetSecurityClaim("organization");
                Organization submitterOrganisasjon = _registerService.GetOrganization(organizationLogin);

                document.submitterId = submitterOrganisasjon.systemId;
                document.submitter = submitterOrganisasjon;
                document.documentowner = submitterOrganisasjon;
                document.documentownerId = submitterOrganisasjon.systemId;

                db.Entry(document).State = EntityState.Modified;
                db.RegisterItems.Add(document);
                db.SaveChanges();

                //Oppdater versjoneringsgruppen
                Kartverket.Register.Models.Version versjonsgruppe = _registerItemService.GetVersionGroup(document.versioningId);
                versjonsgruppe.lastVersionNumber = document.versionNumber;
                db.SaveChanges();

                if (!String.IsNullOrWhiteSpace(parentRegister))
                {
                    return Redirect(HtmlHelperExtensions.VersionsInSubregisterURL(parentRegister, parentRegisterOwner, registername, document.documentowner.seoname, document.seoname));
                }
                else
                {
                    return Redirect(HtmlHelperExtensions.VersionsUrl(registername, document.documentowner.seoname, document.seoname));
                }
            }
            return View(document);
        }


        // GET: Documents/Edit/5
        [Authorize]
        [Route("dokument/{parentRegister}/{registerowner}/{registername}/{itemowner}/{documentname}/rediger")]
        [Route("dokument/{registername}/{organization}/{documentname}/rediger")]
        public ActionResult Edit(string registername, string documentname, int? vnr, string parentRegister)
        {
            string role = HtmlHelperExtensions.GetSecurityClaim("role");
            string user = HtmlHelperExtensions.GetSecurityClaim("organization");

            Document document = (Document)_registerItemService.GetRegisterItemByVersionNr(parentRegister, registername, documentname, vnr);
            if (document == null)
            {
                return HttpNotFound();
            }

            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor")
                && document.register.accessId == 2 && document.documentowner.name.ToLower() == user.ToLower()))
            {
                Viewbags(document);
                return View(document);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("dokument/{parentRegister}/{registerowner}/{registername}/{itemowner}/{documentname}/rediger")]
        [Route("dokument/{registername}/{organization}/{documentname}/rediger")]
        public ActionResult Edit(Document document, string parentRegister, string registername, string documentname, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, bool retired)
        {
            Document originalDocument = (Document)_registerItemService.GetRegisterItemByVersionNr(parentRegister, registername, documentname, document.versionNumber);
            Kartverket.Register.Models.Register register = _registerService.GetSubRegisterByNameAndParent(registername, parentRegister);
            Kartverket.Register.Models.Version versjonsgruppe = _registerItemService.GetVersionGroup(document.versioningId);
            ValidationName(document, register);

            if (ModelState.IsValid)
            {
                if (document.name != null) originalDocument.name = document.name;
                originalDocument.seoname = Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(originalDocument.name);
                if (document.description != null) originalDocument.description = document.description;
                if (document.documentownerId != null) originalDocument.documentownerId = document.documentownerId;
                if (document.approvalDocument != null) originalDocument.approvalDocument = document.approvalDocument;
                if (document.approvalReference != null) originalDocument.approvalReference = document.approvalReference;
                if (document.versionName != null) originalDocument.versionName = document.versionName;

                // Finn alle dokumenter i versjonegruppen
                var allVersions = _registerItemService.GetAllVersionsOfDocument(versjonsgruppe.systemId);

                originalDocument.Accepted = document.Accepted;
                if (document.Accepted == true && originalDocument.statusId == "Submitted")
                {
                    if (allVersions.Count > 1)
                    {
                        //Endre status på nåværende gjeldende eller erstattet versjon
                        foreach (Document item in allVersions)
                        {
                            if (item.statusId == "Valid")
                            {
                                if (item.dateAccepted < document.dateAccepted)
                                {
                                    if (string.IsNullOrWhiteSpace(document.dateAccepted.ToString()))
                                    {
                                        originalDocument.dateAccepted = DateTime.Now;
                                    }
                                    else
                                    {
                                        originalDocument.dateAccepted = document.dateAccepted;
                                    }
                                    item.statusId = "Superseded";
                                    item.dateSuperseded = originalDocument.dateAccepted;
                                    item.modified = DateTime.Now;
                                    originalDocument.statusId = "Valid";
                                    versjonsgruppe.currentVersion = originalDocument.systemId;
                                }
                                if (originalDocument.statusId == "Submitted")
                                {
                                    originalDocument.statusId = "Superseded";
                                    originalDocument.dateSuperseded = DateTime.Now;
                                }
                            }
                        }
                    }
                    else
                    {
                        originalDocument.statusId = "Valid";
                        versjonsgruppe.currentVersion = originalDocument.systemId;
                    }
                    db.SaveChanges();

                    if (!string.IsNullOrWhiteSpace(document.dateAccepted.ToString()))
                    {
                        originalDocument.dateAccepted = document.dateAccepted;
                    }
                    else
                    {
                        originalDocument.dateAccepted = DateTime.Now;
                    }
                }

                if (document.dateAccepted != null && (originalDocument.dateAccepted != document.dateAccepted))
                {
                    originalDocument.dateAccepted = document.dateAccepted;

                    //Endre status på nåværende gjeldende eller erstattet versjon
                    foreach (Document item in allVersions)
                    {
                        if (item.statusId == "Superseded")
                        {
                            if (item.dateAccepted > document.dateAccepted)
                            {
                                item.statusId = "Valid";
                                item.dateSuperseded = null;
                                item.modified = DateTime.Now;
                                originalDocument.statusId = "Superseded";
                                originalDocument.dateSuperseded = DateTime.Now;
                                versjonsgruppe.currentVersion =  item.systemId;
                            }
                        }
                    }
                }

                if (retired || document.Accepted == false)
                {
                    if (originalDocument.statusId == "Valid")
                    {
                        if (allVersions.Count() > 1)
                        {
                            // Sett gjeldende versjon ut fra status...                            
                            foreach (var item in allVersions.Where(o => o.statusId == "Superseded").OrderByDescending(o => o.dateAccepted))
                            {
                                if (item.systemId != document.systemId)
                                {
                                    versjonsgruppe.currentVersion = item.systemId;
                                    item.statusId = "Valid";
                                    item.dateSuperseded = null;
                                    item.modified = DateTime.Now;
                                    break;
                                }
                            }
                            db.SaveChanges();
                            if (versjonsgruppe.currentVersion == document.systemId)
                            {
                                Document nyGjeldendeVersjon = (Document)allVersions.OrderByDescending(o => o.dateSubmitted).FirstOrDefault();
                                versjonsgruppe.currentVersion = nyGjeldendeVersjon.systemId;
                            }
                        }
                    }

                    if (retired)
                    {
                        originalDocument.statusId = "Retired";
                        originalDocument.DateRetired = DateTime.Now;
                    }
                    else
                    {
                        originalDocument.statusId = "NotAccepted";
                        if (string.IsNullOrWhiteSpace(document.dateNotAccepted.ToString()))
                        {
                            originalDocument.dateNotAccepted = DateTime.Now;
                        }
                        else {
                            originalDocument.dateNotAccepted = document.dateNotAccepted;
                        }
                        
                    }
                    if (originalDocument.Accepted == false)
                    {
                        if (string.IsNullOrWhiteSpace(document.dateNotAccepted.ToString()))
                        {
                            originalDocument.dateNotAccepted = DateTime.Now;
                        }
                        else
                        {
                            originalDocument.dateNotAccepted = document.dateNotAccepted;
                        }
                    }
                }

                if (document.documentUrl != null && document.documentUrl != originalDocument.documentUrl)
                {
                    originalDocument.documentUrl = document.documentUrl;
                }
                if (document.submitterId != null) originalDocument.submitterId = document.submitterId;

                string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;

                if (documentfile != null)
                {
                    originalDocument.documentUrl = url + SaveFileToDisk(documentfile, originalDocument.name, originalDocument.register.seoname, originalDocument.versionNumber);
                    if (originalDocument.documentUrl.Contains(".pdf"))
                    {
                        GenerateThumbnail(document, documentfile, url, registername);
                        originalDocument.thumbnail = document.thumbnail;
                    }
                }

                if (thumbnail != null && document.thumbnail != originalDocument.thumbnail)
                {
                    originalDocument.thumbnail = url + SaveFileToDisk(thumbnail, originalDocument.name, originalDocument.register.seoname, originalDocument.versionNumber);
                }

                originalDocument.modified = DateTime.Now;
                db.Entry(originalDocument).State = EntityState.Modified;
                db.SaveChanges();
                Viewbags(document);

                //Retur? Gjeldende versjon!!

                if (!String.IsNullOrWhiteSpace(parentRegister))
                {
                    return Redirect(HtmlHelperExtensions.VersionsInSubregisterURL(parentRegister, register.parentRegister.owner.seoname, registername, originalDocument.documentowner.seoname, originalDocument.seoname));
                }
                else
                {
                    return Redirect(HtmlHelperExtensions.VersionsUrl(registername, originalDocument.documentowner.seoname, originalDocument.seoname));
                }
            }
            Viewbags(document);
            return View(originalDocument);
        }

        // GET: Documents/Delete/5
        [Authorize]
        [Route("dokument/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{documentname}/slett")]
        [Route("dokument/{registername}/{organization}/{documentname}/slett")]
        public ActionResult Delete(string registername, string documentname, int? vnr, string parentregister, string parentregisterowner)
        {
            string role = HtmlHelperExtensions.GetSecurityClaim("role");
            string user = HtmlHelperExtensions.GetSecurityClaim("organization");

            Document document = (Document)_registerItemService.GetRegisterItemByVersionNr(parentregister, registername, documentname, vnr);
            if (document == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && document.register.accessId == 2 && document.documentowner.name.ToLower() == user.ToLower()))
            {
                Viewbags(document);
                return View(document);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("dokument/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{documentname}/slett")]
        [Route("dokument/{registername}/{organization}/{documentname}/slett")]
        public ActionResult DeleteConfirmed(string registername, string documentname, int versionNumber, string parentregister, string parentregisterowner)
        {
            Document document = (Document)_registerItemService.GetRegisterItemByVersionNr(parentregister, registername, documentname, versionNumber);
            string parent = null;
            if (document.register.parentRegisterId != null)
            {
                parent = document.register.parentRegister.seoname;
            }
            Kartverket.Register.Models.Version versjonsgruppe = _registerItemService.GetVersionGroup(document.versioningId);
            //Dersom dokumentet som skal slettes er "gjeldende versjon" så må et annet dokument settes som gjeldende versjon
            // Finn alle dokumenter i versjonsgruppen
            if (document.statusId == "Valid")
            {
                var documentVersions = _registerItemService.GetAllVersionsOfDocument(versjonsgruppe.systemId);
                if (documentVersions.Count() > 0)
                {
                    // Sett gjeldende versjon ut fra status...
                    foreach (var item in documentVersions.Where(d => d.statusId == "Superseded").OrderByDescending(d => d.dateAccepted))
                    {
                        versjonsgruppe.currentVersion = item.systemId;
                        item.statusId = "Valid";
                        item.modified = DateTime.Now;
                        item.dateSuperseded = null;
                        break;                        
                    }
                    if (versjonsgruppe.currentVersion == document.systemId)
                    {
                        Document nyGjeldendeVersjon = (Document)documentVersions.OrderByDescending(o => o.dateSubmitted).FirstOrDefault();
                        versjonsgruppe.currentVersion = nyGjeldendeVersjon.systemId;
                    }
                    db.SaveChanges();
                }

            }

            db.RegisterItems.Remove(document);
            db.SaveChanges();

            if (parent != null)
            {
                return Redirect(HtmlHelperExtensions.SubRegisterUrl(parentregister, parentregisterowner, registername));
            }

            return Redirect(HtmlHelperExtensions.RegisterUrl(registername));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void GenerateThumbnail(Document document, HttpPostedFileBase documentfile, string url, string register)
        {
            string filtype;
            string seofilename;
            MakeSeoFriendlyDocumentName(documentfile, out filtype, out seofilename);

            string input = Path.Combine(Server.MapPath(Constants.DataDirectory + Document.DataDirectory), register + "_" + document.name + "_v" + document.versionNumber + "_" + seofilename + "." + filtype);
            string output = Path.Combine(Server.MapPath(Constants.DataDirectory + Document.DataDirectory), register + "_thumbnail_" + document.name + "_v" + document.versionNumber + ".jpg");
            GhostscriptSharp.GhostscriptWrapper.GeneratePageThumb(input, output, 1, 150, 197);
            document.thumbnail = url + register + "_thumbnail_" + document.name + "_v" + document.versionNumber + ".jpg";
        }


        private string SaveFileToDisk(HttpPostedFileBase file, string name, string register, int vnr)
        {
            string filtype;
            string seofilename;
            MakeSeoFriendlyDocumentName(file, out filtype, out seofilename);

            string filename = register + "_" + name + "_v" + vnr + "_" + seofilename + "." + filtype;
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Document.DataDirectory), filename);
            file.SaveAs(path);
            return filename;
        }

        private static void MakeSeoFriendlyDocumentName(HttpPostedFileBase file, out string filtype, out string seofilename)
        {
            string[] documentfilename = file.FileName.Split('.');
            filtype = documentfilename.Last();
            seofilename = null;
            foreach (string item in documentfilename)
            {
                if (item == filtype)
                {
                    break;
                }
                seofilename += Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(item) + "_";
            }
        }

        private void ValidationName(Document document, Kartverket.Register.Models.Register register)
        {
            var queryResultsDataset = from o in db.Documents
                                      where o.name == document.name &&
                                      o.systemId != document.systemId &&
                                      o.register.name == register.name &&
                                      o.versioningId != document.versioningId &&
                                      o.register.parentRegisterId == register.parentRegisterId
                                      select o.systemId;

            if (queryResultsDataset.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");
            }
        }

        private void Viewbags(Document document)
        {
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", document.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", document.submitterId);
            ViewBag.documentownerId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", document.documentownerId);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }

    }
}
