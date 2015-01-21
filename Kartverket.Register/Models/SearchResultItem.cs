using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class SearchResultItem
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
        public DateTime RegisterItemUpdated { get; set; }
    }
}