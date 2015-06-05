using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class NameSpace : RegisterItem 
    {
        [Display(Name = "URL til tjeneste")]
        public string serviceUrl { get; set; }
    }
}