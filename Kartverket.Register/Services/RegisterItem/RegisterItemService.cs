using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartverket.Register.Helpers;
using Kartverket.Register.Models.ViewModels;

namespace Kartverket.Register.Services.RegisterItem
{
    public class RegisterItemService : IRegisterItemService
    {
        private readonly RegisterDbContext _dbContext;
        private readonly ICodelistValueService _codelistValueService;
        private readonly IUserService _userService;

        public RegisterItemService(RegisterDbContext dbContext)
        {
            _dbContext = dbContext;
            _codelistValueService = new CodelistValueService(_dbContext);
            _userService = new UserService(_dbContext);
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
                        CodelistValue NarrowerItemsBroader = (CodelistValue)GetRegisterItemBySystemId(kodeSomSkalSlettes.systemId);
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
                    CodelistValue narrowerItem = (CodelistValue)GetRegisterItemBySystemId(narrowerId);
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
            CodelistValue broaderItem = (CodelistValue)GetRegisterItemBySystemId(broader);
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

        public Models.RegisterItem GetCurrentRegisterItem(string parentregister, string register, string name)
        {
            Models.RegisterItem registerItem = null;
            if (string.IsNullOrWhiteSpace(parentregister))
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == name || o.name == name) &&
                                   (o.register.seoname == register || o.register.name == register)
                                   && o.versioning.currentVersion == o.systemId
                                   select o;

                registerItem = queryResults.FirstOrDefault();
            }
            else
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == name || o.name == name) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   (o.register.parentRegister.seoname == parentregister || o.register.parentRegister.name == parentregister)
                                   && o.versioning.currentVersion == o.systemId
                                   select o;

