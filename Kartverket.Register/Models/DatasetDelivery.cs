///////////////////////////////////////////////////////////
//  Dataset.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class DatasetDelivery
    {
        [Key]
        public Guid DatasetDeliveryId { get; set; }

        [ForeignKey("Status")]
        public string StatusId { get; set; }
        public virtual DokDeliveryStatus Status { get; set; }

        public string Note { get; set; }

        public bool AutoUpdate { get; set; }

        public DatasetDelivery(string statusId, string note, bool autoUpdate = true)
        {
            DatasetDeliveryId = Guid.NewGuid();
            StatusId = string.IsNullOrWhiteSpace(statusId) ? "notset" : statusId;
            Note = note;
            AutoUpdate = autoUpdate;
        }

        public DatasetDelivery()
        {
            DatasetDeliveryId = Guid.NewGuid();
        }
    }

}//end namespace Datamodell