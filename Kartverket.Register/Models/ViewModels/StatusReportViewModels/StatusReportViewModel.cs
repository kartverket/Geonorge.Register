using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Helpers;
using Kartverket.Register.Services;
using Resources;
using SolrNet.Mapping.Validation.Rules;

namespace Kartverket.Register.Models.ViewModels
{
    public class StatusReportViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfItems { get; set; }
        public SelectList StatusTypeSelectList { get; set; }
        public SelectList ReportsSelectList { get; set; }

        public bool ReportNotExists { get; set; }


        public StatusReportViewModel()
        {
            ReportNotExists = true;
        }

        public double Percent(int numberOf)
        {
            return HtmlHelperExtensions.Percent(numberOf, NumberOfItems);
        }


        public SelectList CreateSelectList(List<StatusReport> statusReports)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            if (statusReports == null)
            {
                items.Add(new SelectListItem() { Text = "Ingen rapporter", Value = null });
            }
            else if (statusReports.Any())
            {
                foreach (var report in statusReports.OrderByDescending(i => i.Date))
                {
                        items.Add(new SelectListItem() { Text = report.Date.ToString(), Value = report.Id.ToString() });
                }
            }
            var selectList = new SelectList(items, "Value", "Text");

            return selectList;
        }
    }
}