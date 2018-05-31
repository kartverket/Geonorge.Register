using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services
{
    public interface IInspireMonitoringService
    {
        SpatialDataSet MappingSpatialDataSet(InspireDataset inspireDataset);
        Monitoring GetInspireMonitoringReport(Models.Register inspireStatusRegister);
        Monitoring GetInspireMonitoringReport(Models.Register inspireRegister, IInspireMonitoring monitoringData);
        void SaveInspireMonitoring(Models.Register inspireStatusRegister);
        InspireMonitoring GetLatestInspireMonitroingData();
        InspireMonitoring GetTodaysInspireMonitroingData(Models.Register inspireRegister);
        List<InspireMonitoring> GetInspireMonitorings();
        List<InspireMonitoring> GetInspireMonitorings(Models.Register inspireRegister);
        InspireMonitoring GetInspireMonitroingDataById(string filterSelectedInspireMonitoringReport);
        InspireReportViewModel GetInspireReportViewModel(Models.Register register, FilterParameters filter);
    }
}
