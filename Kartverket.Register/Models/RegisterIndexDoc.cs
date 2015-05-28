using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SolrNet.Attributes;

namespace Kartverket.Register.Models
{
    public class RegisterIndexDoc
    {
        [SolrField("RegisterName")]
        public string RegisterName { get; set; }

        [SolrField("RegisterDescription")]
        public string RegisterDescription { get; set; }

        [SolrField("RegisterItemName")]
        public string RegisterItemName { get; set; }

        [SolrField("RegisterItemDescription")]
        public string RegisterItemDescription { get; set; }

        [SolrField("RegisterID")]
        public Guid RegisterID { get; set; }

        [SolrUniqueKey("SystemID")]
        public Guid SystemID { get; set; }

        [SolrField("Discriminator")]
        public string Discriminator { get; set; }

        [SolrField("RegisterSeoname")]
        public string RegisterSeoname { get; set; }

        [SolrField("RegisterItemSeoname")]
        public string RegisterItemSeoname { get; set; }

        [SolrField("RegisterItemStatus")]
        public string RegisterItemStatus { get; set; }

        [SolrField("DocumentOwner")]
        public string DocumentOwner { get; set; }

        [SolrField("RegisterItemUpdated")]
        public DateTime RegisterItemUpdated { get; set; }

        [SolrField("RegisteItemUrl")]
        public string RegisteItemUrl { get; set; }

        [SolrField("SubregisterUrl")]
        public string SubregisterUrl { get; set; }

        [SolrField("RegisteItemUrlDocument")]
        public string RegisteItemUrlDocument { get; set; }

        [SolrField("RegisteItemUrlDataset")]
        public string RegisteItemUrlDataset { get; set; }

        [SolrField("subregisterItemUrl")]
        public string subregisterItemUrl { get; set; }

        [SolrField("ParentRegisterUrl")]
        public string ParentRegisterUrl { get; set; }

        [SolrField("ObjektkatalogUrl")]
        public string ObjektkatalogUrl { get; set; }

        [SolrField("Submitter")]
        public string Submitter { get; set; }

        [SolrField("Shortname")]
        public string Shortname { get; set; }

        [SolrField("DatasetOwner")]
        public string DatasetOwner { get; set; }

        [SolrField("ParentRegisterId")]
        public Guid? ParentRegisterId { get; set; }

        [SolrField("ParentRegisterName")]
        public string ParentRegisterName { get; set; }

        [SolrField("ParentRegisterDescription")]
        public string ParentRegisterDescription { get; set; }

        [SolrField("ParentRegisterSeoname")]
        public string ParentRegisterSeoname { get; set; }

        [SolrField("ParentregisterOwner")]
        public string ParentregisterOwner { get; set; }

        [SolrField("CodelistValue")]
        public string CodelistValue { get; set; }

        [SolrField("type")]
        public string Type { get; set; }

        [SolrField("currentVersion")]
        public Guid? currentVersion { get; set; }

        //Search score?
        [SolrField("score")]
        public double? Score { get; set; }

        [SolrField("theme")]
        public string theme { get; set; }

        [SolrField("organization")]
        public string Organization { get; set; }

    }
}