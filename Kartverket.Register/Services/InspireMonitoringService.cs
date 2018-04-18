using System;
using System.Collections.Generic;
using System.Globalization;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Models;
using Kartverket.Register.Services.Register;
using DateTime = System.DateTime;

namespace Kartverket.Register.Services
{
    public class InspireMonitoringService : IInspireMonitoringService
    {
        private Models.Register _inspireRegister;
        private ICollection<RegisterItemV2> _inspireItems;

        public int _NumberOfDatasetsByAnnex;
        public int _NumberOfDatasetsByAnnexI;
        public int _NumberOfDatasetsByAnnexII;
        public int _NumberOfDatasetsByAnnexIII;

        public int _NumberOfDatasetsByAnnexIWithMetadata;
        public int _NumberOfDatasetsByAnnexIIWithMetadata;
        public int _NumberOfDatasetsByAnnexIIIWithMetadata;

        public int _NumberOfDatasetsWithMetadata;
        public int _NumberOfServicesWithMetadata;

        public int _NumberOfDatasetsRegisteredInADiscoveryService;
        public int _NumberOfServicesRegisteredInADiscoveryService;

        public int _NumberOfServicesByServiceTypeDownload;
        public int _NumberOfServicesByServiceTypeView;
        public int _NumberOfServicesByServiceTypeDiscovery;
        public int _NumberOfServicesByServiceTypeInvoke;
        public int _NumberOfServicesByServiceTypeTransformation;
        public int _NumberOfServicesByServiceType;
        public int _NumberOfSdS;

        public int _NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue;
        public int _NumberOfServicesByServiceTypeViewWhereConformityIsTrue;
        public int _NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue;
        public int _NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue;
        public int _NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue;
        public int _NumberOfServicesByServiceTypeWhereConformityIsTrue;

        public int _NumberOfCallsByServiceTypeDiscovery;

        public int _NumberOfCallsByServiceTypeView;
        public int _NumberOfCallsByServiceTypeDownload;
        public int _NumberOfCallsByServiceTypeTransformation;
        public int _NumberOfCallsByServiceTypeInvoke;
        public int _NumberOfCallsByServiceType;

        public int _NumberOfDatasetsAvailableThroughViewOrDownloadService;
        public int _NumberOfDatasetsAvailableThroughDownloadService;
        public int _NumberOfDatasetsAvailableThroughViewService;

        private int _NumberOfDatasetsWithHarmonizedDataAndConformedMetadata;
        private int _NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata;
        private int _NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata;
        private int _NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata;

        private double _AccumulatedCurrentAreaByAnnexI;
        private double _AccumulatedCurrentAreaByAnnexII;
        private double _AccumulatedCurrentAreaByAnnexIII;

        public double _AccumulatedRelevantAreaByAnnexI;
        public double _AccumulatedRelevantAreaByAnnexII;
        public double _AccumulatedRelevantAreaByAnnexIII;


        public Monitoring GetInspireMonitoringReport(Models.Register inspireRegister)
        {
            SetClassVariables(inspireRegister);
            return Mapping();
        }

        private void SetClassVariables(Models.Register inspireRegister)
        {
            _inspireRegister = inspireRegister;
            _inspireItems = _inspireRegister?.RegisterItems;

            _NumberOfDatasetsByAnnex = NumberOfDatasetsByAnnex();
            _NumberOfDatasetsByAnnexI = NumberOfDatasetsByAnnexI();
            _NumberOfDatasetsByAnnexII = NumberOfDatasetsByAnnexII();
            _NumberOfDatasetsByAnnexIII = NumberOfDatasetsByAnnexIII();

            _NumberOfDatasetsByAnnexIWithMetadata = NumberOfDatasetsByAnnexIWithMetadata();
            _NumberOfDatasetsByAnnexIIWithMetadata = NumberOfDatasetsByAnnexIIWithMetadata();
            _NumberOfDatasetsByAnnexIIIWithMetadata = NumberOfDatasetsByAnnexIIIWithMetadata();

            _NumberOfDatasetsWithMetadata = NumberOfDatasetsWithMetadata();
            _NumberOfServicesWithMetadata = NumberOfServicesWithMetadata();

            _NumberOfDatasetsRegisteredInADiscoveryService = NumberOfDatasetsRegisteredInADiscoveryService();
            _NumberOfServicesRegisteredInADiscoveryService = NumberOfServicesRegisteredInADiscoveryService();

            _NumberOfServicesByServiceTypeDownload = NumberOfServicesByServiceType("download");
            _NumberOfServicesByServiceTypeView = NumberOfServicesByServiceType("view");
            _NumberOfServicesByServiceTypeDiscovery = NumberOfServicesByServiceType("discovery");
            _NumberOfServicesByServiceTypeInvoke = NumberOfServicesByServiceType("invoke");
            _NumberOfServicesByServiceTypeTransformation = NumberOfServicesByServiceType("transformation");
            _NumberOfServicesByServiceType = NumberOfServicesByServiceType();
            _NumberOfSdS = NumberOfSdS();

            _NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue = NumberOfServicesByServiceTypeWhereConformityIsTrue("download");
            _NumberOfServicesByServiceTypeViewWhereConformityIsTrue = NumberOfServicesByServiceTypeWhereConformityIsTrue("view");
            _NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue = NumberOfServicesByServiceTypeWhereConformityIsTrue("discovery");
            _NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue = NumberOfServicesByServiceTypeWhereConformityIsTrue("invoke");
            _NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue = NumberOfServicesByServiceTypeWhereConformityIsTrue("transformation");
            _NumberOfServicesByServiceTypeWhereConformityIsTrue = NumberOfServicesByServiceTypeWhereConformityIsTrue();

            _NumberOfCallsByServiceTypeDiscovery = NumberOfCallsByServiceTypeDiscovery();
            _NumberOfCallsByServiceTypeView = NumberOfCallsByServiceTypeView();
            _NumberOfCallsByServiceTypeDownload = NumberOfCallsByServiceTypeDownload();
            _NumberOfCallsByServiceTypeTransformation = NumberOfCallsByServiceTypeTransformation();
            _NumberOfCallsByServiceTypeInvoke = NumberOfCallsByServiceTypeInvoke();
            _NumberOfCallsByServiceType = NumberOfCallsByServiceType();

            _NumberOfDatasetsAvailableThroughViewOrDownloadService = NumberOfDatasetsAvailableThroughViewOrDownloadService();
            _NumberOfDatasetsAvailableThroughDownloadService = NumberOfDatasetsAvailableThroughDownloadService();
            _NumberOfDatasetsAvailableThroughViewService = NumberOfDatasetsAvailableThroughViewService();

            _NumberOfDatasetsWithHarmonizedDataAndConformedMetadata = NumberOfDatasetsWithHarmonizedDataAndConformedMetadata();
            _NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata = NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata();
            _NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata = NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata();
            _NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata = NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata();

            _AccumulatedCurrentAreaByAnnexI = AccumulatedCurrentAreaByAnnexI();
            _AccumulatedCurrentAreaByAnnexII = AccumulatedCurrentAreaByAnnexII();
            _AccumulatedCurrentAreaByAnnexIII = AccumulatedCurrentAreaByAnnexIII();

            _AccumulatedRelevantAreaByAnnexI = AccumulatedRelevantAreaByAnnexI();
            _AccumulatedRelevantAreaByAnnexII = AccumulatedRelevantAreaByAnnexII();
            _AccumulatedRelevantAreaByAnnexIII = AccumulatedRelevantAreaByAnnexIII();
        }


