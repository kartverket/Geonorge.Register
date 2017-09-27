using Kartverket.Register.Models;
using System;
using System.Data.Entity;
using System.Linq;
using Kartverket.DOK.Service;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Helpers;
using Kartverket.Register.Migrations;
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

        public InspireDataset CreateNewInspireDataset(InspireDatasetViewModel inspireDatasetViewModel, string parentregister, string registername)
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

            inspireDataset.InspireDeliveryMetadataId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.MetadataStatus, inspireDatasetViewModel.MetadataNote, true);
            inspireDataset.InspireDeliveryMetadataServiceId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.MetadataServiceStatus, inspireDatasetViewModel.MetadataServiceNote, true);
            inspireDataset.InspireDeliveryDistributionId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.DistributionStatus, inspireDatasetViewModel.DistributionNote, true);
            inspireDataset.InspireDeliveryWmsId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.WmsStatus, inspireDatasetViewModel.WmsNote, true);
            inspireDataset.InspireDeliveryWfsId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.WfsStatus, inspireDatasetViewModel.WfsNote, true);
            inspireDataset.InspireDeliveryAtomFeedId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.AtomFeedStatus, inspireDatasetViewModel.AtomFeedNote, true);
            inspireDataset.InspireDeliveryWfsOrAtomId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.WfsOrAtomStatus, inspireDatasetViewModel.WfsOrAtomNote, true);
            inspireDataset.InspireDeliveryHarmonizedDataId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.HarmonizedDataStatus, inspireDatasetViewModel.HarmonizedDataNote, true);
            inspireDataset.InspireDeliverySpatialDataServiceId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.SpatialDataServiceStatus, inspireDatasetViewModel.SpatialDataServiceNote, true);
            _dbContext.InspireDatasets.Add(inspireDataset);
            _dbContext.SaveChanges();

            return inspireDataset;
        }

        private void GetDeliveryStatuses(InspireDatasetViewModel inspireDatasetViewModel, InspireDataset inspireDataset)
        {
            inspireDatasetViewModel.MetadataStatus = _datasetDeliveryService.GetMetadataStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.MetadataStatus);
            inspireDatasetViewModel.MetadataServiceStatus = "good"; // skal alltid være ok.... inspireDatasetViewModel.InspireDeliveryMetadataStatus;
            inspireDatasetViewModel.DistributionStatus = _datasetDeliveryService.GetDeliveryDistributionStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.DistributionStatus);
            inspireDatasetViewModel.WmsStatus = _datasetDeliveryService.GetDokDeliveryServiceStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.WmsStatus, inspireDataset.UuidService);
            inspireDatasetViewModel.WfsStatus = _datasetDeliveryService.GetWfsStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.WfsStatus);
            inspireDatasetViewModel.AtomFeedStatus = _datasetDeliveryService.GetAtomFeedStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.AtomFeedStatus);
            inspireDatasetViewModel.WfsOrAtomStatus = GetInspireDeliveryWfsOrAtomFeedStatus(inspireDatasetViewModel.WfsStatus, inspireDatasetViewModel.AtomFeedStatus);
            inspireDatasetViewModel.HarmonizedDataStatus = _datasetDeliveryService.GetHarmonizedStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.HarmonizedDataStatus); // TODO Sjekk om det testes på riktige data...
            inspireDatasetViewModel.SpatialDataServiceStatus = _datasetDeliveryService.GetSpatialDataStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.SpatialDataServiceStatus); // TODO Sjekk om det testes på riktige data...
        }

        private static string GetInspireDeliveryWfsOrAtomFeedStatus(string wfsStatus, string atomFeedStatus)
        {
            if (wfsStatus == "good" || atomFeedStatus == "good")
            {
                return "good";
            }
            if (wfsStatus == "useable" || atomFeedStatus == "useable")
            {
                return "useable";
            }
            if (wfsStatus == "deficient" || atomFeedStatus == "deficient")
            {
                return  "deficient";
            }
            if (wfsStatus == "notset" || atomFeedStatus == "notset")
            {
                return "notset";
            }
            return "notset";
        }

        public InspireDatasetViewModel NewInspireDatasetViewModel(string parentRegister, string register)
        {
            var model = new InspireDatasetViewModel {RegisterId = _registerService.GetRegisterId(parentRegister, register)};

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

            if (inspireDataset.InspireDeliveryMetadata != null) 
            {
                inspireDataset.InspireDeliveryMetadata.StatusId = viewModel.MetadataStatus;
                inspireDataset.InspireDeliveryMetadata.Note = viewModel.MetadataNote;
                inspireDataset.InspireDeliveryMetadata.AutoUpdate = viewModel.MetadataAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryMetadataService != null)
            {
                inspireDataset.InspireDeliveryMetadataService.StatusId = viewModel.MetadataServiceStatus;
                inspireDataset.InspireDeliveryMetadataService.Note = viewModel.MetadataServiceNote;
                inspireDataset.InspireDeliveryMetadataService.AutoUpdate = viewModel.MetadataServiceAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryDistribution != null)
            {
                inspireDataset.InspireDeliveryDistribution.StatusId = viewModel.DistributionStatus;
                inspireDataset.InspireDeliveryDistribution.Note = viewModel.DistributionNote;
                inspireDataset.InspireDeliveryDistribution.AutoUpdate= viewModel.DistributionAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryWms != null)
            {
                inspireDataset.InspireDeliveryWms.StatusId = viewModel.WmsStatus;
                inspireDataset.InspireDeliveryWms.Note = viewModel.WmsNote;
                inspireDataset.InspireDeliveryWms.AutoUpdate = viewModel.WmsAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryWfs != null)
            {
                inspireDataset.InspireDeliveryWfs.StatusId = viewModel.WfsStatus;
                inspireDataset.InspireDeliveryWfs.Note = viewModel.WfsNote;
                inspireDataset.InspireDeliveryWfs.AutoUpdate = viewModel.WfsAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryAtomFeed != null)
            {
                inspireDataset.InspireDeliveryAtomFeed.StatusId = viewModel.AtomFeedStatus;
                inspireDataset.InspireDeliveryAtomFeed.Note = viewModel.AtomFeedNote;
                inspireDataset.InspireDeliveryAtomFeed.AutoUpdate = viewModel.AtomFeedAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryWfsOrAtom != null)
            {
                inspireDataset.InspireDeliveryWfsOrAtom.StatusId = viewModel.WfsOrAtomStatus;
                inspireDataset.InspireDeliveryWfsOrAtom.Note = viewModel.WfsOrAtomNote;
                inspireDataset.InspireDeliveryWfsOrAtom.AutoUpdate = viewModel.WfsOrAtomAutoUpdate;
            }

            if (inspireDataset.InspireDeliveryHarmonizedData != null)
            {
                inspireDataset.InspireDeliveryHarmonizedData.StatusId = viewModel.HarmonizedDataStatus;
                inspireDataset.InspireDeliveryHarmonizedData.Note = viewModel.HarmonizedDataNote;
                inspireDataset.InspireDeliveryHarmonizedData.AutoUpdate = viewModel.HarmonizedDataAutoUpdate;
            }

            if (inspireDataset.InspireDeliverySpatialDataService != null)
            {
                inspireDataset.InspireDeliverySpatialDataService.StatusId = viewModel.HarmonizedDataStatus;
                inspireDataset.InspireDeliverySpatialDataService.Note = viewModel.HarmonizedDataNote;
                inspireDataset.InspireDeliverySpatialDataService.AutoUpdate = viewModel.HarmonizedDataAutoUpdate;
            }

            _dbContext.Entry(inspireDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return inspireDataset;
        }

        public InspireDataset UpdateInspireDatasetFromKartkatalogen(InspireDataset originalDataset)
        {
            var inspireDataset = _metadataService.FetchInspireDatasetFromKartkatalogen(originalDataset.Uuid);
            if (inspireDataset == null)
            {
                return originalDataset; //Skal datasettet da fjernes??
            }
            originalDataset.Name = inspireDataset.Name;
            originalDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(originalDataset.Name);
            originalDataset.Description = inspireDataset.Description;
            originalDataset.OwnerId = inspireDataset.OwnerId;
            originalDataset.Modified = DateTime.Now;

            originalDataset.Uuid = inspireDataset.Uuid;
            originalDataset.Notes = inspireDataset.Notes;
            originalDataset.SpecificUsage = inspireDataset.SpecificUsage;
            originalDataset.ProductSheetUrl = inspireDataset.ProductSheetUrl;
            originalDataset.PresentationRulesUrl = inspireDataset.PresentationRulesUrl;
            originalDataset.ProductSpecificationUrl = inspireDataset.ProductSpecificationUrl;
            originalDataset.MetadataUrl = inspireDataset.MetadataUrl;
            originalDataset.DistributionFormat = inspireDataset.DistributionFormat;
            originalDataset.DistributionUrl = inspireDataset.DistributionUrl;
            originalDataset.DistributionArea = inspireDataset.DistributionArea;
            originalDataset.WmsUrl = inspireDataset.WmsUrl;
            originalDataset.ThemeGroupId = inspireDataset.ThemeGroupId;
            originalDataset.DatasetThumbnail = inspireDataset.DatasetThumbnail;
            originalDataset.UuidService = inspireDataset.UuidService;

            if (originalDataset.InspireDeliveryMetadata != null)
            {
                originalDataset.InspireDeliveryMetadata.StatusId = _datasetDeliveryService.GetMetadataStatus(inspireDataset.Uuid, true, originalDataset.InspireDeliveryMetadata.StatusId);
            }
            originalDataset.InspireDeliveryMetadataService.StatusId = "good";
            if (originalDataset.InspireDeliveryDistribution != null)
            {
                originalDataset.InspireDeliveryDistribution.StatusId = _datasetDeliveryService.GetDeliveryDistributionStatus(inspireDataset.Uuid, true, originalDataset.StatusId);
            }
            if (originalDataset.InspireDeliveryWms != null)
            {
                originalDataset.InspireDeliveryWms.StatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(inspireDataset.Uuid, true, originalDataset.InspireDeliveryWms.StatusId, originalDataset.UuidService);
            }

            if (originalDataset.InspireDeliveryWfs != null)
            {
                originalDataset.InspireDeliveryWfs.StatusId = _datasetDeliveryService.GetWfsStatus(inspireDataset.Uuid, true, originalDataset.InspireDeliveryWfs.StatusId);
            }

            if (originalDataset.InspireDeliveryAtomFeed != null)
            {
                originalDataset.InspireDeliveryAtomFeed.StatusId = _datasetDeliveryService.GetAtomFeedStatus(inspireDataset.Uuid, true, originalDataset.InspireDeliveryAtomFeed.StatusId);
            }

            if (originalDataset.InspireDeliveryWfsOrAtom != null)
            {
                originalDataset.InspireDeliveryWfsOrAtom.StatusId = GetInspireDeliveryWfsOrAtomFeedStatus(originalDataset.InspireDeliveryWfs.StatusId, originalDataset.InspireDeliveryWfsOrAtom.StatusId);
            }

            if (originalDataset.InspireDeliveryHarmonizedData != null)
            {
                originalDataset.InspireDeliveryHarmonizedData.StatusId = _datasetDeliveryService.GetHarmonizedStatus(inspireDataset.Uuid, true, originalDataset.InspireDeliveryHarmonizedData.StatusId);
            }

            if (originalDataset.InspireDeliverySpatialDataService != null)
            {
                originalDataset.InspireDeliverySpatialDataService.StatusId = _datasetDeliveryService.GetSpatialDataStatus(inspireDataset.Uuid, true, originalDataset.InspireDeliverySpatialDataService.StatusId);
            }

            _dbContext.Entry(originalDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return originalDataset;
        }

        public void DeleteInspireDataset(InspireDataset inspireDataset)
        {
            _dbContext.InspireDatasets.Remove(inspireDataset);

            //Todo, må slette deliveryDataset?
            _dbContext.SaveChanges();
        }

        public void SynchronizeInspireDatasets()
        {
            var queryResultsRegisterItem = from d in _dbContext.InspireDatasets
                where !string.IsNullOrEmpty(d.Uuid)
                select d;

            var inspireDatasets = queryResultsRegisterItem.ToList();

            foreach (var inspireDataset in inspireDatasets)
            {
                UpdateInspireDatasetFromKartkatalogen(inspireDataset);
            }
            _dbContext.SaveChanges();
            _dbContext.Dispose();
        }
    }
}