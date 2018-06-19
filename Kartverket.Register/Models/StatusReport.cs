using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class StatusReport
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public virtual ICollection<StatusHistory> StatusHistories { get; set; }
    }
}