        public Monitoring Mapping()
        {
            var monitoring = new Monitoring();
            monitoring.documentYear = GetReportingDate();
            monitoring.memberState = CountryCode.NO;
            monitoring.MonitoringMD = MappingMonitoringMd();
            monitoring.RowData = MappingRowData();
            monitoring.Indicators = MappingIndicators();

            return monitoring;
        }


        private Indicators MappingIndicators()
        {
            Indicators indicators = new Indicators();

            indicators.NnConformityIndicators = GetNnConformityIndicators();
            indicators.GeoCoverageIndicators = GetGeoCoverageIndicators();
            indicators.UseNNindicators = GetUseNNindicators();
            indicators.MetadataExistenceIndicators = GetMetadataExsistensIndicators();
            indicators.DiscoveryMetadataIndicators = GetDiscoveryMetadataIndicators();
            indicators.ViewDownloadAccessibilityIndicators = GetViewDownloadAccessibilityIndicators();
            indicators.SpatialDataAndService = GetSpatialDataAndService();
            indicators.MetadataConformityIndicators = new MetadataConformityIndicators();
            indicators.SdsConformantIndicators = GetSdsConformantIndicators();

            return indicators;
        }

        private NnConformityIndicators GetNnConformityIndicators()
        {
            NnConformityIndicators nnConformityIndicators = new NnConformityIndicators();
            nnConformityIndicators.NSi41 = ProportionOfServicesByServiceTypeDiscoveryWhereConformityIsTrue(); // Andel av tjenester NnServiceType="discovery" som har nnConformity="true"(<NSv41>/<NSv_NumDiscServ>)
            nnConformityIndicators.NSi42 = ProportionOfServicesByServiceTypeViewWhereConformityIsTrue(); // Andel av tjenester NnServiceType="view" som har nnConformity="true" (<NSv42>/<NSv_NumViewServ>)
            nnConformityIndicators.NSi43 = ProportionOfServicesByServiceTypeDownloadWhereConformityIsTrue(); // Andel av tjenester NnServiceType="download" som har nnConformity="true" (<NSv43>/<NSv_NumDownServ>)
            nnConformityIndicators.NSi44 = ProportionOfServicesByServiceTypeTransformationWhereConformityIsTrue(); // Andel av tjenester NnServiceType="transformation" som har nnConformity="true" (<NSv44>/<NSv_NumTransfServ>)
            nnConformityIndicators.NSi45 = ProportionOfServicesByServiceTypeInvokeWhereConformityIsTrue(); // Andel av tjenester NnServiceType="invoke" som har nnConformity="true" (<NSv45>/<NSv_NumInvkServ>)
            nnConformityIndicators.NSi4 = ProportionOfServicesWhereConformityIsTrue(); // Andel av tjenester NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true" (<NSv4>/<NSv_NumAllServ>)
            nnConformityIndicators.NnConformity = GetNnConformity();

            return nnConformityIndicators;
        }

