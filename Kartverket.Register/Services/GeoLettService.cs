using Kartverket.Register.Models.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Services
{
    public class GeoLettService
    {
        public List<GeoLett> Get()
        {
            List<GeoLett> geoLettList = new List<GeoLett>();

            var path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/geolett.xlsx");
            var fileinfo = new FileInfo(path);
            OfficeOpenXml.ExcelPackage excelPackage;
            excelPackage = new OfficeOpenXml.ExcelPackage(fileinfo);
            OfficeOpenXml.ExcelWorksheet workSheet;
            workSheet = excelPackage.Workbook.Worksheets[2];

            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;

            for (int row = start.Row + 2; row <= end.Row; row++)
            {
                GeoLett geoLett = new GeoLett();
                geoLett.KontekstType = workSheet.Cells[row, 1].Text;
                geoLett.Tittel = workSheet.Cells[row, 3].Text;
                geoLett.Lenke1 = new Lenke { Tittel = workSheet.Cells[row, 5].Text,  Href = workSheet.Cells[row, 6].Text};
                var datasett = new Datasett { Tittel = workSheet.Cells[row, 14].Text };
                geoLett.Datasett = datasett;
                geoLettList.Add(geoLett);
                if (string.IsNullOrEmpty(geoLett.KontekstType))
                    break;
            }

            return geoLettList;
        }
    }
}