using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireReportViewModel
    {

        public InspireMonitoringViewModel CurrentInspireMonitoring { get; set; }
        public string CurrentInspireMonitoringDate { get; set; }
        public InspireMonitoringViewModel ComparableCandidate { get; set; }
        public string ComparableCandidateDate { get; set; }

        public SelectList InspireMonitoringSelectList { get; set; }
        public SelectList InspireCompareMonitoringSelectList { get; set; }

        public InspireReportViewModel(IInspireMonitoring current, IInspireMonitoring candidate, List<InspireMonitoring> inspireMonitorings)
        {
            CurrentInspireMonitoring = new InspireMonitoringViewModel(current);
            CurrentInspireMonitoringDate = CurrentInspireMonitoring.Date.ToString();
            if (candidate != null)
            {
                ComparableCandidate = new InspireMonitoringViewModel(candidate);
                ComparableCandidateDate = candidate.Date.ToString();
            }

            InspireMonitoringSelectList = CreateSelectList(inspireMonitorings);
            InspireCompareMonitoringSelectList = CreateSelectList(inspireMonitorings);
        }

        private SelectList CreateSelectList(List<InspireMonitoring> inspireMonitorings)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (inspireMonitorings.Any())
            {
                foreach (var monitorings in inspireMonitorings.OrderByDescending(i => i.Date))
                {
                    items.Add(new SelectListItem() { Text = monitorings.Date.ToString(), Value = monitorings.Id.ToString() });
                }
            }
            items.Add(new SelectListItem() { Text = "Dagens status (ikke lagret)", Value = "today" });
            var selectList = new SelectList(items, "Value", "Text");

            return selectList;
        }
    }
}