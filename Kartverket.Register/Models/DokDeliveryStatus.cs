using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Models
{
    public class DokDeliveryStatus
    {
        [Key]
        public string value { get; set; }
        public string description { get; set; }

        public string DescriptionTranslated()
        {
            var cultureName = CultureHelper.GetCurrentCulture();
            var dokDeliveryStatusDescription = description;
            if (!CultureHelper.IsNorwegian())
                dokDeliveryStatusDescription = value;

            return dokDeliveryStatusDescription;
        }
    }
}