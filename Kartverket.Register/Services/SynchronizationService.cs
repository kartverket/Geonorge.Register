using System;
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

        public void UpdatePaths(string systemId, bool transliterNorwegian)
        {
            var sql = @"WITH H AS 
                             (
                            SELECT systemId, parentRegisterId, name, CAST(seoname AS NVARCHAR(300)) AS path
                            FROM Registers
                            WHERE parentRegisterId IS NULL
                            UNION ALL
                            SELECT R.systemId, R.parentRegisterId, R.name, CAST(H.path + '/' + R.seoname AS NVARCHAR(300))
                            FROM Registers R INNER JOIN H ON R.parentRegisterId = H.systemId
                            )
                            SELECT systemId, path FROM H WHERE parentRegisterId ='" + systemId + "' ";
            var items = _dbContext.Database.SqlQuery<ItemRegister>
                (sql)
            .ToList();

            foreach (var item in items)
            {

                _dbContext.Database.ExecuteSqlCommand("UPDATE Registers SET path = '" + item.path + "' WHERE  systemId ='" + item.systemId + "'");

                var registerItems = _dbContext.RegisterItems.Where(r => r.registerId == item.systemId).ToList();

                foreach(var registerItem in registerItems)
                {
                    var seoName = Helpers.RegisterUrls.MakeSeoFriendlyString(registerItem.name, transliterNorwegian);

                    _dbContext.Database.ExecuteSqlCommand("UPDATE RegisterItems SET seoname = '" + seoName + "' WHERE  systemId ='" + registerItem.systemId + "'");

                }

            }
        }
    }

    public class ItemRegister
    {
        public Guid systemId { get; set; }
        public string path { get; set; }
    }
}