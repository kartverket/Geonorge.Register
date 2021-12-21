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

namespace Kartverket.Register.Services
{
    public class MareanoDatasetService : IMareanoDatasetService
    {
        private readonly RegisterDbContext _dbContext;
        private readonly IRegisterService _registerService;
        private readonly IDatasetDeliveryService _datasetDeliveryService;
        private readonly IRegisterItemService _registerItemService;
        private readonly MetadataService _metadataService;
        MetadataModel _metadata;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MareanoDatasetService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerService = new RegisterService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
            _registerItemService = new RegisterItemService(_dbContext);
            _metadataService = new MetadataService(_dbContext);
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

            if (ProductSpesificationStatus != null && dataset.ProductSpesificationStatus.IsGood())
                grade = grade + gradeGood;
            else if (ProductSpesificationStatus != null && dataset.ProductSpesificationStatus.IsGoodOrUseable())
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
            RemoveMareanoDatasets(MareanoDatasetsFromKartkatalogen);
            UpdateMareanoDataset(MareanoDatasetsFromKartkatalogen);


            _dbContext.SaveChanges();
        }

        private void UpdateMareanoDataset(List<MareanoDataset> MareanoDatasetsFromKartkatalogen)
        {
            //Update register
            foreach (var MareanoDataset in MareanoDatasetsFromKartkatalogen)
            {
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
            var wmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(MareanoDataset.Uuid, true, "deficient", MareanoDataset.UuidService);
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
        }

        private void SetFAIR(ref MareanoDataset mareanoDataset)
        {
            mareanoDataset.I1_c_Criteria = null;
            mareanoDataset.I3_a_Criteria = null;
            mareanoDataset.I3_b_Criteria = null;

            int findableWeight = 0;

            if (_metadata?.SimpleMetadata == null)
                return;

            mareanoDataset.F2_a_Criteria = SimpleKeyword.Filter(_metadata.SimpleMetadata.Keywords, SimpleKeyword.TYPE_THEME, null).ToList().Count() >= 3;
            mareanoDataset.F2_b_Criteria = _metadata.SimpleMetadata.Title.Count() <= 100;
            mareanoDataset.F2_c_Criteria = _metadata.SimpleMetadata.Abstract?.Count() >= 200 && _metadata.SimpleMetadata.Abstract?.Count() <= 600;
            mareanoDataset.F3_a_Criteria = _metadata.SimpleMetadata.ResourceReference != null ?_metadata.SimpleMetadata.ResourceReference?.Code != null && _metadata.SimpleMetadata.ResourceReference?.Codespace != null : false;

            if (mareanoDataset.F1_a_Criteria) findableWeight += 25;
            if (mareanoDataset.F2_a_Criteria) findableWeight += 10;
            if (mareanoDataset.F2_b_Criteria) findableWeight += 5;
            if (mareanoDataset.F2_c_Criteria) findableWeight += 10;
            if (mareanoDataset.F3_a_Criteria) findableWeight += 25;
            if (mareanoDataset.F4_a_Criteria) findableWeight += 25;

            mareanoDataset.FindableStatusPerCent = findableWeight;
            mareanoDataset.FindableStatusId = CreateFairDelivery(findableWeight);

            int accesibleWeight = 0;

            mareanoDataset.A1_a_Criteria = CheckWfs(mareanoDataset.Uuid, mareanoDataset.WfsStatus);
            mareanoDataset.A1_b_Criteria = CheckWms(mareanoDataset.Uuid, mareanoDataset.WmsStatus);
            mareanoDataset.A1_c_Criteria = _metadata.SimpleMetadata?.DistributionsFormats != null ? _metadata.SimpleMetadata.DistributionsFormats.Where(p => !string.IsNullOrEmpty(p.Protocol) && p.Protocol.Contains("GEONORGE:DOWNLOAD")).Any() : false;
            mareanoDataset.A1_d_Criteria = mareanoDataset.AtomFeedStatus != null ? mareanoDataset.AtomFeedStatus.IsGood() : false;
            mareanoDataset.A1_e_Criteria = CheckDistributionUrl(mareanoDataset.Uuid, _metadata.SimpleMetadata.DistributionsFormats.Where(f => !string.IsNullOrEmpty(f.Protocol) && f.Protocol.Contains("GEONORGE:DOWNLOAD") || !string.IsNullOrEmpty(f.Protocol) && f.Protocol.Contains("WWW:DOWNLOAD") || !string.IsNullOrEmpty(f.Protocol) && f.Protocol.Contains("GEONORGE:FILEDOWNLOAD")));
            mareanoDataset.A1_f_Criteria = true;

            if (mareanoDataset.A1_a_Criteria) accesibleWeight += 15;
            if (mareanoDataset.A1_b_Criteria) accesibleWeight += 15;
            if (mareanoDataset.A1_c_Criteria) accesibleWeight += 15;
            if (mareanoDataset.A1_d_Criteria) accesibleWeight += 5;
            if (mareanoDataset.A1_e_Criteria) accesibleWeight += 40;
            if (mareanoDataset.A1_f_Criteria) accesibleWeight += 10;
            if (mareanoDataset.A2_a_Criteria) accesibleWeight += 0;

            mareanoDataset.AccesibleStatusPerCent = accesibleWeight;
            mareanoDataset.AccesibleStatusId = CreateFairDelivery(accesibleWeight);

            int interoperableWeight = 0;

            var spatialRepresentation = _metadata.SimpleMetadata.SpatialRepresentation;
            if(spatialRepresentation == "vector")
                mareanoDataset.I1_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "GML").Any();
            else if(spatialRepresentation == "grid") 
                mareanoDataset.I1_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "GeoTIFF" || p.FormatName == "TIFF" || p.FormatName == "JPEG" || p.FormatName == "JPEG2000").Any();