        private NnConformity GetNnConformity()
        {
            NnConformity nnConformity = new NnConformity();
            nnConformity.NSv41 = _NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="discovery" som har nnConformity="true")
            nnConformity.NSv41 = _NumberOfServicesByServiceTypeViewWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="view" som har nnConformity="true")
            nnConformity.NSv41 = _NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="download" som har nnConformity="true")
            nnConformity.NSv41 = _NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="transformation" som har nnConformity="true")
            nnConformity.NSv41 = _NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="invoke" som har nnConformity="true")
            nnConformity.NSv4 = _NumberOfServicesByServiceTypeWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true")

            return nnConformity;
        }

        private GeoCoverageIndicators GetGeoCoverageIndicators()
        {
            GeoCoverageIndicators geoCoverageIndicators = new GeoCoverageIndicators();
            geoCoverageIndicators.DSi11 = 0; // <DSv11_ActArea>/<DSv11_RelArea>
            geoCoverageIndicators.DSi12 = 0; // <DSv12_ActArea>/<DSv12_RelArea>
            geoCoverageIndicators.DSi13 = 0; // <DSv13_ActArea>/<DSv13_RelArea>
            geoCoverageIndicators.DSi1 = 0; // <DSv1_ActArea>/<DSv1_RelArea> 

            geoCoverageIndicators.GeoCoverageSDS = GetGeoCoverageSDS();

            return geoCoverageIndicators;
        }

        private GeoCoverageSDS GetGeoCoverageSDS()
        {
            GeoCoverageSDS geoCoverageSDS = new GeoCoverageSDS();
            geoCoverageSDS.DSv11_ActArea = _AccumulatedCurrentAreaByAnnexI; // Akkumulert aktuelt areal (<actualArea>) for alle Annex1 data
            geoCoverageSDS.DSv12_ActArea = _AccumulatedCurrentAreaByAnnexII; // Akkumulert aktuelt areal (<actualArea>) for alle Annex2 data
            geoCoverageSDS.DSv13_ActArea = _AccumulatedCurrentAreaByAnnexIII; // Akkumulert aktuelt areal (<actualArea>) for alle Annex3 data
            geoCoverageSDS.DSv1_ActArea = AccumulatedCurrentAreaByAnnex(); // Akkumulert aktuelt areal (<actualArea>) for alle Annex1, Annex2 og Annex3 data
            geoCoverageSDS.DSv11_RelArea = _AccumulatedRelevantAreaByAnnexI; // Akkumulert relevant areal (<relevantArea>) for alle Annex1 data
            geoCoverageSDS.DSv12_RelArea = _AccumulatedRelevantAreaByAnnexII; // Akkumulert relevant areal (<relevantArea>) for alle Annex2 data
            geoCoverageSDS.DSv13_RelArea = _AccumulatedRelevantAreaByAnnexIII; // Akkumulert relevant areal (<relevantArea>) for alle Annex3 data
            geoCoverageSDS.DSv1_RelArea = AccumulatedRelevantAreaByAnnex(); // Akkumulert relevant areal (<relevantArea>) for alle Annex1, Annex2 og Annex3 data
            return geoCoverageSDS;
        }



        private UseNNindicators GetUseNNindicators()
        {
            UseNNindicators useNNindicators = new UseNNindicators();
            useNNindicators.NSi31 = AverageNumberOfCallsByServiceTypeDiscovery(); // Gjennomsnittlig antall kall for NnServiceType="discovery" (<NSv31>/<NSv_NumDiscServ>)
            useNNindicators.NSi32 = AverageNumberOfCallsByServiceTypeView(); // Gjennomsnittlig antall kall for NnServiceType="view"(<NSv32>/<NSv_NumViewServ>)
            useNNindicators.NSi33 = AverageNumberOfCallsByServiceTypeDownload(); // Gjennomsnittlig antall kall for NnServiceType="download" (<NSv33>/<NSv_NumDownServ>)
            useNNindicators.NSi34 = AverageNumberOfCallsByServiceTypeTransformation(); // Gjennomsnittlig antall kall for NnServiceType="transformation" (<NSv34>/<NSv_NumTransfServ>)
            useNNindicators.NSi35 = AverageNumberOfCallsByServiceTypeInvoke(); // Gjennomsnittlig antall kall for NnServiceType="invoke" (<NSv35>/<NSv_NumInvkServ>)
            useNNindicators.NSi3 = AverageNumberOfCallsByServiceType(); //Gjennomsnittlig antall kall for NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true" (<NSv3>/<NSv_NumAllServ>) -->
            useNNindicators.UseNN = GetUseNN();

            return useNNindicators;
        }

        private UseNN GetUseNN()
        {
            UseNN useNN = new UseNN();
            useNN.NSv31 = _NumberOfCallsByServiceTypeDiscovery; // Akkumulert antall kall for alle NnServiceType="discovery" 
            useNN.NSv32 = _NumberOfCallsByServiceTypeView; // Akkumulert antall kall for alle NnServiceType="view"
            useNN.NSv33 = _NumberOfCallsByServiceTypeDownload; // Akkumulert antall kall for alle NnServiceType="download"
            useNN.NSv34 = _NumberOfCallsByServiceTypeTransformation; // Akkumulert antall kall for alle NnServiceType="transformation"
            useNN.NSv35 = _NumberOfCallsByServiceTypeInvoke; // Akkumulert antall kall for alle NnServiceType="invoke"
            useNN.NSv3 = _NumberOfCallsByServiceType; // Akkumulert antall kall for alle NnServiceType="discovery + view + download + transformation + invoke"

            return useNN;
        }

        private MetadataExistenceIndicators GetMetadataExsistensIndicators()
        {
            MetadataExistenceIndicators metadataExistenceIndicators = new MetadataExistenceIndicators();
            metadataExistenceIndicators.MDi11 = ProportionOfDatasetsWithMetadataByAnnexI();  // Andel datasett som har metadata av Annex1 data (<MDv11>/<DSv_Num1>)
            metadataExistenceIndicators.MDi12 = ProportionOfDatasetsWithMetadataByAnnexII(); // Andel datasett som har metadata av Annex2 data (<MDv12>/<DSv_Num2>)
            metadataExistenceIndicators.MDi13 = ProportionOfDatasetsWithMetadataByAnnexIII(); // Andel datasett som har metadata av Annex3 data (<MDv13>/<DSv_Num3>)
            metadataExistenceIndicators.MDi14 = ProportionOfServicesWithMetadata(); // Andel tjenester som har metadata (<MDv1_DS>/<NSv_NumAllServ>)
            metadataExistenceIndicators.MDi1 = ProportionOfDatasetsWithMetadata(); // Andel datasett som har metadata

            metadataExistenceIndicators.MetadataExistence = GetMetadataExistence();

            return metadataExistenceIndicators;
        }

        private MetadataExistence GetMetadataExistence()
        {
            MetadataExistence metadataExistence = new MetadataExistence();
            metadataExistence.MDv11 = _NumberOfDatasetsByAnnexIWithMetadata; // Antall Annex1 datasett som har metadata (Alle Annex1 datasett)
            metadataExistence.MDv12 = _NumberOfDatasetsByAnnexIIWithMetadata; // Antall Annex2 datasett som har metadata (Alle Annex2 datasett)
            metadataExistence.MDv13 = _NumberOfDatasetsByAnnexIIIWithMetadata; // Antall Annex3 datasett som har metadata (Alle Annex3 datasett)
            metadataExistence.MDv1_DS = _NumberOfDatasetsWithMetadata; // Antall datasett som har metadata (Alle datasett)
            metadataExistence.MDv14 = _NumberOfServicesWithMetadata; // Antall tjenester som har metadata (Alle tjenester)
            return metadataExistence;
        }


    
        private DiscoveryMetadataIndicators GetDiscoveryMetadataIndicators()
        {
            DiscoveryMetadataIndicators discoveryMetadataIndicators = new DiscoveryMetadataIndicators();
            discoveryMetadataIndicators.NSi11 = ProportionOfDatasetsRegisteredInADiscoveryService(); // Andel datasett som er registrert i en discovery service (<NSv11>/<DSv_Num>)
            discoveryMetadataIndicators.NSi12 = ProportionOfServicesRegisteredInADiscoveryService(); // Andel tjenester som er registrert i en discovery service (<NSv11>/<DSv_Num>)
            discoveryMetadataIndicators.NSi1 = ProportionOfServicesAndDatasetsRegisteredInADiscoveryService(); // Andel datasett og tjenester som er registrert i en discovery service (<NSv11>+<NSv12>/<DSv_Num>+<NSv_NumAllServ>)
            discoveryMetadataIndicators.DiscoveryMetadata = GetDiscoveryMetadata();

            return discoveryMetadataIndicators;
        }


        private DiscoveryMetadata GetDiscoveryMetadata()
        {
            DiscoveryMetadata discoveryMetadata = new DiscoveryMetadata();
            discoveryMetadata.NSv11 = _NumberOfDatasetsRegisteredInADiscoveryService; // Antall datasett som er registrert i en discovery service (Alle datasett)
            discoveryMetadata.NSv12 = _NumberOfServicesRegisteredInADiscoveryService; // Antall tjenester som er registrert i en discovery service (Alle tjenester)
            return discoveryMetadata;
        }

        private ViewDownloadAccessibilityIndicators GetViewDownloadAccessibilityIndicators()
        {
            ViewDownloadAccessibilityIndicators viewDownloadAccessibilityIndicators = new ViewDownloadAccessibilityIndicators();
            viewDownloadAccessibilityIndicators.NSi21 = ProportionOfDatasetsAvailableThroughViewService(); // Andel Annex1-3 datasett som er tilgjengelig gjennom view service (<NSv21>/<NSv_NumViewServ>)
            viewDownloadAccessibilityIndicators.NSi22 = ProportionOfDatasetsAvailableThroughDownloadService(); // Andel Annex1-3 datasett som er tilgjengelig gjennom download service (<NSv22>/<NSv_NumDownServ>)
            viewDownloadAccessibilityIndicators.NSi2 = ProportionOfDatasetsAvailableThroughViewAndDownloadService(); // Andel Annex1-3 datasett som er tilgjengelig gjennom view OG download service (<NSv23>/<NSv_NumViewServ>+<NSv_NumDownServ>)
            viewDownloadAccessibilityIndicators.ViewDownloadAccessibility = GetViewDownloadAccessibility();

            return viewDownloadAccessibilityIndicators;
        }

        private ViewDownloadAccessibility GetViewDownloadAccessibility()
        {
            ViewDownloadAccessibility viewDownloadAccessibility = new ViewDownloadAccessibility();
            viewDownloadAccessibility.NSv21 = _NumberOfDatasetsAvailableThroughViewService; // Antall Annex1-3 datasett som er tilgjengelig gjennom view service (Alle inspiredata som har WMSstatus= god eller brukbar)
            viewDownloadAccessibility.NSv22 = _NumberOfDatasetsAvailableThroughDownloadService; // Antall Annex1-3 datasett som er tilgjengelig gjennom download service service (Alle registerdata som har WFSstatus= god eller brukbar)
            viewDownloadAccessibility.NSv23 = _NumberOfDatasetsAvailableThroughViewOrDownloadService; // Antall Annex1-3 datasett som er tilgjengelig gjennom view OG download service  (Alle registerdata som har WFSstatus OG WMSstatus = god eller brukbar)

            return viewDownloadAccessibility;
        }

        private SpatialDataAndService GetSpatialDataAndService()
        {
            SpatialDataAndService spatialDataAndService = new SpatialDataAndService();

            spatialDataAndService.DSv_Num1 = _NumberOfDatasetsByAnnexI; // Totalt antall datasett for annex1 (<Antall <SpatialDataSet> som har <AnnexI> )
            spatialDataAndService.DSv_Num2 = _NumberOfDatasetsByAnnexII; // Totalt antall datasett for  annex2 (<Antall <SpatialDataSet> som har <AnnexII> )
            spatialDataAndService.DSv_Num3 = _NumberOfDatasetsByAnnexIII; // Totalt antall datasett for  annex3 (<Antall <SpatialDataSet> som har <AnnexIII> )
            spatialDataAndService.DSv_Num = _NumberOfDatasetsByAnnex; // Totalt antall datasett for alle annex (<DSv_Num1>+<DSv_Num2>+<DSv_Num3> )
            spatialDataAndService.SDSv_Num = _NumberOfSdS; // Totalt antall tjenester SDS
            spatialDataAndService.NSv_NumDiscServ = _NumberOfServicesByServiceTypeDiscovery; // Antall NnServiceType="discovery"
            spatialDataAndService.NSv_NumViewServ = _NumberOfServicesByServiceTypeView; // Antall NnServiceType="view"
            spatialDataAndService.NSv_NumDownServ = _NumberOfServicesByServiceTypeDownload; // Antall NnServiceType="download"
            spatialDataAndService.NSv_NumInvkServ = _NumberOfServicesByServiceTypeInvoke; // Antall NnServiceType="invoke"
            spatialDataAndService.NSv_NumAllServ = _NumberOfServicesByServiceType; // Antall NnServiceType="discovery + view + download + transformation + invoke" (Network services)
            spatialDataAndService.NSv_NumTransfServ = _NumberOfServicesByServiceTypeTransformation; // Antall NnServiceType="transformation" 

            return spatialDataAndService;
        }

        private SdsConformantIndicators GetSdsConformantIndicators()
        {
            SdsConformantIndicators sdsConformantIndicators = new SdsConformantIndicators();
            sdsConformantIndicators.DSi21 = ProportionOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata(); // Andel datasett fra Annex1 med konforme metadata og harmoniserte datasett (<DSv21>/<DSv_Num1>)
            sdsConformantIndicators.DSi22 = ProportionOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata(); // Andel datasett fra Annex2 med konforme metadata og harmoniserte datasett (<DSv22>/<DSv_Num2>)
            sdsConformantIndicators.DSi23 = ProportionOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata(); // Andel datasett fra Annex3 med konforme metadata og harmoniserte datasett (<DSv23>/<DSv_Num3>)
            sdsConformantIndicators.DSi2 = ProportionOfDatasetWithHarmonizedDataAndConformedMetadata(); // Andel datasettt fra Annex1,2,3 med konforme metadata og harmoniserte datasett (<DSv2>/<DSv_Num>)
            sdsConformantIndicators.SdsConformant = GetSdsConformant();

            return sdsConformantIndicators;
        }

        private SdsConformant GetSdsConformant()
        {
            SdsConformant sdsConformant = new SdsConformant();
            sdsConformant.DSv21 = _NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata; // Antall datasett fra Annex1 med harmoniserte data og konforme metadata (Dtasett where <AnnexI>:  Harmoniserte data="God" OG Metadatastatus = "God"= )
            sdsConformant.DSv22 = _NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata; // Antall datasett fra Annex2 med harmoniserte data og konforme metadata (Dtasett where <AnnexII>:  Harmoniserte data="God" OG Metadatastatus = "God"= )
            sdsConformant.DSv23 = _NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata; // Antall datasett fra Annex3 med harmoniserte data og konforme metadata (Dtasett where <AnnexIII>:  Harmoniserte data="God" OG Metadatastatus = "God"= )
            sdsConformant.DSv2 = _NumberOfDatasetsWithHarmonizedDataAndConformedMetadata; // (Dtasett where <AnnexI> or <AnnexII> or <AnnexIII>:  Harmoniserte data="God" OG Metadatastatus = "God"= )
            return sdsConformant;
        }




















        private MonitoringMD MappingMonitoringMd()
        {
            var monitoringMd = new MonitoringMD();
            monitoringMd.organizationName = _inspireRegister.owner.name;
            monitoringMd.email = "post@norgedigitalt.no";
            monitoringMd.language = LanguageCode.nor;
            //monitoringMd.monitoringDate = new Date();
            return monitoringMd;
        }

        private RowData MappingRowData()
        {
            var rowData = new RowData();
            rowData.SpatialDataSet = MappingSpatialDataSets();
            rowData.SpatialDataService = MappingSpatialDataServices();
            return rowData;
        }

        private SpatialDataService[] MappingSpatialDataServices()
        {
            List<SpatialDataService> spatialDataServices = new List<SpatialDataService>();

            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    spatialDataServices.Add(MappingSpatialDataService(inspireDataService));
                }
            }
            return spatialDataServices.ToArray();
        }

        private SpatialDataService MappingSpatialDataService(InspireDataService inspireDataService)
        {
            var spatialDataService = new SpatialDataService();
            spatialDataService.name = inspireDataService.Name;
            spatialDataService.respAuthority = inspireDataService.Owner.name;
            spatialDataService.uuid = inspireDataService.Uuid;
            //spatialDataService.Themes = MappingThemes(services.InspireTheme);
            //spatialDataService.MdServiceExistence = MappingServiceExistence(services);
            //spatialDataService.NetworkService = MappingNetworkService(services);
            return spatialDataService;
        }

        private SpatialDataSet[] MappingSpatialDataSets()
        {
            List<SpatialDataSet> spatialDataSetList = new List<SpatialDataSet>();

            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    spatialDataSetList.Add(MappingSpatialDataSet(inspireDataset));
                }
            }
            return spatialDataSetList.ToArray();
        }


        //private NetworkService MappingNetworkService(InspireDataset item)
        //{
        //    NetworkService networkService = new NetworkService();
        //    networkService.nnConformity = false; // TODO skal hentes fra registeret, men registeret må få strengere regler på tjenestestatus 
        //    networkService.NnServiceType = MappingNnServiceType(""); // TODO Protocoll... 
        //    return networkService;
        //}

        //private NnServiceType MappingNnServiceType(string protocol)
        //{
        //    switch (protocol)
        //    {
        //        case "OGC:WMS":
        //            return NnServiceType.view;
        //        case "OGC:WFS":
        //            return NnServiceType.view;
        //        case "OGC:WCS":
        //            return NnServiceType.download;
        //        case "OGC:CSW":
        //            return NnServiceType.discovery;
        //    }
        //    return new NnServiceType();
        //}

        //private MdServiceExistence MappingServiceExistence(InspireDataset item)
        //{
        //    MdServiceExistence mdServiceExistence = new MdServiceExistence();
        //    mdServiceExistence.mdConformity = Accessibility(item.InspireDeliveryMetadata.StatusId);
        //    mdServiceExistence.discoveryAccessibility = Accessibility(item.InspireDeliveryMetadataService.StatusId);

        //    return mdServiceExistence;
        //}

        public SpatialDataSet MappingSpatialDataSet(InspireDataset inspireDataset)
        {
            var spatialDataset = new SpatialDataSet();
            spatialDataset.name = inspireDataset.Name;
            spatialDataset.respAuthority = inspireDataset.Owner.shortname;
            spatialDataset.uuid = inspireDataset.SystemId.ToString();
            spatialDataset.Themes = GetThemes(inspireDataset.InspireThemes);
            spatialDataset.Coverage = MappingCoverage();
            spatialDataset.MdDataSetExistence = MappingMdDatasetEcistence(inspireDataset);
            return spatialDataset;
        }

        private MdDataSetExistence MappingMdDatasetEcistence(InspireDataset inspireDataset)
        {
            MdDataSetExistence mdDataSetExistence = new MdDataSetExistence();
            mdDataSetExistence.MdAccessibility = MappingMdAccessibility(inspireDataset);

            return mdDataSetExistence;
        }

        private MdAccessibility MappingMdAccessibility(InspireDataset inspireDataset)
        {
            MdAccessibility mdAccessibility = new MdAccessibility();
            mdAccessibility.discovery = true;
            mdAccessibility.view = inspireDataset.WmsAndWfsIsGoodOrUseable();
            mdAccessibility.download = inspireDataset.WfsIsGoodOrUseable();

            return mdAccessibility;
        }


        private Coverage MappingCoverage()
        {
            Coverage coverage = new Coverage();
            coverage.actualArea = 323; // TODO, Default verdi.. 
            coverage.relevantArea = 323; // TODO, Default verdi.. 
            return coverage;
        }

        private Date GetReportingDate()
        {
            Date date = new Date();
            date.day = DateTime.Now.Day.ToString();
            date.month = DateTime.Now.Month.ToString();
            date.year = DateTime.Now.Year.ToString();
            return date;
        }







        private bool InspireDatasetHaveInspireThemeOfTypeAnnexI(ICollection<CodelistValue> inspireThems)
        {
            try
            {
                foreach (var inspireTheme in inspireThems)
                {
                    if (IsAnnexI(inspireTheme))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        private bool InspireDatasetHaveInspireThemeOfTypeAnnexII(ICollection<CodelistValue> inspireThems)
        {
            foreach (var inspireTheme in inspireThems)
            {
                if (IsAnnexII(inspireTheme))
                {
                    return true;
                }
            }
            return false;
        }

        private bool InspireDatasetHaveInspireThemeOfTypeAnnexIII(ICollection<CodelistValue> inspireThems)
        {
            foreach (var inspireTheme in inspireThems)
            {
                if (IsAnnexIII(inspireTheme))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsAnnexI(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(AnnexI), inspireThemeValueCamelCase);
            }
            return false;
        }

        private static bool IsAnnexII(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(AnnexII), inspireThemeValueCamelCase);
            }
            return false;
        }

        private static bool IsAnnexIII(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(AnnexIII), inspireThemeValueCamelCase);
            }
            return false;
        }


        private static string CreateCamelCase(string inspireTheme)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            var inspireThemeCamelCase = textInfo.ToTitleCase(inspireTheme);
            inspireThemeCamelCase = Char.ToLowerInvariant(inspireThemeCamelCase[0]) + inspireThemeCamelCase.Substring(1);
            inspireThemeCamelCase = inspireThemeCamelCase.Replace(" ", "");

            return inspireThemeCamelCase;
        }

        private Themes[] GetThemes(ICollection<CodelistValue> inspireThemes)
        {
            var themes = new Themes();
            List<AnnexI> annexiListI = new List<AnnexI>();
            List<AnnexII> annexiListII = new List<AnnexII>();
            List<AnnexIII> annexiListIII = new List<AnnexIII>();

            foreach (var inspireTheme in inspireThemes)
            {
                annexiListI = AnnexIList(inspireTheme);
                annexiListII = AnnexIIList(inspireTheme);
                annexiListIII = AnnexIIIList(inspireTheme);
            }

            themes.AnnexI = annexiListI.ToArray();
            themes.AnnexII = annexiListII.ToArray();
            themes.AnnexIII = annexiListIII.ToArray();

            var themesArray = new[] { themes };

            return themesArray;
        }

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


        private List<AnnexI> AnnexIList(CodelistValue inspireTheme)
        {
            var annexIList = new List<AnnexI>();
            if (inspireTheme != null)
            {
                if (IsAnnexI(inspireTheme))
                {
                    AnnexI annexI = GetAnnexIByInspireTheme(inspireTheme.value);
                    annexIList.Add(annexI);
                }
            }
            return annexIList;
        }

        private List<AnnexII> AnnexIIList(CodelistValue inspireTheme)
        {
            var annexIIList = new List<AnnexII>();
            if (inspireTheme != null)
            {
                if (IsAnnexII(inspireTheme))
                {
                    AnnexII annexII = GetAnnexIIByInspireTheme(inspireTheme.value);
                    annexIIList.Add(annexII);
                }
            }
            return annexIIList;
        }

        private List<AnnexIII> AnnexIIIList(CodelistValue inspireTheme)
        {
            var annexIIIList = new List<AnnexIII>();
            if (inspireTheme != null)
            {
                if (IsAnnexIII(inspireTheme))
                {
                    AnnexIII annexIII = GetAnnexIIIByInspireTheme(inspireTheme.value);
                    annexIIIList.Add(annexIII);
                }
            }
            return annexIIIList;
        }


        private static AnnexI GetAnnexIByInspireTheme(string inspireTheme)
        {
            var inspireThemeCamelCase = CreateCamelCase(inspireTheme);
            return (AnnexI)Enum.Parse(typeof(AnnexI), inspireThemeCamelCase);
        }

        private static AnnexII GetAnnexIIByInspireTheme(string inspireTheme)
        {
            var inspireThemeCamelCase = CreateCamelCase(inspireTheme);
            return (AnnexII)Enum.Parse(typeof(AnnexII), inspireThemeCamelCase);
        }

        private static AnnexIII GetAnnexIIIByInspireTheme(string inspireTheme)
        {
            var inspireThemeCamelCase = CreateCamelCase(inspireTheme);
            return (AnnexIII)Enum.Parse(typeof(AnnexIII), inspireThemeCamelCase);
        }




        // **** Number of... ***

        private int NumberOfServicesRegisteredInADiscoveryService()
        {
            int number = 0;

            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    number++;
                }
            }
            return number;
        }

        private int NumberOfDatasetsRegisteredInADiscoveryService()
        {
            int number = 0;

            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    number++;
                }
            }
            return number;
        }

        private int NumberOfServicesByServiceTypeWhereConformityIsTrue(string serviceType = null)
        {
            int number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.Conformity)
                    {
                        if (string.IsNullOrWhiteSpace(serviceType))
                        {
                            number++;
                        }
                        else
                        {
                            if (inspireDataService.ServiceType == serviceType)
                            {
                                number++;
                            }
                        }
                    }
                }
            }
            return number;
        }

        private int NumberOfServicesWithMetadata()
        {
            var number = 0;

            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.MetadataIsGoodOrDeficent())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsWithMetadata()
        {
            var number = 0;

            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (inspireDataset.MetadataIsGoodOrDeficent())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsByAnnexIWithMetadata()
        {
            var number = 0;

            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexI(inspireDataset.InspireThemes))
                    {
                        if (inspireDataset.MetadataIsGoodOrDeficent())
                        {
                            number++;
                        }
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsByAnnexIIWithMetadata()
        {
            var number = 0;

            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexII(inspireDataset.InspireThemes))
                    {
                        if (inspireDataset.MetadataIsGoodOrDeficent())
                        {
                            number++;
                        }
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsByAnnexIIIWithMetadata()
        {
            var number = 0;

            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(inspireDataset.InspireThemes))
                    {
                        if (inspireDataset.MetadataIsGoodOrDeficent())
                        {
                            number++;
                        }
                    }
                }
            }
            return number;
        }

        private int NumberOfCallsByServiceType()
        {
            int number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    number += inspireDataService.Requests;
                }
            }
            return number;
        }

        private int NumberOfCallsByServiceTypeDownload()
        {
            int number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.ServiceType == "download")
                    {
                        number += inspireDataService.Requests;
                    }
                }
            }
            return number;
        }

        private int NumberOfCallsByServiceTypeDiscovery()
        {
            int number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.ServiceType == "discovery")
                    {
                        number += inspireDataService.Requests;
                    }
                }
            }
            return number;
        }

        private int NumberOfCallsByServiceTypeView()
        {
            int number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.ServiceType == "view")
                    {
                        number += inspireDataService.Requests;
                    }
                }
            }
            return number;
        }

        private int NumberOfCallsByServiceTypeTransformation()
        {
            int number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.ServiceType == "transformation")
                    {
                        number += inspireDataService.Requests;
                    }
                }
            }
            return number;
        }

        private int NumberOfCallsByServiceTypeInvoke()
        {
            int number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.ServiceType == "invoke")
                    {
                        number += inspireDataService.Requests;
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsByAnnexIII()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(inspireDataset.InspireThemes))
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsByAnnexII()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexII(inspireDataset.InspireThemes))
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsByAnnexI()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexI(inspireDataset.InspireThemes))
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int NumberOfSdS()
        {
            var number = 0;
            foreach (RegisterItemV2 item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.IsSds())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsAvailableThroughViewOrDownloadService()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (inspireDataset.WmsAndWfsIsGoodOrUseable())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsAvailableThroughDownloadService()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (inspireDataset.WfsIsGoodOrUseable())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsAvailableThroughViewService()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (inspireDataset.WmsIsGoodOrUseable())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsWithHarmonizedDataAndConformedMetadata()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (inspireDataset.HarmonizedDataAndConformedmetadata())
                    {
                        number++;
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexI(inspireDataset.InspireThemes))
                    {
                        if (inspireDataset.HarmonizedDataAndConformedmetadata())
                        {
                            number++;
                        }
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexII(inspireDataset.InspireThemes))
                    {
                        if (inspireDataset.HarmonizedDataAndConformedmetadata())
                        {
                            number++;
                        }
                    }
                }
            }
            return number;
        }

        private int NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(inspireDataset.InspireThemes))
                    {
                        if (inspireDataset.HarmonizedDataAndConformedmetadata())
                        {
                            number++;
                        }
                    }
                }
            }
            return number;
        }

        private int NumberOfServicesByServiceType(string serviceType = null)
        {
            int number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (string.IsNullOrWhiteSpace(serviceType))
                    {
                        number++;
                    }
                    else
                    {
                        if (inspireDataService.ServiceType == serviceType)
                        {
                            number++;
                        }
                    }
                }
            }
            return number;
        }



        private double AccumulatedCurrentAreaByAnnex()
        {
            return _AccumulatedCurrentAreaByAnnexI + _AccumulatedCurrentAreaByAnnexII + _AccumulatedCurrentAreaByAnnexIII;
        }

        private double AccumulatedCurrentAreaByAnnexI()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexI(inspireDataset.InspireThemes))
                    {
                        number += inspireDataset.Area;
                    }
                }
            }
            return number;
        }

        private double AccumulatedCurrentAreaByAnnexII()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexII(inspireDataset.InspireThemes))
                    {
                        number += inspireDataset.Area;
                    }
                }
            }
            return number;
        }

        private double AccumulatedCurrentAreaByAnnexIII()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(inspireDataset.InspireThemes))
                    {
                        number += inspireDataset.Area;
                    }
                }
            }
            return number;
        }


        private double AccumulatedRelevantAreaByAnnexI()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexI(inspireDataset.InspireThemes))
                    {
                        number += inspireDataset.RelevantArea;
                    }
                }
            }
            return number;
        }

        private double AccumulatedRelevantAreaByAnnexII()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexII(inspireDataset.InspireThemes))
                    {
                        number += inspireDataset.RelevantArea;
                    }
                }
            }
            return number;
        }

        private double AccumulatedRelevantAreaByAnnexIII()
        {
            var number = 0;
            foreach (var item in _inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(inspireDataset.InspireThemes))
                    {
                        number += inspireDataset.RelevantArea;
                    }
                }
            }
            return number;
        }

        private double AccumulatedRelevantAreaByAnnex()
        {
            return _AccumulatedRelevantAreaByAnnexI + _AccumulatedRelevantAreaByAnnexII + _AccumulatedRelevantAreaByAnnexIII;
        }

        // **** Propotion of... ***


        private double AverageNumberOfCallsByServiceTypeDownload()
        {
            return Divide(_NumberOfCallsByServiceTypeDownload, _NumberOfServicesByServiceTypeDownload);
        }

        private double AverageNumberOfCallsByServiceTypeDiscovery()
        {
            return Divide(_NumberOfCallsByServiceTypeDiscovery, _NumberOfServicesByServiceTypeDiscovery);
        }

        private double AverageNumberOfCallsByServiceTypeView()
        {
            return Divide(_NumberOfCallsByServiceTypeView, _NumberOfServicesByServiceTypeView);
        }

        private double AverageNumberOfCallsByServiceTypeTransformation()
        {
            return Divide(_NumberOfCallsByServiceTypeTransformation, _NumberOfServicesByServiceTypeTransformation);
        }

        private double AverageNumberOfCallsByServiceTypeInvoke()
        {
            return Divide(_NumberOfCallsByServiceTypeInvoke, _NumberOfServicesByServiceTypeInvoke);
        }

        private double AverageNumberOfCallsByServiceType()
        {
            return Divide(_NumberOfCallsByServiceType, _NumberOfServicesByServiceType);
        }

        private double ProportionOfDatasetsRegisteredInADiscoveryService()
        {
            return Divide(_NumberOfDatasetsRegisteredInADiscoveryService, _NumberOfDatasetsByAnnex);
        }

        private double ProportionOfServicesRegisteredInADiscoveryService()
        {
            return Divide(_NumberOfServicesRegisteredInADiscoveryService, _NumberOfServicesByServiceType);
        }


        private double ProportionOfDatasetsWithMetadataByAnnexI()
        {
            return Divide(_NumberOfDatasetsByAnnexIWithMetadata, _NumberOfDatasetsByAnnexI);
        }

        private double ProportionOfDatasetsWithMetadataByAnnexII()
        {
            return Divide(_NumberOfDatasetsByAnnexIIWithMetadata, _NumberOfDatasetsByAnnexII);
        }

        private double ProportionOfDatasetsWithMetadataByAnnexIII()
        {
            return Divide(_NumberOfDatasetsByAnnexIIIWithMetadata, _NumberOfDatasetsByAnnexIII);
        }

        private double ProportionOfServicesWithMetadata()
        {
            return Divide(_NumberOfServicesWithMetadata, _NumberOfServicesByServiceType);
        }

        private double ProportionOfDatasetsWithMetadata()
        {
            return Divide(_NumberOfDatasetsWithMetadata, _NumberOfDatasetsByAnnex);
        }

        private int NumberOfDatasetsByAnnex()
        {
            return _NumberOfDatasetsByAnnexI + _NumberOfDatasetsByAnnexII + _NumberOfDatasetsByAnnexIII;
        }

        private double ProportionOfDatasetWithHarmonizedDataAndConformedMetadata()
        {
            return Divide(_NumberOfDatasetsWithHarmonizedDataAndConformedMetadata, _NumberOfDatasetsByAnnex);
        }

        private double ProportionOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata()
        {
            return Divide(_NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata, _NumberOfDatasetsByAnnexI);
        }

        private double ProportionOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata()
        {
            return Divide(_NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata, _NumberOfDatasetsByAnnexII);
        }

        private double ProportionOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata()
        {
            return Divide(_NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata, _NumberOfDatasetsByAnnexIII);
        }

        private double ProportionOfDatasetsAvailableThroughViewService()
        {
            return Divide(_NumberOfDatasetsAvailableThroughViewService, _NumberOfServicesByServiceTypeView);
        }

        private double ProportionOfDatasetsAvailableThroughDownloadService()
        {
            return Divide(_NumberOfDatasetsAvailableThroughDownloadService, _NumberOfServicesByServiceTypeDownload);
        }

        private double ProportionOfDatasetsAvailableThroughViewAndDownloadService()
        {
            return Divide(_NumberOfDatasetsAvailableThroughViewOrDownloadService, (_NumberOfServicesByServiceTypeDownload + _NumberOfServicesByServiceTypeView));
        }

        private double ProportionOfServicesWhereConformityIsTrue()
        {
            return Divide(_NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue, _NumberOfServicesByServiceType);
        }

        private double ProportionOfServicesByServiceTypeDownloadWhereConformityIsTrue()
        {
            return Divide(_NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue, _NumberOfServicesByServiceTypeDownload);
        }

        private double ProportionOfServicesByServiceTypeDiscoveryWhereConformityIsTrue()
        {
            return Divide(_NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue, _NumberOfServicesByServiceTypeDiscovery);
        }

        private double ProportionOfServicesByServiceTypeViewWhereConformityIsTrue()
        {
            return Divide(_NumberOfServicesByServiceTypeViewWhereConformityIsTrue, _NumberOfServicesByServiceTypeView);
        }

        private double ProportionOfServicesByServiceTypeTransformationWhereConformityIsTrue()
        {
            return Divide(_NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue, _NumberOfServicesByServiceTypeTransformation);
        }

        private double ProportionOfServicesByServiceTypeInvokeWhereConformityIsTrue()
        {
            return Divide(_NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue, _NumberOfServicesByServiceTypeInvoke);
        }

        private double ProportionOfServicesAndDatasetsRegisteredInADiscoveryService()
        {
            return Divide((_NumberOfDatasetsRegisteredInADiscoveryService + _NumberOfServicesRegisteredInADiscoveryService), (_NumberOfDatasetsByAnnex + _NumberOfServicesByServiceType));
        }
    }
}
