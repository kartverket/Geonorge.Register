using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Moq;
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
        public void GetNumberOfDatasetsByAnnexI()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexI());
            _inspireItems.Add(_inspireDataset);
            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexI;

            result.Should().Be(1);
        }

        [Fact]
        public void GetNumberOfDatasetsByAnnexII()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexII());
            _inspireItems.Add(_inspireDataset);
            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexII;

            result.Should().Be(1);
        }

        [Fact]
        public void GetNumberOfDatasetsByAnnexIII()
        {
            _inspireDataset.InspireThemes.Add(ThemeOfTypeAnnexIII());
            _inspireItems.Add(_inspireDataset);
            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexIII;

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
    }
}
