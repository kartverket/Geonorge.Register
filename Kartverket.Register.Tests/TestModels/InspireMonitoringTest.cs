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
        private ICollection<InspireDataService> _inspireDataService = new List<InspireDataService>();

        private InspireDataset _inspireDataset;


        public InspireMonitoringTest() {
            _inspireDataset = new InspireDataset();
            _inspireDataset.InspireThemes = new List<CodelistValue>();
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
        public void CountDatasetWhenInspireThemeIsOfTypeAnnexIAndMetadataIsGood() {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata;

            result.Should().Be(1);
        }

        [Fact]
        public void CountDatasetWhenInspireThemeIsOfTypeAnnexIAndMetadataIsDeficent()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata;

            result.Should().Be(1);
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
        public void CountDatasetWhenInspireThemesIsOfTypeAnnexIIAndMetadataIsGood()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata;

            result.Should().Be(1);
        }

        [Fact]
        public void CountDatasetWhenInspireThemesIsOfTypeAnnexIIAndMetadataIsDeficent()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata;

            result.Should().Be(1);
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
        public void CountDatasetsWhenInspireThemeIsOfTypeAnnexIIIAndMetadataIsGood()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("good", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIIWithMetadata;

            result.Should().Be(1);
        }

        [Fact]
        public void CountDatasetsWhenInspireThemeIsOfTypeAnnexIIIAndMetadataIsDeficent()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireDataset.InspireDeliveryMetadata = new DatasetDelivery("deficent", null, true);
            _inspireItems.Add(_inspireDataset);

            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIIIWithMetadata;

            result.Should().Be(1);
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
    }
}
