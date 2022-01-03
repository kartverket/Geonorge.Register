using Kartverket.Register.Models.Translations;
using Newtonsoft.Json;
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
        public Result ContainedItemsResult { get; set; }

        [DataMemberAttribute]
        public string id { get; set; }
        [DataMemberAttribute]
        public string label { get; set; }
        [DataMemberAttribute]
        public string lang { get; set; } = Culture.NorwegianCode;
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string contentsummary { get; set; }
        [DataMemberAttribute]
        public string owner { get; set; }
        [DataMemberAttribute]
        public string status { get; set; }
        [DataMemberAttribute]
        public string manager { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string targetNamespace { get; set; }
        [DataMemberAttribute]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SelectedDOKMunicipality { get; set; }
        public bool ShouldSerializeSelectedDOKMunicipality()
        {
            return !string.IsNullOrEmpty(SelectedDOKMunicipality);
        }

        public Register(Models.Register item, string baseUrl, string selectedDOKMunicipality = null, string cultureName = Culture.NorwegianCode) 
        {
            if (item != null)
            {
                id = baseUrl + item.GetObjectUrl();
                label = item.NameTranslated();
                lang = cultureName;
                contentsummary = item.DescriptionTranslated();
                lastUpdated = item.modified;
                status = item.status.DescriptionTranslated();
                targetNamespace = item.targetNamespace;
                containedItemClass = item.containedItemClass;
                if (item.owner != null) owner = item.owner.NameTranslated();
                if (item.manager != null) manager = item.manager.name;
                containeditems = new List<Registeritem>();
                containedSubRegisters = new List<Register>();
                SelectedDOKMunicipality = selectedDOKMunicipality;
                uuid = item.systemId;
            }
        }

        public Register() { }

        private string GetNameLocale(Models.Register item, string cultureName)
        {
            var name = item.Translations[cultureName]?.Name;
            if (string.IsNullOrEmpty(name))
                name = item.Translations[cultureName]?.Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }
        private string GetDescriptionLocale(Models.Register item, string cultureName)
        {
            var description = item.Translations[cultureName]?.Description;
            if (string.IsNullOrEmpty(description))
                description = item.Translations[cultureName]?.Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }

        public bool IsInspireDataset()
        {
            return containedItemClass == "InspireDataset";
        }
    }
}