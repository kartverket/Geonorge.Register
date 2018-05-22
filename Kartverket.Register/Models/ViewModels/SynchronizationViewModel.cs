using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class SynchronizationViewModel
    {
        public ICollection<Synchronize> SynchronizationJobs { get; set; }
        public ICollection<Synchronize> ActiveSynchronizationJob { get; set; }

        public SynchronizationViewModel(ICollection<Synchronize> synchronizes)
        {
            SynchronizationJobs = synchronizes.Where(s => s.Active == false).OrderByDescending(s => s.Start).ToList();
            ActiveSynchronizationJob = GetActiveSynchronizationJob(synchronizes);
        }

        private List<Synchronize> GetActiveSynchronizationJob(ICollection<Synchronize> synchronizes)
        {
            var activJob =
                from s in synchronizes
                where s.Active
                select s;

            return activJob.OrderByDescending(s => s.Start).ToList();
        }

        public bool ActiveSynchronizationOfDatasets()
        {
            foreach (var activeJobs in ActiveSynchronizationJob)
            {
                if (activeJobs.ItemType == "Datasett")
                {
                    return true;
                }
            }
            return false;
        }

        public bool ActiveSynchronizationOfServices()
        {
            foreach (var activeJobs in ActiveSynchronizationJob)
            {
                if (activeJobs.ItemType == "Tjenester")
                {
                    return true;
                }
            }
            return false;
        }
    }
}