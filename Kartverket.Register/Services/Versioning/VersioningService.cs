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

            RegisterItem currentVersion = currentVersion = queryResults.FirstOrDefault();
            List<RegisterItem> validVersions = queryResults.ToList();
            if (queryResults.Count() > 1)
            {
                
                foreach (RegisterItem item in validVersions)
                {
                    if (item.dateAccepted > currentVersion.dateAccepted)
                    {
                        currentVersion = item;
                    }                    
                }

            }
            else {
                currentVersion = queryResults.FirstOrDefault();
            }

            
            
            // Finne alle versjoner som står som forslag
            queryResults = from ri in _dbContext.RegisterItems
                            where ri.register.seoname == registername
                            && ri.register.parentRegisterId == null
                            && ri.seoname == itemname
                            && (ri.status.value == "Submitted" 
                            || ri.status.value == "Proposal"
                            || ri.status.value == "InProgress"
                            || ri.status.value == "NotAccepted"
                            || ri.status.value == "Accepted"
                            || ri.status.value == "Experimental"
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
                                            && (ri.status.value == "Deprecated"
                                            || ri.status.value == "Superseded"
                                            || ri.status.value == "Retired")
                                          select ri;

             
            
            if (queryResultsHistorical.Count() != 0)
            {
                if (currentVersion == null)
                {
                    currentVersion = queryResults.FirstOrDefault();
                }

            foreach (RegisterItem item in queryResultsHistorical)
	        {
                historicalItems.Add(item);
            }
            }
            foreach (RegisterItem item in validVersions)
            {
                if (item != currentVersion)
                {
                    historicalItems.Add(item);
                }
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