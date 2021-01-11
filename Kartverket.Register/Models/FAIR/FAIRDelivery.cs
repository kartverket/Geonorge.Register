using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.FAIR
{
    public class FAIRDelivery
    {
        public const string Good = "good";
        public const string Satisfactory = "satisfactory";
        public const string Useable = "useable";
        public const string Deficient = "deficient";
        public const string Notset = "notset";

        [Key]
        public Guid FAIRDeliveryId { get; set; }

        [ForeignKey("Status")]
        public string StatusId { get; set; }
        public virtual FAIRDeliveryStatus Status { get; set; }

        public string Note { get; set; }

        public bool AutoUpdate { get; set; }

        public FAIRDelivery(string statusId, string note, bool autoUpdate = true)
        {
            FAIRDeliveryId = Guid.NewGuid();
            StatusId = string.IsNullOrWhiteSpace(statusId) ? Notset : statusId;
            Note = note;
            AutoUpdate = autoUpdate;
        }

        public FAIRDelivery()
        {
            FAIRDeliveryId = Guid.NewGuid();
        }

        public bool IsSet()
        {
            return StatusId != Notset;
        }

        public bool IsGoodOrUseable()
        {
            return StatusId == Good || StatusId == Useable;
        }

        internal bool IsGood()
        {
            return StatusId == Good;
        }

        internal bool IsSatisfactory()
        {
            return StatusId == Satisfactory;
        }
    }

}