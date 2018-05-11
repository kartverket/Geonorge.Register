using System;
using System.Collections.Generic;
using System.Globalization;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Models;
using System.Linq;
using DateTime = System.DateTime;

namespace Kartverket.Register.Services
{
    public class InspireMonitoringService : IInspireMonitoringService
    {
        private Models.Register _inspireRegister;
        private ICollection<RegisterItemV2> _inspireItems;
        private IInspireMonitoring _inspireMonitoring;
        private readonly RegisterDbContext _dbContext;

        public InspireMonitoringService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Monitoring GetInspireMonitoringReport(Models.Register inspireRegister) 
        {
            return GetInspireMonitoringReport(inspireRegister, new InspireMonitoring(inspireRegister.RegisterItems));
        }

        public Monitoring GetInspireMonitoringReport(Models.Register inspireRegister, IInspireMonitoring monitoringData)
        {
            _inspireRegister = inspireRegister;
            _inspireItems = _inspireRegister?.RegisterItems;
            if (_inspireItems == null) _inspireItems = new List<RegisterItemV2>();
            _inspireMonitoring = monitoringData;
            return Mapping();
        }

        public void SaveInspireMonitoring(Models.Register inspireStatusRegister)
        {
            InspireMonitoring monitoring = new InspireMonitoring(inspireStatusRegister.RegisterItems);
            _dbContext.InspireMonitorings.Add(monitoring);
            _dbContext.SaveChanges();
        }

        public InspireMonitoring GetLatestInspireMonitroingData() {
           var queryResults = from o in _dbContext.InspireMonitorings
                              select o;

            InspireMonitoring latestMonitoring = queryResults?.OrderByDescending(o => o.Date).FirstOrDefault();
            return latestMonitoring;

        }

        public InspireMonitoring GetCurrentInspireMonitroingData(Models.Register inspireRegister)
        {
            InspireMonitoring monitoring = new InspireMonitoring(inspireRegister.RegisterItems);
            return monitoring;
        }

        private Monitoring Mapping()
        {
            var monitoring = new Monitoring();
            monitoring.documentYear = GetReportingDate();
            monitoring.memberState = CountryCode.NO;
            monitoring.MonitoringMD = MappingMonitoringMd();
            monitoring.RowData = MappingRowData();
            monitoring.Indicators = MappingIndicators();

            return monitoring;
        }

        private Date GetReportingDate()
        {
            Date date = new Date();
            date.day = DateTime.Now.Day.ToString();
            date.month = DateTime.Now.Month.ToString();
            date.year = DateTime.Now.Year.ToString();
            return date;
        }

        private MonitoringMD MappingMonitoringMd()
        {
            var monitoringMd = new MonitoringMD();
            monitoringMd.organizationName = _inspireRegister.owner?.name;
            monitoringMd.email = "post@norgedigitalt.no";
            monitoringMd.language = LanguageCode.nor;
            //monitoringMd.monitoringDate = new Date();
            return monitoringMd;
        }

        private RowData MappingRowData()
        {
            var rowData = new RowData();
            rowData.SpatialDataSet = GetSpatialDataSets();
            rowData.SpatialDataService = GetSpatialDataServices();
            return rowData;
        }

        private SpatialDataService[] GetSpatialDataServices()
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
            spatialDataService.respAuthority = inspireDataService.Owner?.name;
            spatialDataService.uuid = inspireDataService.Uuid;
            spatialDataService.Themes = GetThemes(inspireDataService.InspireThemes);
            spatialDataService.MdServiceExistence = MappingServiceExistence(inspireDataService);
            spatialDataService.NetworkService = MappingNetworkService(inspireDataService);
            return spatialDataService;
        }

        private NetworkService MappingNetworkService(InspireDataService inspireDataService)
        {
            NetworkService networkService = new NetworkService();
            networkService.directlyAccessible = inspireDataService.IsNetworkService(); //Er dette en Network Service" Skal settes til "true" for <NnServiceType> in (discovery, view, download, transformation, invoke)
            networkService.URL = inspireDataService.Url;
            networkService.userRequest = inspireDataService.Requests;
            try
            {
                networkService.NnServiceType = (NnServiceType)Enum.Parse(typeof(NnServiceType), inspireDataService.ServiceType);
            }
            catch (Exception)
            {
                return networkService;
            }

            return networkService;
        }

