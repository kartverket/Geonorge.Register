
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
    public class SyndicationFeedFormatter : MediaTypeFormatter
    {
        private readonly string atom = "application/atom+xml";
        private readonly string rss = "application/rss+xml";

        public SyndicationFeedFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(atom));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(rss));
            MediaTypeMappings.Add(new UriPathExtensionMapping("rss", rss));
            MediaTypeMappings.Add(new UriPathExtensionMapping("atom", atom));
        }

        Func<Type, bool> SupportedType = (type) =>
        {
            if (type == typeof(Item) || type == typeof(IEnumerable<Item>) || type == typeof(List<Item>) || type == typeof(Models.Api.Register) || type == typeof(IEnumerable<Models.Api.Register>) || type == typeof(List<Models.Api.Register>))
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
                if (type == typeof(Item) || type == typeof(IEnumerable<Item>) || type == typeof(List<Item>))
                    BuildSyndicationFeed(value, writeStream, content.Headers.ContentType.MediaType);
                else if (type == typeof(Models.Api.Register) || type == typeof(IEnumerable<Models.Api.Register>) || type == typeof(List<Models.Api.Register>))
                    BuildSyndicationFeedRegister(value, writeStream, content.Headers.ContentType.MediaType);
            });
        }

        private void BuildSyndicationFeed(object models, Stream stream, string contenttype)
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            var feed = new SyndicationFeed()
            {
                Title = new TextSyndicationContent("Register Feed")
            };

            if (models is IEnumerable<Item>)
            {
                var enumerator = ((IEnumerable<Item>)models).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    items.Add(BuildSyndicationItem(enumerator.Current));
                }
            }
            else
            {
                items.Add(BuildSyndicationItem((Item)models));
            }

            feed.Items = items;

            using (XmlWriter writer = XmlWriter.Create(stream))
            {
                if (string.Equals(contenttype, atom))
                {
                    Atom10FeedFormatter atomformatter = new Atom10FeedFormatter(feed);
                    atomformatter.WriteTo(writer);
                }
                else
                {
                    Rss20FeedFormatter rssformatter = new Rss20FeedFormatter(feed);
                    rssformatter.WriteTo(writer);
                }
            }
        }

        private SyndicationItem BuildSyndicationItem(Item u)
        {
            var item = new SyndicationItem()
            {
                Title = new TextSyndicationContent(u.name),
                BaseUri = new Uri(u.showUrl),
                LastUpdatedTime = u.updated,
                Content = new TextSyndicationContent(u.description)
            };
            item.Authors.Add(new SyndicationPerson() { Name = u.author });
            return item;
        }

        private void BuildSyndicationFeedRegister(object models, Stream stream, string contenttype)
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            var feed = new SyndicationFeed();
           

            if (models is IEnumerable<Models.Api.Register>)
            {

                feed.Title = new TextSyndicationContent("Register Feed");
           
                var enumerator = ((IEnumerable<Models.Api.Register>)models).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    items.Add(BuildSyndicationRegister(enumerator.Current));
                }
            }
            else
            {
                feed.Title = new TextSyndicationContent(((Models.Api.Register)models).label);
                feed.LastUpdatedTime = ((Models.Api.Register)models).lastUpdated;
                feed.Id = ((Models.Api.Register)models).id;
                feed.Links.Add(new SyndicationLink() { Title = ((Models.Api.Register)models).label, Uri = new Uri((((Models.Api.Register)models).id)) });
                foreach (var item in ((Models.Api.Register)models).containeditems)
                {
                    items.Add(BuildSyndicationRegisterItem(item));
                }
                
            }

            feed.Items = items;

            using (XmlWriter writer = XmlWriter.Create(stream))
            {
                if (string.Equals(contenttype, atom))
                {
                    Atom10FeedFormatter atomformatter = new Atom10FeedFormatter(feed);
                    XNamespace atom = "http://www.w3.org/2005/Atom";
                    feed.ElementExtensions.Add(
                        new XElement(atom + "link",
                        new XAttribute("href", ((Models.Api.Register)models).id.Replace("/varsler", "/api/varsler.atom?")),
                        new XAttribute("rel", "self"),
                        new XAttribute("type", "application/atom+xml")));
                    atomformatter.WriteTo(writer);
                }
                else
                {
                    Rss20FeedFormatter rssformatter = new Rss20FeedFormatter(feed, false);
                    XNamespace atom = "http://www.w3.org/2005/Atom";
                    feed.ElementExtensions.Add(
                        new XElement(atom + "link",
                        new XAttribute("href", ((Models.Api.Register)models).id.Replace("/varsler", "/api/varsler.rss?")),
                        new XAttribute("rel", "self"),
                        new XAttribute("type", "application/rss+xml")));

                    feed.ElementExtensions.Add(
                         new XElement("link", ((Models.Api.Register)models).id));
                    rssformatter.WriteTo(writer);
                }
            }
        }
        private SyndicationItem BuildSyndicationRegisterItem(Kartverket.Register.Models.Api.Registeritem u)
        {
            var content = GetContent(u);
            var item = new SyndicationItem()
            {
                Title = new TextSyndicationContent(u.label),
                BaseUri = new Uri(GetBaseUri(u.id)),
                LastUpdatedTime = u.lastUpdated,
                PublishDate = u.dateSubmitted,
                Content = new TextSyndicationContent(content, TextSyndicationContentKind.Html),
                Id = FixSpecialCharacters(u.id)
            };
            item.Links.Add(new SyndicationLink() { Title = u.label, Uri = new Uri((u.id)) });
            item.Authors.Add(new SyndicationPerson() { Name = u.owner });
            item.Categories.Add(new SyndicationCategory() { Name = u.status });
            item.Categories.Add(new SyndicationCategory() { Name = u.owner });
            //if (u.itemclass == "Alert" && !string.IsNullOrEmpty(u.ServiceUuid))
            //    item.ElementExtensions.Add(new XElement("uuid", u.ServiceUuid));
            return item;
        }

        private string GetBaseUri(string id)
        {
            if (!string.IsNullOrEmpty(id)) 
            {
                var url = new Uri(id);
                var host = url.Host;
                return $"https://{host}/varsler"; 
            }
            return id;
        }

        private string FixSpecialCharacters(string id)
        {
            id = id.Replace("æ", "%C3%A6");
            id = id.Replace("ø", "%C3%B8");
            id = id.Replace("å", "%C3%A5");

            return id;
        }

        private string GetContent(Registeritem u)
        {
            var content = u.description;

            if(u.itemclass == "Alert")
            {
                if (!string.IsNullOrEmpty(content))
                    content = content + "<br>";

                if (!string.IsNullOrEmpty(u.owner))
                    content = content + "Dataeier: " + u.owner + "<br>";

                if (!string.IsNullOrEmpty(u.AlertType))
                    content = content + "Type varsel: " + u.AlertType + "<br>";

                if (!string.IsNullOrEmpty(u.ServiceType))
                    content = content + "Tjenestetype: " + u.ServiceType + "<br>";

                if (u.AlertDate != null)
                    content = content + "Siste varsel: " + u.AlertDate.ToString("dd.MM.yyyy") + "<br>";

                if (u.EffectiveDate != null)
                    content = content + "Ikrafttredelse: " + u.EffectiveDate.ToString("dd.MM.yyyy") + "<br>";

                if (!string.IsNullOrEmpty(u.Note))
                    content = content + "Varselet gjelder: " + u.Note + "<br>";

                if (!string.IsNullOrEmpty(u.id))
                    content = content + "Link til tjenestevarsel: " + "<a href='" + u.id + "'>" + u.label + "</a><br>";

                if (!string.IsNullOrEmpty(u.MetadataUrl))
                    content = content + "Link til tjenestebeskrivelse: " + "<a href='" + u.MetadataUrl + "'>" +u.MetadataUrl + "</a><br>";

                if (!string.IsNullOrEmpty(u.Summary))
                    content = content + "Summary: " + u.Summary + "<br>";

                if (u.DateResolved.HasValue)
                    content = content + "DateResolved: " + u.DateResolved.Value + "<br>";

                if (u.Station != null)
                    content = content + "Station: " + u.Station + "<br>";
            }

            if (content != null)
                content = System.Web.HttpUtility.HtmlEncode(content);

            return content;
        }

        private SyndicationItem BuildSyndicationRegister(Models.Api.Register u)
        {
            var item = new SyndicationItem()
            {
                Title = new TextSyndicationContent(u.label),
                BaseUri = new Uri(u.id),
                LastUpdatedTime = u.lastUpdated,
                Content = new TextSyndicationContent(u.contentsummary),
                Id = u.id
            };
            item.Authors.Add(new SyndicationPerson() { Name = u.manager });
            return item;
        }
    }
}