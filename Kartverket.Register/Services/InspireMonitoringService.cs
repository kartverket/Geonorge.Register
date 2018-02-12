using System.Collections.Generic;
using Eu.Europa.Ec.Jrc.Inspire;
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
            monitoring.Indicators = MappingIndicators();
            monitoring.MonitoringMD = MappingMonitoringMd(inspireStatusRegister);
            monitoring.RowData = MappingRowData(inspireStatusRegister.RegisterItems);

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

        private Indicators MappingIndicators()
        {
            Indicators indicators = new Indicators();

            indicators.NnConformityIndicators = new NnConformityIndicators();
            indicators.GeoCoverageIndicators = new GeoCoverageIndicators();
            indicators.UseNNindicators = new UseNNindicators();
            indicators.MetadataExistenceIndicators = new MetadataExistenceIndicators();
            indicators.DiscoveryMetadataIndicators = new DiscoveryMetadataIndicators();
            indicators.ViewDownloadAccessibilityIndicators = new ViewDownloadAccessibilityIndicators();
            indicators.SpatialDataAndService = new SpatialDataAndService();
            indicators.MetadataConformityIndicators = new MetadataConformityIndicators();
            indicators.SdsConformantIndicators = new SdsConformantIndicators();

            return indicators;
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
            List<SpatialDataService> spatialDataService = new List<SpatialDataService>();
            foreach (InspireDataset item in registerItems)
            {
                spatialDataService.Add(MappingSpatialDataService(item));
            }
            return spatialDataService.ToArray();
        }

        private SpatialDataSet[] MappingSpatialDataSets(ICollection<RegisterItemV2> registerItems)
        {
            List<SpatialDataSet> spatialDataSetList = new List<SpatialDataSet>();

            foreach (InspireDataset item in registerItems)
            {
                spatialDataSetList.Add(MappingSpatialDataSet(item));
            }
            return spatialDataSetList.ToArray();
        }

        private SpatialDataService MappingSpatialDataService(InspireDataset item)
        {
            var spatialDataService = new SpatialDataService();
            spatialDataService.name = item.Name;
            spatialDataService.respAuthority = item.Owner.shortname;
            spatialDataService.uuid = item.Uuid;
            spatialDataService.Themes = MappingThemes(item.InspireTheme);
            spatialDataService.MdServiceExistence = MappingServiceExistence(item);
            spatialDataService.NetworkService = MappingNetworkService(item);

            return spatialDataService;
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
            MdDataSetExistence mdDataSetExistence= new MdDataSetExistence();
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
            var themesArray = new[]{themes};

            var annexiList = new List<AnnexI>();
            if (inspireTheme == "Hydrografi")
            {
                annexiList.Add(AnnexI.hydrography);
            }

            themes.AnnexI = annexiList.ToArray();

            return themesArray;
        }
    }
}