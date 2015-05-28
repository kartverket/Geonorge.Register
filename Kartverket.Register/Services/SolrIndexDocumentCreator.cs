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
                indexDoc.CodelistValue = item.CodelistValue;
                indexDoc.currentVersion = item.currentVersion;
                indexDoc.DatasetOwner = item.DatasetOwner;
                indexDoc.Discriminator = item.Discriminator;
                indexDoc.DocumentOwner = item.DocumentOwner;
                indexDoc.ObjektkatalogUrl = item.ObjektkatalogUrl;
                indexDoc.ParentRegisterDescription = item.ParentRegisterDescription;
                indexDoc.ParentRegisterId = item.ParentRegisterId;
                indexDoc.ParentRegisterName = item.ParentRegisterName;
                indexDoc.ParentregisterOwner = item.ParentregisterOwner;
                indexDoc.ParentRegisterSeoname = item.ParentRegisterSeoname;
                indexDoc.ParentRegisterUrl = item.ParentRegisterUrl;
                indexDoc.RegisteItemUrl = item.RegisteItemUrl;
                indexDoc.RegisteItemUrlDataset = item.RegisteItemUrlDataset;
                indexDoc.RegisteItemUrlDocument = item.RegisteItemUrlDocument;
                indexDoc.RegisterDescription = item.RegisterDescription;
                indexDoc.RegisterID = item.RegisterID;
                indexDoc.RegisterItemDescription = item.RegisterItemDescription;
                indexDoc.RegisterItemName = item.RegisterItemName;
                indexDoc.RegisterItemSeoname = item.RegisterItemSeoname;
                indexDoc.RegisterItemStatus = item.RegisterItemStatus;
                indexDoc.RegisterItemUpdated = item.RegisterItemUpdated;
                indexDoc.RegisterName = item.RegisterName;
                indexDoc.RegisterSeoname = item.RegisterSeoname;
                indexDoc.Shortname = item.Shortname;
                indexDoc.Submitter = item.Submitter;
                indexDoc.subregisterItemUrl = item.subregisterItemUrl;
                indexDoc.SubregisterUrl = item.SubregisterUrl;
                indexDoc.Type = getTypeName(item.Discriminator);

                Log.Info(string.Format("Indexing register with systemID={0}", indexDoc.SystemID));


            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return indexDoc;
        }


        public static string getTypeName(string containedItemClass)
        {
            if (containedItemClass == "Document")
            {
                return "Dokumenter";
            }
            else if (containedItemClass == "Dataset")
            {
                return "Datasett";
            }
            else if (containedItemClass == "EPSG")
            {
                return "EPSG-koder";
            }
            else if (containedItemClass == "Organization")
            {
                return "Organisasjoner";
            }
            else if (containedItemClass == "CodelistValue")
            {
                return "Kodeverdier";
            }
            else if (containedItemClass == "Register")
            {
                return "Registre";
            }

            return null;
        }

    }
}