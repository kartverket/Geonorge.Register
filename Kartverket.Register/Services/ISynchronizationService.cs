using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public interface ISynchronizationService
    {
        Synchronize StartSynchronizationJob(Models.Register register, string itemType);
        void StopSynchronizationJob(Synchronize register);
    }
}
