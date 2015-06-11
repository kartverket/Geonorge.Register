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
                seoname = MakeSeoFriendlyString(item.label);
                codelistValue = item.codevalue;  
                concepts = new List<Concept>();
                
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
                seoname = MakeSeoFriendlyString(register.label);
                concepts = new List<Concept>();

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

        public static string MakeSeoFriendlyString(string input)
        {
            string encodedUrl = (input ?? "").ToLower();

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // replace norwegian characters
            encodedUrl = encodedUrl.Replace("å", "a").Replace("æ", "ae").Replace("ø", "o");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "-");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }
    }
}