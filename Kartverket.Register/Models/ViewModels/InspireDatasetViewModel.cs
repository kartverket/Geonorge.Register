using Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireDatasetViewModel : DatasetViewModel
    {
        [Display(Name = "Metadata:")]
        public string MetadataStatus { get; set; }
        public string MetadataNote { get; set; }
        public bool MetadataAutoUpdate { get; set; }

        [Display(Name = "MetadataServiceStatus", ResourceType = typeof(InspireDataSet))]
        public string MetadataServiceStatus { get; set; }
        public string MetadataServiceNote { get; set; }
        public bool MetadataServiceAutoUpdate { get; set; }

        [Display(Name = "DistributionStatus", ResourceType = typeof(InspireDataSet))]
        public string DistributionStatus { get; set; }
        public string DistributionNote { get; set; }
        public bool DistributionAutoUpdate { get; set; }

        [Display(Name = "WMS:")]
        public string WmsStatus { get; set; }
        public string WmsNote { get; set; }
        public bool WmsAutoUpdate { get; set; }

        [Display(Name = "WfsStatus", ResourceType = typeof(InspireDataSet))]
        public string WfsStatus { get; set; }
        public string WfsNote { get; set; }
        public bool WfsAutoUpdate { get; set; }

        [Display(Name = "AtomFeedStatus", ResourceType = typeof(InspireDataSet))]
        public string AtomFeedStatus { get; set; }
        public string AtomFeedNote { get; set; }
        public bool AtomFeedAutoUpdate { get; set; }

        [Display(Name = "WfsOrAtomStatus", ResourceType = typeof(InspireDataSet))]
        public string WfsOrAtomStatus { get; set; }
        public string WfsOrAtomNote { get; set; }
        public bool WfsOrAtomAutoUpdate { get; set; }

        [Display(Name = "HarmonizedDataStatus", ResourceType = typeof(InspireDataSet))]
        public string HarmonizedDataStatus { get; set; }
        public string HarmonizedDataNote { get; set; }
        public bool HarmonizedDataAutoUpdate { get; set; }

        [Display(Name = "SpatialDataServiceStatus", ResourceType = typeof(InspireDataSet))]
        public string SpatialDataServiceStatus { get; set; }
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
                if (inspireDataset.InspireDeliveryMetadata != null)
                {
                    MetadataStatus = inspireDataset.InspireDeliveryMetadata.StatusId;
                    MetadataNote = inspireDataset.InspireDeliveryMetadata.Note;
                    MetadataAutoUpdate = inspireDataset.InspireDeliveryMetadata.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryMetadataService != null)
                {
                    MetadataServiceStatus = inspireDataset.InspireDeliveryMetadataService.StatusId;
                    MetadataServiceNote = inspireDataset.InspireDeliveryMetadataService.Note;
                    MetadataServiceAutoUpdate = inspireDataset.InspireDeliveryMetadataService.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryDistribution != null)
                {
                    DistributionStatus = inspireDataset.InspireDeliveryDistribution.StatusId;
                    DistributionNote = inspireDataset.InspireDeliveryDistribution.Note;
                    DistributionAutoUpdate = inspireDataset.InspireDeliveryDistribution.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryWms != null)
                {
                    WmsStatus = inspireDataset.InspireDeliveryWms.StatusId;
                    WmsNote = inspireDataset.InspireDeliveryWms.Note;
                    WmsAutoUpdate = inspireDataset.InspireDeliveryWms.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryWfs != null)
                {
                    WfsStatus = inspireDataset.InspireDeliveryWfs.StatusId;
                    WfsNote = inspireDataset.InspireDeliveryWfs.Note;
                    WfsAutoUpdate = inspireDataset.InspireDeliveryWfs.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryAtomFeed != null)
                {
                    AtomFeedStatus = inspireDataset.InspireDeliveryAtomFeed.StatusId;
                    AtomFeedNote = inspireDataset.InspireDeliveryAtomFeed.Note;
                    AtomFeedAutoUpdate = inspireDataset.InspireDeliveryAtomFeed.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryWfsOrAtom != null)
                {
                    WfsOrAtomStatus = inspireDataset.InspireDeliveryWfsOrAtom.StatusId;
                    WfsOrAtomNote = inspireDataset.InspireDeliveryWfsOrAtom.Note;
                    WfsOrAtomAutoUpdate = inspireDataset.InspireDeliveryWfsOrAtom.AutoUpdate;
                }
                if (inspireDataset.InspireDeliveryHarmonizedData != null)
                {
                    HarmonizedDataStatus = inspireDataset.InspireDeliveryHarmonizedData.StatusId;
                    HarmonizedDataNote = inspireDataset.InspireDeliveryHarmonizedData.Note;
                    HarmonizedDataAutoUpdate = inspireDataset.InspireDeliveryHarmonizedData.AutoUpdate;
                }
                if (inspireDataset.InspireDeliverySpatialDataService != null)
                {
                    SpatialDataServiceStatus = inspireDataset.InspireDeliverySpatialDataService.StatusId;
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
            else
            {
                return "/inspire/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
            }
        }
    }
}