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
                indexDoc.RegisterItemNameEnglish = item.RegisterItemNameEnglish;
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
                indexDoc.Organization = getOrganizationName(item);

                Log.Info(string.Format("Indexing register with systemID={0}", indexDoc.SystemID));


            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return indexDoc;
        }

        private string getOrganizationName(SearchResultItem item)
        {
            string organizationSEO = "";
            if (!string.IsNullOrWhiteSpace(item.DocumentOwner))
                organizationSEO = item.DocumentOwner;
            else if (!string.IsNullOrWhiteSpace(item.DatasetOwner))
                organizationSEO = item.DatasetOwner;
            else if (!string.IsNullOrWhiteSpace(item.ParentregisterOwner))
                organizationSEO = item.ParentregisterOwner;
            else
                organizationSEO = item.Submitter;

            return mapOrganizationSeoName(organizationSEO);
        }

        private string mapOrganizationSeoName(string seoName)
        {
            RegisterDbContext db = new RegisterDbContext();

            string organizationName = seoName;

            var queryResultsRegisterItem = from o in db.RegisterItems
                                           where o.seoname == seoName
                                           select o.name;

            var name = queryResultsRegisterItem.FirstOrDefault();

            organizationName = name.ToString();

            db.Dispose();
            
            return organizationName;
        }


        public static string getTypeName(string containedItemClass)
        {
            if (containedItemClass == "Document")
            {
                return "dokumenter";
            }
            else if (containedItemClass == "Dataset")
            {
                return "datasett";
            }
            else if (containedItemClass == "EPSG")
            {
                return "epsg-koder";
            }
            else if (containedItemClass == "Organization")
            {
                return "organisasjoner";
            }
            else if (containedItemClass == "CodelistValue")
            {
                return "kodeverdier";
            }
            else if (containedItemClass == "Register")
            {
                return "registre";
            }

            return null;
        }

    }
}