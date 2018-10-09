using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Search;
using Kartverket.Register.Services.Translation;
using Resources;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class SubregisterController : Controller
    {
        private readonly RegisterDbContext _db;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IRegisterService _registerService;
        private readonly ITranslationService _translationService;
        private readonly IRegisterItemService _registerItemService;
        private readonly IAccessControlService _accessControlService;

        public SubregisterController(ITranslationService translationService, RegisterDbContext dbContex, IRegisterItemService registerItemService, IRegisterService registerService, IAccessControlService accessControlService)
        {
            _db = dbContex;
            _registerService = registerService;
            _registerItemService = registerItemService;
            _accessControlService = accessControlService;
            _translationService = translationService;
        }


        // GET: subregister/Create
        [Authorize]
        //[Route("subregister/{registerparant}/{parentRegisterOwner}/{registername}/ny")]
        //[Route("subregister/{registername}/ny")]
        public ActionResult Create(string registerparant, string parentRegisterOwner, string registername)
        {
            var register = new Models.Register();
            register.parentRegister = _registerService.GetRegister(registerparant, registername);

            if (_accessControlService.Access(register.parentRegister))
            {
                register.AddMissingTranslations();
                ViewBagSubregister(register);

                return View(register);
            }
            return HttpNotFound();
        }



        // POST: subregister/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        //[Route("subregister/{registerparant}/{parentRegisterOwner}/{registerName}/ny")]
        //[Route("subregister/{registerName}/ny")]
        public ActionResult Create(Models.Register subRegister, string registerName, string registerparant)
        {
            subRegister.parentRegister = _registerService.GetRegister(registerparant, registerName);
            if (_registerService.RegisterNameAlredyExist(subRegister)) ModelState.AddModelError("ErrorMessage", Registers.ErrorMessageValidationName);

            if (ModelState.IsValid)
            {
                var register = _registerService.CreateNewRegister(subRegister);
                return Redirect(register.GetObjectUrl());
            }
            ViewBagSubregister(subRegister);
            return View(subRegister);
        }

        // GET: Subregister/Edit/5
        [Authorize]
        //[Route("subregister/{registername}/{registerOwner}/{subregister}/rediger")]
        public ActionResult Edit(string registername, string subregister)
        {
            var register = _registerService.GetRegister(registername, subregister);
            if (register == null) return HttpNotFound("Fant ikke register");

            Viewbags(register);
            register.AddMissingTranslations();

            return _accessControlService.Access(register) ? View(register) : throw new HttpException(401, "Access Denied");
        }

        // POST: Subregister/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        //[Route("subregister/{registername}/{registerOwner}/{subregister}/rediger")]
        public ActionResult Edit(Models.Register register, string registername, string subregister)
        {
            var originalRegister = _registerService.GetRegister(registername, subregister);
            if (_accessControlService.Access(originalRegister))
            {
                ValidationName(register, registername);

                if (ModelState.IsValid)
                {
                    if (register.name != null) originalRegister.name = register.name;
                    originalRegister.seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(originalRegister.name);
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

                    originalRegister.MakeAllItemsValid = register.MakeAllItemsValid;
                    if (originalRegister.MakeAllItemsValid)
                        _registerItemService.MakeAllRegisterItemsValid(originalRegister);
                    _translationService.UpdateTranslations(register, originalRegister);
                    _db.Entry(originalRegister).State = EntityState.Modified;
                    _db.SaveChanges();
                    Viewbags(register);

                    return Redirect(originalRegister.GetObjectUrl());

                }
            }
            else
            {
                throw new HttpException(401, "Access Denied");
            }

            Viewbags(register);
            return View(originalRegister);
        }

        // GET: Subregister/Delete/5
        [Authorize]
        //[Route("subregister/{registername}/{owner}/{subregister}/slett")]
        public ActionResult Delete(string registername, string owner, string subregister)
        {
            Models.Register register = _registerService.GetRegister(registername, subregister);

            if (register == null)
            {
                return HttpNotFound();
            }

            if (_accessControlService.Access(register))
            {
                return View(register);
            }

            throw new HttpException(401, "Access Denied");
        }

        // POST: Subregister/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        //[Route("subregister/{registername}/{owner}/{subregister}/slett")]
        public ActionResult DeleteConfirmed(string registername, string owner, string subregister)
        {
            var register = _registerService.GetRegister(registername, subregister);
            if (register != null)
            {
                if (!_accessControlService.Access(register))
                {
                    throw new HttpException(401, "Access Denied");
                }

                var parentRegisterUrl = register.parentRegister.GetObjectUrl();

                _registerService.DeleteRegister(register);

                return Redirect(parentRegisterUrl);
            }

            return HttpNotFound();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }



        // *********************** Hjelpemetoder

        private void ViewBagSubregister(Models.Register register)
        {
            // TODO, fiks!!
            if (register.parentRegister != null)
            {
                if (register.parentRegister.name == "Kodelister" || register.parentRegister.name == "Metadata kodelister" ||
                    register.parentRegister.parentRegister?.name == "Kodelister" ||
                    register.parentRegister.parentRegister?.name == "Metadata kodelister")
                {
                    ViewBag.containedItemClass = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem() {Value = "CodelistValue", Text = "Kodeverdier"},
                        new SelectListItem() {Value = "Register", Text = "Subregister"}
                    }, "Value", "Text", String.Empty);
                }
                else
                {
                    ViewBag.containedItemClass = new SelectList(_db.ContainedItemClass.OrderBy(s => s.description),
                        "value", "description", String.Empty);
                }
            }
            ViewBag.register = register.name;
            ViewBag.registerSEO = register.seoname;
        }

        private void ValidationName(Models.Register subRegister, string register)
        {
            var queryResultsDataset = from o in _db.Registers
                                      where o.name == subRegister.name && o.systemId != subRegister.systemId && o.parentRegister.seoname == register
                                      select o.systemId;

            if (queryResultsDataset.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessage", Registers.ErrorMessageValidationName);
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

        private void Viewbags(Models.Register register)
        {
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(_db.Statuses.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }).OrderBy(o => o.description), "value", "description", register.statusId);
            ViewBag.ownerId = new SelectList(_db.Organizations.ToList().Select(s => new { systemId = s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", register.ownerId);
            ViewBag.parentRegisterId = new SelectList(_db.Registers.ToList().Select(s => new { systemId = s.systemId, name = s.NameTranslated(), containedItemClass = s.containedItemClass }).Where(r => r.containedItemClass == "Register" && r.name != register.name).OrderBy(s => s.name), "systemId", "name", register.parentRegisterId);
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
