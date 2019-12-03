using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using Kartverket.Register.Models;
using System.Linq;
using System.Collections.Generic;
using Kartverket.Register.Helpers;

namespace Kartverket.Register.Controllers
{
    internal class UpdateCodelistService
    {
        private RegisterDbContext db;

        public UpdateCodelistService(RegisterDbContext db)
        {
            this.db = db;
        }

        internal void UpdateMunicipalitiesAllValidDate()
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

        internal void UpdateCountiesAllValidDate()
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Fylker2020.xlsx");
            var fileinfo = new FileInfo(path);
            OfficeOpenXml.ExcelPackage excelPackage;
            excelPackage = new OfficeOpenXml.ExcelPackage(fileinfo);
            OfficeOpenXml.ExcelWorksheet workSheet;
            workSheet = excelPackage.Workbook.Worksheets[1];

            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;

            for (int row = start.Row + 1; row <= end.Row; row++)
            {
                var codevalue = workSheet.Cells[row, 1].Text;
                var validFrom = workSheet.Cells[row, 8].Text;
                var validTo = workSheet.Cells[row, 9].Text;
                System.Diagnostics.Debug.WriteLine("name: " + codevalue + " : " + validFrom + " - " + validTo);


                string validFromDate = null;
                string validToDate = null;

                if (!string.IsNullOrEmpty(validFrom))
                {
                    var date = validFrom.Split('.');
                    validFromDate = date[2] + "-" + date[1] + "-" + date[0];
                    db.Database.ExecuteSqlCommand("UPDATE RegisterItems SET ValidFromDate = @validfrom WHERE(registerId = '11ec4661-acf4-4636-a960-68a8160642a0') AND(name = @codevalue)",
                        new SqlParameter("@codevalue", codevalue), new SqlParameter("@validfrom", validFromDate));
                }

                if (!string.IsNullOrEmpty(validTo))
                {
                    var date = validTo.Split('.');
                    validToDate = date[2] + "-" + date[1] + "-" + date[0];
                    db.Database.ExecuteSqlCommand("UPDATE RegisterItems SET  ValidToDate = @validto WHERE(registerId = '11ec4661-acf4-4636-a960-68a8160642a0') AND(name = @codevalue)",
                        new SqlParameter("@codevalue", codevalue), new SqlParameter("@validto", validToDate));
                }
            }
        }

        internal void UpdateCountiesAllStatus()
        {
            db.Database.ExecuteSqlCommand("UPDATE [kartverket_register].[dbo].[RegisterItems] set statusId = 'Valid' where registerId = '11ec4661-acf4-4636-a960-68a8160642a0' and GETDATE() > ValidFromDate");
            db.Database.ExecuteSqlCommand("UPDATE [kartverket_register].[dbo].[RegisterItems] set statusId = 'Retired' where registerId = '11ec4661-acf4-4636-a960-68a8160642a0' and ValidToDate < GETDATE()");
        }

        internal void UpdateOrganizationsAll()
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/organisasjoner2020.sql");
            var sql = System.IO.File.ReadAllText(path);
            db.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
            db.Database.ExecuteSqlCommand(sql);

            ////This is how sql was created, uncomment to run again
            //var path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Organisasjoner2020.xlsx");
            //var fileinfo = new FileInfo(path);
            //OfficeOpenXml.ExcelPackage excelPackage;
            //excelPackage = new OfficeOpenXml.ExcelPackage(fileinfo);
            //OfficeOpenXml.ExcelWorksheet workSheet;
            //workSheet = excelPackage.Workbook.Worksheets[1];

            //var start = workSheet.Dimension.Start;
            //var end = workSheet.Dimension.End;

            //List<Municipality> NewOrganizations = new List<Municipality>();
            //List<Municipality> UpdatedOrganizations = new List<Municipality>();
            //List<Municipality> SupersededOrganizations = new List<Municipality>();

            ////System.Diagnostics.Debug.WriteLine("kommunenr,navn,kommunenrGammelt,navnGammelt,organisasjonsnummer,status");

            //for (int row = start.Row + 1; row <= end.Row; row++)
            //{
            //    var kommune2019 = workSheet.Cells[row, 2].Text;
            //    var kommune2020 = workSheet.Cells[row, 4].Text;

            //    //System.Diagnostics.Debug.WriteLine("info: " + kommune2019 + " : " + kommune2020);

            //    var kommune2019Kommunenr = kommune2019.Substring(0, 4);
            //    var kommune2019KommuneNavn = kommune2019.Substring(5, kommune2019.Length - 5);
            //    //System.Diagnostics.Debug.WriteLine("kommune2019: " + kommune2019Kommunenr + " : " + kommune2019KommuneNavn);

