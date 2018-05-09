using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireMonitoringViewModel
    {

        // **** Metadata ****
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public double MDi1Value { get; set; }
        public int MDi1Numerator { get; set; }
        public double MDi1Denominator { get; set; }

        public double MDi11Value { get; set; }
        public int MDi11Numerator { get; set; }
        public double MDi11Denominator { get; set; }

        public double MDi12Value { get; set; }
        public int MDi12Numerator { get; set; }
        public double MDi12Denominator { get; set; }

        public double MDi13Value { get; set; }
        public int MDi13Numerator { get; set; }
        public double MDi13Denominator { get; set; }

        public double MDi14Value { get; set; }
        public int MDi14Numerator { get; set; }
        public double MDi14Denominator { get; set; }

        public double MDi2Value { get; set; }
        public int MDi2Numerator { get; set; }
        public double MDi2Denominator { get; set; }

        public double MDi21Value { get; set; }
        public int MDi21Numerator { get; set; }
        public double MDi21Denominator { get; set; }

        public double MDi22Value { get; set; }
        public int MDi22Numerator { get; set; }
        public double MDi22Denominator { get; set; }

        public double MDi23Value { get; set; }
        public int MDi23Numerator { get; set; }
        public double MDi23Denominator { get; set; }

        public double MDi24Value { get; set; }
        public int MDi24Numerator { get; set; }
        public double MDi24Denominator { get; set; }

        // **** Data sets ****

        public double DSi1Value { get; set; }
        public double DSi1Numerator { get; set; }
        public double DSi1Denominator { get; set; }

        public double DSi11Value { get; set; }
        public double DSi11Numerator { get; set; }
        public double DSi11Denominator { get; set; }

        public double DSi12Value { get; set; }
        public double DSi12Numerator { get; set; }
        public double DSi12Denominator { get; set; }

        public double DSi13Value { get; set; }
        public double DSi13Numerator { get; set; }
        public double DSi13Denominator { get; set; }

        public double DSi2Value { get; set; }
        public int DSi2Numerator { get; set; }
        public double DSi2Denominator { get; set; }

        public double DSi21Value { get; set; }
        public int DSi21Numerator { get; set; }
        public double DSi21Denominator { get; set; }

        public double DSi22Value { get; set; }
        public int DSi22Numerator { get; set; }
        public double DSi22Denominator { get; set; }

        public double DSi23Value { get; set; }
        public int DSi23Numerator { get; set; }
        public double DSi23Denominator { get; set; }

        // **** Services ****

        public double NSi1Value { get; set; }
        public int NSi1Numerator { get; set; }
        public double NSi1Denominator { get; set; }

        public double NSi11Value { get; set; }
        public int NSi11Numerator { get; set; }
        public double NSi11Denominator { get; set; }

        public double NSi12Value { get; set; }
        public int NSi12Numerator { get; set; }
        public double NSi12Denominator { get; set; }

        public double NSi2Value { get; set; }
        public int NSi2Numerator { get; set; }
        public double NSi2Denominator { get; set; }

        public double NSi21Value { get; set; }
        public int NSi21Numerator { get; set; }
        public double NSi21Denominator { get; set; }

        public double NSi22Value { get; set; }
        public int NSi22Numerator { get; set; }
        public double NSi22Denominator { get; set; }


        public double NSi3Value { get; set; }
        public int NSi3Numerator { get; set; }
        public double NSi3Denominator { get; set; }

        public double NSi31Value { get; set; }
        public int NSi31Numerator { get; set; }
        public double NSi31Denominator { get; set; }

        public double NSi32Value { get; set; }
        public int NSi32Numerator { get; set; }
        public double NSi32Denominator { get; set; }

        public double NSi33Value { get; set; }
        public int NSi33Numerator { get; set; }
        public double NSi33Denominator { get; set; }

        public double NSi34Value { get; set; }
        public int NSi34Numerator { get; set; }
        public double NSi34Denominator { get; set; }

        public double NSi35Value { get; set; }
        public int NSi35Numerator { get; set; }
        public double NSi35Denominator { get; set; }

        public double NSi4Value { get; set; }
        public int NSi4Numerator { get; set; }
        public double NSi4Denominator { get; set; }

        public double NSi41Value { get; set; }
        public int NSi41Numerator { get; set; }
        public double NSi41Denominator { get; set; }

        public double NSi42Value { get; set; }
        public int NSi42Numerator { get; set; }
        public double NSi42Denominator { get; set; }

        public double NSi43Value { get; set; }
        public int NSi43Numerator { get; set; }
        public double NSi43Denominator { get; set; }

        public double NSi44Value { get; set; }
        public int NSi44Numerator { get; set; }
        public double NSi44Denominator { get; set; }

        public double NSi45Value { get; set; }
        public int NSi45Numerator { get; set; }
        public double NSi45Denominator { get; set; }

        public InspireMonitoringViewModel(IInspireMonitoring inspireMonitoring)
        {
            if (inspireMonitoring != null)
            {
                Date = inspireMonitoring.Date;

                MDi1Value = Percent(inspireMonitoring.ProportionOfDatasetsAndServicesWithMetadata());
                MDi1Numerator = inspireMonitoring.NumberOfDatasetsWithMetadata + inspireMonitoring.NumberOfServicesWithMetadata;
                MDi1Denominator = inspireMonitoring.NumberOfDatasetsByAnnex() + inspireMonitoring.NumberOfServicesByServiceType();

                MDi2Value = Percent(inspireMonitoring.ProportionOfDatasetByAnnexIWithMetadatastatusGood());
                MDi2Numerator = inspireMonitoring.NumberOfServicesWhereMetadataStatusIsgood +
                                inspireMonitoring.NumberOfDatasetsByAnnexWhereMetadataStatusIsgood();
                MDi2Denominator = inspireMonitoring.NumberOfServicesByServiceType() +
                                  inspireMonitoring.NumberOfDatasetsByAnnex();

                DSi1Value = Percent(inspireMonitoring.ProportionOfArealByAnnex());
                DSi1Numerator = inspireMonitoring.AccumulatedCurrentAreaByAnnex();
                DSi1Denominator = inspireMonitoring.AccumulatedRelevantAreaByAnnex();

                DSi2Value = Percent(inspireMonitoring.ProportionOfDatasetWithHarmonizedDataAndConformedMetadata());
                DSi2Numerator = inspireMonitoring.NumberOfDatasetsWithHarmonizedDataAndConformedMetadata();
                DSi2Denominator = inspireMonitoring.NumberOfDatasetsByAnnex();

                NSi1Value = Percent(inspireMonitoring.ProportionOfServicesAndDatasetsRegisteredInADiscoveryService());
                NSi1Numerator = inspireMonitoring.NumberOfDatasetsRegisteredInADiscoveryService +
                                inspireMonitoring.NumberOfServicesRegisteredInADiscoveryService;
                NSi1Denominator = inspireMonitoring.NumberOfDatasetsByAnnex() +
                                  inspireMonitoring.NumberOfServicesByServiceType();

                NSi2Value = Percent(inspireMonitoring.ProportionOfDatasetsAvailableThroughViewAndDownloadService());
                NSi2Numerator = inspireMonitoring.NumberOfDatasetsAvailableThroughViewANDDownloadService;
                NSi2Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDownload +
                                  inspireMonitoring.NumberOfServicesByServiceTypeView;

                NSi3Value = inspireMonitoring.AverageNumberOfCallsByServiceType();
                NSi3Numerator = inspireMonitoring.NumberOfCallsByServiceType();
                NSi3Denominator = inspireMonitoring.NumberOfServicesByServiceType();

                NSi4Value = Percent(inspireMonitoring.ProportionOfServicesWhereConformityIsTrue());
                NSi4Numerator = inspireMonitoring.NumberOfServicesByServiceTypeWhereConformityIsTrue();
                NSi4Denominator = inspireMonitoring.NumberOfServicesByServiceType();

                NSi11Value = Percent(inspireMonitoring.ProportionOfDatasetsRegisteredInADiscoveryService());
                NSi11Numerator = inspireMonitoring.NumberOfDatasetsRegisteredInADiscoveryService;
                NSi11Denominator = inspireMonitoring.NumberOfDatasetsByAnnex();

                MDi11Value = Percent(inspireMonitoring.ProportionOfDatasetsWithMetadataByAnnexI());
                MDi11Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata;
                MDi11Denominator = inspireMonitoring.NumberOfDatasetsByAnnexI;

                MDi12Value = Percent(inspireMonitoring.ProportionOfDatasetsWithMetadataByAnnexII());
                MDi12Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata;                                   
                MDi12Denominator = inspireMonitoring.NumberOfDatasetsByAnnexII;

                MDi13Value = Percent(inspireMonitoring.ProportionOfDatasetsWithMetadataByAnnexIII());
                MDi13Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIIWithMetadata;
                MDi13Denominator = inspireMonitoring.NumberOfDatasetsByAnnexIII;

                MDi21Value = Percent(inspireMonitoring.ProportionOfDatasetByAnnexIWithMetadatastatusGood());
                MDi21Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood;
                MDi21Denominator = inspireMonitoring.NumberOfDatasetsByAnnexI;

                MDi22Value = Percent(inspireMonitoring.ProportionOfDatasetByAnnexIIWithMetadatastatusGood());
                MDi22Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood;
                MDi22Denominator = inspireMonitoring.NumberOfDatasetsByAnnexII;

                MDi23Value = Percent(inspireMonitoring.ProportionOfDatasetByAnnexIIIWithMetadatastatusGood());
                MDi23Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood;
                MDi23Denominator = inspireMonitoring.NumberOfDatasetsByAnnexIII;

                DSi11Value = Percent(inspireMonitoring.ProportionOfArealByAnnexI());
                DSi11Numerator = inspireMonitoring.AccumulatedCurrentAreaByAnnexI;
                DSi11Denominator = inspireMonitoring.AccumulatedRelevantAreaByAnnexI;

                DSi12Value = Percent(inspireMonitoring.ProportionOfArealByAnnexII());
                DSi12Numerator = inspireMonitoring.AccumulatedCurrentAreaByAnnexII;
                DSi12Denominator = inspireMonitoring.AccumulatedRelevantAreaByAnnexII;

                DSi13Value = Percent(inspireMonitoring.ProportionOfArealByAnnexIII());
                DSi13Numerator = inspireMonitoring.AccumulatedCurrentAreaByAnnexIII;
                DSi13Denominator = inspireMonitoring.AccumulatedRelevantAreaByAnnexIII;

                DSi21Value = Percent(inspireMonitoring.ProportionOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata());
                DSi21Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata;
                DSi21Denominator = inspireMonitoring.NumberOfDatasetsByAnnexI;

                DSi22Value = Percent(inspireMonitoring.ProportionOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata());
                DSi22Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata;
                DSi22Denominator = inspireMonitoring.NumberOfDatasetsByAnnexII;

                DSi23Value = Percent(inspireMonitoring.ProportionOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata());
                DSi23Numerator = inspireMonitoring.NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata;
                DSi23Denominator = inspireMonitoring.NumberOfDatasetsByAnnexIII;

                MDi14Value = Percent(inspireMonitoring.ProportionOfServicesWithMetadata());
                MDi14Numerator = inspireMonitoring.NumberOfServicesWithMetadata;
                MDi14Denominator = inspireMonitoring.NumberOfServicesByServiceType();

                MDi24Value = Percent(inspireMonitoring.ProportionOfServicesWithMetadatastatusGood());
                MDi24Numerator = inspireMonitoring.NumberOfServicesWhereMetadataStatusIsgood;
                MDi24Denominator = inspireMonitoring.NumberOfServicesByServiceType();

                NSi12Value = Percent(inspireMonitoring.ProportionOfServicesRegisteredInADiscoveryService());
                NSi12Numerator = inspireMonitoring.NumberOfServicesRegisteredInADiscoveryService;
                NSi12Denominator = inspireMonitoring.NumberOfServicesByServiceType();

                NSi31Value = Percent(inspireMonitoring.AverageNumberOfCallsByServiceTypeDiscovery());
                NSi31Numerator = inspireMonitoring.NumberOfCallsByServiceTypeDiscovery;
                NSi31Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDiscovery;

                NSi32Value = Percent(inspireMonitoring.AverageNumberOfCallsByServiceTypeView());
                NSi32Numerator = inspireMonitoring.NumberOfCallsByServiceTypeView;
                NSi32Denominator = inspireMonitoring.NumberOfServicesByServiceTypeView;

                NSi33Value = Percent(inspireMonitoring.AverageNumberOfCallsByServiceTypeDownload());
                NSi33Numerator = inspireMonitoring.NumberOfCallsByServiceTypeDownload;
                NSi33Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDownload;

                NSi34Value = Percent(inspireMonitoring.AverageNumberOfCallsByServiceTypeTransformation());
                NSi34Numerator = inspireMonitoring.NumberOfCallsByServiceTypeTransformation;
                NSi34Denominator = inspireMonitoring.NumberOfServicesByServiceTypeTransformation;

                NSi35Value = Percent(inspireMonitoring.AverageNumberOfCallsByServiceTypeInvoke());
                NSi35Numerator = inspireMonitoring.NumberOfCallsByServiceTypeInvoke;
                NSi35Denominator = inspireMonitoring.NumberOfServicesByServiceTypeInvoke;

                NSi41Value = Percent(inspireMonitoring.ProportionOfServicesByServiceTypeDiscoveryWhereConformityIsTrue());
                NSi41Numerator = inspireMonitoring.NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue;
                NSi41Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDiscovery;

                NSi42Value = Percent(inspireMonitoring.ProportionOfServicesByServiceTypeViewWhereConformityIsTrue());
                NSi42Numerator = inspireMonitoring.NumberOfServicesByServiceTypeViewWhereConformityIsTrue;
                NSi42Denominator = inspireMonitoring.NumberOfServicesByServiceTypeView;

                NSi43Value = Percent(inspireMonitoring.ProportionOfServicesByServiceTypeDownloadWhereConformityIsTrue());
                NSi43Numerator = inspireMonitoring.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue;
                NSi43Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDownload;

                NSi44Value = Percent(inspireMonitoring.ProportionOfServicesByServiceTypeTransformationWhereConformityIsTrue());
                NSi44Numerator = inspireMonitoring.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue;
                NSi44Denominator = inspireMonitoring.NumberOfServicesByServiceTypeTransformation;

                NSi45Value = Percent(inspireMonitoring.ProportionOfServicesByServiceTypeInvokeWhereConformityIsTrue());
                NSi45Numerator = inspireMonitoring.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue;
                NSi45Denominator = inspireMonitoring.NumberOfServicesByServiceTypeInvoke;

                NSi21Value = Percent(inspireMonitoring.ProportionOfDatasetsAvailableThroughViewService());
                NSi21Numerator = inspireMonitoring.NumberOfDatasetsAvailableThroughViewService;
                NSi21Denominator = inspireMonitoring.NumberOfServicesByServiceTypeView;

                NSi22Value = Percent(inspireMonitoring.ProportionOfDatasetsAvailableThroughDownloadService());
                NSi22Numerator = inspireMonitoring.NumberOfDatasetsAvailableThroughDownloadService;
                NSi22Denominator = inspireMonitoring.NumberOfServicesByServiceTypeDownload;
            }

        }

        public double Percent(double x)
        {
            return Math.Round(x * 100, 2);
        }
    }
}