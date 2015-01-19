using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kartverket.DOK.Models;

namespace Kartverket.DOK.Controllers
{
    [Authorize]
    public class ManageThemeGroupController : Controller
    {
        private Kartverket.Register.Models.RegisterDbContext db = new Kartverket.Register.Models.RegisterDbContext();

        // GET: ThemeGroup
        public ActionResult Index()
        {
            return View(db.ThemeGroup.ToList());
        }

        // GET: ThemeGroup/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThemeGroup themeGroup = db.ThemeGroup.Find(id);
            if (themeGroup == null)
            {
                return HttpNotFound();
            }
            return View(themeGroup);
        }

        // GET: ThemeGroup/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ThemeGroup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] ThemeGroup themeGroup)
        {
            if (ModelState.IsValid)
            {
                db.ThemeGroup.Add(themeGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(themeGroup);
        }

        // GET: ThemeGroup/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThemeGroup themeGroup = db.ThemeGroup.Find(id);
            if (themeGroup == null)
            {
                return HttpNotFound();
            }
            return View(themeGroup);
        }

        // POST: ThemeGroup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] ThemeGroup themeGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(themeGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(themeGroup);
        }

        // GET: ThemeGroup/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThemeGroup themeGroup = db.ThemeGroup.Find(id);
            if (themeGroup == null)
            {
                return HttpNotFound();
            }
            return View(themeGroup);
        }

        // POST: ThemeGroup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThemeGroup themeGroup = db.ThemeGroup.Find(id);
            db.ThemeGroup.Remove(themeGroup);
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
