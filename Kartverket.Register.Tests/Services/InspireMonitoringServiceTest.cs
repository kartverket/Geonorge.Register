using Eu.Europa.Ec.Jrc.Inspire;
using FluentAssertions;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Moq;
using Xunit;

namespace Kartverket.Register.Tests.Services
{
    public class InspireMonitoringServiceTest
    {
        private readonly IInspireMonitoringService _inpsireMonitoringService;
        private readonly IInspireMonitoring _inspireMonitoring;

        public InspireMonitoringServiceTest()
        {
            _inpsireMonitoringService = new InspireMonitoringService();
            _inspireMonitoring = new InspireMonitoring();
        }



        // *********** NnConformityIndicators *******************

        [Fact]
        // (Antall tjenester (<SpatialDataService>) av NnServiceType="discovery" som har nnConformity="true")
        public void NSv41()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NnConformity.NSv41.Should().Be(2);
        }

        [Fact]
        // (Antall tjenester (<SpatialDataService>) av NnServiceType="view" som har nnConformity="true")
        public void NSv42()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeViewWhereConformityIsTrue = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NnConformity.NSv42.Should().Be(2);
        }

        [Fact]
        // (Antall tjenester (<SpatialDataService>) av NnServiceType="download" som har nnConformity="true")
        public void NSv43()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NnConformity.NSv43.Should().Be(2);
        }
        [Fact]
        // (Antall tjenester (<SpatialDataService>) av NnServiceType="transformation" som har nnConformity="true")
        public void NSv44()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NnConformity.NSv44.Should().Be(2);
        }
        [Fact]
        // (Antall tjenester (<SpatialDataService>) av NnServiceType="invoke" som har nnConformity="true")
        public void NSv45()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NnConformity.NSv45.Should().Be(2);
        }
        [Fact]
        // (Antall tjenester (<SpatialDataService>) av NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true")
        public void NSv4()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeViewWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NnConformity.NSv4.Should().Be(10);
        }

        [Fact]
        // Andel av tjenester NnServiceType="discovery" som har nnConformity="true"(<NSv41>/<NSv_NumDiscServ>)
        public void NSi41()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscovery = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NSi41.Should().Be(1);
        }

        [Fact]
        // Andel av tjenester NnServiceType="view" som har nnConformity="true" (<NSv42>/<NSv_NumViewServ>)
        public void NSi42()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeViewWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeView = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NSi42.Should().Be(1);
        }

        [Fact]
        // Andel av tjenester NnServiceType="download" som har nnConformity="true" (<NSv43>/<NSv_NumDownServ>)
        public void NSi43()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDownload = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NSi43.Should().Be(1);
        }

        [Fact]
        // Andel av tjenester NnServiceType="transformation" som har nnConformity="true" (<NSv44>/<NSv_NumTransfServ>)
        public void NSi44()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformation = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NSi44.Should().Be(1);
        }

        [Fact]
        // Andel av tjenester NnServiceType="invoke" som har nnConformity="true" (<NSv45>/<NSv_NumInvkServ>)
        public void NSi45()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeInvoke = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NSi45.Should().Be(1);
        }

        [Fact]
        // Andel av tjenester NnServiceType="invoke" som har nnConformity="true" (<NSv45>/<NSv_NumInvkServ>)
        public void NSi4()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeViewWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue = 2;

            _inspireMonitoring.NumberOfServicesByServiceTypeDownload = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeView = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscovery = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeInvoke = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformation = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.NnConformityIndicators.NSi4.Should().Be(1);
        }




    }
}
