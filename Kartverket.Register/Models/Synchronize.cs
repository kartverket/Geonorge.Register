using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class Synchronize
    {

        public Synchronize()
        {
            FailLog = new HashSet<SyncLogEntry>();
        }

        [Key]
        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public bool Active { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public virtual ICollection<SyncLogEntry> FailLog { get; set; }
    }

    public class SyncLogEntry
    {
        public Guid Id { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }
}