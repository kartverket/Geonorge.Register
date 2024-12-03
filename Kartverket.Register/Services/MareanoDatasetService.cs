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
using System.Net.Http;
using Resources;

namespace Kartverket.Register.Services
{
    public class MareanoDatasetService : IMareanoDatasetService
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

        public MareanoDatasetService(RegisterDbContext dbContext, IFairService fairService)
        {
            _dbContext = dbContext;
            _registerService = new RegisterService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
            _registerItemService = new RegisterItemService(_dbContext);
            _metadataService = new MetadataService(_dbContext);
            _fairService = fairService;
        }

        public MareanoDataset GetMareanoDatasetByName(string registerSeoName, string itemSeoName)
        {
            var queryResult = from i in _dbContext.MareanoDatasets
                              where i.Seoname == itemSeoName &&
                              i.Register.seoname == registerSeoName
                              select i;

            return queryResult.FirstOrDefault();
        }

        public MareanoDatasetViewModel NewMareanoDatasetViewModel(string parentregister, string registername)
        {
            var model = new MareanoDatasetViewModel() { RegisterId = _registerService.GetRegisterId(parentregister, registername) };

            return model;
        }

        public MareanoDataset NewMareanoDataset(MareanoDatasetViewModel MareanoViewModel, string parentregister, string registername)
        {
            var MareanoDataset = new MareanoDataset();

            MareanoDataset.SystemId = Guid.NewGuid();
            MareanoDataset.Name = MareanoViewModel.Name;
            MareanoDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(MareanoDataset.Name);
            MareanoDataset.Description = MareanoViewModel.Description;
            MareanoDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            MareanoDataset.OwnerId = MareanoViewModel.OwnerId;
            MareanoDataset.DateSubmitted = DateTime.Now;
            MareanoDataset.Modified = DateTime.Now;
            MareanoDataset.RegisterId = _registerService.GetRegisterId(parentregister, registername);
            MareanoDataset.VersioningId = _registerItemService.NewVersioningGroup(MareanoDataset);
            MareanoDataset.VersionNumber = 1;
            MareanoDataset.StatusId = "Submitted";

            MareanoDataset.Uuid = MareanoViewModel.Uuid;
            MareanoDataset.Notes = MareanoViewModel.Notes;
            MareanoDataset.SpecificUsage = MareanoViewModel.SpecificUsage;
            MareanoDataset.ProductSheetUrl = MareanoViewModel.ProductSheetUrl;
            MareanoDataset.PresentationRulesUrl = MareanoViewModel.PresentationRulesUrl;
            MareanoDataset.ProductSpecificationUrl = MareanoViewModel.ProductSpecificationUrl;
            MareanoDataset.MetadataUrl = MareanoViewModel.MetadataUrl;
            MareanoDataset.DistributionFormat = MareanoViewModel.DistributionFormat;
            MareanoDataset.DistributionUrl = MareanoViewModel.DistributionUrl;
            MareanoDataset.DistributionArea = MareanoViewModel.DistributionArea;
            MareanoDataset.WmsUrl = MareanoViewModel.WmsUrl;
            MareanoDataset.ThemeGroupId = MareanoViewModel.ThemeGroupId;
            MareanoDataset.DatasetThumbnail = MareanoViewModel.DatasetThumbnail;
            MareanoDataset.DokStatusId = "Proposal";
            MareanoDataset.UuidService = MareanoViewModel.UuidService;

            GetDeliveryStatuses(MareanoViewModel, MareanoDataset);

            MareanoDataset.MetadataStatusId = _datasetDeliveryService.CreateDatasetDelivery(MareanoViewModel.MetadataStatusId, MareanoViewModel.MetadataNote);
            MareanoDataset.ProductSpesificationStatusId = _datasetDeliveryService.CreateDatasetDelivery(MareanoViewModel.WmsStatusId, MareanoViewModel.WmsNote);
            MareanoDataset.SosiDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(MareanoViewModel.WmsStatusId, MareanoViewModel.WmsNote);
            MareanoDataset.GmlDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(MareanoViewModel.WmsStatusId, MareanoViewModel.WmsNote);
            MareanoDataset.WmsStatusId = _datasetDeliveryService.CreateDatasetDelivery(MareanoViewModel.WmsStatusId, MareanoViewModel.WmsNote);
            MareanoDataset.WfsStatusId = _datasetDeliveryService.CreateDatasetDelivery(MareanoViewModel.WfsStatusId, MareanoViewModel.WfsNote);
            MareanoDataset.AtomFeedStatusId = _datasetDeliveryService.CreateDatasetDelivery(MareanoViewModel.AtomFeedStatusId, MareanoViewModel.AtomFeedNote);
            MareanoDataset.CommonStatusId = _datasetDeliveryService.CreateDatasetDelivery(MareanoViewModel.WmsStatusId, MareanoViewModel.WmsNote);
            MareanoDataset.Grade = GetGrade(MareanoDataset);
            _dbContext.MareanoDatasets.Add(MareanoDataset);
            _dbContext.SaveChanges();

            return MareanoDataset;
        }

