using System;
using System.Collections.Generic;
using FluentAssertions;
using Kartverket.Register.Models;
using Xunit;

namespace Kartverket.Register.Tests.TestModels
{
    public class InspireMonitoringTest
    {
        private ICollection<RegisterItemV2> _inspireItems = new List<RegisterItemV2>();
        private ICollection<InspireDataset> _inspireDatasets = new List<InspireDataset>();
        private ICollection<InspireDataService> _inspireDataServices = new List<InspireDataService>();

        private InspireDataset _inspireDataset;
        private InspireDataService _inspireDataService;


        public InspireMonitoringTest() {
            _inspireDataset = new InspireDataset();
            _inspireDataset.InspireThemes = new List<CodelistValue>();

            _inspireDataService = new InspireDataService();
        }



        [Fact]
        public void CountDatasetWhenInspireThemeIsOfTypeAnnexI()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireItems.Add(_inspireDataset);
            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexI;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountDatasetWhenInspireThemeIsNotOfTypeAnnexI()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireItems.Add(_inspireDataset);
            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexI;

            result.Should().Be(0);
        }

        [Fact]
        public void CountDatasetWhenInspireThemeIsOfTypeAnnexII()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireItems.Add(_inspireDataset);
            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexII;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountDatasetWhenInspireThemeIsNotOfTypeAnnexII()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireItems.Add(_inspireDataset);
            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexII;

