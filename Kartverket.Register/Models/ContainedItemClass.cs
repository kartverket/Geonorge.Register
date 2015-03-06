using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class ContainedItemClass
    {
        [Key]
        [Display(Name = "Verdi")]
        public string value { get; set; }
        [Display(Name = "Beskrivelse")]
        public string description { get; set; }
    }
}