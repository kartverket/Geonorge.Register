using System.Collections.Generic;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class InspireMonitoringService : IInspireMonitoringService
    {
        public Monitoring Mapping(Models.Register inspireStatusRegister)
        {
            var monitoring = new Monitoring();
            monitoring.documentYear = new Date();
            monitoring.memberState = CountryCode.NO;
            monitoring.Indicators = MappingIndicators();
            monitoring.MonitoringMD = MappingMonitoringMd(inspireStatusRegister);
            monitoring.RowData = MappingRowData(inspireStatusRegister.RegisterItems);

            return monitoring;
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
            List<SpatialDataSet> spatialDataSetList = new List<SpatialDataSet>();
            foreach (InspireDataset item in registerItems)
            {
                spatialDataSetList.Add(MappingSpatialDataSet(item));
            }
            rowData.SpatialDataSet = spatialDataSetList.ToArray();
            return rowData;
        }

        public SpatialDataSet MappingSpatialDataSet(InspireDataset inspireDataset)
        {
            var spatialDataset = new SpatialDataSet();
            spatialDataset.name = inspireDataset.Name;
            spatialDataset.respAuthority = inspireDataset.Owner.shortname;
            spatialDataset.uuid = inspireDataset.SystemId.ToString();
            return spatialDataset;
        }
    }
}