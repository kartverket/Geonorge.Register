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

        public VersionsItem Versions(string registername, string parantRegister, string itemname)
        {
            // Finn versjonsgruppen
            var queryResultsRegisteritem = from ri in _dbContext.RegisterItems
                                           where ri.register.seoname == registername && ri.register.parentRegister.seoname == parantRegister
                                           && ri.seoname == itemname
                                           select ri.versioningId;

            Guid? vnr = queryResultsRegisteritem.FirstOrDefault();





            List<RegisterItem> suggestionsItems = new List<RegisterItem>();
            List<RegisterItem> historicalItems = new List<RegisterItem>();

            // finne Gjeldende versjon
            var queryResults = from ri in _dbContext.RegisterItems
                               where ri.register.seoname == registername
                                && ri.statusId == "Valid"
                                && ri.versioningId == vnr
                               select ri;

            RegisterItem currentVersion = queryResults.FirstOrDefault();
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

            // Finne alle versjoner som står som forslag
            queryResults = from ri in _dbContext.RegisterItems
                           where ri.register.seoname == registername
                           && ri.versioningId == vnr
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

            if (currentVersion == null)
            {
                currentVersion = queryResults.FirstOrDefault();
            }

            //finne alle historiske versjoner
            var queryResultsHistorical = from ri in _dbContext.RegisterItems
                                         where ri.register.seoname == registername
                                          && ri.versioningId == currentVersion.versioningId
                                          && (ri.status.value == "Deprecated"
                                          || ri.status.value == "Superseded"
                                          || ri.status.value == "Retired")
                                         select ri;

            foreach (RegisterItem item in queryResultsHistorical)
            {
                historicalItems.Add(item);
            }

            if (historicalItems != null)
            {
                if (currentVersion == null)
                {
                    currentVersion = queryResultsHistorical.FirstOrDefault();
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