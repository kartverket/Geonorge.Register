using Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class FairDatasetViewModel : DatasetViewModel
    {
        public string FAIRStatusId { get; set; }
        public virtual FAIRDeliveryStatus FAIRStatus { get; set; }
        public double FAIRStatusPerCent { get; set; }

        [Display(Name = "Findable_Label", ResourceType = typeof(FairDataSet))]
        public string FindableStatusId { get; set; }
        public virtual FAIRDeliveryStatus FindableStatus { get; set; }
        public string FindableNote { get; set; }
        public bool FindableAutoUpdate { get; set; }
        public double FindableStatusPerCent { get; set; }

        [Display(Name = "Accesible_Label", ResourceType = typeof(FairDataSet))]
        public string AccesibleStatusId { get; set; }
        public virtual FAIRDeliveryStatus AccesibleStatus { get; set; }
        public string AccesibleNote { get; set; }
        public bool AccesibleAutoUpdate { get; set; }
        public double AccesibleStatusPerCent { get; set; }

        [Display(Name = "Interoperable_Label", ResourceType = typeof(FairDataSet))]
        public string InteroperableStatusId { get; set; }
        public virtual FAIRDeliveryStatus InteroperableStatus { get; set; }
        public string InteroperableNote { get; set; }
        public bool InteroperableAutoUpdate { get; set; }
        public double InteroperableStatusPerCent { get; set; }

        [Display(Name = "ReUseable_Label", ResourceType = typeof(FairDataSet))]
        public string ReUseableStatusId { get; set; }
        public virtual FAIRDeliveryStatus ReUseableStatus { get; set; }
        public string ReUseableNote { get; set; }
        public bool ReUseableAutoUpdate { get; set; }
        public double ReUseableStatusPerCent { get; set; }

        public bool F1_a_Criteria { get; set; }
        public bool F2_a_Criteria { get; set; }
        public bool F2_b_Criteria { get; set; }
        public bool F2_c_Criteria { get; set; }
        public bool F3_a_Criteria { get; set; }
        public bool F4_a_Criteria { get; set; }
        public bool A1_a_Criteria { get; set; }
        public bool A1_b_Criteria { get; set; }
        public bool A1_c_Criteria { get; set; }
        public bool A1_d_Criteria { get; set; }
        public bool A1_e_Criteria { get; set; }
        public bool A1_f_Criteria { get; set; }
        public bool A2_a_Criteria { get; set; }
        public bool I1_a_Criteria { get; set; }
        public bool I1_b_Criteria { get; set; }
        public bool? I1_c_Criteria { get; set; }
        public bool I2_a_Criteria { get; set; }
        public bool I2_b_Criteria { get; set; }
        public bool? I3_a_Criteria { get; set; }
        public bool? I3_b_Criteria { get; set; }
        public bool R1_a_Criteria { get; set; }
        public bool R2_a_Criteria { get; set; }
        public bool R2_b_Criteria { get; set; }
        public bool R2_c_Criteria { get; set; }
        public bool R2_d_Criteria { get; set; }
        public bool R2_e_Criteria { get; set; }
        public bool R2_f_Criteria { get; set; }
        public bool R3_a_Criteria { get; set; }
        public bool R3_b_Criteria { get; set; }


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

        [Display(Name = "DOK_ProductSheetStatus", ResourceType = typeof(DataSet))]
        public string ProductSheetStatusId { get; set; }
        public virtual DokDeliveryStatus ProductSheetStatus { get; set; }
        public string ProductSheetNote { get; set; }
        public bool ProductSheetAutoUpdate { get; set; }

        [Display(Name = "DOK_PresentationRulesStatus", ResourceType = typeof(DataSet))]
        public string PresentationRulesStatusId { get; set; }
        public virtual DokDeliveryStatus PresentationRulesStatus { get; set; }
        public string PresentationRulesNote { get; set; }
        public bool PresentationRulesAutoUpdate { get; set; }


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

        [Display(Name = "Common", ResourceType = typeof(FairDataSet))]
        public string CommonStatusId { get; set; }
        public virtual DokDeliveryStatus CommonStatus { get; set; }
        public string CommonNote { get; set; }
        public bool CommonAutoUpdate { get; set; }
        public float? Grade { get; set; }

        public FairDatasetViewModel(FairDataset item)
        {
            Update(item);
        }

        public FairDatasetViewModel()
        {
        }

        public void Update(FairDataset FairDataset)
        {
            if (FairDataset != null)
            {
                F1_a_Criteria = FairDataset.F1_a_Criteria;
                F2_a_Criteria = FairDataset.F2_a_Criteria;
                F2_b_Criteria = FairDataset.F2_b_Criteria;
                F2_c_Criteria = FairDataset.F2_c_Criteria;
                F3_a_Criteria = FairDataset.F3_a_Criteria;
                F4_a_Criteria = FairDataset.F4_a_Criteria;
                A1_a_Criteria = FairDataset.A1_a_Criteria;
                A1_b_Criteria = FairDataset.A1_b_Criteria;
                A1_c_Criteria = FairDataset.A1_c_Criteria;
                A1_d_Criteria = FairDataset.A1_d_Criteria;
                A1_e_Criteria = FairDataset.A1_e_Criteria;
                A1_f_Criteria = FairDataset.A1_f_Criteria;
                A2_a_Criteria = FairDataset.A2_a_Criteria;
                I1_a_Criteria = FairDataset.I1_a_Criteria;
                I1_b_Criteria = FairDataset.I1_b_Criteria;
                if (FairDataset.I1_c_Criteria.HasValue)
                    I1_c_Criteria = FairDataset.I1_c_Criteria.Value;
                I2_a_Criteria = FairDataset.I2_a_Criteria;
                I2_b_Criteria = FairDataset.I2_b_Criteria;
                if (FairDataset.I3_a_Criteria.HasValue)
                    I3_a_Criteria = FairDataset.I3_a_Criteria.Value;
                if (FairDataset.I3_b_Criteria.HasValue)
                    I3_b_Criteria = FairDataset.I3_b_Criteria.Value;
                R1_a_Criteria = FairDataset.R1_a_Criteria;
                R2_a_Criteria = FairDataset.R2_a_Criteria;
                R2_b_Criteria = FairDataset.R2_b_Criteria;
                R2_c_Criteria = FairDataset.R2_c_Criteria;
                R2_d_Criteria = FairDataset.R2_d_Criteria;
                R2_e_Criteria = FairDataset.R2_e_Criteria;
                R2_f_Criteria = FairDataset.R2_f_Criteria;
                R3_a_Criteria = FairDataset.R3_a_Criteria;
                R3_b_Criteria = FairDataset.R3_b_Criteria;


                if (FairDataset.FAIRStatus != null)
                {
                    FAIRStatusId = FairDataset.FAIRStatus.StatusId;
                    FAIRStatus = FairDataset.FAIRStatus.Status;
                    FAIRStatusPerCent = FairDataset.FAIRStatusPerCent;
                }

                if (FairDataset.FindableStatus != null)
                {
                    FindableStatusId = FairDataset.FindableStatus.StatusId;
                    FindableStatus = FairDataset.FindableStatus.Status;
                    FindableNote = FairDataset.FindableStatus.Note;
                    FindableAutoUpdate = FairDataset.FindableStatus.AutoUpdate;
                    FindableStatusPerCent = FairDataset.FindableStatusPerCent;
                }
                if (FairDataset.AccesibleStatus != null)
                {
                    AccesibleStatusId = FairDataset.AccesibleStatus.StatusId;
                    AccesibleStatus = FairDataset.AccesibleStatus.Status;
                    AccesibleNote = FairDataset.AccesibleStatus.Note;
                    AccesibleAutoUpdate = FairDataset.AccesibleStatus.AutoUpdate;
                    AccesibleStatusPerCent = FairDataset.AccesibleStatusPerCent;
                }
                if (FairDataset.InteroperableStatus != null)
                {
                    InteroperableStatusId = FairDataset.InteroperableStatus.StatusId;
                    InteroperableStatus = FairDataset.InteroperableStatus.Status;
                    InteroperableNote = FairDataset.InteroperableStatus.Note;
                    InteroperableAutoUpdate = FairDataset.InteroperableStatus.AutoUpdate;
                    InteroperableStatusPerCent = FairDataset.InteroperableStatusPerCent;
                }
                if (FairDataset.ReUseableStatus != null)
                {
                    ReUseableStatusId = FairDataset.ReUseableStatus.StatusId;
                    ReUseableStatus = FairDataset.ReUseableStatus.Status;
                    ReUseableNote = FairDataset.ReUseableStatus.Note;
                    ReUseableAutoUpdate = FairDataset.ReUseableStatus.AutoUpdate;
                    ReUseableStatusPerCent = FairDataset.ReUseableStatusPerCent;
                }
                if (FairDataset.MetadataStatus != null)
                {
                    MetadataStatusId = FairDataset.MetadataStatus.StatusId;
                    MetadataStatus = FairDataset.MetadataStatus.Status;
                    MetadataNote = FairDataset.MetadataStatus.Note;
                    MetadataAutoUpdate = FairDataset.MetadataStatus.AutoUpdate;
                }
                if (FairDataset.ProductSpesificationStatus != null)
                {
                    ProductSpesificationStatusId = FairDataset.ProductSpesificationStatus.StatusId;
                    ProductSpesificationStatus = FairDataset.ProductSpesificationStatus.Status;
                    ProduktspesifikasjonNote = FairDataset.ProductSpesificationStatus.Note;
                    ProduktspesifikasjonAutoUpdate = FairDataset.ProductSpesificationStatus.AutoUpdate;
                }
                if (FairDataset.ProductSheetStatus != null)
                {
                    ProductSheetStatusId = FairDataset.ProductSheetStatus.StatusId;
                    ProductSheetStatus = FairDataset.ProductSheetStatus.Status;
                    ProductSheetNote = FairDataset.ProductSheetStatus.Note;
                    ProductSheetAutoUpdate = FairDataset.ProductSheetStatus.AutoUpdate;
                }
                if (FairDataset.PresentationRulesStatus != null)
                {
                    PresentationRulesStatusId = FairDataset.PresentationRulesStatus.StatusId;
                    PresentationRulesStatus = FairDataset.PresentationRulesStatus.Status;
                    PresentationRulesNote = FairDataset.PresentationRulesStatus.Note;
                    PresentationRulesAutoUpdate = FairDataset.PresentationRulesStatus.AutoUpdate;
                }
                if (FairDataset.SosiDataStatus != null)
                {
                    SosiDataStatusId = FairDataset.SosiDataStatus.StatusId;
                    SosiDataStatus = FairDataset.SosiDataStatus.Status;
                    SosiDataNote = FairDataset.SosiDataStatus.Note;
                    SosiDataAutoUpdate = FairDataset.SosiDataStatus.AutoUpdate;
                }
                if (FairDataset.GmlDataStatus != null)
                {
                    GmlDataStatusId = FairDataset.GmlDataStatus.StatusId;
                    GmlDataStatus = FairDataset.GmlDataStatus.Status;
                    GmlDataNote = FairDataset.GmlDataStatus.Note;
                    GmlDataAutoUpdate = FairDataset.GmlDataStatus.AutoUpdate;
                }
                if (FairDataset.WmsStatus != null)
                {
                    WmsStatusId = FairDataset.WmsStatus.StatusId;
                    WmsStatus = FairDataset.WmsStatus.Status;
                    WmsNote = FairDataset.WmsStatus.Note;
                    WmsAutoUpdate = FairDataset.WmsStatus.AutoUpdate;
                }
                if (FairDataset.WfsStatus != null)
                {
                    WfsStatusId = FairDataset.WfsStatus.StatusId;
                    WfsStatus = FairDataset.WfsStatus.Status;
                    WfsNote = FairDataset.WfsStatus.Note;
                    WfsAutoUpdate = FairDataset.WfsStatus.AutoUpdate;
                }
                if (FairDataset.AtomFeedStatus != null)
                {
                    AtomFeedStatusId = FairDataset.AtomFeedStatus.StatusId;
                    AtomFeedStatus = FairDataset.AtomFeedStatus.Status;
                    AtomFeedNote = FairDataset.AtomFeedStatus.Note;
                    AtomFeedAutoUpdate = FairDataset.AtomFeedStatus.AutoUpdate;
                }
                if (FairDataset.CommonStatus != null)
                {
                    CommonStatusId = FairDataset.CommonStatus.StatusId;
                    CommonStatus = FairDataset.CommonStatus.Status;
                    CommonNote = FairDataset.CommonStatus.Note;
                    CommonAutoUpdate = FairDataset.CommonStatus.AutoUpdate;
                }

                if (FairDataset.Grade.HasValue)
                    Grade = FairDataset.Grade.Value;

                UpdateDataset(FairDataset);
            }
        }

        public string GetFairDatasetEditUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/fair/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
            }
            return "/fair/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
        }

        public string GetFairDatasetDeleteUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/fair/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
            }
            return "/fareano/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
        }

        public object FilterByOrganization()
        {
            return FilterOrganizationUrl() + Owner.seoname;
        }
    }
}