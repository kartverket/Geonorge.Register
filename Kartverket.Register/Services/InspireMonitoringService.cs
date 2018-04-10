using System;
using System.Collections.Generic;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Models;
using DateTime = System.DateTime;

namespace Kartverket.Register.Services
{
    public class InspireMonitoringService : IInspireMonitoringService
    {
        private readonly RegisterDbContext _dbContext;

        public InspireMonitoringService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Monitoring Mapping(Models.Register inspireStatusRegister)
        {
            var monitoring = new Monitoring();
            monitoring.documentYear = GetReportingDate();
            monitoring.memberState = CountryCode.NO;
            monitoring.MonitoringMD = MappingMonitoringMd(inspireStatusRegister);
            monitoring.RowData = MappingRowData(inspireStatusRegister.RegisterItems);
            monitoring.Indicators = MappingIndicators(inspireStatusRegister.RegisterItems, monitoring.RowData);

            return monitoring;
        }

        private Indicators MappingIndicators(ICollection<RegisterItemV2> inspireItems, RowData rowData)
        {
            Indicators indicators = new Indicators();

            indicators.NnConformityIndicators = GetNnConformityIndicators(inspireItems);
            indicators.GeoCoverageIndicators = new GeoCoverageIndicators();
            indicators.UseNNindicators = GetUseNNindicators(inspireItems);
            indicators.MetadataExistenceIndicators = GetMetadataExsistensIndicators(inspireItems);
            indicators.DiscoveryMetadataIndicators = new DiscoveryMetadataIndicators();
            indicators.ViewDownloadAccessibilityIndicators = new ViewDownloadAccessibilityIndicators();
            indicators.SpatialDataAndService = GetSpatialDataAndService(inspireItems);
            indicators.MetadataConformityIndicators = new MetadataConformityIndicators();
            indicators.SdsConformantIndicators = new SdsConformantIndicators();

            return indicators;
        }

        private NnConformityIndicators GetNnConformityIndicators(ICollection<RegisterItemV2> inspireItems)
        {
            NnConformityIndicators nnConformityIndicators = new NnConformityIndicators();
            nnConformityIndicators.NSi41 = ProportionOfServicesWithConformityTrue(inspireItems, "discovery"); // Andel av tjenester NnServiceType="discovery" som har nnConformity="true"(<NSv41>/<NSv_NumDiscServ>)
            nnConformityIndicators.NSi42 = ProportionOfServicesWithConformityTrue(inspireItems, "view"); // Andel av tjenester NnServiceType="view" som har nnConformity="true" (<NSv42>/<NSv_NumViewServ>)
            nnConformityIndicators.NSi43 = ProportionOfServicesWithConformityTrue(inspireItems, "download"); // Andel av tjenester NnServiceType="download" som har nnConformity="true" (<NSv43>/<NSv_NumDownServ>)
            nnConformityIndicators.NSi44 = ProportionOfServicesWithConformityTrue(inspireItems, "transformation"); // Andel av tjenester NnServiceType="transformation" som har nnConformity="true" (<NSv44>/<NSv_NumTransfServ>)
            nnConformityIndicators.NSi45 = ProportionOfServicesWithConformityTrue(inspireItems, "invoke"); // Andel av tjenester NnServiceType="invoke" som har nnConformity="true" (<NSv45>/<NSv_NumInvkServ>)
            nnConformityIndicators.NSi4 = ProportionOfServicesWithConformityTrue(inspireItems); // Andel av tjenester NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true" (<NSv4>/<NSv_NumAllServ>)
            nnConformityIndicators.NnConformity = GetNnConformity(inspireItems);

            return nnConformityIndicators;
        }

