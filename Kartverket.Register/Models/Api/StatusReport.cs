using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;

namespace Kartverket.Register.Models.Api
{
    public class StatusReport
    {
        private const string Good = "good";
        private const string Deficient = "deficient";
        private const string Notset = "notset";
        private const string Useable = "useable";

        public StatusReport()
        {
        }

        
        public DateTime Date { get; set; }
        public int NumberOfItems { get; set; }
    }
}