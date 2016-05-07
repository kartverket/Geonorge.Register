using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Services
{
    public class CoverageService
    {

        private readonly RegisterDbContext _context;

        public CoverageService(RegisterDbContext context)
        {
            _context = context;
        }

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
                if (municipality.ToString() == municipalityCode.TrimStart('0'))
                {
                    foundCoverage = true;
                    break;
                }
            }

            return foundCoverage;
        }

        public bool UpdateAllCoverage()
        {
            var coverageDatasets = _context.CoverageDatasets;

            foreach(var coverage in coverageDatasets.ToList())
            {
                bool coverageFound = false;
                try
                {
                    var uuid = coverage.dataset.Uuid;
                    var organization = (Organization)_context.RegisterItems.Where(org => org.systemId == coverage.MunicipalityId).FirstOrDefault();
                    var municipalityService = new MunicipalityService();
                    var municipalityCode = municipalityService.LookupMunicipalityCodeFromOrganizationNumber(organization.number);
                    coverageFound = GetCoverage(uuid, municipalityCode);
                }
                catch { }

                coverage.Coverage = coverageFound;
                _context.Entry(coverage).State = EntityState.Modified;
                _context.SaveChanges();
            }

            return true;
        }

    }
}