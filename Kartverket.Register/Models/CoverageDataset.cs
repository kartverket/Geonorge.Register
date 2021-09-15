using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Models
{
    public class CoverageDataset
    {
        [Key]
        public Guid CoverageId { get; set; }

        [Display(Name = "Kommune")]
        [ForeignKey("Municipality")]
        public Guid MunicipalityId { get; set; }
        public virtual Organization Municipality { get; set; }

        [Display(Name = "Bekreftet")]
        public bool ConfirmedDok { get; set; }

        [Display(Name = "Dekning")]
        public bool? Coverage { get; set; }

        [ForeignKey("dataset")]
        [Display(Name = "Datasett")]
        public Guid DatasetId { get; set; }
        public virtual Dataset dataset { get; set; }

        [Display(Name = "Merknad")]
        public string Note { get; set; }

        [ForeignKey("CoverageDOKStatus")]
        [Display(Name = "DOK-status")]
        public string CoverageDOKStatusId { get; set; }
        public virtual DokStatus CoverageDOKStatus { get; set; }

        [ForeignKey("MeasureDOKStatus")]
        [Display(Name = "Tiltak")]
        public string MeasureDOKStatusId { get; set; }
        public virtual DokMeasureStatus MeasureDOKStatus { get; set; }

        // DOK Suitability Rating
        [Display(Name = "Regionplan")]
        public bool RegionalPlan { get; set; }
        [Display(Name = "kommuneplanens samfunnsdel")]
        public bool MunicipalSocialPlan { get; set; }
        [Display(Name = "kommuneplanens arealdel")]
        public bool MunicipalLandUseElementPlan { get; set; }
        [Obsolete]
        [Display(Name = "Reguleringsplan område")]
        public bool ZoningPlanArea { get; set; }
        [Display(Name = "Reguleringsplan detalj")]
        [Obsolete]
        public bool ZoningPlanDetails { get; set; }
        [Display(Name = "Reguleringsplan")]
        public bool ZoningPlan { get; set; }
        [Display(Name = "Byggesak")]
        public bool BuildingMatter { get; set; }
        [Display(Name = "Fradeling")]
        public bool PartitionOff { get; set; }
        [Obsolete]
        [Display(Name = "KU og ROS for pbl-planer")]
        public bool EenvironmentalImpactAssessment { get; set; }

        [Display(Name = "KU for pbl-planer")]
        public bool ImpactAssessmentPlanningBuildingAct { get; set; }

        [Display(Name = "ROS for pbl-planer")]
        public bool RiskVulnerabilityAnalysisPlanningBuildingAct { get; set; }

        public string suitabilityAssessmentText { get; set; }

    }
}