///////////////////////////////////////////////////////////
//  Dataset.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class DatasetDelivery
    {
        public const string Useable = "useable";
        public const string Good = "good";
        public const string Notset = "notset";
        public const string Deficient = "deficient";

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
            StatusId = string.IsNullOrWhiteSpace(statusId) ? Notset : statusId;
            Note = note;
            AutoUpdate = autoUpdate;
        }

        public DatasetDelivery()
        {
            DatasetDeliveryId = Guid.NewGuid();
        }

        public bool IsSet()
        {
            return StatusId != Notset;
        }
    }

}//end namespace Datamodell