using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using Kartverket.Register.Services.RegisterItem;
using Kartverket.Register.Services.Versioning;

namespace Kartverket.Register.Services
{
    public class DatasetService : IDatasetService
    {
        private readonly RegisterDbContext _context;
        private readonly IVersioningService _versioningService;
        private readonly IRegisterService _registerService;
        private readonly IRegisterItemService _registerItemService;
        private readonly IDatasetDeliveryService _datasetDeliveryService;
        private readonly CoverageService _coverageService;


        public DatasetService(RegisterDbContext context)
        {
            _context = context;
            _versioningService = new VersioningService(_context);
            _registerService = new RegisterService(_context);
            _registerItemService = new RegisterItemService(_context);
            _datasetDeliveryService = new DatasetDeliveryService(_context);
            _coverageService = new CoverageService(_context);
        }

        public Dataset GetDatasetByUuid(string uuid)
        {
            return _context.Datasets.First(d => d.Uuid == uuid);
        }

        public SelectList GetDokStatusSelectList(string statusId)
        {
            return new SelectList(_context.DokStatuses, "value", "description", statusId);
        }

        public Dataset UpdateDataset(Dataset inputDataset, Dataset originalDataset = null, CoverageDataset coverage = null)
        {
            Dataset dataset = originalDataset ?? new Dataset();
            dataset.systemId = inputDataset.GetSystemId();
            dataset.modified = dataset.GetDateModified();
            dataset.dateSubmitted = dataset.GetDateSubmbitted();
            dataset.register = inputDataset.register;
            dataset.registerId = inputDataset.registerId;

            dataset.DatasetType = dataset.GetDatasetType();
            dataset.statusId = dataset.SetStatusId();
            dataset.dokStatusId = inputDataset.GetDokStatus();
            dataset.dokStatusDateAccepted = inputDataset.GetDokStatusDateAccepted();
            dataset.Kandidatdato = inputDataset.Kandidatdato;
            dataset.versionNumber = dataset.GetVersionNr();
            dataset.versioningId = GetVersioningId(dataset);
            dataset.datasetownerId = GetDatasetOwnerId(inputDataset.datasetownerId);
            dataset.datasetowner = (Organization)_registerItemService.GetRegisterItemBySystemId(dataset.datasetownerId);
            dataset.submitterId = GetSubmitterId(inputDataset.submitterId);
            dataset.submitter = (Organization)_registerItemService.GetRegisterItemBySystemId(dataset.submitterId);
            dataset.Notes = inputDataset.GetNotes();

            dataset.name = inputDataset.GetName();
            dataset.seoname = RegisterUrls.MakeSeoFriendlyString(dataset.name);
            dataset.description = inputDataset.GetDescription();
            dataset.DistributionUrl = inputDataset.GetDistributionUrl();
            dataset.MetadataUrl = inputDataset.GetMetadataUrl();
            dataset.PresentationRulesUrl = inputDataset.GetPresentationRulesUrl();
            dataset.ProductSheetUrl = inputDataset.GetProductSheetUrl();
            dataset.ProductSpecificationUrl = inputDataset.GetProductSpecificationUrl();
            dataset.UuidService = inputDataset.GetServceUuid();
            dataset.WmsUrl = inputDataset.GetWmsUrl();
            dataset.DistributionFormat = inputDataset.GetDistributionFormat();
            dataset.DistributionArea = inputDataset.GetDistributionArea();
            dataset.ThemeGroupId = inputDataset.GetThemeGroupId();
            dataset.datasetthumbnail = inputDataset.Getdatasetthumbnail();
            dataset.Uuid = inputDataset.Uuid;
            dataset.SpecificUsage = inputDataset.SpecificUsage;
            dataset.restricted = inputDataset.restricted;

            dataset.dokDeliveryMetadataStatusId = _datasetDeliveryService.GetMetadataStatus(inputDataset.Uuid, inputDataset.dokDeliveryMetadataStatusAutoUpdate, inputDataset.dokDeliveryMetadataStatusId);
            dataset.dokDeliveryMetadataStatusNote = inputDataset.dokDeliveryMetadataStatusNote;
            dataset.dokDeliveryMetadataStatusAutoUpdate = inputDataset.dokDeliveryMetadataStatusAutoUpdate;
            dataset.dokDeliveryProductSheetStatusId = _registerService.GetDOKStatus(inputDataset.GetProductSheetUrl(), inputDataset.dokDeliveryProductSheetStatusAutoUpdate, inputDataset.dokDeliveryProductSheetStatusId);
            dataset.dokDeliveryProductSheetStatusNote = inputDataset.dokDeliveryProductSheetStatusNote;
            dataset.dokDeliveryProductSheetStatusAutoUpdate = inputDataset.dokDeliveryProductSheetStatusAutoUpdate;
            dataset.dokDeliveryPresentationRulesStatusId = _registerService.GetDOKStatusPresentationRules(inputDataset.GetPresentationRulesUrl(), inputDataset.dokDeliveryPresentationRulesStatusAutoUpdate, inputDataset.dokDeliveryPresentationRulesStatusId, dataset.Uuid);
            dataset.dokDeliveryPresentationRulesStatusNote = inputDataset.dokDeliveryPresentationRulesStatusNote;
            dataset.dokDeliveryPresentationRulesStatusAutoUpdate = inputDataset.dokDeliveryPresentationRulesStatusAutoUpdate;
            dataset.dokDeliveryProductSpecificationStatusId = _registerService.GetDOKStatus(inputDataset.GetProductSpecificationUrl(), inputDataset.dokDeliveryProductSpecificationStatusAutoUpdate, inputDataset.dokDeliveryProductSpecificationStatusId);
            dataset.dokDeliveryProductSpecificationStatusNote = inputDataset.dokDeliveryProductSpecificationStatusNote;
            dataset.dokDeliveryProductSpecificationStatusAutoUpdate = inputDataset.dokDeliveryProductSpecificationStatusAutoUpdate;
            dataset.dokDeliveryWmsStatusId = _datasetDeliveryService.GetDokDeliveryServiceStatus(inputDataset.Uuid, inputDataset.dokDeliveryWmsStatusAutoUpdate, inputDataset.dokDeliveryWmsStatusId, inputDataset.UuidService);
            dataset.dokDeliveryWmsStatusNote = inputDataset.dokDeliveryWmsStatusNote;
            dataset.dokDeliveryWmsStatusAutoUpdate = inputDataset.dokDeliveryWmsStatusAutoUpdate;
            dataset.dokDeliveryWfsStatusId = _datasetDeliveryService.GetWfsStatus(inputDataset.Uuid, inputDataset.dokDeliveryWfsStatusAutoUpdate, inputDataset.dokDeliveryWfsStatusId);
            dataset.dokDeliveryWfsStatusNote = inputDataset.dokDeliveryWfsStatusNote;
            dataset.dokDeliveryWfsStatusAutoUpdate = inputDataset.dokDeliveryWfsStatusAutoUpdate;
            dataset.dokDeliverySosiRequirementsStatusId = _registerService.GetSosiRequirements(inputDataset.Uuid, inputDataset.GetProductSpecificationUrl(), inputDataset.dokDeliverySosiStatusAutoUpdate, inputDataset.dokDeliverySosiRequirementsStatusId);
            dataset.dokDeliverySosiRequirementsStatusNote = inputDataset.dokDeliverySosiRequirementsStatusNote;
            dataset.dokDeliverySosiStatusAutoUpdate = inputDataset.dokDeliverySosiStatusAutoUpdate;
            dataset.dokDeliveryGmlRequirementsStatusId = _registerService.GetGmlRequirements(inputDataset.Uuid, inputDataset.dokDeliveryGmlRequirementsStatusAutoUpdate, inputDataset.dokDeliveryGmlRequirementsStatusId);
            dataset.dokDeliveryGmlRequirementsStatusNote = inputDataset.dokDeliveryGmlRequirementsStatusNote;
            dataset.dokDeliveryGmlRequirementsStatusAutoUpdate = inputDataset.dokDeliveryGmlRequirementsStatusAutoUpdate;
            dataset.dokDeliveryAtomFeedStatusId = _datasetDeliveryService.GetAtomFeedStatus(inputDataset.Uuid, inputDataset.dokDeliveryAtomFeedStatusAutoUpdate, inputDataset.dokDeliveryAtomFeedStatusId);
            dataset.dokDeliveryAtomFeedStatusNote = inputDataset.dokDeliveryAtomFeedStatusNote;
            dataset.dokDeliveryAtomFeedStatusAutoUpdate = inputDataset.dokDeliveryAtomFeedStatusAutoUpdate;
            dataset.dokDeliveryDistributionStatusNote = inputDataset.dokDeliveryDistributionStatusNote;
            dataset.dokDeliveryDistributionStatusAutoUpdate = inputDataset.dokDeliveryDistributionStatusAutoUpdate;
            dataset.dokDeliveryDistributionStatusId = inputDataset.dokDeliveryDistributionStatusId;
            dataset.dokDeliveryDistributionStatusId = _registerService.GetDeliveryDownloadStatus(inputDataset.Uuid, inputDataset.dokDeliveryDistributionStatusAutoUpdate, dataset.dokDeliveryDistributionStatusId, dataset.dokDeliveryWfsStatusId, dataset.dokDeliveryAtomFeedStatusId);

            dataset.Coverage = EditDatasetCoverage(coverage, dataset, originalDataset);

            dataset.RegionalPlan = inputDataset.RegionalPlan;
            dataset.RegionalPlanNote = inputDataset.RegionalPlanNote;
            dataset.MunicipalSocialPlan = inputDataset.MunicipalSocialPlan;
            dataset.MunicipalSocialPlanNote = inputDataset.MunicipalSocialPlanNote;
            dataset.MunicipalLandUseElementPlan = inputDataset.MunicipalLandUseElementPlan;
            dataset.MunicipalLandUseElementPlanNote = inputDataset.MunicipalLandUseElementPlanNote;
            dataset.ZoningPlanArea = inputDataset.ZoningPlanArea;
            dataset.ZoningPlanAreaNote = inputDataset.ZoningPlanAreaNote;
            dataset.ZoningPlanDetails = inputDataset.ZoningPlanDetails;
            dataset.ZoningPlanDetailsNote = inputDataset.ZoningPlanDetailsNote;
            dataset.BuildingMatter = inputDataset.BuildingMatter;
            dataset.BuildingMatterNote = inputDataset.BuildingMatterNote;
            dataset.PartitionOff = inputDataset.PartitionOff;
            dataset.PartitionOffNote = inputDataset.PartitionOffNote;
            dataset.EenvironmentalImpactAssessment = inputDataset.EenvironmentalImpactAssessment;
            dataset.EenvironmentalImpactAssessmentNote = inputDataset.EenvironmentalImpactAssessmentNote;

            dataset.SetAtomAndGmlIsEitherOrRequirement(dataset);

            return dataset;
        }

