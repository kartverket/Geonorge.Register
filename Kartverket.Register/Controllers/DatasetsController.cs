using System;
using System.Web.Mvc;
using Kartverket.Register.Models;
using Kartverket.DOK.Service;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services;
using System.Web;
using System.Net.Http;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Controllers
{
    [HandleError]
    public class DatasetsController : Controller
    {
        private RegisterDbContext db = new RegisterDbContext();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IRegisterService _registerService;
        private IRegisterItemService _registerItemService;
        private IAccessControlService _accessControlService;

        public DatasetsController(IRegisterItemService registerItemService, IRegisterService registerService, IAccessControlService accessControllService)
        {
            _registerItemService = registerItemService;
            _registerService = registerService;
            _accessControlService = accessControllService;
        }

        public DatasetsController()
        {
            _registerItemService = new RegisterItemService(db);
            _registerService = new RegisterService(db);
            _accessControlService = new AccessControlService();
        }

        // GET: Datasets/Create
        [Authorize]
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("dataset/{registername}/ny")]
        public ActionResult Create(string registername, string parentRegister)
        {
            Dataset dataset = new Dataset();
            dataset.register = _registerService.GetRegister(parentRegister, registername);
            if (dataset.register != null)
            {
                ViewBag.ThemeGroupId = _registerItemService.GetThemeGroupSelectList(dataset.ThemeGroupId);
                if (_accessControlService.Access(dataset))
                {
                    return View(dataset);
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound("Finner ikke registeret");
        }


        // POST: Datasets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/ny")]
        [Route("dataset/{registername}/ny")]
        public ActionResult Create(Dataset dataset, string registername, string uuid, string parentRegister, string registerowner)
        {
            dataset.register = _registerService.GetRegister(parentRegister, registername);
            if (dataset.register != null)
            {
                if (uuid != null)
                {
                    Viewbags(dataset);
                    Dataset model = GetMetadataFromKartkatalogen(dataset, uuid);
                    model.register = dataset.register;
                    return View(model);
                }

                if (_accessControlService.Access(dataset))
                {
                    if (ModelState.IsValid)
                    {
                        initialisationDataset(dataset);
                        if (!NameIsValid(dataset))
                        {
                            ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                            Viewbags(dataset);
                            return View(dataset);
                        }
                        _registerItemService.SaveNewRegisterItem(dataset);
                        return Redirect(RegisterUrls.DeatilsRegisterItemUrl(parentRegister, registerowner, registername, dataset.datasetowner.seoname, dataset.seoname));
                    }
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            Viewbags(dataset);
            return View(dataset);
        }


        // GET: Datasets/Edit/5
        [Authorize]
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/{itemowner}/{datasetname}/rediger")]
        [Route("dataset/{registername}/{organization}/{datasetname}/rediger")]
        public ActionResult Edit(string registername, string datasetname, string parentRegister)
        {
            Dataset dataset = (Dataset)_registerItemService.GetRegisterItem(parentRegister, registername, datasetname, 1);
            if (dataset != null)
            {
                if (_accessControlService.Access(dataset))
                {
                    Viewbags(dataset);
                    return View(dataset);
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound("Finner ikke datasettet");
        }


        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/{itemowner}/{datasetname}/rediger")]
        [Route("dataset/{registername}/{organization}/{datasetname}/rediger")]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Dataset dataset, string registername, string datasetname, string uuid, bool dontUpdateDescription, string parentRegister, string registerowner)
        {
            Dataset originalDataset = (Dataset)_registerItemService.GetRegisterItem(parentRegister, registername, datasetname, 1);
            if (originalDataset != null)
            {
                if (uuid != null)
                {
                    Dataset model = model = GetMetadataFromKartkatalogen(dataset, uuid);
                    model.register = originalDataset.register;
                    model.datasetowner = originalDataset.datasetowner;
                    model.submitter = originalDataset.submitter;

                    if (dontUpdateDescription) model.description = originalDataset.description;
                    Viewbags(model);
                    return View(model);
                }

                if (_accessControlService.Access(originalDataset))
                {
                    if (ModelState.IsValid)
                    {
                        initialisationDataset(dataset, originalDataset);
                        if (!NameIsValid(dataset))
                        {
                            ModelState.AddModelError("ErrorMessage", HtmlHelperExtensions.ErrorMessageValidationName());
                            Viewbags(originalDataset);
                            return View(originalDataset);
                        }
                        _registerItemService.SaveEditedRegisterItem(originalDataset);
                        return Redirect(RegisterUrls.DeatilsRegisterItemUrl(parentRegister, registerowner, registername, originalDataset.datasetowner.seoname, originalDataset.seoname));
                    }
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            Viewbags(originalDataset);
            return View(originalDataset);
        }


        // GET: Documents/Delete/5
        [Authorize]
        [Route("dataset/{parentregister}/{parentregisterowner}/{registername}/{itemowner}/{datasetname}/slett")]
        [Route("dataset/{registername}/{organization}/{datasetname}/slett")]
        public ActionResult Delete(string registername, string datasetname, string parentregister, string parentregisterowner)
        {
            Dataset dataset = (Dataset)_registerItemService.GetRegisterItem(parentregister, registername, datasetname, 1);
            if (dataset != null)
            {
                if (_accessControlService.Access(dataset))
                {
                    Viewbags(dataset);
                    return View(dataset);
                }
                else
                {
                    throw new HttpException(401, "Access Denied");
                }
            }
            return HttpNotFound("Finner ikke datasettet");
        }


        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("dataset/{parentregister}/{registerowner}/{registername}/{itemowner}/{datasetname}/slett")]
        [Route("dataset/{registername}/{organization}/{datasetname}/slett")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string registername, string datasetname, string parentregister, string registerowner)
        {
            Dataset dataset = (Dataset)_registerItemService.GetRegisterItem(parentregister, registername, datasetname, 1);
            _registerItemService.SaveDeleteRegisterItem(dataset);
            return Redirect(RegisterUrls.registerUrl(parentregister, registerowner, registername));
        }


        // *** HJELPEMETODER

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }

        public static implicit operator DatasetsController(HttpRequestMessage v)
        {
            throw new NotImplementedException();
        }

        private void initialisationDataset(Dataset dataset)
        {
            dataset.systemId = Guid.NewGuid();
            dataset.modified = DateTime.Now;
            dataset.dateSubmitted = DateTime.Now;
            dataset.registerId = dataset.register.systemId;
            dataset.statusId = "Valid";
            dataset.dokStatusId = "Proposal";
            dataset.versionNumber = GetVersionNr(dataset.versionNumber);
            dataset.name = DatasetName(dataset.name);
            dataset.seoname = DatasetSeoName(dataset.name);
            dataset.versioningId = GetVersioningId(dataset);
            SetDatasetOwnerAndSubmitter(dataset);
            dataset.DatasetType = GetDatasetType(dataset);
        }

        private string GetDatasetType(Dataset dataset)
        {
            if (DokMunicipalDataset(dataset))
            {
                _registerItemService.NewCoverage(dataset);
                return "Kommunalt";
            }
            else {
                return "Nasjonalt";
            }
        }

        private static bool DokMunicipalDataset(Dataset dataset)
        {
            return dataset.register.name == "Det offentlige kartgrunnlaget - Kommunalt";
        }

        private void initialisationDataset(Dataset dataset, Dataset originalDataset)
        {
            originalDataset.name = DatasetName(dataset.name);
            originalDataset.seoname = DatasetSeoName(dataset.name);
            originalDataset.description = dataset.description;
            originalDataset.datasetownerId = DatasetOwnerId(dataset, originalDataset);
            originalDataset.submitterId = DatasetSubmitterId(dataset, originalDataset);

            SetDokStatusId(dataset, originalDataset);

            originalDataset.DistributionUrl = dataset.DistributionUrl;
            originalDataset.MetadataUrl = dataset.MetadataUrl;
            originalDataset.PresentationRulesUrl = dataset.PresentationRulesUrl;
            originalDataset.ProductSheetUrl = dataset.ProductSheetUrl;
            originalDataset.ProductSpecificationUrl = dataset.ProductSpecificationUrl;
            originalDataset.WmsUrl = dataset.WmsUrl;
            originalDataset.DistributionFormat = dataset.DistributionFormat;
            originalDataset.DistributionArea = dataset.DistributionArea;
            originalDataset.Notes = dataset.Notes;
            originalDataset.ThemeGroupId = dataset.ThemeGroupId;
            originalDataset.datasetthumbnail = dataset.datasetthumbnail;

            originalDataset.modified = DateTime.Now;
        }

        private void SetDokStatusId(Dataset dataset, Dataset originalDataset)
        {
            if (dataset.dokStatusId != null)
            {
                originalDataset.statusId = dataset.statusId;
                if (originalDataset.statusId == "Accepted")
                {
                    if (dataset.dokStatusDateAccepted == null)
                    {
                        originalDataset.dokStatusDateAccepted = DateTime.Now;
                    }
                    else
                    {
                        originalDataset.dokStatusDateAccepted = dataset.dokStatusDateAccepted;
                    }
                }
                else
                {
                    originalDataset.dokStatusDateAccepted = null;
                }
            }
        }

        private Guid DatasetSubmitterId(Dataset dataset, Dataset originalDataset)
        {
            if (dataset.submitterId != null) return dataset.submitterId;
            else return originalDataset.submitterId;
        }

        private Guid DatasetOwnerId(Dataset dataset, Dataset originalDataset)
        {
            if (dataset.datasetownerId != null) return dataset.datasetownerId;
            else return originalDataset.datasetownerId;
        }

        private void SetDatasetOwnerAndSubmitter(Dataset dataset)
        {
            string organizationLogin = _accessControlService.GetSecurityClaim("organization");
            Organization submitterOrganisasjon = _registerService.GetOrganization(organizationLogin);
            dataset.submitterId = submitterOrganisasjon.systemId;
            dataset.submitter = submitterOrganisasjon;
            dataset.datasetowner = submitterOrganisasjon;
            dataset.datasetownerId = submitterOrganisasjon.systemId;
        }

        private Guid? GetVersioningId(Dataset dataset)
        {
            if (dataset.versioningId == null)
            {
                return _registerItemService.NewVersioningGroup(dataset);
            }
            else
            {
                return dataset.versioningId;
            }
        }

        private string DatasetSeoName(string name)
        {
            return Helpers.RegisterUrls.MakeSeoFriendlyString(name);
        }

        private string DatasetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "ikke angitt";
            }
            else
            {
                return name;
            }
        }

        private int GetVersionNr(int versionNumber)
        {
            if (versionNumber == 0)
            {
                versionNumber = 1;
            }
            else
            {
                versionNumber++;
            }
            return versionNumber;
        }


        private bool NameIsValid(Dataset dataset)
        {
            return _registerItemService.validateName(dataset);
        }

        private Dataset GetMetadataFromKartkatalogen(Dataset dataset, string uuid)
        {
            var model = new Dataset();
            try
            {
                new MetadataService().UpdateDatasetWithMetadata(model, uuid);
            }
            catch (Exception e)
            {
                TempData["error"] = "Det oppstod en feil ved henting av metadata: " + e.Message;
            }

            ViewBag.ThemeGroupId = new SelectList(db.DOKThemes, "value", "description", dataset.ThemeGroupId);
            ViewBag.statusId = new SelectList(db.Statuses, "value", "description");
            return model;
        }

        private void Viewbags(Dataset dataset)
        {
            ViewBag.registerId = _registerItemService.GetRegisterSelectList(dataset.registerId);
            ViewBag.dokStatusId = _registerItemService.GetDokStatusSelectList(dataset.dokStatusId);
            ViewBag.submitterId = _registerItemService.GetSubmitterSelectList(dataset.submitterId);
            ViewBag.datasetownerId = _registerItemService.GetOwnerSelectList(dataset.datasetownerId);
            ViewBag.ThemeGroupId = _registerItemService.GetThemeGroupSelectList(dataset.ThemeGroupId);
        }
    }
}