                registerItem = queryResults.FirstOrDefault();
            }

            if (registerItem == null)
            {
                registerItem = GetRegisterItemByName(parentregister, register, name);
            }
            return registerItem;

        }

        private Models.RegisterItem GetRegisterItemByName(string parentregister, string register, string item)
        {
            if (string.IsNullOrWhiteSpace(parentregister))
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register)
                                   select o;

                return queryResults.FirstOrDefault();
            }
            else
            {
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   (o.register.parentRegister.seoname == parentregister || o.register.parentRegister.name == parentregister)
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
            var registerItem = GetRegisterItem(parent, register, item, null);
            var versions = new List<Models.RegisterItem>();

            if (registerItem != null)
            {
                var queryResultVersions = from r in _dbContext.RegisterItems
                                          where r.versioningId == registerItem.versioningId
                                          select r;
                versions = queryResultVersions.ToList();
            }
            return versions;

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

        public Guid NewVersioningGroup(Models.RegisterItem registerItem)
        {
            Models.Version versjoneringsGruppe = new Models.Version();
            versjoneringsGruppe.systemId = Guid.NewGuid();
            versjoneringsGruppe.currentVersion = registerItem.systemId;
            versjoneringsGruppe.containedItemClass = registerItem.register.containedItemClass;
            versjoneringsGruppe.lastVersionNumber = registerItem.versionNumber;

            _dbContext.Entry(versjoneringsGruppe).State = EntityState.Modified;
            _dbContext.Versions.Add(versjoneringsGruppe);
            _dbContext.SaveChanges();
            return versjoneringsGruppe.systemId;
        }

        public Guid NewVersioningGroup(RegisterItemV2 registerItem)
        {
            var versjoneringsGruppe = new Models.Version();
            versjoneringsGruppe.systemId = Guid.NewGuid();
            versjoneringsGruppe.currentVersion = registerItem.SystemId;
            versjoneringsGruppe.lastVersionNumber = registerItem.VersionNumber;

            _dbContext.Entry(versjoneringsGruppe).State = EntityState.Modified;
            _dbContext.Versions.Add(versjoneringsGruppe);
            _dbContext.SaveChanges();
            return versjoneringsGruppe.systemId;
        }

        public CoverageDataset NewCoverage(Models.RegisterItem registerItem)
        {
            CoverageDataset coverage = new CoverageDataset();
            coverage.CoverageId = Guid.NewGuid();
            coverage.CoverageDOKStatusId = "Accepted";
            coverage.ConfirmedDok = true;
            coverage.DatasetId = registerItem.systemId;
            if (registerItem is Dataset)
            {
                Dataset dataset = (Dataset)registerItem;
                dataset.dokStatusDateAccepted = DateTime.Now;
                coverage.MunicipalityId = dataset.datasetownerId;
                coverage.Note = dataset.Notes;
                coverage.Coverage = true;
            }

            _dbContext.Entry(coverage).State = EntityState.Modified;
            _dbContext.CoverageDatasets.Add(coverage);
            return coverage;
        }

        public virtual Models.RegisterItem GetRegisterItem(string parentregister, string register, string item, int? vnr, string itemowner = null)
        {
            var registerItems = new List<Models.RegisterItem>();

            if (string.IsNullOrWhiteSpace(parentregister))
            {
                vnr = getVnr(parentregister, register, item, vnr);
                var queryResults = from o in _dbContext.RegisterItems
                                   where (o.seoname == item || o.name == item) &&
                                   (o.register.seoname == register || o.register.name == register) &&
                                   (o.versionNumber == vnr || o.versionNumber == 0)
                                   select o;

                registerItems = queryResults.ToList();
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

                registerItems = queryResults.ToList();
            }
            if (itemowner != null)
            {
                foreach (Models.RegisterItem registerItem in registerItems)
                {
                    if (registerItem is Dataset)
                    {
                        Dataset dataset = (Dataset)registerItem;
                        if (dataset.datasetowner.seoname == itemowner)
                        {
                            return registerItem;
                        }
                    }
                }
            }
            return registerItems.FirstOrDefault();
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
                if (items.Count >= 1)
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
                else
                {
                    if (queryResults.Any())
                    {
                        return queryResults.First().versionNumber;
                    }
                }
            }
            else
            {
                return vnr;
            }
            return 1;
        }

        /// <summary>
        /// Item name must be unique in one registery
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ItemNameIsValid(object model)
        {
            if (model is Models.RegisterItem)
            {
                return model is Dataset ? ValidateNameDataset(model) : ValidateNameRegisterItem(model);
            }
            if (model is RegisterItemV2ViewModel)
            {
                if (model is InspireDatasetViewModel)
                {
                    return ValidNameInspireDataset((InspireDatasetViewModel)model);
                }
            }
            if (model is InspireDataset)
            {
                return ValidNameInspireDataset((InspireDataset)model);
            }
            if (model is GeodatalovDataset)
            {
                return GeodatalovDatasetNameAlreadyExist((GeodatalovDataset)model);
            }
            return false;
        }

        private bool GeodatalovDatasetNameAlreadyExist(GeodatalovDataset geodatalovDataset)
        {
            if (geodatalovDataset == null) throw new ArgumentNullException(nameof(geodatalovDataset));
            var queryResults = from o in _dbContext.GeodatalovDatasets
                               where (o.Name == geodatalovDataset.Name || o.Seoname == RegisterUrls.MakeSeoFriendlyString(geodatalovDataset.Name)) &&
                                     o.SystemId != geodatalovDataset.SystemId
                                     && o.RegisterId == geodatalovDataset.RegisterId
                               select o.SystemId;

            return !queryResults.Any();
        }

        private bool ValidNameInspireDataset(InspireDatasetViewModel inspireDataset)
        {
            var queryResults = from o in _dbContext.InspireDatasets
                               where (o.Name == inspireDataset.Name || o.Seoname == RegisterUrls.MakeSeoFriendlyString(inspireDataset.Name)) &&
                                     o.SystemId != inspireDataset.SystemId
                                     && o.RegisterId == inspireDataset.RegisterId
                               select o.SystemId;

            return !queryResults.ToList().Any();
        }

        private bool ValidNameInspireDataset(InspireDataset inspireDataset)
        {
            if (inspireDataset == null) throw new ArgumentNullException(nameof(inspireDataset));
            var queryResults = from o in _dbContext.InspireDatasets
                               where (o.Name == inspireDataset.Name || o.Seoname == RegisterUrls.MakeSeoFriendlyString(inspireDataset.Name)) &&
                                     o.SystemId != inspireDataset.SystemId
                                     && o.RegisterId == inspireDataset.RegisterId
                               select o.SystemId;

            return !queryResults.ToList().Any();
        }

        private bool ValidateNameRegisterItem(object model)
        {
            Models.RegisterItem registeritem = (Models.RegisterItem)model;
            var queryResults = from o in _dbContext.RegisterItems
                               where (o.name == registeritem.name || o.seoname == RegisterUrls.MakeSeoFriendlyString(registeritem.name)) &&
                                     o.systemId != registeritem.systemId
                                     && o.registerId == registeritem.registerId
                                     && o.versioningId != registeritem.versioningId
                               select o;

            return !queryResults.ToList().Any();
        }

        private bool ValidateNameDataset(object model)
        {
            Dataset dataset = (Dataset)model;
            if (dataset.register.IsDokMunicipal())
            {
                var queryResultsDataset = from o in _dbContext.Datasets
                                          where (o.name == dataset.name || o.seoname == RegisterUrls.MakeSeoFriendlyString(dataset.name))
                                                && o.systemId != dataset.systemId
                                                && o.registerId == dataset.registerId
                                                //&& o.versioningId != dataset.versioningId
                                                && o.datasetownerId == dataset.datasetownerId
                                          select o.systemId;

                return !queryResultsDataset.ToList().Any();
            }

            return ValidateNameRegisterItem(model);
        }

        public void SaveNewRegisterItem(Models.RegisterItem item)
        {
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
            return new SelectList(_dbContext.DokStatuses.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }).OrderBy(s => s.description), "value", "description", dokStatusId);
        }

        public SelectList GetDokDeliveryStatusSelectList(string dokDeliveryStatusId)
        {
            return new SelectList(_dbContext.DokDeliveryStatuses.OrderBy(s => s.description), "value", "description", dokDeliveryStatusId);
        }

        public SelectList GetSubmitterSelectList(Guid submitterId)
        {
            return new SelectList(_dbContext.Organizations.ToList().Select(s => new { systemId = s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", submitterId);
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
            return new SelectList(_dbContext.CodelistValues.ToList().Select(s => new { systemId = s.systemId, name = s.NameTranslated() }).OrderBy(s => s.name), "systemId", "name", broaderItemId);
        }

        public SelectList GetStatusSelectList(Models.RegisterItem registerItem)
        {
            return new SelectList(_dbContext.Statuses.ToList().Select(s => new { value = s.value, description = s.DescriptionTranslated() }).OrderBy(s => s.description), "value", "description", registerItem.statusId);
        }

        public void SaveDeleteRegisterItem(Models.RegisterItem item)
        {
            _dbContext.RegisterItems.Remove(item);
            _dbContext.SaveChanges();
        }

        public Organization GetMunicipalityOrganizationByNr(string municipalityNr)
        {
            if (!string.IsNullOrWhiteSpace(municipalityNr))
            {
                var queryResults = from o in _dbContext.Organizations
                                   where o.MunicipalityCode == municipalityNr
                                   select o;
                return queryResults.FirstOrDefault();
            }
            return null;
        }

        public CodelistValue GetMunicipalityByNr(string municipalNr)
        {
            var queryResult = from c in _dbContext.CodelistValues
                              where c.register.name == "Kommunenummer"
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
                                          && c.value != "2321"
                                          && c.value != "2311"
                                          && c.value != "2211"
                                          && c.value != "2121"
                                          && c.value != "2131"
                                          && c.value != "2111"
                                          select c;

            return queryresultMunicipality.OrderBy(o => o.name).ToList();
        }

        public CoverageDataset GetMunicipalityCoverage(Dataset dataset, Guid? originalDocumentOwnerId = null)
        {
            AccessControlService _accessControlService = new AccessControlService(_dbContext);
            Organization municipality = new Organization();
            if (dataset.IsNationalDataset())
            {
                municipality = _accessControlService.MunicipalUserOrganization();
            }
            else
            {
                if (originalDocumentOwnerId == null || originalDocumentOwnerId == Guid.Empty)
                {
                    municipality = GetMunicipality(dataset.datasetownerId, _accessControlService);
                }
                else
                {
                    municipality = GetMunicipality(originalDocumentOwnerId.Value, _accessControlService);
                }
            }
            var queryResult = from c in _dbContext.CoverageDatasets
                              where c.Municipality.systemId == municipality.systemId
                              && c.dataset.systemId == dataset.systemId
                              select c;

            CoverageDataset municipalCoverage = queryResult.FirstOrDefault();

            return municipalCoverage;
        }

        private Organization GetMunicipality(Guid? owner, AccessControlService _accessControlService)
        {
            Organization municipality;
            if (owner != null && owner != Guid.Empty)
            {
                municipality = (Organization)GetRegisterItemBySystemId(owner.Value);
            }
            else
            {
                municipality = _accessControlService.MunicipalUserOrganization();
            }

            return municipality;
        }

        public void SaveNewCoverage(CoverageDataset cover)
        {
            _dbContext.CoverageDatasets.Add(cover);
            _dbContext.SaveChanges();
        }

        public void DeleteCoverage(CoverageDataset coverage)
        {
            _dbContext.CoverageDatasets.Remove(coverage);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public Models.RegisterItem GetRegisterItemBySystemId(Guid systemId)
        {
            var queryResult = from c in _dbContext.RegisterItems
                              where c.systemId == systemId
                              select c;

            return queryResult.FirstOrDefault();
        }

        

        public string GetDOKMunicipalStatus(Models.RegisterItem municipality)
        {
            var queryResult = from c in _dbContext.Organizations
                              where c.systemId == municipality.systemId
                              select c.StatusConfirmationMunicipalDOK;

            return queryResult.FirstOrDefault();
        }

        public ICollection<RegisterItemV2ViewModel> OrderBy(ICollection<RegisterItemV2ViewModel> registerItems, string sorting)
        {
            if (registerItems != null)
            {
                switch (sorting)
                {
                    // RegisterItemV2
                    case "name":
                        return registerItems.OrderBy(o => o.Name).ToList();
                    case "name_desc":
                        return registerItems.OrderByDescending(o => o.Name).ToList();
                    case "owner":
                        return registerItems.OrderBy(o => o.Owner.name).ToList();
                    case "owner_desc":
                        return registerItems.OrderByDescending(o => o.Owner.name).ToList();
                    case "status":
                        return registerItems.OrderBy(o => o.Status.DescriptionTranslated()).ToList();
                    case "status_desc":
                        return registerItems.OrderByDescending(o => o.Status.DescriptionTranslated()).ToList();
                    case "dateSubmitted":
                        return registerItems.OrderBy(o => o.DateSubmitted).ToList();
                    case "dateSubmitted_desc":
                        return registerItems.OrderByDescending(o => o.DateSubmitted).ToList();
                    case "modified":
                        return registerItems.OrderBy(o => o.Modified).ToList();
                    case "modified_desc":
                        return registerItems.OrderByDescending(o => o.Modified).ToList();
                    case "dateAccepted":
                        return registerItems.OrderBy(o => o.DateAccepted).ToList();
                    case "dateAccepted_desc":
                        return registerItems.OrderByDescending(o => o.DateAccepted).ToList();

                    // DatasetV2
                    case "theme":
                        {
                            var sortedList = registerItems.OfType<DatasetViewModel>().OrderBy(o => o.Theme.description).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "theme_desc":
                        {
                            var sortedList = registerItems.OfType<DatasetViewModel>().OrderByDescending(o => o.Theme.description).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "dokstatus":
                        {
                            var sortedList = registerItems.OfType<DatasetViewModel>().OrderBy(o => o.DokStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "dokstatus_desc":
                        {
                            var sortedList = registerItems.OfType<DatasetViewModel>().OrderByDescending(o => o.DokStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }

                    // InspireDataset
                    case "inspiretheme":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderBy(o => o.InspireThemsAsString()).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspiretheme_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderByDescending(o => o.InspireThemsAsString()).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_metadata_status":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderBy(o => o.MetadataStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_metadata_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderByDescending(o => o.MetadataStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_metadataservice_status":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderBy(o => o.MetadataServiceStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_metadataservice_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderByDescending(o => o.MetadataServiceStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_distribution_status":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderBy(o => o.DistributionStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_distribution_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderByDescending(o => o.DistributionStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_wms_status":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderBy(o => o.WmsStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_wms_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderByDescending(o => o.WmsStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_wfs_status":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderBy(o => o.WfsStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_wfs_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderByDescending(o => o.WfsStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_atom_status":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderBy(o => o.AtomFeedStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_atom_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderByDescending(o => o.AtomFeedStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_wfsoratom_status":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderBy(o => o.WfsOrAtomStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_wfsoratom_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderByDescending(o => o.WfsOrAtomStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_harmonizeddata_status":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderBy(o => o.HarmonizedDataStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_harmonizeddata_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderByDescending(o => o.HarmonizedDataStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_spatialdataservice_status":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderBy(o => o.SpatialDataServiceStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_spatialdataservice_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDatasetViewModel>().OrderByDescending(o => o.SpatialDataServiceStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }

                    // InspireDataService
                    case "inspire_theme_status":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderBy(o => o.InspireThemsAsString()).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_theme_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderByDescending(o => o.InspireThemsAsString()).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_serviceType":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderBy(o => o.InspireDataType).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_serviceType_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderByDescending(o => o.InspireDataType).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "request":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderBy(o => o.Requests).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "request_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderByDescending(o => o.Requests).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "sds":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderBy(o => o.Sds).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "sds_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderByDescending(o => o.Sds).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "networkService":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderBy(o => o.NetworkService).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "networkService_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderByDescending(o => o.NetworkService).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspireService_metadata_status":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderBy(o => o.MetadataStatus).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspireService_metadata_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderByDescending(o => o.MetadataStatus).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_metadataSearchService_status":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderBy(o => o.MetadataInSearchServiceStatus).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_metadataSearchService_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderByDescending(o => o.MetadataInSearchServiceStatus).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_service_status":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderBy(o => o.ServiceStatus).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_service_status_desc":
                        {
                            var sortedList = registerItems.OfType<InspireDataServiceViewModel>().OrderByDescending(o => o.ServiceStatus).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }

                    // GeodatalovDataset
                    case "geodatalov_metadata_status":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.MetadataStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_metadata_status_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.MetadataStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_productspecification_status":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.ProductSpesificationStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_productspecification_status_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.ProductSpesificationStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_sosi_status":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.SosiDataStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_sosi_status_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.SosiDataStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_gml_status":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.GmlDataStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_gml_status_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.GmlDataStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_wms_status":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.WmsStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_wms_status_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.WmsStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_wfs_status":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.WfsStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_wfs_status_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.WfsStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_atom_status":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.AtomFeedStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_atom_status_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.AtomFeedStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_common_status":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.CommonStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_common_status_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.CommonStatusId).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.InspireTheme).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "inspire_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.InspireTheme).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "dok":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.Dok).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "dok_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.Dok).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "nationalt_dataset":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.NationalDataset).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "nationalt_dataset_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.NationalDataset).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "plan":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.Plan).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "plan_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.Plan).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderBy(o => o.Geodatalov).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "geodatalov_desc":
                        {
                            var sortedList = registerItems.OfType<GeodatalovDatasetViewModel>().OrderByDescending(o => o.Geodatalov).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                }
            }
            return registerItems;
        }

        public ICollection<Models.RegisterItem> OrderBy(ICollection<Models.RegisterItem> registerItems, string sorting)
        {
            if (registerItems.Any())
            {
                var text = HttpContext.Current.Request.QueryString["text"] != null ? HttpContext.Current.Request.QueryString["text"].ToString() : "";
                var filterVertikalt = HttpContext.Current.Request.QueryString["filterVertikalt"] != null ? HttpContext.Current.Request.QueryString["filterVertikalt"].ToString() : "";
                var municipality = HttpContext.Current.Request.QueryString["municipality"] != null ? HttpContext.Current.Request.QueryString["municipality"].ToString() : "";
                var filterHorisontalt = HttpContext.Current.Request.QueryString["filterHorisontalt"] != null ? HttpContext.Current.Request.QueryString["filterHorisontalt"].ToString() : "";
                var inspireRequirementParam = HttpContext.Current.Request.QueryString["InspireRequirement"] != null ? HttpContext.Current.Request.QueryString["InspireRequirement"].ToString() : "";
                var nationalRequirementParam = HttpContext.Current.Request.QueryString["nationalRequirement"] != null ? HttpContext.Current.Request.QueryString["nationalRequirement"].ToString() : "";
                var nationalSeaRequirementParam = HttpContext.Current.Request.QueryString["nationalSeaRequirement"] != null ? HttpContext.Current.Request.QueryString["nationalSeaRequirement"].ToString() : "";
                var inspireRegistryTab = HttpContext.Current.Request.QueryString["inspireRegistryTab"] != null ? HttpContext.Current.Request.QueryString["inspireRegistryTab"].ToString() : "";

                if (HttpContext.Current.Request.QueryString.Count < 1)
                {
                    if (HttpContext.Current.Session != null)
                    {
                        if (HttpContext.Current.Session["sortingType"] != null && string.IsNullOrEmpty(sorting))
                            sorting = HttpContext.Current.Session["sortingType"].ToString();

                        if (HttpContext.Current.Session["text"] != null && string.IsNullOrEmpty(text))
                            text = HttpContext.Current.Session["text"].ToString();

                        if (HttpContext.Current.Session["filterVertikalt"] != null && string.IsNullOrEmpty(filterVertikalt))
                            filterVertikalt = HttpContext.Current.Session["filterVertikalt"].ToString();

                        if (HttpContext.Current.Session["filterHorisontalt"] != null && string.IsNullOrEmpty(filterHorisontalt))
                            filterHorisontalt = HttpContext.Current.Session["filterHorisontalt"].ToString();

                        if (HttpContext.Current.Session["InspireRequirement"] != null && string.IsNullOrEmpty(inspireRequirementParam))
                            inspireRequirementParam = HttpContext.Current.Session["InspireRequirement"].ToString();

                        if (HttpContext.Current.Session["nationalRequirement"] != null && string.IsNullOrEmpty(nationalRequirementParam))
                            nationalRequirementParam = HttpContext.Current.Session["nationalRequirement"].ToString();

                        if (HttpContext.Current.Session["nationalSeaRequirement"] != null && string.IsNullOrEmpty(nationalSeaRequirementParam))
                            nationalSeaRequirementParam = HttpContext.Current.Session["nationalSeaRequirement"].ToString();

                        if (HttpContext.Current.Session["municipality"] != null && string.IsNullOrEmpty(municipality))
                            municipality = HttpContext.Current.Session["municipality"].ToString();

                        if (HttpContext.Current.Session["inspireRegistryTab"] != null && string.IsNullOrEmpty(inspireRegistryTab))
                            municipality = HttpContext.Current.Session["inspireRegistryTab"].ToString();



                        string redirect = HttpContext.Current.Request.Path + "?sorting=" + sorting;
                        bool shallRedirect = false;

                        if (text != "")
                        {
                            redirect = redirect + "&text=" + text;
                            shallRedirect = true;
                        }

                        if (filterVertikalt != "")
                        {
                            if (filterVertikalt.Contains(","))
                                filterVertikalt = filterVertikalt.Replace(",false", "");
                            redirect = redirect + "&filterVertikalt=" + filterVertikalt;
                            shallRedirect = true;
                        }

                        if (filterHorisontalt != "")
                        {
                            if (filterHorisontalt.Contains(","))
                                filterHorisontalt = filterHorisontalt.Replace(",false", "");
                            redirect = redirect + "&filterHorisontalt=" + filterHorisontalt;
                            shallRedirect = true;
                        }

                        if (inspireRequirementParam != "")
                        {
                            redirect = redirect + "&inspireRequirement=" + inspireRequirementParam;
                            shallRedirect = true;
                        }

                        if (nationalRequirementParam != "")
                        {
                            redirect = redirect + "&nationalRequirement=" + nationalRequirementParam;
                            shallRedirect = true;
                        }

                        if (nationalSeaRequirementParam != "")
                        {
                            redirect = redirect + "&nationalSeaRequirement=" + nationalSeaRequirementParam;
                            shallRedirect = true;
                        }

                        if (nationalSeaRequirementParam != "")
                        {
                            redirect = redirect + "&municipality=" + municipality;
                            shallRedirect = true;
                        }

                        if (inspireRegistryTab != "")
                        {
                            redirect = redirect + "&inspireRegistryTab=" + inspireRegistryTab;
                            shallRedirect = true;
                        }


                        if (shallRedirect)
                        {
                            HttpContext.Current.Response.Redirect(redirect);
                        }

                    }
                }
                HttpContext.Current.Session["sortingType"] = sorting;
                HttpContext.Current.Session["municipality"] = municipality;
                HttpContext.Current.Session["text"] = text;
                HttpContext.Current.Session["filterVertikalt"] = filterVertikalt;
                HttpContext.Current.Session["filterHorisontalt"] = filterHorisontalt;
                HttpContext.Current.Session["InspireRequirement"] = inspireRequirementParam;
                HttpContext.Current.Session["nationalRequirement"] = nationalRequirementParam;
                HttpContext.Current.Session["nationalSeaRequirement"] = nationalSeaRequirementParam;
                HttpContext.Current.Session["inspireRegistryTab"] = inspireRegistryTab;

                List<string> dokStatusOrder = new List<string>();

                dokStatusOrder.Add("good");
                dokStatusOrder.Add("useable");
                dokStatusOrder.Add("deficient");
                dokStatusOrder.Add("notset");


                var sortedList = registerItems.OrderBy(o => o.NameTranslated()).ToList();

                // ***** RegisterItems

                if (sorting == "name_desc")
                {
                    sortedList = registerItems.OrderByDescending(o => o.NameTranslated()).ToList();
                }
                else if (sorting == "description")
                {
                    sortedList = registerItems.OrderBy(o => o.description).ToList();
                }
                else if (sorting == "description_desc")
                {
                    sortedList = registerItems.OrderByDescending(o => o.description).ToList();
                }
                else if (sorting == "submitter")
                {
                    sortedList = registerItems.OrderBy(o => o.submitter.NameTranslated()).ToList();
                }
                else if (sorting == "submitter_desc")
                {
                    sortedList = registerItems.OrderByDescending(o => o.submitter.name).ToList();
                }
                else if (sorting == "status")
                {
                    sortedList = registerItems.OrderBy(o => o.status.DescriptionTranslated()).ToList();
                }
                else if (sorting == "status_desc")
                {
                    sortedList = registerItems.OrderByDescending(o => o.status.DescriptionTranslated()).ToList();
                }
                else if (sorting == "dateSubmitted")
                {
                    sortedList = registerItems.OrderBy(o => o.dateSubmitted).ToList();
                }
                else if (sorting == "dateSubmitted_desc")
                {
                    sortedList = registerItems.OrderByDescending(o => o.dateSubmitted).ToList();
                }
                else if (sorting == "modified")
                {
                    sortedList = registerItems.OrderBy(o => o.modified).ToList();
                }
                else if (sorting == "modified_desc")
                {
                    sortedList = registerItems.OrderByDescending(o => o.modified).ToList();
                }
                else if (sorting == "dateAccepted")
                {
                    sortedList = registerItems.OrderBy(o => o.dateAccepted).ToList();
                }
                else if (sorting == "dateAccepted_desc")
                {
                    sortedList = registerItems.OrderByDescending(o => o.dateAccepted).ToList();
                }

                // ***** Documents 

                else if (sorting == "documentOwner")
                {
                    var documentSorted = registerItems.OfType<Document>().OrderBy(o => o.documentowner.NameTranslated());
                    sortedList = documentSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "documentOwner_desc")
                {
                    var documentSorted = registerItems.OfType<Document>().OrderByDescending(o => o.documentowner.NameTranslated());
                    sortedList = documentSorted.Cast<Models.RegisterItem>().ToList();
                }

                // ***** Dataset
                else if (sorting == "datasetOwner")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => o.datasetowner.NameTranslated());
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "datasetOwner_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => o.datasetowner.NameTranslated());
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokstatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => o.dokStatus.DescriptionTranslated());
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokstatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => o.dokStatus.DescriptionTranslated());
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "distributionFormat")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => o.DistributionFormat);
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "distributionFormat_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => o.DistributionFormat);
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "wmsUrl")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => o.WmsUrl);
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "wmsUrl_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => o.WmsUrl);
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "theme")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => o.theme == null ? "" : o.theme.value);
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "theme_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => o.theme == null ? "" : o.theme.value);
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "type")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => o.DatasetType);
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "type_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => o.DatasetType);
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }


                //DOK delivery status

                else if (sorting == "dokDeliveryMetadataStatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => dokStatusOrder.IndexOf(o.dokDeliveryMetadataStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryMetadataStatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => dokStatusOrder.IndexOf(o.dokDeliveryMetadataStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }

                else if (sorting == "dokDeliveryProductSheetStatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => dokStatusOrder.IndexOf(o.dokDeliveryProductSheetStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryProductSheetStatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => dokStatusOrder.IndexOf(o.dokDeliveryProductSheetStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryPresentationRulesStatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => dokStatusOrder.IndexOf(o.dokDeliveryPresentationRulesStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryPresentationRulesStatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => dokStatusOrder.IndexOf(o.dokDeliveryPresentationRulesStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryProductSpecificationStatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => dokStatusOrder.IndexOf(o.dokDeliveryProductSpecificationStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryProductSpecificationStatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => dokStatusOrder.IndexOf(o.dokDeliveryProductSpecificationStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryWmsStatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => dokStatusOrder.IndexOf(o.dokDeliveryWmsStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryWmsStatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => dokStatusOrder.IndexOf(o.dokDeliveryWmsStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryWfsStatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => dokStatusOrder.IndexOf(o.dokDeliveryWfsStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryWfsStatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => dokStatusOrder.IndexOf(o.dokDeliveryWfsStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliverySosiRequirementsStatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => dokStatusOrder.IndexOf(o.dokDeliverySosiRequirementsStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliverySosiRequirementsStatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => dokStatusOrder.IndexOf(o.dokDeliverySosiRequirementsStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryDistributionStatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => dokStatusOrder.IndexOf(o.dokDeliveryDistributionStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryDistributionStatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => dokStatusOrder.IndexOf(o.dokDeliveryDistributionStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryGmlRequirementsStatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => dokStatusOrder.IndexOf(o.dokDeliveryGmlRequirementsStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryGmlRequirementsStatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => dokStatusOrder.IndexOf(o.dokDeliveryGmlRequirementsStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryAtomFeedStatus")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderBy(o => dokStatusOrder.IndexOf(o.dokDeliveryAtomFeedStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dokDeliveryAtomFeedStatus_desc")
                {
                    var datasetSorted = registerItems.OfType<Dataset>().OrderByDescending(o => dokStatusOrder.IndexOf(o.dokDeliveryAtomFeedStatusId));
                    sortedList = datasetSorted.Cast<Models.RegisterItem>().ToList();
                }


                // ***** CodelistValue

                else if (sorting == "codevalue")
                {
                    var codevalue = registerItems.OfType<CodelistValue>().OrderBy(o => o.value);
                    sortedList = codevalue.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "codevalue_desc")
                {
                    var codevalue = registerItems.OfType<CodelistValue>().OrderByDescending(o => o.value);
                    sortedList = codevalue.Cast<Models.RegisterItem>().ToList();
                }

                // ***** Organization

                else if (sorting == "number")
                {
                    var number = registerItems.OfType<Organization>().OrderBy(o => o.number);
                    sortedList = number.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "number_desc")
                {
                    var number = registerItems.OfType<Organization>().OrderByDescending(o => o.number);
                    sortedList = number.Cast<Models.RegisterItem>().ToList();
                }

                // ***** EPSG
                else if (sorting == "vertical")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderBy(o => o.verticalReferenceSystem);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "vertical_desc")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderByDescending(o => o.verticalReferenceSystem);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "horizontal")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderBy(o => o.horizontalReferenceSystem);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "horizontal_desc")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderByDescending(o => o.horizontalReferenceSystem);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dimension")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderBy(o => o.dimension == null ? "" : o.dimension.DescriptionTranslated());
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "dimension_desc")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderByDescending(o => o.dimension == null ? "" : o.dimension.DescriptionTranslated());
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "epsg")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderBy(o => o.epsgcode);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "epsg_desc")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderByDescending(o => o.epsgcode);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "sosi")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderBy(o => o.sosiReferencesystem);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "sosi_desc")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderByDescending(o => o.sosiReferencesystem);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "inspireRequirement")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderBy(o => o.inspireRequirement.sortOrder);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "inspireRequirement_desc")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderByDescending(o => o.inspireRequirement.sortOrder);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "nationalRequirement")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderBy(o => o.nationalRequirement.sortOrder);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "nationalRequirement_desc")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderByDescending(o => o.nationalRequirement.sortOrder);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "nationalSeasRequirement")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderBy(o => o.nationalSeasRequirement.sortOrder);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "nationalSeasRequirement_desc")
                {
                    var epsgSorting = registerItems.OfType<EPSG>().OrderByDescending(o => o.nationalSeasRequirement.sortOrder);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }

                // ***** ServiceAlert
                else if (sorting == "alertdate")
                {
                    var epsgSorting = registerItems.OfType<ServiceAlert>().OrderBy(o => o.AlertDate);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "alertdate_desc")
                {
                    var epsgSorting = registerItems.OfType<ServiceAlert>().OrderByDescending(o => o.AlertDate);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "effektivedate")
                {
                    var epsgSorting = registerItems.OfType<ServiceAlert>().OrderBy(o => o.EffectiveDate);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "effektivedate_desc")
                {
                    var epsgSorting = registerItems.OfType<ServiceAlert>().OrderByDescending(o => o.EffectiveDate);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "owner")
                {
                    var epsgSorting = registerItems.OfType<ServiceAlert>().OrderBy(o => o.Owner);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "owner_desc")
                {
                    var epsgSorting = registerItems.OfType<ServiceAlert>().OrderByDescending(o => o.Owner);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "servicetype")
                {
                    var epsgSorting = registerItems.OfType<ServiceAlert>().OrderBy(o => o.ServiceType);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "servicetype_desc")
                {
                    var epsgSorting = registerItems.OfType<ServiceAlert>().OrderByDescending(o => o.ServiceType);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "servicealert")
                {
                    var epsgSorting = registerItems.OfType<ServiceAlert>().OrderBy(o => o.AlertType);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "servicealert_desc")
                {
                    var epsgSorting = registerItems.OfType<ServiceAlert>().OrderByDescending(o => o.AlertType);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }

                return sortedList;
            }
            return registerItems;
        }

        public void MakeAllRegisterItemsValid(Models.Register register)
        {
            foreach (var item in register.items)
            {
                item.statusId = "Valid";
                _dbContext.Entry(item).State = EntityState.Modified;
            }
            _dbContext.SaveChanges();
        }

        public string GetDistributionType(string codeValue)
        {
            var queryResults = from d in _dbContext.CodelistValues
                               where d.value == codeValue
                               select d.name;

            return queryResults.FirstOrDefault();
        }

        public void DeleteRegisterItems(ICollection<Models.RegisterItem> registerItems)
        {
            foreach (var registerItem in registerItems.ToList())
            {
                if (registerItem is CodelistValue codelistValue)
                {
                    RemoveBroaderAndNarrower(codelistValue);
                }
                _dbContext.RegisterItems.Remove(registerItem);
            }
        }

        public void ImportRegisterItemFromFile(Models.Register register, HttpPostedFileBase file)
        {
            var csvreader = new StreamReader(file.InputStream);

            if (csvreader.EndOfStream) return;
            csvreader.ReadLine(); // Overskift
            if (register.ContainedItemClassIsCodelistValue())
            {
                while (!csvreader.EndOfStream)
                {
                    var line = csvreader.ReadLine();
                    var codeListValueImport = line.Split(';');

                    var codelistValue = _codelistValueService.NewCodelistValueFromImport(register, codeListValueImport);
                    if (!ItemNameIsValid(codelistValue)) return;
                    codelistValue.versionNumber = 1;
                    codelistValue.versioningId = NewVersioningGroup(codelistValue);
                    SaveNewRegisterItem(codelistValue);
                }
            }
        }

        public void DeleteCoverageByDatasetId(Guid datasetSystemId)
        {
            var queryResult = from c in _dbContext.CoverageDatasets
                              where c.DatasetId == datasetSystemId
                              select c;

            var coverages = queryResult.ToList();
            foreach (var coverage in coverages)
            {
                DeleteCoverage(coverage);
            }
            _dbContext.SaveChanges();
        }

        public Guid GetOrganizationByName(string organizationName)
        {
            var queryResult = from c in _dbContext.Organizations
                              where c.name == organizationName
                              select c.systemId;

            return queryResult.Any() ? queryResult.FirstOrDefault() : Organization.GetDefaultOrganizationId();
        }

        public Models.Register GetInspireStatusRegisterItems(Models.Register register)
        {
            if (register.RegisterItems.Any())
            {
                var registerItems = new List<Models.RegisterItemV2>();
                foreach (var item in register.RegisterItems)
                {
                    if (item is InspireDataset inspireDataset)
                    {
                        registerItems.Add(inspireDataset);
                    }
                    else if (item is InspireDataService inspireDataService)
                    {
                        registerItems.Add(inspireDataService);
                    }
                }
                register.RegisterItems = registerItems;
            }
            return register;
        }

        public Dataset GetDatasetById(Guid id, Guid reigsterId)
        {
            var queryResult = from c in _dbContext.Datasets
                where (c.systemId == id
                      || c.Uuid == id.ToString()) && c.register.systemId == reigsterId
                              select c;

            return queryResult.FirstOrDefault();
        }

        public Organization GetOrganizationByFilterOrganizationParameter(string filterFilterOrganization)
        {
            var queryResult = from o in _dbContext.Organizations
                where (o.seoname == filterFilterOrganization
                       || o.name == filterFilterOrganization
                       || o.number == filterFilterOrganization)
                select o;

            return queryResult.FirstOrDefault();
        }
    }
}