using Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class GeodatalovDatasetViewModel : DatasetViewModel
    {
        [Display(Name = "InspireTheme", ResourceType = typeof(GeodatalovDataSet))]
        public bool InspireTheme { get; set; }
        [Display(Name = "Dok", ResourceType = typeof(GeodatalovDataSet))]
        public bool Dok { get; set; }
        [Display(Name = "NationalDataset", ResourceType = typeof(GeodatalovDataSet))]
        public bool NationalDataset { get; set; }
        [Display(Name = "Plan", ResourceType = typeof(GeodatalovDataSet))]
        public bool Plan { get; set; }
        [Display(Name = "Geodatalov", ResourceType = typeof(GeodatalovDataSet))]
        public bool Geodatalov { get; set; }

        [Display(Name = "Metadata", ResourceType = typeof(InspireDataSet))]
        public string MetadataStatusId { get; set; }
        public virtual DokDeliveryStatus MetadataStatus { get; set; }
        public string MetadataNote { get; set; }
        public bool MetadataAutoUpdate { get; set; }

        [Display(Name = "DOK_ProductSpecificationStatus", ResourceType = typeof(DataSet))]
        public string ProductSpesificationStatusId { get; set; }
        public virtual DokDeliveryStatus ProductSpesificationStatus { get; set; }
        public string ProduktspesifikasjonNote { get; set; }
        public bool ProduktspesifikasjonAutoUpdate { get; set; }

        [Display(Name = "DOK_Delivery_SosiRequirements", ResourceType = typeof(DataSet))]
        public string SosiDataStatusId { get; set; }
        public virtual DokDeliveryStatus SosiDataStatus { get; set; }
        public string SosiDataNote { get; set; }
        public bool SosiDataAutoUpdate { get; set; }

        [Display(Name = "DOK_Delivery_GmlRequirements", ResourceType = typeof(DataSet))]
        public string GmlDataStatusId { get; set; }
        public virtual DokDeliveryStatus GmlDataStatus { get; set; }
        public string GmlDataNote { get; set; }
        public bool GmlDataAutoUpdate { get; set; }

        [Display(Name = "DOK_Delivery_Wms", ResourceType = typeof(DataSet))]
        public string WmsStatusId { get; set; }
        public virtual DokDeliveryStatus WmsStatus { get; set; }
        public string WmsNote { get; set; }
        public bool WmsAutoUpdate { get; set; }

        [Display(Name = "DOK_Delivery_Wfs", ResourceType = typeof(DataSet))]
        public string WfsStatusId { get; set; }
        public virtual DokDeliveryStatus WfsStatus { get; set; }
        public string WfsNote { get; set; }
        public bool WfsAutoUpdate { get; set; }

        [Display(Name = "DOK_Delivery_AtomFeed", ResourceType = typeof(DataSet))]
        public string AtomFeedStatusId { get; set; }
        public virtual DokDeliveryStatus AtomFeedStatus { get; set; }
        public string AtomFeedNote { get; set; }
        public bool AtomFeedAutoUpdate { get; set; }

        [Display(Name = "Common", ResourceType = typeof(GeodatalovDataSet))]
        public string CommonStatusId { get; set; }
        public virtual DokDeliveryStatus CommonStatus { get; set; }
        public string CommonNote { get; set; }
        public bool CommonAutoUpdate { get; set; }

        public GeodatalovDatasetViewModel(GeodatalovDataset item)
        {
            Update(item);
        }

        public GeodatalovDatasetViewModel()
        {
        }

        public void Update(GeodatalovDataset geodatalovDataset)
        {
            if (geodatalovDataset != null)
            {
                InspireTheme = geodatalovDataset.InspireTheme;
                Dok = geodatalovDataset.Dok;
                NationalDataset = geodatalovDataset.NationalDataset;
                Plan = geodatalovDataset.Plan;
                Geodatalov = geodatalovDataset.Geodatalov;

                if (geodatalovDataset.MetadataStatus != null)
                {
                    MetadataStatusId = geodatalovDataset.MetadataStatus.StatusId;
                    MetadataStatus = geodatalovDataset.MetadataStatus.Status;
                    MetadataNote = geodatalovDataset.MetadataStatus.Note;
                    MetadataAutoUpdate = geodatalovDataset.MetadataStatus.AutoUpdate;
                }
                if (geodatalovDataset.ProductSpesificationStatus != null)
                {
                    ProductSpesificationStatusId = geodatalovDataset.ProductSpesificationStatus.StatusId;
                    ProductSpesificationStatus = geodatalovDataset.ProductSpesificationStatus.Status;
                    ProduktspesifikasjonNote = geodatalovDataset.ProductSpesificationStatus.Note;
                    ProduktspesifikasjonAutoUpdate = geodatalovDataset.ProductSpesificationStatus.AutoUpdate;
                }
                if (geodatalovDataset.SosiDataStatus != null)
                {
                    SosiDataStatusId = geodatalovDataset.SosiDataStatus.StatusId;
                    SosiDataStatus = geodatalovDataset.SosiDataStatus.Status;
                    SosiDataNote = geodatalovDataset.SosiDataStatus.Note;
                    SosiDataAutoUpdate = geodatalovDataset.SosiDataStatus.AutoUpdate;
                }
                if (geodatalovDataset.GmlDataStatus != null)
                {
                    GmlDataStatusId = geodatalovDataset.GmlDataStatus.StatusId;
                    GmlDataStatus = geodatalovDataset.GmlDataStatus.Status;
                    GmlDataNote = geodatalovDataset.GmlDataStatus.Note;
                    GmlDataAutoUpdate = geodatalovDataset.GmlDataStatus.AutoUpdate;
                }
                if (geodatalovDataset.WmsStatus != null)
                {
                    WmsStatusId = geodatalovDataset.WmsStatus.StatusId;
                    WmsStatus = geodatalovDataset.WmsStatus.Status;
                    WmsNote = geodatalovDataset.WmsStatus.Note;
                    WmsAutoUpdate = geodatalovDataset.WmsStatus.AutoUpdate;
                }
                if (geodatalovDataset.WfsStatus != null)
                {
                    WfsStatusId = geodatalovDataset.WfsStatus.StatusId;
                    WfsStatus = geodatalovDataset.WfsStatus.Status;
                    WfsNote = geodatalovDataset.WfsStatus.Note;
                    WfsAutoUpdate = geodatalovDataset.WfsStatus.AutoUpdate;
                }
                if (geodatalovDataset.AtomFeedStatus != null)
                {
                    AtomFeedStatusId = geodatalovDataset.AtomFeedStatus.StatusId;
                    AtomFeedStatus = geodatalovDataset.AtomFeedStatus.Status;
                    AtomFeedNote = geodatalovDataset.AtomFeedStatus.Note;
                    AtomFeedAutoUpdate = geodatalovDataset.AtomFeedStatus.AutoUpdate;
                }
                if (geodatalovDataset.CommonStatus != null)
                {
                    CommonStatusId = geodatalovDataset.CommonStatus.StatusId;
                    CommonStatus = geodatalovDataset.CommonStatus.Status;
                    CommonNote = geodatalovDataset.CommonStatus.Note;
                    CommonAutoUpdate = geodatalovDataset.CommonStatus.AutoUpdate;
                }

                UpdateDataset(geodatalovDataset);
            }
        }

        public string GetGeodatalovDatasetEditUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/geodatalov/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
            }
            return "/geodatalov/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
        }

        public string GetGeodatalovDatasetDeleteUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/geodatalov/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
            }
            return "/geodatalov/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
        }
    }
}