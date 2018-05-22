using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Kartverket.Register.Migrations;

namespace Kartverket.Register.Models
{
    public class Synchronize
    {

        public Synchronize()
        {
            Id = Guid.NewGuid();
            Start = DateTime.Now;
            Active = true;
            FailLog = new HashSet<SyncLogEntry>();
            DeletedLog = new HashSet<SyncLogEntry>();
            AddedLog = new HashSet<SyncLogEntry>();
        }

        [Key]
        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime? Stop { get; set; }
        public bool Active { get; set; }
        public string ItemType { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public int NumberOfItems { get; set; }
        public int NumberOfDeletedItems { get; set; }
        public int NumberOfNewItems { get; set; }

        public virtual ICollection<SyncLogEntry> FailLog { get; set; }
        public virtual ICollection<SyncLogEntry> AddedLog { get; set; }
        public virtual ICollection<SyncLogEntry> DeletedLog { get; set; }

        public void StoppJob()
        {
            Stop = DateTime.Now;
            Active = false;
        }

        public string Time()
        {
            if (Active)
            {
                return "Pågår";
            }

            try
            {
                return (Stop - Start).Value.Minutes.ToString() + " min";
            }
            catch (Exception e)
            {
                return "Stoppet";
            }
        }
    }

    public class SyncLogEntry
    {

        public SyncLogEntry(InspireDataset inspireDataset, string message)
        {
            Id = Guid.NewGuid();
            Uuid = inspireDataset.Uuid;
            Name = inspireDataset.Name;
            Message = message;
        }

        public SyncLogEntry(InspireDataService inspireDataService, string message)
        {
            Id = Guid.NewGuid();
            Uuid = inspireDataService.Uuid;
            Name = inspireDataService.Name;
            Message = message;
        }

        public Guid Id { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }
}