using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System.IO;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Translation;
using Microsoft.Ajax.Utilities;
using Resources;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class CodelistValuesController : Controller
    {
        private readonly RegisterDbContext _db;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;
        private readonly IAccessControlService _accessControlService;
        private readonly ITranslationService _translationService;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CodelistValuesController(IRegisterItemService registerItemService, IRegisterService registerService, IAccessControlService accessControlService, ITranslationService translationService, RegisterDbContext dbContext)
        {
            _registerItemService = registerItemService;
            _registerService = registerService;
            _accessControlService = accessControlService;
            _translationService = translationService;
            _db = dbContext;
        }


        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/ny/import")]
        [Route("kodeliste/{registername}/ny/import")]
        public ActionResult Import(string registername, string parentregister)
        {
            var register = _registerService.GetRegister(parentregister, registername);
            if (register != null)
            {
                if (_accessControlService.Access(register))
                {
                    ViewbagImport(register);
                    return View();
                }
                throw new HttpException(401, "Access Denied");
            }
            return HttpNotFound("Fant ikke register");
        }


        [HttpPost]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/ny/import")]
        [Route("kodeliste/{registername}/ny/import")]
        [Authorize]
        public ActionResult Import(HttpPostedFileBase csvfile, string registername, string registerowner, string parentregister)
        {
            var register = _registerService.GetRegister(parentregister, registername);
            if (register == null) return HttpNotFound();
            if (_accessControlService.Access(register))
            {
                if (csvfile != null)
                {
                    if (ContentTypeIsCsv(csvfile))
                    {
                        _registerItemService.ImportRegisterItemFromFile(register, csvfile);
                        return Redirect(register.GetObjectUrl());
                    }
                    ModelState.AddModelError("ErrorMessagefile", CodelistValues.ErrorMessagefile);
                }
                ViewbagImport(register);
                return View();
            }
            throw new HttpException(401, "Access Denied");
        }

        private static bool ContentTypeIsCsv(HttpPostedFileBase csvfile)
        {
            return csvfile.ContentType == "text/csv" || csvfile.ContentType == "application/vnd.ms-excel";
        }


        // GET: CodelistValues/Create
        [Authorize]
        [Route("kodeliste/{registername}/ny")]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/ny")]
        public ActionResult Create(string registername, string parentregister)
        {
            var codeListValue = new CodelistValue();
            codeListValue.AddMissingTranslations();
            codeListValue.register = _registerService.GetRegister(parentregister, registername);
            if (codeListValue.register != null)
            {
                if (_accessControlService.Access(codeListValue.register))
                {
                    ViewBag.broaderItemsList = _registerItemService.GetBroaderItems();
                    return View(codeListValue);
                }
            }
            return HttpNotFound("Ingen tilgang");
        }


        // POST: CodelistValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/ny")]
        [Route("kodeliste/{registername}/ny")]
        public ActionResult Create(CodelistValue codelistValue, string registername, string parentregister, string registerowner, List<Guid> narrower, Guid? broader)
        {
            codelistValue.register = _registerService.GetRegister(parentregister, registername);
            if (codelistValue.register != null)
            {
                codelistValue.registerId = codelistValue.register.systemId;
                if (_accessControlService.Access(codelistValue.register))
                {
                    if (!_registerItemService.ItemNameAlredyExist(codelistValue))
                    {
                        ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                        return View(codelistValue);
                    }
                    if (ModelState.IsValid)
                    {
                        codelistValue.submitter = _registerService.GetOrganizationByUserName();
                        codelistValue.InitializeNewCodelistValue();
                        codelistValue.versioningId = _registerItemService.NewVersioningGroup(codelistValue);
                        SetBroaderAndNarrowerItems(codelistValue, narrower, broader);

                        _registerItemService.SaveNewRegisterItem(codelistValue);
                        return Redirect(codelistValue.GetObjectUrl());
                    }
                }
            }
            ViewBag.broaderItemsList = _registerItemService.GetBroaderItems();
            return View(codelistValue);
        }

        private void SetBroaderAndNarrowerItems(CodelistValue codelistValue, List<Guid> narrower, Guid? broader)
        {
            if (narrower != null)
            {
                _registerItemService.SetNarrowerItems(narrower, codelistValue);
            }
            if (broader != null)
            {
                _registerItemService.SetBroaderItem(broader.Value, codelistValue);
            }
        }


        // GET: CodelistValues/Edit/5
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger", Name = "editCodelist")]
        [Route("kodeliste/{registername}/{submitter}/{itemname}/rediger")]
        public ActionResult Edit(string registername, string itemname, string parentregister)
        {
            var codelistValue = (CodelistValue)_registerItemService.GetRegisterItem(parentregister, registername, itemname, null);
            if (codelistValue != null)
            {
                if (_accessControlService.Access(codelistValue))
                {
                    Viewbags(codelistValue);
                    codelistValue.AddMissingTranslations();
                    return View(codelistValue);
                }
                return HttpNotFound("Ingen tilgang");
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }


        // POST: CodelistValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        [Route("kodeliste/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(CodelistValue codelistValue, string itemowner, string registername, string itemname, string parentregister, List<Guid> narrower, Guid? broader, string registerowner)
        {
            var originalCodelistValue = (CodelistValue)_registerItemService.GetRegisterItem(parentregister, registername, itemname, null);
            if (originalCodelistValue != null)
            {
                if (_accessControlService.Access(originalCodelistValue))
                {
                    if (ModelState.IsValid)
                    {
                        InitialisationCodelistValue(codelistValue, narrower, broader, originalCodelistValue);
                        if (!NameIsValid(codelistValue))
                        {
                            ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                            Viewbags(originalCodelistValue);
                            return View(originalCodelistValue);
                        }
                        _registerItemService.SaveEditedRegisterItem(originalCodelistValue);
                        return Redirect(RegisterUrls.DeatilsRegisterItemUrl(parentregister, registerowner, registername, itemowner, originalCodelistValue.seoname));
                    }
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            Viewbags(originalCodelistValue);
            return View(originalCodelistValue);
        }


        // GET: CodelistValues/Delete/5
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett")]
        [Route("kodeliste/{registername}/{submitter}/{itemname}/slett")]
        public ActionResult Delete(string registername, string itemname, string parentregister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResults = from o in _db.CodelistValues
                               where o.seoname == itemname && o.register.seoname == registername && o.register.parentRegister.seoname == parentregister
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodelistValue codelistValue = _db.CodelistValues.Find(systId);
            if (codelistValue == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin" || ((role == "nd.metadata" || role == "nd.metadata_editor") && codelistValue.register.accessId == 2 && codelistValue.submitter.name.ToLower() == user.ToLower()))
            {
                Viewbags(codelistValue);
                return View(codelistValue);
            }
            return HttpNotFound("Ingen tilgang");
        }


        // POST: CodelistValues/Delete/5
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett")]
        [Route("kodeliste/{registername}/{submitter}/{itemname}/slett")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string registername, string itemname, string itemowner, string parentregister)
        {
            var queryResults = from o in _db.CodelistValues
                               where o.seoname == itemname && o.register.seoname == registername && o.register.parentRegister.seoname == parentregister
                               select o.systemId;

            Guid systId = queryResults.FirstOrDefault();

            CodelistValue codelistValue = _db.CodelistValues.Find(systId);
            string parent = null;
            if (codelistValue.register.parentRegisterId != null)
            {
                parent = codelistValue.register.parentRegister.seoname;
            }

            _registerItemService.RemoveBroaderAndNarrower(codelistValue);
            codelistValue.broaderItemId = null;
            codelistValue.narrowerItems.Clear();


            _db.Entry(codelistValue).State = EntityState.Modified;
            _db.SaveChanges();

            _db.RegisterItems.Remove(codelistValue);
            _db.SaveChanges();
            if (parent != null)
            {
                return Redirect("/subregister/" + parentregister + "/" + itemowner + "/" + registername);
            }
            return Redirect("/register/" + registername);
        }



        // *** HJELPEMETODER

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
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

        private void Viewbags(CodelistValue codelistValue)
        {
            ViewBag.statusId = _registerItemService.GetStatusSelectList(codelistValue);
            ViewBag.submitterId = _registerItemService.GetSubmitterSelectList(codelistValue.submitterId);
            ViewBag.broaderItemsList = _registerItemService.GetBroaderItems(codelistValue.broaderItemId);
        }

        private void ViewbagImport(Models.Register register)
        {
            ViewBag.registerName = register.name;
            ViewBag.registerSeoName = register.seoname;

            if (register.parentRegisterId != null)
            {
                ViewBag.parentRegister = register.parentRegister.name;
                ViewBag.parentRegisterSeoName = register.parentRegister.seoname;
                ViewBag.parentRegisterOwner = register.parentRegister.owner.seoname;
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }


        private bool NameIsValid(CodelistValue codelistValue)
        {
            return _registerItemService.ItemNameAlredyExist(codelistValue);
        }

        private void InitialisationCodelistValue(CodelistValue codelistValue, List<Guid> narrower, Guid? broader, CodelistValue originalCodelistValue)
        {
            originalCodelistValue.Update(codelistValue);
            
            if (UpdateNarrowerItems(narrower, originalCodelistValue))
            {
                _registerItemService.SetNarrowerItems(narrower, originalCodelistValue);
            }

            if (UpdateBroaderItem(broader, originalCodelistValue))
            {
                SetBroaderItem(broader, originalCodelistValue);
            }

            _translationService.UpdateTranslations(codelistValue, originalCodelistValue);
        }

        private void SetBroaderItem(Guid? broader, CodelistValue originalCodelistValue)
        {
            if (broader == null)
            {
                _registerItemService.SetBroaderItem(originalCodelistValue);
            }
            else
            {
                _registerItemService.SetBroaderItem(broader.Value, originalCodelistValue);
            }
        }

        private static bool UpdateBroaderItem(Guid? broader, CodelistValue originalCodelistValue)
        {
            return broader != null || originalCodelistValue.broaderItemId != null;
        }

        private static bool UpdateNarrowerItems(List<Guid> narrower, CodelistValue originalCodelistValue)
        {
            return (originalCodelistValue.narrowerItems != null && originalCodelistValue.narrowerItems.Count() != 0) || narrower != null;
        }
    }
}
