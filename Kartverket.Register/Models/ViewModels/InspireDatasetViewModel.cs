using Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireDatasetViewModel : DatasetViewModel
    {
        [Display(Name = "InspireTheme", ResourceType = typeof(InspireDataSet))]
        public string InspireTheme { get; set; }

        [Display(Name = "Metadata:")]
        public string MetadataStatusId { get; set; }
        public virtual DokDeliveryStatus MetadataStatus { get; set; }
        public string MetadataNote { get; set; }
        public bool MetadataAutoUpdate { get; set; }

        [Display(Name = "MetadataServiceStatus", ResourceType = typeof(InspireDataSet))]
        public string MetadataServiceStatusId { get; set; }
        public virtual DokDeliveryStatus MetadataServiceStatus { get; set; }
        public string MetadataServiceNote { get; set; }
        public bool MetadataServiceAutoUpdate { get; set; }

        [Display(Name = "DistributionStatus", ResourceType = typeof(InspireDataSet))]
        public string DistributionStatusId { get; set; }
        public virtual DokDeliveryStatus DistributionStatus { get; set; }
        public string DistributionNote { get; set; }
        public bool DistributionAutoUpdate { get; set; }

        [Display(Name = "WMS:")]
        public string WmsStatusId { get; set; }
        public virtual DokDeliveryStatus WmsStatus { get; set; }
        public string WmsNote { get; set; }
        public bool WmsAutoUpdate { get; set; }

        [Display(Name = "WfsStatus", ResourceType = typeof(InspireDataSet))]
        public string WfsStatusId { get; set; }
        public virtual DokDeliveryStatus WfsStatus { get; set; }
        public string WfsNote { get; set; }
        public bool WfsAutoUpdate { get; set; }

        [Display(Name = "AtomFeedStatus", ResourceType = typeof(InspireDataSet))]
        public string AtomFeedStatusId { get; set; }
        public virtual DokDeliveryStatus AtomFeedStatus { get; set; }
        public string AtomFeedNote { get; set; }
        public bool AtomFeedAutoUpdate { get; set; }

        [Display(Name = "WfsOrAtomStatus", ResourceType = typeof(InspireDataSet))]
        public string WfsOrAtomStatusId { get; set; }
        public virtual DokDeliveryStatus WfsOrAtomStatus { get; set; }
        public string WfsOrAtomNote { get; set; }
        public bool WfsOrAtomAutoUpdate { get; set; }

        [Display(Name = "HarmonizedDataStatus", ResourceType = typeof(InspireDataSet))]
        public string HarmonizedDataStatusId { get; set; }
        public virtual DokDeliveryStatus HarmonizedDataStatus { get; set; }
        public string HarmonizedDataNote { get; set; }
        public bool HarmonizedDataAutoUpdate { get; set; }

        [Display(Name = "SpatialDataServiceStatus", ResourceType = typeof(InspireDataSet))]
        public string SpatialDataServiceStatusId { get; set; }
        public virtual DokDeliveryStatus SpatialDataServiceStatus { get; set; }
        public string SpatialDataServiceNote { get; set; }
        public bool SpatialDataServiceAutoUpdate { get; set; }

        public InspireDatasetViewModel(InspireDataset item)
        {
            Update(item);
        }

        public InspireDatasetViewModel()
        {
        }

        public void Update(InspireDataset inspireDataset)
        {
            if (inspireDataset != null)
            {
                InspireTheme = inspireDataset.InspireTheme;
                if (inspireDataset.InspireDeliveryMetadata != null)
                {
                    MetadataStatusId = inspireDataset.InspireDeliveryMetadata.StatusId;
                    MetadataStatus = inspireDataset.InspireDeliveryMetadata.Status;
                    MetadataNote = inspireDataset.InspireDeliveryMetadata.Note;
                    MetadataAutoUpdate = inspireDataset.InspireDeliveryMetadata.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryMetadataService != null)
                {
                    MetadataServiceStatusId = inspireDataset.InspireDeliveryMetadataService.StatusId;
                    MetadataServiceStatus = inspireDataset.InspireDeliveryMetadataService.Status;
                    MetadataServiceNote = inspireDataset.InspireDeliveryMetadataService.Note;
                    MetadataServiceAutoUpdate = inspireDataset.InspireDeliveryMetadataService.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryDistribution != null)
                {
                    DistributionStatusId = inspireDataset.InspireDeliveryDistribution.StatusId;
                    DistributionStatus = inspireDataset.InspireDeliveryDistribution.Status;
                    DistributionNote = inspireDataset.InspireDeliveryDistribution.Note;
                    DistributionAutoUpdate = inspireDataset.InspireDeliveryDistribution.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryWms != null)
                {
                    WmsStatusId = inspireDataset.InspireDeliveryWms.StatusId;
                    WmsStatus = inspireDataset.InspireDeliveryWms.Status;
                    WmsNote = inspireDataset.InspireDeliveryWms.Note;
                    WmsAutoUpdate = inspireDataset.InspireDeliveryWms.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryWfs != null)
                {
                    WfsStatusId = inspireDataset.InspireDeliveryWfs.StatusId;
                    WfsStatus = inspireDataset.InspireDeliveryWfs.Status;
                    WfsNote = inspireDataset.InspireDeliveryWfs.Note;
                    WfsAutoUpdate = inspireDataset.InspireDeliveryWfs.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryAtomFeed != null)
                {
                    AtomFeedStatusId = inspireDataset.InspireDeliveryAtomFeed.StatusId;
                    AtomFeedStatus = inspireDataset.InspireDeliveryAtomFeed.Status;
                    AtomFeedNote = inspireDataset.InspireDeliveryAtomFeed.Note;
                    AtomFeedAutoUpdate = inspireDataset.InspireDeliveryAtomFeed.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryWfsOrAtom != null)
                {
                    WfsOrAtomStatusId = inspireDataset.InspireDeliveryWfsOrAtom.StatusId;
                    WfsOrAtomStatus = inspireDataset.InspireDeliveryWfsOrAtom.Status;
                    WfsOrAtomNote = inspireDataset.InspireDeliveryWfsOrAtom.Note;
                    WfsOrAtomAutoUpdate = inspireDataset.InspireDeliveryWfsOrAtom.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryHarmonizedData != null)
                {
                    HarmonizedDataStatusId = inspireDataset.InspireDeliveryHarmonizedData.StatusId;
                    HarmonizedDataStatus = inspireDataset.InspireDeliveryHarmonizedData.Status;
                    HarmonizedDataNote = inspireDataset.InspireDeliveryHarmonizedData.Note;
                    HarmonizedDataAutoUpdate = inspireDataset.InspireDeliveryHarmonizedData.AutoUpdate;
                }
                if (inspireDataset.InspireDeliverySpatialDataService != null)
                {
                    SpatialDataServiceStatusId = inspireDataset.InspireDeliverySpatialDataService.StatusId;
                    SpatialDataServiceStatus = inspireDataset.InspireDeliverySpatialDataService.Status;
                    SpatialDataServiceNote = inspireDataset.InspireDeliverySpatialDataService.Note;
                    SpatialDataServiceAutoUpdate = inspireDataset.InspireDeliverySpatialDataService.AutoUpdate;
                }
                UpdateDataset(inspireDataset);
            }
        }

        public string GetInspireDatasetEditUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/inspire/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
            }
            return "/inspire/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
        }

        public string GetInspireDatasetDeleteUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/inspire/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
            }
            return "/inspire/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
        }
    }
}