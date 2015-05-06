using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Kartverket.Register.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string ApplicationVersionNumber(this HtmlHelper helper)
        {
            string versionNumber = WebConfigurationManager.AppSettings["BuildVersionNumber"];
            return versionNumber;
        }

        public static string GetSecurityClaim(this HtmlHelper helper, IEnumerable<System.Security.Claims.Claim> claims, string type)
        {
            string result = null;
            foreach (var claim in claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    result = claim.Value;
                    break;
                }
            }

            // bad hack, must fix BAAT
            if (!string.IsNullOrWhiteSpace(result) && type.Equals("organization") && result.Equals("Statens kartverk"))
            {
                result = "Kartverket";
            }

            return result;
        }

        public static bool IsGeonorgeAdmin(this HtmlHelper helper, IEnumerable<System.Security.Claims.Claim> claims)
        {
            bool isInRole = false;
            foreach (var c in claims)
            {
                if (c.Type == "role")
                {
                    if (c.Value == "nd.metadata_admin")
                    {
                        isInRole = true;
                        break;
                    }
                }
            }

            return isInRole;
        }

        public static List<Kartverket.Register.Models.Register> Registers() { 
            RegisterDbContext db = new RegisterDbContext();
            
            var queryResults = from o in db.Registers
                               where o.parentRegisterId == null
                               select o;

            List<Kartverket.Register.Models.Register> RegistersList = new List<Kartverket.Register.Models.Register>();
            foreach (var item in queryResults)
	        {
		        RegistersList.Add(item);
	        }
            RegistersList.OrderBy(r => r.name);

            return RegistersList; 
        }

        public static bool IsGeonorgeEditor(this HtmlHelper helper, IEnumerable<System.Security.Claims.Claim> claims)
        {
           bool isInRole = false;
           foreach (var c in claims)
            {
                if (c.Type == "role")
                {
                    if (c.Value == "nd.metadata_editor" || c.Value == "nd.metadata")
                    {
                        isInRole = true;
                        break;
                    }
                }
            }

           return isInRole;
        }

        public static string GeonorgeUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["GeonorgeUrl"];
        }
        public static string GeonorgeArtiklerUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["GeonorgeArtiklerUrl"];
        }
        public static string NorgeskartUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["NorgeskartUrl"];
        }
        public static string RegistryUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["RegistryUrl"];
        }
        public static string ObjektkatalogUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["ObjektkatalogUrl"];
        }
        public static string KartkatalogenUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["KartkatalogenUrl"];
        }

        public static string DokRegisterUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["DokRegisterUrl"];
        }

        public static string ToUrl(this HtmlHelper helper, string name)
        {
            return MakeSeoFriendlyString(name);
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


        // SORTERING av registeritems
        public static List<Kartverket.Register.Models.RegisterItem> SortingRegisterItems(Kartverket.Register.Models.Register Model, String sortingType)
        {
            var sortedList = Model.items.OrderBy(o => o.name).ToList();
            if (sortingType == "name_desc")
            {
                sortedList = Model.items.OrderByDescending(o => o.name).ToList();
            }
            if (sortingType == "submitter")
            {
                sortedList = Model.items.OrderBy(o => o.submitter.name).ToList();
            }
            if (sortingType == "submitter_desc")
            {
                sortedList = Model.items.OrderByDescending(o => o.submitter.name).ToList();
            }
            else if (sortingType == "status")
            {
                sortedList = Model.items.OrderBy(o => o.status.description).ToList();
            }
            else if (sortingType == "status_desc")
            {
                sortedList = Model.items.OrderByDescending(o => o.status.description).ToList();
            }
            else if (sortingType == "dateSubmitted")
            {
                sortedList = Model.items.OrderBy(o => o.dateSubmitted).ToList();
            }
            else if (sortingType == "dateSubmitted_desc")
            {
                sortedList = Model.items.OrderByDescending(o => o.dateSubmitted).ToList();
            }
            else if (sortingType == "modified")
            {
                sortedList = Model.items.OrderBy(o => o.modified).ToList();
            }
            else if (sortingType == "modified_desc")
            {
                sortedList = Model.items.OrderByDescending(o => o.modified).ToList();
            }
            else if (sortingType == "dateAccepted")
            {
                sortedList = Model.items.OrderBy(o => o.dateAccepted).ToList();
            }
            else if (sortingType == "dateAccepted_desc")
            {
                sortedList = Model.items.OrderByDescending(o => o.dateAccepted).ToList();
            }
            else if (sortingType == "documentOwner")
            {
                var documentOwner = Model.items.OfType<Document>().OrderBy(o => o.documentowner.name);
                sortedList = documentOwner.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "documentOwner_desc")
            {
                var documentOwner = Model.items.OfType<Document>().OrderByDescending(o => o.documentowner.name);
                sortedList = documentOwner.Cast<RegisterItem>().ToList();
            }


            else if (sortingType == "datasetOwner")
            {
                var datasetOwner = Model.items.OfType<Dataset>().OrderBy(o => o.datasetowner.name);
                sortedList = datasetOwner.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "datasetOwner_desc")
            {
                var datasetOwner = Model.items.OfType<Dataset>().OrderByDescending(o => o.datasetowner.name);
                sortedList = datasetOwner.Cast<RegisterItem>().ToList();
            }

            else if (sortingType == "distributionFormat")
            {
                var distributionFormat = Model.items.OfType<Dataset>().OrderBy(o => o.DistributionFormat);
                sortedList = distributionFormat.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "distributionFormat_desc")
            {
                var distributionFormat = Model.items.OfType<Dataset>().OrderByDescending(o => o.DistributionFormat);
                sortedList = distributionFormat.Cast<RegisterItem>().ToList();
            }

            else if (sortingType == "wmsUrl")
            {
                var wmsUrl = Model.items.OfType<Dataset>().OrderBy(o => o.WmsUrl);
                sortedList = wmsUrl.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "wmsUrl_desc")
            {
                var wmsUrl = Model.items.OfType<Dataset>().OrderByDescending(o => o.WmsUrl);
                sortedList = wmsUrl.Cast<RegisterItem>().ToList();
            }

            else if (sortingType == "theme")
            {
                var theme = Model.items.OfType<Dataset>().OrderBy(o => o.theme.value);
                sortedList = theme.Cast<RegisterItem>().ToList();
            }
            else if (sortingType == "theme_desc")
            {
                var theme = Model.items.OfType<Dataset>().OrderByDescending(o => o.theme.value);
                sortedList = theme.Cast<RegisterItem>().ToList();
            }


            
            return sortedList;
        }

        // SORTERING av Register
        public static List<Kartverket.Register.Models.Register> SortingRegisters(Kartverket.Register.Models.Register Model, String sortingType)
        {
            var sortedList = Model.subregisters.OrderBy(o => o.name).ToList();
            if (sortingType == "name_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.name).ToList();
            }
            if (sortingType == "submitter")
            {
                sortedList = Model.subregisters.OrderBy(o => o.owner.name).ToList();
            }
            if (sortingType == "submitter_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.owner.name).ToList();
            }
            else if (sortingType == "status")
            {
                sortedList = Model.subregisters.OrderBy(o => o.status.description).ToList();
            }
            else if (sortingType == "status_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.status.description).ToList();
            }
            else if (sortingType == "dateSubmitted")
            {
                sortedList = Model.subregisters.OrderBy(o => o.dateSubmitted).ToList();
            }
            else if (sortingType == "dateSubmitted_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.dateSubmitted).ToList();
            }
            else if (sortingType == "modified")
            {
                sortedList = Model.subregisters.OrderBy(o => o.modified).ToList();
            }
            else if (sortingType == "modified_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.modified).ToList();
            }
            else if (sortingType == "dateAccepted")
            {
                sortedList = Model.subregisters.OrderBy(o => o.dateAccepted).ToList();
            }
            else if (sortingType == "dateAccepted_desc")
            {
                sortedList = Model.subregisters.OrderByDescending(o => o.dateAccepted).ToList();
            }
            return sortedList;
        }
    }
}