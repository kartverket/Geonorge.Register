using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kartverket.Register.Models
{
    public interface IInspireMonitoring
    {
        DateTime Date { get; set; }

        int NumberOfDatasetsByAnnexI { get; set; }
        int NumberOfDatasetsByAnnexII { get; set; }
        int NumberOfDatasetsByAnnexIII { get; set; }

        int NumberOfDatasetsByAnnexIWithMetadata { get; set; }
        int NumberOfDatasetsByAnnexIIWithMetadata { get; set; }
        int NumberOfDatasetsByAnnexIIIWithMetadata { get; set; }

        int NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood { get; set; }
        int NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood { get; set; }
        int NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood { get; set; }
        int NumberOfServicesWhereMetadataStatusIsgood { get; set; }

        int NumberOfDatasetsWithMetadata { get; set; }
        int NumberOfServicesWithMetadata { get; set; }

        int NumberOfDatasetsRegisteredInADiscoveryService { get; set; }
        int NumberOfServicesRegisteredInADiscoveryService { get; set; }

        int NumberOfServicesByServiceTypeDownload { get; set; }
        int NumberOfServicesByServiceTypeView { get; set; }
        int NumberOfServicesByServiceTypeDiscovery { get; set; }
        int NumberOfServicesByServiceTypeInvoke { get; set; }
        int NumberOfServicesByServiceTypeTransformation { get; set; }
        int NumberOfSdS { get; set; }

        int NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue { get; set; }
        int NumberOfServicesByServiceTypeViewWhereConformityIsTrue { get; set; }
        int NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue { get; set; }
        int NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue { get; set; }
        int NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue { get; set; }

        int NumberOfCallsByServiceTypeDiscovery { get; set; }

        int NumberOfCallsByServiceTypeView { get; set; }
        int NumberOfCallsByServiceTypeDownload { get; set; }
        int NumberOfCallsByServiceTypeTransformation { get; set; }
        int NumberOfCallsByServiceTypeInvoke { get; set; }

        int NumberOfDatasetsAvailableThroughViewANDDownloadService { get; set; }
        int NumberOfDatasetsAvailableThroughDownloadService { get; set; }
        int NumberOfDatasetsAvailableThroughViewService { get; set; }

        int NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata { get; set; }
        int NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata { get; set; }
        int NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata { get; set; }

        double AccumulatedCurrentAreaByAnnexI { get; set; }
        double AccumulatedCurrentAreaByAnnexII { get; set; }
        double AccumulatedCurrentAreaByAnnexIII { get; set; }

        double AccumulatedRelevantAreaByAnnexI { get; set; }
        double AccumulatedRelevantAreaByAnnexII { get; set; }
        double AccumulatedRelevantAreaByAnnexIII { get; set; }


        int NumberOfCallsByServiceType();
        int NumberOfServicesByServiceType();
        int NumberOfDatasetsWithHarmonizedDataAndConformedMetadata();
        int NumberOfServicesByServiceTypeWhereConformityIsTrue();
        int NumberOfDatasetsByAnnexWhereMetadataStatusIsgood();
        double AccumulatedCurrentAreaByAnnex();
        double AccumulatedRelevantAreaByAnnex();
        double AverageNumberOfCallsByServiceTypeDownload();
        double AverageNumberOfCallsByServiceTypeDiscovery();
        double AverageNumberOfCallsByServiceTypeView();
        double AverageNumberOfCallsByServiceTypeTransformation();
        double AverageNumberOfCallsByServiceTypeInvoke();
        double AverageNumberOfCallsByServiceType();
        double ProportionOfDatasetsRegisteredInADiscoveryService();
        double ProportionOfServicesRegisteredInADiscoveryService();
        double ProportionOfDatasetsWithMetadataByAnnexI();
        double ProportionOfDatasetsWithMetadataByAnnexII();
        double ProportionOfDatasetsWithMetadataByAnnexIII();
        double ProportionOfServicesWithMetadata();
        double ProportionOfDatasetsAndServicesWithMetadata();
        int NumberOfDatasetsByAnnex();
        double ProportionOfDatasetWithHarmonizedDataAndConformedMetadata();
        double ProportionOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata();
        double ProportionOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata();
        double ProportionOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata();
        double ProportionOfDatasetsAvailableThroughViewService();
        double ProportionOfDatasetsAvailableThroughDownloadService();
        double ProportionOfDatasetsAvailableThroughViewAndDownloadService();
        double ProportionOfServicesWhereConformityIsTrue();
        double ProportionOfServicesByServiceTypeDownloadWhereConformityIsTrue();
        double ProportionOfServicesByServiceTypeDiscoveryWhereConformityIsTrue();
        double ProportionOfServicesByServiceTypeViewWhereConformityIsTrue();
        double ProportionOfServicesByServiceTypeTransformationWhereConformityIsTrue();
        double ProportionOfServicesByServiceTypeInvokeWhereConformityIsTrue();
        double ProportionOfServicesAndDatasetsRegisteredInADiscoveryService();
        double ProportionOfArealByAnnexI();
        double ProportionOfArealByAnnexII();
        double ProportionOfArealByAnnexIII();
        double ProportionOfArealByAnnex();
        double ProportionOfDatasetByAnnexIWithMetadatastatusGood();
        double ProportionOfDatasetByAnnexIIWithMetadatastatusGood();
        double ProportionOfDatasetByAnnexIIIWithMetadatastatusGood();
        double ProportionOfServicesWithMetadatastatusGood();
        double ProportionOfServicesAndDatasetsWithMetadatastatusGood();
    }

}
