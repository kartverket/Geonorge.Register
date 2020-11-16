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
    public class MareanoDatasetService : IMareanoDatasetService
    {
        private readonly RegisterDbContext _dbContext;
        private readonly IRegisterService _registerService;
        private readonly IDatasetDeliveryService _datasetDeliveryService;
        private readonly IRegisterItemService _registerItemService;
        private readonly MetadataService _metadataService;

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
            _dbContext.MareanoDatasets.Add(MareanoDataset);
            _dbContext.SaveChanges();

            return MareanoDataset;
        }

        private void GetDeliveryStatuses(MareanoDatasetViewModel MareanoDatasetViewModel, MareanoDataset MareanoDataset)
        {
            MareanoDatasetViewModel.MetadataStatusId = _datasetDeliveryService.GetMetadataStatus(MareanoDataset.Uuid, true, MareanoDatasetViewModel.MetadataStatusId);
            MareanoDatasetViewModel.ProductSpesificationStatusId = _registerService.GetDOKStatus(MareanoDataset.ProductSpecificationUrl, true, MareanoDatasetViewModel.ProductSpesificationStatusId);
            MareanoDatasetViewModel.SosiDataStatusId = _registerService.GetSosiRequirements(MareanoDataset.Uuid, MareanoDatasetViewModel.ProductSpecificationUrl, true, MareanoDatasetViewModel.SosiDataStatusId);
            MareanoDatasetViewModel.GmlDataStatusId = _registerService.GetGmlRequirements(MareanoDataset.Uuid, true, MareanoDatasetViewModel.GmlDataStatusId);
            MareanoDatasetViewModel.WmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(MareanoDataset.Uuid, true, MareanoDatasetViewModel.WmsStatusId, MareanoDataset.UuidService);
            MareanoDatasetViewModel.WfsStatusId = _datasetDeliveryService.GetWfsStatus(MareanoDataset.Uuid, true, MareanoDatasetViewModel.WfsStatusId);
            MareanoDatasetViewModel.AtomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(MareanoDataset.Uuid, true, MareanoDatasetViewModel.AtomFeedStatusId);
            MareanoDatasetViewModel.CommonStatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(MareanoDatasetViewModel.WfsStatusId, MareanoDatasetViewModel.AtomFeedStatusId);
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

            //GetDeliveryStatuses(inspireDatasetViewModel, inspireDataset);
            var metadataStatusId = _datasetDeliveryService.GetMetadataStatus(MareanoDataset.Uuid);
            var productSpesificationStatusId = _registerService.GetDOKStatus(MareanoDataset.ProductSpecificationUrl, true, "deficient");
            var sosiDataStatusId = _registerService.GetSosiRequirements(MareanoDataset.Uuid, "", true, "deficient");
            var gmlDataStatusId = _registerService.GetGmlRequirements(MareanoDataset.Uuid, true, "deficient");
            var wmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(MareanoDataset.Uuid, true, "deficient", MareanoDataset.UuidService);
            var wfsStatusId = _datasetDeliveryService.GetWfsStatus(MareanoDataset.Uuid);
            var atomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(MareanoDataset.Uuid);
            var commonStatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(wfsStatusId, atomFeedStatusId);

            MareanoDataset.MetadataStatusId = _datasetDeliveryService.CreateDatasetDelivery(metadataStatusId);
            MareanoDataset.ProductSpesificationStatusId = _datasetDeliveryService.CreateDatasetDelivery(productSpesificationStatusId);
            MareanoDataset.SosiDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(sosiDataStatusId);
            MareanoDataset.GmlDataStatusId = _datasetDeliveryService.CreateDatasetDelivery(gmlDataStatusId);
            MareanoDataset.WmsStatusId = _datasetDeliveryService.CreateDatasetDelivery(wmsStatusId);
            MareanoDataset.WfsStatusId = _datasetDeliveryService.CreateDatasetDelivery(wfsStatusId);
            MareanoDataset.AtomFeedStatusId = _datasetDeliveryService.CreateDatasetDelivery(atomFeedStatusId);
            MareanoDataset.CommonStatusId = _datasetDeliveryService.CreateDatasetDelivery(commonStatusId);
            _dbContext.MareanoDatasets.Add(MareanoDataset);

            _dbContext.MareanoDatasets.Add(MareanoDataset);
            _dbContext.SaveChanges();
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
                originalDataset.WfsStatus.StatusId = _datasetDeliveryService.GetWfsStatus(MareanoDatasetFromKartkatalogen.Uuid, true, originalDataset.WfsStatus.StatusId);
            }

            if (originalDataset.AtomFeedStatus != null)
            {
                originalDataset.AtomFeedStatus.StatusId = _datasetDeliveryService.GetAtomFeedStatus(MareanoDatasetFromKartkatalogen.Uuid, true, originalDataset.AtomFeedStatus.StatusId);
            }

            if (originalDataset.CommonStatus != null)
            {
                originalDataset.CommonStatus.StatusId = _datasetDeliveryService.GetDownloadRequirementsStatus(originalDataset.WfsStatus?.StatusId, originalDataset.AtomFeedStatus?.StatusId);
            }

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

            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/datasets?facets%5b0%5dname=nationalinitiative&facets%5b0%5dvalue=Mareano&Offset=1&limit=6000&mediatype=json";
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
    }
}