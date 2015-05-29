using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class accessType
    {
        [Key]
        public int accessLevel { get; set; }
        public string description { get; set; }
    }
}