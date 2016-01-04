using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Kartverket.Register.Services.RegisterItem
{
    public class RegisterItemService : IRegisterItemService
    {
        private readonly RegisterDbContext _dbContext;
        private IMunicipalityService _municipalityService;
        //private IRegisterService _registerService;

        public RegisterItemService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _municipalityService = new MunicipalityService();
            //_registerService = new RegisterService(_dbContext);
        }

        public void SetNarrowerItems(List<Guid> narrowerList, CodelistValue codelistValue)
        {
            if (codelistValue.narrowerItems != null)
            {
                bool skalSlette = true;
                CodelistValue kodeSomSkalSlettes = null;
                List<CodelistValue> koderSomSkalSlettes = new List<CodelistValue>();

                foreach (CodelistValue narrower in codelistValue.narrowerItems)
                {
                    if (narrowerList != null)
                    {
                        foreach (Guid idNewNarrower in narrowerList)
                        {
                            if (idNewNarrower == narrower.systemId)
                            {
                                skalSlette = false;
                            }
                        }
                    }
                    kodeSomSkalSlettes = narrower;

                    if (skalSlette == true)
                    {
                        CodelistValue NarrowerItemsBroader = (CodelistValue)getItemById(kodeSomSkalSlettes.systemId);
                        if (NarrowerItemsBroader == kodeSomSkalSlettes)
                        {
                            NarrowerItemsBroader.broaderItemId = null;
                        }
                        koderSomSkalSlettes.Add(kodeSomSkalSlettes);
                    }
                    skalSlette = true;
                }
                foreach (CodelistValue item in koderSomSkalSlettes)
                {
                    codelistValue.narrowerItems.Remove(item);
                }
            }

            if (narrowerList != null)
            {
                foreach (Guid narrowerId in narrowerList)
                {
                    CodelistValue narrowerItem = _dbContext.CodelistValues.Find(narrowerId);
                    codelistValue.narrowerItems.Add(narrowerItem);
                    narrowerItem.broaderItemId = codelistValue.systemId;
                    narrowerItem.modified = DateTime.Now;
                }
            }
        }

        public void SetBroaderItem(Guid broader, CodelistValue codelistValue)
        {
            if (codelistValue.broaderItemId != null)
            {
                CodelistValue originalBroaderItem = (CodelistValue)codelistValue.broaderItem;
                originalBroaderItem.narrowerItems.Remove(codelistValue);
            }
            codelistValue.broaderItemId = broader;
            CodelistValue broaderItem = (CodelistValue)getItemById(broader);
            broaderItem.narrowerItems.Add(codelistValue);
        }

        public void SetBroaderItem(CodelistValue originalCodelistValue)
        {
            if (originalCodelistValue.broaderItemId != null)
            {
                CodelistValue originalBroaderItem = (CodelistValue)originalCodelistValue.broaderItem;
                originalBroaderItem.narrowerItems.Remove(originalCodelistValue);
            }
            originalCodelistValue.broaderItemId = null;
        }

        public void RemoveBroaderAndNarrower(CodelistValue codelistValue)
        {
            foreach (CodelistValue narrower in codelistValue.narrowerItems)
            {
                narrower.broaderItemId = null;
            }
            codelistValue.narrowerItems.Clear();

            if (codelistValue.broaderItemId != null)
            {
                CodelistValue broaderItem = (CodelistValue)codelistValue.broaderItem;
                broaderItem.narrowerItems.Remove(codelistValue);
                codelistValue.broaderItemId = null;
            }
        }

        private Models.RegisterItem getItemById(Guid id)
        {
            var queryresult = from ri in _dbContext.RegisterItems
                              where ri.systemId == id
                              select ri;

            Models.RegisterItem item = queryresult.FirstOrDefault();
            return item;
        }

        public Models.RegisterItem GetCurrentRegisterItem(string parentregister, string register, string name)
        {

            if (string.IsNullOrWhiteSpace(parentregister))
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == name || o.name == name) &&
                                   (o.register.seoname == register || o.register.name == register)
                                   && o.versioning.currentVersion == o.systemId
                                   select o;

                return queryResults.FirstOrDefault();
            }
            else
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == name || o.name == name) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   (o.register.parentRegister.seoname == parentregister || o.register.parentRegister.name == parentregister)
                                   && o.versioning.currentVersion == o.systemId
                                   select o;

                return queryResults.FirstOrDefault();
            }
        }

        public Models.Version GetVersionGroup(Guid? versioningId)
        {
            var queryResultVersions = from v in _dbContext.Versions
                                      where v.systemId == versioningId
                                      select v;

            Models.Version versjonsgruppe = queryResultVersions.FirstOrDefault();
            return versjonsgruppe;
        }

        public List<Models.RegisterItem> GetAllVersionsOfItembyVersioningId(Guid? versjonsGruppeId)
        {
            var queryResultsVersionsDocument = from o in _dbContext.RegisterItems
                                               where o.versioningId == versjonsGruppeId
                                               select o;

            List<Models.RegisterItem> versions = queryResultsVersionsDocument.ToList();
            return versions;
        }

        public List<Models.RegisterItem> GetAllVersionsOfItem(string parent, string register, string item)
        {
            Models.RegisterItem version = null;
            if (string.IsNullOrWhiteSpace(parent))
            {
                var queryResultsItem = from o in _dbContext.RegisterItems
                                       where o.register.seoname == register
                                       && o.register.parentRegister == null
                                       && o.seoname == item
                                       select o;

                version = queryResultsItem.FirstOrDefault();
            }
            else
            {
                var queryResultsItem = from o in _dbContext.RegisterItems
                                       where o.register.seoname == register
                                       && o.seoname == item
                                       && o.register.parentRegister.seoname == parent
                                       select o;

                version = queryResultsItem.FirstOrDefault();
            }
            var queryResultVersions = from r in _dbContext.RegisterItems
                                      where r.versioningId == version.versioningId
                                      select r;

            return queryResultVersions.ToList();
        }

        public List<Models.RegisterItem> GetRegisterItemsFromOrganization(string parentname, string registername, string itemowner)
        {
            List<Models.RegisterItem> itemsByOwner = new List<Models.RegisterItem>();
            if (string.IsNullOrWhiteSpace(parentname))
            {
                var queryResult = from r in _dbContext.RegisterItems
                                  where r.register.seoname == registername
                                  && r.register.parentRegister == null
                                  && r.submitter.seoname == itemowner
                                  && r.versioning.currentVersion == r.systemId
                                  select r;

                itemsByOwner = queryResult.ToList();
            }
            else
            {
                var queryResult = from r in _dbContext.RegisterItems
                                  where r.register.seoname == registername
                                  && r.register.parentRegister.seoname == parentname
                                  && r.submitter.seoname == itemowner
                                  && r.versioning.currentVersion == r.systemId
                                  select r;

                itemsByOwner = queryResult.ToList();
            }
            return itemsByOwner;
        }

        public Models.RegisterItem SetStatusId(Models.RegisterItem item, Models.RegisterItem originalItem)
        {
            originalItem.statusId = item.statusId;
            if (originalItem.statusId != "Valid" && item.statusId == "Valid")
            {
                originalItem.dateAccepted = DateTime.Now;
            }
            if (originalItem.statusId == "Valid" && item.statusId != "Valid")
            {
                originalItem.dateAccepted = null;
            }
            return originalItem;
        }

        public Guid NewVersioningGroup(Models.RegisterItem registerItem)
        {
            Models.Version versjoneringsGruppe = new Models.Version();
            versjoneringsGruppe.systemId = Guid.NewGuid();
            versjoneringsGruppe.currentVersion = registerItem.systemId;
            versjoneringsGruppe.containedItemClass = registerItem.register.containedItemClass;
            versjoneringsGruppe.lastVersionNumber = registerItem.versionNumber;

            _dbContext.Entry(versjoneringsGruppe).State = EntityState.Modified;
            _dbContext.Versions.Add(versjoneringsGruppe);
            return versjoneringsGruppe.systemId;
        }

        public CoverageDataset NewCoverage(Models.RegisterItem registerItem)
        {
            CoverageDataset coverage = new CoverageDataset();
            coverage.CoverageId = Guid.NewGuid();
            coverage.CoverageDOKStatusId = "Proposal";
            coverage.ConfirmedDok = true;
            coverage.DatasetId = registerItem.systemId;
            coverage.MunicipalityId = registerItem.submitterId;
            if (registerItem is Dataset)
            {
                Dataset dataset = (Dataset)registerItem;
                coverage.Note = dataset.Notes;
            }

            _dbContext.Entry(coverage).State = EntityState.Modified;
            _dbContext.CoverageDatasets.Add(coverage);
            return coverage;
        }

        public virtual Models.RegisterItem GetRegisterItem(string parentregister, string register, string item, int? vnr)
        {
            if (string.IsNullOrWhiteSpace(parentregister))
            {
                vnr = getVnr(parentregister, register, item, vnr);
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   o.versionNumber == vnr
                                   select o;

                return queryResults.FirstOrDefault();
            }
            else
            {
                vnr = getVnr(parentregister, register, item, vnr);
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   o.versionNumber == vnr &&
                                   (o.register.parentRegister.seoname == parentregister || o.register.parentRegister.name == parentregister)
                                   select o;

                return queryResults.FirstOrDefault();
            }
        }

        private int? getVnr(string parentregister, string register, string item, int? vnr)
        {
            if (vnr == null)
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register)
                                   select o;

                List<Models.RegisterItem> items = queryResults.ToList();
                if (items.Count > 1)
                {
                    foreach (Models.RegisterItem version in items)
                    {
                        if (version.versioningId != null)
                        {
                            if (version.versioning.currentVersion == version.systemId)
                            {
                                return version.versionNumber;
                            }
                        }
                    }
                }
                else {
                    return queryResults.First().versionNumber;
                }
            }
            else {
                return vnr;
            }
            return 1;
        }

        public bool validateName(object model)
        {
            if (model is Models.RegisterItem)
            {
                Models.RegisterItem registeritem = (Models.RegisterItem)model;
                var queryResults = from o in _dbContext.RegisterItems
                                   where o.name == registeritem.name && 
                                         o.systemId != registeritem.systemId 
                                         && o.registerId == registeritem.registerId
                                         && o.versioningId != registeritem.versioningId
                                   select o.systemId;

                if (queryResults.Count() > 0)
                {
                    return false;
                }
                else {
                    return true;
                }
            }
            return false;
        }

        public void SaveNewRegisterItem(Models.RegisterItem item)
        {
            _dbContext.Entry(item).State = EntityState.Modified;
            _dbContext.RegisterItems.Add(item);
            _dbContext.SaveChanges();
        }

        public void SaveEditedRegisterItem(Models.RegisterItem item)
        {
            _dbContext.Entry(item).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public SelectList GetRegisterSelectList(Guid registerId)
        {
            return new SelectList(_dbContext.Registers, "systemId", "name", registerId);
        }

        public SelectList GetDokStatusSelectList(string dokStatusId)
        {
            return new SelectList(_dbContext.DokStatuses.OrderBy(s => s.description), "value", "description", dokStatusId);
        }

        public SelectList GetSubmitterSelectList(Guid submitterId)
        {
            return new SelectList(_dbContext.Organizations.OrderBy(s => s.name), "systemId", "name", submitterId);
        }

        public SelectList GetOwnerSelectList(Guid ownerId)
        {
            return new SelectList(_dbContext.Organizations.OrderBy(s => s.name), "systemId", "name", ownerId);
        }

        public SelectList GetThemeGroupSelectList(string themeGroupId)
        {
            return new SelectList(_dbContext.DOKThemes, "value", "description", themeGroupId);
        }

        public SelectList GetBroaderItems()
        {
            return new SelectList(_dbContext.CodelistValues.OrderBy(s => s.name).Where(s => s.systemId != s.broaderItemId), "systemId", "name");
        }

        public SelectList GetBroaderItems(Guid? broaderItemId)
        {
            return new SelectList(_dbContext.CodelistValues.OrderBy(s => s.name), "systemId", "name", broaderItemId);
        }

        public SelectList GetStatusSelectList(Models.RegisterItem registerItem)
        {
            return new SelectList(_dbContext.Statuses.OrderBy(s => s.description), "value", "description", registerItem.statusId);
        }

        public void SaveDeleteRegisterItem(Models.RegisterItem item)
        {
            _dbContext.RegisterItems.Remove(item);
            _dbContext.SaveChanges();
        }

        public Organization GetMunicipalOrganizationByNr(string municipalityNr)
        {
            string organizationNr = _municipalityService.LookupOrganizationNumberFromMunicipalityCode(municipalityNr);
            return GetOrganizationByOrganizationNr(organizationNr);
        }

        public CodelistValue GetMunicipalByNr(string municipalNr)
        {
            var queryResult = from c in _dbContext.CodelistValues
                              where c.value == municipalNr
                              select c;
            return queryResult.FirstOrDefault();
        }

        private Organization GetOrganizationByOrganizationNr(string number)
        {
            var queryResults = from o in _dbContext.Organizations
                               where o.number == number
                               select o;

            return queryResults.FirstOrDefault();
        }

        public List<CodelistValue> GetMunicipalityList()
        {
            var queryresultMunicipality = from c in _dbContext.CodelistValues
                                          where c.register.name == "Kommunenummer"
                                          select c;

            return queryresultMunicipality.ToList();
        }

        public CoverageDataset GetMunicipalityCoverage(Dataset dataset)
        {
            AccessControlService _accessControlService = new AccessControlService();
            Organization municipality = _accessControlService.MunicipalUserOrganization();
            var queryResult = from c in _dbContext.CoverageDatasets
                              where c.Municipality.systemId == municipality.systemId
                              && c.dataset.systemId == dataset.systemId
                              select c;

            CoverageDataset municipalCoverage = queryResult.FirstOrDefault();

            return municipalCoverage;
        }

        public void SaveNewCoverage(CoverageDataset cover)
        {
            _dbContext.Entry(cover).State = EntityState.Modified;
            _dbContext.CoverageDatasets.Add(cover);
            _dbContext.SaveChanges();
        }

        public void DeleteCoverage(CoverageDataset coverage)
        {
            _dbContext.CoverageDatasets.Remove(coverage);
            _dbContext.SaveChanges();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }    

}