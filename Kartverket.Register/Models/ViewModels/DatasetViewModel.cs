using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class DatasetViewModel : RegisterItemV2VeiwModel
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
        public string DokStatusId { get; set; } = "Proposal";
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
            UpdateRegisterItem(dataset);
            PresentationRulesUrl = dataset.PresentationRulesUrl;
            ProductSheetUrl = dataset.ProductSheetUrl;
            ProductSpecificationUrl = dataset.ProductSpecificationUrl;
            SpecificUsage = dataset.SpecificUsage;
            DatasetThumbnail = dataset.DatasetThumbnail;

            ThemeGroupId = dataset.ThemeGroupId;
            Uuid = dataset.Uuid;
            WmsUrl = dataset.WmsUrl;
            DistributionUrl = dataset.DistributionUrl;
            DistributionArea = dataset.DistributionArea;
            DistributionFormat = dataset.DistributionFormat;
        }
    }
}