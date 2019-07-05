using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Kartverket.Register.Models;

namespace Kartverket.Register.Controllers
{
    internal class UpdateCodelistService
    {
        private RegisterDbContext db;

        public UpdateCodelistService(RegisterDbContext db)
        {
            this.db = db;
        }

        internal void UpdateMunicipalitiesAll()
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Kommuner2020.xlsx");
            var fileinfo = new FileInfo(path);
            OfficeOpenXml.ExcelPackage excelPackage;
            excelPackage = new OfficeOpenXml.ExcelPackage(fileinfo);
            OfficeOpenXml.ExcelWorksheet workSheet;
            workSheet = excelPackage.Workbook.Worksheets[1];

            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
                var codevalue = workSheet.Cells[row, 2].Text;
                var validFrom = workSheet.Cells[row, 8].Text;
                var validTo = workSheet.Cells[row, 9].Text;
                System.Diagnostics.Debug.WriteLine("Codevalue: " + codevalue + " : "+ validFrom + " - " + validTo);


                string validFromDate = null;
                string validToDate = null;

                if (!string.IsNullOrEmpty(validFrom))
                {
                    var date = validFrom.Split('.');
                    validFromDate = date[2] + "-" + date[1] + "-" + date[0];
                    db.Database.ExecuteSqlCommand("UPDATE RegisterItems SET ValidFromDate = @validfrom WHERE(registerId = '54DDDFA8-A9D3-4115-8541-4B0905779054') AND(value = @codevalue)",
                        new SqlParameter("@codevalue", codevalue), new SqlParameter("@validfrom", validFromDate));
                }

                if (!string.IsNullOrEmpty(validTo))
                {
                    var date = validTo.Split('.');
                    validToDate = date[2] + "-" + date[1] + "-" + date[0];
                    db.Database.ExecuteSqlCommand("UPDATE RegisterItems SET  ValidToDate = @validto WHERE(registerId = '54DDDFA8-A9D3-4115-8541-4B0905779054') AND(value = @codevalue)",
                        new SqlParameter("@codevalue", codevalue), new SqlParameter("@validto", validToDate));
                }
            }
        }

        internal void UpdateMunicipalitiesAllStatus()
        {
            db.Database.ExecuteSqlCommand("UPDATE [kartverket_register].[dbo].[RegisterItems] set statusId = 'Valid' where registerId = '54DDDFA8-A9D3-4115-8541-4B0905779054' and GETDATE() > ValidFromDate");
            db.Database.ExecuteSqlCommand("UPDATE [kartverket_register].[dbo].[RegisterItems] set statusId = 'Retired' where registerId = '54DDDFA8-A9D3-4115-8541-4B0905779054' and ValidToDate < GETDATE()");
        }
    }
}