using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class StatusHistory
    {
        [Key]
        public Guid Id { get; set; }
    }
}