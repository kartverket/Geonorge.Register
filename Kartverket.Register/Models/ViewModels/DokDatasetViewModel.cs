using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class DokDatasetViewModel : DatasetViewModel
    {

        [Display(Name = "DOK_Kandidatdato", ResourceType = typeof(DataSet))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Kandidatdato { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);

        //Metadata
        [Display(Name = "DOK_Delivery_Metadata", ResourceType = typeof(DataSet))]
        public string DokDeliveryMetadataStatus { get; set; }
        public string DokDeliveryMetadataStatusNote { get; set; }

        //ProductSheet
        [Display(Name = "DOK_ProductSheetStatus", ResourceType = typeof(DataSet))]
        public string DokDeliveryProductSheetStatus { get; set; }
        public string DokDeliveryProductSheetStatusNote { get; set; }

        //PresentationRules
        [Display(Name = "DOK_PresentationRulesStatus", ResourceType = typeof(DataSet))]
        public string DokDeliveryPresentationRulesStatus { get; set; }
        public string DokDeliveryPresentationRulesStatusNote { get; set; }

        //ProductSpecification
        [Display(Name = "DOK_ProductSpecificationStatus", ResourceType = typeof(DataSet))]
        public string DokDeliveryProductSpecificationStatus { get; set; }
        public string DokDeliveryProductSpecificationStatusNote { get; set; }

        //WMS
        [Display(Name = "DOK_Delivery_Wms", ResourceType = typeof(DataSet))]
        public string DokDeliveryWmsStatus { get; set; }
        public string DokDeliveryWmsStatusNote { get; set; }
        public bool DokDeliveryWmsStatusAutoUpdate { get; set; } = true;

        //WFS
        [Display(Name = "DOK_Delivery_Wfs", ResourceType = typeof(DataSet))]
        public string DokDeliveryWfsStatus { get; set; }
        public string DokDeliveryWfsStatusNote { get; set; }

        //Distribution
        [Display(Name = "DOK_Delivery_Distribution", ResourceType = typeof(DataSet))]
        public string DokDeliveryDistributionStatus { get; set; }
        public string DokDeliveryDistributionStatusNote { get; set; }

        //SOSI requirements
        [Display(Name = "DOK_Delivery_SosiRequirements", ResourceType = typeof(DataSet))]
        public string DokDeliverySosiRequirementsStatus { get; set; }
        public string DokDeliverySosiRequirementsStatusNote { get; set; }

        //GML requirements
        [Display(Name = "DOK_Delivery_GmlRequirements", ResourceType = typeof(DataSet))]
        public string DokDeliveryGmlRequirementsStatus { get; set; }
        public string DokDeliveryGmlRequirementsStatusNote { get; set; }

        //Atom-feed
        [Display(Name = "DOK_Delivery_AtomFeed", ResourceType = typeof(DataSet))]
        public string DokDeliveryAtomFeedStatus { get; set; }
        public string DokDeliveryAtomFeedStatusNote { get; set; }

        public bool? Restricted { get; set; }

        [Display(Name = "ServiceUuid", ResourceType = typeof(DataSet))]
        public string UuidService { get; set; }

        public DokLineChart DokHistoricalChart { get; set; }


        public DokDatasetViewModel()
        {
        }

        public DokDatasetViewModel(Dataset dataset)
        {
            Kandidatdato = dataset.Kandidatdato;
            DokDeliveryMetadataStatus = dataset.dokDeliveryMetadataStatus.DescriptionTranslated();
            DokDeliveryMetadataStatusNote = dataset.dokDeliveryMetadataStatusNote;


            DokDeliveryProductSheetStatus = dataset.dokDeliveryProductSheetStatus.DescriptionTranslated();
            DokDeliveryProductSheetStatusNote = dataset.dokDeliveryProductSheetStatusNote;

            DokDeliveryPresentationRulesStatus = dataset.dokDeliveryPresentationRulesStatus.DescriptionTranslated();
            DokDeliveryPresentationRulesStatusNote = dataset.dokDeliveryPresentationRulesStatusNote;

            DokDeliveryProductSpecificationStatus = dataset.dokDeliveryProductSpecificationStatus.DescriptionTranslated();
            DokDeliveryProductSpecificationStatusNote = dataset.dokDeliveryProductSpecificationStatusNote;

            DokDeliveryWmsStatus = dataset.dokDeliveryWmsStatus.DescriptionTranslated();
            DokDeliveryWmsStatusNote = dataset.dokDeliveryWmsStatusNote;

            DokDeliveryWfsStatus = dataset.dokDeliveryWfsStatus.DescriptionTranslated();
            DokDeliveryWfsStatusNote = dataset.dokDeliveryWfsStatusNote;

            DokDeliveryDistributionStatus = dataset.dokDeliveryDistributionStatus.DescriptionTranslated();
            DokDeliveryDistributionStatusNote = dataset.dokDeliveryDistributionStatusNote;

            DokDeliverySosiRequirementsStatus = dataset.dokDeliverySosiRequirementsStatus.DescriptionTranslated();
            DokDeliverySosiRequirementsStatusNote = dataset.dokDeliverySosiRequirementsStatusNote;

            DokDeliveryGmlRequirementsStatus = dataset.dokDeliveryGmlRequirementsStatus.DescriptionTranslated();
            DokDeliveryGmlRequirementsStatusNote = dataset.dokDeliveryGmlRequirementsStatusNote;

            DokDeliveryAtomFeedStatus = dataset.dokDeliveryAtomFeedStatus.DescriptionTranslated();
            DokDeliveryAtomFeedStatusNote = dataset.dokDeliveryAtomFeedStatusNote;

            UuidService = dataset.UuidService;
            UpdateDataset(dataset);
        }

        public string GetThemeGroupDescription()
        {
            return !string.IsNullOrWhiteSpace(Theme?.DescriptionTranslated()) ? Theme.DescriptionTranslated() : "Ikke angitt";
        }

        public string LogoSrc()
        {
            if (Owner != null) return "~/data/organizations/" + Owner.logoFilename;
            return "";
        }

        public DateTime? GetDateAccepted()
        {
            return DokStatusId == "Accepted" && DateAccepted == null ? DateTime.Now : DateAccepted;
        }

        public virtual string GetDatasetEditUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/dataset/" + Register.seoname + "/" + Register.seoname + "/" + Seoname + "/rediger";
            }
            else
            {
                return "/dataset/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/rediger";
            }
        }

        public virtual string GetDatasetDeleteUrl()
        {
            if (Register.parentRegister == null)
            {
                return "/dataset/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
            }
            else
            {
                return "/dataset/" + Register.parentRegister.seoname + "/" + Register.owner.seoname + "/" + Register.seoname + "/" + Owner.seoname + "/" + Seoname + "/slett";
            }
        }
    }
}