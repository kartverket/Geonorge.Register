using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class StatusReport
    {
        public StatusReport()
        {
            Id = Guid.NewGuid();
            Date = DateTime.Now;
            StatusRegisterItems = new List<RegisterItemStatusReport>();
        }


        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
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

        public bool IsInspireDataserviceReport()
        {
            return StatusRegisterItems.FirstOrDefault() is InspireDataserviceStatusReport;
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


        public int NumberOfItemsByType(string statusType, string status)
        {
            if (statusType == "Metadata")
            {
                return NumberOfItemsWithMetadata(status);
            }
            else if (statusType == "ProductSheet")
            {
                return NumberOfItemsWithProductsheet(status);
            }
            else if (statusType == "PresentationRules")
            {
                return NumberOfItemsWithPresentationRules(status);
            }
            else if (statusType == "ProductSpecification")
            {
                return NumberOfItemsWithProductSpecification(status);
            }
            else if (statusType == "Wms")
            {
                return NumberOfItemsWithWms(status);
            }
            else if (statusType == "Wfs")
            {
                return NumberOfItemsWithWfs(status);
            }
            else if (statusType == "SosiRequirements")
            {
                return NumberOfItemsWithSosiRequirements(status);
            }
            else if (statusType == "GmlRequirements")
            {
                return NumberOfItemsWithGmlRequirements(status);
            }
            else if (statusType == "AtomFeed")
            {
                return NumberOfItemsWithAtomFeed(status);
            }
            else if (statusType == "Distribution")
            {
                return NumberOfItemsWithDistribution(status);
            }


            return 0;
        }

        public int NumberOfInspireDatasets()
        {
            int number = 0;
            foreach (RegisterItemStatusReport item in StatusRegisterItems)
            {
                if (item is InspireDatasetStatusReport inspireDatasetStatusReport)
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
    }


}