        private MdServiceExistence MappingServiceExistence(InspireDataService InspireDataService)
        {
            MdServiceExistence mdServiceExistence = new MdServiceExistence();
            mdServiceExistence.mdConformity = InspireDataService.MetadataIsGood(); // Metadatakrav oppfylt -  Skal settes til "true"
            mdServiceExistence.discoveryAccessibility = true; // Metadata for tjeneste tilgjengelig gjennom søketjeneste Skal settes til "true"
            mdServiceExistence.discoveryAccessibilityUuid = InspireDataService.Uuid; // UUID til metadata for tjenesten

            return mdServiceExistence;
        }

        private SpatialDataSet[] GetSpatialDataSets()
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

        public SpatialDataSet MappingSpatialDataSet(InspireDataset inspireDataset)
        {
            var spatialDataset = new SpatialDataSet();
            spatialDataset.name = inspireDataset.Name;
            spatialDataset.respAuthority = inspireDataset.Owner?.name;
            spatialDataset.uuid = inspireDataset.Uuid;
            spatialDataset.Themes = GetThemes(inspireDataset.InspireThemes);
            spatialDataset.Coverage = GetCoverage(inspireDataset);
            spatialDataset.MdDataSetExistence = GetMdDatasetEcistence(inspireDataset);
            return spatialDataset;
        }

        private MdDataSetExistence GetMdDatasetEcistence(InspireDataset inspireDataset)
        {
            MdDataSetExistence mdDataSetExistence = new MdDataSetExistence();
            mdDataSetExistence.IRConformity = GetIRConformity(inspireDataset);
            mdDataSetExistence.MdAccessibility = GetMdAccessibility(inspireDataset);

            return mdDataSetExistence;
        }

        private IRConformity GetIRConformity(InspireDataset inspireDataset)
        {
            IRConformity iRConformity = new IRConformity();
            iRConformity.structureCompliance = inspireDataset.HarmonizedIsGood(); //  Finnes metadata- Skal settes til "true" GML - harmoniserte skal være "God"
            return iRConformity;
        }

        private MdAccessibility GetMdAccessibility(InspireDataset inspireDataset)
        {
            MdAccessibility mdAccessibility = new MdAccessibility();
            mdAccessibility.discovery = true; // Finnes metadata for datasett?- Skal alltid settes til "true"
            mdAccessibility.view = inspireDataset.WmsIsGoodOrUseable(); // Finnes metadata for tilhørende visningstjeneste?- Skal alltid settes til "true" dersom datasettet er koplet til en wms
            mdAccessibility.download = inspireDataset.WfsOrAtomIsGoodOrUseable();
            mdAccessibility.viewDownload = inspireDataset.WmsAndWfsOrAtomIsGoodOrUseable();

            return mdAccessibility;
        }


        private Coverage GetCoverage(InspireDataset inspireDataset)
        {
            Coverage coverage = new Coverage();
            coverage.actualArea = inspireDataset.Area;
            coverage.relevantArea = inspireDataset.RelevantArea;
            return coverage;
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
            indicators.MetadataConformityIndicators = GetMetadataConformityIndicators();
            indicators.SdsConformantIndicators = GetSdsConformantIndicators();

            return indicators;
        }


