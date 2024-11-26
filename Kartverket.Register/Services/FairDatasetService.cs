using System;
using System.Collections.Generic;
using System.Data.Entity;
using Kartverket.Register.Models;
using System.Linq;
using System.Web.Configuration;
using Kartverket.DOK.Service;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using GeoNorgeAPI;
using Kartverket.Register.Models.FAIR;
using System.Net;
using Kartverket.Register.Models.Api;
using System.Runtime.Remoting.MetadataServices;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Results;

namespace Kartverket.Register.Services
{
    public class FairDatasetService : IFairDatasetService
    {
        private readonly RegisterDbContext _dbContext;
        private readonly IRegisterService _registerService;
        private readonly IDatasetDeliveryService _datasetDeliveryService;
        private readonly IRegisterItemService _registerItemService;
        private readonly MetadataService _metadataService;
        private readonly IFairService _fairService;
        MetadataModel _metadata;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly HttpClient _httpClient = new HttpClient();

        public FairDatasetService(RegisterDbContext dbContext, IFairService fairService)
        {
            _dbContext = dbContext;
            _registerService = new RegisterService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
            _registerItemService = new RegisterItemService(_dbContext);
            _metadataService = new MetadataService(_dbContext);
            _fairService = fairService;
        }

        public FairDataset GetFairDatasetByName(string registerSeoName, string itemSeoName)
        {
            var queryResult = from i in _dbContext.FairDatasets
                              where i.Seoname == itemSeoName &&
                              i.Register.seoname == registerSeoName
                              select i;

            return queryResult.FirstOrDefault();
        }

        public FairDatasetViewModel NewFairDatasetViewModel(string parentregister, string registername)
        {
            var model = new FairDatasetViewModel() { RegisterId = _registerService.GetRegisterId(parentregister, registername) };

            return model;
        }

        public FairDataset NewFairDataset(FairDatasetViewModel FairViewModel, string parentregister, string registername)
        {
            var FairDataset = new FairDataset();

            FairDataset.SystemId = Guid.NewGuid();
            FairDataset.Name = FairViewModel.Name;
            FairDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(FairDataset.Name);
            FairDataset.Description = FairViewModel.Description;
            FairDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            FairDataset.OwnerId = FairViewModel.OwnerId;
            FairDataset.DateSubmitted = DateTime.Now;
            FairDataset.Modified = DateTime.Now;
            FairDataset.RegisterId = _registerService.GetRegisterId(parentregister, registername);
            FairDataset.VersioningId = _registerItemService.NewVersioningGroup(FairDataset);
            FairDataset.VersionNumber = 1;
            FairDataset.StatusId = "Submitted";

            FairDataset.Uuid = FairViewModel.Uuid;
            FairDataset.Notes = FairViewModel.Notes;
            FairDataset.SpecificUsage = FairViewModel.SpecificUsage;
            FairDataset.ProductSheetUrl = FairViewModel.ProductSheetUrl;
            FairDataset.PresentationRulesUrl = FairViewModel.PresentationRulesUrl;
            FairDataset.ProductSpecificationUrl = FairViewModel.ProductSpecificationUrl;
            FairDataset.MetadataUrl = FairViewModel.MetadataUrl;
            FairDataset.DistributionFormat = FairViewModel.DistributionFormat;
            FairDataset.DistributionUrl = FairViewModel.DistributionUrl;
            FairDataset.DistributionArea = FairViewModel.DistributionArea;
            FairDataset.WmsUrl = FairViewModel.WmsUrl;
            FairDataset.ThemeGroupId = FairViewModel.ThemeGroupId;
            FairDataset.DatasetThumbnail = FairViewModel.DatasetThumbnail;
            FairDataset.DokStatusId = "Proposal";
            FairDataset.UuidService = FairViewModel.UuidService;

            GetDeliveryStatuses(FairViewModel, FairDataset);

            FairDataset.MetadataStatusId = _datasetDeliveryService.CreateDatasetDelivery(FairViewModel.MetadataStatusId, FairViewModel.MetadataNote);
            FairDataset.ProductSpesificationStatusId = _datasetDeliveryService.CreateDatasetDelivery(FairViewModel.WmsStatusId, FairViewModel.WmsNote);
            FairDataset.SosiDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(FairViewModel.WmsStatusId, FairViewModel.WmsNote);
            FairDataset.GmlDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(FairViewModel.WmsStatusId, FairViewModel.WmsNote);
            FairDataset.WmsStatusId = _datasetDeliveryService.CreateDatasetDelivery(FairViewModel.WmsStatusId, FairViewModel.WmsNote);
            FairDataset.WfsStatusId = _datasetDeliveryService.CreateDatasetDelivery(FairViewModel.WfsStatusId, FairViewModel.WfsNote);
            FairDataset.AtomFeedStatusId = _datasetDeliveryService.CreateDatasetDelivery(FairViewModel.AtomFeedStatusId, FairViewModel.AtomFeedNote);
            FairDataset.CommonStatusId = _datasetDeliveryService.CreateDatasetDelivery(FairViewModel.WmsStatusId, FairViewModel.WmsNote);
            FairDataset.Grade = GetGrade(FairDataset);
            _dbContext.FairDatasets.Add(FairDataset);
            _dbContext.SaveChanges();

            return FairDataset;
        }