        private NnConformity GetNnConformity(ICollection<RegisterItemV2> inspireItems)
        {
            NnConformity nnConformity = new NnConformity();
            nnConformity.NSv41 = NumberOfServicesByServiceTypeWhereConformityIsTrue(inspireItems, "discovery"); // (Antall tjenester (<SpatialDataService>) av NnServiceType="discovery" som har nnConformity="true")
            nnConformity.NSv41 = NumberOfServicesByServiceTypeWhereConformityIsTrue(inspireItems, "view"); // (Antall tjenester (<SpatialDataService>) av NnServiceType="view" som har nnConformity="true")
            nnConformity.NSv41 = NumberOfServicesByServiceTypeWhereConformityIsTrue(inspireItems, "download"); // (Antall tjenester (<SpatialDataService>) av NnServiceType="download" som har nnConformity="true")
            nnConformity.NSv41 = NumberOfServicesByServiceTypeWhereConformityIsTrue(inspireItems, "transformation"); // (Antall tjenester (<SpatialDataService>) av NnServiceType="transformation" som har nnConformity="true")
            nnConformity.NSv41 = NumberOfServicesByServiceTypeWhereConformityIsTrue(inspireItems, "invoke"); // (Antall tjenester (<SpatialDataService>) av NnServiceType="invoke" som har nnConformity="true")
            nnConformity.NSv4 = NumberOfServicesByServiceTypeWhereConformityIsTrue(inspireItems); // (Antall tjenester (<SpatialDataService>) av NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true")

            return nnConformity;
        }


