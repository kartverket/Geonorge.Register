using Kartverket.Register.Models;
using Kartverket.Register.Models.Api;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Kartverket.Register.Formatter
{
    public class XMLFormatter : MediaTypeFormatter
    {
        private readonly string xml = "application/xml";

        public XMLFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(xml));
            MediaTypeMappings.Add(new UriPathExtensionMapping("xml", xml));
        }

        Func<Type, bool> SupportedType = (type) =>
        {
            if (type == typeof(Models.Api.Register) || type == typeof(Registeritem) || type == typeof(List<InspireDatasetRegistery>))
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
                if (type == typeof(Models.Api.Register) || type == typeof(Kartverket.Register.Models.Api.Registeritem))
                    BuildXMLFeed(value, writeStream);
            });
        }

        private void BuildXMLFeed(object models, Stream stream)
        {

            if (models is Models.Api.Register register)
            {
                if (register.IsInspireDataset())
                {
                    var inspireDatasetRegistery = new InspireDatasetRegistery(register);
                    XmlSerializer s = new XmlSerializer(typeof(InspireDatasetRegistery));
                    using (XmlWriter writer = XmlWriter.Create(stream))
                    {
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                        ns.Add("p4", "http://inspire.jrc.ec.europa.eu/monitoringreporting/rowdata");
                        ns.Add("p3", "http://inspire.jrc.ec.europa.eu/monitoringreporting/indicators");
                        ns.Add("p2", "http://inspire.jrc.ec.europa.eu/monitoringreporting/monitoringmd");
                        ns.Add("p1", "http://inspire.jrc.ec.europa.eu/monitoringreporting/basictype");
                        ns.Add("p", "http://inspire.jrc.ec.europa.eu/monitoringreporting/monitoring");
                        s.Serialize(writer, inspireDatasetRegistery, ns);
                    }
                }
                else
                {
                    XmlSerializer s = new XmlSerializer(typeof(Models.Api.Register));

                    using (XmlWriter writer = XmlWriter.Create(stream))
                    {
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("i", "http://www.w3.org/2001/XMLSchema-instance");
                        s.Serialize(writer, register, ns);
                    }
                }
            }
            else if (models is Registeritem registerItem)
            {
                if (registerItem.IsInspireDataset())
                {
                    var inspireDataset = new Models.Api.InspireDataset(registerItem);
                    XmlSerializer s = new XmlSerializer(typeof(Models.Api.InspireDataset));
                    using (XmlWriter writer = XmlWriter.Create(stream))
                    {
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                        ns.Add("p4", "http://inspire.jrc.ec.europa.eu/monitoringreporting/rowdata");
                        ns.Add("p3", "http://inspire.jrc.ec.europa.eu/monitoringreporting/indicators");
                        ns.Add("p2", "http://inspire.jrc.ec.europa.eu/monitoringreporting/monitoringmd");
                        ns.Add("p1", "http://inspire.jrc.ec.europa.eu/monitoringreporting/basictype");
                        ns.Add("p", "http://inspire.jrc.ec.europa.eu/monitoringreporting/monitoring");
                        s.Serialize(writer, inspireDataset, ns);
                    }
                }
                else
                {
                    XmlSerializer s = new XmlSerializer(typeof(Models.Api.Registeritem));

                    using (XmlWriter writer = XmlWriter.Create(stream))
                    {
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("i", "http://www.w3.org/2001/XMLSchema-instance");
                        s.Serialize(writer, registerItem, ns);
                    }
                }
            }
        }
    }
}