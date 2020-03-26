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
                if (string.IsNullOrEmpty(geoLett.KontekstType))
                    break;
                geoLett.ID = workSheet.Cells[row, 2].Text;
                geoLett.Tittel = workSheet.Cells[row, 3].Text;
                geoLett.ForklarendeTekst = workSheet.Cells[row, 3].Text;
                geoLett.Lenker = new List<Lenke>();
                List<Lenke> lenker = new List<Lenke>();
                if (!string.IsNullOrEmpty(workSheet.Cells[row, 6].Text))
                    lenker.Add (new Lenke { Tittel = workSheet.Cells[row, 5].Text, Href = workSheet.Cells[row, 6].Text });
                if (!string.IsNullOrEmpty(workSheet.Cells[row, 8].Text))
                    lenker.Add(new Lenke { Tittel = workSheet.Cells[row, 7].Text, Href = workSheet.Cells[row, 8].Text });
                if (!string.IsNullOrEmpty(workSheet.Cells[row, 10].Text))
                    lenker.Add (new Lenke { Tittel = workSheet.Cells[row, 9].Text, Href = workSheet.Cells[row, 10].Text });

                if (lenker.Count > 0)
                    geoLett.Lenker = lenker;

                geoLett.Dialogtekst = workSheet.Cells[row, 11].Text;
                geoLett.MuligeTiltak = workSheet.Cells[row, 12].Text;
                geoLett.Veiledning = workSheet.Cells[row, 13].Text;

                ObjectType objectType = new ObjectType { Objekttype = workSheet.Cells[row, 16].Text, Attributt = workSheet.Cells[row, 17].Text,
                    Kodeverdi = workSheet.Cells[row, 18].Text
                };

                int? bufferAvstand = 0;
                if (!string.IsNullOrEmpty(workSheet.Cells[row, 32].Text))
                    bufferAvstand = int.Parse(workSheet.Cells[row, 32].Text);

                var datasett = new Datasett { Tittel = workSheet.Cells[row, 14].Text, UrlMetadata = workSheet.Cells[row, 15].Text, BufferAvstand = bufferAvstand, TypeReferanse = objectType };
                geoLett.Datasett = datasett;

                Referanse referanse = new Referanse();

                if (!string.IsNullOrEmpty(workSheet.Cells[row, 20].Text))
                    referanse.Tek17 = new Lenke { Tittel = workSheet.Cells[row, 19].Text, Href = workSheet.Cells[row, 20].Text };

                if (!string.IsNullOrEmpty(workSheet.Cells[row, 22].Text))
                    referanse.AnnenLov = new Lenke { Tittel = workSheet.Cells[row, 21].Text, Href = workSheet.Cells[row, 22].Text };

                if (!string.IsNullOrEmpty(workSheet.Cells[row, 23].Text))
                    referanse.RundskrivFraDep = new Lenke { Href = workSheet.Cells[row, 23].Text };

                geoLett.Referanse = referanse;

                geoLett.TekniskKommentar = workSheet.Cells[row, 24].Text;

                if (!string.IsNullOrEmpty(workSheet.Cells[row, 25].Text))
                    geoLett.AnnenKommentar = workSheet.Cells[row, 25].Text;

                if (!string.IsNullOrEmpty(workSheet.Cells[row, 26].Text))
                    geoLett.Tegn1 = workSheet.Cells[row, 26].Text;

                if (!string.IsNullOrEmpty(workSheet.Cells[row, 27].Text))
                    geoLett.Tegn2 = workSheet.Cells[row, 27].Text;

                if (!string.IsNullOrEmpty(workSheet.Cells[row, 28].Text))
                    geoLett.Tegn3 = workSheet.Cells[row, 28].Text;

                if (!string.IsNullOrEmpty(workSheet.Cells[row, 29].Text))
                    geoLett.Tegn4 = workSheet.Cells[row, 29].Text;

                if (!string.IsNullOrEmpty(workSheet.Cells[row, 30].Text))
                    geoLett.Tegn5 = workSheet.Cells[row, 30].Text;

                if (!string.IsNullOrEmpty(workSheet.Cells[row, 31].Text))
                    geoLett.Tegn6 = workSheet.Cells[row, 31].Text;

                geoLettList.Add(geoLett);
            }

            return geoLettList;
        }
    }
}