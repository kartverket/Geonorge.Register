using Kartverket.Register.Models;
using System;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services
{
    public class InspireDatasetService : IInspireDatasetService
    {
        private readonly RegisterDbContext _dbContext;
        private readonly IRegisterService _registerService;
        private readonly IDatasetDeliveryService _datasetDeliveryService;
        private readonly IDatasetService _datasetService;

        public InspireDatasetService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerService = new RegisterService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
            _datasetService = new DatasetService(_dbContext);
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

            inspireDataset.InspireDeliveryMetadataId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryMetadataStatus, inspireDatasetViewModel.InspireDeliveryMetadataNote, true);
            inspireDataset.InspireDeliveryMetadataServiceId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryMetadataServiceStatus, inspireDatasetViewModel.InspireDeliveryMetadataServiceNote, true);
            inspireDataset.InspireDeliveryDistributionId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryDistributionStatus, inspireDatasetViewModel.InspireDeliveryDistributionNote, true);
            inspireDataset.InspireDeliveryWmsId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryWmsStatus, inspireDatasetViewModel.InspireDeliveryWmsNote, true);
            inspireDataset.InspireDeliveryWfsId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryWfsStatus, inspireDatasetViewModel.InspireDeliveryWfsNote, true);
            inspireDataset.InspireDeliveryAtomFeedId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryAtomFeedStatus, inspireDatasetViewModel.InspireDeliveryAtomFeedNote, true);
            inspireDataset.InspireDeliveryWfsOrAtomId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryWfsOrAtomStatus, inspireDatasetViewModel.InspireDeliveryWfsOrAtomNote, true);
            inspireDataset.InspireDeliveryHarmonizedDataId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliveryHarmonizedDataStatus, inspireDatasetViewModel.InspireDeliveryHarmonizedDataNote, true);
            inspireDataset.InspireDeliverySpatialDataServiceId = _datasetDeliveryService.CreateDatasetDelivery(inspireDatasetViewModel.InspireDeliverySpatialDataServiceStatus, inspireDatasetViewModel.InspireDeliverySpatialDataServiceNote, true);
            

            _dbContext.InspireDatasets.Add(inspireDataset);
            _dbContext.SaveChanges();
        }

        public InspireDatasetViewModel NewInspireDataset(string parentRegister, string register)
        {
            var model = new InspireDatasetViewModel {Register = _registerService.GetRegister(parentRegister, register)};

            return model;
        }
    }
}