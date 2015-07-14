
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
            MediaTypeMappings.Add(new UriPathExtensionMapping("rss", "application/rss+xml"));
            MediaTypeMappings.Add(new UriPathExtensionMapping("atom", "application/atom+xml"));
        }

        Func<Type, bool> SupportedType = (type) =>
        {
            if (type == typeof(Item) || type == typeof(IEnumerable<Item>) || type == typeof(List<Item>) || type == typeof(Kartverket.Register.Models.Api.Register) || type == typeof(IEnumerable<Kartverket.Register.Models.Api.Register>) || type == typeof(List<Kartverket.Register.Models.Api.Register>))
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
                else if (type == typeof(Kartverket.Register.Models.Api.Register) || type == typeof(IEnumerable<Kartverket.Register.Models.Api.Register>) || type == typeof(List<Kartverket.Register.Models.Api.Register>))
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
           

            if (models is IEnumerable<Kartverket.Register.Models.Api.Register>)
            {

                feed.Title = new TextSyndicationContent("Register Feed");
           
                var enumerator = ((IEnumerable<Kartverket.Register.Models.Api.Register>)models).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    items.Add(BuildSyndicationRegister(enumerator.Current));
                }
            }
            else
            {
                feed.Title = new TextSyndicationContent(((Kartverket.Register.Models.Api.Register)models).label);
                feed.LastUpdatedTime = ((Kartverket.Register.Models.Api.Register)models).lastUpdated;
                feed.Id = ((Kartverket.Register.Models.Api.Register)models).id;
                feed.Links.Add(new SyndicationLink() { Title = ((Kartverket.Register.Models.Api.Register)models).label, Uri = new Uri((((Kartverket.Register.Models.Api.Register)models).id)) });
                foreach (var item in ((Kartverket.Register.Models.Api.Register)models).containeditems)
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
                    atomformatter.WriteTo(writer);
                }
                else
                {
                    Rss20FeedFormatter rssformatter = new Rss20FeedFormatter(feed);
                    rssformatter.WriteTo(writer);
                }
            }
        }
        private SyndicationItem BuildSyndicationRegisterItem(Kartverket.Register.Models.Api.Registeritem u)
        {
            var item = new SyndicationItem()
            {
                Title = new TextSyndicationContent(u.label),
                BaseUri = new Uri(u.id),
                LastUpdatedTime = u.lastUpdated,
                Content = new TextSyndicationContent(u.description),
                Id = u.id
            };
            item.Links.Add(new SyndicationLink() { Title = u.label, Uri = new Uri((u.id)) });
            item.Authors.Add(new SyndicationPerson() { Name = u.owner });
            item.Categories.Add(new SyndicationCategory() { Name = u.status });
            return item;
        }

        private SyndicationItem BuildSyndicationRegister(Kartverket.Register.Models.Api.Register u)
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