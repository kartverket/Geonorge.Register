using Geonorge.AuthLib.Common;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Resources;

namespace Kartverket.Register.Controllers
{
    public class GeoDataCollectionController : Controller
    {
        private readonly RegisterDbContext _dbContext;

        public GeoDataCollectionController(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: GeoDataCollection
        public ActionResult Index(string text = null)
        {
            ViewBag.register = GeodataCollection.RegisterName;
            ViewBag.registerSEO = GeodataCollection.RegisterSeoName;

            if (!string.IsNullOrEmpty(text))
                return View(_dbContext.GeoDataCollections.Where(g=> g.Title.Contains(text) || g.Purpose.Contains(text)).Include("Organization").OrderBy(o => o.Title));
            else
            return View(_dbContext.GeoDataCollections.Include("Organization").OrderBy(o => o.Title));
        }

        // GET: GeoDataCollection/Details/5
        public ActionResult Details(string itemname)
        {
            return View(_dbContext.GeoDataCollections.Include("Organization").Where(o => o.SeoName == itemname).FirstOrDefault());
        }

        // GET: GeoDataCollection/Create
        public ActionResult Create()
        {
            ViewBag.ownerId = new SelectList(_dbContext.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name");
            return View();
        }

        // POST: GeoDataCollection/Create
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost]
        public ActionResult Create(GeoDataCollection collection, string ownerId)
        {
            try
            {
                var org = _dbContext.Organizations.Where(o => o.systemId.ToString() == ownerId).FirstOrDefault();
                collection.systemId = Guid.NewGuid();
                collection.Organization = org;
                collection.SeoName = RegisterUrls.MakeSeoFriendlyString(collection.Title);

                _dbContext.GeoDataCollections.Add(collection);

                _dbContext.SaveChanges();

                return Redirect("Edit?id=" + collection.systemId);
            }
            catch
            {
                return View();
            }
        }

        // GET: GeoDataCollection/Edit/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public ActionResult Edit(string id)
        {
            var collection = _dbContext.GeoDataCollections.Where(o => o.systemId.ToString() == id).FirstOrDefault();
            ViewBag.ownerId = new SelectList(_dbContext.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", collection.Organization.systemId);
            return View(collection);
        }

        // POST: GeoDataCollection/Edit/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost]
        public ActionResult Edit(string systemId, string ownerId, GeoDataCollection collection)
        {
            try
            {
                var geodataCollection = _dbContext.GeoDataCollections.Where(g => g.systemId.ToString() == systemId).FirstOrDefault();
                geodataCollection.Title = collection.Title;
                geodataCollection.SeoName = RegisterUrls.MakeSeoFriendlyString(collection.Title);
                geodataCollection.Link = collection.Link;
                geodataCollection.Purpose = collection.Purpose;

                geodataCollection.DatasetTitle = collection.DatasetTitle;
                geodataCollection.DatasetLink = collection.DatasetLink;
                geodataCollection.Mapper = collection.Mapper;
                geodataCollection.DataOwner = collection.DataOwner;
                geodataCollection.Distributor = collection.Distributor;
                geodataCollection.Methodology = collection.Methodology;
                geodataCollection.ProcessHistory = collection.ProcessHistory;
                geodataCollection.RegistrationRequirements = collection.RegistrationRequirements;
                geodataCollection.MappingRequirements = collection.MappingRequirements;
                geodataCollection.MethodologyDocumentLink = collection.MethodologyDocumentLink;
                geodataCollection.MethodologyLinkWebPage = collection.MethodologyLinkWebPage;
                geodataCollection.SupportSchemes = collection.SupportSchemes;

                var org = _dbContext.Organizations.Where(o => o.systemId.ToString() == ownerId).FirstOrDefault();
                geodataCollection.Organization = org;

                _dbContext.Entry(geodataCollection).State = EntityState.Modified;
                _dbContext.SaveChanges();

                ViewBag.ownerId = new SelectList(_dbContext.Organizations.ToList().Select(s => new { s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", geodataCollection.Organization.systemId);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: GeoDataCollection/Delete/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        public ActionResult Delete(string id)
        {
            return View(_dbContext.GeoDataCollections.Where(o => o.systemId.ToString() == id).FirstOrDefault());
        }

        // POST: GeoDataCollection/Delete/5
        [Authorize(Roles = GeonorgeRoles.MetadataAdmin)]
        [HttpPost]
        public ActionResult Delete(string id, GeoDataCollection collection)
        {
            try
            {
                var geocollection = _dbContext.GeoDataCollections.Where(g => g.systemId.ToString() == id).FirstOrDefault();
                _dbContext.GeoDataCollections.Remove(geocollection);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
