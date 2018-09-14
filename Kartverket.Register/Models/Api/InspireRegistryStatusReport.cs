using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Models.ViewModels;
using Resources;

namespace Kartverket.Register.Models.Api
{
    public class InspireRegistryStatusReport
    {
        public InspireDataSetStatusReport InspireDatasetStatusReport { get; set; }
        public InspireServiceStatusReport InspireServiceStatusReport { get; set; }

        public InspireRegistryStatusReport(StatusReport statusReport)
        {
                InspireDatasetStatusReport = new InspireDataSetStatusReport(statusReport);
                InspireServiceStatusReport = new InspireServiceStatusReport(statusReport);
        }
    }
}