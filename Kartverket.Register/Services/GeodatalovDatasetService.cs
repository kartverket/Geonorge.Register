using System;
using System.Data.Entity;
using Kartverket.Register.Models;
using System.Linq;
using Kartverket.DOK.Service;
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
        private readonly MetadataService _metadataService;

        public GeodatalovDatasetService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _registerService = new RegisterService(_dbContext);
            _datasetDeliveryService = new DatasetDeliveryService(_dbContext);
            _registerItemService = new RegisterItemService(_dbContext);
            _metadataService = new MetadataService();
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

        public GeodatalovDataset NewGeodatalovDataset(GeodatalovDatasetViewModel geodatalovViewModel, string parentregister, string registername)
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
            geodatalovDatasetViewModel.ProductSpesificationStatus = _registerService.GetDOKStatus(geodatalovDataset.ProductSpecificationUrl, true, geodatalovDatasetViewModel.ProductSpesificationStatus);
            geodatalovDatasetViewModel.SosiDataStatus = _registerService.GetSosiRequirements(geodatalovDataset.Uuid, geodatalovDatasetViewModel.ProductSpecificationUrl, true, geodatalovDatasetViewModel.SosiDataStatus);
            geodatalovDatasetViewModel.GmlDataStatus = _registerService.GetGmlRequirements(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.GmlDataStatus);
            geodatalovDatasetViewModel.WmsStatus = _datasetDeliveryService.GetDokDeliveryServiceStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.WmsStatus, geodatalovDataset.UuidService);
            geodatalovDatasetViewModel.WfsStatus = _datasetDeliveryService.GetWfsStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.WfsStatus);
            geodatalovDatasetViewModel.AtomFeedStatus = _datasetDeliveryService.GetAtomFeedStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.AtomFeedStatus);
            geodatalovDatasetViewModel.CommonStatus = "notset"; // TODO
        }

        public GeodatalovDataset UpdateGeodatalovDatasetFromKartkatalogen(GeodatalovDataset originalDataset)
        {
            var geodatalovDataset = _metadataService.FetchInspireDatasetFromKartkatalogen(originalDataset.Uuid);
            if (geodatalovDataset == null)
            {
                return originalDataset; //Skal datasettet da fjernes??
            }
            originalDataset.Name = geodatalovDataset.Name;
            originalDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(originalDataset.Name);
            originalDataset.Description = geodatalovDataset.Description;
            originalDataset.OwnerId = geodatalovDataset.OwnerId;
            originalDataset.Modified = DateTime.Now;

            originalDataset.Uuid = geodatalovDataset.Uuid;
            originalDataset.Notes = geodatalovDataset.Notes;
            originalDataset.SpecificUsage = geodatalovDataset.SpecificUsage;
            originalDataset.ProductSheetUrl = geodatalovDataset.ProductSheetUrl;
            originalDataset.PresentationRulesUrl = geodatalovDataset.PresentationRulesUrl;
            originalDataset.ProductSpecificationUrl = geodatalovDataset.ProductSpecificationUrl;
            originalDataset.MetadataUrl = geodatalovDataset.MetadataUrl;
            originalDataset.DistributionFormat = geodatalovDataset.DistributionFormat;
            originalDataset.DistributionUrl = geodatalovDataset.DistributionUrl;
            originalDataset.DistributionArea = geodatalovDataset.DistributionArea;
            originalDataset.WmsUrl = geodatalovDataset.WmsUrl;
            originalDataset.ThemeGroupId = geodatalovDataset.ThemeGroupId;
            originalDataset.DatasetThumbnail = geodatalovDataset.DatasetThumbnail;
            originalDataset.UuidService = geodatalovDataset.UuidService;

            if (originalDataset.MetadataStatus != null)
            {
                originalDataset.MetadataStatus.StatusId = _datasetDeliveryService.GetMetadataStatus(geodatalovDataset.Uuid, true, originalDataset.MetadataStatus.StatusId);
            }
            if (originalDataset.ProductSpesificationStatus != null)
            {
                originalDataset.ProductSpesificationStatus.StatusId = _registerService.GetDOKStatus(geodatalovDataset.ProductSpecificationUrl, true, originalDataset.ProductSpesificationStatus.StatusId);
            }
            if (originalDataset.SosiDataStatus != null)
            {
                originalDataset.SosiDataStatus.StatusId = _registerService.GetSosiRequirements(geodatalovDataset.Uuid, originalDataset.ProductSpecificationUrl, true, originalDataset.SosiDataStatus.StatusId);
            }

            if (originalDataset.GmlDataStatus != null)
            {
                originalDataset.GmlDataStatus.StatusId = _registerService.GetGmlRequirements(geodatalovDataset.Uuid, true, originalDataset.GmlDataStatus.StatusId);
            }

            if (originalDataset.WmsStatus != null)
            {
                originalDataset.WmsStatus.StatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(geodatalovDataset.Uuid, true, originalDataset.WmsStatus.StatusId, geodatalovDataset.UuidService);
            }

            if (originalDataset.WfsStatus != null)
            {
                originalDataset.WfsStatus.StatusId = _datasetDeliveryService.GetWfsStatus(geodatalovDataset.Uuid, true, originalDataset.WfsStatus.StatusId);
            }

            if (originalDataset.AtomFeedStatus != null)
            {
                originalDataset.AtomFeedStatus.StatusId = _datasetDeliveryService.GetAtomFeedStatus(geodatalovDataset.Uuid, true, originalDataset.AtomFeedStatus.StatusId);
            }

            if (originalDataset.CommonStatus != null)
            {
                originalDataset.CommonStatus.StatusId = "notset";
            }

            _dbContext.Entry(originalDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return originalDataset;
        }

        public GeodatalovDataset UpdateGeodatalovDataset(GeodatalovDatasetViewModel viewModel)
        {
            var geodatalovDataset = GetGeodatalovDatasetBySystemId(viewModel.SystemId);
            geodatalovDataset.Name = viewModel.Name;
            geodatalovDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(geodatalovDataset.Name);
            geodatalovDataset.Description = viewModel.Description;
            geodatalovDataset.SubmitterId = viewModel.SubmitterId;
            geodatalovDataset.OwnerId = viewModel.OwnerId;
            geodatalovDataset.Modified = DateTime.Now;
            geodatalovDataset.DateSubmitted = DateTime.Now;
            geodatalovDataset.RegisterId = viewModel.RegisterId;

            geodatalovDataset.Uuid = viewModel.Uuid;
            geodatalovDataset.Notes = viewModel.Notes;
            geodatalovDataset.SpecificUsage = viewModel.SpecificUsage;
            geodatalovDataset.ProductSheetUrl = viewModel.ProductSheetUrl;
            geodatalovDataset.PresentationRulesUrl = viewModel.PresentationRulesUrl;
            geodatalovDataset.ProductSpecificationUrl = viewModel.ProductSpecificationUrl;
            geodatalovDataset.MetadataUrl = viewModel.MetadataUrl;
            geodatalovDataset.DistributionFormat = viewModel.DistributionFormat;
            geodatalovDataset.DistributionUrl = viewModel.DistributionUrl;
            geodatalovDataset.DistributionArea = viewModel.DistributionArea;
            geodatalovDataset.WmsUrl = viewModel.WmsUrl;
            geodatalovDataset.ThemeGroupId = viewModel.ThemeGroupId;
            geodatalovDataset.DatasetThumbnail = viewModel.DatasetThumbnail;
            geodatalovDataset.DokStatusId = viewModel.DokStatusId;
            geodatalovDataset.DokStatusDateAccepted = viewModel.GetDateAccepted();
            geodatalovDataset.UuidService = viewModel.UuidService;

            if (geodatalovDataset.MetadataStatus != null)
            {
                geodatalovDataset.MetadataStatus.StatusId = viewModel.MetadataStatus;
                geodatalovDataset.MetadataStatus.Note = viewModel.MetadataNote;
                geodatalovDataset.MetadataStatus.AutoUpdate = viewModel.MetadataAutoUpdate;
            }

            if (geodatalovDataset.ProductSpesificationStatus != null)
            {
                geodatalovDataset.ProductSpesificationStatus.StatusId = viewModel.ProductSpesificationStatus;
                geodatalovDataset.ProductSpesificationStatus.Note = viewModel.ProduktspesifikasjonNote;
                geodatalovDataset.ProductSpesificationStatus.AutoUpdate = viewModel.ProduktspesifikasjonAutoUpdate;
            }

            if (geodatalovDataset.SosiDataStatus != null)
            {
                geodatalovDataset.SosiDataStatus.StatusId = viewModel.SosiDataStatus;
                geodatalovDataset.SosiDataStatus.Note = viewModel.SosiDataNote;
                geodatalovDataset.SosiDataStatus.AutoUpdate = viewModel.SosiDataAutoUpdate;
            }

            if (geodatalovDataset.GmlDataStatus != null)
            {
                geodatalovDataset.GmlDataStatus.StatusId = viewModel.GmlDataStatus;
                geodatalovDataset.GmlDataStatus.Note = viewModel.GmlDataNote;
                geodatalovDataset.GmlDataStatus.AutoUpdate = viewModel.GmlDataAutoUpdate;
            }

            if (geodatalovDataset.WmsStatus != null)
            {
                geodatalovDataset.WmsStatus.StatusId = viewModel.WmsStatus;
                geodatalovDataset.WmsStatus.Note = viewModel.WmsNote;
                geodatalovDataset.WmsStatus.AutoUpdate = viewModel.WmsAutoUpdate;
            }

            if (geodatalovDataset.WfsStatus != null)
            {
                geodatalovDataset.WfsStatus.StatusId = viewModel.WfsStatus;
                geodatalovDataset.WfsStatus.Note = viewModel.WfsNote;
                geodatalovDataset.WfsStatus.AutoUpdate = viewModel.WfsAutoUpdate;
            }

            if (geodatalovDataset.AtomFeedStatus != null)
            {
                geodatalovDataset.AtomFeedStatus.StatusId = viewModel.AtomFeedStatus;
                geodatalovDataset.AtomFeedStatus.Note = viewModel.AtomFeedNote;
                geodatalovDataset.AtomFeedStatus.AutoUpdate = viewModel.AtomFeedAutoUpdate;
            }

            if (geodatalovDataset.CommonStatus != null)
            {
                geodatalovDataset.CommonStatus.StatusId = viewModel.CommonStatus;
                geodatalovDataset.CommonStatus.Note = viewModel.CommonNote;
                geodatalovDataset.CommonStatus.AutoUpdate = viewModel.CommonAutoUpdate;
            }

            _dbContext.Entry(geodatalovDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return geodatalovDataset;
        }

        private GeodatalovDataset GetGeodatalovDatasetBySystemId(Guid systemId)
        {
            var queryResult = from i in _dbContext.GeodatalovDatasets
                where i.SystemId == systemId
                select i;

            return queryResult.FirstOrDefault();
        }
    }
}