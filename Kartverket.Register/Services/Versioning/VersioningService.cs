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
            Models.Version versjonsGruppe = new Models.Version();
            if (parantRegister != null)
            {
                var queryResultsRegisteritem = from ri in _dbContext.RegisterItems
                                               where ri.register.seoname == registername && ri.register.parentRegister.seoname == parantRegister
                                               && ri.seoname == itemname
                                            select ri.versioning;

                versjonsGruppe = queryResultsRegisteritem.FirstOrDefault();
            }
            else {
                var queryResultsRegisteritem = from ri in _dbContext.RegisterItems
                                               where ri.register.seoname == registername && ri.register.parentRegister == null
                                               && ri.seoname == itemname
                                               select ri.versioning;

                versjonsGruppe = queryResultsRegisteritem.FirstOrDefault();
            }
            Guid? versjonsGruppeId = versjonsGruppe.systemId;
            
            Guid currentVersionId = versjonsGruppe.currentVersion;
            List<Models.RegisterItem> suggestionsItems = new List<Models.RegisterItem>();
            List<Models.RegisterItem> historicalItems = new List<Models.RegisterItem>();

            // finne Gjeldende versjon i versjonsgruppen
            var queryResults = from ri in _dbContext.RegisterItems
                               where ri.systemId == currentVersionId
                               select ri;

            Models.RegisterItem currentVersion = queryResults.FirstOrDefault();

            // Finne alle versjoner som står som forslag
            queryResults = from ri in _dbContext.RegisterItems
                           where ri.register.seoname == registername
                           && ri.versioningId == versjonsGruppeId
                           && (ri.status.value == "Submitted"
                           //|| ri.status.value == "Proposal"
                           //|| ri.status.value == "InProgress"
                           || ri.status.value == "Draft")
                           //|| ri.status.value == "Accepted"
                           //|| ri.status.value == "Experimental"
                           //|| ri.status.value == "Candidate"                           
                           select ri;

            foreach (Models.RegisterItem item in queryResults)
            {
                suggestionsItems.Add(item);
            }


            //finne alle historiske versjoner
            var queryResultsHistorical = from ri in _dbContext.RegisterItems
                                         where ri.register.seoname == registername
                                          && ri.versioningId == currentVersion.versioningId
                                          && (ri.status.value == "Superseded"
                                          || ri.status.value == "Retired")
                                          //ri.status.value == "Deprecated"                                          
                                         select ri;

            foreach (Models.RegisterItem item in queryResultsHistorical)
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