        private void GetDeliveryStatuses(FairDatasetViewModel FairDatasetViewModel, FairDataset FairDataset)
        {
            FairDatasetViewModel.MetadataStatusId = _datasetDeliveryService.GetMetadataStatus(FairDataset.Uuid, true, FairDatasetViewModel.MetadataStatusId);
            FairDatasetViewModel.ProductSpesificationStatusId = _registerService.GetDOKStatus(FairDataset.ProductSpecificationUrl, true, FairDatasetViewModel.ProductSpesificationStatusId);
            FairDatasetViewModel.ProductSheetStatusId = _registerService.GetDOKStatus(FairDataset.ProductSheetUrl, true, FairDatasetViewModel.ProductSpesificationStatusId);
            FairDatasetViewModel.PresentationRulesStatusId = _registerService.GetDOKStatus(FairDataset.PresentationRulesUrl, true, FairDatasetViewModel.PresentationRulesStatusId);
            FairDatasetViewModel.SosiDataStatusId = _registerService.GetSosiRequirements(FairDataset.Uuid, FairDatasetViewModel.ProductSpecificationUrl, true, FairDatasetViewModel.SosiDataStatusId);
            FairDatasetViewModel.GmlDataStatusId = _registerService.GetGmlRequirements(FairDataset.Uuid, true, FairDatasetViewModel.GmlDataStatusId);
            FairDatasetViewModel.WmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(FairDataset.Uuid, true, FairDatasetViewModel.WmsStatusId, FairDataset.UuidService);
            FairDatasetViewModel.WfsStatusId = _datasetDeliveryService.GetWfsStatus(FairDataset.Uuid, true, FairDatasetViewModel.WfsStatusId);
            FairDatasetViewModel.AtomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(FairDataset.Uuid, true, FairDatasetViewModel.AtomFeedStatusId);
            FairDatasetViewModel.CommonStatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(FairDatasetViewModel.WfsStatusId, FairDatasetViewModel.AtomFeedStatusId);
            if (FairDataset.Grade.HasValue)
                FairDatasetViewModel.Grade = FairDataset.Grade.Value;
        }

        public FairDataset UpdateFairDatasetFromKartkatalogen(FairDataset originalDataset)
        {
            var FairDatasetFromKartkatalogen = _metadataService.FetchFairDatasetFromKartkatalogen(originalDataset.Uuid);
            return FairDatasetFromKartkatalogen == null ? originalDataset : UpdateFairDataset(originalDataset, FairDatasetFromKartkatalogen);
        }

