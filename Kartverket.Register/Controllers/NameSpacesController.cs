using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.RegisterItem;
using Resources;
using Kartverket.Register.Services.Translation;

namespace Kartverket.Register.Controllers
{
    public class NameSpacesController : BaseController
    {
        private readonly RegisterDbContext _db;

        private readonly IRegisterItemService _registerItemService;
        private readonly ITranslationService _translationService;

        public NameSpacesController(RegisterDbContext dbContext, ITranslationService translationService, IRegisterItemService registerItemService)
        {
            _db = dbContext;
            _registerItemService = registerItemService;
            _translationService = translationService;
        }

        // GET: NameSpaces/Create
        [Authorize]
        //[Route("navnerom/{parentRegister}/{registerowner}/{registername}/ny")]
        //[Route("navnerom/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            NameSpace nameSpace = new NameSpace();
            nameSpace.AddMissingTranslations();
            Models.Register register = GetRegister(registername, parentRegister);

            nameSpace.register = register;
            if (register.parentRegisterId != null)
            {
                nameSpace.register.parentRegister = register.parentRegister;
            }

            if (UserHasAccess(nameSpace, "Create"))
            {
                return View(nameSpace);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: NameSpaces/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Route("navnerom/{parentRegister}/{registerowner}/{registername}/ny")]
        //[Route("navnerom/{registername}/ny")]
        public ActionResult Create(NameSpace nameSpace, string registername, string parentRegister)
        {
            nameSpace.register = GetRegister(registername, parentRegister);

            string parentRegisterOwner = null;
            if (nameSpace.register.parentRegisterId != null)
            {
                parentRegisterOwner = nameSpace.register.parentRegister.owner.seoname;
            }

            if (_registerItemService.ItemNameIsValid(nameSpace))
            {
                if (ModelState.IsValid)
                {
                    nameSpace.systemId = Guid.NewGuid();
                    if (nameSpace.name == null)
                        nameSpace.name = "ikke angitt";
                    nameSpace.systemId = Guid.NewGuid();
                    nameSpace.modified = DateTime.Now;
                    nameSpace.dateSubmitted = DateTime.Now;
                    nameSpace.registerId = nameSpace.register.systemId;
                    nameSpace.statusId = "Submitted";
                    nameSpace.seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(nameSpace.name);
                    nameSpace.versionNumber = 1;
                    nameSpace.versioningId = _registerItemService.NewVersioningGroup(nameSpace);

                    Organization organization = GetSubmitter();
                    nameSpace.submitterId = organization.systemId;

                    _db.RegisterItems.Add(nameSpace);
                    _db.SaveChanges();


                    return Redirect(nameSpace.GetObjectUrl());


                }
            }

            return View(nameSpace);
        }

        // GET: NameSpaces/Edit/5
        //[Route("navnerom/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        //[Route("navnerom/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(string registername, string itemname, string parentRegister)
        {
            NameSpace nameSpace = GetRegisterItem(registername, itemname, parentRegister);

            if (nameSpace == null)
            {
                return HttpNotFound();
            }

            if (UserHasAccess(nameSpace, "Edit"))
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
        //[Route("navnerom/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        //[Route("navnerom/{registername}/{itemowner}/{itemname}/rediger")]
        public ActionResult Edit(NameSpace nameSpace, string registername, string itemname, string parentRegister)
        {
            NameSpace originalNameSpace = GetRegisterItem(registername, itemname, parentRegister);
            Models.Register register = GetRegister(registername, parentRegister);
            ValidationName(nameSpace, register);

            if (ModelState.IsValid)
            {
                if (nameSpace.name != null) originalNameSpace.name = nameSpace.name; originalNameSpace.seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(originalNameSpace.name);
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
                _db.Entry(originalNameSpace).State = EntityState.Modified;
                _translationService.UpdateTranslations(nameSpace, originalNameSpace);
                _db.SaveChanges();

                Viewbags(originalNameSpace);
                return Redirect(originalNameSpace.GetObjectUrl());

            }
            Viewbags(originalNameSpace);
            return View(originalNameSpace);
        }

        // GET: NameSpaces/Delete/5
        //[Route("navnerom/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett")]
        //[Route("navnerom/{registername}/{itemowner}/{itemname}/slett")]
        public ActionResult Delete(string itemname, string registername, string parentRegister)
        {
            NameSpace nameSpace = GetRegisterItem(registername, itemname, parentRegister);

            if (nameSpace == null)
            {
                return HttpNotFound();
            }
            if (UserHasAccess(nameSpace, "Delete"))
            {
                Viewbags(nameSpace);
                return View(nameSpace);
            }
            return HttpNotFound("Ingen tilgang");
        }

        // POST: NameSpaces/Delete/5
        [HttpPost, ActionName("Delete")]
        //[Route("navnerom/{parentRegister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett")]
        //[Route("navnerom/{registername}/{itemowner}/{itemname}/slett")]
        public ActionResult DeleteConfirmed(string itemname, string registername, string parentRegister, string registerowner)
        {
            NameSpace nameSpace = GetRegisterItem(registername, itemname, parentRegister);

            if (nameSpace != null)
            {
                var registerUrl = nameSpace.register.GetObjectUrl();
                _db.RegisterItems.Remove(nameSpace);
                _db.SaveChanges();
                return Redirect(registerUrl);
            }
            return HttpNotFound("Finner ikke datasettet");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private Models.Register GetRegister(string registername, string parentRegister)
        {
            var queryResultsRegister = from o in _db.Registers
                                       where o.seoname == registername && o.parentRegister.seoname == parentRegister
                                       select o;

            Models.Register register = queryResultsRegister.FirstOrDefault();
            return register;
        }

        private Organization GetSubmitter()
        {
            string currentUserOrganizationName = CurrentUserOrganizationName();
            var queryResults = from o in _db.Organizations
                               where o.name == currentUserOrganizationName
                               select o;

            return queryResults.FirstOrDefault();
        }

        private NameSpace GetRegisterItem(string registername, string itemname, string parentRegister)
        {
            var query = from o in _db.NameSpases
                                 where o.seoname == itemname &&
                                 o.register.seoname == registername &&
                                 o.register.parentRegister.seoname == parentRegister
                                 select o;

            return query.FirstOrDefault();
        }

        private bool UserHasAccess(NameSpace nameSpace, string when)
        {
            var userEditableRegister = nameSpace.register.accessId == 2;
            if (when == "Create")
            {
                return IsAdmin() || IsEditor() && userEditableRegister && nameSpace.submitter.name.ToLower() == CurrentUserOrganizationName().ToLower();
            }
            else if (when == "Edit" || when == "Delete")
            {
                return IsAdmin() || IsEditor() && userEditableRegister;
            }
            
            return false;
            
        }

        private void Viewbags(NameSpace nameSpace)
        {
            ViewBag.statusId = new SelectList(_db.Statuses.ToList().Select(s => new {s.value, description = s.DescriptionTranslated() }).OrderBy(o => o.description), "value", "description", nameSpace.statusId);
            ViewBag.submitterId = new SelectList(_db.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", nameSpace.submitterId);
        }

        private void ValidationName(NameSpace nameSpace, Models.Register register)
        {
            var queryResultsDataset = from o in _db.NameSpases
                                      where o.name == nameSpace.name &&
                                      o.systemId != nameSpace.systemId &&
                                      o.register.name == register.name &&
                                      o.register.parentRegisterId == register.parentRegisterId
                                      select o.systemId;

            if (queryResultsDataset.Any())
            {
                ModelState.AddModelError("ErrorMessage", Registers.ErrorMessageValidationName);
            }
        }
    }
}
