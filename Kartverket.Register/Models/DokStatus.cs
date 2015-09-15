using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel.DataAnnotations;



namespace Kartverket.Register.Models {
	public class DokStatus {
        [Key]
        public string value { get; set; }
        public string description { get; set; }

	}//end Status

}//end namespace Datamodell