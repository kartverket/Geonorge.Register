using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class DatasetViewModel : RegisterItemV2ViewModel
    {
        [Display(Name = "Uuid")]
        public string Uuid { get; set; }

        [Display(Name = "Merknad")]
        public string Notes { get; set; }

        [Display(Name = "Bruksområde")]
        public string SpecificUsage { get; set; }

        [Display(Name = "Produktark url")]
        public string ProductSheetUrl { get; set; }

        [Display(Name = "Presentasjonsregler url")]
        public string PresentationRulesUrl { get; set; }

        [Display(Name = "Produktspesifikasjon url")]
        public string ProductSpecificationUrl { get; set; }

        [Display(Name = "Metadata url")]
        public string MetadataUrl { get; set; }

        [Display(Name = "Distribusjonsformat")]
        public string DistributionFormat { get; set; }

        [Display(Name = "Distribusjon url")]
        public string DistributionUrl { get; set; }

        [Display(Name = "Distribusjonsområde")]
        public string DistributionArea { get; set; }

        [Display(Name = "WMS url")]
        public string WmsUrl { get; set; }

        [Display(Name = "Tema")]
        public string ThemeGroupId { get; set; }
        public virtual DOKTheme Theme { get; set; }

        [Display(Name = "Miniatyrbilde")]
        public string DatasetThumbnail { get; set; }

        [Display(Name = "DOK-status")]
        public string DokStatusId { get; set; }
        public virtual DokStatus DokStatus { get; set; }

        [Display(Name = "DOK-status godkjent")]
        public DateTime? DokStatusDateAccepted { get; set; }

        //Search for metadata
        public string SearchString { get; set; }
        public List<MetadataItemViewModel> SearchResultList { get; set; }

        //SelectList
        public SelectList DokStatusSelectList { get; set; }

        public void UpdateDataset(DatasetV2 dataset)
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
            Theme = dataset.Theme;
            DatasetThumbnail = dataset.DatasetThumbnail;
            DokStatusId = dataset.DokStatusId;
            DokStatus = dataset.DokStatus;
            DokStatusDateAccepted = dataset.DokStatusDateAccepted;

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
    }
}