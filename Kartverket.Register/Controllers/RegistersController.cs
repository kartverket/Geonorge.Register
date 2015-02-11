using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models;
using System.Text.RegularExpressions;
using PagedList;

namespace Kartverket.Register.Controllers
{
    public class RegistersController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        
        // GET: Registers
        public ActionResult Index()
        {
            setAccessRole();

            return View(db.Registers.OrderBy(r => r.name).ToList());
        }

        // GET: Registers/Details/5
        [Route("register/{name}")]
        public ActionResult Details(string name, string sorting, int? page)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == name
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);
            ViewBag.page = page;
            ViewBag.SortOrder = sorting;            
            ViewBag.sorting = new SelectList(db.Sorting.ToList(), "value", "description");
            ViewBag.register = register.name;

            
            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        [Route("register/{registername}/{submitter}/{itemname}/")]
        //[Route("{documentowner}/{documentname}/")]
        public ActionResult DetailsRegisterItem(string registername, string itemname)
        {            

            var queryResultsRegisterItem = from o in db.RegisterItems
                                         where o.seoname == itemname && o.register.seoname == registername
                                         select o.systemId;

            Guid systId = queryResultsRegisterItem.First();
            Kartverket.Register.Models.RegisterItem registerItem = db.RegisterItems.Find(systId);
            
            return View(registerItem);
        }
        

        // GET: Registers/Create
        [Authorize]
        public ActionResult Create()
        {
            string role = GetSecurityClaim("role");
            if (role == "nd.metadata_admin")
            {
                return View();
            }
            return HttpNotFound();
        }

        // POST: Registers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "systemId,name,description,dateSubmitted,modified,dateAccepted,containedItemClass")] Kartverket.Register.Models.Register register)
        {
            
            if (ModelState.IsValid)
            {
                register.systemId = Guid.NewGuid();

                var queryResults = from o in db.Organizations
                                   where o.name == Session["user"]
                                   select o.systemId;

                register.ownerId = queryResults.First();
                register.statusId = "Submitted";                
                db.Registers.Add(register);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(register);
        }

        [Authorize]
        // GET: Registers/Edit/5
        public ActionResult Edit(Guid? id)
        {
            string role = GetSecurityClaim("role");
            if (role != "nd.metadata_admin")
            {
                return HttpNotFound();
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kartverket.Register.Models.Register register = db.Registers.Find(id);
            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);

        }



        // POST: Registers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        public ActionResult Edit([Bind(Include = "systemId,name,description,dateSubmitted,modified,dateAccepted,containedItemClass,url")] Kartverket.Register.Models.Register register)
        {
            if (ModelState.IsValid)
            {
                db.Entry(register).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(register);
        }

        // GET: Registers/Delete/5
        [Authorize]
        public ActionResult Delete(Guid? id, string name)
        {
            string role = GetSecurityClaim("role");
            if (role != "nd.metadata_admin")
            {
                return HttpNotFound();
            }
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kartverket.Register.Models.Register register = db.Registers.Find(id);
            if (name == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }

        //// POST: Registers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    Kartverket.Register.Models.Register register = db.Registers.Find(id);
        //    db.Registers.Remove(register);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private string HasAccessToRegister()
        {
            string role = GetSecurityClaim("role");
                        
            bool isAdmin = !string.IsNullOrWhiteSpace(role) && role.Equals("nd.metadata_admin");
            bool isEditor = !string.IsNullOrWhiteSpace(role) && role.Equals("nd.metadata"); //nd.metadata_editor

            if (isAdmin)
            {
                return "admin";
            }
            else if (isEditor)
            {
                return "editor";
            }
            else {
                return "guest";
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

        private void setAccessRole()
        {
            string organization = GetSecurityClaim("organization");

            string role = HasAccessToRegister();
            if (role == "admin")
            {
                Session["role"] = "admin";
                Session["user"] = organization;
            }
            else if (role == "editor")
            {
                Session["role"] = "editor";
                Session["user"] = organization;
            }
            else
            {
                Session["role"] = "guest";
                Session["user"] = "guest";
            }

            
        }

    }
}
