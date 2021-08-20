using Kartverket.Register.Models;
using Kartverket.Register.Models.Translations;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kartverket.Register.Helpers
{
    public static class RegisterUrls
    {

        /// <summary>
        /// Add parameters from current url
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="parameter"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public static RouteValueDictionary ToRouteValueDictionary(this NameValueCollection collection, string parameter = null, string parameterValue = null)
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            if (!string.IsNullOrWhiteSpace(parameter) && !string.IsNullOrWhiteSpace(parameterValue))
            {
                nameValueCollection.Add(parameter, parameterValue);
            }

            var routeValueDictionary = new RouteValueDictionary();
            foreach (var key in collection.AllKeys)
            {
                if (key == "page")
                {
                    routeValueDictionary.Remove(key);
                }
                else
                {
                    routeValueDictionary.Add(key, collection[key]);
                }
                if (key == "sorting")
                {
                    routeValueDictionary.Remove(key);
                }
                if (key == "SelectedInspireMonitoringReport")
                {
                    routeValueDictionary.Remove(key);
                }
                if (key == "SelectedComparableCandidate")
                {
                    routeValueDictionary.Remove(key);
                }
                if (key == "Compare")
                {
                    routeValueDictionary.Remove(key);
                }
            }
            foreach (var key in nameValueCollection.AllKeys)
            {
                if (routeValueDictionary.ContainsKey(key))
                {
                    routeValueDictionary.Remove(key);
                }
                routeValueDictionary.Add(key, nameValueCollection[key]);
            }
            return routeValueDictionary;
        }

        public static string GeonorgeUrl(this HtmlHelper helper)
        {
            var url = WebConfigurationManager.AppSettings["GeonorgeUrl"];
            var culture = CultureHelper.GetCurrentCulture();
            if (culture != Culture.NorwegianCode)
                url = url + Culture.EnglishCode;

            return url;
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

        public static string MakeSeoFriendlyString(string input, bool transliterNorwegian = false)
        {
            string encodedUrl = (input ?? "").ToLower();

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9æøå.]", "-");

            if(transliterNorwegian)
                encodedUrl = encodedUrl.Replace("å", "a").Replace("æ", "e").Replace("ø", "o");

            // remove duplicates
            encodedUrl = Regex.Replace(encodedUrl, @"-+", "-");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim('-');

            return encodedUrl;
        }

        public static string GetFileExtension(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains("."))
                {
                    string[] split = path.Split('.');
                    var ext = split.Last();
                    if (AllowedExtension(ext))
                        return ext;
                }
            }

            return null;
        }

        public static bool AllowedExtension(string ext)
        {
            string[] extensions = { "json", "xml", "csv", "gml", "gml", "rdf", "rss", "atom" };
            return extensions.Any(e => e == ext);
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

        public static string GetParentPath(string path)
        {
            if (!string.IsNullOrEmpty(path) && path.Contains("/"))
            {
                path = path.Substring(0, path.LastIndexOf("/") + 1);
            }

            return path;
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

        public static string SynchronizeInspireRegistry()
        {
            return "/api/metadata/synchronize/inspire-statusregister";
        }

        public static string SynchronizeInspireServices()
        {
            return "/api/metadata/synchronize/inspire-statusregister/dataservices";
        }

        public static string DokStatusRegistryReport()
        {
            return "/dok/rapport";
        }

        public static string ApiReportUrlFormat(HttpRequestBase request, string format, string id = null, bool dataset = true, bool service = true)
        {
            string url = "/api" + request.FilePath + "/report";
            if (string.IsNullOrWhiteSpace(id))
            {
                url += "." + format + "?" + request.QueryString;
            }
            else
            {
                url += "/" + id + "." + format + "?" + request.QueryString;
            }
            return url;
        }

        internal static string GetSystemIdFromPath(string path)
        {
            Guid guidOutput;

            if (string.IsNullOrEmpty(path))
                return null;

            var paths = path.Split('/');
            
            foreach(var item in paths)
            {
                var data = RemoveExtension(item);
                if (Guid.TryParse(data, out guidOutput))
                    return data;
            }

            return null;
        }

        internal static string GetPath(string registername, string subregisters)
        {
            if (registername == "register")
                registername = "";

            if(!string.IsNullOrEmpty(subregisters) && subregisters.EndsWith("/"))
                subregisters  = subregisters.Substring(0, subregisters.LastIndexOf("/"));


            var path = registername;
            if (!string.IsNullOrEmpty(subregisters))
                path = path + (!string.IsNullOrEmpty(registername) ? "/" : "") + subregisters;

            if (!string.IsNullOrEmpty(GetSystemIdFromPath(path)))
            {
                var paths = path.Split('/');

                path = "";

                for (int p = 0; p < paths.Length-2;p++)
                {
                    path = path + paths[p];
                    if(p < paths.Length - 3)
                        path = path + "/";
                }
            }

            return path;
        }

        public static string RemoveExtension(string path)
        {
            if (path.Contains("."))
            {
                string[] split = path.Split('.');
                path = string.Join(".", split.Take(split.Length - 1));
            }

            return path;
        }

        public static string GetNewPath(string path, string seoName)
        {
            if (path == null)
                return null;

            if (!path.Contains('/'))
                return seoName;

            return path.Substring(0, path.LastIndexOf('/')) + "/" + seoName; ;
        }

        internal static string CreatePath(string registername, Models.Register parentRegister = null, bool transliterNorwegian = false)
        {
            var path = RegisterUrls.MakeSeoFriendlyString(registername, transliterNorwegian);

            if (parentRegister != null)
            {
                if (!string.IsNullOrEmpty(parentRegister.path))
                {
                    path = parentRegister.path + "/" + path;
                }
            }

            return path;
        }

        internal static string GetItemFromPath(string subregisters)
        {

            if(subregisters != null && subregisters.Contains("/"))
            {
                var item = subregisters.Split('/').Last();
                item = RemoveExtension(item);
                return item;
            }

            return null;
        }
    }
}
