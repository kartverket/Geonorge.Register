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
        private readonly XNamespace ns = "http://www.opengis.net/gml/3.2";
        private readonly XNamespace skosNs = "http://www.w3.org/2004/02/skos/core#";
        private readonly XNamespace rdfNs = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        private readonly XNamespace dctermsNs = "http://purl.org/dc/terms/";
        private readonly XNamespace admsNs = "http://www.w3.org/ns/adms#";
        private readonly XNamespace dcatNs = "http://www.w3.org/ns/dcat#";

        public SKOSFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(rdf));
            MediaTypeMappings.Add(new UriPathExtensionMapping("rdf", rdf));
        }

        Func<Type, bool> SupportedTypeSKOS = (type) =>
        {
            if (type == typeof(Item) ||
                type == typeof(IEnumerable<Item>) ||
                type == typeof(List<Item>) ||
                type == typeof(Models.Api.Register) ||
                type == typeof(Kartverket.Register.Models.Api.Registeritem) ||
                type == typeof(IEnumerable<Models.Api.Register>) ||
                type == typeof(List<Models.Api.Register>) ||
                type == typeof(IEnumerable<Kartverket.Register.Models.Api.Registeritem>) ||
                type == typeof(List<Kartverket.Register.Models.Api.Registeritem>))

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
                if (type == typeof(Item) ||
                type == typeof(IEnumerable<Item>) ||
                type == typeof(List<Item>) ||
                type == typeof(Models.Api.Register) ||
                type == typeof(Kartverket.Register.Models.Api.Registeritem) ||
              
                type == typeof(IEnumerable<Kartverket.Register.Models.Api.Registeritem>) ||
                type == typeof(List<Kartverket.Register.Models.Api.Registeritem>))
                    BuildSKOSFeed(value, writeStream, content.Headers.ContentType.MediaType);
            });
        }

        private void BuildSKOSFeed(object models, Stream stream, string contenttype)
        {
            ConceptSheme conceptSheme = new ConceptSheme(models);

            //TODO! Relative lenker....
            XNamespace baseXML = conceptSheme.id.Replace(conceptSheme.seoname, null);

            XElement xdoc =
                new XElement(rdfNs + "RDF", new XAttribute(XNamespace.Xmlns + "skos", skosNs),
                    new XAttribute(XNamespace.Xmlns + "rdf", rdfNs),
                    new XAttribute(XNamespace.Xmlns + "dcterms", dctermsNs),
                    new XAttribute(XNamespace.Xmlns + "adms", admsNs),
                    new XAttribute(XNamespace.Xmlns + "dcat", dcatNs),
                    new XAttribute(XNamespace.Xml + "base", baseXML),
                    new XElement(skosNs + "ConceptScheme", new XAttribute(rdfNs + "about", conceptSheme.id),
                        GetElementsForConceptScheme(conceptSheme)
                        ),

                    from c in conceptSheme.concepts
                    select new XElement(skosNs + "Concept", new XAttribute(rdfNs + "about", c.id),
                        GetElementsForConcept(conceptSheme, c)
                      ));

            using (XmlWriter writer = XmlWriter.Create(stream))
            {
                xdoc.WriteTo(writer);

            }
        }

        private List<object> GetElementsForConceptScheme(ConceptSheme conceptSheme)
        {
            if (!conceptSheme.concepts.Any() && !conceptSheme.narrower.Any())
            {
                return GetElementsForConcept(conceptSheme, null);
            }

            List<object> elements = new List<object> {
            new XElement(skosNs + "prefLabel", conceptSheme.name, new XAttribute(XNamespace.Xml + "lang", "no")),
            new XElement(dctermsNs + "description", conceptSheme.description, new XAttribute(XNamespace.Xml + "lang", "no")),
            new XElement(dctermsNs + "source", new XAttribute(rdfNs + "resource", conceptSheme.id)),
            };

            if (!string.IsNullOrWhiteSpace(conceptSheme.broader))
            {
                elements.Add(new XElement(skosNs + "broader", new XAttribute(rdfNs + "resource", conceptSheme.broader)));
            }
            foreach (var item in conceptSheme.narrower)
            {
                elements.Add(new XElement(skosNs + "narrower", new XAttribute(rdfNs + "resource", item)));
            }
            return elements;
        }

        private List<object> GetElementsForConcept(ConceptSheme conceptSheme, Concept c)
        {
            var inScheme = new XElement(skosNs + "inScheme", new XAttribute(rdfNs + "resource", conceptSheme.id));
            var topConceptOf = new XElement(skosNs + "topConceptOf", new XAttribute(rdfNs + "resource", conceptSheme.id));

            if (c== null)
            {
                inScheme = null;
                topConceptOf = null;
                c = GetConceptFromScheme(conceptSheme);
            }

            List<object> elements = new List<object> {
                inScheme,
                topConceptOf,
                new XElement(skosNs + "prefLabel", c.name, new XAttribute(XNamespace.Xml + "lang", "no")),
                new XElement(skosNs + "Definition", c.description),
                new XElement(dctermsNs + "description", c.description, new XAttribute(XNamespace.Xml + "lang", "no")),
                new XElement(dctermsNs + "identifier", c.codevalue),
                new XElement(admsNs + "status", c.status),
                GetTemporal(c),
                new XElement(dctermsNs + "source", new XAttribute(rdfNs + "resource", c.id)),
            };

            if (!string.IsNullOrWhiteSpace(c.broader))
            {
                elements.Add(new XElement(skosNs + "broader", new XAttribute(rdfNs + "resource", c.broader)));
            }
            return elements;
        }

        private Concept GetConceptFromScheme(ConceptSheme conceptSheme)
        {
            Concept concept = new Concept(conceptSheme);
            return concept;
        }

        private object GetTemporal(Concept c)
        {
            XElement temporal = null;
            XElement startDate = null;
            XElement endDate = null;

            if (c.ValidFromDate.HasValue || c.ValidToDate.HasValue)
            {
                if (c.ValidFromDate.HasValue)
                    startDate = new XElement(dcatNs + "startDate", c.ValidFromDate);

                if (c.ValidToDate.HasValue)
                    endDate = new XElement(dcatNs + "endDate", c.ValidToDate);

                temporal = new XElement(dctermsNs + "Temporal", 
                    new XElement(dctermsNs + "PeriodeOfTime", startDate, endDate));

               return temporal;
            }

            return null;
        }
    }
}