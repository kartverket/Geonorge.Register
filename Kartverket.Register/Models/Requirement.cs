using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class Requirement
    {
        [Key]
        public string value { get; set; }
        public string description { get; set; }
    }
}