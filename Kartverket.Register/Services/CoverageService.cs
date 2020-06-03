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

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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


        public bool? GetCoverage(string datasetUuid)
        {
            bool? coverage = null;
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

            var organizations = _context.CoverageDatasets.Select(m => m.Municipality.MunicipalityCode).Distinct().ToList();

            foreach (var municipalityCode in organizations)
            {
                try
                {
                    SetCoverage(municipalityCode);

                    var coverageDatasets = _context.CoverageDatasets.Where(c => c.Municipality.MunicipalityCode == municipalityCode).Select(d => d).ToList();
                    foreach (var coverage in coverageDatasets)
                    { 
                        bool? coverageFound = null;
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
            var orgList = (from org in _context.Organizations
                           where org.OrganizationType == Models.OrganizationType.Municipality
                           select new { org.systemId}).ToList();

            foreach(var org in orgList)
            {
                try {
                    var sql = "INSERT INTO CoverageDatasets SELECT NEWID() AS CoverageId, '" + org.systemId + "' AS MunicipalityId, 0 AS ConfirmedDok, systemId AS datasetId, NULL AS Note, dokStatusId AS CoverageDOKStatusId, systemId AS Dataset_systemId, NULL AS coverage, RegionalPlan = 0, MunicipalSocialPlan = 0, MunicipalLandUseElementPlan = 0, ZoningPlanArea = 0, ZoningPlanDetails = 0, BuildingMatter = 0, PartitionOff = 0, EenvironmentalImpactAssessment = 0, suitabilityAssessmentText = '', ZoningPlan = 0, ImpactAssessmentPlanningBuildingAct = 0, RiskVulnerabilityAnalysisPlanningBuildingAct = 0  FROM RegisterItems WHERE(Discriminator = 'Dataset') AND(DatasetType <> 'Kommunalt') AND(systemId NOT IN(SELECT  DatasetId FROM CoverageDatasets AS CoverageDatasets_1 WHERE(MunicipalityId = '" + org.systemId + "')))";
                    _context.Database.ExecuteSqlCommand(sql);
                }
                catch (Exception ex) {
                    Log.Error("Error INSERT INTO CoverageDatasets: " + ex);
                }
            }
        }

        public CoverageDataset NewCoverage(Dataset dataset)
        {
            CoverageDataset coverage = new CoverageDataset();
            coverage.CoverageId = Guid.NewGuid();
            coverage.CoverageDOKStatusId = "Accepted";
            coverage.ConfirmedDok = true;
            coverage.DatasetId = dataset.systemId;
            dataset.dokStatusDateAccepted = DateTime.Now;
            coverage.MunicipalityId = dataset.datasetownerId;
            coverage.Note = dataset.Notes;
            coverage.Coverage = null;

            _context.Entry(coverage).State = EntityState.Modified;
            _context.CoverageDatasets.Add(coverage);
            return coverage;
        }
    }
}