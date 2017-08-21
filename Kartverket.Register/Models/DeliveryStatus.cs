///////////////////////////////////////////////////////////
//  Dataset.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class DeliveryStatus
    {
        [Key]
        public Guid DeliveryStatusId { get; set; }

        [ForeignKey("Status")]
        public string StatusId { get; set; }
        public virtual DokDeliveryStatus Status { get; set; }

        public string Note { get; set; }

        public bool AutoUpdate { get; set; } = true;
    }

}//end namespace Datamodell