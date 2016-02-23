using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class MetadataItemViewModel
    {
        public string Uuid { get; set; }
        public string Title { get; set; }
        public bool Selected { get; set; }
    }
}