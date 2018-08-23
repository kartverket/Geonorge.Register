using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Services;
using Resources;
using SolrNet.Mapping.Validation.Rules;

namespace Kartverket.Register.Models.ViewModels
{
    public class StatusReportViewModel
    {
        private StatusReport statusReport;
        private List<StatusReport> statusReports;


        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfItems { get; set; }
        public SelectList StatusTypeSelectList { get; set; }
        public bool ReportNotExists { get; set; }


        public StatusReportViewModel()
        {
            ReportNotExists = true;
        }

        public double Percent(int numberOf)
        {
            var x = Divide(numberOf, NumberOfItems);
            return Math.Round(x * 100, 2);
        }

        private double Divide(int x, int y)
        {
            try
            {
                if (y == 0)
                {
                    return 0;
                }
                else
                {
                    return (double)x / y;
                }
            }
            catch (Exception e)
            {

                return 0;
            }
        }
    }
}