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



        [Fact]
        // Gjennomsnittlig antall kall for NnServiceType="discovery" (<NSv31>/<NSv_NumDiscServ>)
        public void NSi31()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeDiscovery = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscovery = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv31 = monitoring.Indicators.UseNNindicators.UseNN.NSv31;
            var NSv_NumDiscServ = monitoring.Indicators.SpatialDataAndService.NSv_NumDiscServ;

            var NSi31 = Divide(NSv31, NSv_NumDiscServ);

            NSi31.Should().Be(1);
        }

        [Fact]
        // Gjennomsnittlig antall kall for NnServiceType="view"(<NSv32>/<NSv_NumViewServ>)
        public void NSi32()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeView = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeView = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv32 = monitoring.Indicators.UseNNindicators.UseNN.NSv32;
            var NSv_NumViewServ = monitoring.Indicators.SpatialDataAndService.NSv_NumViewServ;

            var NSi32 = Divide(NSv32, NSv_NumViewServ);

            NSi32.Should().Be(1);
        }

        [Fact]
        // Gjennomsnittlig antall kall for NnServiceType="download" (<NSv33>/<NSv_NumDownServ>)
        public void NSi33()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeDownload = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDownload = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv33 = monitoring.Indicators.UseNNindicators.UseNN.NSv33;
            var NSv_NumDownServ = monitoring.Indicators.SpatialDataAndService.NSv_NumDownServ;

            var NSi33 = Divide(NSv33, NSv_NumDownServ);

            NSi33.Should().Be(1);
        }

        [Fact]
        // Gjennomsnittlig antall kall for NnServiceType="transformation" (<NSv34>/<NSv_NumTransfServ>)
        public void NSi34()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeTransformation = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformation = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv34 = monitoring.Indicators.UseNNindicators.UseNN.NSv34;
            var NSv_NumTransfServ = monitoring.Indicators.SpatialDataAndService.NSv_NumTransfServ;

            var NSi34 = Divide(NSv34, NSv_NumTransfServ);

            NSi34.Should().Be(1);
        }

        [Fact]
        // Gjennomsnittlig antall kall for NnServiceType="invoke" (<NSv35>/<NSv_NumInvkServ>)
        public void NSi35()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeInvoke = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeInvoke = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv35 = monitoring.Indicators.UseNNindicators.UseNN.NSv35;
            var NSv_NumInvkServ = monitoring.Indicators.SpatialDataAndService.NSv_NumInvkServ;

            var NSi35 = Divide(NSv35, NSv_NumInvkServ);

            NSi35.Should().Be(1);
        }

        [Fact]
        //Gjennomsnittlig antall kall for NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true" (<NSv3>/<NSv_NumAllServ>) -->
        public void NSi3()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeDiscovery = 2;
            _inspireMonitoring.NumberOfCallsByServiceTypeView = 2;
            _inspireMonitoring.NumberOfCallsByServiceTypeDownload = 2;
            _inspireMonitoring.NumberOfCallsByServiceTypeTransformation = 2;
            _inspireMonitoring.NumberOfCallsByServiceTypeInvoke = 2;

            _inspireMonitoring.NumberOfServicesByServiceTypeDownload = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeView = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscovery = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeInvoke = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformation = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv3 = monitoring.Indicators.UseNNindicators.UseNN.NSv3;
            var NSv_NumAllServ = monitoring.Indicators.SpatialDataAndService.NSv_NumAllServ;

            var NSi35 = Divide(NSv3, NSv_NumAllServ);

            NSi35.Should().Be(1);
        }


        [Fact]
        // Akkumulert antall kall for alle NnServiceType="discovery" 
        public void NSv31()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeDiscovery = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.UseNNindicators.UseNN.NSv31.Should().Be(2);
        }

        [Fact]
        // Akkumulert antall kall for alle NnServiceType="view"
        public void NSv32()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeView = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.UseNNindicators.UseNN.NSv32.Should().Be(2);
        }

        [Fact]
        // Akkumulert antall kall for alle NnServiceType="download"
        public void NSv33()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeDownload = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.UseNNindicators.UseNN.NSv33.Should().Be(2);
        }

        [Fact]
        // Akkumulert antall kall for alle NnServiceType="transformation"
        public void NSv34()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeTransformation = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.UseNNindicators.UseNN.NSv34.Should().Be(2);
        }

        [Fact]
        // Akkumulert antall kall for alle NnServiceType="invoke"
        public void NSv35()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeInvoke = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.UseNNindicators.UseNN.NSv35.Should().Be(2);
        }


        [Fact]
        // Akkumulert antall kall for alle NnServiceType="discovery + view + download + transformation + invoke"
        public void NSv3()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfCallsByServiceTypeDiscovery = 2;
            _inspireMonitoring.NumberOfCallsByServiceTypeView = 2;
            _inspireMonitoring.NumberOfCallsByServiceTypeDownload = 2;
            _inspireMonitoring.NumberOfCallsByServiceTypeTransformation = 2;
            _inspireMonitoring.NumberOfCallsByServiceTypeInvoke = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            var NSv31 = monitoring.Indicators.UseNNindicators.UseNN.NSv31;
            var NSv32 = monitoring.Indicators.UseNNindicators.UseNN.NSv32;
            var NSv33 = monitoring.Indicators.UseNNindicators.UseNN.NSv33;
            var NSv34 = monitoring.Indicators.UseNNindicators.UseNN.NSv34;
            var NSv35 = monitoring.Indicators.UseNNindicators.UseNN.NSv35;

            var NSv3 = NSv31 + NSv32 + NSv33 + NSv34 + NSv35;

            NSv3.Should().Be(10);
        }


        [Fact]
        // Andel datasett som har metadata av Annex1 data (<MDv11>/<DSv_Num1>)
        public void MDi11()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata = 2;
            _inspireMonitoring.NumberOfDatasetsByAnnexI = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var MDv11 = monitoring.Indicators.MetadataExistenceIndicators.MetadataExistence.MDv11;
            var DSv_Num1 = monitoring.Indicators.SpatialDataAndService.DSv_Num1;

            var MDi11 = Divide(MDv11, DSv_Num1);

            MDi11.Should().Be(1);
        }

        [Fact]
        // Andel datasett som har metadata av Annex2 data (<MDv12>/<DSv_Num2>)
        public void MDi12()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata = 2;
            _inspireMonitoring.NumberOfDatasetsByAnnexII = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var MDv12 = monitoring.Indicators.MetadataExistenceIndicators.MetadataExistence.MDv12;
            var DSv_Num2 = monitoring.Indicators.SpatialDataAndService.DSv_Num2;

            var MDi12 = Divide(MDv12, DSv_Num2);

            MDi12.Should().Be(1);
        }

        [Fact]
        // Andel datasett som har metadata av Annex3 data (<MDv13>/<DSv_Num3>)
        public void MDi13()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsByAnnexIIIWithMetadata = 2;
            _inspireMonitoring.NumberOfDatasetsByAnnexIII = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var MDv13 = monitoring.Indicators.MetadataExistenceIndicators.MetadataExistence.MDv13;
            var DSv_Num3 = monitoring.Indicators.SpatialDataAndService.DSv_Num3;

            var MDi13 = Divide(MDv13, DSv_Num3);

            MDi13.Should().Be(1);
        }

        [Fact]
        // Andel tjenester som har metadata (<MDv14>/<NSv_NumAllServ>)
        public void MDi14()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesWithMetadata = 2;

            _inspireMonitoring.NumberOfServicesByServiceTypeDownload = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeView = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscovery = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeInvoke = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformation = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var MDv14 = monitoring.Indicators.MetadataExistenceIndicators.MetadataExistence.MDv14;
            var NSv_NumAllServ = monitoring.Indicators.SpatialDataAndService.NSv_NumAllServ;

            var MDi14 = Divide(MDv14, NSv_NumAllServ);

            MDi14.Should().Be(0.2);
        }

        [Fact]
        // Andel datasett som har metadata av Annex1 data  (<MDv1_DS>/<DSv_Num>)
        public void MDi1()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsWithMetadata = 2;

            _inspireMonitoring.NumberOfDatasetsByAnnexI = 2;
            _inspireMonitoring.NumberOfDatasetsByAnnexII = 2;
            _inspireMonitoring.NumberOfDatasetsByAnnexIII = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var MDv1_DS = monitoring.Indicators.MetadataExistenceIndicators.MetadataExistence.MDv1_DS;
            var DSv_Num = monitoring.Indicators.SpatialDataAndService.DSv_Num;

            var MDi1 = Divide(MDv1_DS, DSv_Num);

            MDi1.Should().Be(0.33333333333333331);
        }


        [Fact]
        // Antall Annex1 datasett som har metadata (Alle Annex1 datasett)
        public void MDv11()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.MetadataExistenceIndicators.MetadataExistence.MDv11.Should().Be(2);
        }

        [Fact]
        // Antall Annex2 datasett som har metadata (Alle Annex2 datasett)
        public void MDv12()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.MetadataExistenceIndicators.MetadataExistence.MDv12.Should().Be(2);
        }

        [Fact]
        // Antall Annex3 datasett som har metadata (Alle Annex3 datasett)
        public void MDv13()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsByAnnexIIIWithMetadata = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.MetadataExistenceIndicators.MetadataExistence.MDv13.Should().Be(2);
        }

        [Fact]
        // Antall Annex3 datasett som har metadata (Alle Annex3 datasett)
        public void MDv1_DS()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsWithMetadata = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.MetadataExistenceIndicators.MetadataExistence.MDv1_DS.Should().Be(2);
        }

        [Fact]
        // Antall Annex3 datasett som har metadata (Alle Annex3 datasett)
        public void MDv14()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesWithMetadata = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.MetadataExistenceIndicators.MetadataExistence.MDv14.Should().Be(2);
        }


        [Fact]
        // Andel datasett som er registrert i en discovery service (<NSv11>/<DSv_Num>)
        public void NSi11()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsRegisteredInADiscoveryService = 2;

            _inspireMonitoring.NumberOfDatasetsByAnnexI = 2;
            _inspireMonitoring.NumberOfDatasetsByAnnexII = 2;
            _inspireMonitoring.NumberOfDatasetsByAnnexIII = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv11 = monitoring.Indicators.DiscoveryMetadataIndicators.DiscoveryMetadata.NSv11;
            var DSv_Num = monitoring.Indicators.SpatialDataAndService.DSv_Num;

            var NSi11 = Divide(NSv11, DSv_Num);

            NSi11.Should().Be(0.33333333333333331);
        }

        [Fact]
        // Andel tjenester som er registrert i en discovery service (<NSv12>/<NSv_NumAllServ>)
        public void NSi12()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesRegisteredInADiscoveryService = 2;

            _inspireMonitoring.NumberOfServicesByServiceTypeDownload = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeView = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscovery = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeInvoke = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformation = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv12 = monitoring.Indicators.DiscoveryMetadataIndicators.DiscoveryMetadata.NSv12;
            var NSv_NumAllServ = monitoring.Indicators.SpatialDataAndService.NSv_NumAllServ;

            var NSi12 = Divide(NSv12, NSv_NumAllServ);

            NSi12.Should().Be(0.2);
        }

        [Fact]
        // Andel datasett og tjenester som er registrert i en discovery service (<NSv11>+<NSv12>/<DSv_Num>+<NSv_NumAllServ>)
        public void NSi1()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsRegisteredInADiscoveryService = 2;
            _inspireMonitoring.NumberOfServicesRegisteredInADiscoveryService = 2;

            _inspireMonitoring.NumberOfDatasetsByAnnexI = 2;
            _inspireMonitoring.NumberOfDatasetsByAnnexII = 2;
            _inspireMonitoring.NumberOfDatasetsByAnnexIII = 2;

            _inspireMonitoring.NumberOfServicesByServiceTypeDownload = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeView = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDiscovery = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeInvoke = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeTransformation = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv11 = monitoring.Indicators.DiscoveryMetadataIndicators.DiscoveryMetadata.NSv11;
            var NSv12 = monitoring.Indicators.DiscoveryMetadataIndicators.DiscoveryMetadata.NSv12;
            var NSv_NumAllServ = monitoring.Indicators.SpatialDataAndService.NSv_NumAllServ;
            var DSv_Num = monitoring.Indicators.SpatialDataAndService.DSv_Num;

            var NSi1 = Divide(NSv11 + NSv12, NSv_NumAllServ + DSv_Num);

            NSi1.Should().Be(0.25);
        }


        [Fact]
        // Antall datasett som er registrert i en discovery service (Alle datasett)
        public void NSv11()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsRegisteredInADiscoveryService = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.DiscoveryMetadataIndicators.DiscoveryMetadata.NSv11.Should().Be(2);
        }

        [Fact]
        // Antall tjenester som er registrert i en discovery service (Alle tjenester)
        public void NSv12()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfServicesRegisteredInADiscoveryService = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.DiscoveryMetadataIndicators.DiscoveryMetadata.NSv12.Should().Be(2);
        }



        [Fact]
        // Andel Annex1-3 datasett som er tilgjengelig gjennom view service (<NSv21>/<NSv_NumViewServ>)
        public void NSi21()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsAvailableThroughViewService = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeView = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv21 = monitoring.Indicators.ViewDownloadAccessibilityIndicators.ViewDownloadAccessibility.NSv21;
            var NSv_NumViewServ = monitoring.Indicators.SpatialDataAndService.NSv_NumViewServ;

            var NSi21 = Divide(NSv21, NSv_NumViewServ);

            NSi21.Should().Be(1);
        }

        [Fact]
        // Andel Annex1-3 datasett som er tilgjengelig gjennom download service (<NSv22>/<NSv_NumDownServ>)
        public void NSi22()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsAvailableThroughDownloadService = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDownload = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv22 = monitoring.Indicators.ViewDownloadAccessibilityIndicators.ViewDownloadAccessibility.NSv22;
            var NSv_NumDownServ = monitoring.Indicators.SpatialDataAndService.NSv_NumDownServ;

            var NSi22 = Divide(NSv22, NSv_NumDownServ);

            NSi22.Should().Be(1);
        }

        [Fact]
        // Andel Annex1-3 datasett som er tilgjengelig gjennom view OG download service (<NSv23>/<NSv_NumViewServ>+<NSv_NumDownServ>)
        public void NSi2()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsAvailableThroughViewANDDownloadService = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeDownload = 2;
            _inspireMonitoring.NumberOfServicesByServiceTypeView = 2;

            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);

            var NSv23 = monitoring.Indicators.ViewDownloadAccessibilityIndicators.ViewDownloadAccessibility.NSv23;
            var NSv_NumDownServ = monitoring.Indicators.SpatialDataAndService.NSv_NumDownServ;
            var NSv_NumViewServ = monitoring.Indicators.SpatialDataAndService.NSv_NumViewServ;

            var NSi2 = Divide(NSv23, NSv_NumDownServ + NSv_NumViewServ);

            NSi2.Should().Be(0.5);
        }


        [Fact]
        // Antall Annex1-3 datasett som er tilgjengelig gjennom view service (Alle inspiredata som har WMSstatus= god eller brukbar)
        public void NSv21()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsAvailableThroughViewService = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.ViewDownloadAccessibilityIndicators.ViewDownloadAccessibility.NSv21.Should().Be(2);
        }

        [Fact]
        // Antall Annex1-3 datasett som er tilgjengelig gjennom download service service (Alle registerdata som har WFSstatus= god eller brukbar)
        public void NSv22()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsAvailableThroughDownloadService = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.ViewDownloadAccessibilityIndicators.ViewDownloadAccessibility.NSv22.Should().Be(2);
        }

        [Fact]
        // Antall Annex1-3 datasett som er tilgjengelig gjennom view OG download service  (Alle registerdata som har WFSstatus OG WMSstatus = god eller brukbar)
        public void NSv23()
        {
            var register = new Mock<Models.Register>();
            _inspireMonitoring.NumberOfDatasetsAvailableThroughViewANDDownloadService = 2;
            Monitoring monitoring = _inpsireMonitoringService.GetInspireMonitoringReport(register.Object, _inspireMonitoring);
            monitoring.Indicators.ViewDownloadAccessibilityIndicators.ViewDownloadAccessibility.NSv23.Should().Be(2);
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
