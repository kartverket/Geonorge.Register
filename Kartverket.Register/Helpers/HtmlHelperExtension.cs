using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}