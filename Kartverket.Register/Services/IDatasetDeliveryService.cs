using System;
using System.Web.Mvc;

namespace Kartverket.Register.Services
{
    public interface IDatasetDeliveryService
    {
        Guid CreateDatasetDelivery(string deliveryStatusId = "notset", string note = null, bool autoUpdate = true);
        SelectList GetDokDeliveryStatusesAsSelectlist(string modelInspireDeliveryMetadataStatus);
        string GetMetadataStatus(string metadataUuid, bool autoUpdate = true, string currentStatus = "deficient");
        string GetDeliveryDistributionStatus(string metadataUuid, bool autoUpdate = true, string currentStatus = "deficient");
        string GetDokDeliveryServiceStatus(string metadataUuid, bool autoUpdate, string currentStatus, string serviceUuid);
        string GetWfsStatus(string metadataUuid, bool autoUpdate = true, string currentStatus = "deficient");
        string GetAtomFeedStatus(string metadataUuid, bool autoUpdate = true, string currentStatus = "deficient");
        string GetHarmonizedStatus(string metadataUuid, bool autoUpdate = true, string currentStatus = "deficient");
        string GetSpatialDataStatus(string metadataUuid, bool autoUpdate = true, string currentStatus = "deficient");
        string GetDownloadRequirementsStatus(string wfsStatus, string atomFeedStatus);
    }
}
