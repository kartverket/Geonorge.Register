using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Kartverket.Register.Models.StatusReports;

namespace Kartverket.Register.Models
{
    public class StatusReport
    {
        public StatusReport(bool latestSavedDataReport = false)
        {
            Id = Guid.NewGuid();
            Date = DateTime.Now;
            StatusRegisterItems = new List<RegisterItemStatusReport>();
            LatestSavedDataReport = latestSavedDataReport;
        }

        public StatusReport()
        {
        }


        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public bool LatestSavedDataReport { get; set; }
        public virtual Register Register { get; set; }
        public virtual ICollection<RegisterItemStatusReport> StatusRegisterItems { get; set; }


        public int NumberOfIems()
        {
            return StatusRegisterItems.Count;
        }

        public bool IsDokReport()
        {
            return StatusRegisterItems.FirstOrDefault() is DatasetStatusHistory;
        }

        public bool IsInspireRegistryReport()
        {
            return StatusRegisterItems.FirstOrDefault() is InspireDataserviceStatusReport ||
                   StatusRegisterItems.FirstOrDefault() is InspireDatasetStatusReport;
        }

        public bool IsInspireDatasetReport()
        {
            foreach (var item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport)
                    return true;
            }

            return false;

            //return StatusRegisterItems
            //return StatusRegisterItems.Where(is InspireDatasetStatusReport).FirstOrDefault() is InspireDatasetStatusReport;
        }

        public bool IsGeodatalovDatasetReport()
        {
            return StatusRegisterItems.FirstOrDefault() is GeodatalovDatasetStatusReport;
        }

        public bool IsMareanoDatasetReport()
        {
            return StatusRegisterItems.FirstOrDefault() is MareanoDatasetStatusReport;
        }

        public bool IsInspireDataserviceReport()
        {
            return StatusRegisterItems.FirstOrDefault() is InspireDataserviceStatusReport;
        }

        public bool IsMareanoDataserviceReport()
        {
            return StatusRegisterItems.FirstOrDefault() is MareanoDatasetStatusReport;
        }

        public int NumberOfItemsWithMetadata(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.Metadata == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }


        public int NumberOfInspireDatasetsWithMetadata(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport inspireDatasetStatusReport)
                {
                    if (inspireDatasetStatusReport.MetadataInspireDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfInspireDataServiceWithMetadata(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDataserviceStatusReport inspireDataServiceStatusReport)
                {
                    if (inspireDataServiceStatusReport.MetadataInspireDataService == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithMetadata(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.MetadataGeodatalovDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithFindable(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.FindableMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithAccesible(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.AccesibleMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithInteroperable(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.InteroperableMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithProductSheet(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.ProductSheetMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithPresentationRules(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.PresentationRulesMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithReUsable(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.ReUsableMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithMetadata(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.MetadataMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithProductsheet(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.ProductSheet == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithPresentationRules(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.PresentationRules == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithProductSpecification(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.ProductSpecification == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithProductSpecification(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.ProductSpesificationGeodatalovDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithProductSpecification(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.ProductSpesificationMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithWms(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.Wms == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfInspireDatasetsWithWms(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport inspireDatasetStatusReport)
                {
                    if (inspireDatasetStatusReport.WmsInspireDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithWms(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.WmsGeodatalovDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithWms(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.WmsMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithWfsOrAtom(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport inspireDatasetStatusReport)
                {
                    if (inspireDatasetStatusReport.WfsOrAtomInspireDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithWfs(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.Wfs == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfInspireDatasetsWithWfs(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport inspireDatasetStatusReport)
                {
                    if (inspireDatasetStatusReport.WfsInspireDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithWfs(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.WfsGeodatalovDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithWfs(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.WfsMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithSosiRequirements(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.SosiRequirements == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithSosiRequirements(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.SosiDataGeodatalovDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithSosiRequirements(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.SosiDataMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithGmlRequirements(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.GmlRequirements == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithGmlRequirements(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.GmlDataGeodatalovDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithGmlRequirements(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.GmlDataMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithAtomFeed(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.AtomFeed == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfInspireDatasetsWithAtomFeed(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport inspireDatasetStatusReport)
                {
                    if (inspireDatasetStatusReport.AtomFeedInspireDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithAtomFeed(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.AtomFeedGeodatalovDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsWithAtomFeed(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.AtomFeedMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithDistribution(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is DatasetStatusHistory datasetStatusHistory)
                {
                    if (datasetStatusHistory.Distribution == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfInspireDatasetsWithDistribution(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport inspireDatasetStatusReport)
                {
                    if (inspireDatasetStatusReport.DistributionInspireDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithMetadataService(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport inspireDatasetStatusReport)
                {
                    if (inspireDatasetStatusReport.MetadataServiceInspireDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfInspireDataServicesWithMetadataService(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDataserviceStatusReport inspireDataServiceStatusReport)
                {
                    if (inspireDataServiceStatusReport.MetadataInSearchServiceInspireDataService == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithHarmonizedData(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport inspireDatasetStatusReport)
                {
                    if (inspireDatasetStatusReport.HarmonizedDataInspireDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithSpatialDataService(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport inspireDatasetStatusReport)
                {
                    if (inspireDatasetStatusReport.SpatialDataServiceInspireDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithServiceStatus(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDataserviceStatusReport inspireDataServiceStatusReport)
                {
                    if (inspireDataServiceStatusReport.ServiceStatusInspireDataService == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfItemsWithCommon(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.CommonStatusGeodatalovDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfMareanoDatasetsItemsWithCommon(string status)
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is MareanoDatasetStatusReport mareanoDatasetStatusReport)
                {
                    if (mareanoDatasetStatusReport.CommonStatusMareanoDataset == status)
                    {
                        number++;
                    }
                }
            }
            return number;
        }


        public int NumberOfItemsByType(string statusType, string status)
        {
            if (IsGeodatalovDatasetReport())
            {
                return NumberOfGeodatalovDatasetByType(statusType, status);
            }

            if (IsMareanoDataserviceReport())
            {
                return NumberOfMareanoDatasetByType(statusType, status);
            }

            switch (statusType)
            {
                case "Findable":
                    return NumberOfMareanoDatasetsWithFindable(status);
                case "Accesible":
                    return NumberOfMareanoDatasetsWithAccesible(status);
                case "Interoperable":
                    return NumberOfMareanoDatasetsWithInteroperable(status);
                case "ReUseable":
                    return NumberOfMareanoDatasetsWithReUsable(status);
                case "Metadata":
                    return NumberOfItemsWithMetadata(status);
                case "ProductSheet":
                    return NumberOfItemsWithProductsheet(status);
                case "PresentationRules":
                    return NumberOfItemsWithPresentationRules(status);
                case "ProductSpecification":
                    return NumberOfItemsWithProductSpecification(status);
                case "Wms":
                    return NumberOfItemsWithWms(status);
                case "Wfs":
                    return NumberOfItemsWithWfs(status);
                case "SosiRequirements":
                    return NumberOfItemsWithSosiRequirements(status);
                case "GmlRequirements":
                    return NumberOfItemsWithGmlRequirements(status);
                case "AtomFeed":
                    return NumberOfItemsWithAtomFeed(status);
                case "Distribution":
                    return NumberOfItemsWithDistribution(status);
                case "MetadataInspireDataService":
                    return NumberOfInspireDataServiceWithMetadata(status);
                case "MetadataSearchServiceStatus":
                    return NumberOfInspireDataServicesWithMetadataService(status);
                case "DistributionInspireDataService":
                    return NumberOfItemsWithServiceStatus(status);
                case "Sds":
                    return NumberOfInspireDataServiceWithSds();
                case "NetworkService":
                    return NumberOfInspireDataServiceWithNetworkService();
                case "MetadataInspireDataset":
                    return NumberOfInspireDatasetsWithMetadata(status);
                case "MetadataServiceStatus":
                    return NumberOfItemsWithMetadataService(status);
                case "DistributionInspireDataset":
                    return NumberOfInspireDatasetsWithDistribution(status);
                case "WmsInspireDataset":
                    return NumberOfInspireDatasetsWithWms(status);
                case "WfsInspireDataset":
                    return NumberOfInspireDatasetsWithWfs(status);
                case "WfsOrAtom":
                    return NumberOfItemsWithWfsOrAtom(status);
                case "AtomInspireDataset":
                    return NumberOfInspireDatasetsWithAtomFeed(status);
                case "HarmonizedData":
                    return NumberOfItemsWithHarmonizedData(status);
                case "SpatialDataService":
                    return NumberOfItemsWithSpatialDataService(status);
            }

            return 0;




        }

        private int NumberOfMareanoDatasetByType(string statusType, string status)
        {
            switch (statusType)
            {
                case "Findable":
                    return NumberOfMareanoDatasetsWithFindable(status);
                case "Accesible":
                    return NumberOfMareanoDatasetsWithAccesible(status);
                case "Interoperable":
                    return NumberOfMareanoDatasetsWithInteroperable(status);
                case "ReUseable":
                    return NumberOfMareanoDatasetsWithReUsable(status);
                case "Metadata":
                    return NumberOfMareanoDatasetsWithMetadata(status);
                case "ProductSheet":
                    return NumberOfMareanoDatasetsWithProductSheet(status);
                case "PresentationRules":
                    return NumberOfMareanoDatasetsWithPresentationRules(status);
                case "ProductSpecification":
                    return NumberOfMareanoDatasetsWithProductSpecification(status);
                case "Wms":
                    return NumberOfMareanoDatasetsWithWms(status);
                case "Wfs":
                    return NumberOfMareanoDatasetsWithWfs(status);
                case "SosiRequirements":
                    return NumberOfMareanoDatasetsWithSosiRequirements(status);
                case "GmlRequirements":
                    return NumberOfMareanoDatasetsWithGmlRequirements(status);
                case "AtomFeed":
                    return NumberOfMareanoDatasetsWithAtomFeed(status);
                case "Common":
                    return NumberOfMareanoDatasetsItemsWithCommon(status);
            }

            return 0;
        }

        private int NumberOfGeodatalovDatasetByType(string statusType, string status)
        {
            switch (statusType)
            {
                case "Metadata":
                    return NumberOfGeodatalovDatasetsWithMetadata(status);
                case "ProductSpecification":
                    return NumberOfGeodatalovDatasetsWithProductSpecification(status);
                case "SosiRequirements":
                    return NumberOfGeodatalovDatasetsWithSosiRequirements(status);
                case "GmlRequirements":
                    return NumberOfGeodatalovDatasetsWithGmlRequirements(status);
                case "Wms":
                    return NumberOfGeodatalovDatasetsWithWms(status);
                case "Wfs":
                    return NumberOfGeodatalovDatasetsWithWfs(status);
                case "AtomFeed":
                    return NumberOfGeodatalovDatasetsWithAtomFeed(status);
                case "Common":
                    return NumberOfItemsWithCommon(status);
                case "InspireTheme":
                    return NumberOfGeodatalovDatasetsWithInspireTheme();
                case "Dok":
                    return NumberOfGeodatalovDatasetsWithDok();
                case "NationalDataset":
                    return NumberOfGeodatalovDatasetsWithNationalDataset();
                case "Plan":
                    return NumberOfGeodatalovDatasetsWithPlan();
                case "Geodatalov":
                    return NumberOfGeodatalovDatasetsWithGeodatalov();
            }

            return 0;
        }

        public int NumberOfInspireDatasets()
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport)
                {
                    number++;
                }
            }
            return number;
        }

        public int NumberOfInspireDataServiceWithSds()
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDataserviceStatusReport InspireDataServiceStatusReport)
                {
                    if (InspireDataServiceStatusReport.Sds)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfInspireDataServiceWithNetworkService()
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDataserviceStatusReport InspireDataServiceStatusReport)
                {
                    if (InspireDataServiceStatusReport.NetworkService)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfInspireDataServices()
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDataserviceStatusReport)
                {
                    number++;
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithInspireTheme()
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.InspireTheme)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithDok()
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.Dok)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithNationalDataset()
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.NationalDataset)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithPlan()
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.Plan)
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        public int NumberOfGeodatalovDatasetsWithGeodatalov()
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is GeodatalovDatasetStatusReport geodatalovDatasetStatusReport)
                {
                    if (geodatalovDatasetStatusReport.Geodatalov)
                    {
                        number++;
                    }
                }
            }
            return number;
        }
    }


}