using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Services
{
    public class SolrIndexDocumentCreator : IndexDocumentCreator
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public SolrIndexDocumentCreator()
        {

        }

        public List<RegisterIndexDoc> CreateIndexDocs(IEnumerable<object> searchResultItems)
        {
            var documentsToIndex = new List<RegisterIndexDoc>();
            foreach (var item in searchResultItems)
            {
                var register = item as SearchResultItem;
                if (register != null)
                {
                    try
                    {

                        var indexDoc = CreateIndexDoc(register);
                        if (indexDoc != null)
                        {
                            documentsToIndex.Add(indexDoc);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
            return documentsToIndex;
        }

        public RegisterIndexDoc CreateIndexDoc(SearchResultItem item)
        {
            var indexDoc = new RegisterIndexDoc();

            try
            {

                indexDoc.SystemID = item.SystemID;
                Log.Info(string.Format("Indexing register with systemID={0}", indexDoc.SystemID));


            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return indexDoc;
        }

    }
}