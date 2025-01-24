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
using System.Text;
using System.Web.Configuration;
using System.Net;

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
                coverage.Coverage = null;
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

        public virtual Models.RegisterItem GetRegisterItemByPath(string path, string itemName, string vnr)
        {
            var registerItems = new List<Models.RegisterItem>();

            int vnrInt = 0;
            if (int.TryParse(vnr, out int result))
            {
                vnrInt = result;
            }

            var queryResults = from o in _dbContext.RegisterItems
                                where (o.register.path == path || o.register.pathOld == path) &&
                                o.seoname == itemName &&
                                (o.versionName == vnr || o.versionNumber == vnrInt)
                                select o;

            registerItems = queryResults.ToList();
           
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
            if (model is Kartverket.Register.Models.InspireDataService)
            {
                return ValidNameInspireDatasetService((Kartverket.Register.Models.InspireDataService)model);
            }
            if (model is GeodatalovDataset)
            {
                return GeodatalovDatasetNameAlreadyExist((GeodatalovDataset)model);
            }
            if (model is MareanoDataset)
            {
                return MareanoDatasetNameAlreadyExist((MareanoDataset)model);
            }
            if (model is FairDataset)
            {
                return FairDatasetNameAlreadyExist((FairDataset)model);
            }
            return false;
        }

        public void UpdateNameSpaceDatasets() 
        {
            var namespaces = _dbContext.NameSpases.ToList();
            foreach(var nameSpace in namespaces) 
            {
                nameSpace.NameSpaceDatasets = GetNameSpaceDatasets(nameSpace);
                _dbContext.Entry(nameSpace).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
        }

        public ICollection<NamespaceDataset> GetNameSpaceDatasets(NameSpace nameSpace)
        {
            var url = WebConfigurationManager.AppSettings["KartkatalogenUrl"] + "api/datasets-namespace?limit=2000&namespace=" + nameSpace.name;
            var client = new WebClient { Encoding = System.Text.Encoding.UTF8 };
            var json = client.DownloadString(url);
            dynamic datasets = Newtonsoft.Json.Linq.JObject.Parse(json);

            foreach (var dataset in datasets.Results)
            {
                string metadataUuid = dataset.Uuid.Value;

                var nameSpaceDataset = nameSpace.NameSpaceDatasets.Where(n => n.MetadataUuid == metadataUuid).FirstOrDefault();

                if (nameSpaceDataset != null)
                {
                    var title = dataset.Title;
                    var organization = dataset.Organization;
                    var datasetId = dataset.DatasetName;

                    nameSpaceDataset.MetadataNavn = title;
                    nameSpaceDataset.Organisasjon = organization;
                    nameSpaceDataset.DatasettId = datasetId;
                }
                else
                {
                    NamespaceDataset ds = new NamespaceDataset();
                    ds.MetadataUuid = metadataUuid;
                    ds.MetadataNavn = dataset.Title;
                    ds.Organisasjon = dataset.Organization;
                    ds.DatasettId = dataset.DatasetName;
                    ds.SystemId = Guid.NewGuid();
                    ds.NameSpaceId = nameSpace.systemId;
                    nameSpace.NameSpaceDatasets.Add(ds);
                }
            }

            return nameSpace.NameSpaceDatasets;
        }

        private bool GeodatalovDatasetNameAlreadyExist(GeodatalovDataset geodatalovDataset)
        {
            if (string.IsNullOrWhiteSpace(geodatalovDataset.Name))
            {
                return false;
            }
            if (geodatalovDataset == null) throw new ArgumentNullException(nameof(geodatalovDataset));
            var seoFriendlyName = RegisterUrls.MakeSeoFriendlyString(geodatalovDataset.Name);
            var registerId = geodatalovDataset.Register?.systemId ?? geodatalovDataset.RegisterId;
            var queryResults = from o in _dbContext.GeodatalovDatasets
                               where (o.Name == geodatalovDataset.Name || o.Seoname == seoFriendlyName) &&
                                     o.SystemId != geodatalovDataset.SystemId
                                     && o.RegisterId == registerId
                               select o.SystemId;

            return !queryResults.Any();
        }

        private bool MareanoDatasetNameAlreadyExist(MareanoDataset mareanoDataset)
        {
            if (string.IsNullOrWhiteSpace(mareanoDataset.Name))
            {
                return false;
            }
            if (mareanoDataset == null) throw new ArgumentNullException(nameof(mareanoDataset));
            var seoFriendlyName = RegisterUrls.MakeSeoFriendlyString(mareanoDataset.Name);
            var registerId = mareanoDataset.Register?.systemId ?? mareanoDataset.RegisterId;
            var queryResults = from o in _dbContext.MareanoDatasets
                               where (o.Name == mareanoDataset.Name || o.Seoname == seoFriendlyName) &&
                                     o.SystemId != mareanoDataset.SystemId
                                     && o.RegisterId == registerId
                               select o.SystemId;

            return !queryResults.Any();
        }

        private bool FairDatasetNameAlreadyExist(FairDataset fairDataset)
        {
            if (string.IsNullOrWhiteSpace(fairDataset.Name))
            {
                return false;
            }
            if (fairDataset == null) throw new ArgumentNullException(nameof(fairDataset));
            var seoFriendlyName = RegisterUrls.MakeSeoFriendlyString(fairDataset.Name);
            var registerId = fairDataset.Register?.systemId ?? fairDataset.RegisterId;
            var queryResults = from o in _dbContext.FairDatasets
                               where (o.Name == fairDataset.Name || o.Seoname == seoFriendlyName) &&
                                     o.SystemId != fairDataset.SystemId
                                     && o.RegisterId == registerId
                               select o.SystemId;

            return !queryResults.Any();
        }

        private bool ValidNameInspireDataset(InspireDatasetViewModel inspireDataset)
        {
            if (!string.IsNullOrWhiteSpace(inspireDataset.Name))
            {
                var seoFriendlyName = RegisterUrls.MakeSeoFriendlyString(inspireDataset.Name);
                var registerId = inspireDataset.Register?.systemId ?? inspireDataset.RegisterId;
                var queryResults = from o in _dbContext.InspireDatasets
                    where (o.Name == inspireDataset.Name || o.Seoname == seoFriendlyName) &&
                          o.SystemId != inspireDataset.SystemId
                          && o.RegisterId == registerId
                    select o.SystemId;

                return !queryResults.ToList().Any();
            }
            else
            {
                return false;
            }
        }

        private bool ValidNameInspireDatasetService(Kartverket.Register.Models.InspireDataService inspireDatasetService)
        {
            if (!string.IsNullOrWhiteSpace(inspireDatasetService.Name))
            {
                var seoFriendlyName = RegisterUrls.MakeSeoFriendlyString(inspireDatasetService.Name);
                var registerId = inspireDatasetService.Register?.systemId ?? inspireDatasetService.RegisterId;
                var queryResults = from o in _dbContext.InspireDataServices
                                   where (o.Name == inspireDatasetService.Name || o.Seoname == seoFriendlyName) &&
                                         o.SystemId != inspireDatasetService.SystemId
                                         && o.RegisterId == registerId
                                   select o.SystemId;

                return !queryResults.ToList().Any();
            }
            else
            {
                return false;
            }
        }

        private bool ValidNameInspireDataset(InspireDataset inspireDataset)
        {
            if (string.IsNullOrWhiteSpace(inspireDataset.Name))
            {
                return false;
            }
            if (inspireDataset == null) throw new ArgumentNullException(nameof(inspireDataset));
            var seoFriendlyName = RegisterUrls.MakeSeoFriendlyString(inspireDataset.Name);
            var registerId = inspireDataset.Register?.systemId ?? inspireDataset.RegisterId;
            var queryResults = from o in _dbContext.InspireDatasets
                               where (o.Name == inspireDataset.Name || o.Seoname == seoFriendlyName) &&
                                     o.SystemId != inspireDataset.SystemId
                                     && o.RegisterId == registerId
                               select o.SystemId;

            return !queryResults.ToList().Any();
        }

        private bool ValidateNameRegisterItem(object model)
        {
            if(model is CodelistValue) 
            {
                Models.CodelistValue registeritem = (Models.CodelistValue)model;

                if (!string.IsNullOrWhiteSpace(registeritem.value) || !string.IsNullOrWhiteSpace(registeritem.name))
                {
                    if (!string.IsNullOrWhiteSpace(registeritem.value) && (/*registeritem.value.Contains("/") || */registeritem.value.Contains("\\"))) 
                    {
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(registeritem.value) && (!string.IsNullOrWhiteSpace(registeritem.name) &&  (registeritem.name.Contains("/") || registeritem.name.Contains("\\"))))
                    {
                        return false;
                    }
                }

                    if (!string.IsNullOrWhiteSpace(registeritem.value))
                {
                    var registerId = registeritem.register?.systemId ?? registeritem.registerId;


                    var queryResults = from o in _dbContext.CodelistValues
                                       where (o.value == registeritem.value )
                                             && o.systemId != registeritem.systemId
                                             && o.registerId == registerId
                                       select o;

                    return !queryResults.ToList().Any();
                }
            }
            else 
            { 
            Models.RegisterItem registeritem = (Models.RegisterItem)model;
            if (!string.IsNullOrWhiteSpace(registeritem.name))
            {
                var seoFriendlyName = RegisterUrls.MakeSeoFriendlyString(registeritem.name);
                var registerId = registeritem.register?.systemId ?? registeritem.registerId;


                var queryResults = from o in _dbContext.RegisterItems
                    where (o.name == registeritem.name || o.seoname == seoFriendlyName) &&
                          o.systemId != registeritem.systemId
                          && o.registerId == registerId
                          && o.versioningId != registeritem.versioningId
                    select o;

                return !queryResults.ToList().Any();
            }
            }

            return false;
        }

        private bool ValidateNameDataset(object model)
        {
            Dataset dataset = (Dataset)model;
            var registerId = dataset.register?.systemId ?? dataset.registerId;
            if (dataset.register.IsDokMunicipal())
            {
                var seoFriendlyName = RegisterUrls.MakeSeoFriendlyString(dataset.name);
                var queryResultsDataset = from o in _dbContext.Datasets
                                          where (o.name == dataset.name || o.seoname == seoFriendlyName)
                                                && o.systemId != dataset.systemId
                                                && o.registerId == registerId
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
                                          where c.register.path == "sosi-kodelister/inndelinger/inndelingsbase/kommunenummer"
                                          && c.statusId == "Valid"
                                          && c.value != "2321"
                                          && c.value != "2311"
                                          && c.value != "2211"
                                          && c.value != "2121"
                                          && c.value != "2131"
                                          && c.value != "2111"
                                          && c.value != "2100"
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
                    //FAIR
                    case "findable_status":
                        {
                            var sortedList = registerItems.OfType<FairDatasetViewModel>().OrderBy(o => o.FindableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "findable_status_desc":
                        {
                            var sortedList = registerItems.OfType<FairDatasetViewModel>().OrderByDescending(o => o.FindableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "accesible_status":
                        {
                            var sortedList = registerItems.OfType<FairDatasetViewModel>().OrderBy(o => o.AccesibleStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "accesible_status_desc":
                        {
                            var sortedList = registerItems.OfType<FairDatasetViewModel>().OrderByDescending(o => o.AccesibleStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "interoperable_status":
                        {
                            var sortedList = registerItems.OfType<FairDatasetViewModel>().OrderBy(o => o.InteroperableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "interoperable_status_desc":
                        {
                            var sortedList = registerItems.OfType<FairDatasetViewModel>().OrderByDescending(o => o.InteroperableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "reusable_status":
                        {
                            var sortedList = registerItems.OfType<FairDatasetViewModel>().OrderBy(o => o.ReUseableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "reusable_status_desc":
                        {
                            var sortedList = registerItems.OfType<FairDatasetViewModel>().OrderByDescending(o => o.ReUseableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "percent_status":
                        {
                            var sortedList = registerItems.OfType<FairDatasetViewModel>().OrderBy(o => o.FAIRStatusPerCent).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "percent_status_desc":
                        {
                            var sortedList = registerItems.OfType<FairDatasetViewModel>().OrderByDescending(o => o.FAIRStatusPerCent).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    //FAIR mareano
                    case "findable_metadata_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.FindableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "findable_metadata_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.FindableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "accesible_metadata_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.AccesibleStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "accesible_metadata_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.AccesibleStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "interoperable_metadata_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.InteroperableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "interoperable_metadata_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.InteroperableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "reusable_metadata_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.ReUseableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "reusable_metadata_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.ReUseableStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    //Mareano
                    case "mareano_metadata_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.MetadataStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_metadata_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.MetadataStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_productspecification_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.ProductSpesificationStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_productspecification_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.ProductSpesificationStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_productsheet_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.ProductSheetStatus != null ? o.ProductSheetStatus.sortorder : 0).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_productsheet_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.ProductSheetStatus != null ? o.ProductSheetStatus.sortorder : 0).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_presentationrules_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.PresentationRulesStatus != null ? o.PresentationRulesStatus.sortorder : 0).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_presentationrules_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.PresentationRulesStatus != null ? o.PresentationRulesStatus.sortorder : 0).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_sosi_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.SosiDataStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_sosi_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.SosiDataStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_gml_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.GmlDataStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_gml_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.GmlDataStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_wms_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.WmsStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_wms_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.WmsStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_wfs_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.WfsStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_wfs_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.WfsStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_atom_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.AtomFeedStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_atom_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.AtomFeedStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_common_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.CommonStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_common_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.CommonStatus.sortorder).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_status":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderBy(o => o.Grade).ToList();
                            return sortedList.Cast<RegisterItemV2ViewModel>().ToList();
                        }
                    case "mareano_status_desc":
                        {
                            var sortedList = registerItems.OfType<MareanoDatasetViewModel>().OrderByDescending(o => o.Grade).ToList();
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
                    default:
                        {
                            return registerItems.OrderBy(o => o.Name).ToList();
                        }
                }
            }
            return registerItems;
        }

        public ICollection<Models.RegisterItem> OrderBy(ICollection<Models.RegisterItem> registerItems, string sorting)
        {
            if (registerItems.Any())
            {

                bool isCodeListValueRegister = registerItems.FirstOrDefault()?.register?.containedItemClass == "CodelistValue";

                bool orderByNumeric = false;
                bool orderByNameNumeric = false;

                if (isCodeListValueRegister)
                {
                    var notValidIntegers = registerItems.OfType<CodelistValue>().Select(str => {
                        int value;
                        bool success = int.TryParse(str.value, out value);
                        return new { value, success };
                     })
                      .Where(pair => !pair.success)
                      .Select(pair => pair.value);

                    if (notValidIntegers.Count() == 0)
                    {
                        orderByNumeric = true;
                    }

                    var diff = registerItems.OfType<CodelistValue>().Where(n => n.name != n.value).ToList();

                    if (diff.Count() == 0 && notValidIntegers.Count() == 0)
                    {
                        orderByNameNumeric = true;
                    }

                    if (string.IsNullOrEmpty(sorting)) 
                    { 
                        if (HttpContext.Current.Request.QueryString["sorting"] != null)
                            sorting = HttpContext.Current.Request.QueryString["sorting"].ToString();
                        
                        if (string.IsNullOrEmpty(sorting))
                            if(!orderByNameNumeric)
                                sorting = "name";
                            else if(orderByNumeric)
                                sorting = "codevalue";
                    }
                }


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

                if (orderByNameNumeric)
                    sortedList = registerItems.OrderBy(o => string.IsNullOrEmpty(o.NameTranslated()) ? int.Parse("0") : int.Parse(o.NameTranslated())).ToList();

                if (registerItems != null && registerItems.Count > 0 
                    && registerItems.First() is Alert && string.IsNullOrEmpty(sorting))
                    sorting = "alertdate_desc";

                // ***** RegisterItems

                if (sorting == "name_desc")
                {
                    if (orderByNameNumeric)
                        sortedList = registerItems.OrderByDescending(o => string.IsNullOrEmpty(o.NameTranslated()) ? int.Parse("0") : int.Parse(o.NameTranslated())).ToList();
                    else
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
                    if (orderByNumeric) 
                    {
                        var codevalue = registerItems.OfType<CodelistValue>().OrderBy(o => string.IsNullOrEmpty(o.value) ? int.Parse("0") : int.Parse(o.value));
                        sortedList = codevalue.Cast<Models.RegisterItem>().ToList();
                    }
                    else 
                    { 
                        var codevalue = registerItems.OfType<CodelistValue>().OrderBy(o => o.value);
                        sortedList = codevalue.Cast<Models.RegisterItem>().ToList();
                    }
                }
                else if (sorting == "codevalue_desc")
                {
                    if (orderByNumeric)
                    {
                        var codevalue = registerItems.OfType<CodelistValue>().OrderByDescending(o => string.IsNullOrEmpty(o.value) ? int.Parse("0") : int.Parse(o.value));
                        sortedList = codevalue.Cast<Models.RegisterItem>().ToList();
                    }
                    else
                    {
                        var codevalue = registerItems.OfType<CodelistValue>().OrderByDescending(o => o.value);
                        sortedList = codevalue.Cast<Models.RegisterItem>().ToList();
                    }
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

                // ***** Alert
                else if (sorting == "alertdate")
                {
                    var epsgSorting = registerItems.OfType<Alert>().OrderBy(o => o.AlertDate);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "alertdate_desc")
                {
                    var epsgSorting = registerItems.OfType<Alert>().OrderByDescending(o => o.AlertDate);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "effektivedate")
                {
                    var epsgSorting = registerItems.OfType<Alert>().OrderBy(o => o.EffectiveDate);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "effektivedate_desc")
                {
                    var epsgSorting = registerItems.OfType<Alert>().OrderByDescending(o => o.EffectiveDate);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "owner")
                {
                    var epsgSorting = registerItems.OfType<Alert>().OrderBy(o => o.Owner);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "owner_desc")
                {
                    var epsgSorting = registerItems.OfType<Alert>().OrderByDescending(o => o.Owner);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "servicetype")
                {
                    var epsgSorting = registerItems.OfType<Alert>().OrderBy(o => o.Type);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "servicetype_desc")
                {
                    var epsgSorting = registerItems.OfType<Alert>().OrderByDescending(o => o.Type);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "alert")
                {
                    var epsgSorting = registerItems.OfType<Alert>().OrderBy(o => o.AlertType);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }
                else if (sorting == "alert_desc")
                {
                    var epsgSorting = registerItems.OfType<Alert>().OrderByDescending(o => o.AlertType);
                    sortedList = epsgSorting.Cast<Models.RegisterItem>().ToList();
                }

                return sortedList;
            }
            return registerItems;
        }

        public void MakeAllRegisterItemsValid(Models.Register register, DateTime? itemsValidFrom = null, DateTime? itemsValidTo = null)
        {
            foreach (var item in register.items)
            {
                if (register.ContainedItemClassIsCodelistValue())
                {
                    var codelistvalue = item as CodelistValue;

                    codelistvalue.ValidFromDate = null;
                    codelistvalue.ValidToDate = null;

                    if (itemsValidFrom.HasValue)
                        codelistvalue.ValidFromDate = itemsValidFrom.Value;
                    if (itemsValidTo.HasValue)
                        codelistvalue.ValidToDate = itemsValidTo.Value;

                    codelistvalue.statusId = "Valid";
                    _dbContext.Entry(codelistvalue).State = EntityState.Modified;
                }
                else {
                    item.statusId = "Valid";
                    _dbContext.Entry(item).State = EntityState.Modified;
                }
            }
            _dbContext.SaveChanges();
        }

        public void MakeAllRegisterItemsRetired(Models.Register register)
        {
            foreach (var item in register.items)
            {
                item.statusId = "Retired";
                _dbContext.Entry(item).State = EntityState.Modified;
            }
            _dbContext.SaveChanges();
        }

        public void MakeAllRegisterItemsDraft(Models.Register register)
        {
            foreach (var item in register.items)
            {
                item.statusId = "Draft";
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
                    var codeListValueImport = StringExtensions.SplitQuoted(line, ';', '"');

                    var codelistValue = _codelistValueService.NewCodelistValueFromImport(register, codeListValueImport);
                    if (!ItemNameIsValid(codelistValue)) return;
                    codelistValue.versionNumber = 1;
                    codelistValue.versioningId = NewVersioningGroup(codelistValue);
                    SaveNewRegisterItem(codelistValue);
                }
            }
        }

        public void ImportRegisterItemHierarchyFromFile(Models.Register register, HttpPostedFileBase file, string codelistforhierarchy)
        {
            var csvreader = new StreamReader(file.InputStream);

            if (csvreader.EndOfStream) return;
            csvreader.ReadLine();
            if (register.ContainedItemClassIsCodelistValue())
            {
                while (!csvreader.EndOfStream)
                {
                    var line = csvreader.ReadLine();
                    var codeListValueImport = StringExtensions.SplitQuoted(line, ';', '"');

                    var broader = codeListValueImport[0];
                    var narrower = codeListValueImport[1];

                    var broaderItem = _dbContext.CodelistValues.Where(c => c.registerId == register.systemId && c.value == broader).FirstOrDefault();

                    if(broaderItem != null) 
                    {
                        var id = Guid.Parse(codelistforhierarchy);
                        var codelistValues = _dbContext.CodelistValues.Where(c => c.registerId == id && c.value == narrower).ToList();
                        if(codelistValues != null) 
                        {
                            if(codelistValues.Count == 1) 
                            {
                                var codelistValue = codelistValues.First();
                                _dbContext.Database.ExecuteSqlCommand("update RegisterItems set [broaderItemId] = '"+ broaderItem.systemId + "' ,[CodelistValue_systemId]='"+ broaderItem.systemId + "'  WHERE systemId = '"+ codelistValue.systemId + "'");
                            }
                        }
                    }
                    
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

        public Models.Register GetInspireStatusRegisterItems(Models.Register register, FilterParameters filter)
        {
            if (register.RegisterItems.Any())
            {
                var registerItems = new List<Models.RegisterItemV2>();
                foreach (var item in register.RegisterItems)
                {
                    if (Inspire.IncludeInFilter(item, filter))
                    {
                        registerItems.Add(item);
                    }
                }
                register.RegisterItems = registerItems;

                if (filter != null && filter.Offset > 0)
                    register.RegisterItems = register.RegisterItems.Skip(filter.Offset).ToList();
                if (filter != null && filter.Limit > 0)
                    register.RegisterItems = register.RegisterItems.Take(filter.Limit).ToList();

            }
            return register;
        }

        private bool ExcludeFilter(object inspire, FilterParameters filter)
        {
            if(filter != null && !string.IsNullOrEmpty(filter.filterOrganization))
            {
                var inspireData = inspire as InspireDataset;
                if (inspireData != null && inspireData.Owner.seoname.ToLower() == filter.filterOrganization.ToLower())
                    return false;

                var inspireDataService = inspire as InspireDataService;
                if (inspireDataService != null && inspireDataService.Owner.seoname.ToLower() == filter.filterOrganization.ToLower())
                    return false;

                return true;
            }

            return false;
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

        public List<Models.RegisterItem> GetAllVersionsOfRegisterItem(Models.Register register, string itemSystemId)
        {
            Models.RegisterItem registerItem = null;

            if (!string.IsNullOrEmpty(itemSystemId)) 
                registerItem = GetRegisterItemBySystemId(Guid.Parse(itemSystemId));

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
    }

    public static class StringExtensions
    {
        public static string[] SplitQuoted(this string input, char separator, char quotechar)
        {
            List<string> tokens = new List<string>();

            StringBuilder sb = new StringBuilder();
            bool escaped = false;
            foreach (char c in input)
            {
                if (c.Equals(separator) && !escaped)
                {
                    // we have a token
                    tokens.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else if (c.Equals(separator) && escaped)
                {
                    // ignore but add to string
                    sb.Append(c);
                }
                else if (c.Equals(quotechar))
                {
                    escaped = !escaped;
                    sb.Append(c);
                }
                else
                {
                    sb.Append(c);
                }
            }
            tokens.Add(sb.ToString().Trim());

            return tokens.ToArray();
        }
    }

}