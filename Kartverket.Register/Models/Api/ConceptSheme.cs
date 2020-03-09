using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kartverket.Register.Helpers;
using System.Text.RegularExpressions;

namespace Kartverket.Register.Models.Api
{
    public class ConceptSheme
    {
        public string id { get; set; }
        public string name { get; set; }
        public string seoname { get; set; }
        public string description { get; set; }
        public List<Concept> concepts { get; set; }
        public string codelistValue { get; set; }
        public string owner { get; set; }
        public string status { get; set; }
        public virtual string broader { get; set; }
        public List<string> narrower { get; set; }

        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }

        public ConceptSheme(object models)
        {
            concepts = new List<Concept>();
            narrower = new List<string>();

            if (models is Registeritem)
            {
                Registeritem item = (Registeritem)models;
                id = item.id;
                name = item.label;
                owner = item.owner;
                status = item.status;
                if (!string.IsNullOrWhiteSpace(item.description))
                {
                    description = item.description;
                }
                else
                {
                    description = "";
                }
                seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(item.label);
                codelistValue = item.codevalue;
                concepts = new List<Concept>();
                if (item.broader != null)
                {
                    broader = item.broader;
                }
                else
                {
                    broader = "";
                }
                
                foreach (var nitem in item.narrower)    
	            {
                    narrower.Add(nitem);
	            }

                ValidFromDate = item.ValidFrom;
                ValidToDate = item.ValidTo;
            }
            if (models is Register)
            {
                Register register = (Register)models;
                id = register.id;
                name = register.label;
                owner = register.owner;
                if (!string.IsNullOrWhiteSpace(register.contentsummary))
                {
                    description = register.contentsummary;
                }
                else
                {
                    description = "";
                }
                codelistValue = "";
                seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(register.label);
                concepts = new List<Concept>();
                broader = "";

                if (register.containedSubRegisters != null)
                {
                    foreach (Register reg in register.containedSubRegisters)
                    {
                        Concept conceptItem = new Concept(reg);
                        concepts.Add(conceptItem);
                    }
                }

                foreach (Registeritem item in register.containeditems)
                {
                    Concept conceptItem = new Concept(item);
                    concepts.Add(conceptItem);
                }
            }          
        }     
    }
}