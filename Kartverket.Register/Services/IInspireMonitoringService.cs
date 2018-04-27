using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public interface IInspireMonitoringService
    {
        SpatialDataSet MappingSpatialDataSet(InspireDataset inspireDataset);
        Monitoring GetInspireMonitoringReport(Models.Register inspireStatusRegister);
    }
}
