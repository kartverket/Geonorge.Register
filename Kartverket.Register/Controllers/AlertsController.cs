using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services;
using System.Collections.Generic;
using System.Net;
using Kartverket.Register.Models.Translations;
using System.Linq;
using System.Web;
using System.IO;
using System;

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
                if (_accessControlService.AddToRegister(alert.register))
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
        public ActionResult Create(Alert alert, string parentRegister, string registerName, string[] tagslist, HttpPostedFileBase imagefile1, HttpPostedFileBase imagefile2, string category = Constants.AlertCategoryService)
        {
            alert.register = _registerService.GetRegister(parentRegister, registerName);
            if (alert.register != null)
            {
                if (_accessControlService.AddToRegister(alert.register))
                {
                    if (ModelState.IsValid)
                    {
                        var selectedStatusId = alert.statusId;
                        var alertTranslation = new AlertTypes(_registerService, category).GetAlertType(alert.AlertType);
                        alert.GetMetadataByUuid();
                        alert.AlertCategory = category;
                        alert.submitter = _registerService.GetOrganizationByUserName();
                        alert.InitializeNewAlert();
                        if (!string.IsNullOrEmpty(selectedStatusId))
                            alert.statusId = selectedStatusId;
                        if (category == Constants.AlertCategoryOperation)
                        {
                            alert.Owner = alert.submitter.name;
                            alert.name = alert.UuidExternal;
                            alert.Tags = new List<Tag>();
                            if(tagslist != null)
                            { 
                                foreach (var tagId in tagslist)
                                {
                                    var tag = _dbContext.Tags.Where(t => t.value == tagId).FirstOrDefault();
                                    alert.Tags.Add(tag);
                                }
                            }
                        }
                        alert.AlertType = alertTranslation.Key.Value;
                        for (int t = 0; t < alert.Translations.Count; t++)
                        {
                            var translation = alertTranslation.Value;
                            alert.Translations[t].AlertType = translation.Where(c => c.Culture.Equals(alert.Translations[t].CultureName)).Select(s => s.AlertType).FirstOrDefault();
                        }
                        alert.versioningId = _registerItemService.NewVersioningGroup(alert);

                        if (imagefile1 != null && imagefile1.ContentLength > 0)
                        {
                            alert.Image1Thumbnail = SaveImageOptimizedToDisk(imagefile1, alert.systemId.ToString());

                            alert.Image1 = SaveImageToDisk(imagefile1, alert.systemId.ToString());
                        }

                        if (imagefile2 != null && imagefile2.ContentLength > 0)
                        {
                            alert.Image2Thumbnail = SaveImageOptimizedToDisk(imagefile2, alert.systemId.ToString());

                            alert.Image2 = SaveImageToDisk(imagefile2, alert.systemId.ToString());
                        }


                        alert.register.modified = System.DateTime.Now;

                        _registerItemService.SaveNewRegisterItem(alert);
                        return Redirect(alert.GetObjectUrl());
                    }
                }
            }
            ViewBags(alert, category);
            return View(alert);
        }

        // GET: Alerts/Edit
        [Authorize]
        public ActionResult Edit(string systemid)
        {
            Alert alert = _dbContext.Alerts.Where(a => a.systemId == new Guid(systemid)).FirstOrDefault();
            if (alert.register != null)
            {
                if (_accessControlService.AddToRegister(alert.register))
                {
                    return View(alert);
                }
            }
            return HttpNotFound();
        }

        // POST: Alerts/Edit
        [HttpPost]
        [Authorize]
        public ActionResult Edit(Alert alert)
        {
            Alert alertOriginal = _dbContext.Alerts.Where(a => a.systemId == alert.systemId).FirstOrDefault();
            if (alert.register != null)
            {
                if (_accessControlService.AddToRegister(alert.register))
                {
                    alertOriginal.EffectiveDate = alert.EffectiveDate;
                    alertOriginal.Summary = alert.Summary;

                    _registerItemService.SaveEditedRegisterItem(alertOriginal);

                    return Redirect("/varsler/" + alertOriginal.seoname + "/" + alertOriginal.systemId);
                }
            }
            return HttpNotFound();
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

            ViewBag.tagsList = new MultiSelectList(_dbContext.Tags.Select(t => new { Key = t.value, Value = t.description  }), "Key", "Value");
            ViewBag.departmentId = new SelectList(_dbContext.Departments.Select(t => new { Key = t.value, Value = t.description }), "Key", "Value", alert.departmentId);
            ViewBag.statusId = new SelectList(_dbContext.Statuses.Where(s => s.value == "Valid" || s.value == "Retired").Select(t => new { Key = t.value, Value = t.description }).OrderByDescending(o => o.Key), "Key", "Value", alert.statusId);
            ViewBag.stations = _dbContext.Stations.ToList();
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

        private string SaveImageToDisk(HttpPostedFileBase file, string seo)
        {
            string filename = seo + "_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + "img/"), filename);
            file.SaveAs(path);
            return filename;
        }

        private string SaveImageOptimizedToDisk(HttpPostedFileBase file, string seo)
        {
            string filename = seo + "_thumb_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + "img/"), filename);

            ImageResizer.ImageJob newImage =
               new ImageResizer.ImageJob(file, path,
               new ImageResizer.Instructions("maxwidth=300;maxheight=1000;quality=75"));

            newImage.Build();

            return filename;
        }
    }
}
