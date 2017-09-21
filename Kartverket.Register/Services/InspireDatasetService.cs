using Kartverket.Register.Models;
using System;
using System.Linq;
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

        public InspireDatasetService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerService = new RegisterService(_dbContext);
            _registerItemService = new RegisterItemService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
        }

        public InspireDataset CreateNewInspireDataset(InspireDatasetViewModel inspireDatasetViewModel, string parentregister, string registername)
        {
            var inspireDataset = new InspireDataset();
            inspireDataset.Name = inspireDatasetViewModel.Name;
            inspireDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(inspireDataset.Name);
            inspireDataset.Description = inspireDatasetViewModel.Description;
            inspireDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            inspireDataset.OwnerId = inspireDatasetViewModel.OwnerId;
            inspireDataset.DateSubmitted = DateTime.Now;
            inspireDataset.Modified = DateTime.Now;
            inspireDataset.RegisterId = _registerService.GetRegisterId(parentregister, registername);

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

            inspireDataset.VersioningId = _registerItemService.NewVersioningGroup(inspireDataset);
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

        public InspireDatasetViewModel NewInspireDataset(string parentRegister, string register)
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
    }
}