using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireMonitoringViewModel
    {

        // **** Metadata ****

        public int MDi1Value { get; set; }
        public int MDi1Numerator { get; set; }
        public double MDi1Denominator { get; set; }

        public int MDi11Value { get; set; }
        public int MDi11Numerator { get; set; }
        public double MDi11Denominator { get; set; }

        public int MDi12Value { get; set; }
        public int MDi12Numerator { get; set; }
        public double MDi12Denominator { get; set; }

        public int MDi13Value { get; set; }
        public int MDi13Numerator { get; set; }
        public double MDi13Denominator { get; set; }

        public int MDi14Value { get; set; }
        public int MDi14Numerator { get; set; }
        public double MDi14Denominator { get; set; }

        public int MDi2Value { get; set; }
        public int MDi2Numerator { get; set; }
        public double MDi2Denominator { get; set; }

        public int MDi21Value { get; set; }
        public int MDi21Numerator { get; set; }
        public double MDi21Denominator { get; set; }
                      
        public int MDi22Value { get; set; }
        public int MDi22Numerator { get; set; }
        public double MDi22Denominator { get; set; }
                      
        public int MDi23Value { get; set; }
        public int MDi23Numerator { get; set; }
        public double MDi23Denominator { get; set; }
                      
        public int MDi24Value { get; set; }
        public int MDi24Numerator { get; set; }
        public double MDi24Denominator { get; set; }

        // **** Data sets ****

        public int DSi1Value { get; set; }
        public double DSi1Numerator { get; set; }
        public double DSi1Denominator { get; set; }

        public int DSi11Value { get; set; }
        public double DSi11Numerator { get; set; }
        public double DSi11Denominator { get; set; }

        public int DSi12Value { get; set; }
        public double DSi12Numerator { get; set; }
        public double DSi12Denominator { get; set; }

        public int DSi13Value { get; set; }
        public double DSi13Numerator { get; set; }
        public double DSi13Denominator { get; set; }

        public int DSi2Value { get; set; }
        public int DSi2Numerator { get; set; }
        public double DSi2Denominator { get; set; }
                      
        public int DSi21Value { get; set; }
        public int DSi21Numerator { get; set; }
        public double DSi21Denominator { get; set; }
                      
        public int DSi22Value { get; set; }
        public int DSi22Numerator { get; set; }
        public double DSi22Denominator { get; set; }
                      
        public int DSi23Value { get; set; }
        public int DSi23Numerator { get; set; }
        public double DSi23Denominator { get; set; }

        // **** Services ****

        public int NSi1Value { get; set; }
        public int NSi1Numerator { get; set; }
        public double NSi1Denominator { get; set; }

        public int NSi11Value { get; set; }
        public int NSi11Numerator { get; set; }
        public double NSi11Denominator { get; set; }

        public int NSi12Value { get; set; }
        public int NSi12Numerator { get; set; }
        public double NSi12Denominator { get; set; }

        public int NSi2Value { get; set; }
        public int NSi2Numerator { get; set; }
        public double NSi2Denominator { get; set; }
                      
        public int NSi21Value { get; set; }
        public int NSi21Numerator { get; set; }
        public double NSi21Denominator { get; set; }
                      
        public int NSi22Value { get; set; }
        public int NSi22Numerator { get; set; }
        public double NSi22Denominator { get; set; }


        public int NSi3Value { get; set; }
        public int NSi3Numerator { get; set; }
        public double NSi3Denominator { get; set; }
                      
        public int NSi31Value { get; set; }
        public int NSi31Numerator { get; set; }
        public double NSi31Denominator { get; set; }
                      
        public int NSi32Value { get; set; }
        public int NSi32Numerator { get; set; }
        public double NSi32Denominator { get; set; }

        public int NSi33Value { get; set; }
        public int NSi33Numerator { get; set; }
        public double NSi33Denominator { get; set; }

        public int NSi34Value { get; set; }
        public int NSi34Numerator { get; set; }
        public double NSi34Denominator { get; set; }

        public int NSi35Value { get; set; }
        public int NSi35Numerator { get; set; }
        public double NSi35Denominator { get; set; }

        public int NSi4Value { get; set; }
        public int NSi4Numerator { get; set; }
        public double NSi4Denominator { get; set; }
                      
        public int NSi41Value { get; set; }
        public int NSi41Numerator { get; set; }
        public double NSi41Denominator { get; set; }
                      
        public int NSi42Value { get; set; }
        public int NSi42Numerator { get; set; }
        public double NSi42Denominator { get; set; }
                      
        public int NSi43Value { get; set; }
        public int NSi43Numerator { get; set; }
        public double NSi43Denominator { get; set; }
                      
        public int NSi44Value { get; set; }
        public int NSi44Numerator { get; set; }
        public double NSi44Denominator { get; set; }
                      
        public int NSi45Value { get; set; }
        public int NSi45Numerator { get; set; }
        public double NSi45Denominator { get; set; }

        public InspireMonitoringViewModel(InspireMonitoring inspireMonitoring)
        {
            MDi1Value = 0;
            MDi1Numerator = inspireMonitoring.NumberOfDatasetsWithMetadata;
            MDi1Denominator = inspireMonitoring.NumberOfDatasetsByAnnex();

            MDi2Value = 0;
            MDi2Numerator = inspireMonitoring.NumberOfServicesWhereMetadataStatusIsgood + inspireMonitoring.NumberOfDatasetsByAnnexWhereMetadataStatusIsgood();
            MDi2Denominator = inspireMonitoring.NumberOfServicesByServiceType() + inspireMonitoring.NumberOfDatasetsByAnnex();

            DSi1Value = 0;
            DSi1Numerator = inspireMonitoring.AccumulatedCurrentAreaByAnnex();
            DSi1Denominator = inspireMonitoring.AccumulatedRelevantAreaByAnnex();

            DSi2Value = 0;
            DSi2Numerator = inspireMonitoring.NumberOfDatasetsWithHarmonizedDataAndConformedMetadata();
            DSi2Denominator = inspireMonitoring.NumberOfDatasetsByAnnex();

            NSi1Value = 0;
            NSi1Numerator = inspireMonitoring.NumberOfDatasetsRegisteredInADiscoveryService + inspireMonitoring.NumberOfServicesRegisteredInADiscoveryService;
            NSi1Denominator = inspireMonitoring.NumberOfDatasetsByAnnex() + inspireMonitoring.NumberOfServicesByServiceType();

            NSi2Value = 0;
            NSi2Numerator = inspireMonitoring.NumberOfDatasetsAvailableThroughViewANDDownloadService;
            NSi2Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDownload + inspireMonitoring.NumberOfServicesByServiceTypeView;

            NSi3Value = 0;
            NSi3Numerator = inspireMonitoring.NumberOfCallsByServiceType();
            NSi3Denominator = inspireMonitoring.NumberOfServicesByServiceType();

            NSi4Value = 0;
            NSi4Numerator = inspireMonitoring.NumberOfServicesByServiceTypeWhereConformityIsTrue();
            NSi4Denominator = inspireMonitoring.NumberOfServicesByServiceType();

            NSi11Value = 0;
            NSi11Numerator = inspireMonitoring.NumberOfDatasetsRegisteredInADiscoveryService;
            NSi11Denominator = inspireMonitoring.NumberOfDatasetsByAnnex();

            MDi11Value = 0;
            MDi11Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata;
            MDi11Denominator = inspireMonitoring.NumberOfDatasetsByAnnexI;

            MDi12Value = 0;
            MDi12Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata;
            MDi12Denominator = inspireMonitoring.NumberOfDatasetsByAnnexII;

            MDi13Value = 0;
            MDi13Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIIWithMetadata;
            MDi13Denominator = inspireMonitoring.NumberOfDatasetsByAnnexIII;

            MDi21Value = 0;
            MDi21Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood;
            MDi21Denominator = inspireMonitoring.NumberOfDatasetsByAnnexI;

            MDi22Value = 0;
            MDi22Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood;
            MDi22Denominator = inspireMonitoring.NumberOfDatasetsByAnnexII;

            MDi23Value = 0;
            MDi23Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood;
            MDi23Denominator = inspireMonitoring.NumberOfDatasetsByAnnexIII;

            DSi11Value = 0;
            DSi11Numerator = inspireMonitoring.AccumulatedCurrentAreaByAnnexI;
            DSi11Denominator = inspireMonitoring.AccumulatedRelevantAreaByAnnexI;

            DSi12Value = 0;
            DSi12Numerator = inspireMonitoring.AccumulatedCurrentAreaByAnnexII;
            DSi12Denominator = inspireMonitoring.AccumulatedRelevantAreaByAnnexII;

            DSi13Value = 0;
            DSi13Numerator = inspireMonitoring.AccumulatedCurrentAreaByAnnexIII;
            DSi13Denominator = inspireMonitoring.AccumulatedRelevantAreaByAnnexIII;

            DSi21Value = 0;
            DSi21Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata;
            DSi21Denominator = inspireMonitoring.NumberOfDatasetsByAnnexI;
               
            DSi22Value = 0;
            DSi22Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata;
            DSi22Denominator = inspireMonitoring.NumberOfDatasetsByAnnexII;
               
            DSi23Value = 0;
            DSi23Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata;
            DSi23Denominator = inspireMonitoring.NumberOfDatasetsByAnnexIII;

            MDi14Value = 0;
            MDi14Numerator = inspireMonitoring.NumberOfServicesWithMetadata;
            MDi14Denominator = inspireMonitoring.NumberOfServicesByServiceType();

            MDi24Value = 0;
            MDi24Numerator = inspireMonitoring.NumberOfServicesWhereMetadataStatusIsgood;
            MDi24Denominator = inspireMonitoring.NumberOfServicesByServiceType();

            NSi12Value = 0;
            NSi12Numerator = inspireMonitoring.NumberOfServicesRegisteredInADiscoveryService;
            NSi12Denominator = inspireMonitoring.NumberOfServicesByServiceType();

            NSi3Value = 0;
            NSi3Numerator = inspireMonitoring.NumberOfCallsByServiceType();
            NSi3Denominator = inspireMonitoring.NumberOfServicesByServiceType();

            NSi31Value = 0;
            NSi31Numerator = inspireMonitoring.NumberOfCallsByServiceTypeDiscovery;
            NSi31Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDiscovery;

            NSi32Value = 0;
            NSi32Numerator = inspireMonitoring.NumberOfCallsByServiceTypeView;
            NSi32Denominator = inspireMonitoring.NumberOfServicesByServiceTypeView;

            NSi33Value = 0;
            NSi33Numerator = inspireMonitoring.NumberOfCallsByServiceTypeDownload;
            NSi33Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDownload;

            NSi34Value = 0;
            NSi34Numerator = inspireMonitoring.NumberOfCallsByServiceTypeTransformation;
            NSi34Denominator = inspireMonitoring.NumberOfServicesByServiceTypeTransformation;

            NSi35Value = 0;
            NSi35Numerator = inspireMonitoring.NumberOfCallsByServiceTypeInvoke;
            NSi35Denominator = inspireMonitoring.NumberOfServicesByServiceTypeInvoke;

            NSi4Value = 0;
            NSi4Numerator = inspireMonitoring.NumberOfServicesByServiceTypeWhereConformityIsTrue();
            NSi4Denominator = inspireMonitoring.NumberOfServicesByServiceType();

            NSi41Value = 0;
            NSi41Numerator = inspireMonitoring.NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue;
            NSi41Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDiscovery;
               
            NSi42Value = 0;
            NSi42Numerator = inspireMonitoring.NumberOfServicesByServiceTypeViewWhereConformityIsTrue;
            NSi42Denominator = inspireMonitoring.NumberOfServicesByServiceTypeView;
               
            NSi43Value = 0;
            NSi43Numerator = inspireMonitoring.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue;
            NSi43Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDownload;
               
            NSi44Value = 0;
            NSi44Numerator = inspireMonitoring.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue;
            NSi44Denominator = inspireMonitoring.NumberOfServicesByServiceTypeTransformation;
               
            NSi45Value = 0;
            NSi45Numerator = inspireMonitoring.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue;
            NSi45Denominator = inspireMonitoring.NumberOfServicesByServiceTypeInvoke;

            NSi21Value = 0;
            NSi21Numerator = inspireMonitoring.NumberOfDatasetsAvailableThroughViewService;
            NSi21Denominator = inspireMonitoring.NumberOfServicesByServiceTypeView;

            NSi22Value = 0;
            NSi22Numerator = inspireMonitoring.NumberOfDatasetsAvailableThroughDownloadService;
            NSi22Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDownload;


        }

    }
}