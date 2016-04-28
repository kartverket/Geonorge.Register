using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class DokDeliveryStatus
    {
        [Key]
        public string value { get; set; }
        public string description { get; set; }
    }
}