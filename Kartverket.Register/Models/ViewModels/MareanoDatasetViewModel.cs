using Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class MareanoDatasetViewModel : DatasetViewModel
    {
        [Display(Name = "Findable_Label", ResourceType = typeof(MareanoDataSet))]
        public string FindableStatusId { get; set; }
        public virtual FAIRDeliveryStatus FindableStatus { get; set; }
        public string FindableNote { get; set; }
        public bool FindableAutoUpdate { get; set; }

        [Display(Name = "Accesible_Label", ResourceType = typeof(MareanoDataSet))]
        public string AccesibleStatusId { get; set; }
        public virtual FAIRDeliveryStatus AccesibleStatus { get; set; }
        public string AccesibleNote { get; set; }
        public bool AccesibleAutoUpdate { get; set; }

        [Display(Name = "Interoperable_Label", ResourceType = typeof(MareanoDataSet))]
        public string InteroperableStatusId { get; set; }
        public virtual FAIRDeliveryStatus InteroperableStatus { get; set; }
        public string InteroperableNote { get; set; }
        public bool InteroperableAutoUpdate { get; set; }

        [Display(Name = "ReUseable_Label", ResourceType = typeof(MareanoDataSet))]
        public string ReUseableStatusId { get; set; }
        public virtual FAIRDeliveryStatus ReUseableStatus { get; set; }
        public string ReUseableNote { get; set; }
        public bool ReUseableAutoUpdate { get; set; }


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

        [Display(Name = "Common", ResourceType = typeof(MareanoDataSet))]
        public string CommonStatusId { get; set; }
        public virtual DokDeliveryStatus CommonStatus { get; set; }
        public string CommonNote { get; set; }
        public bool CommonAutoUpdate { get; set; }

        public MareanoDatasetViewModel(MareanoDataset item)
        {
            Update(item);
        }

        public MareanoDatasetViewModel()
        {
        }

        public void Update(MareanoDataset mareanoDataset)
        {
            if (mareanoDataset != null)
            {
                if (mareanoDataset.FindableStatus != null)
                {
                    FindableStatusId = mareanoDataset.FindableStatus.StatusId;
                    FindableStatus = mareanoDataset.FindableStatus.Status;
                    FindableNote = mareanoDataset.FindableStatus.Note;
                    FindableAutoUpdate = mareanoDataset.FindableStatus.AutoUpdate;
                }
                if (mareanoDataset.AccesibleStatus != null)
                {
                    AccesibleStatusId = mareanoDataset.AccesibleStatus.StatusId;
                    AccesibleStatus = mareanoDataset.AccesibleStatus.Status;
                    AccesibleNote = mareanoDataset.AccesibleStatus.Note;
                    AccesibleAutoUpdate = mareanoDataset.AccesibleStatus.AutoUpdate;
                }
                if (mareanoDataset.InteroperableStatus != null)
                {
                    InteroperableStatusId = mareanoDataset.InteroperableStatus.StatusId;
                    InteroperableStatus = mareanoDataset.InteroperableStatus.Status;
                    InteroperableNote = mareanoDataset.InteroperableStatus.Note;
                    InteroperableAutoUpdate = mareanoDataset.InteroperableStatus.AutoUpdate;
                }
                if (mareanoDataset.ReUseableStatus != null)
                {
                    ReUseableStatusId = mareanoDataset.ReUseableStatus.StatusId;
                    ReUseableStatus = mareanoDataset.ReUseableStatus.Status;
                    ReUseableNote = mareanoDataset.ReUseableStatus.Note;
                    ReUseableAutoUpdate = mareanoDataset.ReUseableStatus.AutoUpdate;
                }
                if (mareanoDataset.MetadataStatus != null)
                {
                    MetadataStatusId = mareanoDataset.MetadataStatus.StatusId;
                    MetadataStatus = mareanoDataset.MetadataStatus.Status;
                    MetadataNote = mareanoDataset.MetadataStatus.Note;
                    MetadataAutoUpdate = mareanoDataset.MetadataStatus.AutoUpdate;
                }
                if (mareanoDataset.ProductSpesificationStatus != null)
                {
                    ProductSpesificationStatusId = mareanoDataset.ProductSpesificationStatus.StatusId;
                    ProductSpesificationStatus = mareanoDataset.ProductSpesificationStatus.Status;
                    ProduktspesifikasjonNote = mareanoDataset.ProductSpesificationStatus.Note;
                    ProduktspesifikasjonAutoUpdate = mareanoDataset.ProductSpesificationStatus.AutoUpdate;
                }
                if (mareanoDataset.SosiDataStatus != null)
                {
                    SosiDataStatusId = mareanoDataset.SosiDataStatus.StatusId;
                    SosiDataStatus = mareanoDataset.SosiDataStatus.Status;
                    SosiDataNote = mareanoDataset.SosiDataStatus.Note;
                    SosiDataAutoUpdate = mareanoDataset.SosiDataStatus.AutoUpdate;
                }
                if (mareanoDataset.GmlDataStatus != null)
                {
                    GmlDataStatusId = mareanoDataset.GmlDataStatus.StatusId;
                    GmlDataStatus = mareanoDataset.GmlDataStatus.Status;
                    GmlDataNote = mareanoDataset.GmlDataStatus.Note;
                    GmlDataAutoUpdate = mareanoDataset.GmlDataStatus.AutoUpdate;
                }
                if (mareanoDataset.WmsStatus != null)
                {
                    WmsStatusId = mareanoDataset.WmsStatus.StatusId;
                    WmsStatus = mareanoDataset.WmsStatus.Status;
                    WmsNote = mareanoDataset.WmsStatus.Note;
                    WmsAutoUpdate = mareanoDataset.WmsStatus.AutoUpdate;
                }
                if (mareanoDataset.WfsStatus != null)
                {
                    WfsStatusId = mareanoDataset.WfsStatus.StatusId;
                    WfsStatus = mareanoDataset.WfsStatus.Status;
                    WfsNote = mareanoDataset.WfsStatus.Note;
                    WfsAutoUpdate = mareanoDataset.WfsStatus.AutoUpdate;
                }
                if (mareanoDataset.AtomFeedStatus != null)
                {
                    AtomFeedStatusId = mareanoDataset.AtomFeedStatus.StatusId;
                    AtomFeedStatus = mareanoDataset.AtomFeedStatus.Status;
                    AtomFeedNote = mareanoDataset.AtomFeedStatus.Note;
                    AtomFeedAutoUpdate = mareanoDataset.AtomFeedStatus.AutoUpdate;
                }
                if (mareanoDataset.CommonStatus != null)
                {
                    CommonStatusId = mareanoDataset.CommonStatus.StatusId;
                    CommonStatus = mareanoDataset.CommonStatus.Status;
                    CommonNote = mareanoDataset.CommonStatus.Note;
                    CommonAutoUpdate = mareanoDataset.CommonStatus.AutoUpdate;
                }

                UpdateDataset(mareanoDataset);
            }
        }

        public string GetMareanoDatasetEditUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/mareano/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
            }
            return "/mareano/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
        }

        public string GetMareanoDatasetDeleteUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/mareano/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
            }
            return "/mareano/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
        }

        public object FilterByOrganization()
        {
            return FilterOrganizationUrl() + Owner.seoname;
        }
    }
}