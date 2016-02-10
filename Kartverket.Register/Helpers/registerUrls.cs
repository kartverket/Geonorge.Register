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
    public static class RegisterUrls
    {

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

        public static string EditorUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["EditorUrl"];
        }

        public static string ToUrl(this HtmlHelper helper, string name)
        {
            return MakeSeoFriendlyString(name);
        }

        public static string urlFormat(HttpRequestBase request, string format)
        {
            string url = request.FilePath + "." + format + "?" + request.QueryString;
            return url;
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


        public static string registerUrl(string parentregister, string registerOwner, string register)
        {
            if (string.IsNullOrWhiteSpace(parentregister))
            {
                return "/register/" + register;
            }
            else
            {
                return "/subregister/" + parentregister + "/" + registerOwner + "/" + register;
            }
        }

        public static string DeatilsDocumentUrl(string parentRegister, string registerOwner, string register, string itemOwner, string item)
        {
            if (string.IsNullOrWhiteSpace(parentRegister))
            {
                return "/register/versjoner/" + register + "/" + itemOwner + "/" + item;
            }
            else
            {
                return "/subregister/versjoner/" + parentRegister + "/" + registerOwner + "/" + register + "/" + itemOwner + "/" + item;
            }
        }

        public static string DeatilsRegisterItemUrl(string parentRegister, string registerOwner, string register, string itemOwner, string item)
        {
            if (string.IsNullOrWhiteSpace(parentRegister))
            {
                return "/register/" + register + "/" + itemOwner + "/" + item;
            }
            else
            {
                return "/subregister/" + parentRegister + "/" + registerOwner + "/" + register + "/" + itemOwner + "/" + item;
            }
        }

        public static string FilterByOrganizationUrl(string parentRegister, string registerOwner, string register, string itemOwner)
        {
            if (string.IsNullOrWhiteSpace(parentRegister))
            {
                return "/register/" + register + "/" + itemOwner;
            }
            else
            {
                return "/subregister/" + parentRegister + "/" + registerOwner + "/" + register + "/" + itemOwner;
            }
        }

        public static string EditRegisterItemUrl(string parentRegister, string registerOwner, string register, string itemOwner, string item, string itemClass)
        {
            if (string.IsNullOrWhiteSpace(parentRegister))
            {
                string url = register + "/" + itemOwner + "/" + item + "/rediger";
                
                if (itemClass == "Document") return "/dokument/" + url;
                else if (itemClass == "CodelistValue") return "/kodeliste/" + url;
                else if (itemClass == "Register") return "/subregister/" + url;
                else if (itemClass == "Organization") return "/organisasjoner/" + url;
                else if (itemClass == "Dataset") return "/dataset/" + url;
                else if (itemClass == "EPSG") return "/epsg/" + url;
                else if (itemClass == "NameSpace") return "/navnerom/" + url;
            }
            else
            {
                string url = parentRegister + "/" + registerOwner + "/" + register + "/" + itemOwner + "/" + item + "/rediger";

                if (itemClass == "Document") return "/dokument/" + url;
                else if (itemClass == "CodelistValue") return "/kodeliste/" + url;
                else if (itemClass == "Register") return "/subregister/" + url;
                else if (itemClass == "Organization") return "/organisasjoner/" + url;
                else if (itemClass == "Dataset") return "/dataset/" + url;
                else if (itemClass == "EPSG") return "/epsg/" + url;
                else if (itemClass == "NameSpace") return "/navnerom/" + url;
            }
            return "#";
        }


        public static string DeleteRegisterUrl(string parentRegister, string registerOwner, string register)
        {
            if (string.IsNullOrWhiteSpace(parentRegister))
            {
                return "/slett/" + register;
            }
            else
            {
                return "/subregister/" + parentRegister + "/" + registerOwner + "/" + register + "/slett";
            }
        }

        public static string EditRegisterUrl(string parentRegister, string registerOwner, string register)
        {
            if (string.IsNullOrWhiteSpace(parentRegister))
            {
                return "/rediger/" + register;
            }
            else
            {
                return "/subregister/" + parentRegister + "/" + registerOwner + "/" + register + "/rediger";
            }
        }

        public static string DeleteRegisterItemUrl(string parentRegister, string registerOwner, string register, string itemOwner, string item, string itemClass)
        {
            if (string.IsNullOrWhiteSpace(parentRegister))
            {
                string url = register + "/" + itemOwner + "/" + item + "/slett";

                if (itemClass == "Document") return "/dokument/" + url;
                else if (itemClass == "CodelistValue") return "/kodeliste/" + url;
                else if (itemClass == "Register") return "/subregister/" + url;
                else if (itemClass == "Organization") return "/organisasjoner/" + url;
                else if (itemClass == "Dataset") return "/dataset/" + url;
                else if (itemClass == "EPSG") return "/epsg/" + url;
                else if (itemClass == "NameSpace") return "/navnerom/" + url;
            }
            else
            {
                string url = parentRegister + "/" + registerOwner + "/" + register + "/" + itemOwner + "/" + item + "/slett";

                if (itemClass == "Document") return "/dokument/" + url;
                else if (itemClass == "CodelistValue") return "/kodeliste/" + url;
                else if (itemClass == "Register") return "/subregister/" + url;
                else if (itemClass == "Organization") return "/organisasjoner/" + url;
                else if (itemClass == "Dataset") return "/dataset/" + url;
                else if (itemClass == "EPSG") return "/epsg/" + url;
                else if (itemClass == "NameSpace") return "/navnerom/" + url;
            }
            return "#";
        }

        public static string CreateNewRegisterItemUrl(string parentRegister, string registerOwner, string register, string itemClass)
        {
            if (string.IsNullOrWhiteSpace(parentRegister))
            {
                string url = register + "/ny";

                if (itemClass == "Document") return "/dokument/" + url;
                else if (itemClass == "CodelistValue") return "/kodeliste/" + url;
                else if (itemClass == "Register") return "/subregister/" + url;
                else if (itemClass == "Organization") return "/organisasjoner/" + url;
                else if (itemClass == "Dataset") return "/dataset/" + url;
                else if (itemClass == "EPSG") return "/epsg/" + url;
                else if (itemClass == "NameSpace") return "/navnerom/" + url;
            }
            else {
                string url = parentRegister + "/" + registerOwner + "/" + register + "/ny";

                if (itemClass == "Document") return "/dokument/" + url;
                else if (itemClass == "CodelistValue") return "/kodeliste/" + url;
                else if (itemClass == "Register") return "/subregister/" + url;
                else if (itemClass == "Organization") return "/organisasjoner/" + url;
                else if (itemClass == "Dataset") return "/dataset/" + url;
                else if (itemClass == "EPSG") return "/epsg/" + url;
                else if (itemClass == "NameSpace") return "/navnerom/" + url;
            }
            return "#";
        }

        public static string CreateNewRegisterItemVersionUrl(string parentRegister, string registerOwner, string register, string itemOwner, string item, string itemClass)
        {
            if (string.IsNullOrWhiteSpace(parentRegister))
            {
                string url = "versjon/" + register + "/" + itemOwner + "/" + item + "/ny";

                if (itemClass == "Document") return "/dokument/" + url;
                else if (itemClass == "CodelistValue") return "/kodeliste/" + url;
                else if (itemClass == "Register") return "/subregister/" + url;
                else if (itemClass == "Organization") return "/organisasjoner/" + url;
                else if (itemClass == "Dataset") return "/dataset/" + url;
                else if (itemClass == "EPSG") return "/epsg/" + url;
                else if (itemClass == "NameSpace") return "/navnerom/" + url;
            }
            else
            {
                string url = "versjon/" + parentRegister + "/" + registerOwner + "/" + register + "/" + itemOwner + "/" + item + "/ny";

                if (itemClass == "Document") return "/dokument/" + url;
                else if (itemClass == "CodelistValue") return "/kodeliste/" + url;
                else if (itemClass == "Register") return "/subregister/" + url;
                else if (itemClass == "Organization") return "/organisasjoner/" + url;
                else if (itemClass == "Dataset") return "/dataset/" + url;
                else if (itemClass == "EPSG") return "/epsg/" + url;
                else if (itemClass == "NameSpace") return "/navnerom/" + url;
            }
            return "#";
        }
    }
}