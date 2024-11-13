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
        MetadataModel _metadata;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly HttpClient _httpClient = new HttpClient();

        public FairDatasetService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerService = new RegisterService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
            _registerItemService = new RegisterItemService(_dbContext);
            _metadataService = new MetadataService(_dbContext);
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

        private void SetFAIR(ref FairDataset FairDataset)
        {
            FairDataset.I1_c_Criteria = null;
            FairDataset.I3_a_Criteria = null;
            FairDataset.I3_b_Criteria = null;

            int findableWeight = 0;

            if (_metadata?.SimpleMetadata == null)
                return;

            FairDataset.F2_a_Criteria = SimpleKeyword.Filter(_metadata.SimpleMetadata.Keywords.ToList(), SimpleKeyword.TYPE_THEME, null)?.Count() >= 3;
            FairDataset.F2_b_Criteria = _metadata.SimpleMetadata.Title.Count() <= 105;
            FairDataset.F2_c_Criteria = _metadata.SimpleMetadata.Abstract?.Count() >= 200;
            FairDataset.F2_d_Criteria = _metadata.SimpleMetadata.BoundingBox != null;
            FairDataset.F2_e_Criteria = _metadata.SimpleMetadata.DateCreated != null;
            FairDataset.F3_a_Criteria = _metadata.SimpleMetadata.ResourceReference != null ? _metadata.SimpleMetadata.ResourceReference?.Code != null && _metadata.SimpleMetadata.ResourceReference?.Codespace != null : false;

            if (FairDataset.F1_a_Criteria) findableWeight += 20;
            if (FairDataset.F2_a_Criteria) findableWeight += 10;
            if (FairDataset.F2_b_Criteria) findableWeight += 5;
            if (FairDataset.F2_c_Criteria) findableWeight += 10;
            if (FairDataset.F2_d_Criteria) findableWeight += 10;
            if (FairDataset.F2_e_Criteria) findableWeight += 5; 
            if (FairDataset.F3_a_Criteria) findableWeight += 20;
            if (FairDataset.F4_a_Criteria) findableWeight += 20;

            FairDataset.FindableStatusPerCent = findableWeight;
            FairDataset.FindableStatusId = CreateFairDelivery(findableWeight);

            int accesibleWeight = 0;

            FairDataset.A1_a_Criteria = CheckWfs(FairDataset.Uuid, FairDataset.WfsStatus);
            FairDataset.A1_b_Criteria = CheckWms(FairDataset.Uuid, FairDataset.WmsStatus);
            FairDataset.A1_c_Criteria = _metadata.SimpleMetadata?.DistributionsFormats != null ? _metadata.SimpleMetadata.DistributionsFormats.Where(p => !string.IsNullOrEmpty(p.Protocol) && p.Protocol.Contains("GEONORGE:DOWNLOAD")).Any() : false;
            FairDataset.A1_d_Criteria = FairDataset.AtomFeedStatus != null ? FairDataset.AtomFeedStatus.IsGood() : false;
            FairDataset.A1_e_Criteria = CheckDistributionUrl(FairDataset.Uuid, _metadata.SimpleMetadata.DistributionsFormats.Where(f => !string.IsNullOrEmpty(f.Protocol) && f.Protocol.Contains("GEONORGE:DOWNLOAD") || !string.IsNullOrEmpty(f.Protocol) && f.Protocol.Contains("WWW:DOWNLOAD") || !string.IsNullOrEmpty(f.Protocol) && f.Protocol.Contains("GEONORGE:FILEDOWNLOAD")));
            FairDataset.A1_f_Criteria = true;

            if (FairDataset.A1_a_Criteria) accesibleWeight += 15;
            if (FairDataset.A1_b_Criteria) accesibleWeight += 15;
            if (FairDataset.A1_c_Criteria) accesibleWeight += 15;
            if (FairDataset.A1_d_Criteria) accesibleWeight += 5;
            if (FairDataset.A1_e_Criteria) accesibleWeight += 50;
            if (FairDataset.A1_f_Criteria) accesibleWeight += 0;
            if (FairDataset.A2_a_Criteria) accesibleWeight += 0;

            FairDataset.AccesibleStatusPerCent = accesibleWeight;
            FairDataset.AccesibleStatusId = CreateFairDelivery(accesibleWeight);

            int interoperableWeight = 0;

            var spatialRepresentation = _metadata.SimpleMetadata.SpatialRepresentation;
            if (spatialRepresentation == "vector")
            {
                FairDataset.I1_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "GML" || p.FormatName == "GeoJSON" || p.FormatName == "JSON-FG" || p.FormatName == "JSON-LD" || p.FormatName == "GeoPackage" || p.FormatName == "COPC" || p.FormatName == "GeoParquet" || p.FormatName == "Shape").Any();
                if (!FairDataset.I1_b_Criteria)
                    FairDataset.I1_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "NetCDF-CF").Any();
            }
            else if (spatialRepresentation == "grid")
            {
                FairDataset.I1_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "GeoTIFF" || p.FormatName == "TIFF" || p.FormatName == "JPEG" || p.FormatName == "JPEG2000" || p.FormatName == "GeoPackage" || p.FormatName == "COG" || p.FormatName == "COPC").Any();
                if (!FairDataset.I1_b_Criteria)
                    FairDataset.I1_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "NetCDF-CF").Any();
            }
            if (spatialRepresentation != "grid")
                FairDataset.I1_c_Criteria = _metadata.SimpleMetadata.QualitySpecifications != null && _metadata.SimpleMetadata.QualitySpecifications.Count > 0
                                            ? _metadata.SimpleMetadata.QualitySpecifications.Where(r => !string.IsNullOrEmpty(r.Explanation) && r.Explanation.StartsWith("GML-filer er i henhold")).Any() : false;
            FairDataset.I2_a_Criteria = _metadata.SimpleMetadata.Keywords.Where(k => !string.IsNullOrEmpty(k.Thesaurus)).ToList().Count() >= 1;
            FairDataset.I2_b_Criteria = SimpleKeyword.Filter(_metadata.SimpleMetadata.Keywords, null, SimpleKeyword.THESAURUS_NATIONAL_THEME).ToList().Count() >= 1;
            if (spatialRepresentation == "vector")
            {
                FairDataset.I3_a_Criteria = _metadata.SimpleMetadata.QualitySpecifications != null && _metadata.SimpleMetadata.QualitySpecifications.Count > 0
                                            ? _metadata.SimpleMetadata.QualitySpecifications.Where(r => !string.IsNullOrEmpty(r.Title) && r.Title.Contains("SOSI produktspesifikasjon")).Any() : false;
                if(!FairDataset.I3_a_Criteria.HasValue && !FairDataset.I3_a_Criteria.Value) 
                { 
                    FairDataset.I3_a_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata.ApplicationSchema);
                }
                FairDataset.I3_b_Criteria = _metadata.SimpleMetadata.QualitySpecifications != null && _metadata.SimpleMetadata.QualitySpecifications.Count > 0
                                            ? _metadata.SimpleMetadata.QualitySpecifications.Where(r => !string.IsNullOrEmpty(r.Explanation) && r.Explanation.Contains("er i henhold")).Any() : false;
            }
            else 
            {
                FairDataset.I3_a_Criteria = true;
                FairDataset.I3_b_Criteria = true;
            }
            if (FairDataset.I1_a_Criteria) interoperableWeight += 20;
            if (FairDataset.I1_b_Criteria) interoperableWeight += 10;
            if (!FairDataset.I1_c_Criteria.HasValue || (FairDataset.I1_c_Criteria.HasValue && FairDataset.I1_c_Criteria.Value)) interoperableWeight += 20;
            if (FairDataset.I2_a_Criteria) interoperableWeight += 10;
            if (FairDataset.I2_b_Criteria) interoperableWeight += 10;
            if (!FairDataset.I3_a_Criteria.HasValue || (FairDataset.I3_a_Criteria.HasValue && FairDataset.I3_a_Criteria.Value)) interoperableWeight += 10;
            if (!FairDataset.I3_b_Criteria.HasValue || (FairDataset.I3_b_Criteria.HasValue && FairDataset.I3_b_Criteria.Value)) interoperableWeight += 20;

            FairDataset.InteroperableStatusPerCent = interoperableWeight;
            FairDataset.InteroperableStatusId = CreateFairDelivery(interoperableWeight);

            int reusableWeight = 0;

            FairDataset.R1_a_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata.Constraints?.UseConstraintsLicenseLink);
            FairDataset.R1_b_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata.Constraints?.AccessConstraintsLink);
            FairDataset.R2_a_Criteria = _metadata.SimpleMetadata?.ProcessHistory.Count() > 200;
            FairDataset.R2_b_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.MaintenanceFrequency);
            FairDataset.R2_c_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.ProductSpecificationUrl);
            FairDataset.R2_d_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.ResolutionScale);
            FairDataset.R2_e_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.CoverageUrl)
                                           || !string.IsNullOrEmpty(_metadata.SimpleMetadata?.CoverageGridUrl)
                                           || !string.IsNullOrEmpty(_metadata.SimpleMetadata?.CoverageCellUrl);

            FairDataset.R2_f_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.SpecificUsage);

            FairDataset.R2_g_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.ContactMetadata?.Organization);
            FairDataset.R2_h_Criteria = !string.IsNullOrEmpty(_metadata.SimpleMetadata?.ContactPublisher?.Organization);

            FairDataset.R3_b_Criteria = _metadata.SimpleMetadata.DistributionsFormats.Where(p => p.FormatName == "GML" || p.FormatName == "GeoTIFF" || p.FormatName == "TIFF" || p.FormatName == "JPEG" || p.FormatName == "JPEG2000" || p.FormatName == "NetCDF" || p.FormatName == "NetCDF-CF").Any();

            if (FairDataset.R1_a_Criteria) reusableWeight += 30;
            if (FairDataset.R1_b_Criteria) reusableWeight += 10; 
            if (FairDataset.R2_a_Criteria) reusableWeight += 10;
            if (FairDataset.R2_b_Criteria) reusableWeight += 5;
            if (FairDataset.R2_c_Criteria) reusableWeight += 10;
            if (FairDataset.R2_d_Criteria) reusableWeight += 5;
            if (FairDataset.R2_e_Criteria) reusableWeight += 5;
            if (FairDataset.R2_f_Criteria) reusableWeight += 5;
            if (FairDataset.R2_g_Criteria) reusableWeight += 5;
            if (FairDataset.R2_h_Criteria) reusableWeight += 5;
            if (FairDataset.R3_a_Criteria) reusableWeight += 5;
            if (FairDataset.R3_b_Criteria) reusableWeight += 5;

            FairDataset.ReUseableStatusPerCent = reusableWeight;
            FairDataset.ReUseableStatusId = CreateFairDelivery(reusableWeight);

            int fairWeight = (findableWeight + accesibleWeight + interoperableWeight + reusableWeight) / 4;
            FairDataset.FAIRStatusPerCent = fairWeight;
            FairDataset.FAIRStatusId = CreateFairDelivery(fairWeight);

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
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("Maps"))
                            return true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("Tiles"))
                            return true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("Styles"))
                            return true;
                    }

                    if (hasWms || hasWMTS)
                        return true;
                }
            }

            return false;
        }

        private bool CheckWfs(string uuid, DatasetDelivery datasetDelivery)
        {
            if (datasetDelivery != null)
            {
                bool hasWfs = datasetDelivery.IsGoodOrUseable();
                bool hasWcs = false;
                var distros = GetDistributions(uuid);
                if (distros != null)
                {
                    foreach (var distro in distros)
                    {
                        string protocol = distro?.Protocol;
                        if (!string.IsNullOrEmpty(protocol) && protocol.Contains("WCS"))
                            hasWcs = true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("Features"))
                            return true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("Coverages"))
                            return true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("EDR"))
                            return true;
                        else if (!string.IsNullOrEmpty(protocol) && protocol.Contains("OpenDAP"))
                            return true;
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
            if (distributions != null && distributions.Count() > 0)
            {
                var url = distributions.FirstOrDefault().URL;
                var protocol = distributions.FirstOrDefault().Protocol;

                if (!string.IsNullOrEmpty(url) && (url.StartsWith("https://")))
                {
                    try
                    {
                        if(protocol == "GEONORGE:DOWNLOAD")
                            url = url + uuid;

                        _httpClient.DefaultRequestHeaders.Accept.Clear();
                        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                        Log.Debug("Connecting to: " + url);

                        HttpResponseMessage response = _httpClient.GetAsync(new Uri(url)).Result;

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            Log.Error("Url svarer ikke: " + url + " , statuskode: " + response.StatusCode);
                        }
                        else
                            return true;

                    }
                    catch (Exception ex)
                    {
                        Log.Error("Url svarer ikke: " + url, ex);
                        return false;
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