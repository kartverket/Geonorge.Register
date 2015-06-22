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
            try 
            { 
                Log.Info(string.Format("Indexing {0} docs", docs.Count()));
                _solr.AddRange(docs);
                _solr.Commit();
            }
            catch (SolrNet.Exceptions.SolrConnectionException)
            {
                Log.Error("Connection to solr failed");
            }
            catch (Exception ex)
            {
                Log.Error("Error:" + ex.Message);
            }
        }

        public void DeleteIndex()
        {
            try
            { 
                Log.Info("Deletes intire index for reindexing");
                SolrQuery sq = new SolrQuery("*:*");
                _solr.Delete(sq);
                _solr.Commit();
            }
            catch (SolrNet.Exceptions.SolrConnectionException)
            {
                Log.Error("Connection to solr failed");
            }
            catch (Exception ex)
            {
                Log.Error("Error:" + ex.Message);
            }
        }

        public void Index(RegisterIndexDoc doc)
        {
            Log.Info(string.Format("Indexing single document systemID={0}", doc.SystemID));
            try
            {
                _solr.Add(doc);
                _solr.Commit();
            }
            catch (SolrNet.Exceptions.SolrConnectionException)
            {
                Log.Error("Connection to solr failed");
            }
            catch (Exception ex)
            {
                Log.Error("Error:" + ex.Message);
            }
        }


        public void RemoveIndexDocument(string systemID)
        {
            try
            {
                Log.Info(string.Format("Removes document systemID={0} from index", systemID));
                _solr.Delete(systemID);
                _solr.Commit();
            }
            catch (SolrNet.Exceptions.SolrConnectionException)
            {
                Log.Error("Connection to solr failed");
            }
            catch (Exception ex)
            {
                Log.Error("Error:" + ex.Message);
            }
        }
    }
}