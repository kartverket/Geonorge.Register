using Kartverket.Register.Models.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kartverket.Register.Formatter
{
    public class CsvFormatter : BufferedMediaTypeFormatter
    {
        private readonly string csv = "text/csv";

        public CsvFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(csv));
            MediaTypeMappings.Add(new UriPathExtensionMapping("csv", csv));
        }

        Func<Type, bool> SupportedTypeCSV = (type) =>
        {
            if (type == typeof(Models.Api.Register) ||
                type == typeof(Kartverket.Register.Models.Api.Registeritem) ||
                type == typeof(IEnumerable<Models.Api.Register>) ||
                type == typeof(List<Models.Api.Register>) ||
                type == typeof(IEnumerable<Kartverket.Register.Models.Api.Registeritem>) ||
                type == typeof(List<Kartverket.Register.Models.Api.Registeritem>))

                return true;
            else
                return false;
        };

        public override bool CanWriteType(System.Type type)
        {
            return SupportedTypeCSV(type);
        }
        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            if (type == typeof(Models.Api.Register) ||
                type == typeof(Kartverket.Register.Models.Api.Registeritem) ||
                type == typeof(IEnumerable<Models.Api.Register>) ||
                type == typeof(List<Models.Api.Register>) ||
                type == typeof(IEnumerable<Kartverket.Register.Models.Api.Registeritem>) ||
                type == typeof(List<Kartverket.Register.Models.Api.Registeritem>))
                BuildCSV(value, writeStream, content.Headers.ContentType.MediaType);
        }

        private void BuildCSV(object models, Stream stream, string contenttype)
        {
            StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
            if (models is Models.Api.Register)
            {
                Models.Api.Register register = (Models.Api.Register)models;
                ConvertRegisters(streamWriter, register);
            }
            if (models is Models.Api.Registeritem)
            {
                Models.Api.Registeritem registerItem = (Models.Api.Registeritem)models;
                streamWriter.WriteLine(RegisterItemHeading(registerItem.itemclass));
                ConvertRegisterItemToCSV(streamWriter, registerItem);
                foreach (Models.Api.Registeritem item in registerItem.versions.OrderBy(v => v.versionNumber))
                {
                    ConvertRegisterItemToCSV(streamWriter, item);
                }
            }
            if (models is List<Models.Api.Register>)
            {
                streamWriter.WriteLine(RegisterHeading());
                List<Models.Api.Register> registers = (List<Models.Api.Register>)models;
                foreach (Models.Api.Register reg in registers.OrderBy(r => r.label))
                {
                    ConvertRegisterToCSV(streamWriter, reg);
                }
            }
            if (models is List<Models.Api.Registeritem>)
            {
                List<Models.Api.Registeritem> registerItems = (List<Models.Api.Registeritem>)models;
                if (registerItems.Count() > 0)
                {
                    streamWriter.WriteLine(RegisterItemHeading(registerItems[0].itemclass));
                    foreach (Models.Api.Registeritem item in registerItems.OrderBy(r => r.label))
                    {
                        ConvertRegisterItemToCSV(streamWriter, item);
                    }
                }
            }
            streamWriter.Close();
        }

        private void ConvertRegisters(StreamWriter streamWriter, Models.Api.Register register)
        {
            if (register.containeditems != null)
            {
                streamWriter.WriteLine(RegisterItemHeading(register.containedItemClass));
                foreach (Models.Api.Registeritem item in register.containeditems.ToList().OrderBy(i => i.label))
                {
                    ConvertRegisterItemToCSV(streamWriter, item);
                }
            }
            else if (register.containedSubRegisters != null)
            {
                streamWriter.WriteLine(RegisterHeading());
                foreach (Models.Api.Register item in register.containedSubRegisters.ToList())
                {
                    ConvertRegisterToCSV(streamWriter, item);
                }
            }
        }

        private static void ConvertRegisterItemToCSV(StreamWriter streamWriter, Models.Api.Registeritem item)
        {
            item.description = RemoveBreaksFromDescription(item.description);
            string text = null;
            if (item.itemclass == "Document")
            {
                text = item.label + ";" + item.owner + ";" + item.status + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.documentreference + ";" + item.id;
            }
            else if (item.itemclass == "CodelistValue")
            {
                text = item.label + ";" + item.codevalue +";" + item.owner + ";" + item.status + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.id;
            }
            else if (item.itemclass == "EPSG")
            {
                text = item.label + ";" + item.epsgcode + ";" + item.sosiReferencesystem + ";" + item.verticalReferenceSystem + ";" + item.horizontalReferenceSystem + ";" + item.dimension + ";" + item.owner + ";" + item.status + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.id;
            }
            else if (item.itemclass == "Dataset")
            {
                text = item.theme + ";" + item.label + ";" + item.owner + ";" + item.dokStatus + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + ";" + item.versionNumber + ";" + item.description + ";" + item.id;
             }

            streamWriter.WriteLine(text);
        }

        private static void ConvertRegisterToCSV(StreamWriter streamWriter, Models.Api.Register register)
        {
            register.contentsummary = RemoveBreaksFromDescription(register.contentsummary);
            string text = register.id + ";" + register.label + ";" + register.contentsummary + ";" + register.owner + ";" + register.lastUpdated.ToString("dd/MM/yyyy");
            streamWriter.WriteLine(text);
        }

        private static string RemoveBreaksFromDescription(string description)
        {
            string replaceWith = " ";
            if (!string.IsNullOrWhiteSpace(description))
            {
                description = description.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
            }
            else
            {
                description = "ikke angitt";
            }

            return description;
        }

        private string RegisterItemHeading(string containedItemClass)
        {
            if (containedItemClass == "Document")
            {
                return "Navn; Eier; Status; Oppdatert; Versjons Id; Beskrivelse; Dokumentreferanse; ID";
            }
            else if (containedItemClass == "CodelistValue")
            {
                return "Navn; Kode; Eier; Status; Oppdatert; Versjons Id; Beskrivelse; ID";
            }
            else if (containedItemClass == "EPSG")
            {
                return "Tittel; EPSG; SOSI; Vertikalt; Horisontalt; Dimensjon; Eier; Status; Oppdatert; Versjons Id; Beskrivelse; ID";
            }
            else if (containedItemClass == "Dataset")
            {
                return "Temagruppe; Navn; Eier; DOK-status; Oppdatert; Versjons Id; Beskrivelse; ID";
            }
            return null;
        }

        private string RegisterHeading()
        {
            return "Id ;Navn; Beskrivelse; Eier; Oppdatert";
        }
    }
}