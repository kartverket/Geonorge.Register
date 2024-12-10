using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class RegisterItemStatusReport
    {
        public RegisterItemStatusReport()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        [MaxLength(255)]
        public string OrganizationSeoName { get; set; }
        public float? Grade { get; set; }
        public double FindableStatusPerCent { get; set; }
        public double AccessibleStatusPerCent { get; set; }
        public double InteroperableStatusPerCent { get; set; }
        public double ReUseableStatusPerCent { get; set; }
    }
}