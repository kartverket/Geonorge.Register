using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Models
{
    public class Dimension
    {
        [Key]
        public string value { get; set; }
        public string description { get; set; }

        public string DescriptionTranslated()
        {
            var cultureName = CultureHelper.GetCurrentCulture();
            var dimensionDescription = description;
            if (!CultureHelper.IsNorwegian())
                dimensionDescription = value;

            return dimensionDescription;
        }
    }
}