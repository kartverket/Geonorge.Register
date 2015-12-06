using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System.IO;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Register;
using System.Collections.Generic;

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

        // GET: Documents/Create
        [Authorize]
        [Route("dokument/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("dokument/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            Document document = new Document();
            document.register = _registerService.GetRegister(parentRegister, registername);

            if (HasAccess(document))
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
        public ActionResult Create(Document document, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, string registername, string parentRegister, string registerowner)
        {
            // Finn register         
            document.register = _registerService.GetSubregisterByName(parentRegister, registername);
            if (HasAccess(document))
            {
                if (!NameIsValid(document))
                {
                    ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                }
                else if (ModelState.IsValid)
                {
                    initialisationDocument(document, documentfile, thumbnail, registername);
                    return Redirect(RegisterUrls.DeatilsDocumentUrl(parentRegister, registerowner, registername, document.documentowner.seoname, document.seoname));
                }
            }
            return View(document);
        }


        // GET: Documents/CreateNewVersion
        [Authorize]
        [Route("dokument/versjon/{parentRegister}/{parentRegisterOwner}/{registername}/{itemOwner}/{itemname}/ny")]
        [Route("dokument/versjon/{registername}/{itemOwner}/{itemname}/ny")]
        public ActionResult CreateNewVersion(string parentRegister, string registername, string itemname)
        {
            Document document = (Document)_registerItemService.GetCurrentRegisterItem(parentRegister, registername, itemname);

            if (HasAccess(document))
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
        public ActionResult CreateNewVersion(Document document, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, string parentRegisterOwner, string parentRegister, string registername, string itemname)
        {
            document.register = _registerService.GetSubregisterByName(parentRegister, registername);
            if (HasAccess(document))
            {
                if (!NameIsValid(document))
                {
                    ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                }
                else if (ModelState.IsValid)
                {
                    initialisationDocument(document, documentfile, thumbnail, registername);
                    UpdateVersioningGroup(document);
                    return Redirect(RegisterUrls.DeatilsDocumentUrl(parentRegister, parentRegisterOwner, registername, document.documentowner.seoname, document.seoname));
                }
            }
            return View(document);
        }

        // GET: Documents/Edit/5
        [Authorize]
        [Route("dokument/{parentregister}/{registerowner}/{registername}/{itemowner}/{documentname}/rediger")]
        [Route("dokument/{registername}/{itemowner}/{documentname}/rediger")]
        public ActionResult Edit(string parentregister, string registername, string documentname, int? vnr)
        {
            string role = HtmlHelperExtensions.GetSecurityClaim("role");
            string user = HtmlHelperExtensions.GetSecurityClaim("organization");
            Document document = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, vnr.Value);

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
        [Route("dokument/{parentregister}/{registerowner}/{registername}/{itemowner}/{documentname}/rediger")]
        [Route("dokument/{registername}/{itemowner}/{documentname}/rediger")]
        public ActionResult Edit(Document document, string parentregister, string registerowner, string registername, string itemowner, string documentname, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, bool retired)
        {
            Document originalDocument = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, document.versionNumber);
            Models.Version versjonsgruppe = _registerItemService.GetVersionGroup(document.versioningId);

            if (!NameIsValid(document))
            {
                ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
            }
            else if (ModelState.IsValid)
            {
                if (document.name != null) originalDocument.name = document.name;
                originalDocument.seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(originalDocument.name);
                if (document.description != null) originalDocument.description = document.description;
                if (document.documentownerId != null) originalDocument.documentownerId = document.documentownerId;
                if (document.approvalDocument != null) originalDocument.approvalDocument = document.approvalDocument;
                if (document.approvalReference != null) originalDocument.approvalReference = document.approvalReference;
                if (document.versionName != null) originalDocument.versionName = document.versionName;

                // Finn alle dokumenter i versjonegruppen
                var allVersions = _registerItemService.GetAllVersionsOfItembyVersioningId(versjonsgruppe.systemId);

                originalDocument.Accepted = document.Accepted;
                if (document.Accepted == true && originalDocument.statusId == "Submitted" || originalDocument.statusId == "Draft")
                {
                    if (allVersions.Count > 1)
                    {
                        //Endre status på nåværende gjeldende eller erstattet versjon
                        foreach (Document item in allVersions)
                        {
                            if (item.statusId == "Valid")
                            {
                                if (document.dateAccepted == null)
                                {
                                    document.dateAccepted = DateTime.Now;
                                }
                                if (item.dateAccepted == null || item.dateAccepted < document.dateAccepted)
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

                    if (originalDocument.statusId == "Submitted" || originalDocument.statusId == "Draft")
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

                    //Endre stuts på erstattede versjoner
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
                                versjonsgruppe.currentVersion = item.systemId;
                            }
                        }
                    }
                }

                if (retired || document.Accepted == false)
                {
                    if (originalDocument.statusId == "Valid")
                    {
                        GetNewCurrentVersion(document, versjonsgruppe, allVersions);
                    }

                    if (retired)
                    {
                        originalDocument.statusId = "Retired";
                        originalDocument.DateRetired = DateTime.Now;
                    }
                    else
                    {
                        originalDocument.statusId = "Draft";
                        if (string.IsNullOrWhiteSpace(document.dateNotAccepted.ToString()))
                        {
                            originalDocument.dateNotAccepted = DateTime.Now;
                        }
                        else
                        {
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
                        document.thumbnail = GenerateThumbnail(originalDocument, documentfile, url);
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

                return Redirect(RegisterUrls.DeatilsDocumentUrl(parentregister, registerowner, registername, itemowner, documentname));
            }
            Viewbags(document);
            return View(originalDocument);
        }

        private bool NameIsValid(Document document)
        {
            return _registerItemService.validateName(document);
        }

        // GET: Documents/Delete/5
        [Authorize]
        [Route("dokument/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{documentname}/slett")]
        [Route("dokument/{registername}/{organization}/{documentname}/slett")]
        public ActionResult Delete(string registername, string documentname, int? vnr, string parentregister, string parentregisterowner)
        {
            string role = HtmlHelperExtensions.GetSecurityClaim("role");
            string user = HtmlHelperExtensions.GetSecurityClaim("organization");

            Document document = new Document();
            document = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, vnr.Value);

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
            Document document = new Document();
            document = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, versionNumber);
            string parent = null;
            if (document.register.parentRegisterId != null)
            {
                parent = document.register.parentRegister.seoname;
            }
            Models.Version versjonsgruppe = _registerItemService.GetVersionGroup(document.versioningId);
            //Dersom dokumentet som skal slettes er "gjeldende versjon" så må et annet dokument settes som gjeldende versjon
            // Finn alle dokumenter i versjonsgruppen
            if (document.systemId == versjonsgruppe.currentVersion)
            {
                var versionsOfDocument = _registerItemService.GetAllVersionsOfItembyVersioningId(versjonsgruppe.systemId);
                if (versionsOfDocument.Count() > 1)
                {
                    GetNewCurrentVersion(document, versjonsgruppe, versionsOfDocument);
                    db.RegisterItems.Remove(document);
                }
                else
                {
                    db.RegisterItems.Remove(document);
                    db.SaveChanges();
                    db.Versions.Remove(versjonsgruppe);
                }
            }
            else
            {
                db.RegisterItems.Remove(document);
            }
            db.SaveChanges();

            return Redirect(RegisterUrls.registerUrl(parentregister, parentregisterowner, registername));
        }

        private void UpdateVersioningGroup(Document document)
        {
            Models.Version versjonsgruppe = _registerItemService.GetVersionGroup(document.versioningId);
            versjonsgruppe.lastVersionNumber = document.versionNumber;
            db.SaveChanges();
        }

        private void GetNewCurrentVersion(Document document, Models.Version versjonsgruppe, List<Models.RegisterItem> versionsOfDocument)
        {
            foreach (var item in versionsOfDocument.Where(d => d.statusId == "Superseded").OrderByDescending(d => d.dateAccepted))
            {
                versjonsgruppe.currentVersion = item.systemId;
                item.statusId = "Valid";
                item.modified = DateTime.Now;
                item.dateSuperseded = null;
                break;
            }
            if (versjonsgruppe.currentVersion == document.systemId)
            {
                Document nyGjeldendeVersjon = (Document)versionsOfDocument.Where(o => o.statusId == "Retired").Where(o => o.systemId != document.systemId).OrderByDescending(d => d.DateRetired).FirstOrDefault();
                if (nyGjeldendeVersjon != null)
                {
                    versjonsgruppe.currentVersion = nyGjeldendeVersjon.systemId;
                }
                else
                {
                    nyGjeldendeVersjon = (Document)versionsOfDocument.Where(o => o.statusId == "Draft").Where(o => o.systemId != document.systemId).OrderBy(d => d.dateSubmitted).FirstOrDefault();
                    if (nyGjeldendeVersjon != null)
                    {
                        versjonsgruppe.currentVersion = nyGjeldendeVersjon.systemId;
                    }
                    else
                    {
                        nyGjeldendeVersjon = (Document)versionsOfDocument.Where(o => o.statusId == "Submitted").Where(o => o.systemId != document.systemId).OrderBy(d => d.dateSubmitted).FirstOrDefault();
                        if (nyGjeldendeVersjon != null)
                        {
                            versjonsgruppe.currentVersion = nyGjeldendeVersjon.systemId;
                        }
                        else
                        {
                            nyGjeldendeVersjon = (Document)versionsOfDocument.FirstOrDefault();
                        }
                    }
                }
            }
            db.Entry(versjonsgruppe).State = EntityState.Modified;
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private static bool HasAccess(Models.RegisterItem registerItem)
        {
            string role = HtmlHelperExtensions.GetSecurityClaim("role");
            string user = HtmlHelperExtensions.GetSecurityClaim("organization");

            if (true)
            {

            }
            return role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && registerItem.register.accessId == 2);
        }

        private string GenerateThumbnail(Document document, HttpPostedFileBase documentfile, string url)
        {
            if (document.documentUrl.Contains(".pdf"))
            {
                string filtype;
                string seofilename;
                MakeSeoFriendlyDocumentName(documentfile, out filtype, out seofilename);

                string input = Path.Combine(Server.MapPath(Constants.DataDirectory + Document.DataDirectory), document.register.seoname + "_" + document.name + "_v" + document.versionNumber + "_" + seofilename + "." + filtype);
                string output = Path.Combine(Server.MapPath(Constants.DataDirectory + Document.DataDirectory), document.register.seoname + "_thumbnail_" + document.name + "_v" + document.versionNumber + ".jpg");
                GhostscriptSharp.GhostscriptWrapper.GeneratePageThumb(input, output, 1, 150, 197);
                return url + document.register.seoname + "_thumbnail_" + document.name + "_v" + document.versionNumber + ".jpg";
            }
            else
            {
                return "/Content/pdf.jpg";
            }
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
                seofilename += RegisterUrls.MakeSeoFriendlyString(item) + "_";
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

        private void initialisationDocument(Document document, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, string registername)
        {
            document.systemId = Guid.NewGuid();
            document.modified = DateTime.Now;
            document.dateSubmitted = DateTime.Now;
            document.registerId = document.register.systemId;
            document.statusId = "Submitted";
            document.versionNumber = getVersionNr(document.versionNumber);
            document.name = DocumentName(document.name);
            document.seoname = DocumentSeoName(document.name);
            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;
            document.documentUrl = documentUrl(url, documentfile, document);
            document.thumbnail = GetThumbnail(document, documentfile, url, thumbnail);
            document.versioningId = GetVersioningId(document);

            SetDocumentOwnerAndSubmitter(document);

            db.Entry(document).State = EntityState.Modified;
            db.RegisterItems.Add(document);
            db.SaveChanges();
        }

        private string GetThumbnail(Document document, HttpPostedFileBase documentfile, string url, HttpPostedFileBase thumbnail)
        {
            if (documentfile != null)
            {
                document.thumbnail = GenerateThumbnail(document, documentfile, url);
            }

            if (thumbnail != null && document.thumbnail.Contains(thumbnail.FileName))
            {
                document.thumbnail = url + SaveFileToDisk(thumbnail, document.name, document.register.seoname, document.versionNumber);
            }
            return document.thumbnail;
        }

        private Guid? GetVersioningId(Document document)
        {
            if (document.versioningId == null)
            {
                return _registerItemService.NewVersioningGroup(document);
            }
            else
            {
                return document.versioningId;
            }
        }

        private int getVersionNr(int versionNumber)
        {
            if (versionNumber == 0)
            {
                versionNumber = 1;
            }
            else
            {
                versionNumber++;
            }
            return versionNumber;
        }

        private void SetDocumentOwnerAndSubmitter(Document document)
        {
            string organizationLogin = HtmlHelperExtensions.GetSecurityClaim("organization");
            Organization submitterOrganisasjon = _registerService.GetOrganization(organizationLogin);
            document.submitterId = submitterOrganisasjon.systemId;
            document.submitter = submitterOrganisasjon;
            document.documentowner = submitterOrganisasjon;
            document.documentownerId = submitterOrganisasjon.systemId;
        }

        private string DocumentSeoName(string name)
        {
            return Helpers.RegisterUrls.MakeSeoFriendlyString(name);
        }

        private string DocumentName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "ikke angitt";
            }
            else
            {
                return name;
            }
        }

        private string documentUrl(string url, HttpPostedFileBase documentfile, Document document)
        {
            if (documentfile != null)
            {
                return url + SaveFileToDisk(documentfile, document.name, document.register.seoname, document.versionNumber);
            }
            else
            {
                return "ikke angitt";
            }
        }
    }
}
