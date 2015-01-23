using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class Registeritem
    {
        public string id { get; set; }
        public string label { get; set; }
        public string itemclass { get; set; }
        public string status { get; set; }

        public string logo { get; set; }
        public string documentreference { get; set; }
    }
}