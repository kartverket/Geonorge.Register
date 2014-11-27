using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System;

namespace Kartverket.Register.Controllers
{
    public class OrganizationsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        // GET: Organizations
        public ActionResult Index()
        {
            return View(db.Organizations.ToList());
        }

        // GET: Organizations/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // GET: Organizations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Organizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "number,name")] Organization organization, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                organization.systemId = Guid.NewGuid();
                organization.currentVersion = new Models.Version() { systemId = Guid.NewGuid(), versionInfo = "0.1" };
                organization.dateSubmitted = DateTime.Now;
                organization.status = new Status() { value = "Submitted" };

                if (file != null && file.ContentLength > 0)
                {
                    organization.logoFilename = SaveLogoToDisk(file, organization.number);
                }
                db.Organizations.Add(organization);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(organization);
        }

        private string SaveLogoToDisk(HttpPostedFileBase file, string organizationNumber)
        {
            string filename = organizationNumber + "_" + Path.GetFileName(file.FileName);
            var path = Path.Combine(Server.MapPath(Constants.DataDirectory + Organization.DataDirectory), filename);
            file.SaveAs(path);
            return filename;
        }

        // GET: Organizations/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // POST: Organizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Organization organization, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                Organization originalOrganization = db.Organizations.Find(organization.number);
                originalOrganization.name = organization.name;
                if (file != null && file.ContentLength > 0)
                {
                    originalOrganization.logoFilename = SaveLogoToDisk(file, organization.number);
                }
                db.Entry(originalOrganization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(organization);
        }

        // GET: Organizations/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // POST: Organizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Organization organization = db.Organizations.Find(id);
            db.Organizations.Remove(organization);
            db.SaveChanges();
            return RedirectToAction("Index");
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
