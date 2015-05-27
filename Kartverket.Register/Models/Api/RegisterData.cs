using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class RegisterData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string RegisterName { get; set; }
        public string RegisterDescription { get; set; }
        public string RegisterUrl { get; set; }


        public RegisterData(SearchResultItem item)
        {
            Id = item.SystemID.ToString();
            Name = item.RegisterItemName;
            Url = item.RegisteItemUrl;
            Description = item.RegisterItemDescription;
            Type = item.Discriminator;
            RegisterName = item.RegisterName;
            RegisterDescription = item.RegisterDescription;
            RegisterUrl = item.ParentRegisterUrl;
            
        }

        public static List<RegisterData> CreateFromList(IEnumerable<SearchResultItem> items)
        {
            return items.Select(item => new RegisterData(item)).ToList();
        }
    }
}