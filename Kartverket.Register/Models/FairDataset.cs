using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Kartverket.Register.Models.FAIR;
using Resources;
using System.Collections.Generic;

namespace Kartverket.Register.Models
{
    public class FairDataset : DatasetV2
    {
        //FAIR delivery status

        //Findable
        [ForeignKey("FindableStatus"), Required, Display(Name = "Findable-status:")]
        public Guid FindableStatusId { get; set; }
        public double FindableStatusPerCent { get; set; }
        public virtual FAIRDelivery FindableStatus { get; set; }

        //Accesible 
        [ForeignKey("AccesibleStatus"), Required, Display(Name = "Accesible-status:")]
        public Guid AccesibleStatusId { get; set; }
        public double AccesibleStatusPerCent { get; set; }
        public virtual FAIRDelivery AccesibleStatus { get; set; }

        //Interoperable 
        [ForeignKey("InteroperableStatus"), Required, Display(Name = "Interoperable-status:")]
        public Guid InteroperableStatusId { get; set; }
        public double InteroperableStatusPerCent { get; set; }
        public virtual FAIRDelivery InteroperableStatus { get; set; }

        //Re-useable 
        [ForeignKey("ReUseableStatus"), Required, Display(Name = "Re-useable-status:")]
        public Guid ReUseableStatusId { get; set; }
        public double ReUseableStatusPerCent { get; set; }
        public virtual FAIRDelivery ReUseableStatus { get; set; }

        //Total FAIR 
        [ForeignKey("FAIRStatus"), Required, Display(Name = "FAIR-status:")]
        public Guid FAIRStatusId { get; set; }
        public double FAIRStatusPerCent { get; set; }
        public virtual FAIRDelivery FAIRStatus { get; set; }


        #region Findable

