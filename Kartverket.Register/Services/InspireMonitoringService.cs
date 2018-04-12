using System;
using System.Collections.Generic;
using System.Globalization;
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


        private SpatialDataAndService GetSpatialDataAndService(ICollection<RegisterItemV2> inspireItems)
        {
            SpatialDataAndService spatialDataAndService = new SpatialDataAndService();

            spatialDataAndService.DSv_Num1 = NumberOfDatasetsByAnnexI(inspireItems); // Totalt antall datasett for annex1 (<Antall <SpatialDataSet> som har <AnnexI> )
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



        private MetadataExistenceIndicators GetMetadataExsistensIndicators(ICollection<RegisterItemV2> inspireItems)
        {
            MetadataExistenceIndicators metadataExistenceIndicators = new MetadataExistenceIndicators();
            metadataExistenceIndicators.MDi11 = 0; // Andel datasett som har metadata av Annex1 data (<MDv11>/<DSv_Num1>)
            metadataExistenceIndicators.MDi12 = 0; // Andel datasett som har metadata av Annex2 data (<MDv12>/<DSv_Num2>)
            metadataExistenceIndicators.MDi13 = 0; // Andel datasett som har metadata av Annex3 data (<MDv13>/<DSv_Num3>)
            metadataExistenceIndicators.MDi14 = 0; // Andel tjenester som har metadata (<MDv1_DS>/<NSv_NumAllServ>)
            metadataExistenceIndicators.MDi1 = 0; // Andel datasett som har metadata av Annex1 data  (<MDv14>/<DSv_Num>)

            metadataExistenceIndicators.MetadataExistence = GetMetadataExistence(inspireItems);

            metadataExistenceIndicators.MetadataExistence.MDv11 = 0;
            metadataExistenceIndicators.MetadataExistence.MDv12 = 0;
            metadataExistenceIndicators.MetadataExistence.MDv13 = 0;
            metadataExistenceIndicators.MetadataExistence.MDv1_DS = GetNumberOfItemsThatHaveMetadata(inspireItems);
            metadataExistenceIndicators.MetadataExistence.MDv14 = 0;

            return metadataExistenceIndicators;
        }

        private MetadataExistence GetMetadataExistence(ICollection<RegisterItemV2> inspireItems)
        {
            MetadataExistence metadataExistence = new MetadataExistence();
            metadataExistence.MDv11 = NumberOfDatasetsByAnnexIWithMetadata(inspireItems); // Antall Annex1 datasett som har metadata (Alle Annex1 datasett)
            metadataExistence.MDv12 = NumberOfDatasetsByAnnexIIWithMetadata(inspireItems); // Antall Annex2 datasett som har metadata (Alle Annex2 datasett)
            metadataExistence.MDv13 = NumberOfDatasetsByAnnexIIIWithMetadata(inspireItems); // Antall Annex3 datasett som har metadata (Alle Annex3 datasett)
            metadataExistence.MDv1_DS = NumberOfDatasetsWithMetadata(inspireItems); // Antall datasett som har metadata (Alle datasett)
            metadataExistence.MDv14 = NumberOfServicesWithMetadata(inspireItems); // Antall tjenester som har metadata (Alle tjenester)
            return metadataExistence;
        }

        private int NumberOfServicesWithMetadata(ICollection<RegisterItemV2> inspireItems)
        {
            var numberOfServicesWithMetadata = 0;

            foreach (var item in inspireItems)
            {
                if (item is InspireDataService inspireDataService)
                {
                    if (inspireDataService.MetadataIsGoodOrDeficent())
                    {
                        numberOfServicesWithMetadata++;
                    }
                }
            }
            return numberOfServicesWithMetadata;
        }

        private int NumberOfDatasetsWithMetadata(ICollection<RegisterItemV2> inspireItems)
        {
            var numberOfDatasetsWithMetadata = 0;

            foreach (var item in inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (inspireDataset.MetadataIsGoodOrDeficent())
                    {
                        numberOfDatasetsWithMetadata++;
                    }
                }
            }
            return numberOfDatasetsWithMetadata;
        }

        private int NumberOfDatasetsByAnnexIWithMetadata(ICollection<RegisterItemV2> inspireItems)
        {
            var numberOfDatasetsByAnnexIWithMetadata = 0;

            foreach (var item in inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexI(inspireDataset.InspireThemes))
                    {
                        if (inspireDataset.MetadataIsGoodOrDeficent())
                        {
                            numberOfDatasetsByAnnexIWithMetadata++;
                        }
                    }
                }
            }
            return numberOfDatasetsByAnnexIWithMetadata;
        }

        private int NumberOfDatasetsByAnnexIIWithMetadata(ICollection<RegisterItemV2> inspireItems)
        {
            var numberOfDatasetsByAnnexIIWithMetadata = 0;

            foreach (var item in inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexII(inspireDataset.InspireThemes))
                    {
                        if (inspireDataset.MetadataIsGoodOrDeficent())
                        {
                            numberOfDatasetsByAnnexIIWithMetadata++;
                        }
                    }
                }
            }
            return numberOfDatasetsByAnnexIIWithMetadata;
        }

        private int NumberOfDatasetsByAnnexIIIWithMetadata(ICollection<RegisterItemV2> inspireItems)
        {
            var numberOfDatasetsByAnnexIIIWithMetadata = 0;

            foreach (var item in inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(inspireDataset.InspireThemes))
                    {
                        if (inspireDataset.MetadataIsGoodOrDeficent())
                        {
                            numberOfDatasetsByAnnexIIIWithMetadata++;
                        }
                    }
                }
            }
            return numberOfDatasetsByAnnexIIIWithMetadata;
        }

        private int GetNumberOfItemsThatHaveMetadata(ICollection<RegisterItemV2> inspireItems)
        {
            int datasetsThatHaveMetadata = 0;
            foreach (var item in inspireItems)
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

        private static bool IsAnnexI(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(AnnexI), inspireThemeValueCamelCase);
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

        private static bool IsAnnexII(CodelistValue inspireTheme)
        {
            if (inspireTheme != null)
            {
                var inspireThemeValueCamelCase = CreateCamelCase(inspireTheme.value);
                return Enum.IsDefined(typeof(AnnexII), inspireThemeValueCamelCase);
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
                    AnnexIII annexIII = GetAnnexIIIByInspireTheme(inspireTheme.value);
                    annexIIIList.Add(annexIII);
                }
            }
            return annexIIIList;
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


        private int NumberOfDatasetsByAnnexIII(ICollection<RegisterItemV2> inspireItems)
        {
            var numberOfDatasetsByAnnexIII = 0;
            foreach (var item in inspireItems)
            {
                if (item is InspireDataset inspireDataset)
                {
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexIII(inspireDataset.InspireThemes))
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
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexII(inspireDataset.InspireThemes))
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
                    if (InspireDatasetHaveInspireThemeOfTypeAnnexI(inspireDataset.InspireThemes))
                    {
                        numberOfDatasetsByAnnexI++;

                    }
                }
            }
            return numberOfDatasetsByAnnexI;
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
    }
}
