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
        public void CountDatasetWhenInspireThemeIsOfTypeAnnexIAndMetadataIsGoodOrDeficent()
        {
            var inspireDataset = new InspireDataset();
            inspireDataset.InspireThemes = new List<CodelistValue>();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata;

            result.Should().Be(2);
        }

        [Fact]
        public void NotCountDatasetWhenInspireThemeIsAnnexIAndMetadataIsOtherThenGoodOrDeficent()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata;

            result.Should().Be(0);
        }


        [Fact]
        public void CountDatasetWhenInspireThemesIsOfTypeAnnexIIAndMetadataIsGoodOrDeficent()
        {
            InspireDataset inspireDataset = CreateInspireDataset(); ;
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata;

            result.Should().Be(2);
        }

        [Fact]
        public void NotCountDatasetsWhenInspireThemeIsOfTypeAnnexIIAndMetadataIsOtherThenGoodOrDeficent()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("notset", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata;

            result.Should().Be(0);
        }



        [Fact]
        public void CountDatasetsWhenInspireThemeIsOfTypeAnnexIIIAndMetadataIsGoodOrDeficent()
        {
            InspireDataset inspireDataset = CreateInspireDataset();
            inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(inspireDataset);

            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
            _inspireItems.Add(_inspireDataset);


            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIIWithMetadata;

            result.Should().Be(2);
        }

        [Fact]
        public void NotCountDatasetsWhenInspireThemeIsOfTypeAnnexIIIAndMetadataIsOtherThenGoodOrDeficent()
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
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
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
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
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

            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
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

            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
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

            _inspireDataService.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
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

            _inspireDataService.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
            _inspireItems.Add(_inspireDataService);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfServicesWithMetadata;

            result.Should().Be(1);
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
