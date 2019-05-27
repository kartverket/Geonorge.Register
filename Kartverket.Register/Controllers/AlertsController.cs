using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services;
using Kartverket.Register.Helpers;
using System.Collections.Generic;
using System.Net;
using Kartverket.Register.Services.Versioning;
using Kartverket.Register.Models.Translations;
using System.Linq;

namespace Kartverket.Register.Controllers
{
    public class AlertsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();
        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _accessControlService;
        private IVersioningService _versioningService;

        public AlertsController(IRegisterItemService registerItemServive, IRegisterService registerService, IAccessControlService accessControlService, IVersioningService versioningService)
        {
            _registerItemService = registerItemServive;
            _registerService = registerService;
            _accessControlService = accessControlService;
            _versioningService = versioningService;
        }

        // GET: Alerts/Create
        [Authorize]
        //[Route("tjenestevarsler/{parentregister}/{registerowner}/{registerName}/ny")]
        //[Route("tjenestevarsler/{registerName}/ny")]
        public ActionResult Create(string parentRegister, string registerName, string category = Constants.AlertCategoryService)
        {
            Alert alert = new Alert();
            alert.AddMissingTranslations();
            alert.register = _registerService.GetRegister(parentRegister, registerName);
            ViewBags(alert, category);

            if (alert.register != null)
            {
                if (_accessControlService.HasAccessTo(alert.register))
                {
                    return View(alert);
                }
            }
            return HttpNotFound();
        }


        // POST: Alerts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        //[Route("tjenestevarsler/{parentregister}/{registerowner}/{registerName}/ny")]
        //[Route("tjenestevarsler/{registerName}/ny")]
        public ActionResult Create(Alert alert, string parentRegister, string registerName, string category = Constants.AlertCategoryService)
        {
            alert.register = _registerService.GetRegister(parentRegister, registerName);
            if (alert.register != null)
            {
                if (_accessControlService.HasAccessTo(alert.register))
                {
                    //if (!_registerItemService.ItemNameIsValid(alert))
                    //{
                    //    ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                    //    return View(alert);
                    //}
                    if (ModelState.IsValid)
                    {
                        var alertTranslation = new AlertTypes(_registerService, category).GetAlertType(alert.AlertType);
                        alert.GetMetadataByUuid();
                        alert.AlertCategory = category;
                        alert.submitter = _registerService.GetOrganizationByUserName();
                        alert.InitializeNewAlert();
                        if (category == Constants.AlertCategoryOperation)
                        {
                            alert.Owner = alert.submitter.name;
                            alert.name = alert.UuidExternal;
                        }
                        alert.AlertType = alertTranslation.Key.Value;
                        for (int t = 0; t < alert.Translations.Count; t++)
                        {
                            var translation = alertTranslation.Value;
                            alert.Translations[t].AlertType = translation.Where(c => c.Culture.Equals(alert.Translations[t].CultureName)).Select(s => s.AlertType).FirstOrDefault();
                        }
                        alert.versioningId = _registerItemService.NewVersioningGroup(alert);
                        alert.register.modified = System.DateTime.Now;
                        _registerItemService.SaveNewRegisterItem(alert);
                        return Redirect(alert.GetObjectUrl());
                    }
                }
            }
            ViewBags(alert, category);
            return View(alert);
        }


        private void ViewBags(Alert alert, string category)
        {
            if (string.IsNullOrEmpty(category))
                category = Constants.AlertCategoryService;
            //ViewBag.AlertType = new SelectList(alert.GetAlertTypes(), alert.AlertType);
            ViewBag.AlertType = new SelectList(new AlertTypes(_registerService, category).GetAlertTypes() , "Key", "Value", alert.AlertType);
            ViewBag.UuidExternal = new SelectList(GetServicesFromKartkatalogen(category), "Key", "Value", alert.UuidExternal);
            ViewBag.Category = category;
            ViewBag.AlertType = new SelectList(new AlertTypes(_registerService, category).GetAlertTypes(), "Key", "Value", alert.AlertType);
            Dictionary<string, string> items = new Dictionary<string, string>();
            var operation = Resources.Resource.AlertCategory(Constants.AlertCategoryOperation);
            items.Add(operation, operation);
            ViewBag.UuidExternal = new SelectList(items, "Key", "Value", operation);
            if (category == Constants.AlertCategoryService || category == Constants.AlertCategoryDataset)
                ViewBag.UuidExternal = new SelectList(GetServicesFromKartkatalogen(category), "Key", "Value", alert.UuidExternal);
        }

        public Dictionary<string, string> GetServicesFromKartkatalogen(string category)
        {
            Dictionary<string, string> ServiceList = new Dictionary<string, string>();
            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/search/?facets[0]name=type&facets[0]value=service&limit=1000&orderby=title";
            var organization = _accessControlService.GetSecurityClaim("organization")[0];
            if (category == Constants.AlertCategoryDataset) { 
                if(_accessControlService.IsAdmin())
                    url = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/search/?facets[0]name=type&facets[0]value=dataset&limit=7000&orderby=title";
                else if(organization.ToLower() == "kartverket")
                    url = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/search/?facets[0]name=type&facets[0]value=dataset&limit=4000&orderby=title&facets[1]name=organization&facets[1]value=Kartverket&facets[2]name=organization&facets[2]value=Geovekst";
                else
                    url= System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/search/?facets[0]name=type&facets[0]value=dataset&limit=4000&orderby=title&facets[1]name=organization&facets[1]value=" + organization;
            }
            WebClient c = new WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var data = c.DownloadString(url);
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);

            var result = response["Results"];

            foreach (var service in result)
            {
                var ServiceUuid = service["Uuid"].ToString();
                if (string.IsNullOrWhiteSpace(ServiceUuid))
                    ServiceUuid = service["Uuid"].ToString();

                var Organization = service["Organization"] != null ? service["Organization"].ToString() : "";

                if (!ServiceList.ContainsKey(ServiceUuid))
                {
                    if (_accessControlService.IsAdmin() || _accessControlService.IsItemOwner(Organization, _accessControlService.GetSecurityClaim("organization")[0]))
                    {
                        if (!service["Title"].ToString().StartsWith("Høydedata") && !service["Title"].ToString().StartsWith("Ortofoto"))
                            ServiceList.Add(ServiceUuid, service["Title"].ToString());
                    }
                    
                }
            }
            return ServiceList;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