            if (spatialRepresentation != "grid") 
                mareanoDataset.I1_c_Criteria = _metadata.SimpleMetadata.QualitySpecifications != null 
                                            ? _metadata.SimpleMetadata.QualitySpecifications.Where(r => r.Responsible == "uml-gml" && r.Result.HasValue && r.Result.Value == true).Any() : false;
            mareanoDataset.I2_a_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata.TopicCategory);
            mareanoDataset.I2_b_Criteria = SimpleKeyword.Filter(_metadata.SimpleMetadata.Keywords, null, SimpleKeyword.THESAURUS_NATIONAL_THEME).ToList().Count() >= 1;
            if (spatialRepresentation != "grid") { 
                mareanoDataset.I3_a_Criteria = SimpleKeyword.Filter(_metadata.SimpleMetadata.Keywords, null, SimpleKeyword.THESAURUS_CONCEPT).ToList().Count() >= 1;
                mareanoDataset.I3_b_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata.ApplicationSchema);
            }
            if (mareanoDataset.I1_a_Criteria) interoperableWeight += 20;
            if (mareanoDataset.I1_b_Criteria) interoperableWeight += 10;
            if (!mareanoDataset.I1_c_Criteria.HasValue || (mareanoDataset.I1_c_Criteria.HasValue && mareanoDataset.I1_c_Criteria.Value)) interoperableWeight += 20;
            if (mareanoDataset.I2_a_Criteria) interoperableWeight += 10;
            if (mareanoDataset.I2_b_Criteria) interoperableWeight += 10;
            if (!mareanoDataset.I3_a_Criteria.HasValue || (mareanoDataset.I3_a_Criteria.HasValue && mareanoDataset.I3_a_Criteria.Value)) interoperableWeight += 10;
            if (!mareanoDataset.I3_b_Criteria.HasValue || (mareanoDataset.I3_b_Criteria.HasValue && mareanoDataset.I3_b_Criteria.Value)) interoperableWeight += 20;

            mareanoDataset.InteroperableStatusPerCent = interoperableWeight;
            mareanoDataset.InteroperableStatusId = CreateFairDelivery(interoperableWeight);

            int reusableWeight = 0;

            mareanoDataset.R1_a_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata.Constraints?.UseConstraintsLicenseLink);
            mareanoDataset.R2_a_Criteria = _metadata.SimpleMetadata?.ProcessHistory.Count() > 200;
            mareanoDataset.R2_b_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.MaintenanceFrequency);
            mareanoDataset.R2_c_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.ProductSpecificationUrl);
            mareanoDataset.R2_d_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.ResolutionScale);
            mareanoDataset.R2_e_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.CoverageUrl) 
                                           || !string.IsNullOrEmpty(_metadata.SimpleMetadata?.CoverageGridUrl) 
                                           || !string.IsNullOrEmpty(_metadata.SimpleMetadata?.CoverageCellUrl);

            mareanoDataset.R2_f_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.Purpose);
            mareanoDataset.R3_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "GML" || p.FormatName == "GeoTIFF" || p.FormatName == "TIFF" || p.FormatName == "JPEG" || p.FormatName == "JPEG2000" || p.FormatName == "NetCDF" || p.FormatName == "NetCDF-CF").Any();

            if (mareanoDataset.R1_a_Criteria) reusableWeight += 30;
            if (mareanoDataset.R2_a_Criteria) reusableWeight += 10;
            if (mareanoDataset.R2_b_Criteria) reusableWeight += 5;
            if (mareanoDataset.R2_c_Criteria) reusableWeight += 10;
            if (mareanoDataset.R2_d_Criteria) reusableWeight += 5;
            if (mareanoDataset.R2_e_Criteria) reusableWeight += 5;
            if (mareanoDataset.R2_f_Criteria) reusableWeight += 5;
            if (mareanoDataset.R3_a_Criteria) reusableWeight += 15;
            if (mareanoDataset.R3_b_Criteria) reusableWeight += 15;

            mareanoDataset.ReUseableStatusPerCent = reusableWeight;
            mareanoDataset.ReUseableStatusId = CreateFairDelivery(reusableWeight);

            int fairWeight = (findableWeight + accesibleWeight + interoperableWeight + reusableWeight) / 4;
            mareanoDataset.FAIRStatusPerCent = fairWeight;
            mareanoDataset.FAIRStatusId = CreateFairDelivery(fairWeight);

        }

        private bool CheckWms(string uuid, DatasetDelivery datasetDelivery)
        {
            if (datasetDelivery != null)
            {
                bool hasWms = datasetDelivery.IsGoodOrUseable();
                bool hasWMTS = false;
                var distros = GetDistributions(uuid);
                if (distros != null)
                {
                    foreach (var distro in distros)
                    {
                        string protocol = distro?.Protocol;
                        if (!string.IsNullOrEmpty(protocol) && protocol.Contains("WMTS"))
                            hasWMTS = true;
                    }

                    if (hasWms || hasWMTS)
                        return true;
                }
            }

            return false;
        }

        private bool CheckWfs(string uuid, DatasetDelivery datasetDelivery)
        {
            if(datasetDelivery != null)
            {
                bool hasWfs = datasetDelivery.IsGoodOrUseable();
                bool hasWcs = false;
                var distros = GetDistributions(uuid);
                if(distros != null)
                {
                    foreach(var distro in distros)
                    {
                        string protocol = distro?.Protocol;
                        if (!string.IsNullOrEmpty(protocol) && protocol.Contains("WCS"))
                            hasWcs = true;
                    }

                    if (hasWfs || hasWcs)
                        return true;
                }
            }

            return false;
        }

        public static dynamic GetDistributions(string metadataUuid)
        {
            try
            {
                var metadataUrl = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/distributions/" + metadataUuid;
                var c = new WebClient { Encoding = System.Text.Encoding.UTF8 };

                var json = c.DownloadString(metadataUrl);

                dynamic metadata = Newtonsoft.Json.Linq.JArray.Parse(json);
                return metadata;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool CheckDistributionUrl(string uuid, IEnumerable<SimpleDistribution> distributions)
        {
            if(distributions != null && distributions.Count() > 0)
            {
                var url = distributions.FirstOrDefault().URL;
                var protocol = distributions.FirstOrDefault().Protocol;

                if (!string.IsNullOrEmpty(url) && (url.StartsWith("https://")))
                {
                    var downloadServiceUrl = url;
                    try {
                        downloadServiceUrl = downloadServiceUrl + uuid;
                        var c = new System.Net.WebClient();

                        var file = c.DownloadString(downloadServiceUrl);
                        return true;
                        }
                    catch (Exception ex) {
                        Log.Error("Error downloading " + downloadServiceUrl + " , error: " + ex.Message);
                    }
                }
            }

            return false;
        }

        public Guid CreateFairDelivery(int weight, string fairStatusId = null, string note = "", bool autoupdate = true)
        {
            if (string.IsNullOrEmpty(fairStatusId))
            {
                if (weight > 90)
                    fairStatusId = FAIRDelivery.Good;
                else if (weight >= 75 && weight <= 90)
                    fairStatusId = FAIRDelivery.Satisfactory;
                else if (weight < 75 && weight >= 50)
                    fairStatusId = FAIRDelivery.Useable;
                else
                    fairStatusId = FAIRDelivery.Deficient;
            }

            var fairDelivery = new FAIRDelivery(fairStatusId, note, autoupdate);
            _dbContext.FAIRDeliveries.Add(fairDelivery);
            _dbContext.SaveChanges();
            return fairDelivery.FAIRDeliveryId;
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
                originalDataset.WmsStatus.StatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(MareanoDatasetFromKartkatalogen.Uuid, true, originalDataset.WmsStatus.StatusId, MareanoDatasetFromKartkatalogen.UuidService);
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

            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/datasets?facets%5b0%5dname=nationalinitiative&facets%5b0%5dvalue=Mareano&limit=6000&mediatype=json";
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

    internal class MetadataModel
    {
        public GeoNorgeAPI.SimpleMetadata SimpleMetadata { get; set; }
    }
}