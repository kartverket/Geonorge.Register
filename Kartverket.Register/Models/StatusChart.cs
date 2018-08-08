using System.Collections.Generic;
using System.Globalization;

namespace Kartverket.Register.Models
{
    public class StatusChart
    {
        public List<string> Labels { get; set; }

        public List<int> PointSize { get; set; }
        public List<int> Good { get; set; }
        public List<int> Deficient { get; set; }
        public List<int> Notset { get; set; }
        public List<int> Useable { get; set; }

        public StatusChart(List<StatusReport> statusReports, StatusReport selectedStatusReport, string statusType = "Metadata")
        {
            Labels = new List<string>();
            PointSize = new List<int>();

            Good = new List<int>();
            Deficient = new List<int>();
            Notset = new List<int>();
            Useable = new List<int>();

            if (statusReports != null)
            {

                foreach (var statusReport in statusReports)
                {
                    PointSize.Add(selectedStatusReport != null && statusReport.Id == selectedStatusReport.Id ? 7 : 3);
                    var xName = statusReport.Date.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("nb-NO"));

                    Labels.Add(xName);
                    Good.Add(statusReport.NumberOfItemsByType(statusType, "good"));
                    Deficient.Add(statusReport.NumberOfItemsByType(statusType, "deficient"));
                    Notset.Add(statusReport.NumberOfItemsByType(statusType, "notset"));
                    Useable.Add(statusReport.NumberOfItemsByType(statusType, "useable"));
                }
            }
        }
    }
}