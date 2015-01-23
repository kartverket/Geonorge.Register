using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class Register
    {
        public string id { get; set; }
        public string label { get; set; }

        public string contentsummary { get; set; }
        public string owner { get; set; }
        public string manager { get; set; }
        public string controlbody { get; set; }
        public List<Registeritem> containeditems { get; set; }
    }
}