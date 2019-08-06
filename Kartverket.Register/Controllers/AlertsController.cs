using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services;
using System.Collections.Generic;
using System.Net;
using Kartverket.Register.Models.Translations;
using System.Linq;

namespace Kartverket.Register.Controllers
{
    public class AlertsController : BaseController
    {
        private readonly RegisterDbContext _dbContext;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;
        private readonly IAccessControlService _accessControlService;

        public AlertsController(RegisterDbContext dbContext, IRegisterItemService registerItemServive, IRegisterService registerService, IAccessControlService accessControlService)
        {
            _dbContext = dbContext;
            _registerItemService = registerItemServive;
            _registerService = registerService;
            _accessControlService = accessControlService;
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
            Dictionary<string, string> serviceList = new Dictionary<string, string>();
            var urlToKartkatalogenApi = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"];
            string url = urlToKartkatalogenApi + "api/search/?facets[0]name=type&facets[0]value=service&limit=1000&orderby=title";
            var currentUserOrganizationName = CurrentUserOrganizationName();
            
            // TODO - handle limit in API request
            if (category == Constants.AlertCategoryDataset) { 
                if(_accessControlService.IsAdmin())
                    url = urlToKartkatalogenApi + "api/search/?facets[0]name=type&facets[0]value=dataset&limit=7000&orderby=title";
                else if(currentUserOrganizationName.ToLower() == "kartverket")
                    url = urlToKartkatalogenApi + "api/search/?facets[0]name=type&facets[0]value=dataset&limit=4000&orderby=title&facets[1]name=organization&facets[1]value=Kartverket&facets[2]name=organization&facets[2]value=Geovekst";
                else
                    url= urlToKartkatalogenApi + "api/search/?facets[0]name=type&facets[0]value=dataset&limit=4000&orderby=title&facets[1]name=organization&facets[1]value=" + currentUserOrganizationName;
            }
            WebClient c = new WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var data = c.DownloadString(url);
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);

            var result = response["Results"];

            foreach (var service in result)
            {
                var serviceUuid = service["Uuid"].ToString();

                var serviceOrganization = service["Organization"] != null ? service["Organization"].ToString() : "";

                if (!serviceList.ContainsKey(serviceUuid))
                {
                    if (_accessControlService.IsAdmin() || _accessControlService.IsItemOwner(serviceOrganization, currentUserOrganizationName))
                    {
                        if (!service["Title"].ToString().StartsWith("Høydedata") && !service["Title"].ToString().StartsWith("Ortofoto"))
                            serviceList.Add(serviceUuid, service["Title"].ToString());
                    }
                    
                }
            }
            return serviceList;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