        private double ProportionOfServicesWithConformityTrue(ICollection<RegisterItemV2> inspireItems, string serviceType = null)
        {
            int numberOfServicesByServiceType = NumberOfServicesByServiceType(inspireItems, serviceType);
            int numberOfServicesWhereConformityIsTrue = NumberOfServicesByServiceTypeWhereConformityIsTrue(inspireItems, serviceType);
            double averageNumberOfCalls = 0;

            if (numberOfServicesByServiceType != 0)
            {
                averageNumberOfCalls = numberOfServicesWhereConformityIsTrue / numberOfServicesByServiceType;
            }
            return averageNumberOfCalls;
        }
        private int NumberOfServicesByServiceTypeWhereConformityIsTrue(ICollection<RegisterItemV2> inspireItems, string serviceType = null)
        {
            int servicesWhereConformityIsTrue = 0;
            foreach (var item in inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.Conformity)
                    {
                        if (string.IsNullOrWhiteSpace(serviceType))
                        {
                            servicesWhereConformityIsTrue++;
                        }
                        else
                        {
                            if (inspireDataService.ServiceType == serviceType)
                            {
                                servicesWhereConformityIsTrue++;
                            }
                        }
                    }
                }
            }
            return servicesWhereConformityIsTrue;
        }

        private UseNNindicators GetUseNNindicators(ICollection<RegisterItemV2> inspireItems)
        {
            UseNNindicators useNNindicators = new UseNNindicators();
            useNNindicators.NSi31 = AverageNumberOfCallsByServiceType(inspireItems, "discovery"); // Gjennomsnittlig antall kall for NnServiceType="discovery" (<NSv31>/<NSv_NumDiscServ>)
            useNNindicators.NSi32 = AverageNumberOfCallsByServiceType(inspireItems, "view"); // Gjennomsnittlig antall kall for NnServiceType="view"(<NSv32>/<NSv_NumViewServ>)
            useNNindicators.NSi33 = AverageNumberOfCallsByServiceType(inspireItems, "download"); // Gjennomsnittlig antall kall for NnServiceType="download" (<NSv33>/<NSv_NumDownServ>)
            useNNindicators.NSi34 = AverageNumberOfCallsByServiceType(inspireItems, "transformation"); // Gjennomsnittlig antall kall for NnServiceType="transformation" (<NSv34>/<NSv_NumTransfServ>)
            useNNindicators.NSi35 = AverageNumberOfCallsByServiceType(inspireItems, "invoke"); // Gjennomsnittlig antall kall for NnServiceType="invoke" (<NSv35>/<NSv_NumInvkServ>)
            useNNindicators.NSi3 = NumberOfCallsByServiceTypeWhereConformityIsTrue(inspireItems); //Gjennomsnittlig antall kall for NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true" (<NSv3>/<NSv_NumAllServ>) -->
            useNNindicators.UseNN = GetUseNN(inspireItems);

            return useNNindicators;
        }

        private UseNN GetUseNN(ICollection<RegisterItemV2> inspireItems)
        {
            UseNN useNN = new UseNN();
            useNN.NSv31 = NumberOfCallsByServiceType(inspireItems, "discovery"); // Akkumulert antall kall for alle NnServiceType="discovery" 
            useNN.NSv32 = NumberOfCallsByServiceType(inspireItems, "view"); // Akkumulert antall kall for alle NnServiceType="view"
            useNN.NSv33 = NumberOfCallsByServiceType(inspireItems, "download"); // Akkumulert antall kall for alle NnServiceType="download"
            useNN.NSv34 = NumberOfCallsByServiceType(inspireItems, "transformation"); // Akkumulert antall kall for alle NnServiceType="transformation"
            useNN.NSv35 = NumberOfCallsByServiceType(inspireItems, "invoke"); // Akkumulert antall kall for alle NnServiceType="invoke"
            useNN.NSv3 = NumberOfCallsByServiceType(inspireItems); // Akkumulert antall kall for alle NnServiceType="discovery + view + download + transformation + invoke"

            return useNN;
        }

        private int NumberOfCallsByServiceTypeWhereConformityIsTrue(ICollection<RegisterItemV2> inspireItems)
        {
            int numberOfCalls = 0;
            foreach (var item in inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.Conformity)
                    {
                        numberOfCalls = inspireDataService.Requests;
                    }
                }
            }
            return numberOfCalls;
        }

        private int NumberOfCallsByServiceType(ICollection<RegisterItemV2> inspireItems, string serviceType = null)
        {
            int numberOfCalls = 0;
            foreach (var item in inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (string.IsNullOrWhiteSpace(serviceType))
                    {
                        numberOfCalls += inspireDataService.Requests;
                    }
                    else
                    {
                        if (inspireDataService.ServiceType == serviceType)
                        {
                            numberOfCalls += inspireDataService.Requests;
                        }
                    }
                }
            }
            return numberOfCalls;
        }

        private double AverageNumberOfCallsByServiceType(ICollection<RegisterItemV2> inspireItems, string serviceType = null)
        {
            int numberOfCalls = NumberOfCallsByServiceType(inspireItems, serviceType);
            int numberOfServicesByServiceType = NumberOfServicesByServiceType(inspireItems, serviceType);
            var averageNumberOfCalls = 0;
            if (numberOfServicesByServiceType != 0)
            {
                averageNumberOfCalls = numberOfCalls / numberOfServicesByServiceType;
            }
            return averageNumberOfCalls;
        }

        private int NumberOfServicesByServiceType(ICollection<RegisterItemV2> inspireItems, string serviceType = null)
        {
            int numberOfServices = 0;
            foreach (var item in inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (string.IsNullOrWhiteSpace(serviceType))
                    {
                        numberOfServices++;
                    }
                    else
                    {
                        if (inspireDataService.ServiceType == serviceType)
                        {
                            numberOfServices++;
                        }
                    }
                }
            }
            return numberOfServices;
        }

        private Date GetReportingDate()
        {
            Date date = new Date();
            date.day = DateTime.Now.Day.ToString();
            date.month = DateTime.Now.Month.ToString();
            date.year = DateTime.Now.Year.ToString();
            return date;
        }

        private SpatialDataAndService GetSpatialDataAndService(ICollection<RegisterItemV2> inspireItems)
        {
            SpatialDataAndService spatialDataAndService = new SpatialDataAndService();

            spatialDataAndService.DSv_Num1 = NumberOfDatasetsByAnnexI(inspireItems); // Totalt antall datasett for  annex1 (<Antall <SpatialDataSet> som har <AnnexI> )
            spatialDataAndService.DSv_Num2 = NumberOfDatasetsByAnnexII(inspireItems); // Totalt antall datasett for  annex2 (<Antall <SpatialDataSet> som har <AnnexII> )
            spatialDataAndService.DSv_Num3 = NumberOfDatasetsByAnnexIII(inspireItems); // Totalt antall datasett for  annex3 (<Antall <SpatialDataSet> som har <AnnexIII> )
            spatialDataAndService.SDSv_Num = NumberOfSdS(inspireItems); // Totalt antall tjenester SDS
            spatialDataAndService.NSv_NumDiscServ = NumberOfServicesByServiceType(inspireItems, "discovery"); // Antall NnServiceType="discovery"
            spatialDataAndService.NSv_NumViewServ = NumberOfServicesByServiceType(inspireItems, "view"); // Antall NnServiceType="view"
            spatialDataAndService.NSv_NumDownServ = NumberOfServicesByServiceType(inspireItems, "download"); // Antall NnServiceType="download"
            spatialDataAndService.NSv_NumInvkServ = NumberOfServicesByServiceType(inspireItems, "invoke"); // Antall NnServiceType="invoke"
            spatialDataAndService.NSv_NumAllServ = NumberOfServicesByServiceType(inspireItems); // Antall NnServiceType="discovery + view + download + transformation + invoke" (Network services)
            spatialDataAndService.NSv_NumTransfServ = NumberOfServicesByServiceType(inspireItems, "transformation"); // Antall NnServiceType="transformation" 

            return spatialDataAndService;
        }

        private int NumberOfDatasetsByAnnexIII(ICollection<RegisterItemV2> inspireItems)
        {
            var numberOfDatasetsByAnnexIII = 0;
            foreach (var item in inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (IsAnnexIII(inspireDataset.InspireTheme))
                    {
                        numberOfDatasetsByAnnexIII++;
                    }
                }
            }
            return numberOfDatasetsByAnnexIII;
        }



        private int NumberOfDatasetsByAnnexII(ICollection<RegisterItemV2> inspireItems)
        {
            var numberOfDatasetsByAnnexII = 0;
            foreach (var item in inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (IsAnnexII(inspireDataset.InspireTheme))
                    {
                        numberOfDatasetsByAnnexII++;
                    }
                }
            }
            return numberOfDatasetsByAnnexII;
        }

        private int NumberOfDatasetsByAnnexI(ICollection<RegisterItemV2> inspireItems)
        {
            var numberOfDatasetsByAnnexI = 0;
            foreach (var item in inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (IsAnnexI(inspireDataset.InspireTheme))
                    {
                        numberOfDatasetsByAnnexI++;
                    }
                }
            }
            return numberOfDatasetsByAnnexI;
        }


        private int NumberOfSdS(ICollection<RegisterItemV2> inspireItems)
        {
            var numberOfSds = 0;
            foreach (RegisterItemV2 item in inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.IsSds())
                    {
                        numberOfSds++;
                    }
                }
            }
            return numberOfSds;
        }

        private MetadataExistenceIndicators GetMetadataExsistensIndicators(ICollection<RegisterItemV2> inspireDatasets)
        {
            MetadataExistenceIndicators metadataExistenceIndicators = new MetadataExistenceIndicators();
            metadataExistenceIndicators.MDi11 = 0;
            metadataExistenceIndicators.MDi12 = 0;
            metadataExistenceIndicators.MDi13 = 0;
            metadataExistenceIndicators.MDi14 = 0;
            metadataExistenceIndicators.MDi1 = 0;

            metadataExistenceIndicators.MetadataExistence = new MetadataExistence();

            metadataExistenceIndicators.MetadataExistence.MDv11 = 0;
            metadataExistenceIndicators.MetadataExistence.MDv12 = 0;
            metadataExistenceIndicators.MetadataExistence.MDv13 = 0;
            metadataExistenceIndicators.MetadataExistence.MDv1_DS = GetNumberOfDatasetsThatHaveMetadata(inspireDatasets);
            metadataExistenceIndicators.MetadataExistence.MDv14 = 0;

            return metadataExistenceIndicators;
        }

        private int GetNumberOfDatasetsThatHaveMetadata(ICollection<RegisterItemV2> inspireDatasets)
        {
            int datasetsThatHaveMetadata = 0;
            foreach (var item in inspireDatasets)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (inspireDataset.HaveMetadata())
                    {
                        datasetsThatHaveMetadata++;
                    }
                }
                else if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.HaveMetadata())
                    {
                        datasetsThatHaveMetadata++;
                    }
                }
            }
            return datasetsThatHaveMetadata;
        }

        private MonitoringMD MappingMonitoringMd(Models.Register inspireStatusRegister)
        {
            var monitoringMd = new MonitoringMD();
            monitoringMd.organizationName = inspireStatusRegister.owner.name;
            monitoringMd.email = "post@norgedigitalt.no";
            monitoringMd.language = LanguageCode.nor;
            //monitoringMd.monitoringDate = new Date();
            return monitoringMd;
        }

        private RowData MappingRowData(ICollection<RegisterItemV2> registerItems)
        {
            var rowData = new RowData();
            rowData.SpatialDataSet = MappingSpatialDataSets(registerItems);
            rowData.SpatialDataService = MappingSpatialDataServices(registerItems);
            return rowData;
        }

        private SpatialDataService[] MappingSpatialDataServices(ICollection<RegisterItemV2> registerItems)
        {
            List<SpatialDataService> spatialDataServices = new List<SpatialDataService>();

            foreach (var item in registerItems)
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

        private SpatialDataSet[] MappingSpatialDataSets(ICollection<RegisterItemV2> registerItems)
        {
            List<SpatialDataSet> spatialDataSetList = new List<SpatialDataSet>();

            foreach (var item in registerItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    spatialDataSetList.Add(MappingSpatialDataSet(inspireDataset));
                }
            }
            return spatialDataSetList.ToArray();
        }


        private NetworkService MappingNetworkService(InspireDataset item)
        {
            NetworkService networkService = new NetworkService();
            networkService.nnConformity = false; // TODO skal hentes fra registeret, men registeret må få strengere regler på tjenestestatus 
            networkService.NnServiceType = MappingNnServiceType(""); // TODO Protocoll... 
            return networkService;
        }

        private NnServiceType MappingNnServiceType(string protocol)
        {
            switch (protocol)
            {
                case "OGC:WMS":
                    return NnServiceType.view;
                case "OGC:WFS":
                    return NnServiceType.view;
                case "OGC:WCS":
                    return NnServiceType.download;
                case "OGC:CSW":
                    return NnServiceType.discovery;
            }
            return new NnServiceType();
        }

        private MdServiceExistence MappingServiceExistence(InspireDataset item)
        {
            MdServiceExistence mdServiceExistence = new MdServiceExistence();
            mdServiceExistence.mdConformity = Accessibility(item.InspireDeliveryMetadata.StatusId);
            mdServiceExistence.discoveryAccessibility = Accessibility(item.InspireDeliveryMetadataService.StatusId);

            return mdServiceExistence;
        }

        public SpatialDataSet MappingSpatialDataSet(InspireDataset inspireDataset)
        {
            var spatialDataset = new SpatialDataSet();
            spatialDataset.name = inspireDataset.Name;
            spatialDataset.respAuthority = inspireDataset.Owner.shortname;
            spatialDataset.uuid = inspireDataset.SystemId.ToString();
            spatialDataset.Themes = GetThemes(inspireDataset.InspireTheme);
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
            mdAccessibility.view = Accessibility(inspireDataset.InspireDeliveryWms.StatusId);
            mdAccessibility.download = Accessibility(inspireDataset.InspireDeliveryWfsOrAtom.StatusId);

            return mdAccessibility;
        }

        private bool Accessibility(string statusId)
        {
            return statusId == "good" || statusId == "useable";
        }

        private Coverage MappingCoverage()
        {
            Coverage coverage = new Coverage();
            coverage.actualArea = 323; // TODO, Default verdi.. 
            coverage.relevantArea = 323; // TODO, Default verdi.. 
            return coverage;
        }

        private Themes[] GetThemes(CodelistValue inspireTheme)
        {
            var themes = new Themes();
            List<AnnexI> annexiListI = AnnexIList(inspireTheme);
            List<AnnexII> annexiListII = AnnexIIList(inspireTheme);
            List<AnnexIII> annexiListIII = AnnexIIIList(inspireTheme);

            themes.AnnexI = annexiListI.ToArray();
            themes.AnnexII = annexiListII.ToArray();
            themes.AnnexIII = annexiListIII.ToArray();

            var themesArray = new[] { themes };

            return themesArray;
        }

        private List<AnnexI> AnnexIList(CodelistValue inspireTheme)
        {
            var annexIList = new List<AnnexI>();
            if (inspireTheme != null)
            {
                if (IsAnnexI(inspireTheme))
                {
                    AnnexI annexI = (AnnexI)Enum.Parse(typeof(AnnexI), inspireTheme.value);
                    annexIList.Add(annexI);
                }
            }
            return annexIList;
        }

        private static bool IsAnnexI(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                return Enum.IsDefined(typeof(AnnexI), inspireTheme.value);
            }
            return false;
        }

        private List<AnnexII> AnnexIIList(CodelistValue inspireTheme)
        {
            var annexIIList = new List<AnnexII>();
            if (inspireTheme != null)
            {
                if (IsAnnexII(inspireTheme))
                {
                    AnnexII annexII = (AnnexII)Enum.Parse(typeof(AnnexII), inspireTheme.value);
                    annexIIList.Add(annexII);
                }
            }
            return annexIIList;
        }

        private static bool IsAnnexII(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                return Enum.IsDefined(typeof(AnnexII), inspireTheme.value);
            }
            return false;
        }

        private List<AnnexIII> AnnexIIIList(CodelistValue inspireTheme)
        {
            var annexIIIList = new List<AnnexIII>();
            if (inspireTheme != null)
            {
                if (IsAnnexIII(inspireTheme))
                {
                    AnnexIII annexIII = (AnnexIII)Enum.Parse(typeof(AnnexIII), inspireTheme.value);
                    annexIIIList.Add(annexIII);
                }
            }
            return annexIIIList;
        }

        private static bool IsAnnexIII(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                return Enum.IsDefined(typeof(AnnexIII), inspireTheme.value);
            }
            return false;
        }

        public List<SpatialDataService> FetchRelatedServicesFromKartkatalogen(string uuid)
        {
            List<SpatialDataService> spatialDataServiceList = new List<SpatialDataService>();

            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/distribution-lists/" + uuid;
            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            try
            {
                var json = c.DownloadString(url);

                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(json);
                if (data != null)
                {
                    foreach (var services in data.RelatedViewServices)
                    {
                        var spatialDataService = new SpatialDataService();
                        spatialDataService.name = services.Title;
                        spatialDataService.respAuthority = services.Organization;
                        spatialDataService.uuid = services.Uuid;
                        //spatialDataService.Themes = MappingThemes(services.InspireTheme);
                        //spatialDataService.MdServiceExistence = MappingServiceExistence(services);
                        //spatialDataService.NetworkService = MappingNetworkService(services);
                        spatialDataServiceList.Add(spatialDataService);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(url);
                return null;
            }

            return spatialDataServiceList;
        }



    }
}
