using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Configuration;
using Kartverket.DOK.Service;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services.RegisterItem;

namespace Kartverket.Register.Services
{
    public class InspireDatasetService : IInspireDatasetService
    {
        private readonly RegisterDbContext _dbContext;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;
        private readonly IDatasetDeliveryService _datasetDeliveryService;
        private readonly MetadataService _metadataService;

        public InspireDatasetService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerService = new RegisterService(_dbContext);
            _registerItemService = new RegisterItemService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
            _metadataService = new MetadataService();
        }

        public InspireDataset NewInspireDataset(InspireDatasetViewModel inspireDatasetViewModel, string parentregister, string registername)
        {
            var inspireDataset = new InspireDataset();

            inspireDataset.SystemId = Guid.NewGuid();
            inspireDataset.Name = inspireDatasetViewModel.Name;
            inspireDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(inspireDataset.Name);
            inspireDataset.Description = inspireDatasetViewModel.Description;
            inspireDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            inspireDataset.OwnerId = inspireDatasetViewModel.OwnerId;
            inspireDataset.DateSubmitted = DateTime.Now;
            inspireDataset.Modified = DateTime.Now;
            inspireDataset.RegisterId = _registerService.GetRegisterId(parentregister, registername);
            inspireDataset.VersioningId = _registerItemService.NewVersioningGroup(inspireDataset);
            inspireDataset.VersionNumber = 1;
            inspireDataset.StatusId = "Submitted";

            inspireDataset.Uuid = inspireDatasetViewModel.Uuid;
            inspireDataset.Notes = inspireDatasetViewModel.Notes;
            inspireDataset.SpecificUsage = inspireDatasetViewModel.SpecificUsage;
            inspireDataset.ProductSheetUrl = inspireDatasetViewModel.ProductSheetUrl;
            inspireDataset.PresentationRulesUrl = inspireDatasetViewModel.PresentationRulesUrl;
            inspireDataset.ProductSpecificationUrl = inspireDatasetViewModel.ProductSpecificationUrl;
            inspireDataset.MetadataUrl = inspireDatasetViewModel.MetadataUrl;
            inspireDataset.DistributionFormat = inspireDatasetViewModel.DistributionFormat;
            inspireDataset.DistributionUrl = inspireDatasetViewModel.DistributionUrl;
            inspireDataset.DistributionArea = inspireDatasetViewModel.DistributionArea;
            inspireDataset.WmsUrl = inspireDatasetViewModel.WmsUrl;
            inspireDataset.ThemeGroupId = inspireDatasetViewModel.ThemeGroupId;
            inspireDataset.DatasetThumbnail = inspireDatasetViewModel.DatasetThumbnail;
            inspireDataset.DokStatusId = "Proposal";
            inspireDataset.UuidService = inspireDatasetViewModel.UuidService;

            GetDeliveryStatuses(inspireDatasetViewModel, inspireDataset);

            inspireDataset.InspireTheme = inspireDatasetViewModel.InspireTheme;
            inspireDataset.InspireDeliveryMetadataId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.MetadataStatusId, inspireDatasetViewModel.MetadataNote);
            inspireDataset.InspireDeliveryMetadataServiceId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.MetadataServiceStatusId, inspireDatasetViewModel.MetadataServiceNote);
            inspireDataset.InspireDeliveryDistributionId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.DistributionStatusId, inspireDatasetViewModel.DistributionNote);
            inspireDataset.InspireDeliveryWmsId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.WmsStatusId, inspireDatasetViewModel.WmsNote);
            inspireDataset.InspireDeliveryWfsId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.WfsStatusId, inspireDatasetViewModel.WfsNote);
            inspireDataset.InspireDeliveryAtomFeedId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.AtomFeedStatusId, inspireDatasetViewModel.AtomFeedNote);
            inspireDataset.InspireDeliveryWfsOrAtomId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.WfsOrAtomStatusId, inspireDatasetViewModel.WfsOrAtomNote);
            inspireDataset.InspireDeliveryHarmonizedDataId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.HarmonizedDataStatusId, inspireDatasetViewModel.HarmonizedDataNote);
            inspireDataset.InspireDeliverySpatialDataServiceId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.SpatialDataServiceStatusId, inspireDatasetViewModel.SpatialDataServiceNote);
            _dbContext.InspireDatasets.Add(inspireDataset);
            _dbContext.SaveChanges();

            return inspireDataset;
        }

        private void NewInspireDatasetFromKartkatalogen(InspireDataset inspireDataset)
        {
            if (_registerItemService.ItemNameAlredyExist(inspireDataset)) return;
            inspireDataset.SystemId = Guid.NewGuid();
            inspireDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(inspireDataset.Name);
            inspireDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            inspireDataset.DateSubmitted = DateTime.Now;
            inspireDataset.Modified = DateTime.Now;
            inspireDataset.RegisterId = _registerService.GetInspireStatusRegisterId();
            inspireDataset.VersioningId = _registerItemService.NewVersioningGroup(inspireDataset);
            inspireDataset.VersionNumber = 1;
            inspireDataset.StatusId = "Submitted";
            inspireDataset.DokStatusId = "Proposal";

            //GetDeliveryStatuses(inspireDatasetViewModel, inspireDataset);
            var metadataStatus = _datasetDeliveryService.GetMetadataStatus(inspireDataset.Uuid);
            var metadataServiceStatus = "good";
            var distributionStatus = _datasetDeliveryService.GetDeliveryDistributionStatus(inspireDataset.Uuid, inspireDataset.DistributionUrl);
            var wmsStatus = _datasetDeliveryService.GetDokDeliveryServiceStatus(inspireDataset.Uuid, true, "deficient", inspireDataset.UuidService);
            var wfsStatus = _datasetDeliveryService.GetWfsStatus(inspireDataset.Uuid);
            var atomFeedStatus = _datasetDeliveryService.GetAtomFeedStatus(inspireDataset.Uuid);
            var wfsOrAtomStatus = _datasetDeliveryService.GetDownloadRequirementsStatus(wfsStatus, atomFeedStatus);
            var harmonizedDataStatus = _datasetDeliveryService.GetHarmonizedStatus(inspireDataset.Uuid);
            var spatialDataServiceStatusId = _datasetDeliveryService.GetSpatialDataStatus(inspireDataset.Uuid);

            inspireDataset.InspireDeliveryMetadataId = _datasetDeliveryService.CreateDatasetDelivery(metadataStatus);
            inspireDataset.InspireDeliveryMetadataServiceId = _datasetDeliveryService.CreateDatasetDelivery(metadataServiceStatus);
            inspireDataset.InspireDeliveryDistributionId = _datasetDeliveryService.CreateDatasetDelivery(distributionStatus);
            inspireDataset.InspireDeliveryWmsId = _datasetDeliveryService.CreateDatasetDelivery(wmsStatus);
            inspireDataset.InspireDeliveryWfsId = _datasetDeliveryService.CreateDatasetDelivery(wfsStatus);
            inspireDataset.InspireDeliveryAtomFeedId = _datasetDeliveryService.CreateDatasetDelivery(atomFeedStatus);
            inspireDataset.InspireDeliveryWfsOrAtomId = _datasetDeliveryService.CreateDatasetDelivery(wfsOrAtomStatus);
            inspireDataset.InspireDeliveryHarmonizedDataId = _datasetDeliveryService.CreateDatasetDelivery(harmonizedDataStatus);
            inspireDataset.InspireDeliverySpatialDataServiceId = _datasetDeliveryService.CreateDatasetDelivery(spatialDataServiceStatusId);

            _dbContext.InspireDatasets.Add(inspireDataset);
            _dbContext.SaveChanges();
        }

        private void GetDeliveryStatuses(InspireDatasetViewModel inspireDatasetViewModel, InspireDataset inspireDataset)
        {
            inspireDatasetViewModel.MetadataStatusId = _datasetDeliveryService.GetMetadataStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.MetadataStatusId);
            inspireDatasetViewModel.MetadataServiceStatusId = "good"; // skal alltid være ok.... inspireDatasetViewModel.InspireDeliveryMetadataStatus;
            inspireDatasetViewModel.DistributionStatusId = _datasetDeliveryService.GetDeliveryDistributionStatus(inspireDataset.Uuid, inspireDataset.DistributionUrl, true, inspireDatasetViewModel.DistributionStatusId);
            inspireDatasetViewModel.WmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.WmsStatusId, inspireDataset.UuidService);
            inspireDatasetViewModel.WfsStatusId = _datasetDeliveryService.GetWfsStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.WfsStatusId);
            inspireDatasetViewModel.AtomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.AtomFeedStatusId);
            inspireDatasetViewModel.WfsOrAtomStatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(inspireDatasetViewModel.WfsStatusId, inspireDatasetViewModel.AtomFeedStatusId);
            inspireDatasetViewModel.HarmonizedDataStatusId = _datasetDeliveryService.GetHarmonizedStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.HarmonizedDataStatusId);
            inspireDatasetViewModel.SpatialDataServiceStatusId = _datasetDeliveryService.GetSpatialDataStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.SpatialDataServiceStatusId);
        }

        public InspireDatasetViewModel NewInspireDatasetViewModel(string parentRegister, string register)
        {
            var model = new InspireDatasetViewModel
            {
                RegisterId = _registerService.GetRegisterId(parentRegister, register)
            };

            return model;
        }

        public InspireDataset GetInspireDatasetByName(string registerSeoName, string itemSeoName)
        {
            var queryResult = from i in _dbContext.InspireDatasets
                              where i.Seoname == itemSeoName &&
                                    i.Register.seoname == registerSeoName
                              select i;

            return queryResult.FirstOrDefault();
        }

        private InspireDataset GetInspireDatasetBySystemId(Guid systemId)
        {
            var queryResult = from i in _dbContext.InspireDatasets
                              where i.SystemId == systemId
                              select i;

            return queryResult.FirstOrDefault();
        }

        public InspireDataset UpdateInspireDataset(InspireDatasetViewModel viewModel)
        {
            var inspireDataset = GetInspireDatasetBySystemId(viewModel.SystemId);
            inspireDataset.Name = viewModel.Name;
            inspireDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(inspireDataset.Name);
            inspireDataset.Description = viewModel.Description;
            inspireDataset.SubmitterId = viewModel.SubmitterId;
            inspireDataset.OwnerId = viewModel.OwnerId;
            inspireDataset.Modified = DateTime.Now;
            inspireDataset.DateSubmitted = DateTime.Now;
            inspireDataset.RegisterId = viewModel.RegisterId;

            inspireDataset.Uuid = viewModel.Uuid;
            inspireDataset.Notes = viewModel.Notes;
            inspireDataset.SpecificUsage = viewModel.SpecificUsage;
            inspireDataset.ProductSheetUrl = viewModel.ProductSheetUrl;
            inspireDataset.PresentationRulesUrl = viewModel.PresentationRulesUrl;
            inspireDataset.ProductSpecificationUrl = viewModel.ProductSpecificationUrl;
            inspireDataset.MetadataUrl = viewModel.MetadataUrl;
            inspireDataset.DistributionFormat = viewModel.DistributionFormat;
            inspireDataset.DistributionUrl = viewModel.DistributionUrl;
            inspireDataset.DistributionArea = viewModel.DistributionArea;
            inspireDataset.WmsUrl = viewModel.WmsUrl;
            inspireDataset.ThemeGroupId = viewModel.ThemeGroupId;
            inspireDataset.DatasetThumbnail = viewModel.DatasetThumbnail;
            inspireDataset.DokStatusId = viewModel.DokStatusId;
            inspireDataset.DokStatusDateAccepted = viewModel.GetDateAccepted();
            inspireDataset.UuidService = viewModel.UuidService;

            inspireDataset.InspireTheme = viewModel.InspireTheme;
            if (inspireDataset.InspireDeliveryMetadata != null)
            {
                inspireDataset.InspireDeliveryMetadata.StatusId = viewModel.MetadataStatusId;
                inspireDataset.InspireDeliveryMetadata.Note = viewModel.MetadataNote;
                inspireDataset.InspireDeliveryMetadata.AutoUpdate = viewModel.MetadataAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryMetadataService != null)
            {
                inspireDataset.InspireDeliveryMetadataService.StatusId = viewModel.MetadataServiceStatusId;
                inspireDataset.InspireDeliveryMetadataService.Note = viewModel.MetadataServiceNote;
                inspireDataset.InspireDeliveryMetadataService.AutoUpdate = viewModel.MetadataServiceAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryDistribution != null)
            {
                inspireDataset.InspireDeliveryDistribution.StatusId = viewModel.DistributionStatusId;
                inspireDataset.InspireDeliveryDistribution.Note = viewModel.DistributionNote;
                inspireDataset.InspireDeliveryDistribution.AutoUpdate = viewModel.DistributionAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryWms != null)
            {
                inspireDataset.InspireDeliveryWms.StatusId = viewModel.WmsStatusId;
                inspireDataset.InspireDeliveryWms.Note = viewModel.WmsNote;
                inspireDataset.InspireDeliveryWms.AutoUpdate = viewModel.WmsAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryWfs != null)
            {
                inspireDataset.InspireDeliveryWfs.StatusId = viewModel.WfsStatusId;
                inspireDataset.InspireDeliveryWfs.Note = viewModel.WfsNote;
                inspireDataset.InspireDeliveryWfs.AutoUpdate = viewModel.WfsAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryAtomFeed != null)
            {
                inspireDataset.InspireDeliveryAtomFeed.StatusId = viewModel.AtomFeedStatusId;
                inspireDataset.InspireDeliveryAtomFeed.Note = viewModel.AtomFeedNote;
                inspireDataset.InspireDeliveryAtomFeed.AutoUpdate = viewModel.AtomFeedAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryWfsOrAtom != null)
            {
                inspireDataset.InspireDeliveryWfsOrAtom.StatusId = viewModel.WfsOrAtomStatusId;
                inspireDataset.InspireDeliveryWfsOrAtom.Note = viewModel.WfsOrAtomNote;
                inspireDataset.InspireDeliveryWfsOrAtom.AutoUpdate = viewModel.WfsOrAtomAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryHarmonizedData != null)
            {
                inspireDataset.InspireDeliveryHarmonizedData.StatusId = viewModel.HarmonizedDataStatusId;
                inspireDataset.InspireDeliveryHarmonizedData.Note = viewModel.HarmonizedDataNote;
                inspireDataset.InspireDeliveryHarmonizedData.AutoUpdate = viewModel.HarmonizedDataAutoUpdate;
            }

            if (inspireDataset.InspireDeliverySpatialDataService != null)
            {
                inspireDataset.InspireDeliverySpatialDataService.StatusId = viewModel.SpatialDataServiceStatusId;
                inspireDataset.InspireDeliverySpatialDataService.Note = viewModel.SpatialDataServiceNote;
                inspireDataset.InspireDeliverySpatialDataService.AutoUpdate = viewModel.SpatialDataServiceAutoUpdate;
            }

            _dbContext.Entry(inspireDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return inspireDataset;
        }

        public InspireDataset UpdateInspireDataset(InspireDataset originalDataset, InspireDataset inspireDatasetFromKartkatalogen)
        {
            originalDataset.Name = inspireDatasetFromKartkatalogen.Name;
            originalDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(originalDataset.Name);
            originalDataset.Description = inspireDatasetFromKartkatalogen.Description;
            originalDataset.OwnerId = inspireDatasetFromKartkatalogen.OwnerId;
            originalDataset.Modified = DateTime.Now;

            originalDataset.Uuid = inspireDatasetFromKartkatalogen.Uuid;
            originalDataset.SpecificUsage = inspireDatasetFromKartkatalogen.SpecificUsage;
            originalDataset.ProductSheetUrl = inspireDatasetFromKartkatalogen.ProductSheetUrl;
            originalDataset.PresentationRulesUrl = inspireDatasetFromKartkatalogen.PresentationRulesUrl;
            originalDataset.ProductSpecificationUrl = inspireDatasetFromKartkatalogen.ProductSpecificationUrl;
            originalDataset.MetadataUrl = inspireDatasetFromKartkatalogen.MetadataUrl;
            originalDataset.DistributionFormat = inspireDatasetFromKartkatalogen.DistributionFormat;
            originalDataset.DistributionUrl = inspireDatasetFromKartkatalogen.DistributionUrl;
            originalDataset.DistributionArea = inspireDatasetFromKartkatalogen.DistributionArea;
            originalDataset.WmsUrl = inspireDatasetFromKartkatalogen.WmsUrl;
            originalDataset.ThemeGroupId = inspireDatasetFromKartkatalogen.ThemeGroupId;
            originalDataset.DatasetThumbnail = inspireDatasetFromKartkatalogen.DatasetThumbnail;
            originalDataset.UuidService = inspireDatasetFromKartkatalogen.UuidService;

            originalDataset.InspireTheme = inspireDatasetFromKartkatalogen.InspireTheme;
            if (originalDataset.InspireDeliveryMetadata != null)
            {
                originalDataset.InspireDeliveryMetadata.StatusId =
                    _datasetDeliveryService.GetMetadataStatus(inspireDatasetFromKartkatalogen.Uuid, true,
                        originalDataset.InspireDeliveryMetadata.StatusId);
            }
            originalDataset.InspireDeliveryMetadataService.StatusId = "good";
            if (originalDataset.InspireDeliveryDistribution != null)
            {
                originalDataset.InspireDeliveryDistribution.StatusId =
                    _datasetDeliveryService.GetDeliveryDistributionStatus(inspireDatasetFromKartkatalogen.Uuid, inspireDatasetFromKartkatalogen.DistributionUrl, true,
                        originalDataset.InspireDeliveryDistribution.StatusId);
            }
            if (originalDataset.InspireDeliveryWms != null)
            {
                originalDataset.InspireDeliveryWms.StatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(
                    inspireDatasetFromKartkatalogen.Uuid, true, originalDataset.InspireDeliveryWms.StatusId,
                    originalDataset.UuidService);
            }

            if (originalDataset.InspireDeliveryWfs != null)
            {
                originalDataset.InspireDeliveryWfs.StatusId = _datasetDeliveryService.GetWfsStatus(inspireDatasetFromKartkatalogen.Uuid,
                    true, originalDataset.InspireDeliveryWfs.StatusId);
            }

            if (originalDataset.InspireDeliveryAtomFeed != null)
            {
                originalDataset.InspireDeliveryAtomFeed.StatusId =
                    _datasetDeliveryService.GetAtomFeedStatus(inspireDatasetFromKartkatalogen.Uuid, true,
                        originalDataset.InspireDeliveryAtomFeed.StatusId);
            }

            if (originalDataset.InspireDeliveryWfsOrAtom != null)
            {
                if (originalDataset.InspireDeliveryWfs != null && originalDataset.InspireDeliveryAtomFeed != null)
                    originalDataset.InspireDeliveryWfsOrAtom.StatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(originalDataset.InspireDeliveryWfs.StatusId, originalDataset.InspireDeliveryAtomFeed.StatusId);
                else originalDataset.InspireDeliveryWfsOrAtom.StatusId = "notset";

            }

            if (originalDataset.InspireDeliveryHarmonizedData != null)
            {
                originalDataset.InspireDeliveryHarmonizedData.StatusId =
                    _datasetDeliveryService.GetHarmonizedStatus(inspireDatasetFromKartkatalogen.Uuid, true,
                        originalDataset.InspireDeliveryHarmonizedData.StatusId);
            }

            if (originalDataset.InspireDeliverySpatialDataService != null)
            {
                originalDataset.InspireDeliverySpatialDataService.StatusId =
                    _datasetDeliveryService.GetSpatialDataStatus(inspireDatasetFromKartkatalogen.Uuid, true,
                        originalDataset.InspireDeliverySpatialDataService.StatusId);
            }

            _dbContext.Entry(originalDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return originalDataset;
        }

        public InspireDataset UpdateInspireDatasetFromKartkatalogen(InspireDataset originalDataset)
        {
            var inspireDatasetFromKartkatalogen = _metadataService.FetchInspireDatasetFromKartkatalogen(originalDataset.Uuid);
            return inspireDatasetFromKartkatalogen == null ? originalDataset : UpdateInspireDataset(originalDataset, inspireDatasetFromKartkatalogen);
        }

        public void DeleteInspireDataset(InspireDataset inspireDataset)
        {
            _dbContext.InspireDatasets.Remove(inspireDataset);

            //Todo, må slette deliveryDataset?
            _dbContext.SaveChanges();
        }

        public void SynchronizeInspireDatasets()
        {
            var inspireDatasetsFromKartkatalogen = FetchInspireDatasetsFromKartkatalogen();
            RemoveInspireDatasets(inspireDatasetsFromKartkatalogen);
            UpdateInspireDataset(inspireDatasetsFromKartkatalogen);
            _dbContext.SaveChanges();
        }

        private void UpdateInspireDataset(List<InspireDataset> inspireDatasetsFromKartkatalogen)
        {
            //Update register
            foreach (var inspireDatasetFromKartkatalogen in inspireDatasetsFromKartkatalogen)
            {
                var originalInspireDataset = GetInspireDatasetByUuid(inspireDatasetFromKartkatalogen.Uuid);
                if (originalInspireDataset != null)
                {
                    UpdateInspireDataset(originalInspireDataset, inspireDatasetFromKartkatalogen);
                }
                else
                {
                    NewInspireDatasetFromKartkatalogen(inspireDatasetFromKartkatalogen);
                }
            }
        }

        private void RemoveInspireDatasets(List<InspireDataset> inspireDatasetsFromKartkatalogen)
        {
            var inspireDatasetsFromRegister = GetInspireDatasets();
            var exists = false;
            var removeDatasets = new List<InspireDataset>();

            foreach (var inspireDatasetFromRegister in inspireDatasetsFromRegister)
            {
                if (inspireDatasetsFromKartkatalogen.Any(inspireDatasetFromKartkatalog => inspireDatasetFromKartkatalog.Uuid == inspireDatasetFromRegister.Uuid))
                {
                    exists = true;
                }
                if (!exists)
                {
                    removeDatasets.Add(inspireDatasetFromRegister);
                }
                exists = false;
            }
            foreach (var inspireDataset in removeDatasets)
            {
                DeleteInspireDataset(inspireDataset);
            }
        }

        private InspireDataset GetInspireDatasetByUuid(string uuid)
        {
            var queryResult = from i in _dbContext.InspireDatasets
                              where i.Uuid == uuid
                              select i;

            return queryResult.FirstOrDefault();
        }

        private List<InspireDataset> GetInspireDatasets()
        {
            var queryResultsRegisterItem = from d in _dbContext.InspireDatasets
                                           where !string.IsNullOrEmpty(d.Uuid)
                                           select d;

            var inspireDatasets = queryResultsRegisterItem.ToList();
            return inspireDatasets;
        }

        private List<InspireDataset> FetchInspireDatasetsFromKartkatalogen()
        {
            var inspireDatasetsFromKartkatalogen = new List<InspireDataset>();

            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/datasets?facets%5b0%5dname=nationalinitiative&facets%5b0%5dvalue=Inspire&Offset=1&limit=500&mediatype=json";
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
                        var inspireDataset = _metadataService.FetchInspireDatasetFromKartkatalogen(item.Uuid.ToString());
                        if (inspireDataset != null)
                        {
                            inspireDatasetsFromKartkatalogen.Add(inspireDataset);
                        }
                    }
                }
                return inspireDatasetsFromKartkatalogen;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }
        }



         //**** INSPIRE DATA SERVICE****

        public void SynchronizeInspireDataServices()
        {
            var originalInspireDataServices = GetInspireDataService();
            var inspireDataServicesFromKartkatalogen = FetchInspireDataServicesFromKartkatalogen();

            RemoveInspireDataServices(originalInspireDataServices, inspireDataServicesFromKartkatalogen);
            AddOrUpdateInspireDataServices(originalInspireDataServices, inspireDataServicesFromKartkatalogen);

            _dbContext.SaveChanges();
        }


        private void AddOrUpdateInspireDataServices(ICollection<InspireDataService> inspireDataServicesFromRegister, List<InspireDataService> inspireDataServicesFromKartkatalogen)
        {
            foreach (var inspireDataServiceFromKartkatalogen in inspireDataServicesFromKartkatalogen)
            {
                var originalInspireDataService = GetInspireDataServiceByUuid(inspireDataServiceFromKartkatalogen.Uuid);
                if (originalInspireDataService != null)
                {
                    UpdateInspireDataService(originalInspireDataService, inspireDataServiceFromKartkatalogen);
                }
                else
                {
                    NewInspireDataServiceFromKartkatalogen(inspireDataServiceFromKartkatalogen);
                }
            }
        }

        private void NewInspireDataServiceFromKartkatalogen(InspireDataService inspireDataService)
        {
            if (_registerItemService.ItemNameAlredyExist(inspireDataService)) return;
            inspireDataService.SystemId = Guid.NewGuid();
            inspireDataService.Seoname = RegisterUrls.MakeSeoFriendlyString(inspireDataService.Name);
            inspireDataService.SubmitterId = _registerService.GetOrganizationIdByUserName();
            inspireDataService.DateSubmitted = DateTime.Now;
            inspireDataService.Modified = DateTime.Now;
            inspireDataService.RegisterId = _registerService.GetInspireStatusRegisterId();
            inspireDataService.VersioningId = _registerItemService.NewVersioningGroup(inspireDataService);
            inspireDataService.VersionNumber = 1;
            inspireDataService.StatusId = "Submitted";

            var metadataStatus = _datasetDeliveryService.GetMetadataStatus(inspireDataService.Uuid);
            var inspireDeliveryMetadataInSearchService = "good";
            var inspireDeliveryServiceStatus = _datasetDeliveryService.GetServiceStatus(inspireDataService.Uuid, "deficient");
            inspireDataService.InspireDeliveryMetadataId = _datasetDeliveryService.CreateDatasetDelivery(metadataStatus);
            inspireDataService.InspireDeliveryMetadataInSearchServiceId = _datasetDeliveryService.CreateDatasetDelivery(inspireDeliveryMetadataInSearchService);
            inspireDataService.InspireDeliveryServiceStatusId = _datasetDeliveryService.CreateDatasetDelivery(inspireDeliveryServiceStatus);

            _dbContext.InspireDataServices.Add(inspireDataService);
            _dbContext.SaveChanges();
        }

        private void RemoveInspireDataServices(ICollection<InspireDataService> inspireDataServicesFromRegister, List<InspireDataService> inspireDataServicesFromKartkatalogen)
        {
            var exists = false;
            var removeDataServices = new List<InspireDataService>();

            foreach (var inspireDataServiceFromRegister in inspireDataServicesFromRegister)
            {
                if (inspireDataServicesFromKartkatalogen.Any(inspireDataServiceFromKartkatalog => inspireDataServiceFromKartkatalog.Uuid == inspireDataServiceFromRegister.Uuid))
                {
                    exists = true;
                }
                if (!exists)
                {
                    removeDataServices.Add(inspireDataServiceFromRegister);
                }
                exists = false;
            }
            foreach (var inspireDataService in removeDataServices)
            {
                DeleteInspireDataService(inspireDataService);
            }
        }

        private void DeleteInspireDataService(InspireDataService inspireDataService)
        {
            _dbContext.InspireDataServices.Remove(inspireDataService);
            _dbContext.SaveChanges();
        }

        private List<InspireDataService> FetchInspireDataServicesFromKartkatalogen()
        {
            List<InspireDataService> inspireDataServices = new List<InspireDataService>();

            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/servicedirectory?facets%5b0%5dname=nationalinitiative&facets%5b0%5dvalue=Inspire&Offset=1&limit=500&mediatype=json";
            var c = new System.Net.WebClient { Encoding = System.Text.Encoding.UTF8 };
            try
            {
                var json = c.DownloadString(url);
                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (data != null)
                {
                    var result = data.Results;

                    foreach (var service in result)
                    {
                        var inspireDataService = new InspireDataService();
                        inspireDataService.Name = service.Title;
                        inspireDataService.Uuid = service.Uuid;
                        inspireDataService.ServiceType = service.Protocol;
                    }
                }
                return inspireDataServices;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }
        }

        private InspireDataService GetInspireDataServiceByUuid(string uuid)
        {
            var queryResult = from i in _dbContext.InspireDataServices
                where i.Uuid == uuid
                select i;

            return queryResult.FirstOrDefault();
        }

        public InspireDataService UpdateInspireDataService(InspireDataService originalDataService, InspireDataService inspireDataServiceFromKartkatalogen)
        {
            originalDataService.Name = inspireDataServiceFromKartkatalogen.Name;
            originalDataService.Seoname = RegisterUrls.MakeSeoFriendlyString(originalDataService.Name);
            originalDataService.Description = inspireDataServiceFromKartkatalogen.Description;
            originalDataService.OwnerId = inspireDataServiceFromKartkatalogen.OwnerId;
            originalDataService.Modified = DateTime.Now;

            originalDataService.Uuid = inspireDataServiceFromKartkatalogen.Uuid;
            originalDataService.ServiceType = inspireDataServiceFromKartkatalogen.ServiceType;
            
            if (originalDataService.InspireDeliveryMetadata != null)
            {
                originalDataService.InspireDeliveryMetadata.StatusId =
                    _datasetDeliveryService.GetMetadataStatus(inspireDataServiceFromKartkatalogen.Uuid, true,
                        originalDataService.InspireDeliveryMetadata.StatusId);
            }

            originalDataService.InspireDeliveryMetadataInSearchService.StatusId = "good";

            if (originalDataService.InspireDeliveryServiceStatus != null)
            {
                originalDataService.InspireDeliveryServiceStatus.StatusId =  _datasetDeliveryService.GetServiceStatus(inspireDataServiceFromKartkatalogen.Uuid, originalDataService.InspireDeliveryServiceStatus.StatusId);
            }

            originalDataService.NetworkService = inspireDataServiceFromKartkatalogen.NetworkService;
            originalDataService.Url = inspireDataServiceFromKartkatalogen.Url;
            originalDataService.Theme = inspireDataServiceFromKartkatalogen.Theme;

            _dbContext.Entry(originalDataService).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return originalDataService;
        }

        public ICollection<InspireDataService> GetInspireDataService()
        {
            var queryResult = from i in _dbContext.InspireDataServices
                select i;

            return queryResult.Any() ? queryResult.ToList() : new List<InspireDataService>();
        }

        public ICollection<InspireDataServiceViewModel> ConvertToViewModel(ICollection<InspireDataService> inspireDataServices)
        {
            List<InspireDataServiceViewModel> inspireDataServiceViewModel = new List<InspireDataServiceViewModel>();
            foreach (InspireDataService inspireDataService in inspireDataServices)
            {
                inspireDataServiceViewModel.Add(new InspireDataServiceViewModel(inspireDataService));
            }

            return inspireDataServiceViewModel;
        }

    }
}