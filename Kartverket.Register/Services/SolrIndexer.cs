using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using Kartverket.Register.Models;

namespace Kartverket.Register.Services
{
    public class SolrIndexer : Indexer
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISolrOperations<RegisterIndexDoc> _solr;

        public SolrIndexer()
        {
            _solr = ServiceLocator.Current.GetInstance<ISolrOperations<RegisterIndexDoc>>();
        }

        public void Index(IEnumerable<RegisterIndexDoc> docs)
        {
            Log.Info(string.Format("Indexing {0} docs", docs.Count()));
            _solr.AddRange(docs);
            _solr.Commit();
        }

        public void DeleteIndex()
        {
            Log.Info("Deletes intire index for reindexing");
            SolrQuery sq = new SolrQuery("*:*");
            _solr.Delete(sq);
            _solr.Commit();
        }

        public void Index(RegisterIndexDoc doc)
        {
            Log.Info(string.Format("Indexing single document systemID={0}", doc.SystemID));
            _solr.Add(doc);
            _solr.Commit();
        }


        public void RemoveIndexDocument(string systemID)
        {
            Log.Info(string.Format("Removes document systemID={0} from index", systemID));
            _solr.Delete(systemID);
            _solr.Commit();
        }
    }
}