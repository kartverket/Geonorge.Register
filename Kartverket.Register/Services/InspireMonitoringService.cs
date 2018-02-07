using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class InspireMonitoringService : IInspireMonitoringService
    {
        public Monitoring Mapping(Models.Register inspireStatusRegister)
        {
            var monitoring = new Monitoring();

            monitoring.memberState = CountryCode.NO;
            monitoring.MonitoringMD = new MonitoringMD();
            monitoring.MonitoringMD.organizationName = "Kartverket";
            monitoring.MonitoringMD.email = "post@norgedigitalt.no";
            monitoring.MonitoringMD.language = LanguageCode.nor;

            monitoring.RowData = new RowData();
            List<SpatialDataSet> spatialDataSetList = new List<SpatialDataSet>();
            foreach (InspireDataset item in inspireStatusRegister.RegisterItems)
            {
                spatialDataSetList.Add(Mapping(item));
            }
            monitoring.RowData.SpatialDataSet = spatialDataSetList.ToArray();

            return monitoring;
        }

        public SpatialDataSet Mapping(InspireDataset inspireDataset)
        {
            var spatialDataset = new SpatialDataSet();
            spatialDataset.name = inspireDataset.Name;
            spatialDataset.respAuthority = inspireDataset.Owner.shortname;
            spatialDataset.uuid = inspireDataset.SystemId.ToString();
            return spatialDataset;
        }
    }
}