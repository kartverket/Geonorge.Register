﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class SynchronizationService : ISynchronizationService
    {

        private readonly RegisterDbContext _dbContext;

        public SynchronizationService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Synchronize StartSynchronizationJob(Models.Register register, string itemType)
        {
            var synchronizationJob = new Synchronize();
            synchronizationJob.Active = true;
            synchronizationJob.ItemType = itemType;
            register.Synchronizes.Add(synchronizationJob);
            _dbContext.SaveChanges();
            return synchronizationJob;
        }

        public void StopSynchronizationJob(Synchronize synchronizationJob)
        {
            synchronizationJob.StoppJob();
            _dbContext.SaveChanges();
        }
    }
}