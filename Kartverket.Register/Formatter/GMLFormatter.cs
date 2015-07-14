using Kartverket.Register.Models;
using Kartverket.Register.Models.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Kartverket.Register.Formatter
{
    public class GMLFormatter : MediaTypeFormatter
    {
        private readonly string gml = "application/xml";

        public GMLFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(gml));
            MediaTypeMappings.Add(new UriPathExtensionMapping("gml", gml));
        }

        Func<Type, bool> SupportedType = (type) =>
        {
            if (type == typeof(Kartverket.Register.Models.Api.Register) || type == typeof(Kartverket.Register.Models.Api.Registeritem))
                return true;
            else
                return false;
        };

        public override bool CanReadType(Type type)
        {
            return SupportedType(type);
        }

        public override bool CanWriteType(Type type)
        {
            return SupportedType(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                if (type == typeof(Kartverket.Register.Models.Api.Register) || type == typeof(Kartverket.Register.Models.Api.Registeritem))
                    BuildSKOSFeed(value, writeStream, content.Headers.ContentType.MediaType);                
            });
        }

        private void BuildSKOSFeed(object models, Stream stream, string contenttype)
            {
                if (models is Kartverket.Register.Models.Api.Register)
                {
                    Kartverket.Register.Models.Api.Register register = (Kartverket.Register.Models.Api.Register)models;

                    string targetNamespace = "";
                    string nameSpace = "";
                    if (register.targetNamespace != null)
                    {
                        nameSpace = register.targetNamespace;
                        if (register.targetNamespace.EndsWith("/"))
                        {
                            targetNamespace = register.targetNamespace + register.label;
                        }
                        else
                        {
                            targetNamespace = register.targetNamespace + "/" + register.label;
                        }
                    }

                    XNamespace ns = "http://www.opengis.net/gml/3.2";
                    XNamespace xsiNs = "http://www.w3.org/2001/XMLSchema-instance";
                    XNamespace gmlNs = "http://www.opengis.net/gml/3.2";
                    XElement xdoc =
                        new XElement(gmlNs + "Dictionary", new XAttribute(XNamespace.Xmlns + "xsi", xsiNs),
                            new XAttribute(XNamespace.Xmlns + "gml", gmlNs),
                            new XAttribute(xsiNs + "schemaLocation", "http://www.opengis.net/gml/3.2 http://schemas.opengis.net/gml/3.2.1/gml.xsd"),
                            new XAttribute(gmlNs + "id", register.id),
                            new XElement(gmlNs + "description"),
                            new XElement(gmlNs + "identifier",
                                new XAttribute("codeSpace", nameSpace), register.label),

                            from k in register.containeditems
                            select new XElement(gmlNs + "dictionaryEntry", new XElement(gmlNs + "Definition", new XAttribute(gmlNs + "id", "_" + k.codevalue),
                              new XElement(gmlNs + "description", k.description),
                              new XElement(gmlNs + "identifier", new XAttribute("codeSpace", targetNamespace), k.codevalue),
                              new XElement(gmlNs + "name", k.label)
                              )));

                    using (XmlWriter writer = XmlWriter.Create(stream))
                    {
                        xdoc.WriteTo(writer);

                    }
                }
        }
    }
}