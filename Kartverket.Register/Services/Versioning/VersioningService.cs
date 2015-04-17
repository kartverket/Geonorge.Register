using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Services.Versioning
{
    public class VersioningService : IVersioningService
    {
        private readonly RegisterDbContext _dbContext;

        public VersioningService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public VersionsItem Versions(string registername, string itemname)
        {
            List<RegisterItem> suggestionsItems = new List<RegisterItem>();
            List<RegisterItem> historicalItems = new List<RegisterItem>();

            // finne Gjeldende versjon
            var queryResults = from ri in _dbContext.RegisterItems
                               where ri.register.seoname == registername
                                && ri.register.parentRegisterId == null
                                && ri.seoname == itemname
                                && ri.status.value == "Valid"
                               select ri;

            RegisterItem currentVersion = queryResults.FirstOrDefault();
            
            // Finne alle versjoner som står som forslag
            queryResults = from ri in _dbContext.RegisterItems
                            where ri.register.seoname == registername
                            && ri.register.parentRegisterId == null
                            && ri.seoname == itemname
                            && (ri.status.value == "Submitted" 
                            || ri.status.value == "Proposal"
                            || ri.status.value == "InProgress"
                            || ri.status.value == "Candidate")
                            select ri;

            foreach (RegisterItem item in queryResults)
	        {
                suggestionsItems.Add(item);		 
	        }

            if (currentVersion == null){
                currentVersion = queryResults.FirstOrDefault();
            }            

            //finne alle historiske versjoner
             var queryResultsHistorical = from ri in _dbContext.RegisterItems
                                           where ri.register.seoname == registername
                                            && ri.register.parentRegisterId == null
                                            && ri.versioningId == currentVersion.versioningId
                                            && (ri.status.value == "deprecated"
                                            || ri.status.value == "superseeded"
                                            || ri.status.value == "retired")
                                           select ri;

            foreach (RegisterItem item in queryResultsHistorical)
	        {
                historicalItems.Add(item);
            }

            return new VersionsItem
            {
                currentVersion = currentVersion,
                historical = historicalItems,
                suggestions = suggestionsItems
            };
        }
    }
}