        public FairDataset UpdateFairDataset(FairDatasetViewModel viewModel)
        {
            var FairDataset = GetFairDatasetBySystemId(viewModel.SystemId);
            FairDataset.Name = viewModel.Name;
            FairDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(FairDataset.Name);
            FairDataset.Description = viewModel.Description;
            FairDataset.SubmitterId = viewModel.SubmitterId;
            FairDataset.OwnerId = viewModel.OwnerId;
            FairDataset.Modified = DateTime.Now;
            FairDataset.DateSubmitted = DateTime.Now;
            FairDataset.RegisterId = viewModel.RegisterId;

            FairDataset.Uuid = viewModel.Uuid;
            FairDataset.Notes = viewModel.Notes;
            FairDataset.SpecificUsage = viewModel.SpecificUsage;
            FairDataset.ProductSheetUrl = viewModel.ProductSheetUrl;
            FairDataset.PresentationRulesUrl = viewModel.PresentationRulesUrl;
            FairDataset.ProductSpecificationUrl = viewModel.ProductSpecificationUrl;
            FairDataset.MetadataUrl = viewModel.MetadataUrl;
            FairDataset.DistributionFormat = viewModel.DistributionFormat;
            FairDataset.DistributionUrl = viewModel.DistributionUrl;
            FairDataset.DistributionArea = viewModel.DistributionArea;
            FairDataset.WmsUrl = viewModel.WmsUrl;
            FairDataset.ThemeGroupId = viewModel.ThemeGroupId;
            FairDataset.DatasetThumbnail = viewModel.DatasetThumbnail;
            FairDataset.DokStatusId = viewModel.DokStatusId;
            FairDataset.DokStatusDateAccepted = viewModel.GetDateAccepted();
            FairDataset.UuidService = viewModel.UuidService;

            if (FairDataset.MetadataStatus != null)
            {
                FairDataset.MetadataStatus.StatusId = viewModel.MetadataStatusId;
                FairDataset.MetadataStatus.Note = viewModel.MetadataNote;
                FairDataset.MetadataStatus.AutoUpdate = viewModel.MetadataAutoUpdate;
            }

            if (FairDataset.ProductSpesificationStatus != null)
            {
                FairDataset.ProductSpesificationStatus.StatusId = viewModel.ProductSpesificationStatusId;
                FairDataset.ProductSpesificationStatus.Note = viewModel.ProduktspesifikasjonNote;
                FairDataset.ProductSpesificationStatus.AutoUpdate = viewModel.ProduktspesifikasjonAutoUpdate;
            }

            if (FairDataset.SosiDataStatus != null)
            {
                FairDataset.SosiDataStatus.StatusId = viewModel.SosiDataStatusId;
                FairDataset.SosiDataStatus.Note = viewModel.SosiDataNote;
                FairDataset.SosiDataStatus.AutoUpdate = viewModel.SosiDataAutoUpdate;
            }

            if (FairDataset.GmlDataStatus != null)
            {
                FairDataset.GmlDataStatus.StatusId = viewModel.GmlDataStatusId;
                FairDataset.GmlDataStatus.Note = viewModel.GmlDataNote;
                FairDataset.GmlDataStatus.AutoUpdate = viewModel.GmlDataAutoUpdate;
            }

            if (FairDataset.WmsStatus != null)
            {
                FairDataset.WmsStatus.StatusId = viewModel.WmsStatusId;
                FairDataset.WmsStatus.Note = viewModel.WmsNote;
                FairDataset.WmsStatus.AutoUpdate = viewModel.WmsAutoUpdate;
            }

            if (FairDataset.WfsStatus != null)
            {
                FairDataset.WfsStatus.StatusId = viewModel.WfsStatusId;
                FairDataset.WfsStatus.Note = viewModel.WfsNote;
                FairDataset.WfsStatus.AutoUpdate = viewModel.WfsAutoUpdate;
            }

            if (FairDataset.AtomFeedStatus != null)
            {
                FairDataset.AtomFeedStatus.StatusId = viewModel.AtomFeedStatusId;
                FairDataset.AtomFeedStatus.Note = viewModel.AtomFeedNote;
                FairDataset.AtomFeedStatus.AutoUpdate = viewModel.AtomFeedAutoUpdate;
            }

            if (FairDataset.CommonStatus != null)
            {
                FairDataset.CommonStatus.StatusId = viewModel.CommonStatusId;
                FairDataset.CommonStatus.Note = viewModel.CommonNote;
                FairDataset.CommonStatus.AutoUpdate = viewModel.CommonAutoUpdate;
            }

            _dbContext.Entry(FairDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return FairDataset;
        }

        private float GetGrade(FairDataset dataset)
        {
            float grade = 0;

            float gradeGood = 1;
            float gradeUseable = 0.5F;

            var MetadataStatus = _dbContext.DatasetDeliveries.Where(s => s.DatasetDeliveryId == dataset.MetadataStatusId).FirstOrDefault();
            var ProductSpesificationStatus = _dbContext.DatasetDeliveries.Where(s => s.DatasetDeliveryId == dataset.ProductSpesificationStatusId).FirstOrDefault();
            var ProductSheetStatus = _dbContext.DatasetDeliveries.Where(s => s.DatasetDeliveryId == dataset.ProductSheetStatusId).FirstOrDefault();
            var PresentationRulesStatus = _dbContext.DatasetDeliveries.Where(s => s.DatasetDeliveryId == dataset.PresentationRulesStatusId).FirstOrDefault();
            var SosiDataStatus = _dbContext.DatasetDeliveries.Where(s => s.DatasetDeliveryId == dataset.SosiDataStatusId).FirstOrDefault();
            var GmlDataStatus = _dbContext.DatasetDeliveries.Where(s => s.DatasetDeliveryId == dataset.GmlDataStatusId).FirstOrDefault();
            var WmsStatus = _dbContext.DatasetDeliveries.Where(s => s.DatasetDeliveryId == dataset.WmsStatusId).FirstOrDefault();
            var WfsStatus = _dbContext.DatasetDeliveries.Where(s => s.DatasetDeliveryId == dataset.WfsStatusId).FirstOrDefault();
            var AtomFeedStatus = _dbContext.DatasetDeliveries.Where(s => s.DatasetDeliveryId == dataset.AtomFeedStatusId).FirstOrDefault();
            var CommonStatus = _dbContext.DatasetDeliveries.Where(s => s.DatasetDeliveryId == dataset.CommonStatusId).FirstOrDefault();


            if (MetadataStatus != null && MetadataStatus.IsGood())
                grade = grade + gradeGood;
            else if (MetadataStatus != null && MetadataStatus.IsGoodOrUseable())
                grade = grade + gradeUseable;

            if (ProductSpesificationStatus != null && ProductSpesificationStatus.IsGood())
                grade = grade + gradeGood;
            else if (ProductSpesificationStatus != null && ProductSpesificationStatus.IsGoodOrUseable())
                grade = grade + gradeUseable;

            if (ProductSheetStatus != null && ProductSheetStatus.IsGood())
                grade = grade + gradeGood;
            else if (ProductSheetStatus != null && ProductSheetStatus.IsGoodOrUseable())
                grade = grade + gradeUseable;

            if (PresentationRulesStatus != null && PresentationRulesStatus.IsGood())
                grade = grade + gradeGood;
            else if (PresentationRulesStatus != null && PresentationRulesStatus.IsGoodOrUseable())
                grade = grade + gradeUseable;

            if (SosiDataStatus != null && SosiDataStatus.IsGood())
                grade = grade + gradeGood;
            else if (SosiDataStatus != null && SosiDataStatus.IsGoodOrUseable())
                grade = grade + gradeUseable;

            if (GmlDataStatus != null && GmlDataStatus.IsGood())
                grade = grade + gradeGood;
            else if (GmlDataStatus != null && GmlDataStatus.IsGoodOrUseable())
                grade = grade + gradeUseable;

            if (WmsStatus != null && WmsStatus.IsGood())
                grade = grade + gradeGood;
            else if (WmsStatus != null && WmsStatus.IsGoodOrUseable())
                grade = grade + gradeUseable;

            if (WfsStatus != null && WfsStatus.IsGood())
                grade = grade + gradeGood;
            else if (WfsStatus != null && WfsStatus.IsGoodOrUseable())
                grade = grade + gradeUseable;

            if (AtomFeedStatus != null && AtomFeedStatus.IsGood())
                grade = grade + gradeGood;
            else if (AtomFeedStatus != null && AtomFeedStatus.IsGoodOrUseable())
                grade = grade + gradeUseable;

            if (CommonStatus != null && CommonStatus.IsGood())
                grade = grade + gradeGood;
            else if (CommonStatus != null && CommonStatus.IsGoodOrUseable())
                grade = grade + gradeUseable;

            return grade;
        }

        public void DeleteFairDataset(FairDataset FairDataset)
        {
            if (FairDataset.FairDatasetTypes != null && FairDataset.FairDatasetTypes.Count > 0)
            {
                foreach (var fairDatasetType in FairDataset.FairDatasetTypes.ToList())
                {
                    _dbContext.Entry(fairDatasetType).State = EntityState.Deleted;
                }
                FairDataset.FairDatasetTypes.Clear();
                _dbContext.SaveChanges();
            }

            _dbContext.FairDatasets.Remove(FairDataset);

            //Todo, må slette deliveryDataset?
            _dbContext.SaveChanges();
        }

        public FairDataset GetFairDatasetById(string uuid)
        {
            Guid guid = Guid.Parse(uuid);
            var queryResult = from i in _dbContext.FairDatasets
                              where i.Uuid == uuid
                                    || i.SystemId == guid
                              select i;

            return queryResult.FirstOrDefault();
        }


        private FairDataset GetFairDatasetBySystemId(Guid systemId)
        {
            var queryResult = from i in _dbContext.FairDatasets
                              where i.SystemId == systemId
                              select i;

            return queryResult.FirstOrDefault();
        }

        public void SynchronizeFairDatasets()
        {
            var FairDatasetsFromKartkatalogen = FetchFairDatasets();
            if (FairDatasetsFromKartkatalogen != null && FairDatasetsFromKartkatalogen.Count > 0)
            {
                RemoveFairDatasets(FairDatasetsFromKartkatalogen);
                UpdateFairDataset(FairDatasetsFromKartkatalogen);
                _dbContext.SaveChanges();
            }
        }

        private void UpdateFairDataset(List<FairDataset> FairDatasetsFromKartkatalogen)
        {
            //Update register
            foreach (var FairDataset in FairDatasetsFromKartkatalogen)
            {
                try
                {
                    Log.Info("Start updating FairDataset: " + FairDataset.Uuid);
                    _metadata = GetMetadata(FairDataset);

                    var originalFairDataset = GetFairDatasetByUuid(FairDataset.Uuid);
                    if (originalFairDataset != null)
                    {
                        UpdateFairDataset(originalFairDataset, FairDataset);
                    }
                    else
                    {
                        NewFairDatasetFromKartkatalogen(FairDataset);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error updating FairDataset: " + FairDataset.Uuid, ex);
                }

                Log.Info("Finished updating FairDataset: " + FairDataset.Uuid);
            }
        }

        private MetadataModel GetMetadata(FairDataset FairDataset)
        {
            MetadataModel metadata = new MetadataModel();
            metadata.SimpleMetadata = MetadataService.FetchMetadata(FairDataset.Uuid);
            return metadata;
        }

        private void NewFairDatasetFromKartkatalogen(FairDataset FairDataset)
        {
            if (!_registerItemService.ItemNameIsValid(FairDataset)) return;
            FairDataset.SystemId = Guid.NewGuid();
            FairDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(FairDataset.Name);
            FairDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            FairDataset.DateSubmitted = DateTime.Now;
            FairDataset.Modified = DateTime.Now;
            FairDataset.RegisterId = _registerService.GetFairRegisterId();
            FairDataset.VersioningId = _registerItemService.NewVersioningGroup(FairDataset);
            FairDataset.VersionNumber = 1;
            FairDataset.StatusId = "Submitted";
            FairDataset.DokStatusId = "Proposal";

            var metadataStatusId = _datasetDeliveryService.GetMetadataStatus(FairDataset.Uuid);
            var productSpesificationStatusId = _registerService.GetDOKStatus(FairDataset.ProductSpecificationUrl, true, "deficient");
            var productSheetStatusId = _registerService.GetDOKStatus(FairDataset.ProductSheetUrl, true, "deficient");
            var presentationRulesStatusId = _registerService.GetDOKStatus(FairDataset.PresentationRulesUrl, true, "deficient");
            var sosiDataStatusId = _registerService.GetSosiRequirements(FairDataset.Uuid, "", true, "deficient");
            var gmlDataStatusId = _registerService.GetGmlRequirements(FairDataset.Uuid, true, "deficient");
            var wmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(FairDataset.Uuid, true, "deficient", FairDataset.UuidService, FairDataset);
            var wfsStatusId = _datasetDeliveryService.GetWfsStatus(FairDataset.Uuid);
            var atomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(FairDataset.Uuid);
            var commonStatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(wfsStatusId, atomFeedStatusId);



            FairDataset.MetadataStatusId = _datasetDeliveryService.CreateDatasetDelivery(metadataStatusId);
            FairDataset.ProductSpesificationStatusId = _datasetDeliveryService.CreateDatasetDelivery(productSpesificationStatusId);
            FairDataset.ProductSheetStatusId = _datasetDeliveryService.CreateDatasetDelivery(productSheetStatusId);
            FairDataset.PresentationRulesStatusId = _datasetDeliveryService.CreateDatasetDelivery(presentationRulesStatusId);
            FairDataset.SosiDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(sosiDataStatusId);
            FairDataset.GmlDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(gmlDataStatusId);
            FairDataset.WmsStatusId = _datasetDeliveryService.CreateDatasetDelivery(wmsStatusId);
            FairDataset.WfsStatusId = _datasetDeliveryService.CreateDatasetDelivery(wfsStatusId);
            FairDataset.AtomFeedStatusId = _datasetDeliveryService.CreateDatasetDelivery(atomFeedStatusId);
            FairDataset.CommonStatusId = _datasetDeliveryService.CreateDatasetDelivery(commonStatusId);
            FairDataset.Grade = GetGrade(FairDataset);

            SetFAIR(ref FairDataset);

            _dbContext.FairDatasets.Add(FairDataset);
            _dbContext.SaveChanges();
            _dbContext.DetachAllEntities();
        }

        private void SetFAIR(ref FairDataset fairDataset)
        {
            var dataset = _fairService.GetFair(_metadata, fairDataset.WfsStatus, fairDataset.WmsStatus, fairDataset.AtomFeedStatus);

            fairDataset.F2_a_Criteria = dataset.F2_a_Criteria;
            fairDataset.F2_b_Criteria = dataset.F2_b_Criteria;
            fairDataset.F2_c_Criteria = dataset.F2_c_Criteria;
            fairDataset.F2_d_Criteria = dataset.F2_d_Criteria;
            fairDataset.F2_e_Criteria = dataset.F2_e_Criteria;
            fairDataset.F3_a_Criteria = dataset.F3_a_Criteria;

            fairDataset.FindableStatusPerCent = dataset.FindableStatusPerCent;

            var findableStatus = dataset.FindableStatus;
            _dbContext.FAIRDeliveries.Add(findableStatus);
            _dbContext.SaveChanges();
            fairDataset.FindableStatusId = findableStatus.FAIRDeliveryId;

            fairDataset.A1_a_Criteria = dataset.A1_a_Criteria;
            fairDataset.A1_b_Criteria = dataset.A1_b_Criteria;
            fairDataset.A1_c_Criteria = dataset.A1_c_Criteria;
            fairDataset.A1_d_Criteria = dataset.A1_d_Criteria;
            fairDataset.A1_e_Criteria = dataset.A1_e_Criteria;
            fairDataset.A1_f_Criteria = dataset.A1_f_Criteria;

            fairDataset.AccesibleStatusPerCent = dataset.AccesibleStatusPerCent;
            var accesibleStatus = dataset.AccesibleStatus;
            _dbContext.FAIRDeliveries.Add(accesibleStatus);
            _dbContext.SaveChanges();
            fairDataset.AccesibleStatusId = accesibleStatus.FAIRDeliveryId;


            fairDataset.I1_a_Criteria = dataset.I1_a_Criteria;
            fairDataset.I1_b_Criteria = dataset.I1_b_Criteria;
            //fairDataset.I1_c_Criteria = dataset.I1_c_Criteria; //Moved to I3_a_Criteria
            fairDataset.I2_a_Criteria = dataset.I2_a_Criteria;
            fairDataset.I2_b_Criteria = dataset.I2_b_Criteria;
            fairDataset.I3_a_Criteria = dataset.I3_a_Criteria;
            fairDataset.I3_b_Criteria = dataset.I3_b_Criteria;
            fairDataset.I3_c_Criteria = dataset.I3_c_Criteria;


            fairDataset.InteroperableStatusPerCent = dataset.InteroperableStatusPerCent;
            var interoperableStatus = dataset.InteroperableStatus;
            _dbContext.FAIRDeliveries.Add(interoperableStatus);
            _dbContext.SaveChanges();
            fairDataset.InteroperableStatusId = interoperableStatus.FAIRDeliveryId;


            fairDataset.R1_a_Criteria = dataset.R1_a_Criteria;
            fairDataset.R1_b_Criteria = dataset.R1_b_Criteria;
            fairDataset.R2_a_Criteria = dataset.R2_a_Criteria;
            fairDataset.R2_b_Criteria = dataset.R2_b_Criteria;
            fairDataset.R2_c_Criteria = dataset.R2_c_Criteria;
            fairDataset.R2_d_Criteria = dataset.R2_d_Criteria;
            fairDataset.R2_e_Criteria = dataset.R2_e_Criteria;
            fairDataset.R2_f_Criteria = dataset.R2_f_Criteria;
            fairDataset.R2_g_Criteria = dataset.R2_g_Criteria;
            fairDataset.R2_h_Criteria = dataset.R2_h_Criteria;
            fairDataset.R3_b_Criteria = dataset.R3_b_Criteria;

            fairDataset.ReUseableStatusPerCent = dataset.ReUseableStatusPerCent;
            var reusableStatus = dataset.ReUseableStatus;
            _dbContext.FAIRDeliveries.Add(reusableStatus);
            _dbContext.SaveChanges();
            fairDataset.ReUseableStatusId = reusableStatus.FAIRDeliveryId;


            fairDataset.FAIRStatusPerCent = dataset.FAIRStatusPerCent;
            var fairStatus = dataset.FAIRStatus;
            _dbContext.FAIRDeliveries.Add(fairStatus);
            _dbContext.SaveChanges();
            fairDataset.FAIRStatusId = fairStatus.FAIRDeliveryId;

        }

        private FairDataset UpdateFairDataset(FairDataset originalDataset, FairDataset FairDatasetFromKartkatalogen)
        {
            originalDataset.Name = FairDatasetFromKartkatalogen.Name;
            originalDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(originalDataset.Name);
            originalDataset.Description = FairDatasetFromKartkatalogen.Description;
            originalDataset.OwnerId = FairDatasetFromKartkatalogen.OwnerId;
            originalDataset.Modified = DateTime.Now;

            originalDataset.Uuid = FairDatasetFromKartkatalogen.Uuid;
            originalDataset.Notes = FairDatasetFromKartkatalogen.Notes;
            originalDataset.SpecificUsage = FairDatasetFromKartkatalogen.SpecificUsage;
            originalDataset.ProductSheetUrl = FairDatasetFromKartkatalogen.ProductSheetUrl;
            originalDataset.PresentationRulesUrl = FairDatasetFromKartkatalogen.PresentationRulesUrl;
            originalDataset.ProductSpecificationUrl = FairDatasetFromKartkatalogen.ProductSpecificationUrl;
            originalDataset.MetadataUrl = FairDatasetFromKartkatalogen.MetadataUrl;
            originalDataset.DistributionFormat = FairDatasetFromKartkatalogen.DistributionFormat;
            originalDataset.DistributionUrl = FairDatasetFromKartkatalogen.DistributionUrl;
            originalDataset.DistributionArea = FairDatasetFromKartkatalogen.DistributionArea;
            originalDataset.WmsUrl = FairDatasetFromKartkatalogen.WmsUrl;
            originalDataset.ThemeGroupId = FairDatasetFromKartkatalogen.ThemeGroupId;
            originalDataset.DatasetThumbnail = FairDatasetFromKartkatalogen.DatasetThumbnail;
            originalDataset.UuidService = FairDatasetFromKartkatalogen.UuidService;

            if (originalDataset.MetadataStatus != null)
            {
                originalDataset.MetadataStatus.StatusId = _datasetDeliveryService.GetMetadataStatus(FairDatasetFromKartkatalogen.Uuid, true, originalDataset.MetadataStatus.StatusId);
            }
            if (originalDataset.ProductSpesificationStatus != null)
            {
                originalDataset.ProductSpesificationStatus.StatusId = _registerService.GetDOKStatus(FairDatasetFromKartkatalogen.ProductSpecificationUrl, true, originalDataset.ProductSpesificationStatus.StatusId);
            }

            if (originalDataset.ProductSheetStatus == null)
            {
                originalDataset.ProductSheetStatus = CreateDatasetDelivery("notset", null, true);
            }

            if (originalDataset.PresentationRulesStatus == null)
            {
                originalDataset.PresentationRulesStatus = CreateDatasetDelivery("notset", null, true);
            }

            if (originalDataset.ProductSheetStatus != null)
            {
                originalDataset.ProductSheetStatus.StatusId = _registerService.GetDOKStatus(FairDatasetFromKartkatalogen.ProductSheetUrl, true, originalDataset.ProductSheetStatus.StatusId);
            }


            if (originalDataset.PresentationRulesStatus != null)
            {
                originalDataset.PresentationRulesStatus.StatusId = _registerService.GetDOKStatus(FairDatasetFromKartkatalogen.PresentationRulesUrl, true, originalDataset.PresentationRulesStatus.StatusId);
            }

            if (originalDataset.SosiDataStatus != null)
            {
                originalDataset.SosiDataStatus.StatusId = _registerService.GetSosiRequirements(FairDatasetFromKartkatalogen.Uuid, originalDataset.ProductSpecificationUrl, true, originalDataset.SosiDataStatus.StatusId);
            }

            if (originalDataset.GmlDataStatus != null)
            {
                originalDataset.GmlDataStatus.StatusId = _registerService.GetGmlRequirements(FairDatasetFromKartkatalogen.Uuid, true, originalDataset.GmlDataStatus.StatusId);
            }

            if (originalDataset.WmsStatus != null)
            {
                originalDataset.WmsStatus.StatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(FairDatasetFromKartkatalogen.Uuid, true, originalDataset.WmsStatus.StatusId, FairDatasetFromKartkatalogen.UuidService, FairDatasetFromKartkatalogen);
            }

            if (originalDataset.WfsStatus != null)
            {
                originalDataset.WfsStatus.StatusId = _datasetDeliveryService.GetWfsStatus(FairDatasetFromKartkatalogen.Uuid, true, originalDataset.WfsStatus.StatusId, FairDatasetFromKartkatalogen.UuidService);
            }

            if (originalDataset.AtomFeedStatus != null)
            {
                originalDataset.AtomFeedStatus.StatusId = _datasetDeliveryService.GetAtomFeedStatus(FairDatasetFromKartkatalogen.Uuid, true, originalDataset.AtomFeedStatus.StatusId);
            }

            if (originalDataset.CommonStatus != null)
            {
                originalDataset.CommonStatus.StatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(originalDataset.WfsStatus?.StatusId, originalDataset.AtomFeedStatus?.StatusId);
            }

            originalDataset.Grade = GetGrade(originalDataset);
            if(originalDataset.FairDatasetTypes != null && originalDataset.FairDatasetTypes.Count > 0) {
                foreach (var fairDatasetType in originalDataset.FairDatasetTypes.ToList())
                {
                    _dbContext.Entry(fairDatasetType).State = EntityState.Deleted;
                }
                originalDataset.FairDatasetTypes.Clear();
                _dbContext.SaveChanges();
            }
            originalDataset.FairDatasetTypes = FairDatasetFromKartkatalogen.FairDatasetTypes;

            SetFAIR(ref originalDataset);

            _dbContext.Entry(originalDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();
            _dbContext.DetachAllEntities();

            return originalDataset;
        }

        public FairDataset GetFairDatasetByUuid(string uuid)
        {
            var queryResult = from i in _dbContext.FairDatasets.Include("FairDatasetTypes")
                              where i.Uuid == uuid
                              select i;

            return queryResult.FirstOrDefault();
        }

        private void RemoveFairDatasets(List<FairDataset> FairDatasetsFromKartkatalogen)
        {
            var FairDatasetsFromRegister = GetFairDatasets();
            var exists = false;
            var removeDatasets = new List<FairDataset>();

            foreach (var FairDatasetFromRegister in FairDatasetsFromRegister)
            {
                if (FairDatasetsFromKartkatalogen.Any(FairDatasetFromKartkatalog => FairDatasetFromKartkatalog.Uuid == FairDatasetFromRegister.Uuid))
                {
                    exists = true;
                }
                if (!exists)
                {
                    removeDatasets.Add(FairDatasetFromRegister);
                }
                exists = false;
            }
            foreach (var FairDataset in removeDatasets)
            {
                DeleteFairDataset(FairDataset);
            }
        }

        private List<FairDataset> GetFairDatasets()
        {
            var queryResultsRegisterItem = from d in _dbContext.FairDatasets.Include("FairDatasetTypes")
                                           where !string.IsNullOrEmpty(d.Uuid)
                                           select d;

            var FairDatasets = queryResultsRegisterItem.ToList();
            return FairDatasets;
        }

        private List<FairDataset> FetchFairDatasets()
        {
            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/datasets?facets[0]name=nationalinitiative&facets[0]value=Mareano&facets[1]name=nationalinitiative&facets[1]value=MarineGrunnkart&limit=6000&mediatype=json&listhidden=true";
            //var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/datasets?text=041f1e6e-bdbc-4091-b48f-8a5990f3cc5b&limit=1"; //use for test 1 dataset + remove DOK add
            var client = new System.Net.WebClient { Encoding = System.Text.Encoding.UTF8 };

            List<string> datasetUuids = new List<string>();
            var fairDatasetsFromKartkatalogen = new List<FairDataset>();

            try
            {
                var dokRegister = _registerService.GetDokStatusRegister();

                foreach (var dataset in dokRegister.items.Cast<Dataset>().ToList())
                {
                    datasetUuids.Add(dataset.Uuid);
                }

                var json = client.DownloadString(url);
                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (data != null)
                {
                    var result = data.Results;

                    foreach (var item in result)
                    {
                        datasetUuids.Add(item.Uuid.ToString());
                    }
                }

                var items = datasetUuids.Distinct().ToList();

                foreach (var item in items)
                {
                    var FairDataset = _metadataService.FetchFairDatasetFromKartkatalogen(item);
                    if (FairDataset != null)
                    {
                        fairDatasetsFromKartkatalogen.Add(FairDataset);
                    }
                }


                return fairDatasetsFromKartkatalogen;
            }
            catch (Exception e)
            {
                Log.Error("Error fetching Fair datasets from Kartkatalogen, url " + url, e);
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }
        }

        public DatasetDelivery CreateDatasetDelivery(string deliveryStatusId, string note, bool autoupdate = true)
        {
            var datasetDelivery = new DatasetDelivery(deliveryStatusId, note, autoupdate);
            return datasetDelivery;
        }
    }
}