using System;
using System.Collections.Generic;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.DOK.Service;
using Kartverket.Register.Models;
using Microsoft.Ajax.Utilities;
using DateTime = System.DateTime;

namespace Kartverket.Register.Services
{
    public class InspireMonitoringService : IInspireMonitoringService
    {
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
            indicators.SpatialDataAndService = GetSpatialDataAndService(rowData);
            indicators.MetadataConformityIndicators = new MetadataConformityIndicators();
            indicators.SdsConformantIndicators = new SdsConformantIndicators();

            return indicators;
        }

        private NnConformityIndicators GetNnConformityIndicators(ICollection<RegisterItemV2> inspireItems)
        {
            NnConformityIndicators nnConformityIndicators = new NnConformityIndicators();
            nnConformityIndicators.NSi41 = ProportionOfServicesWithConformityTrue(inspireItems, "discovery"); // Andel av tjenester NnServiceType="discovery" som har nnConformity="true"(<NSv41>/<NSv_NumDiscServ>)
            nnConformityIndicators.NSi41 = ProportionOfServicesWithConformityTrue(inspireItems, "view"); // Andel av tjenester NnServiceType="view" som har nnConformity="true" (<NSv42>/<NSv_NumViewServ>)
            nnConformityIndicators.NSi41 = ProportionOfServicesWithConformityTrue(inspireItems, "download"); // Andel av tjenester NnServiceType="download" som har nnConformity="true" (<NSv43>/<NSv_NumDownServ>)
            nnConformityIndicators.NSi41 = ProportionOfServicesWithConformityTrue(inspireItems, "transformation"); // Andel av tjenester NnServiceType="transformation" som har nnConformity="true" (<NSv44>/<NSv_NumTransfServ>)
            nnConformityIndicators.NSi41 = ProportionOfServicesWithConformityTrue(inspireItems, "invoke"); // Andel av tjenester NnServiceType="invoke" som har nnConformity="true" (<NSv45>/<NSv_NumInvkServ>)
            nnConformityIndicators.NSi41 = ProportionOfServicesWithConformityTrue(inspireItems); // Andel av tjenester NnServiceType="discovery + view + download + transformation + invoke" som har nnConformity="true" (<NSv4>/<NSv_NumAllServ>)
            return nnConformityIndicators;
        }

        private double ProportionOfServicesWithConformityTrue(ICollection<RegisterItemV2> inspireItems, string serviceType = null)
        {
            int numberOfServicesByServiceType = NumberOfServicesByServiceType(inspireItems, serviceType);
            int numberOfServicesWhereConformityIsTrue = NumberOfServicesWhereConformityIsTrue(inspireItems, serviceType);
            var averageNumberOfCalls = numberOfCalls / numberOfServicesByServiceType;
            return averageNumberOfCalls;
        }

        private int NumberOfServicesWhereConformityIsTrue(ICollection<RegisterItemV2> inspireItems, string serviceType)
        {
            int servicesWhereConformityIsTrue = 0;
            foreach (var item in inspireItems)
            {
                if (item is InspireDataService inspireDataService)
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
            var averageNumberOfCalls = numberOfCalls / numberOfServicesByServiceType;
            return averageNumberOfCalls;
        }

        private int NumberOfServicesByServiceType(ICollection<RegisterItemV2> inspireItems, string serviceType)
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

        private SpatialDataAndService GetSpatialDataAndService(RowData rowData)
        {
            SpatialDataAndService spatialDataAndService = new SpatialDataAndService();
            spatialDataAndService.NSv_NumAllServ = rowData.SpatialDataService.Length;

            return spatialDataAndService;
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
            monitoringMd.monitoringDate = new Date();
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
            spatialDataset.Themes = MappingThemes(inspireDataset.InspireTheme);
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

        private Themes[] MappingThemes(string inspireTheme)
        {
            // TODO få tak i riktig theme...
            var themes = new Themes();
            var themesArray = new[] { themes };

            var annexiList = new List<AnnexI>();
            if (inspireTheme == "Hydrografi")
            {
                annexiList.Add(AnnexI.hydrography);
            }

            themes.AnnexI = annexiList.ToArray();

            return themesArray;
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