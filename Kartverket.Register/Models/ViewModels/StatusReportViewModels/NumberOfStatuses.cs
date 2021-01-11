using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class NumberOfStatuses
    {
        public int Good { get; set; }
        public int Deficient { get; set; }
        public int Notset { get; set; }
        public int Useable { get; set; }
        public int Satisfactory { get; set; }

        public NumberOfStatuses(int good, int useable, int deficient, int notset, int satisfactory = 0) {
            Good = good;
            Useable = useable;
            Deficient = deficient;
            Notset = notset;
            Satisfactory = satisfactory;
        }
    }
}