        private NnConformityIndicators GetNnConformityIndicators()
        {
            NnConformityIndicators nnConformityIndicators = new NnConformityIndicators();
            nnConformityIndicators.NSi41 = _inspireMonitoring.ProportionOfServicesByServiceTypeDiscoveryWhereConformityIsTrue(); // Andel av tjenester NnServiceType="discovery" som har nnConformity="true"(<NSv41>/<NSv_NumDiscServ>)
            nnConformityIndicators.NSi42 = _inspireMonitoring.ProportionOfServicesByServiceTypeViewWhereConformityIsTrue(); // Andel av tjenester NnServiceType="view" som har nnConformity="true" (<NSv42>/<NSv_NumViewServ>)
            nnConformityIndicators.NSi43 = _inspireMonitoring.ProportionOfServicesByServiceTypeDownloadWhereConformityIsTrue(); // Andel av tjenester NnServiceType="download" som har nnConformity="true" (<NSv43>/<NSv_NumDownServ>)
            nnConformityIndicators.NSi44 = _inspireMonitoring.ProportionOfServicesByServiceTypeTransformationWhereConformityIsTrue(); // Andel av tjenester NnServiceType="transformation" som har nnConformity="true" (<NSv44>/<NSv_NumTransfServ>)
            nnConformityIndicators.NSi45 = _inspireMonitoring.ProportionOfServicesByServiceTypeInvokeWhereConformityIsTrue(); // Andel av tjenester NnServiceType="invoke" som har nnConformity="true" (<NSv45>/<NSv_NumInvkServ>)
            nnConformityIndicators.NSi4 = _inspireMonitoring.ProportionOfServicesWhereConformityIsTrue(); // Andel av tjenester NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true" (<NSv4>/<NSv_NumAllServ>)
            nnConformityIndicators.NnConformity = GetNnConformity();

            return nnConformityIndicators;
        }

