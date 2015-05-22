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
using System.IO;
using System.Text;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class CodelistValuesController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: CodelistValues
        public ActionResult Index()
        {
            var registerItems = db.RegisterItems.Include(c => c.register).Include(c => c.status).Include(c => c.submitter);
            return View(registerItems.ToList());
        }


        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/ny/import")]
        [Route("kodeliste/{registername}/ny/import")]
        public ActionResult Import(string registername, string parentregister)
        {
            var queryResults = from o in db.Registers
                               where o.seoname == registername && (o.parentRegister.seoname == null || o.parentRegister.seoname == parentregister)
                               select o.systemId;

            Guid sysId = queryResults.FirstOrDefault();
            Kartverket.Register.Models.Register register = db.Registers.Find(sysId);

            ViewbagImport(register);

            string role = GetSecurityClaim("role");
            if (role == "nd.metadata_admin" || role == "nd.metadata" || role == "nd.metadata_editor")
            {
                return View();
            }
            else
                return HttpNotFound("Ingen tilgang");
        }

        private void ViewbagImport(Kartverket.Register.Models.Register register)
        {
            ViewBag.registerName = register.name;
            ViewBag.registerSeoName = register.seoname;

            if (register.parentRegisterId != null)
            {
                ViewBag.parentRegister = register.parentRegister.name;
                ViewBag.parentRegisterSeoName = register.parentRegister.seoname;
                ViewBag.parentRegisterOwner = register.parentRegister.owner.seoname;
            }
        }


        [HttpPost]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/ny/import")]
        [Route("kodeliste/{registername}/ny/import")]
        [Authorize]
        public ActionResult Import(HttpPostedFileBase csvfile, string registername, string parentregister)
        {
            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername && (o.parentRegister.seoname == null || o.parentRegister.seoname == parentregister)
                                       select o.systemId;
            Guid sysId = queryResultsRegister.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(sysId);

            if (csvfile != null)
            {
                StreamReader csvreader = new StreamReader(csvfile.InputStream);

                // Første rad er overskrift
                if (!csvreader.EndOfStream)
                {
                    csvreader.ReadLine();
                }

                while (!csvreader.EndOfStream)
                {
                    var line = csvreader.ReadLine();
                    var code = line.Split(';');

                    if (code.Count() == 3)
                    {

                        //kodenavn, kodeverdi, beskrivelse
                        CodelistValue codelistValue = new CodelistValue();
                        codelistValue.systemId = Guid.NewGuid();
                        codelistValue.name = code[0];
                        codelistValue.value = code[1];
                        codelistValue.description = code[2];

                        //test på om navnet finnes fra før og at kodeverdi ikke er null                 
                        if (ValidationNameImport(codelistValue, register) && codelistValue.value != null)
                        {
                            //int versjonsnr = 2;
                            //FinnesNavnFraFor(registername, codelistValue, versjonsnr);

                            string organizationLogin = GetSecurityClaim("organization");
                            var queryResultsOrganization = from o in db.Organizations
                                                           where o.name == organizationLogin
                                                           select o.systemId;
                            Guid orgId = queryResultsOrganization.First();
                            Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                            codelistValue.submitterId = orgId;
                            codelistValue.submitter = submitterOrganisasjon;
                            codelistValue.registerId = sysId;
                            codelistValue.modified = DateTime.Now;
                            codelistValue.dateSubmitted = DateTime.Now;
                            codelistValue.registerId = register.systemId;
                            codelistValue.statusId = "Submitted";
                            codelistValue.seoname = MakeSeoFriendlyString(codelistValue.name);

                            db.RegisterItems.Add(codelistValue);
                            db.SaveChanges();
                        }
                    }
                    if (csvfile.ContentType != "text/csv" && csvfile.ContentType != "application/vnd.ms-excel")
                    {
                        ModelState.AddModelError("ErrorMessagefile", "Filen har feil innhold!");
                        ViewbagImport(register);
                        return View();
                    }
                }

                if (register.parentRegisterId != null)
                {
                    return Redirect("/subregister/" + register.parentRegister.seoname + "/" + register.owner.seoname + "/" + registername);
                }
                return Redirect("/register/" + registername);
            }
            ViewbagImport(register);
            return View();
        }


        // GET: CodelistValues/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodelistValue codelistValue = db.CodelistValues.Find(id);
            if (codelistValue == null)
            {
                return HttpNotFound();
            }
            return View(codelistValue);
        }

        // GET: CodelistValues/Create
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/ny")]
        [Route("kodeliste/{registername}/ny")]
        public ActionResult Create(string registername, string parentregister)
        {
            CodelistValue codeListValue = new CodelistValue();
            var queryResults = from o in db.Registers
                               where o.seoname == registername && (o.parentRegister.seoname == null || o.parentRegister.seoname == parentregister)
                               select o.systemId;

            Guid systId = queryResults.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(systId);
            codeListValue.register = register;

            if (register.parentRegisterId != null)
            {
                codeListValue.register.parentRegister = register.parentRegister;
            }
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            if (role == "nd.metadata_admin" || role == "nd.metadata" || role == "nd.metadata_editor")
            {
                return View(codeListValue);
            }
            return HttpNotFound();
        }

        // POST: CodelistValues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/ny")]
        [Route("kodeliste/{registername}/ny")]
        public ActionResult Create(CodelistValue codelistValue, string registername, string parentregister)
        {
            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername && (o.parentRegister.name == null || o.parentRegister.seoname == parentregister)
                                       select o.systemId;

            Guid regId = queryResultsRegister.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);
            string parentRegisterOwner = null;
            if (register.parentRegisterId != null)
            {
                parentRegisterOwner = register.parentRegister.owner.seoname;
            }
            ValidationName(codelistValue, register);

            if (ModelState.IsValid)
            {
                codelistValue.systemId = Guid.NewGuid();
                codelistValue.modified = DateTime.Now;
                codelistValue.dateSubmitted = DateTime.Now;
                codelistValue.registerId = regId;
                codelistValue.statusId = "Submitted";

                if (codelistValue.name == null || codelistValue.name.Length == 0)
                {
                    codelistValue.name = "ikke angitt";
                    codelistValue.seoname = codelistValue.systemId.ToString();
                }

                if (codelistValue.description == null || codelistValue.description.Length == 0)
                {
                    codelistValue.description = "ikke angitt";
                }

                codelistValue.seoname = MakeSeoFriendlyString(codelistValue.name);

                string url = System.Web.Configuration.WebConfigurationManager.AppSettings["RegistryUrl"] + "data/" + Document.DataDirectory;

                string organizationLogin = GetSecurityClaim("organization");

                var queryResults = from o in db.Organizations
                                   where o.name == organizationLogin
                                   select o.systemId;

                Guid orgId = queryResults.First();
                Organization submitterOrganisasjon = db.Organizations.Find(orgId);

                codelistValue.submitterId = orgId;
                codelistValue.submitter = submitterOrganisasjon;

                db.Entry(codelistValue).State = EntityState.Modified;
                //db.SaveChanges();

                db.RegisterItems.Add(codelistValue);
                db.SaveChanges();
                if (!String.IsNullOrWhiteSpace(parentregister))
                {
                    return Redirect("/subregister/" + parentregister + "/" + parentRegisterOwner + "/" + registername + "/" + "/" + codelistValue.submitter.seoname + "/" + codelistValue.seoname);
                }
                else
                {
                    return Redirect("/register/" + registername + "/" + codelistValue.submitter.seoname + "/" + codelistValue.seoname);
                }

            }
            return View(codelistValue);
        }

        // GET: CodelistValues/Edit/5
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        [Route("kodeliste/{registername}/{submitter}/{itemname}/rediger")]
        public ActionResult Edit(string registername, string itemname, string parentregister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResults = from o in db.CodelistValues
                               where o.seoname == itemname && o.register.seoname == registername && (o.register.parentRegister.name == null || o.register.parentRegister.seoname == parentregister)
                               select o.systemId;

            Guid systId = queryResults.First();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodelistValue codelistValue = db.CodelistValues.Find(systId);

            if (codelistValue == null)
            {
                return HttpNotFound();
            }

            if (role == "nd.metadata_admin" || user.ToLower() == codelistValue.submitter.name.ToLower())
            {
                Viewbags(codelistValue);
                return View(codelistValue);
            }
            return HttpNotFound();
        }

        // POST: CodelistValues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/rediger")]
        [Route("kodeliste/{registername}/{submitterCodelist}/{itemname}/rediger")]
        public ActionResult Edit(CodelistValue codelistValue, string submitterCodelist, string registername, string itemname, string parentregister)
        {
            var queryResults = from o in db.CodelistValues
                               where o.seoname == itemname && o.register.seoname == registername && (o.register.parentRegister.name == null || o.register.parentRegister.seoname == parentregister)
                               select o.systemId;

            Guid systId = queryResults.First();
            CodelistValue originalCodelistValue = db.CodelistValues.Find(systId);

            var queryResultsRegister = from o in db.Registers
                                       where o.seoname == registername && (o.parentRegister.name == null || o.parentRegister.seoname == parentregister)
                                       select o.systemId;

            Guid regId = queryResultsRegister.First();
            Kartverket.Register.Models.Register register = db.Registers.Find(regId);

            ValidationName(codelistValue, register);


            if (ModelState.IsValid)
            {
                if (codelistValue.name != null) originalCodelistValue.name = codelistValue.name; originalCodelistValue.seoname = MakeSeoFriendlyString(originalCodelistValue.name);
                if (codelistValue.description != null) originalCodelistValue.description = codelistValue.description;
                if (codelistValue.statusId != null) originalCodelistValue.statusId = codelistValue.statusId;
                if (codelistValue.submitterId != null) originalCodelistValue.submitterId = codelistValue.submitterId;
                if (codelistValue.value != null) originalCodelistValue.value = codelistValue.value;

                if (codelistValue.statusId != null)
                {
                    originalCodelistValue.statusId = codelistValue.statusId;
                    if (originalCodelistValue.statusId != "Accepted" && codelistValue.statusId == "Accepted")
                    {
                        originalCodelistValue.dateAccepted = DateTime.Now;
                    }
                    if (originalCodelistValue.statusId == "Accepted" && codelistValue.statusId != "Accepted")
                    {
                        originalCodelistValue.dateAccepted = null;
                    }
                }

                originalCodelistValue.modified = DateTime.Now;
                db.Entry(originalCodelistValue).State = EntityState.Modified;
                db.SaveChanges();
                Viewbags(codelistValue);


                if (originalCodelistValue.register.parentRegisterId != null)
                {
                    return Redirect("/subregister/" + originalCodelistValue.register.parentRegister.seoname + "/" + originalCodelistValue.register.owner.seoname + "/" + registername + "/" + originalCodelistValue.submitter.seoname + "/" + originalCodelistValue.seoname);
                }

                return Redirect("/register/" + originalCodelistValue.register.seoname + "/" + originalCodelistValue.submitter.seoname + "/" + originalCodelistValue.seoname);
            }
            Viewbags(originalCodelistValue);
            return View(originalCodelistValue);
        }

        // GET: CodelistValues/Delete/5
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett")]
        [Route("kodeliste/{registername}/{submitter}/{itemname}/slett")]
        public ActionResult Delete(string registername, string itemname, string parentregister)
        {
            string role = GetSecurityClaim("role");
            string user = GetSecurityClaim("organization");

            var queryResults = from o in db.CodelistValues
                               where o.seoname == itemname && o.register.seoname == registername && (o.register.parentRegister.name == null || o.register.parentRegister.seoname == parentregister)
                               select o.systemId;

            Guid systId = queryResults.First();

            if (systId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CodelistValue codelistValue = db.CodelistValues.Find(systId);
            if (codelistValue == null)
            {
                return HttpNotFound();
            }
            if (role == "nd.metadata_admin" || user.ToLower() == codelistValue.submitter.name.ToLower())
            {
                return View(codelistValue);
            }
            return HttpNotFound();
        }

        // POST: CodelistValues/Delete/5
        [Authorize]
        [Route("kodeliste/{parentregister}/{registerowner}/{registername}/{itemowner}/{itemname}/slett")]
        [Route("kodeliste/{registername}/{submitter}/{itemname}/slett")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string registername, string itemname, string itemowner, string parentregister)
        {
            var queryResults = from o in db.CodelistValues
                               where o.seoname == itemname && o.register.seoname == registername && (o.register.parentRegister.name == null || o.register.parentRegister.seoname == parentregister)
                               select o.systemId;

            Guid systId = queryResults.First();

            CodelistValue codelistValue = db.CodelistValues.Find(systId);
            string parent = null;
            if (codelistValue.register.parentRegisterId != null)
            {
                parent = codelistValue.register.parentRegister.seoname;
            }


            db.RegisterItems.Remove(codelistValue);
            db.SaveChanges();
            if (parent != null)
            {
                return Redirect("/subregister/" + parentregister + "/" + itemowner + "/" + registername);
            }
            return Redirect("/register/" + registername);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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

        private void ValidationName(CodelistValue codelistValue, Kartverket.Register.Models.Register register)
        {
            var queryResultsDataset = from o in db.CodelistValues
                                      where o.name == codelistValue.name &&
                                            o.systemId != codelistValue.systemId &&
                                            o.register.name == register.name &&
                                            (o.register.parentRegister == null || o.register.parentRegisterId == register.parentRegisterId)
                                      select o.systemId;

            if (queryResultsDataset.Count() > 0)
            {
                ModelState.AddModelError("ErrorMessage", "Navnet finnes fra før!");
            }
        }

        private bool ValidationNameImport(CodelistValue codelistValue, Kartverket.Register.Models.Register register)
        {
            var queryResultsDataset = from o in db.CodelistValues
                                      where o.name == codelistValue.name &&
                                            o.systemId != codelistValue.systemId &&
                                            o.register.name == register.name &&
                                            (o.register.parentRegister == null || o.register.parentRegisterId == register.parentRegisterId)
                                      select o.systemId;

            if (queryResultsDataset.Count() > 0)
            {
                return false;
            }

            return true;
        }

        private static string MakeSeoFriendlyString(string input)
        {
            string encodedUrl = (input ?? "").ToLower();

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // replace norwegian characters
            encodedUrl = encodedUrl.Replace("å", "a").Replace("æ", "ae").Replace("ø", "o");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }

        //private void FinnesNavnFraFor(string registername, CodelistValue codelistValue, int versjonsnr)
        //{
        //    CodelistValue testname = new CodelistValue();

        //    testname = codelistValue;
        //    if (testname.name.Contains("("))
        //    {
        //        string[] nametab = testname.name.Split('(', ')');
        //        string name = nametab[0];
        //        int vnr = Convert.ToInt32(nametab[1]) + 1;

        //        testname.name = name + "(" + vnr + ")";
        //    }
        //    else { 
        //        testname.name += "(2)";
        //    }

        //    if (!ValidationNameImport(codelistValue, registername))
        //    {
        //        FinnesNavnFraFor(registername, testname, versjonsnr);
        //    }

        //    codelistValue.name = testname.name;
        //}

        private void Viewbags(CodelistValue codelistValue)
        {
            //ViewBag.registerId = new SelectList(db.Registers, "systemId", "name", document.registerId);
            ViewBag.statusId = new SelectList(db.Statuses.OrderBy(s => s.description), "value", "description", codelistValue.statusId);
            ViewBag.submitterId = new SelectList(db.Organizations.OrderBy(s => s.name), "systemId", "name", codelistValue.submitterId);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }
    }
}
