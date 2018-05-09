using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eu.Europa.Ec.Jrc.Inspire;
using FluentAssertions;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;
using Moq;
using Xunit;
using DateTime = System.DateTime;

namespace Kartverket.Register.Tests.TestModels.ViewModel
{
    public class InspireMonitoringViewModelTest
    {
        private readonly IInspireMonitoring _inspireMonitoring;
        private Mock<IInspireMonitoring> _mockInspireMonitoring;

        public InspireMonitoringViewModelTest()
        {
            _inspireMonitoring = new InspireMonitoring();
            _mockInspireMonitoring = new Mock<IInspireMonitoring>();
        }

        [Fact]
        public void MDi1Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsWithMetadata()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi1Value.Should().Be(20);
        }

        [Fact]
        public void MDi1Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsWithMetadata).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi1Numerator.Should().Be(2);
        }

        [Fact]
        public void MDi1Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnex()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi1Denominator.Should().Be(2);
        }

        [Fact]
        public void MDi2Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetByAnnexIWithMetadatastatusGood()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi2Value.Should().Be(20);
        }

        [Fact]
        public void MDi2Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesWhereMetadataStatusIsgood).Returns(2);
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexWhereMetadataStatusIsgood()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi2Numerator.Should().Be(4);
        }

        [Fact]
        public void MDi2Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceType()).Returns(2);
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnex()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi2Denominator.Should().Be(4);
        }

        [Fact]
        public void DSi1Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfArealByAnnex()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi1Value.Should().Be(20);
        }

        [Fact]
        public void DSi1Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AccumulatedCurrentAreaByAnnex()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi1Numerator.Should().Be(2);
        }

        [Fact]
        public void DSi1Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AccumulatedRelevantAreaByAnnex()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi1Denominator.Should().Be(2);
        }

        [Fact]
        public void DSi2Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetWithHarmonizedDataAndConformedMetadata()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi2Value.Should().Be(20);
        }

        [Fact]
        public void DSi2Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsWithHarmonizedDataAndConformedMetadata()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi2Numerator.Should().Be(2);
        }

        [Fact]
        public void DSi2Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnex()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi2Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi1Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfServicesAndDatasetsRegisteredInADiscoveryService()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi1Value.Should().Be(20);
        }

        [Fact]
        public void NSi1Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsRegisteredInADiscoveryService).Returns(2);
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesRegisteredInADiscoveryService).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi1Numerator.Should().Be(4);
        }

        [Fact]
        public void NSi1Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnex()).Returns(2);
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceType()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi1Denominator.Should().Be(4);
        }

        [Fact]
        public void NSi2Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsAvailableThroughViewAndDownloadService()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi2Value.Should().Be(20);
        }

        [Fact]
        public void NSi2Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsAvailableThroughViewANDDownloadService).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi2Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi2Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeDownload).Returns(2);
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeView).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi2Denominator.Should().Be(4);
        }


        [Fact]
        public void NSi3Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AverageNumberOfCallsByServiceType()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi3Value.Should().Be(2);
        }

        [Fact]
        public void NSi3Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfCallsByServiceType()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi3Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi3Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceType()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi3Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi4Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfServicesWhereConformityIsTrue()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi4Value.Should().Be(20);
        }

        [Fact]
        public void NSi4Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeWhereConformityIsTrue()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi4Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi4Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceType()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi4Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi11Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsRegisteredInADiscoveryService()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi11Value.Should().Be(20);
        }

        [Fact]
        public void NSi11Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsRegisteredInADiscoveryService).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi11Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi11Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnex()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi11Denominator.Should().Be(2);
        }

        [Fact]
        public void MDi11Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsWithMetadataByAnnexI()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi11Value.Should().Be(20);
        }

        [Fact]
        public void MDi11Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIWithMetadata).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi11Numerator.Should().Be(2);
        }

        [Fact]
        public void MDi11Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexI).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi11Denominator.Should().Be(2);
        }

        [Fact]
        public void MDi12Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsWithMetadataByAnnexII()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi12Value.Should().Be(20);
        }

        [Fact]
        public void MDi12Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIIWithMetadata).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi12Numerator.Should().Be(2);
        }

        [Fact]
        public void MDi12Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexII).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi12Denominator.Should().Be(2);
        }

        [Fact]
        public void MDi13Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsWithMetadataByAnnexIII()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi13Value.Should().Be(20);
        }

        [Fact]
        public void MDi13Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIIIWithMetadata).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi13Numerator.Should().Be(2);
        }

        [Fact]
        public void MDi13Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIII).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi13Denominator.Should().Be(2);
        }

        [Fact]
        public void MDi21Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetByAnnexIWithMetadatastatusGood()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi21Value.Should().Be(20);
        }

        [Fact]
        public void MDi21Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi21Numerator.Should().Be(2);
        }

        [Fact]
        public void MDi21Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexI).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi21Denominator.Should().Be(2);
        }

        [Fact]
        public void MDi22Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetByAnnexIIWithMetadatastatusGood()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi22Value.Should().Be(20);
        }

        [Fact]
        public void MDi22Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi22Numerator.Should().Be(2);
        }

        [Fact]
        public void MDi22Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexII).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi22Denominator.Should().Be(2);
        }

        [Fact]
        public void MDi23Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetByAnnexIIIWithMetadatastatusGood()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi23Value.Should().Be(20);
        }

        [Fact]
        public void MDi23Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi23Numerator.Should().Be(2);
        }

        [Fact]
        public void MDi23Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIII).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi23Denominator.Should().Be(2);
        }

        [Fact]
        public void DSi11Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfArealByAnnexI()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi11Value.Should().Be(20);
        }

        [Fact]
        public void DSi11Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AccumulatedCurrentAreaByAnnexI).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi11Numerator.Should().Be(2);
        }

        [Fact]
        public void DSi11Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AccumulatedRelevantAreaByAnnexI).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi11Denominator.Should().Be(2);
        }

        [Fact]
        public void DSi12Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfArealByAnnexII()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi12Value.Should().Be(20);
        }

        [Fact]
        public void DSi12Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AccumulatedCurrentAreaByAnnexII).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi12Numerator.Should().Be(2);
        }

        [Fact]
        public void DSi12Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AccumulatedRelevantAreaByAnnexII).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi12Denominator.Should().Be(2);
        }


        [Fact]
        public void DSi13Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfArealByAnnexIII()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi13Value.Should().Be(20);
        }

        [Fact]
        public void DSi13Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AccumulatedCurrentAreaByAnnexIII).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi13Numerator.Should().Be(2);
        }

        [Fact]
        public void DSi13Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AccumulatedRelevantAreaByAnnexIII).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi13Denominator.Should().Be(2);
        }

        [Fact]
        public void DSi21Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi21Value.Should().Be(20);
        }

        [Fact]
        public void DSi21Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi21Numerator.Should().Be(2);
        }

        [Fact]
        public void DSi21Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexI).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi21Denominator.Should().Be(2);
        }

        [Fact]
        public void DSi22Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi22Value.Should().Be(20);
        }

        [Fact]
        public void DSi22Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi22Numerator.Should().Be(2);
        }

        [Fact]
        public void DSi22Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexII).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi22Denominator.Should().Be(2);
        }

        [Fact]
        public void DSi23Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi23Value.Should().Be(20);
        }

        [Fact]
        public void DSi23Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi23Numerator.Should().Be(2);
        }

        [Fact]
        public void DSi23Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsByAnnexIII).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.DSi23Denominator.Should().Be(2);
        }

        [Fact]
        public void MDi14Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfServicesWithMetadata()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi14Value.Should().Be(20);
        }

        [Fact]
        public void MDi14Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesWithMetadata).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi14Numerator.Should().Be(2);
        }

        [Fact]
        public void MDi14Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceType()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi14Denominator.Should().Be(2);
        }

        [Fact]
        public void MDi24Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfServicesWithMetadatastatusGood()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi24Value.Should().Be(20);
        }

        [Fact]
        public void MDi24Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesWhereMetadataStatusIsgood).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi24Numerator.Should().Be(2);
        }

        [Fact]
        public void MDi24Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceType()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.MDi24Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi12Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfServicesRegisteredInADiscoveryService()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi12Value.Should().Be(20);
        }

        [Fact]
        public void NSi12Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesRegisteredInADiscoveryService).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi12Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi12Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceType()).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi12Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi31Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AverageNumberOfCallsByServiceTypeDiscovery()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi31Value.Should().Be(20);
        }

        [Fact]
        public void NSi31Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfCallsByServiceTypeDiscovery).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi31Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi31Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeDiscovery).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi31Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi32Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AverageNumberOfCallsByServiceTypeView()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi32Value.Should().Be(20);
        }

        [Fact]
        public void NSi32Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfCallsByServiceTypeView).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi32Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi32Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeView).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi32Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi33Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AverageNumberOfCallsByServiceTypeDownload()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi33Value.Should().Be(20);
        }

        [Fact]
        public void NSi33Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfCallsByServiceTypeDownload).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi33Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi33Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeDownload).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi33Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi34Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AverageNumberOfCallsByServiceTypeTransformation()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi34Value.Should().Be(20);
        }

        [Fact]
        public void NSi34Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfCallsByServiceTypeTransformation).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi34Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi34Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeTransformation).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi34Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi35Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.AverageNumberOfCallsByServiceTypeInvoke()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi35Value.Should().Be(20);
        }

        [Fact]
        public void NSi35Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfCallsByServiceTypeInvoke).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi35Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi35Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeInvoke).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi35Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi41Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfServicesByServiceTypeDiscoveryWhereConformityIsTrue()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi41Value.Should().Be(20);
        }

        [Fact]
        public void NSi41Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi41Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi41Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeDiscovery).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi41Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi42Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfServicesByServiceTypeViewWhereConformityIsTrue()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi42Value.Should().Be(20);
        }

        [Fact]
        public void NSi42Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeViewWhereConformityIsTrue).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi42Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi42Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeView).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi42Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi43Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfServicesByServiceTypeDownloadWhereConformityIsTrue()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi43Value.Should().Be(20);
        }

        [Fact]
        public void NSi43Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi43Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi43Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeDownload).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi43Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi44Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfServicesByServiceTypeTransformationWhereConformityIsTrue()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi44Value.Should().Be(20);
        }

        [Fact]
        public void NSi44Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi44Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi44Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeTransformation).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi44Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi45Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfServicesByServiceTypeInvokeWhereConformityIsTrue()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi45Value.Should().Be(20);
        }

        [Fact]
        public void NSi45Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi45Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi45Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeInvoke).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi45Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi21Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsAvailableThroughViewService()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi21Value.Should().Be(20);
        }

        [Fact]
        public void NSi21Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsAvailableThroughViewService).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi21Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi21Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeView).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi21Denominator.Should().Be(2);
        }

        [Fact]
        public void NSi22Value()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.ProportionOfDatasetsAvailableThroughDownloadService()).Returns(0.2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi22Value.Should().Be(20);
        }

        [Fact]
        public void NSi22Numerator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfDatasetsAvailableThroughDownloadService).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi22Numerator.Should().Be(2);
        }

        [Fact]
        public void NSi22Denominator()
        {
            _mockInspireMonitoring.As<IInspireMonitoring>().Setup(m => m.NumberOfServicesByServiceTypeDownload).Returns(2);
            var inspireMonitoringViewModel = new InspireMonitoringViewModel(_mockInspireMonitoring.Object);
            inspireMonitoringViewModel.NSi22Denominator.Should().Be(2);
        }



    }
}
