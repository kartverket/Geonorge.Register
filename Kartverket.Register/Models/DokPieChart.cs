using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class DokPieChart
    {
            public DokPieChart(int good, int useable, int deficient, int notset)
            {
                Good = good;
                Useable = useable;
                Deficient = deficient;
                NotSet = notset;
            }

            public int Good { get; set; }
            public int Useable { get; set; }
            public int Deficient { get; set; }
            public int NotSet { get; set; }
    }
}