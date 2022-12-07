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
    }
}