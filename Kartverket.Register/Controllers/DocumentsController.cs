using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System.IO;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services;
using GhostscriptSharp.Settings;
using System.Drawing;
using GhostscriptSharp;
//ghostscriptsharp MIT license:
//Copyright(c) 2009 Matthew Ephraim
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class DocumentsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _accessControlService;

        public DocumentsController()
        {
            _registerItemService = new RegisterItemService(db);
            _registerService = new RegisterService(db);
            _accessControlService = new AccessControlService();
        }


        // GET: Documents/Create
        [Authorize]
        [Route("dokument/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("dokument/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            Document document = new Document();
            document.register = _registerService.GetRegister(parentRegister, registername);
            if (document.register != null)
            {
                if (_accessControlService.Access(document.register))
                {
                    return View(document);
                }
                return HttpNotFound("Ingen tilgang");
            }
            return HttpNotFound("Finner ikke registeret");
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
            document.register = _registerService.GetSubregisterByName(parentRegister, registername);
            if (_accessControlService.Access(document.register))
            {
                if (!NameIsValid(document))
                {
                    ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                }
                else if (ModelState.IsValid)
                {
                    document = initialisationDocument(document, documentfile, thumbnail);
                    return Redirect(document.GetObjectUrl());
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
            if (document != null)
            {
                if (_accessControlService.Access(document.register))
                {
                    Viewbags(document);
                    return View(document);
                }
                else {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound("Finner ikke dokumentet");
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
            if (_accessControlService.Access(document.register))
            {
                if (!NameIsValid(document))
                {
                    ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                }
                else if (ModelState.IsValid)
                {
                    document = initialisationDocument(document, documentfile, thumbnail);
                    return Redirect(document.GetObjectUrl());
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
            Document document = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, vnr.Value);
            if (document != null)
            {
                if (_accessControlService.Access(document))
                {
                    Viewbags(document);
                    return View(document);
                }
                return HttpNotFound("Ingen tilgang");
            }
            return HttpNotFound();
        }


        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("dokument/{parentregister}/{registerowner}/{registername}/{itemowner}/{documentname}/rediger")]
        [Route("dokument/{registername}/{itemowner}/{documentname}/rediger")]
        public ActionResult Edit(Document document, string parentregister, string registerowner, string registername, string itemowner, string documentname, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, bool retired, bool sosi)
        {
            Document originalDocument = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, document.versionNumber);
            if (originalDocument != null)
            {
                if (!NameIsValid(document))
                {
                    ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                }
                else if (ModelState.IsValid)
                {
                    document = initialisationDocument(document, documentfile, thumbnail, retired, sosi, originalDocument);
                    return Redirect(document.GetObjectUrl());
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
            Document document = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, vnr.Value);
            if (document != null)
            {
                if (_accessControlService.Access(document))
                {
                    Viewbags(document);
                    return View(document);
                }
                else {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound();
        }


        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("dokument/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{documentname}/slett")]
        [Route("dokument/{registername}/{organization}/{documentname}/slett")]
        public ActionResult DeleteConfirmed(string registername, string documentname, int versionNumber, string parentregister, string parentregisterowner)
        {
            Document document = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, versionNumber);
            string registerUrl = document.register.GetObjectUrl();
            if (DocumentIsCurrentVersion(document))
            {
                Models.Version versjonsgruppe = document.versioning;
                var versionsOfDocument = _registerItemService.GetAllVersionsOfItembyVersioningId(versjonsgruppe.systemId);
                if (versionsOfDocument.Count() > 1)
                {
                    GetNewCurrentVersion(document);
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

            return Redirect(registerUrl);
        }



        //******* HJELPEMETODER ********
        private static bool DocumentIsCurrentVersion(Document document)
        {
            return document.systemId == document.versioning.currentVersion;
        }

        /// <summary>
        /// Validate item name. Item name must be unique in a register. 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private bool NameIsValid(Document document)
        {
            return _registerItemService.validateName(document);
        }

        /// <summary>
        /// Update latest version number in versioning group.
        /// </summary>
        /// <param name="versioningId"></param>
        /// <param name="versionNumber"></param>
        private void UpdateLatestVersionNumberInVersioningGroup(Guid? versioningId, int versionNumber)
        {
            Models.Version versjonsgruppe = _registerItemService.GetVersionGroup(versioningId);
            versjonsgruppe.lastVersionNumber = versionNumber;
            db.SaveChanges();
        }

        /// <summary>
        /// If you delete or change status on current version, it needs to be replaced by another version
        /// </summary>
        /// <param name="document"></param>
        private void GetNewCurrentVersion(Document document)
        {
            var versionsOfDocument = _registerItemService.GetAllVersionsOfItembyVersioningId(document.versioning.systemId);
            foreach (var item in versionsOfDocument.Where(d => d.statusId == "Superseded").OrderByDescending(d => d.dateAccepted))
            {
                document.versioning.currentVersion = item.systemId;
                item.statusId = "Valid";
                item.modified = DateTime.Now;
                item.dateSuperseded = null;
                break;
            }
            if (document.versioning.currentVersion == document.systemId)
            {
                Document nyGjeldendeVersjon = (Document)versionsOfDocument.Where(o => o.statusId == "Draft").Where(o => o.systemId != document.systemId).OrderBy(d => d.dateSubmitted).FirstOrDefault();
                if (nyGjeldendeVersjon != null)
                {
                    document.versioning.currentVersion = nyGjeldendeVersjon.systemId;
                }
                else
                {
                    nyGjeldendeVersjon = (Document)versionsOfDocument.Where(o => o.statusId == "Submitted").Where(o => o.systemId != document.systemId).OrderBy(d => d.dateSubmitted).FirstOrDefault();
                    if (nyGjeldendeVersjon != null)
                    {
                        document.versioning.currentVersion = nyGjeldendeVersjon.systemId;
                    }
                    else
                    {
                        nyGjeldendeVersjon = (Document)versionsOfDocument.Where(o => o.statusId == "Retired").Where(o => o.systemId != document.systemId).OrderByDescending(d => d.DateRetired).FirstOrDefault();
                        if (nyGjeldendeVersjon != null)
                        {
                            document.versioning.currentVersion = nyGjeldendeVersjon.systemId;
                        }
                        else
                        {
                            nyGjeldendeVersjon = (Document)versionsOfDocument.FirstOrDefault();
                        }
                    }
                }
            }
            db.Entry(document.versioning).State = EntityState.Modified;
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

        /// <summary>
        /// Generate a thumbnail from frontpage of a PDF document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="documentfile"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GenerateThumbnail(Document document, HttpPostedFileBase documentfile, string url)
        {
            if (document.documentUrl.Contains(".pdf"))
            {
                string filtype;
                string seofilename = MakeSeoFriendlyDocumentName(documentfile, out filtype, out seofilename);

                string input = Path.Combine(Server.MapPath(Constants.DataDirectory + Document.DataDirectory), document.register.name + "_" + document.name + "_v" + document.versionNumber + "_" + seofilename + "." + filtype);
                string output = Path.Combine(Server.MapPath(Constants.DataDirectory + Document.DataDirectory), document.register.name + "_thumbnail_" + document.name + "_v" + document.versionNumber + "_" + seofilename + ".jpg");
                GhostscriptWrapper.GenerateOutput(input, output, GsSettings());

                ImageResizer.ImageJob newImage =
                                new ImageResizer.ImageJob(output, output,
                                new ImageResizer.Instructions("maxwidth=160;maxheight=300;quality=75"));

                newImage.Build();

                return url + document.register.seoname + "_thumbnail_" + document.name + "_v" + document.versionNumber + "_" + seofilename + ".jpg";

            }
            else
            {
                return "/Content/pdf.jpg";
            }
        }

        private static GhostscriptSettings GsSettings()
        {
            GhostscriptSettings gsSettings = new GhostscriptSettings
            {
                Device = GhostscriptDevices.jpeg,
                Page = new GhostscriptPages
                {
                    Start = 1,
                    End = 1,
                    AllPages = false
                },
                Resolution = new Size
                {
                    //dpi
                    Height = 20,
                    Width = 20
                },
                Size = new GhostscriptPageSize
                {
                    Native = GhostscriptPageSizes.a4
                }
            };
            return gsSettings;
        }

        private string SaveFileToDisk(HttpPostedFileBase file, string name, string register, int vnr)
        {
            string filtype;
            string seofilename = MakeSeoFriendlyDocumentName(file, out filtype, out seofilename);

            string filename = register + "_" + name + "_v" + vnr + "_" + seofilename + "." + filtype;
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Document.DataDirectory), filename);
            file.SaveAs(path);
            return filename;
        }

        /// <summary>
        /// Makes an SEO friendly document name
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filtype"></param>
        /// <param name="seofilename"></param>
        private string MakeSeoFriendlyDocumentName(HttpPostedFileBase file, out string filtype, out string seofilename)
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
            return seofilename;
        }

        private void Viewbags(Document document)
        {
            ViewBag.statusId = _registerItemService.GetStatusSelectList(document);
            ViewBag.submitterId = _registerItemService.GetSubmitterSelectList(document.submitterId);
            ViewBag.documentownerId = _registerItemService.GetOwnerSelectList(document.documentownerId);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }


        private Document initialisationDocument(Document inputDocument, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, bool retired = false, bool sosi = false, Document originalDocument = null)
        {
            Document document = GetDocument(originalDocument);
            document.systemId = GetSystemId(document.systemId);
            document.name = DocumentName(inputDocument.name);
            document.seoname = DocumentSeoName(inputDocument.name);
            document.modified = DateTime.Now;
            document.description = inputDocument.description;
            document.approvalDocument = inputDocument.approvalDocument;
            document.approvalReference = inputDocument.approvalReference;
            document.ApplicationSchema = inputDocument.ApplicationSchema;
            document.GMLApplicationSchema = inputDocument.GMLApplicationSchema;
            document.versionName = inputDocument.versionName;
            document.versionNumber = GetVersionNr(inputDocument.versionNumber, originalDocument, inputDocument);
            document.registerId = GetRegisterId(inputDocument, document);
            document.Accepted = inputDocument.Accepted;
            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;
            document.documentUrl = documentUrl(url, documentfile, document.documentUrl, document.name, document.register.name, document.versionNumber);
            document.thumbnail = GetThumbnail(document, documentfile, url, thumbnail);
            document.documentownerId = GetDocumentOwnerId(inputDocument.documentownerId);
            document.submitterId = GetSubmitterId(inputDocument.submitterId);
            document.versioningId = GetVersioningId(document, inputDocument.versioningId);

            if (originalDocument == null)
            {
                document.dateSubmitted = DateTime.Now;
                document.statusId = "Submitted";
                db.Entry(document).State = EntityState.Modified;
                db.RegisterItems.Add(document);
            }
            else {
                ApprovalProcess(document, retired, inputDocument, sosi);
                db.Entry(document).State = EntityState.Modified;
            }
            db.SaveChanges();
            return document;
        }

        /// <summary>
        /// Returns systemId
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        private Guid GetSystemId(Guid systemId)
        {
            if (systemId == Guid.Empty)
            {
                return Guid.NewGuid();
            }
            else {
                return systemId;
            }
        }

        /// <summary>
        /// Returns registerId
        /// </summary>
        /// <param name="inputDocument"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        private Guid GetRegisterId(Document inputDocument, Document document)
        {
            if (inputDocument.register == null)
            {
                return document.register.systemId;
            }
            else {
                document.register = inputDocument.register;
                return inputDocument.register.systemId;
            }
        }

        private Document GetDocument(Document originalDocument)
        {
            if (originalDocument != null)
            {
                return originalDocument;
            }
            else {
                return new Document();
            }
        }

        private void ApprovalProcess(Document document, bool retired, Document inputDocument, bool sosi)
        {
            if (document.Accepted == true)
            {
                document = SetStatusIdWhenDocumentIsAccepted(document, inputDocument, retired, sosi);
            }
            else if (document.Accepted == false)
            {
                document.statusId = GetStatusIdWhenDocumentIsNotAccepted(document, retired, inputDocument);
            }
        }

        /// <summary>
        /// If document is not accepted, document status can be either  "Retired or draft"
        /// </summary>
        /// <param name="document"></param>
        /// <param name="retired"></param>
        /// <param name="inputDocument"></param>
        /// <returns></returns>
        private string GetStatusIdWhenDocumentIsNotAccepted(Document document, bool retired, Document inputDocument)
        {
            if (document.statusId == "Valid" || document.statusId == "Sosi-valid")
            {
                GetNewCurrentVersion(document);
            }

            if (retired)
            {
                document.statusId = "Retired";
                document.DateRetired = DateTime.Now;
            }
            else
            {
                document.statusId = "Draft";
                document.dateNotAccepted = GetDateNotAccepted(document, inputDocument);
            }
            return document.statusId;
        }

        /// <summary>
        /// If date not accepted is not selected, set date time now.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="inputDocument"></param>
        /// <returns></returns>
        private DateTime? GetDateNotAccepted(Document document, Document inputDocument)
        {
            if (string.IsNullOrWhiteSpace(inputDocument.dateNotAccepted.ToString()))
            {
                return DateTime.Now;
            }
            else
            {
                return document.dateNotAccepted;
            }
        }

        /// <summary>
        /// If document is accepted, document status can be either  "Retired", "Valid" or "Superseded"
        /// </summary>
        /// <param name="document"></param>
        /// <param name="inputDocument"></param>
        /// <param name="retired"></param>
        /// <param name="sosi"></param>
        /// <returns></returns>
        private Document SetStatusIdWhenDocumentIsAccepted(Document document, Document inputDocument, bool retired, bool sosi)
        {
            Document currentVersion = (Document)_registerItemService.GetRegisterItemBySystemId(document.versioning.currentVersion);
            if (retired == true)
            {
                document.statusId = "Retired";
                document.DateRetired = GetDateRetired(inputDocument.DateRetired);
                document.dateAccepted = DateAccepted(document.dateAccepted, inputDocument.dateAccepted);
            }
            else {
                if (document.statusId == "Submitted" || document.statusId == "Draft")
                {
                    document.dateAccepted = DateAccepted(document.dateAccepted, inputDocument.dateAccepted);
                    if (OtherVersions(document))
                    {
                        document.statusId = CheckIfCurrentVersionIsValid(document, currentVersion, sosi);
                    }
                    else
                    {
                        document.statusId = SetDocumentStatusToValid(document, sosi);
                    }
                }
                else {
                    document.statusId = SetDocumentStatusToValid(document, sosi);
                }
                db.SaveChanges();

                if (DateAcceptedIsChanged(document, inputDocument))
                {
                    document.statusId = CheckIfItIsANewerDateAcceptedAmongSupersededVersions(document, inputDocument, currentVersion);
                }
            }
            return document;
        }

        /// <summary>
        /// If date retired is not selected, set date time now.
        /// </summary>
        /// <param name="dateRetired"></param>
        /// <returns></returns>
        private DateTime? GetDateRetired(DateTime? dateRetired)
        {
            if (dateRetired == null)
            {
                return DateTime.Now;
            }
            else {
                return dateRetired;
            }
        }

        /// <summary>
        /// If current version is valid, date accepted decides if status should be set as "Valid" or "Superseded". 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="currentVersion"></param>
        /// <param name="sosi"></param>
        /// <returns></returns>
        private string CheckIfCurrentVersionIsValid(Document document, Document currentVersion, bool sosi)
        {
            if (currentVersion.statusId == "Valid" || currentVersion.statusId == "Sosi-valid")
            {
                if (DocumentDateAcceptedAreLatest(document.dateAccepted, currentVersion.dateAccepted))
                {
                    return SetDocumentStatusToValid(document, sosi, currentVersion);
                }
                else
                {
                    return SetDocumentStatusToSuperseded(document);
                }
            }
            else {
                return SetDocumentStatusToValid(document, sosi);
            }
        }

        /// <summary>
        /// If it is a newer Date accepted among superseded versions, statusId should be set as "Superseded". Else StatusId should be "Valid"
        /// </summary>
        /// <param name="document"></param>
        /// <param name="inputDocoment"></param>
        /// <param name="currentVersion"></param>
        /// <returns></returns>
        private string CheckIfItIsANewerDateAcceptedAmongSupersededVersions(Document document, Document inputDocoment, Document currentVersion)
        {
            //document.dateAccepted = inputDocoment.dateAccepted;
            var allVersions = _registerItemService.GetAllVersionsOfItembyVersioningId(document.versioning.systemId);

            foreach (Document item in allVersions)
            {
                if (item.statusId == "Superseded")
                {
                    if (item.dateAccepted > inputDocoment.dateAccepted)
                    {
                        item.statusId = "Valid";
                        item.dateSuperseded = null;
                        item.modified = DateTime.Now;
                        document.versioning.currentVersion = item.systemId;
                        document.statusId = SetDocumentStatusToSuperseded(document);
                    }
                }
            }
            return document.statusId;
        }

        private static bool DateAcceptedIsChanged(Document document, Document inputDocument)
        {
            return document.dateAccepted != null && (inputDocument.dateAccepted != document.dateAccepted);
        }

        /// <summary>
        /// Set document status to "Superseded"
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private string SetDocumentStatusToSuperseded(Document document)
        {
            document.dateSuperseded = DateTime.Now;
            return "Superseded";
        }

        /// <summary>
        /// Set document status to "Valid". If there is an other valid version, change status of version to "Superseded"
        /// </summary>
        /// <param name="document"></param>
        /// <param name="version"></param>
        /// <param name="sosi"></param>
        /// <returns></returns>
        private string SetDocumentStatusToValid(Document document, bool sosi, Document version = null)
        {
            if (version != null)
            {
                version.statusId = SetDocumentStatusToSuperseded(version);
                version.modified = DateTime.Now;
            }
            document.versioning.currentVersion = document.systemId;

            if (sosi)
            {
                return "Sosi-valid";
            }
            return "Valid";
        }

        /// <summary>
        /// Check if date accepted on selected version is later then the current version of the document. 
        /// </summary>
        /// <param name="dateAccepted"></param>
        /// <param name="currentVersionDateAccepted"></param>
        /// <returns></returns>
        private static bool DocumentDateAcceptedAreLatest(DateTime? dateAccepted, DateTime? currentVersionDateAccepted)
        {
            return currentVersionDateAccepted == null || currentVersionDateAccepted < dateAccepted;
        }

        /// <summary>
        /// If date accepted is not selected, set date time now.
        /// </summary>
        /// <param name="dateAccepted"></param>
        /// <param name="inputDateAccepted"></param>
        /// <returns></returns>
        private DateTime? DateAccepted(DateTime? dateAccepted, DateTime? inputDateAccepted)
        {
            if (dateAccepted == null && inputDateAccepted == null)
            {
                return DateTime.Now;
            }
            else {
                return inputDateAccepted;
            }
        }

        /// <summary>
        /// Checks whether there are other versions of the document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private bool OtherVersions(Document document)
        {
            var versions = _registerItemService.GetAllVersionsOfItembyVersioningId(document.versioningId);
            return versions.Count > 1;
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

        /// <summary>
        /// Gets new or return allreadu existing versioningId
        /// </summary>
        /// <param name="document"></param>
        /// <param name="inputVersioningId"></param>
        /// <returns></returns>
        private Guid GetVersioningId(Document document, Guid inputVersioningId)
        {
            if ((document.versioningId == null || document.versioningId == Guid.Empty) && (inputVersioningId == null || inputVersioningId == Guid.Empty))
            {
                return _registerItemService.NewVersioningGroup(document);
            }
            else if (inputVersioningId != Guid.Empty && inputVersioningId != null)
            {
                UpdateLatestVersionNumberInVersioningGroup(inputVersioningId, document.versionNumber);
                return inputVersioningId;
            }
            else
            {
                return document.versioningId;
            }
        }


        private int GetVersionNr(int versionNumber, Document originalDocument, Document currentDocument)
        {
            if (originalDocument == null)
            {
                if (versionNumber == 0)
                {
                    versionNumber = 1;
                }
                else
                {
                    if (currentDocument != null)
                    {
                        versionNumber = db.Documents.Where(v => v.versioningId == currentDocument.versioningId)
                       .Select(n => n.versionNumber).Max() + 1;
                    }
                    else { 
                    versionNumber++;
                    }
                }
            }
            return versionNumber;
        }

        private Guid GetSubmitterId(Guid submitterId)
        {
            if (submitterId == Guid.Empty)
            {
                Organization submitterOrganisasjon = _registerService.GetOrganizationByUserName();
                return submitterOrganisasjon.systemId;
            }
            else {
                return submitterId;
            }
        }

        private Guid GetDocumentOwnerId(Guid documentOwnerId)
        {
            if (documentOwnerId == Guid.Empty)
            {
                Organization submitterOrganisasjon = _registerService.GetOrganizationByUserName();
                return submitterOrganisasjon.systemId;
            }
            else {
                return documentOwnerId;
            }
        }

        /// <summary>
        /// Make SEO friendly name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string DocumentSeoName(string name)
        {
            return RegisterUrls.MakeSeoFriendlyString(name);
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

        private string documentUrl(string url, HttpPostedFileBase documentfile, string documenturl, string documentname, string registername, int versionNr)
        {
            if (documentfile != null)
            {
                string fileName = SaveFileToDisk(documentfile, documentname, registername, versionNr);

                if (System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaRemoteSynchEnabled"] == "false" ? false : true)
                {
                    string syncUrl = new SchemaSynchronizer().Synchronize(documentfile, fileName);
                    if (!string.IsNullOrEmpty(syncUrl))
                        url = syncUrl;
                }
                string documentUrl = url + fileName;
                return documentUrl;
            }
            else if (documenturl != null)
            {
                return documenturl;
            }
            else
            {
                return "ikke angitt";
            }
        }
    }
}
