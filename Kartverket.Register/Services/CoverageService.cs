using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Kartverket.Register.Services
{
    public class CoverageService
    {

        private readonly RegisterDbContext _context;

        List<string> coverageList;

        string coverageApiUrl = "https://ws.geonorge.no/dekningsApi/kommune?kid=";

        public CoverageService(RegisterDbContext context, string municipalityCode)
        {
            _context = context;
            coverageList = FetchMunicipalityDatasets(municipalityCode);
        }

        private List<string> FetchMunicipalityDatasets(string municipalityCode)
        {
            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var data = c.DownloadString(coverageApiUrl + municipalityCode);
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);
            var datasets = response[municipalityCode].Children().Values<String>();
            return datasets.ToList();
        }


        public bool GetCoverage(string datasetUuid)
        {
            bool coverage = false;
            string name;

            if (!string.IsNullOrEmpty(datasetUuid) && DokCoverageWmsMapping.DatasetUuidToWmsLayerMapping.ContainsKey(datasetUuid))
            {
                name = DokCoverageWmsMapping.DatasetUuidToWmsLayerMapping[datasetUuid];
                coverage = GetMunicipalityCoverage(name);     
            }
            
            return coverage;
        }

        private bool GetMunicipalityCoverage(string name)
        {
            bool coverage = false;

            if (coverageList.Contains(name))
                coverage = true;

            return coverage;
        }

    }
}