﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kartverket.Register.Models
{
    public class Tag
    {
        [Key]
        public string value { get; set; }
        public string description { get; set; }

        public virtual ICollection<Alert> Alerts { get; set; }
    }
}