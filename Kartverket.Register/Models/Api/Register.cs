using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class Register
    {
        public string id { get; set; }
        public string label { get; set; }
        public string lang { get; set; } = "no";
        public string contentsummary { get; set; }
        public string owner { get; set; }
        public string manager { get; set; }
        public string controlbody { get; set; }
        public string containedItemClass { get; set; }
        public Guid uuid { get; set; }
        public List<Registeritem> containeditems { get; set; }
        public List<Register> containedSubRegisters { get; set; }
        public DateTime lastUpdated { get; set; }
        public string targetNamespace { get; set; }
        public string SelectedDOKMunicipality { get; set; }

        public Register(Models.Register item, string baseUrl, string selectedDOKMunicipality = null, string cultureName = "nb-NO") 
        {
            id = baseUrl + item.GetObjectUrl();
            label = GetNameLocale(item, cultureName);
            lang = cultureName.Substring(0, 2);
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

        private string GetNameLocale(Models.Register item, string cultureName)
        {
            var name = item.Translations[cultureName].Name;
            if (string.IsNullOrEmpty(name))
                name = item.Translations[cultureName.Substring(0,2)].Name;
            if (string.IsNullOrEmpty(name))
                name = item.name;

            return name;
        }
        private string GetDescriptionLocale(Models.Register item, string cultureName)
        {
            var description = item.Translations[cultureName].Description;
            if (string.IsNullOrEmpty(description))
                description = item.Translations[cultureName.Substring(0, 2)].Description;
            if (string.IsNullOrEmpty(description))
                description = item.description;

            return description;
        }
    }
}