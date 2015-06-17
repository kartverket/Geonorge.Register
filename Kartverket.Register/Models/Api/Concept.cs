using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kartverket.Register.Helpers;
using System.Text.RegularExpressions;

namespace Kartverket.Register.Models.Api
{
    public class Concept
    {
        public string id { get; set; }
        public string name { get; set; }
        public string seoname { get; set; }
        public string description { get; set; }
        public string codevalue { get; set; }
        public string owner { get; set; }
        public string broader { get; set; }
        public List<string> narrower { get; set; }

        public Concept(object models)
        {

            if (models is Register)
            {
                Register register = (Register)models;
                id = register.id;
                name = register.label;
                seoname = MakeSeoFriendlyString(register.label);
                owner = register.owner;

                if (!string.IsNullOrWhiteSpace(register.contentsummary))
                {
                    description = register.contentsummary;
                }
                else
                {
                    description = "";
                }
                codevalue = "";
                broader = "";

            }
            else if (models is Registeritem)
            {
                Registeritem item = (Registeritem)models;
                id = item.id;
                name = item.label;
                seoname = MakeSeoFriendlyString(item.label);
                owner = item.owner;
                if (!string.IsNullOrWhiteSpace(item.description))
                {
                    description = item.description;
                }
                else
                {
                    description = "";
                }

                if (!string.IsNullOrWhiteSpace(item.codevalue))
                {
                    codevalue = item.codevalue;
                }
                else
                {
                    codevalue = "";
                }
                if (item.broader != null)
                {
                    broader = item.broader;
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