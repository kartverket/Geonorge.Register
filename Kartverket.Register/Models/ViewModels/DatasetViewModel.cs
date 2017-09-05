using System;

namespace Kartverket.Register.Models.ViewModels
{
    public class DatasetViewModel : RegisterItemV2VeiwModel
    {

        public string Uuid { get; set; }
        public string Notes { get; set; }
        public string SpecificUsage { get; set; }
        public string ProductSheetUrl { get; set; }
        public string PresentationRulesUrl { get; set; }
        public string ProductSpecificationUrl { get; set; }
        public string MetadataUrl { get; set; }
        public string DistributionFormat { get; set; }
        public string DistributionUrl { get; set; }
        public string DistributionArea { get; set; }
        public string WmsUrl { get; set; }
        public string ThemeGroupId { get; set; }
        public virtual DOKTheme Theme { get; set; }
        public string DatasetThumbnail { get; set; }
        public string DokStatusId { get; set; } = "Proposal";
        public virtual DokStatus DokStatus { get; set; }
        public DateTime? DokStatusDateAccepted { get; set; }

    }
}