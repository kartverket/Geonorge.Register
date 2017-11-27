using Kartverket.Register.Models.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    [DataContractAttribute]
    public class Register
    {
        [DataMemberAttribute]
        public string id { get; set; }
        [DataMemberAttribute]
        public string label { get; set; }
        [DataMemberAttribute]
        public string lang { get; set; } = Culture.NorwegianCode;
        [DataMemberAttribute]
        public string contentsummary { get; set; }
        [DataMemberAttribute]
        public string owner { get; set; }
        [DataMemberAttribute]
        public string manager { get; set; }
        [DataMemberAttribute]
        public string controlbody { get; set; }
        [DataMemberAttribute]
        public string containedItemClass { get; set; }
        [DataMemberAttribute]
        public Guid uuid { get; set; }
        [DataMemberAttribute]
        public List<Registeritem> containeditems { get; set; }
        [DataMemberAttribute]
        public List<Register> containedSubRegisters { get; set; }
        [DataMemberAttribute]
        public DateTime lastUpdated { get; set; }
        [DataMemberAttribute]
        public string targetNamespace { get; set; }
        [DataMemberAttribute]
        public string SelectedDOKMunicipality { get; set; }

        public Register(Models.Register item, string baseUrl, string selectedDOKMunicipality = null, string cultureName = Culture.NorwegianCode) 
        {
            id = baseUrl + item.GetObjectUrl();
            label = GetNameLocale(item, cultureName);
            lang = cultureName;
            contentsummary = GetDescriptionLocale(item, cultureName);
            lastUpdated = item.modified;
            targetNamespace = item.targetNamespace;
            containedItemClass = item.containedItemClass;
            if (item.owner != null) owner = item.owner.seoname;
            if (item.manager != null) manager = item.manager.seoname;
            containeditems = new List<Registeritem>();
            containedSubRegisters = new List<Register>();
            SelectedDOKMunicipality = selectedDOKMunicipality;
            uuid = item.systemId;
        }

        public Register() { }

        private string GetNameLocale(Models.Register item, string cultureName)
        {
            var name = item.Translations[cultureName].Name;
            if (string.IsNullOrEmpty(name))
                name = item.Translations[cultureName].Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }
        private string GetDescriptionLocale(Models.Register item, string cultureName)
        {
            var description = item.Translations[cultureName].Description;
            if (string.IsNullOrEmpty(description))
                description = item.Translations[cultureName].Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
    }
}