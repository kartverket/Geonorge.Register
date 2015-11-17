using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kartverket.Register.Services.RegisterItem
{
    public class RegisterItemService : IRegisterItemService
    {
        private readonly RegisterDbContext _dbContext;

        public RegisterItemService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
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

        private Kartverket.Register.Models.RegisterItem getItemById(Guid id)
        {
            var queryresult = from ri in _dbContext.RegisterItems
                              where ri.systemId == id
                              select ri;

            Kartverket.Register.Models.RegisterItem item = queryresult.FirstOrDefault();
            return item;
        }

        public Kartverket.Register.Models.RegisterItem GetCurrentRegisterItem(string parentregister, string register, string name)
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

        public Kartverket.Register.Models.RegisterItem GetCurrentSubregisterItem(string parentregister, string register, string name)
        {
            var queryResults = from o in _dbContext.RegisterItems
                               where (o.seoname == name || o.name == name) &&
                               (o.register.seoname == register || o.register.name == register) &&
                               (o.register.parentRegister.seoname == parentregister || o.register.parentRegister.name == parentregister)
                               && o.versioning.currentVersion == o.systemId
                               select o;

            Kartverket.Register.Models.RegisterItem registerItem = queryResults.FirstOrDefault();

            return registerItem;
        }

        public Models.Version GetVersionGroup(Guid? versioningId)
        {
            var queryResultVersions = from v in _dbContext.Versions
                                      where v.systemId == versioningId
                                      select v;

            Kartverket.Register.Models.Version versjonsgruppe = queryResultVersions.FirstOrDefault();
            return versjonsgruppe;
        }

        public Models.RegisterItem GetRegisterItemByVersionNr(string register, string item, int? vnr)
        {
            var queryResults = from o in _dbContext.RegisterItems
                               where (o.seoname == item || o.name == item) &&
                               (o.register.seoname == register || o.register.name == register) &&
                               o.versionNumber == vnr
                               select o;

            Models.RegisterItem registerItem = queryResults.FirstOrDefault();

            if (registerItem == null)
            {
                queryResults = from o in _dbContext.RegisterItems
                               where (o.seoname == item || o.name == item) &&
                               (o.register.seoname == register || o.register.name == register) &&
                               o.versionNumber == 1
                               select o;
                registerItem = queryResults.FirstOrDefault();
            }
            return registerItem;
        }

        public Models.RegisterItem GetSubregisterItemByVersionNr(string parentRegister, string register, string item, int? vnr)
        {
            var queryResults = from o in _dbContext.RegisterItems
                               where (o.seoname == item || o.name == item) &&
                               (o.register.seoname == register || o.register.name == register) &&
                               o.versionNumber == vnr &&
                               (o.register.parentRegister.seoname == parentRegister || o.register.parentRegister.name == parentRegister)
                               select o;

            Models.RegisterItem registerItem = queryResults.FirstOrDefault();
            return registerItem;
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
            Kartverket.Register.Models.Version versjoneringsGruppe = new Kartverket.Register.Models.Version();
            versjoneringsGruppe.systemId = Guid.NewGuid();
            versjoneringsGruppe.currentVersion = registerItem.systemId;
            versjoneringsGruppe.containedItemClass = registerItem.register.containedItemClass;
            versjoneringsGruppe.lastVersionNumber = registerItem.versionNumber;

            _dbContext.Entry(versjoneringsGruppe).State = EntityState.Modified;
            _dbContext.Versions.Add(versjoneringsGruppe);
            return versjoneringsGruppe.systemId;
        }

        public Models.RegisterItem GetRegisterItemByVersionNr(string parentregister, string register, string item, int vnr)
        {
            if (string.IsNullOrWhiteSpace(parentregister))
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   o.versionNumber == vnr
                                   select o;

                Models.RegisterItem registerItem = queryResults.FirstOrDefault();

                if (registerItem == null)
                {
                    queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   o.versionNumber == 1
                                   select o;
                    registerItem = queryResults.FirstOrDefault();
                }
                return registerItem;
            }
            else
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   o.versionNumber == vnr &&
                                   (o.register.parentRegister.seoname == parentregister || o.register.parentRegister.name == parentregister)
                                   select o;

                Models.RegisterItem registerItem = queryResults.FirstOrDefault();
                return registerItem;
            }
        }

        public Models.RegisterItem GetRegisterItem(string parentregister, string register, string item, int vnr = 1)
        {
            if (string.IsNullOrWhiteSpace(parentregister))
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   o.versionNumber == vnr
                                   select o;

                Models.RegisterItem registerItem = queryResults.FirstOrDefault();

                if (registerItem == null)
                {
                    queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   o.versionNumber == 1
                                   select o;
                    registerItem = queryResults.FirstOrDefault();
                }
                return registerItem;
            }
            else
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   o.versionNumber == vnr &&
                                   (o.register.parentRegister.seoname == parentregister || o.register.parentRegister.name == parentregister)
                                   select o;

                Models.RegisterItem registerItem = queryResults.FirstOrDefault();
                return registerItem;
            }
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
    }
}