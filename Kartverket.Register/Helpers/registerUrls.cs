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

        public static string DokEvaluationCriteriaUrl(this HtmlHelper helper)
        {
            return WebConfigurationManager.AppSettings["DokEvaluationCriteriaUrl"];
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

        public static string InspireMonitoringReportUrl()
        {
            string url = "/api/register/inspire-statusregister/monitoring-report";
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

        /// <summary>
        /// Makes an SEO friendly document name
        /// </summary>
        /// <param name="file"></param>
        /// <param name="filtype"></param>
        /// <param name="seofilename"></param>
        public static string MakeSeoFriendlyDocumentName(HttpPostedFileBase file, out string filtype, out string seofilename)
        {
            var documentfilename = file.FileName.Split('.');
            filtype = documentfilename.Last();
            seofilename = null;
            foreach (var item in documentfilename)
            {
                if (item == filtype)
                {
                    break;
                }
                seofilename += MakeSeoFriendlyString(item) + "_";
            }
            return seofilename;
        }

    }
}