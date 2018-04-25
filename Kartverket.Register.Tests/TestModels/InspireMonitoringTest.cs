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
        private ICollection<InspireDataset> _inspireDataset = new List<InspireDataset>();
        private ICollection<InspireDataService> _inspireDataService = new List<InspireDataService>();



        [Fact]
        public void GetNumberOfDatasetsByAnnexI()
        {
            _inspireItems.Add(InspireDatasetWithInspireThemeOfTypeAnnexI());
            var inspireMonitoring = new InspireMonitoring(_inspireItems);

            var result = inspireMonitoring.NumberOfDatasetsByAnnexI;

            result.Should().Be(1);

        }

        private InspireDataset InspireDatasetWithInspireThemeOfTypeAnnexI()
        {
            var inspireDataset = new InspireDataset();
            inspireDataset.InspireThemes = new List<CodelistValue>();
            inspireDataset.InspireThemes.Add(InspireThemeOfTypeAnnexI());
            return inspireDataset;
        }

        private CodelistValue InspireThemeOfTypeAnnexI()
        {
            return new CodelistValue {
                name = "Administrative enheter",
                value = "Administrative units"
            };
        }
    }
}
