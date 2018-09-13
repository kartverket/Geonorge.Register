using Kartverket.Register.Helpers;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class DatasetViewModel : RegisterItemV2ViewModel
    {
        [Display(Name = "Uuid")]
        public string Uuid { get; set; }

        [Display(Name = "DOK_Note", ResourceType = typeof(DataSet))]
        public string Notes { get; set; }

        [Display(Name = "DOK_SpecificUsage", ResourceType = typeof(DataSet))]
        public string SpecificUsage { get; set; }

        [Display(Name = "DOK_ProductSheetUrl", ResourceType = typeof(DataSet))]
        public string ProductSheetUrl { get; set; }

        [Display(Name = "DOK_PresentationRulesUrl", ResourceType = typeof(DataSet))]
        public string PresentationRulesUrl { get; set; }

        [Display(Name = "DOK_ProductSpecificationUrl", ResourceType = typeof(DataSet))]
        public string ProductSpecificationUrl { get; set; }

        [Display(Name = "Metadata url")]
        public string MetadataUrl { get; set; }

        [Display(Name = "DOK_Distribusjonsformat", ResourceType = typeof(DataSet))]
        public string DistributionFormat { get; set; }

        [Display(Name = "DOK_DistributionUrl", ResourceType = typeof(DataSet))]
        public string DistributionUrl { get; set; }

        [Display(Name = "DOK_DistributionArea", ResourceType = typeof(DataSet))]
        public string DistributionArea { get; set; }

        [Display(Name = "WMS url")]
        public string WmsUrl { get; set; }

        [Display(Name = "DOK_Temagruppe", ResourceType = typeof(DataSet))]
        public string ThemeGroupId { get; set; }
        public virtual DOKTheme Theme { get; set; }

        [Display(Name = "DOK_Datasetthumbnail", ResourceType = typeof(DataSet))]
        public string DatasetThumbnail { get; set; }

        [Display(Name = "DOK-status")]
        public string DokStatusId { get; set; }
        public virtual DokStatus DokStatus { get; set; }

        [Display(Name = "DOK_StatusDateAccepted", ResourceType = typeof(DataSet))]
        public DateTime? DokStatusDateAccepted { get; set; }

        [Display(Name = "ServiceUuid", ResourceType = typeof(DataSet))]
        public string UuidService { get; set; }

        public ICollection<DatasetStatusHistory> StatusHistories { get; set; }

        //Search for metadata
        public string SearchString { get; set; }
        public List<MetadataItemViewModel> SearchResultList { get; set; }

        //SelectList
        public SelectList DokStatusSelectList { get; set; }

        [Display(Name = "DOK_DatasetType", ResourceType = typeof(DataSet))]
        public string DatasetType { get; set; }

        public void UpdateDataset(DatasetV2 dataset)
        {
            var cultureName = CultureHelper.GetCurrentCulture();

            string specificUsage = !CultureHelper.IsNorwegian(cultureName) && !string.IsNullOrEmpty(dataset.SpecificUsageEnglish)
                          ? dataset.SpecificUsageEnglish : dataset.SpecificUsage;

            Uuid = dataset.Uuid;
            Notes = dataset.Notes;
            MetadataUrl = dataset.MetadataUrl;
            SpecificUsage = specificUsage;
            ProductSheetUrl = dataset.ProductSheetUrl;
            PresentationRulesUrl = dataset.PresentationRulesUrl;
            ProductSpecificationUrl = dataset.ProductSpecificationUrl;
            DistributionFormat = dataset.DistributionFormat;
            DistributionUrl = dataset.DistributionUrl;
            DistributionArea = dataset.DistributionArea;
            WmsUrl = dataset.WmsUrl;
            ThemeGroupId = dataset.ThemeGroupId;
            Theme = dataset.Theme;
            DatasetThumbnail = dataset.DatasetThumbnail;
            DokStatusId = dataset.DokStatusId;
            DokStatus = dataset.DokStatus;
            DokStatusDateAccepted = dataset.DokStatusDateAccepted;
            UuidService = dataset.UuidService;

            UpdateRegisterItem(dataset);
        }

        public DatasetViewModel()
        {
        }

        public void UpdateDataset(Dataset dataset)
        {
            Uuid = dataset.Uuid;
            Notes = dataset.Notes;
            MetadataUrl = dataset.MetadataUrl;
            SpecificUsage = dataset.SpecificUsage;
            ProductSheetUrl = dataset.ProductSheetUrl;
            PresentationRulesUrl = dataset.PresentationRulesUrl;
            ProductSpecificationUrl = dataset.ProductSpecificationUrl;
            DistributionFormat = dataset.DistributionFormat;
            DistributionUrl = dataset.DistributionUrl;
            DistributionArea = dataset.DistributionArea;
            WmsUrl = dataset.WmsUrl;
            ThemeGroupId = dataset.ThemeGroupId;
            DatasetThumbnail = dataset.datasetthumbnail;
            DokStatusId = dataset.dokStatusId;
            DokStatus = dataset.dokStatus;
            DokStatusDateAccepted = dataset.dateAccepted;
            UuidService = dataset.UuidService;
            StatusHistories = dataset.StatusHistories;
            DatasetType = dataset.GetDatasetType();

            UpdateRegisterItem(dataset);
        }

        public string GetThemeGroupDescription()
        {
            return !string.IsNullOrWhiteSpace(Theme?.description) ? Theme.description : "Ikke angitt";
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
    }
}