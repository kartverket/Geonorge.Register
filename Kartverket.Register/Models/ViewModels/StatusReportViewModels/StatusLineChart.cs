﻿using System.Collections.Generic;
using System.Globalization;

namespace Kartverket.Register.Models
{
    public class StatusLineChart
    {
        public List<string> Labels { get; set; }

        public List<int> PointSize { get; set; }
        public List<int> Good { get; set; }
        public List<int> Satisfactory { get; set; }
        public List<int> Deficient { get; set; }
        public List<int> Notset { get; set; }
        public List<int> Useable { get; set; }

        public List<double> FindablePercent { get; set; }
        public List<double> AccessiblePercent { get; set; }
        public List<double> InteroperablePercent { get; set; }
        public List<double> ReUseablePercent { get; set; }

        public StatusLineChart(List<StatusReport> statusReports, StatusReport selectedStatusReport, string statusType = "Metadata")
        {
            Labels = new List<string>();
            PointSize = new List<int>();

            Good = new List<int>();
            Satisfactory = new List<int>();
            Deficient = new List<int>();
            Notset = new List<int>();
            Useable = new List<int>();
            FindablePercent = new List<double>();
            AccessiblePercent = new List<double>();
            InteroperablePercent = new List<double>();
            ReUseablePercent = new List<double>();

            if (statusReports != null)
            {
                foreach (var statusReport in statusReports)
                {
                    PointSize.Add(selectedStatusReport != null && statusReport.Id == selectedStatusReport.Id ? 7 : 3);
                    var xName = statusReport.Date.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("nb-NO"));

                    Labels.Add(xName);
                    Good.Add(statusReport.NumberOfItemsByType(statusType, "good"));
                    Satisfactory.Add(statusReport.NumberOfItemsByType(statusType, "satisfactory"));
                    Deficient.Add(statusReport.NumberOfItemsByType(statusType, "deficient"));
                    Notset.Add(statusReport.NumberOfItemsByType(statusType, "notset"));
                    Useable.Add(statusReport.NumberOfItemsByType(statusType, "useable"));

                    FindablePercent.Add((int) statusReport.FairDatasetsFindablePercent());
                    AccessiblePercent.Add((int)statusReport.FairDatasetsAccessiblePercent());
                    InteroperablePercent.Add((int)statusReport.FairDatasetsInteroperablePercent());
                    ReUseablePercent.Add((int)statusReport.FairDatasetsReuseablePercent());
                }
            }
        }
    }
}