        private void GetDeliveryStatuses(MareanoDatasetViewModel MareanoDatasetViewModel, MareanoDataset MareanoDataset)
        {
            MareanoDatasetViewModel.MetadataStatusId = _datasetDeliveryService.GetMetadataStatus(MareanoDataset.Uuid, true, MareanoDatasetViewModel.MetadataStatusId);
            MareanoDatasetViewModel.ProductSpesificationStatusId = _registerService.GetDOKStatus(MareanoDataset.ProductSpecificationUrl, true, MareanoDatasetViewModel.ProductSpesificationStatusId);
            MareanoDatasetViewModel.ProductSheetStatusId = _registerService.GetDOKStatus(MareanoDataset.ProductSheetUrl, true, MareanoDatasetViewModel.ProductSpesificationStatusId);
            MareanoDatasetViewModel.PresentationRulesStatusId = _registerService.GetDOKStatus(MareanoDataset.PresentationRulesUrl, true, MareanoDatasetViewModel.PresentationRulesStatusId);
            MareanoDatasetViewModel.SosiDataStatusId = _registerService.GetSosiRequirements(MareanoDataset.Uuid, MareanoDatasetViewModel.ProductSpecificationUrl, true, MareanoDatasetViewModel.SosiDataStatusId);
            MareanoDatasetViewModel.GmlDataStatusId = _registerService.GetGmlRequirements(MareanoDataset.Uuid, true, MareanoDatasetViewModel.GmlDataStatusId);
            MareanoDatasetViewModel.WmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(MareanoDataset.Uuid, true, MareanoDatasetViewModel.WmsStatusId, MareanoDataset.UuidService);
            MareanoDatasetViewModel.WfsStatusId = _datasetDeliveryService.GetWfsStatus(MareanoDataset.Uuid, true, MareanoDatasetViewModel.WfsStatusId);
            MareanoDatasetViewModel.AtomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(MareanoDataset.Uuid, true, MareanoDatasetViewModel.AtomFeedStatusId);
            MareanoDatasetViewModel.CommonStatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(MareanoDatasetViewModel.WfsStatusId, MareanoDatasetViewModel.AtomFeedStatusId);
            if(MareanoDataset.Grade.HasValue)
                MareanoDatasetViewModel.Grade = MareanoDataset.Grade.Value;
        }

        public MareanoDataset UpdateMareanoDatasetFromKartkatalogen(MareanoDataset originalDataset)
        {
            var MareanoDatasetFromKartkatalogen = _metadataService.FetchMareanoDatasetFromKartkatalogen(originalDataset.Uuid);
            return MareanoDatasetFromKartkatalogen == null ? originalDataset : UpdateMareanoDataset(originalDataset, MareanoDatasetFromKartkatalogen);
        }