            //    var kommune2020Kommunenr = kommune2020.Substring(0, 4);
            //    var kommune2020KommuneNavn = kommune2020.Substring(5, kommune2020.Length - 5);
            //    //System.Diagnostics.Debug.WriteLine("kommune2020: " + kommune2020Kommunenr + " : " + kommune2020KommuneNavn);

            //    var url = "https://data.brreg.no/enhetsregisteret/api/enheter?kommunenummer=" + kommune2019Kommunenr + "&organisasjonsform=KOMM";
            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Accept.Clear();
            //        try
            //        {
            //            var response = client.GetAsync(new Uri(url)).Result;

            //            if (response.StatusCode == HttpStatusCode.OK)
            //            {
            //                var text = response.Content.ReadAsStringAsync().Result;
            //                dynamic data = Newtonsoft.Json.Linq.JObject.Parse(text);
            //                string orgnr = "";
            //                string orgnrNew = "";
            //                var units = data._embedded.enheter;

            //                foreach (var unit in units)
            //                {

            //                    if (unit.navn.ToString().Contains("FORHÅNDSREGISTRERING") || unit.navn.ToString().StartsWith("NYE"))
            //                        orgnrNew = unit.organisasjonsnummer;
            //                    else
            //                        orgnr = unit.organisasjonsnummer;
            //                }

            //                var numberOfUnits = units.Count;
            //                if (kommune2019KommuneNavn == kommune2020KommuneNavn && numberOfUnits == 1)
            //                    UpdatedOrganizations.Add(new Municipality { MunicipalityCode = kommune2020Kommunenr, Name = kommune2020KommuneNavn, MunicipalityCodeOld = kommune2019Kommunenr, Number = orgnr, NameOld = kommune2019KommuneNavn });
            //                else if (numberOfUnits > 1)
            //                {

            //                    //NewOrganizations.Add(kommune2020Kommunenr + " " + kommune2020KommuneNavn + ", orgnummer = " + orgnrNew);
            //                    if (kommune2019KommuneNavn == kommune2020KommuneNavn)
            //                        UpdatedOrganizations.Add(new Municipality { MunicipalityCode = kommune2020Kommunenr, Name = kommune2020KommuneNavn, MunicipalityCodeOld = kommune2019Kommunenr, Number = orgnrNew, NameOld = kommune2019KommuneNavn });
            //                    else
            //                        NewOrganizations.Add(new Municipality { MunicipalityCode = kommune2020Kommunenr, MunicipalityCodeOld = kommune2019Kommunenr, Number = orgnrNew, Name = kommune2020KommuneNavn, NameOld = kommune2019KommuneNavn });

            //                    if (kommune2019KommuneNavn != kommune2020KommuneNavn)
            //                        SupersededOrganizations.Add(new Municipality { MunicipalityCodeOld = kommune2019Kommunenr, MunicipalityCode = kommune2020Kommunenr, Name = kommune2020KommuneNavn, Number = orgnr, NameOld = kommune2019KommuneNavn });
            //                }
            //                else
            //                { 
            //                    if (kommune2019KommuneNavn != kommune2020KommuneNavn && numberOfUnits == 1)
            //                        UpdatedOrganizations.Add(new Municipality { MunicipalityCode = kommune2020Kommunenr, Name= kommune2020KommuneNavn,  MunicipalityCodeOld = kommune2019Kommunenr, Number = orgnr, NameOld = kommune2019KommuneNavn });

            //                    SupersededOrganizations.Add(new Municipality { MunicipalityCodeOld = kommune2019Kommunenr, MunicipalityCode = kommune2020Kommunenr, Name = kommune2020KommuneNavn, Number = orgnr, NameOld = kommune2019KommuneNavn });
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            System.Diagnostics.Debug.WriteLine(ex);
            //        }
            //    }
            //}

            ////System.Diagnostics.Debug.WriteLine("Nye kommuner");
            //foreach (var municipality in NewOrganizations)
            //{
            //    Guid guid = Guid.NewGuid();
            //    string name = municipality.Name.Substring(0, 1) + municipality.Name.Substring(1).ToLower() + " kommune";
            //    string seoname = RegisterUrls.MakeSeoFriendlyString(municipality.Name);

            //    var sqlV = "INSERT INTO Versions(systemId, Register_systemId, currentVersion, lastVersionNumber, containedItemClass)";
            //    sqlV = sqlV + "VALUES('" + guid + "',NULL,'" + guid + "',1,'Organization')";
            //    System.Diagnostics.Debug.WriteLine(sqlV);

            //    var sql = "INSERT INTO RegisterItems(systemId, name, dateSubmitted, modified, number, Discriminator, versioningId, statusId, registerId, submitterId, seoname, OrganizationType, MunicipalityCode)";
            //    sql = sql + "VALUES('" + guid + "','" + name + "',getDate(), getDate() ,'" + municipality.Number + "','Organization','" + guid + "','Valid','fcb0685d-24eb-4156-9ac8-25fa30759094','10087020-f17c-45e1-8542-02acbcf3d8a3','" + seoname + "','municipality','" + municipality.MunicipalityCode + "')";
            //    System.Diagnostics.Debug.WriteLine(sql);
            //    //System.Diagnostics.Debug.WriteLine(municipality.MunicipalityCode + "," + municipality.Name + "," + municipality.MunicipalityCodeOld + "," + municipality.NameOld + "," + municipality.Number + ",ny");

