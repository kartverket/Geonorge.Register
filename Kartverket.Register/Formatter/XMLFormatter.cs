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
using Eu.Europa.Ec.Jrc.Inspire;
using Kartverket.Register.Services;

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
            if (type == typeof(Models.Api.Register) || type == typeof(Registeritem) 
            || type == typeof(Monitoring) 
            || type == typeof(SpatialDataSet))
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
                if (type == typeof(Models.Api.Register) || type == typeof(Registeritem) || type == typeof(Monitoring) || type == typeof(SpatialDataSet))
                    BuildXMLFeed(value, writeStream);
            });
        }

        private void BuildXMLFeed(object models, Stream stream)
        {
            if (models is Monitoring monitoring)
            {
                XmlSerializer s = new XmlSerializer(typeof(Monitoring));
                using (XmlWriter writer = XmlWriter.Create(stream))
                {
                    s.Serialize(writer, monitoring);
                }
            }
            else if (models is Models.Api.Register register)
            {
                XmlSerializer s = new XmlSerializer(typeof(Models.Api.Register));

                using (XmlWriter writer = XmlWriter.Create(stream))
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("i", "http://www.w3.org/2001/XMLSchema-instance");
                    s.Serialize(writer, register, ns);
                }
            }
            else if (models is Registeritem registerItem)
            {
                    XmlSerializer s = new XmlSerializer(typeof(Registeritem));

                    using (XmlWriter writer = XmlWriter.Create(stream))
                    {
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("i", "http://www.w3.org/2001/XMLSchema-instance");
                        s.Serialize(writer, registerItem, ns);
                    }                
            }
            else if (models is SpatialDataSet spatialDataSet)
            {
                XmlSerializer s = new XmlSerializer(typeof(SpatialDataSet));
                using (XmlWriter writer = XmlWriter.Create(stream))
                {
                    s.Serialize(writer, spatialDataSet);
                }
            }
        }
    }
}