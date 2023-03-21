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
using Kartverket.Register.Services.Notify;
using Kartverket.Register.Services.Translation;
using Kartverket.Geonorge.Utilities.LogEntry;
using System.Web.Configuration;
using Ionic.Zip;
using Ghostscript.NET.Rasterizer;
using System.Drawing.Imaging;
using System.Collections.Generic;
//ghostscriptsharp MIT license:
//Copyright(c) 2009 Matthew Ephraim
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

//ghostscript GNU Affero General Public License:
//<Register for documents, generate thumbnail>
//Copyright (C) 2022, Norwegian Mapping Authority
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Affero General Public License as
//published by the Free Software Foundation, either version 3 of the
//License, or (at your option) any later version.
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Affero General Public License for more details.
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see https://www.gnu.org/licenses/.


namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class DocumentsController : Controller
    {
        private readonly RegisterDbContext db;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        private IDocumentService _documentService;
        private IAccessControlService _accessControlService;
        private INotificationService _notificationService;
        private ITranslationService _translationService;
        private ILogEntryService _logEntryService;
        public ILogEntryService LogEntryService
        {
            get
            {
                if (_logEntryService == null)
                    _logEntryService = new LogEntryService(WebConfigurationManager.AppSettings["LogApi"], WebConfigurationManager.AppSettings["LogApiKey"], new Kartverket.Geonorge.Utilities.Organization.HttpClientFactory());

                return _logEntryService;
            }
            set { _logEntryService = value; }
        }
        string ErrorMessageIllegalSchemaLocation = "Beklager, skjemaet ble ikke lastet opp! Ugyldig skjemaplassering. Sjekk targetNamespace i XSD. Skal starte med «http(s)//skjema.geonorge.no» eller «http(s)://skjema.test.geonorge.no»";

        public DocumentsController(RegisterDbContext dbContext, INotificationService notificationService, ITranslationService translationService, IRegisterService registerService, IRegisterItemService registerItemService, IAccessControlService accessControlService, IDocumentService documentService)
        {
            db = dbContext;
            _registerItemService = registerItemService;
            _registerService = registerService;
            _accessControlService = accessControlService;
            _notificationService = notificationService;
            _translationService = translationService;
            _documentService = documentService;
        }


        // GET: Documents/Create
        [Authorize]
        //[Route("dokument/{parentRegister}/{registerowner}/{registername}/ny")]
        //[Route("dokument/{registername}/ny")]
        public ActionResult Create(string systemid)
        {
            Document document = new Document();
            document.AddMissingTranslations();
            document.register = _registerService.GetRegisterBySystemId(Guid.Parse(systemid));
            Viewbags(document);
            if (document.register != null)
            {
                if (_accessControlService.AddToRegister(document.register))
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
        //[Route("dokument/{parentRegister}/{registerowner}/{registername}/ny")]
        //[Route("dokument/{registername}/ny")]
        public ActionResult Create(Document document, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, string registername, string parentRegister, string registerId, HttpPostedFileBase schematronfile = null, string zipIsAsciiDoc = null, HttpPostedFileBase documentfileEnglish = null, string zipIsAsciiDocEnglish = null)
        {
            document.register = _registerService.GetRegisterBySystemId(Guid.Parse(registerId));
            if (_accessControlService.AddToRegister(document.register))
            {
                if (!NameIsValid(document))
                {
                    ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                }
                else if (ModelState.IsValid)
                {
                    document = initialisationDocument(document, documentfile, thumbnail, false, false, null, schematronfile, zipIsAsciiDoc, documentfileEnglish, zipIsAsciiDocEnglish);
                    if (ModelState.IsValid)
                        return Redirect(document.GetObjectUrl());
                }
                Viewbags(document);
            }
            return View(document);
        }


        // GET: Documents/CreateNewVersion
        [Authorize]
        //[Route("dokument/versjon/{parentRegister}/{parentRegisterOwner}/{registername}/{itemOwner}/{itemname}/ny")]
        //[Route("dokument/versjon/{registername}/{itemOwner}/{itemname}/ny")]
        public ActionResult CreateNewVersion(string parentRegister, string registername, string itemname)
        {
            Document document = (Document)_registerItemService.GetCurrentRegisterItem(parentRegister, registername, itemname);
            if (document != null)
            {
                if (_accessControlService.AddToRegister(document.register))
                {
                    Viewbags(document);
                    return View(document);
                }
                else
                {
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
        //[Route("dokument/versjon/{parentRegister}/{parentRegisterOwner}/{registername}/{itemOwner}/{itemname}/ny")]
        //[Route("dokument/versjon/{registername}/{itemOwner}/{itemname}/ny")]
        public ActionResult CreateNewVersion(Document document, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, string parentRegisterOwner, string parentRegister, string registername, string itemname, HttpPostedFileBase schematronfile = null, string zipIsAsciiDoc = null, HttpPostedFileBase documentfileEnglish = null, string zipIsAsciiDocEnglish = null)
        {
            document.register = _registerService.GetSubregisterByName(parentRegister, registername);
            if (_accessControlService.AddToRegister(document.register))
            {
                if (!NameIsValid(document))
                {
                    ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                }
                else if (ModelState.IsValid)
                {
                    document = initialisationDocument(document, documentfile, thumbnail, false, false, null, schematronfile, zipIsAsciiDoc, documentfileEnglish, zipIsAsciiDocEnglish);
                    if (ModelState.IsValid)
                        return Redirect(document.GetObjectUrl());
                }
            }
            return View(document);
        }


        // GET: Documents/Edit/5
        [Authorize]
        //[Route("dokument/{parentregister}/{registerowner}/{registername}/{itemowner}/{documentname}/rediger")]
        //[Route("dokument/{registername}/{itemowner}/{documentname}/rediger")]
        public ActionResult Edit(string parentregister, string registername, string documentname, int vnr)
        {
            var document = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, vnr);
            if (document != null)
            {
                if (_accessControlService.HasAccessTo(document))
                {
                    document.AddMissingTranslations();
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
        //[Route("dokument/{parentregister}/{registerowner}/{registername}/{itemowner}/{documentname}/rediger")]
        //[Route("dokument/{registername}/{itemowner}/{documentname}/rediger")]
        public ActionResult Edit(Document document, string parentregister, string registerowner, string registername, string itemowner, string documentname, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, bool retired, bool sosi, HttpPostedFileBase schematronfile = null, string zipIsAsciiDoc = null, HttpPostedFileBase documentfileEnglish = null, string zipIsAsciiDocEnglish = null)
        {
            var originalDocument = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, document.versionNumber);
            if (originalDocument != null)
            {
                if (!_registerItemService.ItemNameIsValid(document))
                {
                    ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                }
                else if (ModelState.IsValid)
                {
                    document.documentUrl = originalDocument.documentUrl;
                    document.documentUrlEnglish = originalDocument.documentUrlEnglish;

                    string url = GetUrlForRegister(originalDocument, document);
                    DocumentFile documentFile = documentUrl(url, documentfile, document.documentUrl, document.name, originalDocument.register.name, document.versionNumber, document.Accepted?.ToString(), originalDocument?.Accepted.ToString(), originalDocument, document, schematronfile, zipIsAsciiDoc, documentfileEnglish, zipIsAsciiDocEnglish);
                    if (!string.IsNullOrEmpty(documentFile.Url))
                        document.documentUrl = documentFile.Url;
                    if (!string.IsNullOrEmpty(documentFile.Filename2AsciiDoc))
                        document.documentUrl2 = documentFile.Filename2AsciiDoc;
                    if (!string.IsNullOrEmpty(documentFile.UrlEnglish))
                        document.documentUrlEnglish = documentFile.UrlEnglish;
                    document.documentUrlSchematron = documentFile.UrlSchematron;
                    if (document.documentUrl == "IllegalSchemaLocation")
                    {
                        ModelState.AddModelError("ErrorMessageFileName", ErrorMessageIllegalSchemaLocation);
                        document.documentUrl = "";
                    }
                    else {
                        if (document.register == null)
                            document.register = originalDocument.register;
                        document.thumbnail = GetThumbnail(document, originalDocument, documentfile, url, thumbnail);
                        originalDocument = _documentService.UpdateDocument(originalDocument, document, documentfile, thumbnail, retired, sosi);


                        //document = initialisationDocument(document, documentfile, thumbnail, retired, sosi, originalDocument);
                        return Redirect(originalDocument.GetObjectUrl());
                    }
                }
            }
            Viewbags(document);
            return View(originalDocument);
        }


        // GET: Documents/Delete/5
        [Authorize]
        //[Route("dokument/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{documentname}/slett")]
        //[Route("dokument/{registername}/{organization}/{documentname}/slett")]
        public ActionResult Delete(string registername, string documentname, int? vnr, string parentregister, string parentregisterowner)
        {
            Document document = (Document)_registerItemService.GetRegisterItem(parentregister, registername, documentname, vnr.Value);
            if (document != null)
            {
                if (_accessControlService.HasAccessTo(document))
                {
                    Viewbags(document);
                    return View(document);
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound();
        }


        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        //[Route("dokument/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{documentname}/slett")]
        //[Route("dokument/{registername}/{organization}/{documentname}/slett")]
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
                    RemoveLinks(document);
                    db.RegisterItems.Remove(document);
                }
                else
                {
                    RemoveLinks(document);
                    db.RegisterItems.Remove(document);
                    db.SaveChanges();
                    db.Versions.Remove(versjonsgruppe);
                }
            }
            else
            {
                RemoveLinks(document);
                db.RegisterItems.Remove(document);
            }
            db.SaveChanges();

            RemoveFiles(document);

            return Redirect(registerUrl);
        }

        private static void RemoveLinks(Document document)
        {
            var documentUrlAttachments = document.DocumentUrlAttachments.ToList();
            foreach (var attachment in documentUrlAttachments)
            {
                document.DocumentUrlAttachments.Remove(attachment);
            }
        }

        [Authorize]
        public ActionResult Logg(string systemid, int limit = 50, bool displayAllElements = false)
        {
            var document = (Document)_registerItemService.GetRegisterItemBySystemId(Guid.Parse(systemid));
            if (document == null)
                return HttpNotFound();

            string elementid = "";

            if (!displayAllElements)
            {
                elementid = document.documentUrl?.Split('/').Last();
            }

            var log = _logEntryService.GetGMLApplicationSchemasAsync(limit, elementid).Result;
            ViewBag.LogEntries = log;
            Viewbags(document);

            return View(document);
        }

        private void RemoveFiles(Document document)
        {
            if (document != null && !string.IsNullOrEmpty(document.documentUrl) && document.documentUrl.EndsWith(".xsd"))
            {
                try
                {
                    new SchemaSynchronizer().RemoveFiles(document);
                }
                catch (Exception ex)
                {
                    Log.Error("Error deleting document file: ", ex);
                }
            }
            try
            {
                var url = new Uri(document.documentUrl);
                var filePath = url.LocalPath;
                var directory = Constants.DataDirectory + Document.DataDirectory + filePath;
                if (url.Host.Contains("standarder"))
                    directory = Constants.DataDirectory + Document.DataDirectory + "standarder/" + filePath;

                if (directory.EndsWith(".html"))
                {
                    directory = Path.GetDirectoryName(directory);
                    var path = Server.MapPath(directory);
                    Directory.Delete(path, true);
                }
                else
                {
                    var path = Server.MapPath(directory);
                    System.IO.File.Delete(path);
                }

                if (!string.IsNullOrEmpty(document.documentUrlEnglish))
                {
                    url = new Uri(document.documentUrlEnglish);
                    filePath = url.LocalPath;
                    directory = Constants.DataDirectory + Document.DataDirectory + filePath;
                    if (url.Host.Contains("standarder"))
                        directory = Constants.DataDirectory + Document.DataDirectory + "standarder/" + filePath;

                    if (directory.EndsWith(".html"))
                    {
                        directory = Path.GetDirectoryName(directory);
                        var path = Server.MapPath(directory);
                        Directory.Delete(path, true);
                    }
                    else
                    {
                        var path = Server.MapPath(directory);
                        System.IO.File.Delete(path);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error deleting document file: ", ex);
            }


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
            return _registerItemService.ItemNameIsValid(document);
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
        private string GenerateThumbnail(Document document, Document originalDocument, HttpPostedFileBase documentfile, string url)
        {
            if (document.documentUrl.Contains(".pdf"))
            {
                string filtype, name;
                string seofilename = "";
                if(documentfile.ContentType == "application/x-zip-compressed") 
                {
                    documentfile.InputStream.Position = 0;
                    using (ZipFile zip = ZipFile.Read(documentfile.InputStream))
                    {
                        seofilename = zip.EntryFileNames.Where(f => f.EndsWith(".pdf")).FirstOrDefault();
                    }
                }
                else 
                {
                    seofilename = MakeSeoFriendlyDocumentName(documentfile, out filtype, out seofilename) + Path.GetExtension(documentfile.FileName);
                }
                name = RegisterUrls.MakeSeoFriendlyString(document.name);

                var originalPath = document != null && document.register != null ? document.register.path : originalDocument.register.path;
                var registerPath = originalPath + "/" + name + "/" + document.versionName;
                var directory = Constants.DataDirectory + Document.DataDirectory + registerPath;
                var path = Server.MapPath(directory);
                //System.IO.Directory.CreateDirectory(path);

                path = Path.Combine(path, seofilename);

                string input = path;
                string thumbFileName = seofilename.Replace(".pdf", ".jpg");
                string output = path.Replace(".pdf", ".jpg");
                //GhostscriptWrapper.GenerateOutput(input, output, GsSettings());
                var rasterizer = new GhostscriptRasterizer();
                var fileStream = System.IO.File.OpenRead(input);
                rasterizer.Open(fileStream); //opens the PDF file for rasterizing
                var pdf2PNG = rasterizer.GetPage(100, 1);
                pdf2PNG.Save(output, ImageFormat.Png);
                pdf2PNG.Dispose();
                rasterizer.Close();
                fileStream.Close();

                ImageResizer.ImageJob newImage =
                                new ImageResizer.ImageJob(output, output,
                                new ImageResizer.Instructions("maxwidth=160;maxheight=300;quality=75"));

                newImage.Build();

                //remove standarder from path
                if (registerPath.StartsWith("standarder/"))
                    registerPath = registerPath.Replace("standarder/", "");
                return url + registerPath + "/" + thumbFileName;

            }
            else if (document.documentUrl.Contains(".xsd"))
            {
                return "/Content/xsd.svg";
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

        private DocumentFile SaveFileToDisk(HttpPostedFileBase file, string name, string register, int vnr,
            Document originalDocument, Document document, string zipIsAsciiDoc = null, HttpPostedFileBase fileEnglish = null, string zipIsAsciiDocEnglish = null)
        {

            DocumentFile documentFile = new DocumentFile();

            string filtype;
            string seofilename = null;

            string seofilenameToAsciiDoc = null;

            if (file != null)
                seofilename = MakeSeoFriendlyDocumentName(file, out filtype, out seofilename) + Path.GetExtension(file.FileName);

            string filtypeEnglish;
            string seofilenameEnglish = null;

            if (fileEnglish != null)
                seofilenameEnglish = MakeSeoFriendlyDocumentName(fileEnglish, out filtypeEnglish, out seofilenameEnglish) + Path.GetExtension(fileEnglish.FileName);

            name = RegisterUrls.MakeSeoFriendlyString(name);

            var originalPath = document != null && document.register != null ? document.register.path : originalDocument.register.path;
            var registerPath = originalPath + "/" + name + "/" + document.versionName;
            var directory = Constants.DataDirectory + Document.DataDirectory + registerPath;
            var path = Server.MapPath(directory);
            System.IO.Directory.CreateDirectory(path);

            if (file != null && !string.IsNullOrEmpty(zipIsAsciiDoc) && Path.GetExtension(file.FileName) == ".zip")
            {
                using (ZipFile zip = ZipFile.Read(file.InputStream))
                {
                    seofilename = zip.EntryFileNames.Where(f => f.EndsWith(".html")).FirstOrDefault();

                    var pdf = zip.EntryFileNames.Where(f => f.EndsWith(".pdf")).FirstOrDefault();

                    if (!string.IsNullOrEmpty(pdf)) {
                        seofilenameToAsciiDoc = seofilename;
                        seofilename = pdf;
                    }
                    try { 
                    zip.ExtractAll(path);
                    }
                    catch (Exception exx) { Log.Error(exx); }
                }
            }
            else if (!string.IsNullOrEmpty(zipIsAsciiDocEnglish) && Path.GetExtension(fileEnglish.FileName) == ".zip")
            {
                using (ZipFile zip = ZipFile.Read(fileEnglish.InputStream))
                {
                    seofilenameEnglish = zip.EntryFileNames.Where(f => f.EndsWith(".html")).FirstOrDefault();

                    zip.ExtractAll(path);
                }
            }
            else
            {
                var pathOriginal = path;
                if (!string.IsNullOrEmpty(seofilename))
                {
                    //if(seofilename.StartsWith("pdf/"))
                    //    System.IO.Directory.CreateDirectory(path + "\\pdf");

                    path = Path.Combine(path, seofilename);
                    file.SaveAs(path);
                }

                if (!string.IsNullOrEmpty(seofilenameEnglish))
                {
                    path = Path.Combine(pathOriginal, seofilenameEnglish);
                    fileEnglish.SaveAs(path);
                }
            }

            //remove standarder from path
            if (registerPath.StartsWith("standarder/"))
                registerPath = registerPath.Replace("standarder/", "");

            documentFile.Filename = registerPath + "/" + seofilename;
            if (!string.IsNullOrEmpty(seofilenameEnglish))
                documentFile.FilenameEnglish = registerPath + "/" + seofilenameEnglish;

            if(!string.IsNullOrEmpty(seofilenameToAsciiDoc))
                documentFile.Filename2AsciiDoc = registerPath + "/" + seofilenameToAsciiDoc;

            return documentFile;
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
                seofilename += RegisterUrls.MakeSeoFriendlyString(item);
            }
            return seofilename;
        }

        private void Viewbags(Document document)
        {
            ViewBag.statusId = _registerItemService.GetStatusSelectList(document);
            ViewBag.submitterId = _registerItemService.GetSubmitterSelectList(document.submitterId);
            if (document.documentownerId == Guid.Empty)
                document.documentownerId = GetDocumentOwnerId(document.documentownerId);
            ViewBag.documentownerId = _registerItemService.GetOwnerSelectList(document.documentownerId);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }


        private Document initialisationDocument(Document inputDocument, HttpPostedFileBase documentfile, HttpPostedFileBase thumbnail, bool retired = false, bool sosi = false, Document originalDocument = null, HttpPostedFileBase schematronfile = null, string zipIsAsciiDoc = null, HttpPostedFileBase documentfileEnglish = null, string zipIsAsciiDocEnglish = null)
        {
            Document document = GetDocument(originalDocument);
            document.systemId = GetSystemId(document.systemId);
            document.name = DocumentName(inputDocument.name);
            document.seoname = DocumentSeoName(inputDocument.name);
            document.modified = DateTime.Now;
            document.description = inputDocument.description;
            document.approvalDocument = inputDocument.approvalDocument;
            document.approvalReference = inputDocument.approvalReference;
            document.UmlModelTreeStructureLink = inputDocument.UmlModelTreeStructureLink;
            document.ApplicationSchema = inputDocument.ApplicationSchema;
            document.GMLApplicationSchema = inputDocument.GMLApplicationSchema;
            document.CartographyFile = inputDocument.CartographyFile;
            document.CartographyDetailsUrl = inputDocument.CartographyDetailsUrl;
            document.versionName = inputDocument.versionName;
            document.versionNumber = GetVersionNr(inputDocument.versionNumber, originalDocument, inputDocument);
            document.registerId = GetRegisterId(inputDocument, document);
            document.Accepted = inputDocument.Accepted;
            document.documentownerId = GetDocumentOwnerId(inputDocument.documentownerId);
            if (document.documentownerId != Guid.Empty)
                document.documentowner = db.Organizations.Where(o => o.systemId == document.documentownerId).FirstOrDefault();
            document.submitterId = GetSubmitterId(inputDocument.submitterId);
            string url = GetUrlForRegister(document, originalDocument);
            DocumentFile documentFiles = documentUrl(url, documentfile, document.documentUrl, document.name, document.register.name, document.versionNumber, document?.status?.value, originalDocument?.status?.value, originalDocument, document, schematronfile, zipIsAsciiDoc, documentfileEnglish, zipIsAsciiDocEnglish);
            document.documentUrl = documentFiles.Url;
            document.documentUrlEnglish = documentFiles.UrlEnglish;
            document.documentUrlSchematron = documentFiles.UrlSchematron;
            if (document.documentUrl == "IllegalSchemaLocation")
            {
                ModelState.AddModelError("ErrorMessageFileName", ErrorMessageIllegalSchemaLocation);
                document.documentUrl = "";
                return document;
            }
            if (!string.IsNullOrEmpty(documentFiles.Filename2AsciiDoc)) 
            { 
                document.documentUrl2 = documentFiles.Filename2AsciiDoc;
            }
            else
            {
                document.documentUrl2 = inputDocument.documentUrl2;
            }
            document.thumbnail = GetThumbnail(document, originalDocument, documentfile, url, thumbnail);
            document.versioningId = GetVersioningId(document, inputDocument.versioningId);

            bool sendNotification = false;

            if (originalDocument == null)
            {
                document.dateSubmitted = DateTime.Now;
                if(document.documentownerId != Guid.Empty)
                    document.documentowner = db.Organizations.Where(o => o.systemId == document.documentownerId).FirstOrDefault();
                document.statusId = "Submitted";
                db.Entry(document).State = EntityState.Modified;
                db.RegisterItems.Add(document);
                if (!_accessControlService.IsAdmin())
                    sendNotification = true;
            }
            else
            {
                ApprovalProcess(document, retired, inputDocument, sosi);
                db.Entry(document).State = EntityState.Modified;
            }

            _translationService.UpdateTranslations(inputDocument, document);

            db.SaveChanges();

            document.DocumentUrlAttachments = new List<Link>();

            foreach (var link in inputDocument.DocumentUrlAttachments)
            {
                if (!string.IsNullOrEmpty(link.Url)) 
                { 
                    var attachment = new Link { Url = link.Url, Text = link.Text };
                    document.DocumentUrlAttachments.Add(attachment);
                    db.Entry(attachment).State = EntityState.Added;
                    db.SaveChanges();
                }
            }

            if (sendNotification)
                _notificationService.SendSubmittedNotification(document);

            return document;
        }

        private string GetUrlForRegister(Document originalDocument, Document document)
        {
            if(document != null && document.register != null) 
            { 
                if (document.IsStandard())
                    return WebConfigurationManager.AppSettings["StandardsUrl"];
                else
                    return WebConfigurationManager.AppSettings["DocumentsUrl"];
            }
            else
            {
                if (originalDocument.IsStandard())
                    return WebConfigurationManager.AppSettings["StandardsUrl"];
                else
                    return WebConfigurationManager.AppSettings["DocumentsUrl"];
            }
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
            else
            {
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
            else
            {
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
            else
            {
                return new Document();
            }
        }

        private void ApprovalProcess(Document document, bool retired, Document inputDocument, bool sosi)
        {
            if (document.Accepted == true)
            {
                SetStatusIdWhenDocumentIsAccepted(document, inputDocument, retired, sosi);
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
            else
            {
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
                else
                {
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
            else
            {
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
            else
            {
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
            else
            {
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

        private string GetThumbnail(Document document, Document originalDocument, HttpPostedFileBase documentfile, string url, HttpPostedFileBase thumbnail)
        {
            if (documentfile != null)
            {
                document.thumbnail = GenerateThumbnail(document,originalDocument, documentfile, url);
            }

            if (thumbnail != null /*&& document.thumbnail.Contains(thumbnail.FileName)*/)
            {
                var file = SaveFileToDisk(thumbnail, document.name, document.register.seoname, document.versionNumber, originalDocument, document);
                document.thumbnail = url + file.Filename;

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
                    else
                    {
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
            else
            {
                return submitterId;
            }
        }

        private Guid GetDocumentOwnerId(Guid documentOwnerId)
        {
            if (documentOwnerId == Guid.Empty)
            {
                Organization submitterOrganisasjon = _registerService.GetOrganizationByUserName();
                return submitterOrganisasjon != null ? submitterOrganisasjon.systemId : Organization.GetDefaultOrganizationId();
            }
            else
            {
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

        private DocumentFile documentUrl(string url, HttpPostedFileBase documentfile, string documenturl, string documentname, string registername, int versionNr, string status, string previousStatus, Document originalDocument, Document document,  HttpPostedFileBase schematronfile = null, string zipIsAsciiDoc = null, HttpPostedFileBase documentfileEnglish = null, string zipIsAsciiDocEnglish = null)
        {
            DocumentFile documentUrl = new DocumentFile();

            if (documentfile != null || documentfileEnglish != null)
            {
                var schemaRemoteSynchEnabled = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaRemoteSynchEnabled"] == "false" ? false : true;

                if (registername == "GML applikasjonsskjema" && schemaRemoteSynchEnabled)
                {
                    documentUrl = new SchemaSynchronizer().Synchronize(documentfile, schematronfile);
                    return documentUrl;
                }
                else 
                {
                    var documentFile = SaveFileToDisk(documentfile, documentname, registername, versionNr, originalDocument, document, zipIsAsciiDoc, documentfileEnglish, zipIsAsciiDocEnglish);
                    if(!string.IsNullOrEmpty(documentFile.Filename))
                        documentUrl.Url = url + documentFile.Filename;
                    if (!string.IsNullOrEmpty(documentFile.Filename2AsciiDoc))
                        documentUrl.Filename2AsciiDoc = url + documentFile.Filename2AsciiDoc;
                    if (!string.IsNullOrEmpty(documentFile.FilenameEnglish))
                        documentUrl.UrlEnglish = url + documentFile.FilenameEnglish;
                    return documentUrl;
                }
            }
            else if (documenturl != null)
            {
                var schemaRemoteSynchEnabled = System.Web.Configuration.WebConfigurationManager.AppSettings["SchemaRemoteSynchEnabled"] == "false" ? false : true;
                if (registername == "GML applikasjonsskjema" && schemaRemoteSynchEnabled)
                {
                    if(previousStatus != status)
                    { 
                        if (status == "True")
                            documenturl =  new SchemaSynchronizer().Synchronize(documenturl);
                    }
                }

                documentUrl.Url = documenturl;
                return documentUrl;
            }
            else
            {
                documentUrl.Url = "ikke angitt";
                return documentUrl;
            }
        }
    }
}
