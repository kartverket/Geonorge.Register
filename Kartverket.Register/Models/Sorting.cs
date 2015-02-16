using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class Sorting
    {
        [Key]
        public string value { get; set; }
        public string description { get; set; }
    }
}