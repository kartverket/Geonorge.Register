using Eu.Europa.Ec.Jrc.Inspire;
using FluentAssertions;
using Kartverket.Register.Models;
using Kartverket.Register.Services;
using Moq;
using System;
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

            var NSv41 = monitoring.Indicators.NnConformityIndicators.NnConformity.NSv41;
            var NSv_NumDiscServ = monitoring.Indicators.SpatialDataAndService.NSv_NumDiscServ;

            var NSi41 = Divide(NSv41, NSv_NumDiscServ);

            NSi41.Should().Be(1);
        }

        [Fact]
        // Andel av tjenester NnServiceType="view" som har nnConformity="true" (<NSv42>/<NSv_NumViewServ>)
        public void NSi42()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeViewWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeView = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv42 = monitoring.Indicators.NnConformityIndicators.NnConformity.NSv42;
            var NSv_NumViewServ = monitoring.Indicators.SpatialDataAndService.NSv_NumViewServ;

            var NSi42 = Divide(NSv42, NSv_NumViewServ);

            NSi42.Should().Be(1);
        }

        [Fact]
        // Andel av tjenester NnServiceType="download" som har nnConformity="true" (<NSv43>/<NSv_NumDownServ>)
        public void NSi43()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDownload = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv43 = monitoring.Indicators.NnConformityIndicators.NnConformity.NSv43;
            var NSv_NumDownServ = monitoring.Indicators.SpatialDataAndService.NSv_NumDownServ;

            var NSi43 = Divide(NSv43, NSv_NumDownServ);

            NSi43.Should().Be(1);
        }

        [Fact]
        // Andel av tjenester NnServiceType="transformation" som har nnConformity="true" (<NSv44>/<NSv_NumTransfServ>)
        public void NSi44()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformation = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv44 = monitoring.Indicators.NnConformityIndicators.NnConformity.NSv44;
            var NSv_NumTransfServ = monitoring.Indicators.SpatialDataAndService.NSv_NumTransfServ;

            var NSi44 = Divide(NSv44, NSv_NumTransfServ);

            NSi44.Should().Be(1);

        }

        [Fact]
        // Andel av tjenester NnServiceType="invoke" som har nnConformity="true" (<NSv45>/<NSv_NumInvkServ>)
        public void NSi45()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeInvoke = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv45 = monitoring.Indicators.NnConformityIndicators.NnConformity.NSv45;
            var NSv_NumInvkServ = monitoring.Indicators.SpatialDataAndService.NSv_NumInvkServ;

            var NSi45 = Divide(NSv45, NSv_NumInvkServ);

            NSi45.Should().Be(1);
        }

        [Fact]
        // Andel av tjenester NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true" (<NSv4>/<NSv_NumAllServ>)
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

            var NSv4 = monitoring.Indicators.NnConformityIndicators.NnConformity.NSv4;
            var NSv_NumAllServ = monitoring.Indicators.SpatialDataAndService.NSv_NumAllServ;

            var NSi4 = Divide(NSv4, NSv_NumAllServ);

            NSi4.Should().Be(1);

        }



        // *********** GeoCoverageIndicators *******************


        [Fact]
        // <DSv11_ActArea>/<DSv11_RelArea>
        public void DSi11()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexI = 2;
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexI = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var DSv11_ActArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv11_ActArea;
            var DSv11_RelArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv11_RelArea;

            var DSi11 = Divide(DSv11_ActArea, DSv11_RelArea);

            DSi11.Should().Be(1);
        }

        [Fact]
        // <DSv12_ActArea>/<DSv12_RelArea>
        public void DSi12()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexII = 2;
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexII = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var DSv12_ActArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv12_ActArea;
            var DSv12_RelArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv12_RelArea;

            var DSi12 = Divide(DSv12_ActArea, DSv12_RelArea);

            DSi12.Should().Be(1);
        }

        [Fact]
        // <DSv13_ActArea>/<DSv13_RelArea>
        public void DSi13()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexIII = 2;
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexIII = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var DSv13_ActArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv13_ActArea;
            var DSv13_RelArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv13_RelArea;

            var DSi13 = Divide(DSv13_ActArea, DSv13_RelArea);

            DSi13.Should().Be(1);
        }

        [Fact]
        // <DSv1_ActArea>/<DSv1_RelArea> 
        public void DSi1()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexI = 2;
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexII = 2;
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexIII = 2;

            _inspireMonitoring.AccumulatedRelevantAreaByAnnexI = 2;
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexII = 2;
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexIII = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);


            var DSv1_ActArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv1_ActArea;
            var DSv1_RelArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv1_RelArea;

            var DSi1 = Divide(DSv1_ActArea, DSv1_RelArea);

            DSi1.Should().Be(1);
        }



        [Fact]
        // Akkumulert aktuelt areal (<actualArea>) for alle Annex1 data
        public void DSv11_ActArea()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexI = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv11_ActArea.Should().Be(2);
        }

        [Fact]
        // Akkumulert aktuelt areal (<actualArea>) for alle Annex2 data
        public void DSv12_ActArea()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexII = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv12_ActArea.Should().Be(2);
        }

        [Fact]
        // Akkumulert aktuelt areal (<actualArea>) for alle Annex3 data
        public void DSv13_ActArea()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexIII = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv13_ActArea.Should().Be(2);
        }

        [Fact]
        // Akkumulert aktuelt areal(<actualArea>) for alle Annex1, Annex2 og Annex3 data
        public void DSv1_ActArea()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexI = 2;
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexII = 2;
            _inspireMonitoring.AccumulatedCurrentAreaByAnnexIII = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            var DSv11_ActArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv11_ActArea;
            var DSv12_ActArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv12_ActArea;
            var DSv13_ActArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv13_ActArea;

            var DSv1_ActArea = DSv11_ActArea + DSv12_ActArea + DSv13_ActArea;

            DSv1_ActArea.Should().Be(6);
        }


        [Fact]
        // Akkumulert relevant areal (<relevantArea>) for alle Annex1 data
        public void DSv11_RelArea()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexI = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv11_RelArea.Should().Be(2);
        }

        [Fact]
        // Akkumulert relevant areal (<relevantArea>) for alle Annex2 data
        public void DSv12_RelArea()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexII = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv12_RelArea.Should().Be(2);
        }

        [Fact]
        // Akkumulert relevant areal (<relevantArea>) for alle Annex3 data
        public void DSv13_RelArea()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexIII = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv13_RelArea.Should().Be(2);
        }


        [Fact]
        // Akkumulert relevant areal (<relevantArea>) for alle Annex1, Annex2 og Annex3 data
        public void DSv1DSv1_RelArea_ActArea()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexI = 2;
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexII = 2;
            _inspireMonitoring.AccumulatedRelevantAreaByAnnexIII = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            var DSv11_RelArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv11_RelArea;
            var DSv12_RelArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv12_RelArea;
            var DSv13_RelArea = monitoring.Indicators.GeoCoverageIndicators.GeoCoverageSDS.DSv13_RelArea;

            var DSv1_ActArea = DSv11_RelArea + DSv12_RelArea + DSv13_RelArea;

            DSv1_ActArea.Should().Be(6);
        }



        // **********************

        private double Divide(int x, int y)
        {
            try
            {
                if (y == 0)
                {
                    return 0;
                }
                else
                {
                    return (double)x / y;
                }
            }
            catch (Exception e)
            {

                return 0;
            }
        }

        private double Divide(double x, double y)
        {
            try
            {
                if (y == 0)
                {
                    return 0;
                }
                else
                {
                    return x / y;
                }
            }
            catch (Exception e)
            {

                return 0;
            }
        }

    }
}
