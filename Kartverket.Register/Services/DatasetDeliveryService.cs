using Kartverket.Register.Models;
using System;
using System.Web.Mvc;

namespace Kartverket.Register.Services
{
    public class DatasetDeliveryService : IDatasetDeliveryService
    {
        private readonly RegisterDbContext _dbContext;


        public DatasetDeliveryService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Guid CreateDatasetDelivery(string deliveryStatusId, string note, bool autoupdate = true)
        {
            DatasetDelivery datasetDelivery = new DatasetDelivery(deliveryStatusId, note, autoupdate);
            _dbContext.DatasetDeliveries.Add(datasetDelivery);
            _dbContext.SaveChanges();
            return datasetDelivery.DatasetDeliveryId;
        }

        public SelectList GetDokDeliveryStatusesAsSelectlist(string statusId)
        {
            return new SelectList(_dbContext.DokDeliveryStatuses, "value", "description", statusId);
        }
    }
}