            result.Should().Be(0);
        }

        [Fact]
        public void CountDatasetWhenInspireThemeIsOfTypeAnnexIII()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireItems.Add(_inspireDataset);
            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIII;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountDatasetsWhenInspireThemeIsNotOfTypeAnnexIII()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireItems.Add(_inspireDataset);
            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIII;

            result.Should().Be(0);
        }



        [Fact]
        public void CountDatasetWhenInspireThemeIsOfTypeAnnexIAndMetadataIsSet()
        {
            var inspireDataset = new InspireDataset();
            inspireDataset.InspireThemes = new List<CodelistValue>();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficient", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata;

            result.Should().Be(2);
        }

        [Fact]
        public void NotCountDatasetWhenInspireThemeIsAnnexIAndMetadataIsNotSet()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata;

            result.Should().Be(0);
        }


        [Fact]
        public void CountDatasetWhenInspireThemesIsOfTypeAnnexIIAndMetadataIsSet()
        {
            InspireDataset inspireDataset = CreateInspireDataset(); ;
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficient", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata;

            result.Should().Be(2);
        }

        [Fact]
        public void NotCountDatasetsWhenInspireThemeIsOfTypeAnnexIIAndMetadataIsNotSet()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata;

            result.Should().Be(0);
        }



        [Fact]
        public void CountDatasetsWhenInspireThemeIsOfTypeAnnexIIIAndMetadataIsSet()
        {
            InspireDataset inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficient", null, true);
            _inspireItems.Add(_inspireDataset);


            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIIWithMetadata;

            result.Should().Be(2);
        }

        [Fact]
        public void NotCountDatasetsWhenInspireThemeIsOfTypeAnnexIIIAndMetadataIsNotSet()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIIWithMetadata;

            result.Should().Be(0);
        }


        [Fact]
        public void CountDatasetsWhenInspireThemeIsOfTypeAnnexIAndMetadataIsGood()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);


            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountDatasetsWhenInspireThemeIsOfTypeAnnexIAndMetadataOtherThenGood()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficient", null, true);
            _inspireItems.Add(_inspireDataset);


            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood;

            result.Should().Be(0);
        }

        [Fact]
        public void CountDatasetsWhenInspireThemeIsOfTypeAnnexIIAndMetadataIsGood()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);


            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountDatasetsWhenInspireThemeIsOfTypeAnnexIIAndMetadataIsOtherThenGood()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(_inspireDataset);


            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood;

            result.Should().Be(0);
        }

        [Fact]
        public void CountDatasetsWhenInspireThemeIsOfTypeAnnexIIIAndMetadataIsGood()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);


            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountDatasetsWhenInspireThemeIsOfTypeAnnexIIIAndMetadataIsOtherThenGood()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficient", null, true);
            _inspireItems.Add(_inspireDataset);


            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood;

            result.Should().Be(0);
        }


        [Fact]
        public void CountServiceWhenMetadataIsGood()
        {
            _inspireDataService.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesWhereMetadataStatusIsgood;

            result.Should().Be(1);
        }

        [Fact]
        public void CountDatasetIfMetadataStatusIsGoodOrDeficent()
        {
            InspireDataset inspireDataset = CreateInspireDataset();
            inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficient", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsWithMetadata;

            result.Should().Be(2);
        }

        [Fact]
        public void NotCountDatasetIfMetadataStatusIsOtherThenGoodOrDeficent()
        {
            InspireDataset inspireDataset = CreateInspireDataset();
            inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficient", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsWithMetadata;

            result.Should().Be(1);
        }


        [Fact]
        public void CountServiceIfMetadataStatusIsGoodOrDeficent()
        {
            InspireDataService inspireDataService = CreateInspireDataService();
            inspireDataService.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(inspireDataService);

            _inspireDataService.InspireDeliveryMetadata = new DatasetDelivery("deficient", null, true);
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesWithMetadata;

            result.Should().Be(2);
        }

        

        [Fact]
        public void NotCountServiceIfMetadataStatusIsOtherThenGoodOrDeficent()
        {
            InspireDataService inspireDataService = CreateInspireDataService();
            inspireDataService.InspireDeliveryMetadata = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(inspireDataService);

            _inspireDataService.InspireDeliveryMetadata = new DatasetDelivery("deficient", null, true);
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesWithMetadata;

            result.Should().Be(1);
        }


        [Fact]
        public void CountDatasetIfItsRegisteredInADiscoveryService()
        {
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsRegisteredInADiscoveryService;

            result.Should().Be(1);
        }

        [Fact]
        public void CountServiceIfItsRegisteredInADiscoveryService()
        {
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesRegisteredInADiscoveryService;

            result.Should().Be(1);
        }

        [Fact]
        public void CountServiceIfServiceTypeIsDownload()
        {
            _inspireDataService.ServiceType = "download";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeDownload;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfServiceTypeIsOtherThenDownload()
        {
            _inspireDataService.ServiceType = "";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeDownload;

            result.Should().Be(0);
        }

        [Fact]
        public void CountServiceIfServiceTypeIsView()
        {
            _inspireDataService.ServiceType = "view";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeView;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfServiceTypeIsOtherThenView()
        {
            _inspireDataService.ServiceType = "download";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeView;

            result.Should().Be(0);
        }

        [Fact]
        public void CountServiceIfServiceTypeIsDiscovery()
        {
            _inspireDataService.ServiceType = "discovery";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeDiscovery;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfServiceTypeIsOtherThenDiscovery()
        {
            _inspireDataService.ServiceType = "download";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeDiscovery;

            result.Should().Be(0);
        }

        [Fact]
        public void CountServiceIfServiceTypeIsInvoke()
        {
            _inspireDataService.ServiceType = "invoke";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeInvoke;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfServiceTypeIsOtherThenInvoke()
        {
            _inspireDataService.ServiceType = "vhbjknml";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeInvoke;

            result.Should().Be(0);
        }

        public void CountServiceIfServiceTypeIsTransformation()
        {
            _inspireDataService.ServiceType = "transformation";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeTransformation;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfServiceTypeIsOtherThenTransformation()
        {
            _inspireDataService.ServiceType = "vhbjknml";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeTransformation;

            result.Should().Be(0);
        }

        [Fact]
        public void CountServiceIfInspireDataTypeIsNotWmsOrWfs()
        {
            _inspireDataService.InspireDataType = "other";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfSdS;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfInspireDataTypeIsWfs()
        {
            InspireDataService inspireDataService = CreateInspireDataService();
            inspireDataService.InspireDataType = "other";
            _inspireItems.Add(inspireDataService);

            _inspireDataService.InspireDataType = "WFS-tjeneste";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfSdS;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfInspireDataTypeIsWms()
        {
            InspireDataService inspireDataService = CreateInspireDataService();
            inspireDataService.InspireDataType = "other";
            _inspireItems.Add(inspireDataService);

            _inspireDataService.InspireDataType = "WMS-tjeneste";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfSdS;

            result.Should().Be(1);
        }

        [Fact]
        public void CountServiceIfServiceTypeIsDownloadAndConformityIsTrue()
        {
            _inspireDataService.InspireDeliveryServiceStatus = new DatasetDelivery("good", null, true);
            _inspireDataService.ServiceType = "download";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfServiceTypeIsDownloadAndConformityIsFalse()
        {
            _inspireDataService.InspireDeliveryServiceStatus = new DatasetDelivery("deficient", null, true);
            _inspireDataService.ServiceType = "download";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue;

            result.Should().Be(0);
        }

        [Fact]
        public void CountServiceIfServiceTypeIsViewAndConformityIsTrue()
        {
            _inspireDataService.InspireDeliveryServiceStatus = new DatasetDelivery("good", null, true);
            _inspireDataService.ServiceType = "view";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeViewWhereConformityIsTrue;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfServiceTypeIsViewAndConformityIsFalse()
        {
            _inspireDataService.InspireDeliveryServiceStatus = new DatasetDelivery("deficient", null, true);
            _inspireDataService.ServiceType = "view";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeViewWhereConformityIsTrue;

            result.Should().Be(0);
        }

        [Fact]
        public void CountServiceIfServiceTypeIsDiscoveryAndConformityIsTrue()
        {
            _inspireDataService.InspireDeliveryServiceStatus = new DatasetDelivery("good", null, true);
            _inspireDataService.ServiceType = "discovery";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfServiceTypeIsDiscoveryAndConformityIsFalse()
        {
            _inspireDataService.InspireDeliveryServiceStatus = new DatasetDelivery("deficient", null, true);
            _inspireDataService.ServiceType = "discovery";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue;

            result.Should().Be(0);
        }

        [Fact]
        public void CountServiceIfServiceTypeIsInvokeAndConformityIsTrue()
        {
            _inspireDataService.InspireDeliveryServiceStatus = new DatasetDelivery("good", null, true);
            _inspireDataService.ServiceType = "invoke";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfServiceTypeIsInvokeAndConformityIsFalse()
        {
            _inspireDataService.InspireDeliveryServiceStatus = new DatasetDelivery("deficient", null, true);
            _inspireDataService.ServiceType = "invoke";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue;

            result.Should().Be(0);
        }

        [Fact]
        public void CountServiceIfServiceTypeIsTransformationAndConformityIsTrue()
        {
            _inspireDataService.InspireDeliveryServiceStatus = new DatasetDelivery("good", null, true);
            _inspireDataService.ServiceType = "transformation";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountServiceIfServiceTypeIsTransformationAndConformityIsFalse()
        {
            _inspireDataService.InspireDeliveryServiceStatus = new DatasetDelivery("deficient", null, true);
            _inspireDataService.ServiceType = "transformation";
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue;

            result.Should().Be(0);
        }

        [Fact]
        public void CountNumberOfCallsByServiceTypeDiscorvery()
        {
            var inspireDataService = CreateInspireDataService();
            inspireDataService.ServiceType = "discovery";
            inspireDataService.Requests = 2;
            _inspireItems.Add(inspireDataService);

            _inspireDataService.ServiceType = "discovery";
            _inspireDataService.Requests = 3;
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfCallsByServiceTypeDiscovery;

            result.Should().Be(5);
        }

        [Fact]
        public void CountNumberOfCallsByServiceTypeView()
        {
            var inspireDataService = CreateInspireDataService();
            inspireDataService.ServiceType = "view";
            inspireDataService.Requests = 2;
            _inspireItems.Add(inspireDataService);

            _inspireDataService.ServiceType = "view";
            _inspireDataService.Requests = 3;
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfCallsByServiceTypeView;

            result.Should().Be(5);
        }

        [Fact]
        public void CountNumberOfCallsByServiceTypeDownload()
        {
            var inspireDataService = CreateInspireDataService();
            inspireDataService.ServiceType = "download";
            inspireDataService.Requests = 2;
            _inspireItems.Add(inspireDataService);

            _inspireDataService.ServiceType = "download";
            _inspireDataService.Requests = 3;
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfCallsByServiceTypeDownload;

            result.Should().Be(5);
        }

        [Fact]
        public void CountNumberOfCallsByServiceTypeTransformation()
        {
            var inspireDataService = CreateInspireDataService();
            inspireDataService.ServiceType = "transformation";
            inspireDataService.Requests = 2;
            _inspireItems.Add(inspireDataService);

            _inspireDataService.ServiceType = "transformation";
            _inspireDataService.Requests = 3;
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfCallsByServiceTypeTransformation;

            result.Should().Be(5);
        }

        [Fact]
        public void CountNumberOfCallsByServiceTypeInvoke()
        {
            var inspireDataService = CreateInspireDataService();
            inspireDataService.ServiceType = "invoke";
            inspireDataService.Requests = 2;
            _inspireItems.Add(inspireDataService);

            _inspireDataService.ServiceType = "invoke";
            _inspireDataService.Requests = 3;
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfCallsByServiceTypeInvoke;

            result.Should().Be(5);
        }

        [Fact]
        public void CountDatasetIfWmsAndWfsIsGoodOrUseable()
        {
            _inspireDataset.InspireDeliveryWms = new DatasetDelivery("useable", null, true);
            _inspireDataset.InspireDeliveryWfs = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsAvailableThroughViewANDDownloadService;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountDatasetIfWmsIsNotGoodOrUseable()
        {
            _inspireDataset.InspireDeliveryWms = new DatasetDelivery("notset", null, true);
            _inspireDataset.InspireDeliveryWfs = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsAvailableThroughViewANDDownloadService;

            result.Should().Be(0);
        }

        [Fact]
        public void NotCountDatasetIfWfsIsNotGoodOrUseable()
        {
            _inspireDataset.InspireDeliveryWms = new DatasetDelivery("good", null, true);
            _inspireDataset.InspireDeliveryWfs = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsAvailableThroughViewANDDownloadService;

            result.Should().Be(0);
        }


        [Fact]
        public void CountDatasetIfWfsIsGood()
        {
            _inspireDataset.InspireDeliveryWfs = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsAvailableThroughDownloadService;

            result.Should().Be(1);
        }

        [Fact]
        public void CountDatasetIfWfsIsUseable()
        {
            _inspireDataset.InspireDeliveryWfs = new DatasetDelivery("useable", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsAvailableThroughDownloadService;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountDatasetIfWfsIsNeitherGoodOrUseable()
        {
            _inspireDataset.InspireDeliveryWfs = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsAvailableThroughDownloadService;

            result.Should().Be(0);
        }


        [Fact]
        public void CountDatasetIfWmsIsGood()
        {
            _inspireDataset.InspireDeliveryWms = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsAvailableThroughViewService;

            result.Should().Be(1);
        }

        [Fact]
        public void CountDatasetIfWmsIsUseable()
        {
            _inspireDataset.InspireDeliveryWms = new DatasetDelivery("useable", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsAvailableThroughViewService;

            result.Should().Be(1);
        }

        [Fact]
        public void NotCountDatasetIfWmsIsNeitherGoodOrUseable()
        {
            _inspireDataset.InspireDeliveryWms = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsAvailableThroughViewService;

            result.Should().Be(0);
        }

        [Fact]
        public void CountDatasetIfInspireThemeIsOfTypeAnnexIAndHarmonizedDataIsGoodAndMetadataIsGood() {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireDataset.InspireDeliveryHarmonizedData = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata;

            result.Should().Be(1);
        }

        [Fact]
        public void CountDatasetIfInspireThemeIsOfTypeAnnexIIAndHarmonizedDataIsGoodAndMetadataIsGood()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireDataset.InspireDeliveryHarmonizedData = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata;

            result.Should().Be(1);
        }

        [Fact]
        public void CountDatasetIfInspireThemeIsOfTypeAnnexIIIAndHarmonizedDataIsGoodAndMetadataIsGood()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireDataset.InspireDeliveryHarmonizedData = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata;

            result.Should().Be(1);
        }

        public void CountAccumulatedCurrentAreaWhenInspireThemeIsOfTypeAnnexI() {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            inspireDataset.Area = 4;

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.Area = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedCurrentAreaByAnnexI;

            result.Should().Be(9);
        }

        public void CountAccumulatedCurrentAreaWhenInspireThemeIsOfTypeAnnexII()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            inspireDataset.Area = 4;

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.Area = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedCurrentAreaByAnnexII;

            result.Should().Be(9);
        }

        public void CountAccumulatedCurrentAreaWhenInspireThemeIsOfTypeAnnexIII()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            inspireDataset.Area = 4;

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.Area = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedCurrentAreaByAnnexIII;

            result.Should().Be(9);
        }

        public void CountAccumulatedRelevantAreaWhenInspireThemeIsOfTypeAnnexI()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            inspireDataset.RelevantArea = 4;

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.RelevantArea = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedRelevantAreaByAnnexI;

            result.Should().Be(9);
        }

        public void CountAccumulatedRelevantAreaWhenInspireThemeIsOfTypeAnnexII()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            inspireDataset.RelevantArea = 4;

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.RelevantArea = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedRelevantAreaByAnnexII;

            result.Should().Be(9);
        }

        public void CountAccumulatedRelevantAreaWhenInspireThemeIsOfTypeAnnexIII()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            inspireDataset.RelevantArea = 4;

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.RelevantArea = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedRelevantAreaByAnnexIII;

            result.Should().Be(9);
        }


        // ******** Hjelpemetoder **********

        private CodelistValue ThemeOfTypeAnnexI()
        {
            return new CodelistValue {
                name = "Administrative enheter",
                value = "Administrative units"
            };
        }

        private CodelistValue ThemeOfTypeAnnexII()
        {
            return new CodelistValue
            {
                name = "Geologi",
                value = "Geology"
            };
        }

        private CodelistValue ThemeOfTypeAnnexIII()
        {
            return new CodelistValue
            {
                name = "Havområder",
                value = "Sea regions"
            };
        }

        private InspireDataset CreateInspireDataset()
        {
            return new InspireDataset
            {
                InspireThemes = new List<CodelistValue>()
            };
        }

        private InspireDataService CreateInspireDataService()
        {
            return new InspireDataService
            {
                InspireThemes = new List<CodelistValue>()
            };
        }
    }
}
