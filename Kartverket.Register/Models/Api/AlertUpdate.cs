using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class AlertUpdate
    {
        [Display(Name = "ResolvedDate")]
        public DateTime? DateResolved { get; set; }

        public string Summary { get; set; }

    }
}