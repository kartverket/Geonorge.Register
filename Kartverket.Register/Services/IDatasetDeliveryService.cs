using System;
using Kartverket.Register.Models;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services
{
    public interface IDatasetDeliveryService
    {
        Guid CreateDatasetDelivery(string deliveryStatusId, string note, bool autoupdate);
    }
}
