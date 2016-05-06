using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Services
{
    public class CoverageService
    {

        string coverageApiUrl = "https://ws.geonorge.no/dekningsApi/dekning?datasett=";

        public bool GetCoverage(string datasetUuid, string municipalityCode)
        {
            bool coverage = false;
            string name;

            if (!string.IsNullOrEmpty(datasetUuid) && DokCoverageWmsMapping.DatasetUuidToWmsLayerMapping.ContainsKey(datasetUuid))
            {
                name = DokCoverageWmsMapping.DatasetUuidToWmsLayerMapping[datasetUuid];
                coverage = FetchDatasetCoverage(name, municipalityCode);
                
            }
            
            return coverage;
        }

        private bool FetchDatasetCoverage(string name, string municipalityCode)
        {
            bool foundCoverage = false;

            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var data = c.DownloadString(coverageApiUrl + name);
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);

            var municipalities = response["kommuner"];

            foreach (var municipality in municipalities)
            {
                if (municipality.ToString() == municipalityCode)
                {
                    foundCoverage = true;
                    break;
                }
            }

            return foundCoverage;
        }
    }
}