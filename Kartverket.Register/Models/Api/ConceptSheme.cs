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
        public virtual string broader { get; set; }
        public List<Registeritem> narrower { get; set; }

        public ConceptSheme(object models)
        {
            if (models is Registeritem)
            {
                Registeritem item = (Registeritem)models;
                id = item.id;
                name = item.label;
                owner = item.owner;
                if (!string.IsNullOrWhiteSpace(item.description))
                {
                    description = item.description;
                }
                else
                {
                    description = "";
                }
                seoname = Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(item.label);
                codelistValue = item.codevalue;
                concepts = new List<Concept>();
                if (item.broader != null)
                {
                    broader = item.broader.id;
                }
                else
                {
                    broader = "";
                }
                if (item.narrower != null)
                {
                    narrower = item.narrower;
                }

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
                seoname = Helpers.HtmlHelperExtensions.MakeSeoFriendlyString(register.label);
                concepts = new List<Concept>();
                broader = "";

                foreach (Register reg in register.containedSubRegisters)
                {
                    Concept conceptItem = new Concept(reg);
                    concepts.Add(conceptItem);
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