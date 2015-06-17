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
    public class SKOSFormatter : MediaTypeFormatter
    {
        private readonly string rdf = "application/rdf+xml";

        public SKOSFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(rdf));
        }

        Func<Type, bool> SupportedTypeSKOS = (type) =>
        {
            if (type == typeof(Kartverket.Register.Models.Api.Register) || type == typeof(Kartverket.Register.Models.Api.Registeritem))
                return true;
            else
                return false;
        };

        public override bool CanReadType(Type type)
        {
            return SupportedTypeSKOS(type);
        }

        public override bool CanWriteType(Type type)
        {
            return SupportedTypeSKOS(type);
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
            ConceptSheme conceptSheme = new ConceptSheme(models);

            XNamespace ns = "http://www.opengis.net/gml/3.2";
            XNamespace skosNs = "http://www.w3.org/2004/02/skos/core#";
            XNamespace rdfNs = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
            XNamespace dctermsNs = "http://purl.org/dc/terms/";
            XNamespace baseXML = conceptSheme.id.Replace(conceptSheme.seoname, null);

            XElement xdoc =
                new XElement(rdfNs + "RDF", new XAttribute(XNamespace.Xmlns + "skos", skosNs),
                    new XAttribute(XNamespace.Xmlns + "rdf", rdfNs),
                    new XAttribute(XNamespace.Xmlns + "dcterms", dctermsNs),
                    new XAttribute(XNamespace.Xml + "base", baseXML),
                    new XElement(skosNs + "ConceptScheme", new XAttribute(rdfNs + "about", conceptSheme.seoname),
                        new XElement(skosNs + "prefLabel", conceptSheme.name, new XAttribute(XNamespace.Xml + "lang", "no")),
                        new XElement(dctermsNs + "description", conceptSheme.description, new XAttribute(XNamespace.Xml + "lang", "no")),
                        new XElement(dctermsNs + "source", new XAttribute(rdfNs + "resource", conceptSheme.id))
                        ),

                    from c in conceptSheme.concepts
                    select new XElement(skosNs + "Concept", new XAttribute(rdfNs + "about", conceptSheme.seoname + "/" + c.owner +  "/" + c.seoname), 
                        new XElement(skosNs + "inSheme", new XAttribute(rdfNs + "resource", conceptSheme.seoname)),
                        new XElement(skosNs + "topConceptOf", new XAttribute(rdfNs + "resource", conceptSheme.seoname)),
                        new XElement(skosNs + "prefLabel", c.name, new XAttribute(XNamespace.Xml + "lang", "no")),
                        new XElement(dctermsNs + "description", c.codevalue, new XAttribute(XNamespace.Xml + "lang", "no")),
                        new XElement(skosNs + "broader", new XAttribute(rdfNs + "resource", c.broader)),                        
                        //new XElement(skosNs + "narrower", new XAttribute(rdfNs + "resource", "narrowerItem")), 
                        new XElement(dctermsNs + "source", new XAttribute(rdfNs + "resource", c.id))
                      ));

            using (XmlWriter writer = XmlWriter.Create(stream))
            {
                xdoc.WriteTo(writer);
            }
        }
    }
}