        private NnConformity GetNnConformity()
        {
            NnConformity nnConformity = new NnConformity();
            nnConformity.NSv41 = _inspireMonitoring.NumberOfServicesByServiceTypeDiscoveryWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="discovery" som har nnConformity="true")
            nnConformity.NSv42 = _inspireMonitoring.NumberOfServicesByServiceTypeViewWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="view" som har nnConformity="true")
            nnConformity.NSv43 = _inspireMonitoring.NumberOfServicesByServiceTypeDownloadWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="download" som har nnConformity="true")
            nnConformity.NSv44 = _inspireMonitoring.NumberOfServicesByServiceTypeTransformationWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="transformation" som har nnConformity="true")
            nnConformity.NSv45 = _inspireMonitoring.NumberOfServicesByServiceTypeInvokeWhereConformityIsTrue; // (Antall tjenester (<SpatialDataService>) av NnServiceType="invoke" som har nnConformity="true")
            nnConformity.NSv4 = _inspireMonitoring.NumberOfServicesByServiceTypeWhereConformityIsTrue(); // (Antall tjenester (<SpatialDataService>) av NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true")

            return nnConformity;
        }



        private GeoCoverageIndicators GetGeoCoverageIndicators()
        {
            GeoCoverageIndicators geoCoverageIndicators = new GeoCoverageIndicators();
            geoCoverageIndicators.DSi11 = _inspireMonitoring.ProportionOfArealByAnnexI(); // <DSv11_ActArea>/<DSv11_RelArea>
            geoCoverageIndicators.DSi12 = _inspireMonitoring.ProportionOfArealByAnnexII(); // <DSv12_ActArea>/<DSv12_RelArea>
            geoCoverageIndicators.DSi13 = _inspireMonitoring.ProportionOfArealByAnnexIII(); // <DSv13_ActArea>/<DSv13_RelArea>
            geoCoverageIndicators.DSi1 = _inspireMonitoring.ProportionOfArealByAnnex(); // <DSv1_ActArea>/<DSv1_RelArea> 

            geoCoverageIndicators.GeoCoverageSDS = GetGeoCoverageSDS();

            return geoCoverageIndicators;
        }


        private GeoCoverageSDS GetGeoCoverageSDS()
        {
            GeoCoverageSDS geoCoverageSDS = new GeoCoverageSDS();
            geoCoverageSDS.DSv11_ActArea = _inspireMonitoring.AccumulatedCurrentAreaByAnnexI; // Akkumulert aktuelt areal (<actualArea>) for alle Annex1 data
            geoCoverageSDS.DSv12_ActArea = _inspireMonitoring.AccumulatedCurrentAreaByAnnexII; // Akkumulert aktuelt areal (<actualArea>) for alle Annex2 data
            geoCoverageSDS.DSv13_ActArea = _inspireMonitoring.AccumulatedCurrentAreaByAnnexIII; // Akkumulert aktuelt areal (<actualArea>) for alle Annex3 data
            geoCoverageSDS.DSv1_ActArea = _inspireMonitoring.AccumulatedCurrentAreaByAnnex(); // Akkumulert aktuelt areal (<actualArea>) for alle Annex1, Annex2 og Annex3 data
            geoCoverageSDS.DSv11_RelArea = _inspireMonitoring.AccumulatedRelevantAreaByAnnexI; // Akkumulert relevant areal (<relevantArea>) for alle Annex1 data
            geoCoverageSDS.DSv12_RelArea = _inspireMonitoring.AccumulatedRelevantAreaByAnnexII; // Akkumulert relevant areal (<relevantArea>) for alle Annex2 data
            geoCoverageSDS.DSv13_RelArea = _inspireMonitoring.AccumulatedRelevantAreaByAnnexIII; // Akkumulert relevant areal (<relevantArea>) for alle Annex3 data
            geoCoverageSDS.DSv1_RelArea = _inspireMonitoring.AccumulatedRelevantAreaByAnnex(); // Akkumulert relevant areal (<relevantArea>) for alle Annex1, Annex2 og Annex3 data
            return geoCoverageSDS;
        }


        private UseNNindicators GetUseNNindicators()
        {
            UseNNindicators useNNindicators = new UseNNindicators();
            useNNindicators.NSi31 = _inspireMonitoring.AverageNumberOfCallsByServiceTypeDiscovery(); // Gjennomsnittlig antall kall for NnServiceType="discovery" (<NSv31>/<NSv_NumDiscServ>)
            useNNindicators.NSi32 = _inspireMonitoring.AverageNumberOfCallsByServiceTypeView(); // Gjennomsnittlig antall kall for NnServiceType="view"(<NSv32>/<NSv_NumViewServ>)
            useNNindicators.NSi33 = _inspireMonitoring.AverageNumberOfCallsByServiceTypeDownload(); // Gjennomsnittlig antall kall for NnServiceType="download" (<NSv33>/<NSv_NumDownServ>)
            useNNindicators.NSi34 = _inspireMonitoring.AverageNumberOfCallsByServiceTypeTransformation(); // Gjennomsnittlig antall kall for NnServiceType="transformation" (<NSv34>/<NSv_NumTransfServ>)
            useNNindicators.NSi35 = _inspireMonitoring.AverageNumberOfCallsByServiceTypeInvoke(); // Gjennomsnittlig antall kall for NnServiceType="invoke" (<NSv35>/<NSv_NumInvkServ>)
            useNNindicators.NSi3 = _inspireMonitoring.AverageNumberOfCallsByServiceType(); //Gjennomsnittlig antall kall for NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true" (<NSv3>/<NSv_NumAllServ>) -->
            useNNindicators.UseNN = GetUseNN();

            return useNNindicators;
        }

        private UseNN GetUseNN()
        {
            UseNN useNN = new UseNN();
            useNN.NSv31 = _inspireMonitoring.NumberOfCallsByServiceTypeDiscovery; // Akkumulert antall kall for alle NnServiceType="discovery" 
            useNN.NSv32 = _inspireMonitoring.NumberOfCallsByServiceTypeView; // Akkumulert antall kall for alle NnServiceType="view"
            useNN.NSv33 = _inspireMonitoring.NumberOfCallsByServiceTypeDownload; // Akkumulert antall kall for alle NnServiceType="download"
            useNN.NSv34 = _inspireMonitoring.NumberOfCallsByServiceTypeTransformation; // Akkumulert antall kall for alle NnServiceType="transformation"
            useNN.NSv35 = _inspireMonitoring.NumberOfCallsByServiceTypeInvoke; // Akkumulert antall kall for alle NnServiceType="invoke"
            useNN.NSv3 = _inspireMonitoring.NumberOfCallsByServiceType(); // Akkumulert antall kall for alle NnServiceType="discovery + view + download + transformation + invoke"

            return useNN;
        }

        private MetadataExistenceIndicators GetMetadataExsistensIndicators()
        {
            MetadataExistenceIndicators metadataExistenceIndicators = new MetadataExistenceIndicators();
            metadataExistenceIndicators.MDi11 = _inspireMonitoring.ProportionOfDatasetsWithMetadataByAnnexI();  // Andel datasett som har metadata av Annex1 data (<MDv11>/<DSv_Num1>)
            metadataExistenceIndicators.MDi12 = _inspireMonitoring.ProportionOfDatasetsWithMetadataByAnnexII(); // Andel datasett som har metadata av Annex2 data (<MDv12>/<DSv_Num2>)
            metadataExistenceIndicators.MDi13 = _inspireMonitoring.ProportionOfDatasetsWithMetadataByAnnexIII(); // Andel datasett som har metadata av Annex3 data (<MDv13>/<DSv_Num3>)
            metadataExistenceIndicators.MDi14 = _inspireMonitoring.ProportionOfServicesWithMetadata(); // Andel tjenester som har metadata (<MDv14>/<NSv_NumAllServ>)
            metadataExistenceIndicators.MDi1 = _inspireMonitoring.ProportionOfDatasetsAndServicesWithMetadata(); // <MDv1_DS>+<MDv14> /<NSv_NumAllServ>+<DSv_Num>

            metadataExistenceIndicators.MetadataExistence = GetMetadataExistence();

            return metadataExistenceIndicators;
        }

        private MetadataExistence GetMetadataExistence()
        {
            MetadataExistence metadataExistence = new MetadataExistence();
            metadataExistence.MDv11 = _inspireMonitoring.NumberOfDatasetsByAnnexIWithMetadata; // Antall Annex1 datasett som har metadata (Alle Annex1 datasett)
            metadataExistence.MDv12 = _inspireMonitoring.NumberOfDatasetsByAnnexIIWithMetadata; // Antall Annex2 datasett som har metadata (Alle Annex2 datasett)
            metadataExistence.MDv13 = _inspireMonitoring.NumberOfDatasetsByAnnexIIIWithMetadata; // Antall Annex3 datasett som har metadata (Alle Annex3 datasett)
            metadataExistence.MDv1_DS = _inspireMonitoring.NumberOfDatasetsWithMetadata; // Antall datasett som har metadata (Alle datasett)
            metadataExistence.MDv14 = _inspireMonitoring.NumberOfServicesWithMetadata; // Antall tjenester som har metadata (Alle tjenester)
            return metadataExistence;
        }



        private DiscoveryMetadataIndicators GetDiscoveryMetadataIndicators()
        {
            DiscoveryMetadataIndicators discoveryMetadataIndicators = new DiscoveryMetadataIndicators();
            discoveryMetadataIndicators.NSi11 = _inspireMonitoring.ProportionOfDatasetsRegisteredInADiscoveryService(); // Andel datasett som er registrert i en discovery service (<NSv11>/<DSv_Num>)
            discoveryMetadataIndicators.NSi12 = _inspireMonitoring.ProportionOfServicesRegisteredInADiscoveryService(); // Andel tjenester som er registrert i en discovery service (<NSv12>/<NSv_NumAllServ>)
            discoveryMetadataIndicators.NSi1 = _inspireMonitoring.ProportionOfServicesAndDatasetsRegisteredInADiscoveryService(); // Andel datasett og tjenester som er registrert i en discovery service (<NSv11>+<NSv12>/<DSv_Num>+<NSv_NumAllServ>)
            discoveryMetadataIndicators.DiscoveryMetadata = GetDiscoveryMetadata();

            return discoveryMetadataIndicators;
        }


        private DiscoveryMetadata GetDiscoveryMetadata()
        {
            DiscoveryMetadata discoveryMetadata = new DiscoveryMetadata();
            discoveryMetadata.NSv11 = _inspireMonitoring.NumberOfDatasetsRegisteredInADiscoveryService; // Antall datasett som er registrert i en discovery service (Alle datasett)
            discoveryMetadata.NSv12 = _inspireMonitoring.NumberOfServicesRegisteredInADiscoveryService; // Antall tjenester som er registrert i en discovery service (Alle tjenester)
            return discoveryMetadata;
        }

        private ViewDownloadAccessibilityIndicators GetViewDownloadAccessibilityIndicators()
        {
            ViewDownloadAccessibilityIndicators viewDownloadAccessibilityIndicators = new ViewDownloadAccessibilityIndicators();
            viewDownloadAccessibilityIndicators.NSi21 = _inspireMonitoring.ProportionOfDatasetsAvailableThroughViewService(); // Andel Annex1-3 datasett som er tilgjengelig gjennom view service (<NSv21>/<NSv_NumViewServ>)
            viewDownloadAccessibilityIndicators.NSi22 = _inspireMonitoring.ProportionOfDatasetsAvailableThroughDownloadService(); // Andel Annex1-3 datasett som er tilgjengelig gjennom download service (<NSv22>/<NSv_NumDownServ>)
            viewDownloadAccessibilityIndicators.NSi2 = _inspireMonitoring.ProportionOfDatasetsAvailableThroughViewAndDownloadService(); // Andel Annex1-3 datasett som er tilgjengelig gjennom view OG download service (<NSv23>/<NSv_NumViewServ>+<NSv_NumDownServ>)
            viewDownloadAccessibilityIndicators.ViewDownloadAccessibility = GetViewDownloadAccessibility();

            return viewDownloadAccessibilityIndicators;
        }

        private ViewDownloadAccessibility GetViewDownloadAccessibility()
        {
            ViewDownloadAccessibility viewDownloadAccessibility = new ViewDownloadAccessibility();
            viewDownloadAccessibility.NSv21 = _inspireMonitoring.NumberOfDatasetsAvailableThroughViewService; // Antall Annex1-3 datasett som er tilgjengelig gjennom view service (Alle inspiredata som har WMSstatus= god eller brukbar)
            viewDownloadAccessibility.NSv22 = _inspireMonitoring.NumberOfDatasetsAvailableThroughDownloadService; // Antall Annex1-3 datasett som er tilgjengelig gjennom download service service (Alle registerdata som har WFSstatus= god eller brukbar)
            viewDownloadAccessibility.NSv23 = _inspireMonitoring.NumberOfDatasetsAvailableThroughViewANDDownloadService; // Antall Annex1-3 datasett som er tilgjengelig gjennom view OG download service  (Alle registerdata som har WFSstatus OG WMSstatus = god eller brukbar)

            return viewDownloadAccessibility;
        }

        private SpatialDataAndService GetSpatialDataAndService()
        {
            SpatialDataAndService spatialDataAndService = new SpatialDataAndService();

            spatialDataAndService.DSv_Num1 = _inspireMonitoring.NumberOfDatasetsByAnnexI; // Totalt antall datasett for annex1 (<Antall <SpatialDataSet> som har <AnnexI> )
            spatialDataAndService.DSv_Num2 = _inspireMonitoring.NumberOfDatasetsByAnnexII; // Totalt antall datasett for  annex2 (<Antall <SpatialDataSet> som har <AnnexII> )
            spatialDataAndService.DSv_Num3 = _inspireMonitoring.NumberOfDatasetsByAnnexIII; // Totalt antall datasett for  annex3 (<Antall <SpatialDataSet> som har <AnnexIII> )
            spatialDataAndService.DSv_Num = _inspireMonitoring.NumberOfDatasetsByAnnex(); // Totalt antall datasett for alle annex (<DSv_Num1>+<DSv_Num2>+<DSv_Num3> )
            spatialDataAndService.SDSv_Num = _inspireMonitoring.NumberOfSdS; // Totalt antall tjenester SDS
            spatialDataAndService.NSv_NumDiscServ = _inspireMonitoring.NumberOfServicesByServiceTypeDiscovery; // Antall NnServiceType="discovery"
            spatialDataAndService.NSv_NumViewServ = _inspireMonitoring.NumberOfServicesByServiceTypeView; // Antall NnServiceType="view"
            spatialDataAndService.NSv_NumDownServ = _inspireMonitoring.NumberOfServicesByServiceTypeDownload; // Antall NnServiceType="download"
            spatialDataAndService.NSv_NumInvkServ = _inspireMonitoring.NumberOfServicesByServiceTypeInvoke; // Antall NnServiceType="invoke"
            spatialDataAndService.NSv_NumAllServ = _inspireMonitoring.NumberOfServicesByServiceType(); // Antall NnServiceType="discovery + view + download + transformation + invoke" (Network services)
            spatialDataAndService.NSv_NumTransfServ = _inspireMonitoring.NumberOfServicesByServiceTypeTransformation; // Antall NnServiceType="transformation" 

            return spatialDataAndService;
        }

        private MetadataConformityIndicators GetMetadataConformityIndicators()
        {
            MetadataConformityIndicators metadataConformityIndicators = new MetadataConformityIndicators();
            metadataConformityIndicators.MDi21 = _inspireMonitoring.ProportionOfDatasetByAnnexIWithMetadatastatusGood(); // Andel Annex1 datasett med godkjente metadata (<MDv21>/<DSv_Num1>)
            metadataConformityIndicators.MDi22 = _inspireMonitoring.ProportionOfDatasetByAnnexIIWithMetadatastatusGood(); // Andel Annex2 datasett med godkjente metadata (<MDv22>/<DSv_Num2>)
            metadataConformityIndicators.MDi23 = _inspireMonitoring.ProportionOfDatasetByAnnexIIIWithMetadatastatusGood(); // Andel Annex3 datasett med godkjente metadata (<MDv23>/<DSv_Num3>)
            metadataConformityIndicators.MDi24 = _inspireMonitoring.ProportionOfServicesWithMetadatastatusGood(); // Andel tjenester med godkjente metadata (<MDv24>/<NSv_NumAllServ>)
            metadataConformityIndicators.MDi2 = _inspireMonitoring.ProportionOfServicesAndDatasetsWithMetadatastatusGood(); // Andel tjenester OG datasett med godkjente metadata (<MDv24>+<MDv2_DS>/<NSv_NumAllServ>+<DSv_Num>)
            metadataConformityIndicators.MetadataConformity = GetMetadataConformity();

            return metadataConformityIndicators;
        }


        private MetadataConformity GetMetadataConformity()
        {
            MetadataConformity metadataConformity = new MetadataConformity();
            metadataConformity.MDv21 = _inspireMonitoring.NumberOfDatasetsByAnnexIWhereMetadataStatusIsgood; // Totalt antall datasett for  annex1 med Metadatastatus = "God i registeret" (Antall elementer av <SpatialDataSet> where <AnnexI> OG <structureCompliance> = "true")
            metadataConformity.MDv22 = _inspireMonitoring.NumberOfDatasetsByAnnexIIWhereMetadataStatusIsgood; // Totalt antall datasett for  annex2 med Metadatastatus = "God i registeret" (Antall elementer av <SpatialDataSet> where <AnnexII> OG <structureCompliance> = "true")
            metadataConformity.MDv23 = _inspireMonitoring.NumberOfDatasetsByAnnexIIIWhereMetadataStatusIsgood; // Totalt antall datasett for  annex3 med Metadatastatus = "God i registeret" (Antall elementer av <SpatialDataSet> where <AnnexIII> OG <structureCompliance> = "true")
            metadataConformity.MDv2_DS = _inspireMonitoring.NumberOfDatasetsByAnnexWhereMetadataStatusIsgood(); // Totalt antall datasett med Metadatastatus = "God" i registeret for annex1,2,3 (<MDv21>+<MDv22>+<MDv23>)
            metadataConformity.MDv24 = _inspireMonitoring.NumberOfServicesWhereMetadataStatusIsgood; // Totalt antall tjenester med konforme metadata (Antall tjenester som har Metadatastatus = "God". Antall elementer av <SpatialDataService> where <mdConformity> = "true")

            return metadataConformity;
        }


        private SdsConformantIndicators GetSdsConformantIndicators()
        {
            SdsConformantIndicators sdsConformantIndicators = new SdsConformantIndicators();
            sdsConformantIndicators.DSi21 = _inspireMonitoring.ProportionOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata(); // Andel datasett fra Annex1 med konforme metadata og harmoniserte datasett (<DSv21>/<DSv_Num1>)
            sdsConformantIndicators.DSi22 = _inspireMonitoring.ProportionOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata(); // Andel datasett fra Annex2 med konforme metadata og harmoniserte datasett (<DSv22>/<DSv_Num2>)
            sdsConformantIndicators.DSi23 = _inspireMonitoring.ProportionOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata(); // Andel datasett fra Annex3 med konforme metadata og harmoniserte datasett (<DSv23>/<DSv_Num3>)
            sdsConformantIndicators.DSi2 = _inspireMonitoring.ProportionOfDatasetWithHarmonizedDataAndConformedMetadata(); // Andel datasettt fra Annex1,2,3 med konforme metadata og harmoniserte datasett (<DSv2>/<DSv_Num>)
            sdsConformantIndicators.SdsConformant = GetSdsConformant();

            return sdsConformantIndicators;
        }

        private SdsConformant GetSdsConformant()
        {
            SdsConformant sdsConformant = new SdsConformant();
            sdsConformant.DSv21 = _inspireMonitoring.NumberOfDatasetsByAnnexIWithHarmonizedDataAndConformedMetadata; // Antall datasett fra Annex1 med harmoniserte data og konforme metadata (Dtasett where <AnnexI>:  Harmoniserte data="God" OG Metadatastatus = "God"= )
            sdsConformant.DSv22 = _inspireMonitoring.NumberOfDatasetsByAnnexIIWithHarmonizedDataAndConformedMetadata; // Antall datasett fra Annex2 med harmoniserte data og konforme metadata (Dtasett where <AnnexII>:  Harmoniserte data="God" OG Metadatastatus = "God"= )
            sdsConformant.DSv23 = _inspireMonitoring.NumberOfDatasetsByAnnexIIIWithHarmonizedDataAndConformedMetadata; // Antall datasett fra Annex3 med harmoniserte data og konforme metadata (Dtasett where <AnnexIII>:  Harmoniserte data="God" OG Metadatastatus = "God"= )
            sdsConformant.DSv2 = _inspireMonitoring.NumberOfDatasetsWithHarmonizedDataAndConformedMetadata(); // (Dtasett where <AnnexI> or <AnnexII> or <AnnexIII>:  Harmoniserte data="God" OG Metadatastatus = "God"= )
            return sdsConformant;
        }
        

        private Themes[] GetThemes(ICollection<CodelistValue> inspireThemes)
        {
            var themes = new Themes();
            List<AnnexI> annexiListI = new List<AnnexI>();
            List<AnnexII> annexiListII = new List<AnnexII>();
            List<AnnexIII> annexiListIII = new List<AnnexIII>();

            foreach (var inspireTheme in inspireThemes)
            {
                if (IsAnnexI(inspireTheme))
                {
                    AnnexI annexI = GetAnnexIByInspireTheme(inspireTheme.value);
                    annexiListI.Add(annexI);
                }
                if (IsAnnexII(inspireTheme))
                {
                    AnnexII annexII = GetAnnexIIByInspireTheme(inspireTheme.value);
                    annexiListII.Add(annexII);
                }
                if (IsAnnexIII(inspireTheme))
                {
                    AnnexIII annexIII = GetAnnexIIIByInspireTheme(inspireTheme.value);
                    annexiListIII.Add(annexIII);
                }
            }

            themes.AnnexI = annexiListI.ToArray();
            themes.AnnexII = annexiListII.ToArray();
            themes.AnnexIII = annexiListIII.ToArray();

            var themesArray = new[] { themes };

            return themesArray;
        }

        
        private static string CreateCamelCase(string inspireTheme)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            var inspireThemeCamelCase = textInfo.ToTitleCase(inspireTheme);
            inspireThemeCamelCase = Char.ToLowerInvariant(inspireThemeCamelCase[0]) + inspireThemeCamelCase.Substring(1);
            inspireThemeCamelCase = inspireThemeCamelCase.Replace("/", " ");
            inspireThemeCamelCase = inspireThemeCamelCase.Replace(" ", "");
            inspireThemeCamelCase = inspireThemeCamelCase.Replace("-", "");

            return inspireThemeCamelCase;
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

    }
}
