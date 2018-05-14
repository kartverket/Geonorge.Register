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
        private InspireDataset _inspireDataset;
        private InspireDataService _inspireDataService;
        private InspireMonitoring _inspireMonitoringTest;


        public InspireMonitoringTest()
        {
            _inspireDataset = new InspireDataset();
            _inspireDataset.InspireThemes = new List<CodelistValue>();
            _inspireDataService = new InspireDataService();
            _inspireMonitoringTest = InspireMonitoringTestData();
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
        public void CountDatasetIfInspireThemeIsOfTypeAnnexIAndHarmonizedDataIsGoodAndMetadataIsGood()
        {
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

        [Fact]
        public void CountAccumulatedCurrentAreaWhenInspireThemeIsOfTypeAnnexI()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            inspireDataset.Area = 4;
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.Area = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedCurrentAreaByAnnexI;

            result.Should().Be(9);
        }

        [Fact]
        public void CountAccumulatedCurrentAreaWhenInspireThemeIsOfTypeAnnexII()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            inspireDataset.Area = 4;
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.Area = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedCurrentAreaByAnnexII;

            result.Should().Be(9);
        }

        [Fact]
        public void CountAccumulatedCurrentAreaWhenInspireThemeIsOfTypeAnnexIII()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            inspireDataset.Area = 4;
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.Area = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedCurrentAreaByAnnexIII;

            result.Should().Be(9);
        }

        [Fact]
        public void CountAccumulatedRelevantAreaWhenInspireThemeIsOfTypeAnnexI()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            inspireDataset.RelevantArea = 4;
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.RelevantArea = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedRelevantAreaByAnnexI;

            result.Should().Be(9);
        }

        [Fact]
        public void CountAccumulatedRelevantAreaWhenInspireThemeIsOfTypeAnnexII()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            inspireDataset.RelevantArea = 4;
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.RelevantArea = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedRelevantAreaByAnnexII;

            result.Should().Be(9);
        }

        [Fact]
        public void CountAccumulatedRelevantAreaWhenInspireThemeIsOfTypeAnnexIII()
        {
            var inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            inspireDataset.RelevantArea = 4;
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.RelevantArea = 5;
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.AccumulatedRelevantAreaByAnnexIII;

            result.Should().Be(9);
        }

        [Fact]
        public void NumberOfCallsByServiceType()
        {
            var result = _inspireMonitoringTest.NumberOfCallsByServiceType();
            result.Should().Be(10);
        }

        [Fact]
        public void NumberOfServicesByServiceType()
        {
            var result = _inspireMonitoringTest.NumberOfServicesByServiceType();
            result.Should().Be(10);
        }

        [Fact]
        public void NumberOfDatasetsWithHarmonizedDataAndConformedMetadata()
        {
            var result = _inspireMonitoringTest.NumberOfDatasetsWithHarmonizedDataAndConformedMetadata();
            result.Should().Be(6);
        }

        [Fact]
        public void NumberOfServicesByServiceTypeWhereConformityIsTrue()
        {
            var result = _inspireMonitoringTest.NumberOfServicesByServiceTypeWhereConformityIsTrue();
            result.Should().Be(10);
        }

        [Fact]
        public void NumberOfDatasetsByAnnexWhereMetadataStatusIsgood()
        {
            var result = _inspireMonitoringTest.NumberOfDatasetsByAnnexWhereMetadataStatusIsgood();
            result.Should().Be(6);
        }

        [Fact]
        public void AccumulatedCurrentAreaByAnnex()
        {
            var result = _inspireMonitoringTest.AccumulatedCurrentAreaByAnnex();
            result.Should().Be(6);
        }

        [Fact]
        public void AccumulatedRelevantAreaByAnnex()
        {
            var result = _inspireMonitoringTest.AccumulatedRelevantAreaByAnnex();
            result.Should().Be(6);
        }

        [Fact]
        public void AverageNumberOfCallsByServiceTypeDiscovery()
        {
            var result = _inspireMonitoringTest.AverageNumberOfCallsByServiceTypeDiscovery();
            result.Should().Be(1);
        }

        [Fact]
        public void AverageNumberOfCallsByServiceTypeView()
        {
            var result = _inspireMonitoringTest.AverageNumberOfCallsByServiceTypeView();
            result.Should().Be(1);
        }

        [Fact]
        public void AverageNumberOfCallsByServiceTypeTransformation()
        {
            var result = _inspireMonitoringTest.AverageNumberOfCallsByServiceTypeTransformation();
            result.Should().Be(1);
        }

        [Fact]
        public void AverageNumberOfCallsByServiceTypeInvoke()
        {
            var result = _inspireMonitoringTest.AverageNumberOfCallsByServiceTypeInvoke();
            result.Should().Be(1);
        }

        [Fact]
        public void AverageNumberOfCallsByServiceType()
        {
            var result = _inspireMonitoringTest.AverageNumberOfCallsByServiceType();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetsRegisteredInADiscoveryService()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsRegisteredInADiscoveryService();
            result.Should().Be(0.33333333333333331);
        }

        [Fact]
        public void ProportionOfServicesRegisteredInADiscoveryService()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesRegisteredInADiscoveryService();
            result.Should().Be(0.2);
        }

        [Fact]
        public void ProportionOfDatasetsWithMetadataByAnnexI()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsWithMetadataByAnnexI();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetsWithMetadataByAnnexII()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsWithMetadataByAnnexII();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetsWithMetadataByAnnexIII()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsWithMetadataByAnnexIII();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfServicesWithMetadata()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesWithMetadata();
            result.Should().Be(0.2);
        }

        [Fact]
        public void ProportionOfDatasetsWithMetadata()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsAndServicesWithMetadata();
            result.Should().Be(0.25);
        }

        [Fact]
        public void NumberOfDatasetsByAnnex()
        {
            var result = _inspireMonitoringTest.NumberOfDatasetsByAnnex();
            result.Should().Be(6);
        }

        [Fact]
        public void ProportionOfDatasetWithHarmonizedDataAndConformedMetadata()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetWithHarmonizedDataAndConformedMetadata();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetsAvailableThroughViewService()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsAvailableThroughViewService();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetsAvailableThroughDownloadService()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsAvailableThroughDownloadService();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetsAvailableThroughViewAndDownloadService()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetsAvailableThroughViewAndDownloadService();
            result.Should().Be(0.5);
        }

        [Fact]
        public void ProportionOfServicesWhereConformityIsTrue()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesWhereConformityIsTrue();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfServicesByServiceTypeDownloadWhereConformityIsTrue()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesByServiceTypeDownloadWhereConformityIsTrue();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfServicesByServiceTypeDiscoveryWhereConformityIsTrue()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesByServiceTypeDiscoveryWhereConformityIsTrue();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfServicesByServiceTypeViewWhereConformityIsTrue()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesByServiceTypeViewWhereConformityIsTrue();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfServicesByServiceTypeTransformationWhereConformityIsTrue()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesByServiceTypeTransformationWhereConformityIsTrue();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfServicesByServiceTypeInvokeWhereConformityIsTrue()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesByServiceTypeInvokeWhereConformityIsTrue();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfServicesAndDatasetsRegisteredInADiscoveryService()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesAndDatasetsRegisteredInADiscoveryService();
            result.Should().Be(0.25);
        }

        [Fact]
        public void ProportionOfArealByAnnexI()
        {
            var result = _inspireMonitoringTest.ProportionOfArealByAnnexI();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfArealByAnnexII()
        {
            var result = _inspireMonitoringTest.ProportionOfArealByAnnexII();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfArealByAnnexIII()
        {
            var result = _inspireMonitoringTest.ProportionOfArealByAnnexIII();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfArealByAnnex()
        {
            var result = _inspireMonitoringTest.ProportionOfArealByAnnex();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetByAnnexIWithMetadatastatusGood()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetByAnnexIWithMetadatastatusGood();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetByAnnexIIWithMetadatastatusGood()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetByAnnexIIWithMetadatastatusGood();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfDatasetByAnnexIIIWithMetadatastatusGood()
        {
            var result = _inspireMonitoringTest.ProportionOfDatasetByAnnexIIIWithMetadatastatusGood();
            result.Should().Be(1);
        }

        [Fact]
        public void ProportionOfServicesWithMetadatastatusGood()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesWithMetadatastatusGood();
            result.Should().Be(0.2);
        }

        [Fact]
        public void ProportionOfServicesAndDatasetsWithMetadatastatusGood()
        {
            var result = _inspireMonitoringTest.ProportionOfServicesAndDatasetsWithMetadatastatusGood();
            result.Should().Be(0.5);
        }










        // ******** Hjelpemetoder **********

        private CodelistValue ThemeOfTypeAnnexI()
        {
            return new CodelistValue
            {
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

        private InspireMonitoring InspireMonitoringTestData()
        {
            InspireMonitoring inspireMonitoring = new InspireMonitoring()
            {
                NumberOfDatasetsByAnnexI = 2,
                NumberOfDatasetsByAnnexII = 2,
                NumberOfDatasetsByAnnexIII = 2,

                NumberOfDatasetsByAnnexIWithMetadata = 2,
                NumberOfDatasetsByAnnexIIWithMetadata = 2,
                NumberOfDatasetsByAnnexIIIWithMetadata = 2,

                NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood = 2,
                NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood = 2,
                NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood = 2,
                NumberOfServicesWhereMetadataStatusIsgood = 2,

                NumberOfDatasetsWithMetadata = 2,
                NumberOfServicesWithMetadata = 2,

                NumberOfServicesRegisteredInADiscoveryService = 2,
                NumberOfDatasetsRegisteredInADiscoveryService = 2,

                NumberOfServicesByServiceTypeDownload = 2,
                NumberOfServicesByServiceTypeView = 2,
                NumberOfServicesByServiceTypeDiscovery = 2,
                NumberOfServicesByServiceTypeInvoke = 2,
                NumberOfServicesByServiceTypeTransformation = 2,
                NumberOfSdS = 2,

                NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue = 2,
                NumberOfServicesByServiceTypeViewWhereConformityIsTrue = 2,
                NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue = 2,
                NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue = 2,
                NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue = 2,

                NumberOfCallsByServiceTypeDiscovery = 2,
                NumberOfCallsByServiceTypeView = 2,
                NumberOfCallsByServiceTypeDownload = 2,
                NumberOfCallsByServiceTypeTransformation = 2,
                NumberOfCallsByServiceTypeInvoke = 2,

                NumberOfDatasetsAvailableThroughViewANDDownloadService = 2,
                NumberOfDatasetsAvailableThroughDownloadService = 2,
                NumberOfDatasetsAvailableThroughViewService = 2,

                NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata = 2,
                NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata = 2,
                NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata = 2,

                AccumulatedCurrentAreaByAnnexI = 2,
                AccumulatedCurrentAreaByAnnexII = 2,
                AccumulatedCurrentAreaByAnnexIII = 2,

                AccumulatedRelevantAreaByAnnexI = 2,
                AccumulatedRelevantAreaByAnnexII = 2,
                AccumulatedRelevantAreaByAnnexIII = 2,

            };
            return inspireMonitoring;
        }

    }
}
