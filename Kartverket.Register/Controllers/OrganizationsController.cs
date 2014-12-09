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

        [Route("register/{registerId}/organisasjoner/ny")]
        public ActionResult Create(string registerId)
        {
            //ViewBag.ThemeGroupId = new SelectList(db.Organizations, "Id", "Name");
            return View();
        }

        // POST: Organizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("register/{registerId}/organisasjoner/ny")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Organization organization, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string registerId)
        {
            if (ModelState.IsValid)
            {
                organization.systemId = Guid.NewGuid();
                //organization.currentVersion = new Models.Version() { systemId = Guid.NewGuid(), versionInfo = "0.1" };
                //organization.name = name;

                if (organization.name == null) {
                    organization.name = "ikke angitt";
                }
                
                organization.modified = DateTime.Now;
                organization.dateSubmitted = DateTime.Now;
                organization.registerId = Guid.Parse(registerId);
                organization.statusId = "Submitted";
                organization.submitter = null;


                if (fileSmal != null && fileSmal.ContentLength > 0)
                {
                    organization.logoFilename = SaveLogoToDisk(fileSmal, organization.number);
                }
                if (fileLarge != null && fileLarge.ContentLength > 0)
                {
                    organization.largeLogo = SaveLogoToDisk(fileLarge, organization.number);
                }

                db.RegisterItems.Add(organization);
                db.SaveChanges();
            }
            //ViewBag.statusId = new SelectList(db.Statuses, "value", "description", organization.statusId);
            //ViewBag.submitterId = new SelectList(db.Organizations, "SystemId", "name", organization.submitterId);
            //ViewBag.ThemeGroupId = new SelectList(db.Organizations, "Id", "Name", organization.registerId); // TEST
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
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", organization.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations, "SystemId", "name", organization.submitterId);
            return View(organization);
        }

        [Route("organisasjoner/rediger/{name}/{id}")]
        public ActionResult Edit(string name, Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kartverket.Register.Models.Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description", organization.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations, "SystemId", "name", organization.submitterId);
            return View(organization);
        }


        // POST: Organizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("organisasjoner/rediger/{name}/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Organization organization, string submitterId, string number, string description, string contact, HttpPostedFileBase fileSmal, HttpPostedFileBase fileLarge, string statusID, string id)
        {
            if (ModelState.IsValid)
            {
                Organization originalOrganization = db.Organizations.Find(Guid.Parse(id));
                
                if (submitterId != null)
                {
                    originalOrganization.submitterId = organization.submitterId;
                }
                if (number != null && number.Length > 0)
                {
                    originalOrganization.number = organization.number;
                }
                if (description != null && description.Length > 0)
                {
                    originalOrganization.description = organization.description; 
                }
                if (contact != null && contact.Length > 0)
                {
                    originalOrganization.contact = organization.contact;
                }
                if (statusID != null)
                {
                    originalOrganization.statusId = statusID;
                }
                if (fileSmal != null && fileSmal.ContentLength > 0)
                {
                    originalOrganization.logoFilename = SaveLogoToDisk(fileSmal, organization.number);
                }
                if (fileLarge != null && fileLarge.ContentLength > 0)
                {
                    originalOrganization.largeLogo = SaveLogoToDisk(fileLarge, organization.number);
                }
                
                db.Entry(originalOrganization).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.statusId = new SelectList(db.Statuses, "value", "description", organization.statusId);
                ViewBag.submitterId = new SelectList(db.Organizations, "SystemId", "name", organization.submitterId);
                return RedirectToAction("Edit");
            }
            return View(organization);
        }

        // GET: Organizations/Delete/5
        [Route("organisasjoner/slett/{name}/{id}")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(Guid.Parse(id));
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // POST: Registers/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("organisasjoner/slett/{name}/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Organization organization, Guid id, string name)
        {
            Organization originalOrganization = db.Organizations.Find(id);

            db.Organizations.Remove(originalOrganization);
            db.SaveChanges();
            return Redirect("/register/organisasjoner/"+ originalOrganization.registerId);
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

