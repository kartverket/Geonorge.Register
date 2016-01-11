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
using System.Collections.Generic;

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
                    Dataset model = GetMetadataFromKartkatalogen(dataset, uuid);
                    Viewbags(dataset);
                    return View(model);
                }
                else if (_accessControlService.Access(dataset))
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
                if (AccessEditingDataset(dataset))
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


        // POST: Dataset/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("dataset/{parentRegister}/{registerowner}/{registername}/{itemowner}/{datasetname}/rediger")]
        [Route("dataset/{registername}/{organization}/{datasetname}/rediger")]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(Dataset dataset, CoverageDataset coverage, string registername, string datasetname, string uuid, string parentRegister, string registerowner, bool dontUpdateDescription = false)
        {
            Dataset originalDataset = (Dataset)_registerItemService.GetRegisterItem(parentRegister, registername, datasetname, 1);
            if (originalDataset != null)
            {
                if (uuid != null)
                {
                    Dataset model = GetMetadataFromKartkatalogen(originalDataset, uuid, dontUpdateDescription);
                    Viewbags(model);
                    return View(model);
                }
                if (originalDataset.DatasetType == "Nasjonalt")
                {
                    if (_accessControlService.IsAdmin())
                    {
                        return EditDataset(dataset, registername, parentRegister, registerowner, originalDataset);
                    }
                    if (_accessControlService.IsMunicipalUser())
                    {
                        return EditCoverageDataset(coverage, registername, parentRegister, registerowner, originalDataset);
                    }
                }
                if (_accessControlService.Access(originalDataset))
                {
                    if (ModelState.IsValid)
                    {
                        return EditDataset(dataset, registername, parentRegister, registerowner, originalDataset);
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


        // POST: Dataset/Delete/5
        [HttpPost, ActionName("Delete")]
        [Route("dataset/{parentregister}/{registerowner}/{registername}/{itemowner}/{datasetname}/slett")]
        [Route("dataset/{registername}/{organization}/{datasetname}/slett")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string registername, string datasetname, string parentregister, string registerowner)
        {
            Dataset dataset = (Dataset)_registerItemService.GetRegisterItem(parentregister, registername, datasetname, 1);
            DeleteCoverageDataset(dataset);
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
            dataset.dokStatusId = GetDokStatusId(dataset.register);
            dataset.versionNumber = GetVersionNr(dataset.versionNumber);
            dataset.name = DatasetName(dataset.name);
            dataset.seoname = DatasetSeoName(dataset.name);
            dataset.versioningId = GetVersioningId(dataset);
            SetDatasetOwnerAndSubmitter(dataset);
            dataset.DatasetType = GetDatasetType(dataset);
        }

        private string GetDokStatusId(Models.Register register)
        {
            if (register.name == "Det offentlige kartgrunnlaget - Kommunalt")
            {
                return "Accepted";
            }
            else {
                return "Proposal";
            }
        }

        private string GetDatasetType(Dataset dataset)
        {
            if (DokMunicipalDataset(dataset))
            {
                dataset.Coverage.Add(_registerItemService.NewCoverage(dataset));
                return "Kommunalt";
            }
            else
            {
                return "Nasjonalt";
            }
        }

        private static bool DokMunicipalDataset(Dataset dataset)
        {
            return dataset.register.name == "Det offentlige kartgrunnlaget - Kommunalt";
        }

        private void initialisationDataset(Dataset dataset, Dataset originalDataset)
        {
            if (originalDataset.register.name ==  "Det offentlige kartgrunnlaget - Kommunalt")
            {
                foreach (CoverageDataset coverage in originalDataset.Coverage)
                {
                    coverage.MunicipalityId = dataset.datasetownerId;
                    coverage.CoverageDOKStatusId = dataset.dokStatusId;
                    _registerItemService.Save();
                }
            }

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
                originalDataset.dokStatusId = dataset.dokStatusId;
                if (originalDataset.dokStatusId == "Accepted")
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
            Organization submitterOrganisasjon = _registerService.GetOrganizationByUserName();
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

        private Dataset GetMetadataFromKartkatalogen(Dataset dataset, string uuid, bool dontUpdateDescription = false)
        {
            var model = new Dataset();
            try
            {
                new MetadataService().UpdateDatasetWithMetadata(model, uuid, dataset, dontUpdateDescription);
            }
            catch (Exception e)
            {
                TempData["error"] = "Det oppstod en feil ved henting av metadata: " + e.Message;
            }
            return model;
        }

        private void Viewbags(Dataset dataset)
        {
            ViewBag.registerId = _registerItemService.GetRegisterSelectList(dataset.registerId);
            ViewBag.dokStatusId = _registerItemService.GetDokStatusSelectList(dataset.dokStatusId);
            ViewBag.CoverageDOKStatusId = _registerItemService.GetDokStatusSelectList(null);
            ViewBag.submitterId = _registerItemService.GetSubmitterSelectList(dataset.submitterId);
            ViewBag.datasetownerId = _registerItemService.GetOwnerSelectList(dataset.datasetownerId);
            ViewBag.ThemeGroupId = _registerItemService.GetThemeGroupSelectList(dataset.ThemeGroupId);
        }

        private bool AccessEditingDataset(Dataset dataset)
        {
            return (dataset.DatasetType == "Nasjonalt" && _accessControlService.IsMunicipalUser()) || _accessControlService.Access(dataset);
        }

        private ActionResult EditDataset(Dataset dataset, string registername, string parentRegister, string registerowner, Dataset originalDataset)
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

        private ActionResult EditCoverageDataset(CoverageDataset coverage, string registername, string parentRegister, string registerowner, Dataset originalDataset)
        {
            initialisationCoverageDataset(coverage, originalDataset);
            _registerItemService.SaveEditedRegisterItem(originalDataset);
            return Redirect(RegisterUrls.DeatilsRegisterItemUrl(parentRegister, registerowner, registername, originalDataset.datasetowner.seoname, originalDataset.seoname));
        }

        private void initialisationCoverageDataset(CoverageDataset coverage, Dataset originalDataset)
        {
            CoverageDataset originalCoverage = _registerItemService.GetMunicipalityCoverage(originalDataset);
            if (originalCoverage == null)
            {
                CoverageDataset newCoverage = new CoverageDataset()
                {
                    CoverageId = Guid.NewGuid(),
                    CoverageDOKStatus = coverage.CoverageDOKStatus,
                    CoverageDOKStatusId = coverage.CoverageDOKStatusId,
                    ConfirmedDok = coverage.ConfirmedDok,
                    DatasetId = originalDataset.systemId,
                    MunicipalityId = SetMunicipality(),
                    Note = coverage.Note
                };

                _registerItemService.SaveNewCoverage(newCoverage);
                originalDataset.Coverage.Add(newCoverage);

            }
            else
            {
                originalCoverage.ConfirmedDok = coverage.ConfirmedDok;
                originalCoverage.CoverageDOKStatusId = coverage.CoverageDOKStatusId;
                originalCoverage.Note = coverage.Note;
            }

        }

        private Guid SetMunicipality()
        {
            Organization municipality = _accessControlService.MunicipalUserOrganization();
            return municipality.systemId;
        }

        private void DeleteCoverageDataset(Dataset dataset)
        {
            if (dataset.Coverage != null)
            {
                for (int i = 0; i < dataset.Coverage.Count; i++)
                {
                    dataset.Coverage[i].DatasetId = Guid.Empty;
                    dataset.Coverage[i].dataset = null;
                    _registerItemService.DeleteCoverage(dataset.Coverage[i]);
                }
                dataset.Coverage.Clear();
            }
        }

    }
}
