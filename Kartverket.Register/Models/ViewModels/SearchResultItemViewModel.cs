using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.ViewModels
{
    public class SearchResultItemViewModel
    {
        public string RegisterName { get; set; }
        public string RegisterDescription { get; set; }
        public string RegisterItemName { get; set; }
        public string RegisterItemDescription { get; set; }
        public Guid RegisterID { get; set; }
        public Guid SystemID { get; set; }
        public string Discriminator { get; set; }
        public string RegisterSeoname { get; set; }
        public string RegisterItemSeoname { get; set; }
        public string DocumentOwner { get; set; }
        public string Submitter { get; set; }
        public string Shortname { get; set; }
        public string RegisteItemUrl { get; set; }
        public string SubregisterUrl { get; set; }
        public string subregisterItemUrl { get; set; }
        public string ParentRegisterUrl { get; set; }
        public string RegisteItemUrlDocument { get; set; }
        public string RegisteItemUrlDataset { get; set; }
        public string ObjektkatalogUrl { get; set; }   
        public string DatasetOwner { get; set; }
        public Guid? ParentRegisterId { get; set; }
        public string ParentRegisterName { get; set; }
        public string ParentRegisterDescription { get; set; }
        public string ParentRegisterSeoname { get; set; }
        public string ParentregisterOwner { get; set; }
        public string CodelistValue { get; set; }
        public string Type { get; set; }
        public Guid? currentVersion { get; set; }

        private SearchResultItemViewModel(SearchResultItem item)
        {
            RegisterName = item.RegisterName;
            RegisterDescription = item.RegisterDescription;
            RegisterItemName = item.RegisterItemName;
            RegisterItemDescription = item.RegisterItemDescription;
            RegisterID = item.RegisterID;
            SystemID = item.SystemID;
            Discriminator = item.Discriminator;
            RegisterSeoname = item.RegisterSeoname;
            RegisterItemSeoname = item.RegisterItemSeoname;
            DocumentOwner = item.DocumentOwner;
            Submitter = item.Submitter;
            Shortname = item.Shortname;
            RegisteItemUrl = item.RegisteItemUrl;
            SubregisterUrl = item.SubregisterUrl;
            subregisterItemUrl = item.subregisterItemUrl;
            ParentRegisterName = item.ParentRegisterName;
            ParentRegisterDescription = item.ParentRegisterDescription;
            ParentRegisterUrl = item.ParentRegisterUrl;
            RegisteItemUrlDataset = item.RegisteItemUrlDataset;
            RegisteItemUrlDocument = item.RegisteItemUrlDocument;
            ObjektkatalogUrl = item.ObjektkatalogUrl;
            Type = item.Type;
            currentVersion = item.currentVersion;



        }

        public static List<SearchResultItemViewModel> CreateFromList(IEnumerable<SearchResultItem> items)
        {
            return items.Select(item => new SearchResultItemViewModel(item)).ToList();
        }

    }
}