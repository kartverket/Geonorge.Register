using Resources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class MareanoDatasetViewModel : DatasetViewModel
    {
        public string FAIRStatusId { get; set; }
        public virtual FAIRDeliveryStatus FAIRStatus { get; set; }
        public double FAIRStatusPerCent { get; set; }

        [Display(Name = "Findable_Label", ResourceType = typeof(MareanoDataSet))]
        public string FindableStatusId { get; set; }
        public virtual FAIRDeliveryStatus FindableStatus { get; set; }
        public string FindableNote { get; set; }
        public bool FindableAutoUpdate { get; set; }
        public double FindableStatusPerCent { get; set; }

        [Display(Name = "Accesible_Label", ResourceType = typeof(MareanoDataSet))]
        public string AccesibleStatusId { get; set; }
        public virtual FAIRDeliveryStatus AccesibleStatus { get; set; }
        public string AccesibleNote { get; set; }
        public bool AccesibleAutoUpdate { get; set; }
        public double AccesibleStatusPerCent { get; set; }

        [Display(Name = "Interoperable_Label", ResourceType = typeof(MareanoDataSet))]
        public string InteroperableStatusId { get; set; }
        public virtual FAIRDeliveryStatus InteroperableStatus { get; set; }
        public string InteroperableNote { get; set; }
        public bool InteroperableAutoUpdate { get; set; }
        public double InteroperableStatusPerCent { get; set; }

        [Display(Name = "ReUseable_Label", ResourceType = typeof(MareanoDataSet))]
        public string ReUseableStatusId { get; set; }
        public virtual FAIRDeliveryStatus ReUseableStatus { get; set; }
        public string ReUseableNote { get; set; }
        public bool ReUseableAutoUpdate { get; set; }
        public double ReUseableStatusPerCent { get; set; }

        public bool F1_a_Criteria { get; set; }
        public bool F2_a_Criteria { get; set; }
        public bool F2_b_Criteria { get; set; }
        public bool F2_c_Criteria { get; set; }
        public bool F2_d_Criteria { get; set; }
        public bool F2_e_Criteria { get; set; }
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
        //public bool? I1_c_Criteria { get; set; } //Moved to I3_a_Criteria
        public bool I2_a_Criteria { get; set; }
        public bool I2_b_Criteria { get; set; }
        public bool? I3_a_Criteria { get; set; }
        public bool? I3_b_Criteria { get; set; }
        public bool? I3_c_Criteria { get; set; }
        public bool R1_a_Criteria { get; set; }
        public bool R1_b_Criteria { get; set; }
        public bool R2_a_Criteria { get; set; }
        public bool R2_b_Criteria { get; set; }
        public bool R2_c_Criteria { get; set; }
        public bool R2_d_Criteria { get; set; }
        public bool R2_e_Criteria { get; set; }
        public bool R2_f_Criteria { get; set; }
        public bool R2_g_Criteria { get; set; }
        public bool R2_h_Criteria { get; set; }
        public bool R2_i_Criteria { get; set; }
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

        [Display(Name = "Common", ResourceType = typeof(MareanoDataSet))]
        public string CommonStatusId { get; set; }
        public virtual DokDeliveryStatus CommonStatus { get; set; }
        public string CommonNote { get; set; }
        public bool CommonAutoUpdate { get; set; }
        public float? Grade { get; set; }

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
                F1_a_Criteria = mareanoDataset.F1_a_Criteria;
                F2_a_Criteria = mareanoDataset.F2_a_Criteria;
                F2_b_Criteria = mareanoDataset.F2_b_Criteria;
                F2_c_Criteria = mareanoDataset.F2_c_Criteria;
                F2_d_Criteria = mareanoDataset.F2_d_Criteria;
                F2_e_Criteria = mareanoDataset.F2_e_Criteria;
                F3_a_Criteria = mareanoDataset.F3_a_Criteria;
                F4_a_Criteria = mareanoDataset.F4_a_Criteria;
                A1_a_Criteria = mareanoDataset.A1_a_Criteria;
                A1_b_Criteria = mareanoDataset.A1_b_Criteria;
                A1_c_Criteria = mareanoDataset.A1_c_Criteria;
                A1_d_Criteria = mareanoDataset.A1_d_Criteria;
                A1_e_Criteria = mareanoDataset.A1_e_Criteria;
                A1_f_Criteria = mareanoDataset.A1_f_Criteria;
                A2_a_Criteria = mareanoDataset.A2_a_Criteria;
                I1_a_Criteria = mareanoDataset.I1_a_Criteria;
                I1_b_Criteria = mareanoDataset.I1_b_Criteria;
                //if(mareanoDataset.I1_c_Criteria.HasValue)
                //    I1_c_Criteria = mareanoDataset.I1_c_Criteria.Value; //Moved to I3_a_Criteria
                I2_a_Criteria = mareanoDataset.I2_a_Criteria;
                I2_b_Criteria = mareanoDataset.I2_b_Criteria;
                if(mareanoDataset.I3_a_Criteria.HasValue)
                    I3_a_Criteria = mareanoDataset.I3_a_Criteria.Value;
                if(mareanoDataset.I3_b_Criteria.HasValue)
                    I3_b_Criteria = mareanoDataset.I3_b_Criteria.Value;
                if (mareanoDataset.I3_c_Criteria.HasValue)
                    I3_c_Criteria = mareanoDataset.I3_c_Criteria.Value;
                R1_a_Criteria = mareanoDataset.R1_a_Criteria;
                R1_b_Criteria = mareanoDataset.R1_b_Criteria;
                R2_a_Criteria = mareanoDataset.R2_a_Criteria;
                R2_b_Criteria = mareanoDataset.R2_b_Criteria;
                R2_c_Criteria = mareanoDataset.R2_c_Criteria;
                R2_d_Criteria = mareanoDataset.R2_d_Criteria;
                R2_e_Criteria = mareanoDataset.R2_e_Criteria;
                R2_f_Criteria = mareanoDataset.R2_f_Criteria;
                R2_g_Criteria = mareanoDataset.R2_g_Criteria;
                R2_h_Criteria = mareanoDataset.R2_h_Criteria;
                R2_i_Criteria = mareanoDataset.R2_i_Criteria;
                R3_a_Criteria = mareanoDataset.R3_a_Criteria;
                R3_b_Criteria = mareanoDataset.R3_b_Criteria;


                if (mareanoDataset.FAIRStatus != null)
                {
                    FAIRStatusId = mareanoDataset.FAIRStatus.StatusId;
                    FAIRStatus = mareanoDataset.FAIRStatus.Status;
                    FAIRStatusPerCent = mareanoDataset.FAIRStatusPerCent;
                }

                if (mareanoDataset.FindableStatus != null)
                {
                    FindableStatusId = mareanoDataset.FindableStatus.StatusId;
                    FindableStatus = mareanoDataset.FindableStatus.Status;
                    FindableNote = mareanoDataset.FindableStatus.Note;
                    FindableAutoUpdate = mareanoDataset.FindableStatus.AutoUpdate;
                    FindableStatusPerCent = mareanoDataset.FindableStatusPerCent;
                }
                if (mareanoDataset.AccesibleStatus != null)
                {
                    AccesibleStatusId = mareanoDataset.AccesibleStatus.StatusId;
                    AccesibleStatus = mareanoDataset.AccesibleStatus.Status;
                    AccesibleNote = mareanoDataset.AccesibleStatus.Note;
                    AccesibleAutoUpdate = mareanoDataset.AccesibleStatus.AutoUpdate;
                    AccesibleStatusPerCent = mareanoDataset.AccesibleStatusPerCent;
                }
                if (mareanoDataset.InteroperableStatus != null)
                {
                    InteroperableStatusId = mareanoDataset.InteroperableStatus.StatusId;
                    InteroperableStatus = mareanoDataset.InteroperableStatus.Status;
                    InteroperableNote = mareanoDataset.InteroperableStatus.Note;
                    InteroperableAutoUpdate = mareanoDataset.InteroperableStatus.AutoUpdate;
                    InteroperableStatusPerCent = mareanoDataset.InteroperableStatusPerCent;
                }
                if (mareanoDataset.ReUseableStatus != null)
                {
                    ReUseableStatusId = mareanoDataset.ReUseableStatus.StatusId;
                    ReUseableStatus = mareanoDataset.ReUseableStatus.Status;
                    ReUseableNote = mareanoDataset.ReUseableStatus.Note;
                    ReUseableAutoUpdate = mareanoDataset.ReUseableStatus.AutoUpdate;
                    ReUseableStatusPerCent = mareanoDataset.ReUseableStatusPerCent;
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
                if (mareanoDataset.ProductSheetStatus != null)
                {
                    ProductSheetStatusId = mareanoDataset.ProductSheetStatus.StatusId;
                    ProductSheetStatus = mareanoDataset.ProductSheetStatus.Status;
                    ProductSheetNote = mareanoDataset.ProductSheetStatus.Note;
                    ProductSheetAutoUpdate = mareanoDataset.ProductSheetStatus.AutoUpdate;
                }
                if (mareanoDataset.PresentationRulesStatus != null)
                {
                    PresentationRulesStatusId = mareanoDataset.PresentationRulesStatus.StatusId;
                    PresentationRulesStatus = mareanoDataset.PresentationRulesStatus.Status;
                    PresentationRulesNote = mareanoDataset.PresentationRulesStatus.Note;
                    PresentationRulesAutoUpdate = mareanoDataset.PresentationRulesStatus.AutoUpdate;
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

                if (mareanoDataset.Grade.HasValue)
                    Grade = mareanoDataset.Grade.Value;

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