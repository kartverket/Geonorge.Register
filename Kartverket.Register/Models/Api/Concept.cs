﻿using System;
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
        public string status { get; set; }
        public string broader { get; set; }

        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        //public List<Registeritem> narrower { get; set; }

        public Concept(object models)
        {

            if (models is Register)
            {
                Register register = (Register)models;
                id = register.id;
                name = register.label;
                seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(register.label);
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
                seoname = Helpers.RegisterUrls.MakeSeoFriendlyString(item.label);
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
                else {
                    broader = "";
                }

                ValidFromDate = item.ValidFrom;
                ValidToDate = item.ValidTo;

                status = item.status;
            }
        }
    }
}