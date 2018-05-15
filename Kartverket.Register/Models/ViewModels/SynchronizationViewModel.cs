using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class SynchronizationViewModel
    {
        public ICollection<Synchronize> SynchronizationJobs { get; set; }
        public Synchronize ActiveSynchronizationJob { get; set; }

        public SynchronizationViewModel(ICollection<Synchronize> synchronizes)
        {
            SynchronizationJobs = synchronizes;
            ActiveSynchronizationJob = GetActiveSynchronizationJob();
        }

        private Synchronize GetActiveSynchronizationJob()
        {
            var activJob =
                from s in SynchronizationJobs
                where s.Active
                select s;

            return activJob.FirstOrDefault();
        }
    }
}