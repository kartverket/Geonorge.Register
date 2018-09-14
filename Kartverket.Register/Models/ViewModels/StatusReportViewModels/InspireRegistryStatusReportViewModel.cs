using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;

namespace Kartverket.Register.Models.ViewModels
{
    public class InspireRegistryStatusReportViewModel : StatusReportViewModel
    {


        public InspireDatasetStatusReportViweModel InspireDatasetStatusReport { get; set; }
        public InspireDataserviceStatusReportViweModel InspireDataserviceStatusReport { get; set; }

        //public DokLineChart DokHistoricalChart { get; set; }
        
        public InspireRegistryStatusReportViewModel(StatusReport statusReport, List<StatusReport> statusReports, FilterParameters filter)
        {
            ReportsSelectList = CreateSelectList(statusReports);

            if (statusReport != null)
            {
                ReportNotExists = false;
                Id = statusReport.Id;
                Date = statusReport.Date;
                NumberOfItems = statusReport.NumberOfIems();

                InspireDatasetStatusReport = new InspireDatasetStatusReportViweModel(statusReport, statusReports, filter.InspireDatasetStatusType);
                InspireDataserviceStatusReport = new InspireDataserviceStatusReportViweModel(statusReport, statusReports, filter.InspireDataServiceStatusType);                
            }
            else
            {
                ReportNotExists = true;
            }
        }

        private SelectList CreateStatusTypeSelectList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem() { Text = Shared.ShowAll, Value = "all" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Metadata, Value = "Metadata" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_ProductSheet, Value = "ProductSheet" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_PresentationRules, Value = "PresentationRules" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_ProductSpesification, Value = "ProductSpecification" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Wms, Value = "Wms" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Wfs, Value = "Wfs" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_SosiRequirements, Value = "SosiRequirements" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_GmlRequirements, Value = "GmlRequirements" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_AtomFeed, Value = "AtomFeed" });
            items.Add(new SelectListItem() { Text = DataSet.DOK_Delivery_Distribution, Value = "Distribution" });

            var selectList = new SelectList(items, "Value", "Text");

            return selectList;
        }
    }
}