        [Display(Name = "F1_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool F1_a_Criteria { get; set; } = true;

        [Display(Name = "F2_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool F2_a_Criteria { get; set; } = false;

        [Display(Name = "F2_b_Criteria", ResourceType = typeof(FairDataSet))]
        public bool F2_b_Criteria { get; set; } = false;

        [Display(Name = "F2_c_Criteria", ResourceType = typeof(FairDataSet))]
        public bool F2_c_Criteria { get; set; } = false;

        [Display(Name = "F2_d_Criteria", ResourceType = typeof(FairDataSet))]
        public bool F2_d_Criteria { get; set; } = false;

        [Display(Name = "F2_e_Criteria", ResourceType = typeof(FairDataSet))]
        public bool F2_e_Criteria { get; set; } = false;

        [Display(Name = "F3_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool F3_a_Criteria { get; set; } = false;

        [Display(Name = "F4_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool F4_a_Criteria { get; set; } = true;

        #endregion

        #region Accesible

        [Display(Name = "A1_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool A1_a_Criteria { get; set; } = false;

        [Display(Name = "A1_b_Criteria", ResourceType = typeof(FairDataSet))]
        public bool A1_b_Criteria { get; set; } = false;

        [Display(Name = "A1_c_Criteria", ResourceType = typeof(FairDataSet))]
        public bool A1_c_Criteria { get; set; } = false;

        [Display(Name = "A1_d_Criteria", ResourceType = typeof(FairDataSet))]
        public bool A1_d_Criteria { get; set; } = false;

        [Display(Name = "A1_e_Criteria", ResourceType = typeof(FairDataSet))]
        public bool A1_e_Criteria { get; set; } = false;

        [Display(Name = "A1_f_Criteria", ResourceType = typeof(FairDataSet))]
        public bool A1_f_Criteria { get; set; } = true;

        [Display(Name = "A2_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool A2_a_Criteria { get; set; } = false;

        #endregion

        #region Interoperable

        [Display(Name = "I1_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool I1_a_Criteria { get; set; } = true;

        [Display(Name = "I1_b_Criteria", ResourceType = typeof(FairDataSet))]
        public bool I1_b_Criteria { get; set; } = false;

        [Display(Name = "I1_c_Criteria", ResourceType = typeof(FairDataSet))]
        public bool? I1_c_Criteria { get; set; } = null;

        [Display(Name = "I2_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool I2_a_Criteria { get; set; } = false;

        [Display(Name = "I2_b_Criteria", ResourceType = typeof(FairDataSet))]
        public bool I2_b_Criteria { get; set; } = false;

        [Display(Name = "I3_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool? I3_a_Criteria { get; set; } = null;

        [Display(Name = "I3_b_Criteria", ResourceType = typeof(FairDataSet))]
        public bool? I3_b_Criteria { get; set; } = null;

        #endregion

        #region Re-useable

        [Display(Name = "R1_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool R1_a_Criteria { get; set; } = false;

        [Display(Name = "R2_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool R2_a_Criteria { get; set; } = false;

        [Display(Name = "R2_b_Criteria", ResourceType = typeof(FairDataSet))]
        public bool R2_b_Criteria { get; set; } = false;

        [Display(Name = "R2_c_Criteria", ResourceType = typeof(FairDataSet))]
        public bool R2_c_Criteria { get; set; } = false;

        [Display(Name = "R2_d_Criteria", ResourceType = typeof(FairDataSet))]
        public bool R2_d_Criteria { get; set; } = false;

        [Display(Name = "R2_e_Criteria", ResourceType = typeof(FairDataSet))]
        public bool R2_e_Criteria { get; set; } = false;

        [Display(Name = "R2_f_Criteria", ResourceType = typeof(FairDataSet))]
        public bool R2_f_Criteria { get; set; } = false;

        [Display(Name = "R3_a_Criteria", ResourceType = typeof(FairDataSet))]
        public bool R3_a_Criteria { get; set; } = true;

        [Display(Name = "R3_b_Criteria", ResourceType = typeof(FairDataSet))]
        public bool R3_b_Criteria { get; set; } = false;

        #endregion

        //Mareano delivery statuses

        //Metadata
        [ForeignKey("MetadataStatus"), Required, Display(Name = "Metadata:")]
        public Guid MetadataStatusId { get; set; }
        public virtual DatasetDelivery MetadataStatus { get; set; }

        //Produktspesifikasjon
        [ForeignKey("ProductSpesificationStatus"), Required, Display(Name = "Produktspesifikasjon:")]
        public Guid ProductSpesificationStatusId { get; set; }
        public virtual DatasetDelivery ProductSpesificationStatus { get; set; }

        //Produktark
        [ForeignKey("ProductSheetStatus"), Required, Display(Name = "Produktark:")]
        public Guid? ProductSheetStatusId { get; set; }
        public virtual DatasetDelivery ProductSheetStatus { get; set; }

        //Tegneregler
        [ForeignKey("PresentationRulesStatus"), Required, Display(Name = "Tegneregler:")]
        public Guid? PresentationRulesStatusId { get; set; }
        public virtual DatasetDelivery PresentationRulesStatus { get; set; }

        //SOSI-data i hht nasjonal produkstspesifikasjon
        [ForeignKey("SosiDataStatus"), Required, Display(Name = "SOSI-data i hht nasjonal produkstspesifikasjon :")]
        public Guid SosiDataStatusId { get; set; }
        public virtual DatasetDelivery SosiDataStatus { get; set; }

        //GML-data i hht nasjonal produkstspesifikasjon
        [ForeignKey("GmlDataStatus"), Required, Display(Name = "GML-data i hht nasjonal produkstspesifikasjon :")]
        public Guid GmlDataStatusId { get; set; }
        public virtual DatasetDelivery GmlDataStatus { get; set; }

        //View service (Visningstjeneste)
        [ForeignKey("WmsStatus"), Required, Display(Name = "Visningstjeneste WMS/WMTS:")]
        public Guid WmsStatusId { get; set; }
        public virtual DatasetDelivery WmsStatus { get; set; }

        //WFS
        [ForeignKey("WfsStatus"), Required, Display(Name = "Nedlastingstjeneste WFS:")]
        public Guid WfsStatusId { get; set; }
        public virtual DatasetDelivery WfsStatus { get; set; }

        //Atom-feed
        [ForeignKey("AtomFeedStatus"), Required, Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public Guid AtomFeedStatusId { get; set; }
        public virtual DatasetDelivery AtomFeedStatus { get; set; }

        //Nedlastingstjeneste - felles/kombi
        [ForeignKey("CommonStatus"), Required, Display(Name = "Nedlastingstjeneste Atom-feed:")]
        public Guid CommonStatusId { get; set; }
        public virtual DatasetDelivery CommonStatus { get; set; }

        public float? Grade { get; set; }

        public ICollection<FairDatasetType> FairDatasetTypes { get; set; }


        public string DetailPageUrl()
        {
            return Register.GetObjectUrl() + "/" + Seoname + "/" + Uuid;
        }
    }
}