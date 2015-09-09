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
            if (type == typeof(Kartverket.Register.Models.Api.Register) ||
                type == typeof(IEnumerable<Kartverket.Register.Models.Api.Register>) ||
                type == typeof(List<Kartverket.Register.Models.Api.Register>) ||
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
            if (type == typeof(Kartverket.Register.Models.Api.Register) ||
                type == typeof(IEnumerable<Kartverket.Register.Models.Api.Register>) ||
                type == typeof(List<Kartverket.Register.Models.Api.Register>) ||
                type == typeof(IEnumerable<Kartverket.Register.Models.Api.Registeritem>) ||
                type == typeof(List<Kartverket.Register.Models.Api.Registeritem>))
                BuildCSV(value, writeStream, content.Headers.ContentType.MediaType);
        }

        private void BuildCSV(object models, Stream stream, string contenttype)
        {
            StreamWriter streamWriter = new StreamWriter(stream);
            if (models is Models.Api.Register)
            {
                Models.Api.Register register = (Models.Api.Register)models;
                ConvertRegisters(streamWriter, register);
            }
            if (models is Models.Api.Registeritem)
            {
                streamWriter.Write(RegisterItemHeading());
                Models.Api.Registeritem registerItem = (Models.Api.Registeritem)models;
                ConvertRegisterItemToCSV(streamWriter, registerItem);
            }
            if (models is List<Models.Api.Registeritem>)
            {
                streamWriter.Write(RegisterItemHeading());
                List<Models.Api.Registeritem> registerItems = (List<Models.Api.Registeritem>)models;
                ConvertRegisterItems(streamWriter, registerItems);
            }
        }

        private StringBuilder ConvertRegisterItems(StreamWriter streamWriter, List<Models.Api.Registeritem> registerItemList)
        {
            StringBuilder text = new StringBuilder(RegisterItemHeading());                

            foreach (Models.Api.Registeritem item in registerItemList)
            {
                ConvertRegisterItemToCSV(streamWriter, item);
            }
            return text;
        }

        private void ConvertRegisters(StreamWriter streamWriter, Models.Api.Register register)
        {
            if (register.containeditems != null)
            {
                streamWriter.Write(RegisterItemHeading());
                foreach (Models.Api.Registeritem item in register.containeditems.ToList())
                {
                    ConvertRegisterItemToCSV(streamWriter, item);
                }
            }
            else if (register.containedSubRegisters != null)
            {
                streamWriter.Write(RegisterHeading());
                foreach (Models.Api.Register item in register.containedSubRegisters.ToList())
                {
                    ConvertRegisterToCSV(streamWriter, item);
                }
            }
        }

        private static void ConvertRegisterItemToCSV(StreamWriter streamWriter, Models.Api.Registeritem item)
        {
            item.description = RemoveBreaksFromDescription(item.description);
            streamWriter.Write(item.id + ";" + item.label + ";" + item.itemclass + ";" + item.status + ";" + item.description + ";" + item.owner + ";" + item.lastUpdated.ToString("dd/MM/yyyy") + "\n");
        }

        private static void ConvertRegisterToCSV(StreamWriter streamWriter, Models.Api.Register register)
        {
            register.contentsummary = RemoveBreaksFromDescription(register.contentsummary);
            streamWriter.Write(register.id + ";" + register.label + ";" + register.contentsummary + ";" + register.owner + ";" + register.lastUpdated.ToString("dd/MM/yyyy") + "\n");
        }

        private static string RemoveBreaksFromDescription(string description)
        {
            string replaceWith = " ";
            description = description.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
            return description;
        }

        private string RegisterItemHeading()
        {
            return "Id ;Navn; Type; Status; Beskrivelse; Eier; Oppdatert\n";
        }

        private string RegisterHeading()
        {
            return "Id ;Navn; Beskrivelse; Eier; Oppdatert\n";
        }
    }
}