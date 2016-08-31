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

        public CoverageService(RegisterDbContext context)
        {
            _context = context;     
        }

        public void SetCoverage(string municipalityCode)
        {
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

        public string UpdateDatasetsWithCoverage()
        {

            AddCoverageDatasetsNotSet();

            var organizations = _context.CoverageDatasets.Select(m => m.Municipality.number).Distinct().ToList();

            foreach (var orgNumber in organizations)
            {
                try
                {
                    var municipalityService = new MunicipalityService();
                    var municipalityCode = municipalityService.LookupMunicipalityCodeFromOrganizationNumber(orgNumber);
                    SetCoverage(municipalityCode);

                    var coverageDatasets = _context.CoverageDatasets.Where(c => c.Municipality.number == orgNumber).Select(d => d).ToList();
                    foreach (var coverage in coverageDatasets)
                    { 
                        bool coverageFound = false;
                        coverageFound = GetCoverage(coverage.dataset.Uuid);
                        coverage.Coverage = coverageFound;

                        _context.Database.ExecuteSqlCommand("UPDATE CoverageDatasets SET Coverage = '" + coverage.Coverage + "' WHERE CoverageId='" + coverage.CoverageId +  "'");
                        
                    }
                }
                catch { }
            }

            return "updated";
        }

        private void AddCoverageDatasetsNotSet()
        {
            var municipalityList = (from mun in MunicipalityData.MunicipalityFromOrganizationNumberToCode
                                    select mun.Key).ToList();

            var orgList = (from o in municipalityList
                           join org in _context.Organizations on o equals org.number
                           select new { org.systemId}).ToList();

            foreach(var org in orgList)
            {
                try { 
                _context.Database.ExecuteSqlCommand("INSERT INTO CoverageDatasets SELECT NEWID() AS CoverageId, '" + org.systemId + "' AS MunicipalityId, 0 AS ConfirmedDok, systemId AS datasetId, NULL AS Note, dokStatusId AS CoverageDOKStatusId, systemId AS Dataset_systemId, 0 AS coverage FROM RegisterItems WHERE(Discriminator = 'Dataset') AND(DatasetType <> 'Kommunalt') AND(systemId NOT IN(SELECT  DatasetId FROM CoverageDatasets AS CoverageDatasets_1 WHERE(MunicipalityId = '" + org.systemId + "')))");
                }
                catch { }
            }
        }
    }
}