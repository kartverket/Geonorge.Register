using System;
using System.Web.Mvc;

namespace Kartverket.Register.Services
{
    public interface IDatasetDeliveryService
    {
        Guid CreateDatasetDelivery(string deliveryStatusId, string note, bool autoUpdate);
        SelectList GetDokDeliveryStatusesAsSelectlist(string modelInspireDeliveryMetadataStatus);
        string GetMetadataStatus(string metadataUuid, bool autoUpdate, string currentStatus);
        string GetDeliveryDistributionStatus(string metadataUuid, bool autoUpdate, string currentStatus);
        string GetDokDeliveryServiceStatus(string metadataUuid, bool autoUpdate, string currentStatus, string serviceUuid);
        string GetWfsStatus(string metadataUuid, bool autoUpdate, string currentStatus);
        string GetAtomFeedStatus(string metadataUuid, bool autoUpdate, string currentStatus);
        string GetHarmonizedStatus(string metadataUuid, bool autoUpdate, string currentStatus);
        string GetSpatialDataStatus(string metadataUuid, bool autoUpdate, string currentStatus);
    }
}
