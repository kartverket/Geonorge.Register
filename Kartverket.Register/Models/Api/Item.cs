using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class Item
    {
        public string name { get; set; }
        public string description { get; set; }
        public string author { get; set; }
        public DateTime updated { get; set; }
        public string status { get; set; }
        public string showUrl { get; set; }
        public string editUrl { get; set; }
    }
}