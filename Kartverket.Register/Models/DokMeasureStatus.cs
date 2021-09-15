using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Models {
	public class DokMeasureStatus {
        [Key]
        public string value { get; set; }
        public string description { get; set; }

        public string DescriptionTranslated()
        {
            var cultureName = CultureHelper.GetCurrentCulture();
            var dokStatusDescription = description;
            if (!CultureHelper.IsNorwegian())
                dokStatusDescription = value;

            return dokStatusDescription;
        }
    }//end Status

}//end namespace Datamodell