        public MareanoDataset UpdateMareanoDataset(MareanoDatasetViewModel viewModel)
        {
            var MareanoDataset = GetMareanoDatasetBySystemId(viewModel.SystemId);
            MareanoDataset.Name = viewModel.Name;
            MareanoDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(MareanoDataset.Name);
            MareanoDataset.Description = viewModel.Description;
            MareanoDataset.SubmitterId = viewModel.SubmitterId;
            MareanoDataset.OwnerId = viewModel.OwnerId;
            MareanoDataset.Modified = DateTime.Now;
            MareanoDataset.DateSubmitted = DateTime.Now;
            MareanoDataset.RegisterId = viewModel.RegisterId;

            MareanoDataset.Uuid = viewModel.Uuid;
            MareanoDataset.Notes = viewModel.Notes;
            MareanoDataset.SpecificUsage = viewModel.SpecificUsage;
            MareanoDataset.ProductSheetUrl = viewModel.ProductSheetUrl;
            MareanoDataset.PresentationRulesUrl = viewModel.PresentationRulesUrl;
            MareanoDataset.ProductSpecificationUrl = viewModel.ProductSpecificationUrl;
            MareanoDataset.MetadataUrl = viewModel.MetadataUrl;
            MareanoDataset.DistributionFormat = viewModel.DistributionFormat;
            MareanoDataset.DistributionUrl = viewModel.DistributionUrl;
            MareanoDataset.DistributionArea = viewModel.DistributionArea;
            MareanoDataset.WmsUrl = viewModel.WmsUrl;
            MareanoDataset.ThemeGroupId = viewModel.ThemeGroupId;
            MareanoDataset.DatasetThumbnail = viewModel.DatasetThumbnail;
            MareanoDataset.DokStatusId = viewModel.DokStatusId;
            MareanoDataset.DokStatusDateAccepted = viewModel.GetDateAccepted();
            MareanoDataset.UuidService = viewModel.UuidService;

            if (MareanoDataset.MetadataStatus != null)
            {
                MareanoDataset.MetadataStatus.StatusId = viewModel.MetadataStatusId;
                MareanoDataset.MetadataStatus.Note = viewModel.MetadataNote;
                MareanoDataset.MetadataStatus.AutoUpdate = viewModel.MetadataAutoUpdate;
            }

            if (MareanoDataset.ProductSpesificationStatus != null)
            {
                MareanoDataset.ProductSpesificationStatus.StatusId = viewModel.ProductSpesificationStatusId;
                MareanoDataset.ProductSpesificationStatus.Note = viewModel.ProduktspesifikasjonNote;
                MareanoDataset.ProductSpesificationStatus.AutoUpdate = viewModel.ProduktspesifikasjonAutoUpdate;
            }

            if (MareanoDataset.SosiDataStatus != null)
            {
                MareanoDataset.SosiDataStatus.StatusId = viewModel.SosiDataStatusId;
                MareanoDataset.SosiDataStatus.Note = viewModel.SosiDataNote;
                MareanoDataset.SosiDataStatus.AutoUpdate = viewModel.SosiDataAutoUpdate;
            }

            if (MareanoDataset.GmlDataStatus != null)
            {
                MareanoDataset.GmlDataStatus.StatusId = viewModel.GmlDataStatusId;
                MareanoDataset.GmlDataStatus.Note = viewModel.GmlDataNote;
                MareanoDataset.GmlDataStatus.AutoUpdate = viewModel.GmlDataAutoUpdate;
            }

            if (MareanoDataset.WmsStatus != null)
            {
                MareanoDataset.WmsStatus.StatusId = viewModel.WmsStatusId;
                MareanoDataset.WmsStatus.Note = viewModel.WmsNote;
                MareanoDataset.WmsStatus.AutoUpdate = viewModel.WmsAutoUpdate;
            }

            if (MareanoDataset.WfsStatus != null)
            {
                MareanoDataset.WfsStatus.StatusId = viewModel.WfsStatusId;
                MareanoDataset.WfsStatus.Note = viewModel.WfsNote;
                MareanoDataset.WfsStatus.AutoUpdate = viewModel.WfsAutoUpdate;
            }

            if (MareanoDataset.AtomFeedStatus != null)
            {
                MareanoDataset.AtomFeedStatus.StatusId = viewModel.AtomFeedStatusId;
                MareanoDataset.AtomFeedStatus.Note = viewModel.AtomFeedNote;
                MareanoDataset.AtomFeedStatus.AutoUpdate = viewModel.AtomFeedAutoUpdate;
            }

            if (MareanoDataset.CommonStatus != null)
            {
                MareanoDataset.CommonStatus.StatusId = viewModel.CommonStatusId;
                MareanoDataset.CommonStatus.Note = viewModel.CommonNote;
                MareanoDataset.CommonStatus.AutoUpdate = viewModel.CommonAutoUpdate;
            }

            _dbContext.Entry(MareanoDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return MareanoDataset;
        }

        private float GetGrade(MareanoDataset dataset)
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

        public void DeleteMareanoDataset(MareanoDataset MareanoDataset)
        {
            _dbContext.MareanoDatasets.Remove(MareanoDataset);

            //Todo, må slette deliveryDataset?
            _dbContext.SaveChanges();
        }

        public MareanoDataset GetMareanoDatasetById(string uuid)
        {
            Guid guid = Guid.Parse(uuid);
            var queryResult = from i in _dbContext.MareanoDatasets
                              where i.Uuid == uuid
                                    || i.SystemId == guid
                              select i;

            return queryResult.FirstOrDefault();
        }


        private MareanoDataset GetMareanoDatasetBySystemId(Guid systemId)
        {
            var queryResult = from i in _dbContext.MareanoDatasets
                              where i.SystemId == systemId
                              select i;

            return queryResult.FirstOrDefault();
        }

        public void SynchronizeMareanoDatasets()
        {
            var MareanoDatasetsFromKartkatalogen = FetchMareanoDatasetsFromKartkatalogen();
            if(MareanoDatasetsFromKartkatalogen != null && MareanoDatasetsFromKartkatalogen.Count > 0) 
            { 
                RemoveMareanoDatasets(MareanoDatasetsFromKartkatalogen);
                UpdateMareanoDataset(MareanoDatasetsFromKartkatalogen);
            _dbContext.SaveChanges();
            }
        }

        private void UpdateMareanoDataset(List<MareanoDataset> MareanoDatasetsFromKartkatalogen)
        {
            //Update register
            foreach (var MareanoDataset in MareanoDatasetsFromKartkatalogen)
            {
                try { 
                Log.Info("Start updating MareanoDataset: " + MareanoDataset.Uuid);
                _metadata = GetMetadata(MareanoDataset);

                var originalMareanoDataset = GetMareanoDatasetByUuid(MareanoDataset.Uuid);
                if (originalMareanoDataset != null)
                {
                    UpdateMareanoDataset(originalMareanoDataset, MareanoDataset);
                }
                else
                {
                    NewMareanoDatasetFromKartkatalogen(MareanoDataset);
                }
                }
                catch (Exception ex)
                {
                    Log.Error("Error updating MareanoDataset: " + MareanoDataset.Uuid, ex);
                }

                Log.Info("Finished updating MareanoDataset: " + MareanoDataset.Uuid);
            }
        }

        private MetadataModel GetMetadata(MareanoDataset mareanoDataset)
        {
            MetadataModel metadata = new MetadataModel();
            metadata.SimpleMetadata = MetadataService.FetchMetadata(mareanoDataset.Uuid);
            return metadata;
        }

        private void NewMareanoDatasetFromKartkatalogen(MareanoDataset MareanoDataset)
        {
            if (!_registerItemService.ItemNameIsValid(MareanoDataset)) return;
            MareanoDataset.SystemId = Guid.NewGuid();
            MareanoDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(MareanoDataset.Name);
            MareanoDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            MareanoDataset.DateSubmitted = DateTime.Now;
            MareanoDataset.Modified = DateTime.Now;
            MareanoDataset.RegisterId = _registerService.GetMareanoStatusRegisterId();
            MareanoDataset.VersioningId = _registerItemService.NewVersioningGroup(MareanoDataset);
            MareanoDataset.VersionNumber = 1;
            MareanoDataset.StatusId = "Submitted";
            MareanoDataset.DokStatusId = "Proposal";

            var metadataStatusId = _datasetDeliveryService.GetMetadataStatus(MareanoDataset.Uuid);
            var productSpesificationStatusId = _registerService.GetDOKStatus(MareanoDataset.ProductSpecificationUrl, true, "deficient");
            var productSheetStatusId = _registerService.GetDOKStatus(MareanoDataset.ProductSheetUrl, true, "deficient");
            var presentationRulesStatusId = _registerService.GetDOKStatus(MareanoDataset.PresentationRulesUrl, true, "deficient");
            var sosiDataStatusId = _registerService.GetSosiRequirements(MareanoDataset.Uuid, "", true, "deficient");
            var gmlDataStatusId = _registerService.GetGmlRequirements(MareanoDataset.Uuid, true, "deficient");
            var wmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(MareanoDataset.Uuid, true, "deficient", MareanoDataset.UuidService, MareanoDataset);
            var wfsStatusId = _datasetDeliveryService.GetWfsStatus(MareanoDataset.Uuid);
            var atomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(MareanoDataset.Uuid);
            var commonStatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(wfsStatusId, atomFeedStatusId);



            MareanoDataset.MetadataStatusId = _datasetDeliveryService.CreateDatasetDelivery(metadataStatusId);
            MareanoDataset.ProductSpesificationStatusId = _datasetDeliveryService.CreateDatasetDelivery(productSpesificationStatusId);
            MareanoDataset.ProductSheetStatusId = _datasetDeliveryService.CreateDatasetDelivery(productSheetStatusId);
            MareanoDataset.PresentationRulesStatusId = _datasetDeliveryService.CreateDatasetDelivery(presentationRulesStatusId);
            MareanoDataset.SosiDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(sosiDataStatusId);
            MareanoDataset.GmlDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(gmlDataStatusId);
            MareanoDataset.WmsStatusId = _datasetDeliveryService.CreateDatasetDelivery(wmsStatusId);
            MareanoDataset.WfsStatusId = _datasetDeliveryService.CreateDatasetDelivery(wfsStatusId);
            MareanoDataset.AtomFeedStatusId = _datasetDeliveryService.CreateDatasetDelivery(atomFeedStatusId);
            MareanoDataset.CommonStatusId = _datasetDeliveryService.CreateDatasetDelivery(commonStatusId);
            MareanoDataset.Grade = GetGrade(MareanoDataset);

            SetFAIR(ref MareanoDataset);

            _dbContext.MareanoDatasets.Add(MareanoDataset);
            _dbContext.SaveChanges();
            _dbContext.DetachAllEntities();
        }

        private void SetFAIR(ref MareanoDataset fairDataset)
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
            fairDataset.R2_i_Criteria = dataset.R2_i_Criteria;
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

        private MareanoDataset UpdateMareanoDataset(MareanoDataset originalDataset, MareanoDataset MareanoDatasetFromKartkatalogen)
        {
            originalDataset.Name = MareanoDatasetFromKartkatalogen.Name;
            originalDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(originalDataset.Name);
            originalDataset.Description = MareanoDatasetFromKartkatalogen.Description;
            originalDataset.OwnerId = MareanoDatasetFromKartkatalogen.OwnerId;
            originalDataset.Modified = DateTime.Now;

            originalDataset.Uuid = MareanoDatasetFromKartkatalogen.Uuid;
            originalDataset.Notes = MareanoDatasetFromKartkatalogen.Notes;
            originalDataset.SpecificUsage = MareanoDatasetFromKartkatalogen.SpecificUsage;
            originalDataset.ProductSheetUrl = MareanoDatasetFromKartkatalogen.ProductSheetUrl;
            originalDataset.PresentationRulesUrl = MareanoDatasetFromKartkatalogen.PresentationRulesUrl;
            originalDataset.ProductSpecificationUrl = MareanoDatasetFromKartkatalogen.ProductSpecificationUrl;
            originalDataset.MetadataUrl = MareanoDatasetFromKartkatalogen.MetadataUrl;
            originalDataset.DistributionFormat = MareanoDatasetFromKartkatalogen.DistributionFormat;
            originalDataset.DistributionUrl = MareanoDatasetFromKartkatalogen.DistributionUrl;
            originalDataset.DistributionArea = MareanoDatasetFromKartkatalogen.DistributionArea;
            originalDataset.WmsUrl = MareanoDatasetFromKartkatalogen.WmsUrl;
            originalDataset.ThemeGroupId = MareanoDatasetFromKartkatalogen.ThemeGroupId;
            originalDataset.DatasetThumbnail = MareanoDatasetFromKartkatalogen.DatasetThumbnail;
            originalDataset.UuidService = MareanoDatasetFromKartkatalogen.UuidService;

            if (originalDataset.MetadataStatus != null)
            {
                originalDataset.MetadataStatus.StatusId = _datasetDeliveryService.GetMetadataStatus(MareanoDatasetFromKartkatalogen.Uuid, true, originalDataset.MetadataStatus.StatusId);
            }
            if (originalDataset.ProductSpesificationStatus != null)
            {
                originalDataset.ProductSpesificationStatus.StatusId = _registerService.GetDOKStatus(MareanoDatasetFromKartkatalogen.ProductSpecificationUrl, true, originalDataset.ProductSpesificationStatus.StatusId);
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
                originalDataset.ProductSheetStatus.StatusId = _registerService.GetDOKStatus(MareanoDatasetFromKartkatalogen.ProductSheetUrl, true, originalDataset.ProductSheetStatus.StatusId);
            }


            if (originalDataset.PresentationRulesStatus != null)
            {
                originalDataset.PresentationRulesStatus.StatusId = _registerService.GetDOKStatus(MareanoDatasetFromKartkatalogen.PresentationRulesUrl, true, originalDataset.PresentationRulesStatus.StatusId);
            }

            if (originalDataset.SosiDataStatus != null)
            {
                originalDataset.SosiDataStatus.StatusId = _registerService.GetSosiRequirements(MareanoDatasetFromKartkatalogen.Uuid, originalDataset.ProductSpecificationUrl, true, originalDataset.SosiDataStatus.StatusId);
            }

            if (originalDataset.GmlDataStatus != null)
            {
                originalDataset.GmlDataStatus.StatusId = _registerService.GetGmlRequirements(MareanoDatasetFromKartkatalogen.Uuid, true, originalDataset.GmlDataStatus.StatusId);
            }

            if (originalDataset.WmsStatus != null)
            {
                originalDataset.WmsStatus.StatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(MareanoDatasetFromKartkatalogen.Uuid, true, originalDataset.WmsStatus.StatusId, MareanoDatasetFromKartkatalogen.UuidService, MareanoDatasetFromKartkatalogen);
            }

            if (originalDataset.WfsStatus != null)
            {
                originalDataset.WfsStatus.StatusId = _datasetDeliveryService.GetWfsStatus(MareanoDatasetFromKartkatalogen.Uuid, true, originalDataset.WfsStatus.StatusId, MareanoDatasetFromKartkatalogen.UuidService);
            }

            if (originalDataset.AtomFeedStatus != null)
            {
                originalDataset.AtomFeedStatus.StatusId = _datasetDeliveryService.GetAtomFeedStatus(MareanoDatasetFromKartkatalogen.Uuid, true, originalDataset.AtomFeedStatus.StatusId);
            }

            if (originalDataset.CommonStatus != null)
            {
                originalDataset.CommonStatus.StatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(originalDataset.WfsStatus?.StatusId, originalDataset.AtomFeedStatus?.StatusId);
            }

            originalDataset.Grade = GetGrade(originalDataset);

            SetFAIR(ref originalDataset);

            _dbContext.Entry(originalDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();
            _dbContext.DetachAllEntities();

            return originalDataset;
        }

        public MareanoDataset GetMareanoDatasetByUuid(string uuid)
        {
            var queryResult = from i in _dbContext.MareanoDatasets
                              where i.Uuid == uuid
                              select i;

            return queryResult.FirstOrDefault();
        }

        private void RemoveMareanoDatasets(List<MareanoDataset> MareanoDatasetsFromKartkatalogen)
        {
            var MareanoDatasetsFromRegister = GetMareanoDatasets();
            var exists = false;
            var removeDatasets = new List<MareanoDataset>();

            foreach (var MareanoDatasetFromRegister in MareanoDatasetsFromRegister)
            {
                if (MareanoDatasetsFromKartkatalogen.Any(MareanoDatasetFromKartkatalog => MareanoDatasetFromKartkatalog.Uuid == MareanoDatasetFromRegister.Uuid))
                {
                    exists = true;
                }
                if (!exists)
                {
                    removeDatasets.Add(MareanoDatasetFromRegister);
                }
                exists = false;
            }
            foreach (var MareanoDataset in removeDatasets)
            {
                DeleteMareanoDataset(MareanoDataset);
            }
        }

        private List<MareanoDataset> GetMareanoDatasets()
        {
            var queryResultsRegisterItem = from d in _dbContext.MareanoDatasets
                                           where !string.IsNullOrEmpty(d.Uuid)
                                           select d;

            var MareanoDatasets = queryResultsRegisterItem.ToList();
            return MareanoDatasets;
        }

        private List<MareanoDataset> FetchMareanoDatasetsFromKartkatalogen()
        {
            var MareanoDatasetsFromKartkatalogen = new List<MareanoDataset>();

            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/datasets?facets%5b0%5dname=nationalinitiative&facets%5b0%5dvalue=Mareano&limit=6000&mediatype=json&listhidden=true";
            var c = new System.Net.WebClient { Encoding = System.Text.Encoding.UTF8 };
            try
            {
                var json = c.DownloadString(url);
                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (data != null)
                {
                    var result = data.Results;

                    foreach (var item in result)
                    {
                        var MareanoDataset = _metadataService.FetchMareanoDatasetFromKartkatalogen(item.Uuid.ToString());
                        if (MareanoDataset != null)
                        {
                            MareanoDatasetsFromKartkatalogen.Add(MareanoDataset);
                        }
                    }
                }
                return MareanoDatasetsFromKartkatalogen;
            }
            catch (Exception e)
            {
                Log.Error("Error fetching Mareano datasets from Kartkatalogen, url " + url, e);
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

    public class MetadataModel
    {
        public GeoNorgeAPI.SimpleMetadata SimpleMetadata { get; set; }
    }
}