            //}
            ////System.Diagnostics.Debug.WriteLine("--------------------------");

            ////System.Diagnostics.Debug.WriteLine("Kommuner med nytt kommunenummer");
            //foreach (var municipality in UpdatedOrganizations)
            //{
            //    string name = municipality.Name.Substring(0, 1) + municipality.Name.Substring(1).ToLower() + " kommune";
            //    string seoname = RegisterUrls.MakeSeoFriendlyString(municipality.Name);

            //    //System.Diagnostics.Debug.WriteLine(municipality);
            //    string sql;
            //    if(municipality.Name != municipality.NameOld)
            //        sql = "UPDATE [kartverket_register].[dbo].[RegisterItems] set MunicipalityCode='" + municipality.MunicipalityCode + "', number='" + municipality.Number + "' , name = '" + name + "', seoname = '"+ seoname + "' WHERE MunicipalityCode='" + municipality.MunicipalityCodeOld + "'";
            //    else
            //        sql = "UPDATE [kartverket_register].[dbo].[RegisterItems] set MunicipalityCode='" + municipality.MunicipalityCode + "', number='" + municipality.Number + "' WHERE MunicipalityCode='" + municipality.MunicipalityCodeOld + "'";
            //    System.Diagnostics.Debug.WriteLine(sql);
            //    //System.Diagnostics.Debug.WriteLine(municipality.MunicipalityCode + "," + municipality.Name + "," + municipality.MunicipalityCodeOld + "," + municipality.NameOld + "," + municipality.Number + ",oppdatert");
            //}
            ////System.Diagnostics.Debug.WriteLine("--------------------------");

            ////System.Diagnostics.Debug.WriteLine("Utgåtte kommuner");
            //foreach (var municipality in SupersededOrganizations)
            //{
            //    //System.Diagnostics.Debug.WriteLine(municipality);
            //    if (municipality.MunicipalityCodeOld == "0231")
            //    {
            //        var sqlDataset = "DELETE FROM [RegisterItems] WHERE datasetownerId='7FF49DC1-5BD6-433E-BB93-932CE6BCA715'";
            //        System.Diagnostics.Debug.WriteLine(sqlDataset);

            //        var sqlCoverageRetry = "DELETE FROM CoverageDatasets FROM CoverageDatasets INNER JOIN RegisterItems ON CoverageDatasets.MunicipalityId = RegisterItems.systemId WHERE(RegisterItems.MunicipalityCode = '" + municipality.MunicipalityCodeOld + "')";
            //        System.Diagnostics.Debug.WriteLine(sqlCoverageRetry);
            //        var sqlRetry = "DELETE FROM [kartverket_register].[dbo].[RegisterItems] WHERE MunicipalityCode='" + municipality.MunicipalityCodeOld + "'";
            //        System.Diagnostics.Debug.WriteLine(sqlRetry);

            //        var sqlDatasetRetry = "DELETE FROM [RegisterItems] WHERE datasetownerId='7FF49DC1-5BD6-433E-BB93-932CE6BCA715'";
            //        System.Diagnostics.Debug.WriteLine(sqlDatasetRetry);
            //    }

            //    var sqlCoverage = "DELETE FROM CoverageDatasets FROM CoverageDatasets INNER JOIN RegisterItems ON CoverageDatasets.MunicipalityId = RegisterItems.systemId WHERE(RegisterItems.MunicipalityCode = '" + municipality.MunicipalityCodeOld + "')";
            //    System.Diagnostics.Debug.WriteLine(sqlCoverage);
            //    var sql = "DELETE FROM [kartverket_register].[dbo].[RegisterItems] WHERE MunicipalityCode='" + municipality.MunicipalityCodeOld + "'";
            //    System.Diagnostics.Debug.WriteLine(sql);
            //    //System.Diagnostics.Debug.WriteLine(municipality.MunicipalityCode + "," + municipality.Name + "," + municipality.MunicipalityCodeOld + "," + municipality.NameOld + "," + municipality.Number + ",utgått");
            //}
        }

        internal void UpdateMunicipalities()
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/kommuner2020.sql");
            var sql = System.IO.File.ReadAllText(path);
            db.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
            db.Database.ExecuteSqlCommand(sql);
        }
    }

    class Municipality
    {
        public string MunicipalityCode { get; set; }
        public string MunicipalityCodeOld { get; set; }
        public string Name { get; set; }
        public string NameOld { get; set; }
        public string Number { get; set; }
    }
}