        public List<CoverageDataset> EditDatasetCoverage(CoverageDataset coverage, Dataset dataset, Dataset originalDataset = null)
        {
            if (coverage != null && originalDataset != null)
            {
                CoverageDataset originalCoverage = _registerItemService.GetMunicipalityCoverage(dataset, originalDataset.datasetownerId);

                if (dataset.IsMunicipalDataset())
                {
                    if (originalCoverage != null)
                    {
                        originalCoverage.MunicipalityId = dataset.datasetownerId;
                        originalCoverage.CoverageDOKStatusId = dataset.dokStatusId;
                        originalCoverage.ConfirmedDok = coverage.ConfirmedDok;
                        _registerItemService.Save();
                    }
                }
                else
                {
                    if (originalCoverage == null)
                    {
                        CoverageDataset newCoverage = new CoverageDataset()
                        {
                            CoverageId = Guid.NewGuid(),
                            CoverageDOKStatus = coverage.CoverageDOKStatus,
                            CoverageDOKStatusId = coverage.CoverageDOKStatusId,
                            ConfirmedDok = coverage.ConfirmedDok,
                            DatasetId = dataset.systemId,
                            MunicipalityId = _registerService.GetOrganizationIdByUserName(),
                            Note = coverage.Note,
                        };
                        _registerItemService.SaveNewCoverage(newCoverage);
                        dataset.Coverage.Add(newCoverage);
                    }
                    else
                    {
                        originalCoverage.ConfirmedDok = coverage.ConfirmedDok;
                        originalCoverage.CoverageDOKStatusId = coverage.CoverageDOKStatusId;
                        originalCoverage.Note = coverage.Note;
                    }
                }
            }
            else if (coverage == null && dataset.IsMunicipalDataset())
            {
                dataset.Coverage.Add(_registerItemService.NewCoverage(dataset));
            }

            return dataset.Coverage;
        }

        private Guid GetSubmitterId(Guid submitterId)
        {
            if (submitterId == null || submitterId == Guid.Empty)
            {
                Organization submitter = _registerService.GetOrganizationByUserName();
                return submitter.systemId;
            }
            return submitterId;
        }

        private Guid GetDatasetOwnerId(Guid datasetownerId)
        {
            if (datasetownerId == Guid.Empty)
            {
                Organization datasetOwner = _registerService.GetOrganizationByUserName();
                return datasetOwner.systemId;
            }
            return datasetownerId;
        }

        private Guid GetVersioningId(Dataset dataset)
        {
            return dataset.versioningId == Guid.Empty ? _versioningService.NewVersioningGroup(dataset) : dataset.GetVersioningId();
        }
    }
}