using Kartverket.Register.Models;
using System;
using System.Linq;
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

        public InspireDatasetService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerService = new RegisterService(_dbContext);
            _registerItemService = new RegisterItemService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
        }

        public void CreateNewInspireDataset(InspireDatasetViewModel inspireDatasetViewModel, string parentregister, string registername)
        {
            var inspireDataset = new InspireDataset();
            inspireDataset.Name = inspireDatasetViewModel.Name;
            inspireDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(inspireDataset.Name);
            inspireDataset.Description = inspireDatasetViewModel.Description;
            inspireDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            inspireDataset.OwnerId = _registerService.GetOrganizationIdByUserName(); //  TODO, fiks
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
            inspireDataset.DokStatusId = inspireDatasetViewModel.DokStatusId;

            GetDeliveryStatuses(inspireDatasetViewModel, inspireDataset);

            inspireDataset.InspireDeliveryMetadataId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryMetadataStatus, inspireDatasetViewModel.InspireDeliveryMetadataNote, true);
            inspireDataset.InspireDeliveryMetadataServiceId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryMetadataServiceStatus, inspireDatasetViewModel.InspireDeliveryMetadataServiceNote, true);
            inspireDataset.InspireDeliveryDistributionId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryDistributionStatus, inspireDatasetViewModel.InspireDeliveryDistributionNote, true);
            inspireDataset.InspireDeliveryWmsId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryWmsStatus, inspireDatasetViewModel.InspireDeliveryWmsNote, true);
            inspireDataset.InspireDeliveryWfsId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryWfsStatus, inspireDatasetViewModel.InspireDeliveryWfsNote, true);
            inspireDataset.InspireDeliveryAtomFeedId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryAtomFeedStatus, inspireDatasetViewModel.InspireDeliveryAtomFeedNote, true);
            inspireDataset.InspireDeliveryWfsOrAtomId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryWfsOrAtomStatus, inspireDatasetViewModel.InspireDeliveryWfsOrAtomNote, true);
            inspireDataset.InspireDeliveryHarmonizedDataId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryHarmonizedDataStatus, inspireDatasetViewModel.InspireDeliveryHarmonizedDataNote, true);
            inspireDataset.InspireDeliverySpatialDataServiceId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliverySpatialDataServiceStatus, inspireDatasetViewModel.InspireDeliverySpatialDataServiceNote, true);

            inspireDataset.VersioningId = _registerItemService.NewVersioningGroup(inspireDataset);
            _dbContext.InspireDatasets.Add(inspireDataset);
            _dbContext.SaveChanges();
        }

        private void GetDeliveryStatuses(InspireDatasetViewModel inspireDatasetViewModel, InspireDataset inspireDataset)
        {
            inspireDatasetViewModel.InspireDeliveryMetadataStatus = _datasetDeliveryService.GetMetadataStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.InspireDeliveryMetadataStatus);
            inspireDatasetViewModel.InspireDeliveryMetadataServiceStatus = "good"; // skal alltid være ok.... inspireDatasetViewModel.InspireDeliveryMetadataStatus;
            inspireDatasetViewModel.InspireDeliveryDistributionStatus = _datasetDeliveryService.GetDeliveryDistributionStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.InspireDeliveryDistributionStatus);
            inspireDatasetViewModel.InspireDeliveryWmsStatus = _datasetDeliveryService.GetDokDeliveryServiceStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.InspireDeliveryWmsStatus, inspireDataset.UuidService);
            inspireDatasetViewModel.InspireDeliveryWfsStatus = _datasetDeliveryService.GetWfsStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.InspireDeliveryWfsStatus);
            inspireDatasetViewModel.InspireDeliveryAtomFeedStatus = _datasetDeliveryService.GetAtomFeedStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.InspireDeliveryAtomFeedStatus);
            inspireDatasetViewModel.InspireDeliveryWfsOrAtomStatus = GetInspireDeliveryWfsOrAtomFeedStatus(inspireDatasetViewModel.InspireDeliveryWfsStatus, inspireDatasetViewModel.InspireDeliveryAtomFeedStatus);
            inspireDatasetViewModel.InspireDeliveryHarmonizedDataStatus = _datasetDeliveryService.GetHarmonizedStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.InspireDeliveryHarmonizedDataStatus); // TODO Sjekk om det testes på riktige data...
            inspireDatasetViewModel.InspireDeliverySpatialDataServiceStatus = _datasetDeliveryService.GetSpatialDataStatus(inspireDataset.Uuid, true, inspireDatasetViewModel.InspireDeliverySpatialDataServiceStatus); // TODO Sjekk om det testes på riktige data...
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

        public InspireDataset GetInspireDatasetByName(string itemname)
        {
            var queryResult = from i in _dbContext.InspireDatasets
                              where i.Seoname == itemname
                              select i;

            return queryResult.FirstOrDefault();
        }
    }
}