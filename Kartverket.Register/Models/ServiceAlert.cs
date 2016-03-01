using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class ServiceAlert : RegisterItem
    {
        public DateTime AlertDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string AlertType { get; set; }
        public string ServiceType { get; set; }
        [ForeignKey("Owner")]
        public Guid OwnerId { get; set; }
        public virtual Organization Owner { get; set; }
        public string Note { get; set; }
        public string ServiceMetadataUrl { get; set; }
        public string ServiceUuid { get; set; }
    }
}