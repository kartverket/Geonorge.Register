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
            _metadataService = new MetadataService(_dbContext);
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
            geodatalovDataset.MetadataStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.MetadataStatusId, geodatalovViewModel.MetadataNote);
            geodatalovDataset.ProductSpesificationStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WmsStatusId, geodatalovViewModel.WmsNote);
            geodatalovDataset.SosiDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WmsStatusId, geodatalovViewModel.WmsNote);
            geodatalovDataset.GmlDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WmsStatusId, geodatalovViewModel.WmsNote);
            geodatalovDataset.WmsStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WmsStatusId, geodatalovViewModel.WmsNote);
            geodatalovDataset.WfsStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WfsStatusId, geodatalovViewModel.WfsNote);
            geodatalovDataset.AtomFeedStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.AtomFeedStatusId, geodatalovViewModel.AtomFeedNote);
            geodatalovDataset.CommonStatusId = _datasetDeliveryService.CreateDatasetDelivery(geodatalovViewModel.WmsStatusId, geodatalovViewModel.WmsNote);
            _dbContext.GeodatalovDatasets.Add(geodatalovDataset);
            _dbContext.SaveChanges();

            return geodatalovDataset;
        }

        private void GetDeliveryStatuses(GeodatalovDatasetViewModel geodatalovDatasetViewModel, GeodatalovDataset geodatalovDataset)
        {
            geodatalovDatasetViewModel.MetadataStatusId = _datasetDeliveryService.GetMetadataStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.MetadataStatusId);
            geodatalovDatasetViewModel.ProductSpesificationStatusId = _registerService.GetDOKStatus(geodatalovDataset.ProductSpecificationUrl, true, geodatalovDatasetViewModel.ProductSpesificationStatusId);
            geodatalovDatasetViewModel.SosiDataStatusId = _registerService.GetSosiRequirements(geodatalovDataset.Uuid, geodatalovDatasetViewModel.ProductSpecificationUrl, true, geodatalovDatasetViewModel.SosiDataStatusId);
            geodatalovDatasetViewModel.GmlDataStatusId = _registerService.GetGmlRequirements(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.GmlDataStatusId);
            geodatalovDatasetViewModel.WmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.WmsStatusId, geodatalovDataset.UuidService);
            geodatalovDatasetViewModel.WfsStatusId = _datasetDeliveryService.GetWfsStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.WfsStatusId);
            geodatalovDatasetViewModel.AtomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(geodatalovDataset.Uuid, true, geodatalovDatasetViewModel.AtomFeedStatusId);
            geodatalovDatasetViewModel.CommonStatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(geodatalovDatasetViewModel.WfsStatusId, geodatalovDatasetViewModel.AtomFeedStatusId);
        }

        public GeodatalovDataset UpdateGeodatalovDatasetFromKartkatalogen(GeodatalovDataset originalDataset)
        {
            var geodatalovDatasetFromKartkatalogen = _metadataService.FetchGeodatalovDatasetFromKartkatalogen(originalDataset.Uuid);
            return geodatalovDatasetFromKartkatalogen == null ? originalDataset : UpdateGeodatalovDataset(originalDataset, geodatalovDatasetFromKartkatalogen);
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
                geodatalovDataset.MetadataStatus.StatusId = viewModel.MetadataStatusId;
                geodatalovDataset.MetadataStatus.Note = viewModel.MetadataNote;
                geodatalovDataset.MetadataStatus.AutoUpdate = viewModel.MetadataAutoUpdate;
            }

            if (geodatalovDataset.ProductSpesificationStatus != null)
            {
                geodatalovDataset.ProductSpesificationStatus.StatusId = viewModel.ProductSpesificationStatusId;
                geodatalovDataset.ProductSpesificationStatus.Note = viewModel.ProduktspesifikasjonNote;
                geodatalovDataset.ProductSpesificationStatus.AutoUpdate = viewModel.ProduktspesifikasjonAutoUpdate;
            }

            if (geodatalovDataset.SosiDataStatus != null)
            {
                geodatalovDataset.SosiDataStatus.StatusId = viewModel.SosiDataStatusId;
                geodatalovDataset.SosiDataStatus.Note = viewModel.SosiDataNote;
                geodatalovDataset.SosiDataStatus.AutoUpdate = viewModel.SosiDataAutoUpdate;
            }

            if (geodatalovDataset.GmlDataStatus != null)
            {
                geodatalovDataset.GmlDataStatus.StatusId = viewModel.GmlDataStatusId;
                geodatalovDataset.GmlDataStatus.Note = viewModel.GmlDataNote;
                geodatalovDataset.GmlDataStatus.AutoUpdate = viewModel.GmlDataAutoUpdate;
            }

            if (geodatalovDataset.WmsStatus != null)
            {
                geodatalovDataset.WmsStatus.StatusId = viewModel.WmsStatusId;
                geodatalovDataset.WmsStatus.Note = viewModel.WmsNote;
                geodatalovDataset.WmsStatus.AutoUpdate = viewModel.WmsAutoUpdate;
            }

            if (geodatalovDataset.WfsStatus != null)
            {
                geodatalovDataset.WfsStatus.StatusId = viewModel.WfsStatusId;
                geodatalovDataset.WfsStatus.Note = viewModel.WfsNote;
                geodatalovDataset.WfsStatus.AutoUpdate = viewModel.WfsAutoUpdate;
            }

            if (geodatalovDataset.AtomFeedStatus != null)
            {
                geodatalovDataset.AtomFeedStatus.StatusId = viewModel.AtomFeedStatusId;
                geodatalovDataset.AtomFeedStatus.Note = viewModel.AtomFeedNote;
                geodatalovDataset.AtomFeedStatus.AutoUpdate = viewModel.AtomFeedAutoUpdate;
            }

            if (geodatalovDataset.CommonStatus != null)
            {
                geodatalovDataset.CommonStatus.StatusId = viewModel.CommonStatusId;
                geodatalovDataset.CommonStatus.Note = viewModel.CommonNote;
                geodatalovDataset.CommonStatus.AutoUpdate = viewModel.CommonAutoUpdate;
            }

            _dbContext.Entry(geodatalovDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return geodatalovDataset;
        }

        public void DeleteGeodatalovDataset(GeodatalovDataset geodatalovDataset)
        {
            _dbContext.GeodatalovDatasets.Remove(geodatalovDataset);

            //Todo, må slette deliveryDataset?
            _dbContext.SaveChanges();
        }

        public GeodatalovDataset GetGeodatalovDatasetById(string uuid)
        {
            Guid guid = Guid.Parse(uuid);
            var queryResult = from i in _dbContext.GeodatalovDatasets
                where i.Uuid == uuid
                      || i.SystemId == guid
                              select i;

            return queryResult.FirstOrDefault();
        }


        private GeodatalovDataset GetGeodatalovDatasetBySystemId(Guid systemId)
        {
            var queryResult = from i in _dbContext.GeodatalovDatasets
                where i.SystemId == systemId
                select i;

            return queryResult.FirstOrDefault();
        }

        public void SynchronizeGeodatalovDatasets()
        {
            var geodatalovDatasetsFromKartkatalogen = FetchGeodatalovDatasetsFromKartkatalogen();
            RemoveGeodatalovDatasets(geodatalovDatasetsFromKartkatalogen);
            UpdateGeodatalovDataset(geodatalovDatasetsFromKartkatalogen);
            

            _dbContext.SaveChanges();
        }

        private void UpdateGeodatalovDataset(List<GeodatalovDataset> geodatalovDatasetsFromKartkatalogen)
        {
            //Update register
            foreach (var geodatalovDataset in geodatalovDatasetsFromKartkatalogen)
            {
                var originalGeodatalovDataset = GetGeodatalovDatasetByUuid(geodatalovDataset.Uuid);
                if (originalGeodatalovDataset != null)
                {
                    UpdateGeodatalovDataset(originalGeodatalovDataset, geodatalovDataset);
                }
                else
                {
                    NewGeodatalovDatasetFromKartkatalogen(geodatalovDataset);
                }
            }
        }

        private void NewGeodatalovDatasetFromKartkatalogen(GeodatalovDataset geodatalovDataset)
        {
            if (!_registerItemService.ItemNameIsValid(geodatalovDataset)) return;
            geodatalovDataset.SystemId = Guid.NewGuid();
            geodatalovDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(geodatalovDataset.Name);
            geodatalovDataset.SubmitterId = _registerService.GetOrganizationIdByUserName();
            geodatalovDataset.DateSubmitted = DateTime.Now;
            geodatalovDataset.Modified = DateTime.Now;
            geodatalovDataset.RegisterId = _registerService.GetGeodatalovStatusRegisterId();
            geodatalovDataset.VersioningId = _registerItemService.NewVersioningGroup(geodatalovDataset);
            geodatalovDataset.VersionNumber = 1;
            geodatalovDataset.StatusId = "Submitted";
            geodatalovDataset.DokStatusId = "Proposal";

            //GetDeliveryStatuses(inspireDatasetViewModel, inspireDataset);
            var metadataStatusId = _datasetDeliveryService.GetMetadataStatus(geodatalovDataset.Uuid);
            var productSpesificationStatusId = _registerService.GetDOKStatus(geodatalovDataset.ProductSpecificationUrl, true, "deficient");
            var sosiDataStatusId = _registerService.GetSosiRequirements(geodatalovDataset.Uuid, "", true, "deficient");
            var gmlDataStatusId = _registerService.GetGmlRequirements(geodatalovDataset.Uuid, true, "deficient");
            var wmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(geodatalovDataset.Uuid, true, "deficient", geodatalovDataset.UuidService);
            var wfsStatusId = _datasetDeliveryService.GetWfsStatus(geodatalovDataset.Uuid);
            var atomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(geodatalovDataset.Uuid);
            var commonStatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(wfsStatusId, atomFeedStatusId);

            geodatalovDataset.MetadataStatusId = _datasetDeliveryService.CreateDatasetDelivery(metadataStatusId);
            geodatalovDataset.ProductSpesificationStatusId = _datasetDeliveryService.CreateDatasetDelivery(productSpesificationStatusId);
            geodatalovDataset.SosiDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(sosiDataStatusId);
            geodatalovDataset.GmlDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(gmlDataStatusId);
            geodatalovDataset.WmsStatusId = _datasetDeliveryService.CreateDatasetDelivery(wmsStatusId);
            geodatalovDataset.WfsStatusId = _datasetDeliveryService.CreateDatasetDelivery(wfsStatusId);
            geodatalovDataset.AtomFeedStatusId = _datasetDeliveryService.CreateDatasetDelivery(atomFeedStatusId);
            geodatalovDataset.CommonStatusId = _datasetDeliveryService.CreateDatasetDelivery(commonStatusId);
            _dbContext.GeodatalovDatasets.Add(geodatalovDataset);

            _dbContext.GeodatalovDatasets.Add(geodatalovDataset);
            _dbContext.SaveChanges();
        }

        private GeodatalovDataset UpdateGeodatalovDataset(GeodatalovDataset originalDataset, GeodatalovDataset geodatalovDatasetFromKartkatalogen)
        {
            originalDataset.Name = geodatalovDatasetFromKartkatalogen.Name;
            originalDataset.Seoname = RegisterUrls.MakeSeoFriendlyString(originalDataset.Name);
            originalDataset.Description = geodatalovDatasetFromKartkatalogen.Description;
            originalDataset.OwnerId = geodatalovDatasetFromKartkatalogen.OwnerId;
            originalDataset.Modified = DateTime.Now;

            originalDataset.Uuid = geodatalovDatasetFromKartkatalogen.Uuid;
            originalDataset.Notes = geodatalovDatasetFromKartkatalogen.Notes;
            originalDataset.SpecificUsage = geodatalovDatasetFromKartkatalogen.SpecificUsage;
            originalDataset.ProductSheetUrl = geodatalovDatasetFromKartkatalogen.ProductSheetUrl;
            originalDataset.PresentationRulesUrl = geodatalovDatasetFromKartkatalogen.PresentationRulesUrl;
            originalDataset.ProductSpecificationUrl = geodatalovDatasetFromKartkatalogen.ProductSpecificationUrl;
            originalDataset.MetadataUrl = geodatalovDatasetFromKartkatalogen.MetadataUrl;
            originalDataset.DistributionFormat = geodatalovDatasetFromKartkatalogen.DistributionFormat;
            originalDataset.DistributionUrl = geodatalovDatasetFromKartkatalogen.DistributionUrl;
            originalDataset.DistributionArea = geodatalovDatasetFromKartkatalogen.DistributionArea;
            originalDataset.WmsUrl = geodatalovDatasetFromKartkatalogen.WmsUrl;
            originalDataset.ThemeGroupId = geodatalovDatasetFromKartkatalogen.ThemeGroupId;
            originalDataset.DatasetThumbnail = geodatalovDatasetFromKartkatalogen.DatasetThumbnail;
            originalDataset.UuidService = geodatalovDatasetFromKartkatalogen.UuidService;

            originalDataset.InspireTheme = geodatalovDatasetFromKartkatalogen.InspireTheme;
            originalDataset.Dok = geodatalovDatasetFromKartkatalogen.Dok;
            originalDataset.NationalDataset = geodatalovDatasetFromKartkatalogen.NationalDataset;
            originalDataset.Plan = geodatalovDatasetFromKartkatalogen.Plan;
            originalDataset.Geodatalov = geodatalovDatasetFromKartkatalogen.Geodatalov;

            if (originalDataset.MetadataStatus != null)
            {
                originalDataset.MetadataStatus.StatusId = _datasetDeliveryService.GetMetadataStatus(geodatalovDatasetFromKartkatalogen.Uuid, true, originalDataset.MetadataStatus.StatusId);
            }
            if (originalDataset.ProductSpesificationStatus != null)
            {
                originalDataset.ProductSpesificationStatus.StatusId = _registerService.GetDOKStatus(geodatalovDatasetFromKartkatalogen.ProductSpecificationUrl, true, originalDataset.ProductSpesificationStatus.StatusId);
            }
            if (originalDataset.SosiDataStatus != null)
            {
                originalDataset.SosiDataStatus.StatusId = _registerService.GetSosiRequirements(geodatalovDatasetFromKartkatalogen.Uuid, originalDataset.ProductSpecificationUrl, true, originalDataset.SosiDataStatus.StatusId);
            }

            if (originalDataset.GmlDataStatus != null)
            {
                originalDataset.GmlDataStatus.StatusId = _registerService.GetGmlRequirements(geodatalovDatasetFromKartkatalogen.Uuid, true, originalDataset.GmlDataStatus.StatusId);
            }

            if (originalDataset.WmsStatus != null)
            {
                originalDataset.WmsStatus.StatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(geodatalovDatasetFromKartkatalogen.Uuid, true, originalDataset.WmsStatus.StatusId, geodatalovDatasetFromKartkatalogen.UuidService);
            }

            if (originalDataset.WfsStatus != null)
            {
                originalDataset.WfsStatus.StatusId = _datasetDeliveryService.GetWfsStatus(geodatalovDatasetFromKartkatalogen.Uuid, true, originalDataset.WfsStatus.StatusId);
            }

            if (originalDataset.AtomFeedStatus != null)
            {
                originalDataset.AtomFeedStatus.StatusId = _datasetDeliveryService.GetAtomFeedStatus(geodatalovDatasetFromKartkatalogen.Uuid, true, originalDataset.AtomFeedStatus.StatusId);
            }

            if (originalDataset.CommonStatus != null)
            {
                    originalDataset.CommonStatus.StatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(originalDataset.WfsStatus?.StatusId, originalDataset.AtomFeedStatus?.StatusId);
            }

            _dbContext.Entry(originalDataset).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return originalDataset;
        }

        public GeodatalovDataset GetGeodatalovDatasetByUuid(string uuid)
        {
            var queryResult = from i in _dbContext.GeodatalovDatasets
                where i.Uuid == uuid
                select i;

            return queryResult.FirstOrDefault();
        }

        private void RemoveGeodatalovDatasets(List<GeodatalovDataset> geodatalovDatasetsFromKartkatalogen)
        {
            var geodatalovDatasetsFromRegister = GetGeodatalovDatasets();
            var exists = false;
            var removeDatasets = new List<GeodatalovDataset>();

            foreach (var geodatalovDatasetFromRegister in geodatalovDatasetsFromRegister)
            {
                if (geodatalovDatasetsFromKartkatalogen.Any(geodatalovDatasetFromKartkatalog => geodatalovDatasetFromKartkatalog.Uuid == geodatalovDatasetFromRegister.Uuid))
                {
                    exists = true;
                }
                if (!exists)
                {
                    removeDatasets.Add(geodatalovDatasetFromRegister);
                }
                exists = false;
            }
            foreach (var geodatalovDataset in removeDatasets)
            {
                DeleteGeodatalovDataset(geodatalovDataset);
            }
        }

        private List<GeodatalovDataset> GetGeodatalovDatasets()
        {
            var queryResultsRegisterItem = from d in _dbContext.GeodatalovDatasets
                where !string.IsNullOrEmpty(d.Uuid)
                select d;

            var geodatalovDatasets = queryResultsRegisterItem.ToList();
            return geodatalovDatasets;
        }

        private List<GeodatalovDataset> FetchGeodatalovDatasetsFromKartkatalogen()
        {
            var geodatalovDatasetsFromKartkatalogen = new List<GeodatalovDataset>();

            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/datasets?facets%5b0%5dname=nationalinitiative&facets%5b0%5dvalue=geodataloven&facets%5b1%5dname=nationalinitiative&facets%5b1%5dvalue=Norge digitalt&Offset=1&limit=6000&mediatype=json";
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
                        var geodatalovDataset = _metadataService.FetchGeodatalovDatasetFromKartkatalogen(item.Uuid.ToString());
                        if (geodatalovDataset != null)
                        {
                            geodatalovDatasetsFromKartkatalogen.Add(geodatalovDataset);
                        }
                    }
                }
                return geodatalovDatasetsFromKartkatalogen;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }
        }
    }
}