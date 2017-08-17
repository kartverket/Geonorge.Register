///////////////////////////////////////////////////////////
//  Dataset.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class DatasetNy : RegisterItemNy
    {
        [Display(Name = "Uuid:")]
        public string Uuid { get; set; }

        [Display(Name = "Merknad:")]
        public string Notes { get; set; }

        [Display(Name = "Bruksområde:")]
        public string SpecificUsage { get; set; }

        [Display(Name = "Produktark url:")]
        public string ProductSheetUrl { get; set; }

        [Display(Name = "Presentasjonsregler url:")]
        public string PresentationRulesUrl { get; set; }

        [Display(Name = "Produktspesifikasjon url:")]
        public string ProductSpecificationUrl { get; set; }

        [Display(Name = "Metadata url:")]
        public string MetadataUrl { get; set; }

        [Display(Name = "Distribusjonsformat:")]
        public string DistributionFormat { get; set; }

        [Display(Name = "Distribusjon url:")]
        public string DistributionUrl { get; set; }

        [Display(Name = "Distribusjonsområde:")]
        public string DistributionArea { get; set; }

        [Display(Name = "WMS url:")]
        public string WmsUrl { get; set; }

        [Display(Name = "Tema:")]
        [ForeignKey("theme")]
        public string ThemeGroupId { get; set; }

        public virtual DOKTheme theme { get; set; }

        [Display(Name = "Miniatyrbilde:")]
        public string datasetthumbnail { get; set; }

        [ForeignKey("DokStatus")]
        [Display(Name = "DOK-status:")]
        public string DokStatusId { get; set; }

        public virtual DokStatus DokStatus { get; set; }

        [Display(Name = "DOK-status godkjent:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DokStatusDateAccepted { get; set; }
    }

}//end namespace Datamodell