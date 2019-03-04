using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Kartverket.Register.Services
{
    public class DokCoverageWmsMapping
    {
        public static Dictionary<string, string> DatasetUuidToWmsLayerMapping = new Dictionary<string, string>();

        static DokCoverageWmsMapping() { UpdateMappings(); }

        public static void UpdateMappings()
        {
            Dictionary<string, string> DatasetUuidToWmsLayerMappings = new Dictionary<string, string>();

            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["CoverageApi"] + "datasett";
            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var data = c.DownloadString(url);
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);

            var codeList = response["datasets"];

            foreach (var code in codeList)
            {
                JToken uuidToken = code["uuid"];
                string uuid = uuidToken?.ToString();

                JToken nameToken = code["navn"];
                string name = nameToken?.ToString();

                if(!DatasetUuidToWmsLayerMappings.ContainsKey(uuid))
                    DatasetUuidToWmsLayerMappings.Add(uuid, name);

            }

            DatasetUuidToWmsLayerMapping = DatasetUuidToWmsLayerMappings;

        }
    }
}