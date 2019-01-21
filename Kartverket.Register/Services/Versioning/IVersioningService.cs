using Kartverket.Register.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kartverket.Register.Services.Versioning
{
    public interface IVersioningService
    {
        VersionsItem Versions(string registername, string parantRegister, string itemname);
        Guid NewVersioningGroup(Models.RegisterItem registerItem);
        void DeleteVersionGroup(Guid versioningId);
        List<Models.RegisterItem> GetVersionsByVersioningId(Guid versioningId);
        void UpdateCurrentVersionOfVersionGroup(Guid versioningId, Guid systemId);
        Document GetLatestSupersededVersion(Guid versioningId);
        Document SetLatestDocumentWithStatusIdDraftAsCurrent(Guid versioningId);
        Document SetLatestDocumentWithStatusIdSubmittedAsCurrent(Guid versioningId);
        Document SetLatestDocumentWithStatusIdRetiredAsCurrent(Guid versioningId);
    }
}
