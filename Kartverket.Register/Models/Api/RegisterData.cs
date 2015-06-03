using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class RegisterData
    {
        /// <summary>
        /// The uniqueidentifier
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The name, title for the register item
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The url to view the register item
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// The description for register item
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The type of register
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The name of the register
        /// </summary>
        public string RegisterName { get; set; }
        /// <summary>
        /// The description of the register
        /// </summary>
        public string RegisterDescription { get; set; }
        /// <summary>
        /// The url of the register
        /// </summary>
        public string RegisterUrl { get; set; }
        /// <summary>
        /// Register owner/responsible
        /// </summary>
        public string Organization { get; set; }


        public RegisterData(SearchResultItem item)
        {
            Id = item.SystemID.ToString();
            Name = item.RegisterItemName;
            Url = item.RegisteItemUrl;
            Description = item.RegisterItemDescription;
            Type = item.Type;
            RegisterName = item.RegisterName;
            RegisterDescription = item.RegisterDescription;
            RegisterUrl = item.ParentRegisterUrl;
            Organization = item.organization;
            
        }

        public static List<RegisterData> CreateFromList(IEnumerable<SearchResultItem> items)
        {
            return items.Select(item => new RegisterData(item)).ToList();
        }
    }
}