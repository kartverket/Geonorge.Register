using System;

namespace Kartverket.Register.Models.ViewModels
{
    public class DokMunicipalSuitabilityAssessmentRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Themegroup { get; set; }
        public string Owner { get; set; }
        public string NationalDokStatus { get; set; }
        public string Type { get; set; }
        public Guid MunicipalityId { get; set; }
        public string MunicipalityName { get; set; }
        public string MunicipalityCode { get; set; }
        public bool RegionalPlan { get; set; }
        public bool MunicipalSocialPlan { get; set; }
        public bool MunicipalLandUseElementPlan { get; set; }
        public bool ZoningPlanArea { get; set; }
        public bool ZoningPlanDetails { get; set; }
        public bool BuildingMatter { get; set; }
        public bool PartitionOff { get; set; }
        public bool EnvironmentalImpactAssessment { get; set; }


        public DokMunicipalSuitabilityAssessmentRow(Dataset dataset, Organization municipality)
        {
            Id = dataset.systemId;
            Name = dataset.name;
            Themegroup = dataset.ThemeGroupId;
            Owner = dataset.datasetowner.name;
            MunicipalityId = municipality.systemId;
            if (dataset.IsNationalDataset())
            {
                NationalDokStatus = dataset.dokStatus.description;
            }
            Type = dataset.DatasetType;
            RegionalPlan = dataset.GetCoverageRegionalPlaneByUser(municipality.systemId);
            MunicipalSocialPlan = dataset.GetCoverageMunicipalSocialPlanByUser(municipality.systemId);
            MunicipalLandUseElementPlan = dataset.GetCoverageMunicipalLandUseElementPlanByUser(municipality.systemId);
            ZoningPlanArea = dataset.GetCoverageZoningPlanAreaByUser(municipality.systemId);
            ZoningPlanDetails = dataset.GetCoverageZoningPlanDetailsByUser(municipality.systemId);
            BuildingMatter = dataset.GetCoverageBuildingMatterByUser(municipality.systemId);
            PartitionOff = dataset.GetCoveragePartitionOffByUser(municipality.systemId);
            EnvironmentalImpactAssessment = dataset.GetCoverageEenvironmentalImpactAssessmentByUser(municipality.systemId);
        }

        public DokMunicipalSuitabilityAssessmentRow() {

        }
    }
}