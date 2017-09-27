using System;
using Kartverket.Register.Models;
using System.Linq;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.ViewModels;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;

namespace Kartverket.Register.Services
{
    public class GeodatalovDatasetService : IGeodatalovDatasetService
    {
        private readonly RegisterDbContext _dbContext;
        private readonly IRegisterService _registerService;
        private readonly IDatasetDeliveryService _datasetDeliveryService;
        private readonly IRegisterItemService _registerItemService;

        public GeodatalovDatasetService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerService = new RegisterService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
            _registerItemService = new RegisterItemService(_dbContext);
        }

        public GeodatalovDataset GetGeodatalovDatasetByName(string registerSeoName, string itemSeoName)
        {
            var queryResult = from i in _dbContext.GeodatalovDatasets
                              where i.Seoname == itemSeoName &&
                              i.Register.seoname == registerSeoName
                              select i;

            return queryResult.FirstOrDefault();
        }

        public GeodatalovDatasetViewModel NewGeodatalovDatasetViewModel(string parentregister, string registername)
        {
            var model = new GeodatalovDatasetViewModel() { RegisterId = _registerService.GetRegisterId(parentregister, registername) };

            return model;
        }

        public GeodatalovDataset CreateNewGeodatalovDataset(GeodatalovDatasetViewModel geodatalovViewModel, string parentregister, string registername)
        {
            var geodatalovDataset = new GeodatalovDataset();

            geodatalovDataset.SystemId = Guid.NewGuid();
            geodatalovDataset.Name = geodatalovViewModel.Name;
            geodatalovDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(geodatalovDataset.Name);
            geodatalovDataset.Description = geodatalovViewModel.Description;
            geodatalovDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            geodatalovDataset.OwnerId = geodatalovViewModel.OwnerId;
            geodatalovDataset.DateSubmitted = DateTime.Now;
            geodatalovDataset.Modified = DateTime.Now;
            geodatalovDataset.RegisterId = _registerService.GetRegisterId(parentregister, registername);
            geodatalovDataset.VersioningId = _registerItemService.NewVersioningGroup(geodatalovDataset);
            geodatalovDataset.VersionNumber = 1;
            geodatalovDataset.StatusId = "Submitted";

            geodatalovDataset.Uuid = geodatalovViewModel.Uuid;
            geodatalovDataset.Notes = geodatalovViewModel.Notes;
            geodatalovDataset.SpecificUsage = geodatalovViewModel.SpecificUsage;
            geodatalovDataset.ProductSheetUrl = geodatalovViewModel.ProductSheetUrl;
            geodatalovDataset.PresentationRulesUrl = geodatalovViewModel.PresentationRulesUrl;
            geodatalovDataset.ProductSpecificationUrl = geodatalovViewModel.ProductSpecificationUrl;
            geodatalovDataset.MetadataUrl = geodatalovViewModel.MetadataUrl;
            geodatalovDataset.DistributionFormat = geodatalovViewModel.DistributionFormat;
            geodatalovDataset.DistributionUrl = geodatalovViewModel.DistributionUrl;
            geodatalovDataset.DistributionArea = geodatalovViewModel.DistributionArea;
            geodatalovDataset.WmsUrl = geodatalovViewModel.WmsUrl;
            geodatalovDataset.ThemeGroupId = geodatalovViewModel.ThemeGroupId;
            geodatalovDataset.DatasetThumbnail = geodatalovViewModel.DatasetThumbnail;
            geodatalovDataset.DokStatusId = "Proposal";
            geodatalovDataset.UuidService = geodatalovViewModel.UuidService;

            GetDeliveryStatuses(geodatalovViewModel, geodatalovDataset);

            geodatalovDataset.InspireTheme = geodatalovViewModel.InspireTheme;
            geodatalovDataset.Dok = geodatalovViewModel.Dok;
            geodatalovDataset.NationalDataset = geodatalovViewModel.NationalDataset;
            geodatalovDataset.Plan = geodatalovViewModel.Plan;
            geodatalovDataset.Geodatalov = geodatalovViewModel.Geodatalov;
            geodatalovDataset.MetadataStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.MetadataStatus, geodatalovViewModel.MetadataNote, true);
            geodatalovDataset.ProductSpesificationStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WmsStatus, geodatalovViewModel.WmsNote, true);
            geodatalovDataset.SosiDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WmsStatus, geodatalovViewModel.WmsNote, true);
            geodatalovDataset.GmlDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WmsStatus, geodatalovViewModel.WmsNote, true);
            geodatalovDataset.WmsStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WmsStatus, geodatalovViewModel.WmsNote, true);
            geodatalovDataset.WfsStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WfsStatus, geodatalovViewModel.WfsNote, true);
            geodatalovDataset.AtomFeedStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.AtomFeedStatus, geodatalovViewModel.AtomFeedNote, true);
            geodatalovDataset.CommonStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WmsStatus, geodatalovViewModel.WmsNote, true);
            _dbContext.GeodatalovDatasets.Add(geodatalovDataset);
            _dbContext.SaveChanges();

            return geodatalovDataset;
        }

        private void GetDeliveryStatuses(GeodatalovDatasetViewModel geodatalovDatasetViewModel, GeodatalovDataset geodatalovDataset)
        {
            geodatalovDatasetViewModel.MetadataStatus = _datasetDeliveryService.GetMetadataStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.MetadataStatus);
            geodatalovDatasetViewModel.ProductSpesificationStatus = _registerService.GetDOKStatus(geodatalovDatasetViewModel.ProductSpecificationUrl, true, geodatalovDatasetViewModel.ProductSpesificationStatus); ;
            geodatalovDatasetViewModel.SosiDataStatus = _registerService.GetSosiRequirements(geodatalovDataset.Uuid, geodatalovDatasetViewModel.ProductSpecificationUrl, true, geodatalovDatasetViewModel.SosiDataStatus);
            geodatalovDatasetViewModel.GmlDataStatus = _registerService.GetGmlRequirements(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.GmlDataStatus);
            geodatalovDatasetViewModel.WmsStatus = _datasetDeliveryService.GetDokDeliveryServiceStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.WmsStatus, geodatalovDataset.UuidService);
            geodatalovDatasetViewModel.WfsStatus = _datasetDeliveryService.GetWfsStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.WfsStatus);
            geodatalovDatasetViewModel.AtomFeedStatus = _datasetDeliveryService.GetAtomFeedStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.AtomFeedStatus);
            geodatalovDatasetViewModel.CommonStatus = "notset"; // TODO
        }
    }
}